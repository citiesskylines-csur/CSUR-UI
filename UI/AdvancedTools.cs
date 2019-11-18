using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Plugins;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using CSUR_UI.Util;
using ColossalFramework.Math;
using CSUR_UI.NewData;

namespace CSUR_UI.UI
{
    public class AdvancedTools : ToolBase
    {
        public static AdvancedTools instance;
        ushort m_hover;
        public static ushort m_step;
        Vector3 pos0;
        Vector3 pos1;
        Vector3 pos2;
        ushort node0;
        ushort node1;
        ushort node2;
        byte radius;
        byte rampMode;
        byte roadType;

        public static NetInfo m_netInfo;

        Color hcolor = new Color32(0, 181, 255, 255);
        Color scolor = new Color32(95, 166, 0, 244);
        Color m_errorColorInfo = new Color(1f, 0.25f, 0.1875f, 0.75f);
        Color m_validColorInfo = new Color(0f, 0f, 0f, 0.5f);

        public Assembly m_networkSkinsAssembly;

        public NetManager Manager
        {
            get { return Singleton<NetManager>.instance; }
        }
        public NetNode GetNode(ushort id)
        {
            return Manager.m_nodes.m_buffer[id];
        }
        public NetSegment GetSegment(ushort id)
        {
            return Manager.m_segments.m_buffer[id];
        }

        protected override void Awake()
        {
            base.Awake();
            radius = 30;
            rampMode = 0;
            roadType = 0;
            m_step = 0;
        }
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastInput input = new RaycastInput(ray, Camera.main.farClipPlane);
            input.m_ignoreTerrain = false;
            input.m_ignoreNodeFlags = NetNode.Flags.None;
            RayCast(input, out RaycastOutput output);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                enabled = false;
                ToolsModifierControl.SetTool<DefaultTool>();
                m_step = 0;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (m_step != 0)
                {
                    m_step--;
                }
            }

            if (m_step != 4)
            {
                switch (m_step)
                {
                    case 0: CustomShowToolInfo(true, "Please use ShotCut key to select ramp type", output.m_hitPos); break;
                    case 1: CustomShowToolInfo(true, "Please use ShotCut key to select road type", output.m_hitPos); break;
                    case 2:
                        determineHoveredElements(); pos0 = GetNode(m_hover).m_position; node0 = m_hover; CustomShowToolInfo(true, "Please select start node", output.m_hitPos); break;
                    case 3:
                        if (rampMode != 2)
                        {
                            pos1 = output.m_hitPos;
                            CustomShowToolInfo(true, "Please select round centre and adjust radius " + radius.ToString(), output.m_hitPos);
                        }
                        else
                        {
                            determineHoveredElements();
                            pos1 = GetNode(m_hover).m_position;
                            node1 = m_hover;
                            CustomShowToolInfo(true, "Please select post start node for start direction " + radius.ToString(), output.m_hitPos);
                        }
                        break;
                    default: break;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (m_step > 1 && m_step < 4)
                    {
                        m_step++;
                    }
                }
            }
            else
            {
                determineHoveredElements(); pos2 = GetNode(m_hover).m_position; node2 = m_hover; CustomShowToolInfo(true, "Please select end node. Now radius " + radius.ToString(), output.m_hitPos);
                if (Input.GetMouseButtonUp(0))
                {
                    if (m_step == 4)
                    {
                        if (rampMode == 0)
                        {
                            Build3RoundRoad(false, null);
                        }
                        else if (rampMode == 2)
                        {
                            Build1RoundRoad(false, null);
                        }
                        m_step = 0;
                        CustomShowToolInfo(show: false, null, Vector3.zero);
                        ToolsModifierControl.SetTool<DefaultTool>();
                        enabled = false;
                    }
                }
            }
        }

        protected override void OnToolGUI(Event e)
        {
            if (enabled == true)
            {
                if (OptionsKeymappingRoadTool.m_add.IsPressed(e)) radius = (byte)COMath.Clamp(radius + 1, 10, 250);
                if (OptionsKeymappingRoadTool.m_addPlus.IsPressed(e)) radius = (byte)COMath.Clamp(radius + 10, 10, 250);
                if (OptionsKeymappingRoadTool.m_minus.IsPressed(e)) radius = (byte)COMath.Clamp(radius - 1, 10, 250);
                if (OptionsKeymappingRoadTool.m_minusPlus.IsPressed(e)) radius = (byte)COMath.Clamp(radius - 10, 10, 250);
                if (OptionsKeymappingRoadTool.m_elevated.IsPressed(e))
                {
                    if (m_step == 1) m_step++;
                    roadType = 1;
                }
                if (OptionsKeymappingRoadTool.m_ground.IsPressed(e))
                {
                    if (m_step == 1) m_step++;
                    roadType = 0;
                }
                if (OptionsKeymappingRoadTool.m_leftRamp3Round.IsPressed(e))
                {
                    if (m_step == 0) m_step++;
                    rampMode = 0;
                }
                if (OptionsKeymappingRoadTool.m_rightRamp3Round.IsPressed(e))
                {
                    if (m_step == 0) m_step++;
                    rampMode = 1;
                }
                if (OptionsKeymappingRoadTool.m_leftRamp1Round.IsPressed(e))
                {
                    if (m_step == 0) m_step++;
                    rampMode = 2;
                }
                if (OptionsKeymappingRoadTool.m_build.IsPressed(e))
                {
                    if (m_step == 4)
                    {
                        if (rampMode == 0)
                        {
                            Build3RoundRoad(false, null);
                        }
                        else if (rampMode == 2)
                        {
                            Build1RoundRoad(false, null);
                        }
                        m_step = 0;
                        CustomShowToolInfo(show: false, null, Vector3.zero);
                        ToolsModifierControl.SetTool<DefaultTool>();
                        enabled = false;
                    }
                }
            }
        }

        private bool determineHoveredElements()
        {
            bool flag = !UIView.IsInsideUI() && Cursor.visible;
            if (flag)
            {
                m_hover = 0;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastInput input = new RaycastInput(ray, Camera.main.farClipPlane);
                input.m_netService.m_itemLayers = (ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                input.m_netService.m_service = ItemClass.Service.Road;
                input.m_ignoreTerrain = true;
                input.m_ignoreNodeFlags = NetNode.Flags.None;
                if (ToolBase.RayCast(input, out RaycastOutput output))
                {
                    m_hover = output.m_netNode;
                }
                else
                {
                    input.m_netService.m_itemLayers = (ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                    input.m_netService.m_service = ItemClass.Service.PublicTransport;
                    input.m_netService.m_subService = ItemClass.SubService.PublicTransportTrain;
                    input.m_ignoreTerrain = true;
                    input.m_ignoreNodeFlags = NetNode.Flags.None;
                    if (ToolBase.RayCast(input, out output))
                    {
                        m_hover = output.m_netNode;
                    }
                    else
                    {
                        input.m_netService.m_itemLayers = (ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                        input.m_netService.m_service = ItemClass.Service.PublicTransport;
                        input.m_netService.m_subService = ItemClass.SubService.PublicTransportMetro;
                        input.m_ignoreTerrain = true;
                        input.m_ignoreNodeFlags = NetNode.Flags.None;
                        if (ToolBase.RayCast(input, out output))
                        {
                            m_hover = output.m_netNode;
                        }
                    }
                }
                ushort HoveredSegmentId = 0;
                RaycastInput input2 = new RaycastInput(ray, Camera.main.farClipPlane);
                input2.m_netService.m_itemLayers = (ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                input2.m_netService.m_service = ItemClass.Service.Road;
                input2.m_ignoreTerrain = true;
                input2.m_ignoreSegmentFlags = NetSegment.Flags.None;
                if (ToolBase.RayCast(input2, out RaycastOutput output2))
                {
                    HoveredSegmentId = output2.m_netSegment;
                }
                else
                {
                    input2.m_netService.m_itemLayers = (ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                    input2.m_netService.m_service = ItemClass.Service.PublicTransport;
                    input2.m_netService.m_subService = ItemClass.SubService.PublicTransportTrain;
                    input2.m_ignoreTerrain = true;
                    input2.m_ignoreSegmentFlags = NetSegment.Flags.None;
                    if (ToolBase.RayCast(input2, out output2))
                    {
                        HoveredSegmentId = output2.m_netSegment;
                    }
                    else
                    {
                        input2.m_netService.m_itemLayers = (ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                        input2.m_netService.m_service = ItemClass.Service.PublicTransport;
                        input2.m_netService.m_subService = ItemClass.SubService.PublicTransportMetro;
                        input2.m_ignoreTerrain = true;
                        input2.m_ignoreSegmentFlags = NetSegment.Flags.None;
                        if (ToolBase.RayCast(input2, out output2))
                        {
                            HoveredSegmentId = output2.m_netSegment;
                        }
                    }
                }
                if (m_hover <= 0 && HoveredSegmentId > 0)
                {
                    ushort startNode = Singleton<NetManager>.instance.m_segments.m_buffer[HoveredSegmentId].m_startNode;
                    ushort endNode = Singleton<NetManager>.instance.m_segments.m_buffer[HoveredSegmentId].m_endNode;
                    float magnitude = (output2.m_hitPos - Singleton<NetManager>.instance.m_nodes.m_buffer[startNode].m_position).magnitude;
                    float magnitude2 = (output2.m_hitPos - Singleton<NetManager>.instance.m_nodes.m_buffer[endNode].m_position).magnitude;
                    if (magnitude < magnitude2 && magnitude < 75f)
                    {
                        m_hover = startNode;
                    }
                    else if (magnitude2 < magnitude && magnitude2 < 75f)
                    {
                        m_hover = endNode;
                    }
                }
                if (m_hover == 0)
                {
                    return HoveredSegmentId != 0;
                }
                return true;
            }
            return flag;
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (enabled == true)
            {
                if (m_hover != 0 && (m_step != 4))
                {
                    ushort netNode = m_hover;
                    var Instance = Singleton<NetManager>.instance;
                    NetInfo info = Instance.m_nodes.m_buffer[netNode].Info;
                    Vector3 position = Instance.m_nodes.m_buffer[netNode].m_position;
                    float alpha = 1f;
                    var toolColor = new Color32(95, 166, 0, 244);
                    NetTool.CheckOverlayAlpha(info, ref alpha);
                    toolColor.a = (byte)(toolColor.a * alpha);
                    Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, position, Mathf.Max(6f, info.m_halfWidth * 2f), -1f, 1280f, renderLimits: false, alphaBlend: true);
                }
                if (m_step == 4)
                {
                    if (rampMode == 0)
                    {
                        Build3RoundRoad(true, cameraInfo);
                    }
                    else if (rampMode == 2)
                    {
                        Build1RoundRoad(true, cameraInfo);
                    }
                    //Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, debugA1pos, Mathf.Max(6f, 16f), -1f, 1280f, renderLimits: false, alphaBlend: true);
                    //Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, debugA2pos, Mathf.Max(6f, 16f), -1f, 1280f, renderLimits: false, alphaBlend: true);
                    //Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, debugB1pos, Mathf.Max(6f, 16f), -1f, 1280f, renderLimits: false, alphaBlend: true);
                    //Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, debugB2pos, Mathf.Max(6f, 16f), -1f, 1280f, renderLimits: false, alphaBlend: true);
                }
            }
        }

        private void AdjustElevation(ushort startNode, float elevation)
        {
            var nm = Singleton<NetManager>.instance;
            var node = nm.m_nodes.m_buffer[startNode];
            var ele = (byte)Mathf.Clamp(Mathf.RoundToInt(Math.Max(node.m_elevation, elevation)), 0, 255);
            var terrain = Singleton<TerrainManager>.instance.SampleRawHeightSmoothWithWater(node.m_position, false, 0f);
            node.m_elevation = ele;
            node.m_position = new Vector3(node.m_position.x, ele + terrain, node.m_position.z);
            if (elevation < 1f)
            {
                node.m_flags |= NetNode.Flags.OnGround;
            }
            else
            {
                node.m_flags &= ~NetNode.Flags.OnGround;
                UpdateSegment(node.m_segment0, elevation);
                UpdateSegment(node.m_segment1, elevation);
                UpdateSegment(node.m_segment2, elevation);
                UpdateSegment(node.m_segment3, elevation);
                UpdateSegment(node.m_segment4, elevation);
                UpdateSegment(node.m_segment5, elevation);
                UpdateSegment(node.m_segment6, elevation);
                UpdateSegment(node.m_segment7, elevation);
            }
            nm.m_nodes.m_buffer[startNode] = node;
            //Singleton<NetManager>.instance.UpdateNode(startNode);
        }

        private void UpdateSegment(ushort segmentId, float elevation)
        {
            if (segmentId == 0)
            {
                return;
            }
            var netManager = Singleton<NetManager>.instance;
            if (elevation > 4)
            {
                var errors = default(ToolBase.ToolErrors);
                netManager.m_segments.m_buffer[segmentId].Info =
                    netManager.m_segments.m_buffer[segmentId].Info.m_netAI.GetInfo(elevation, elevation, 5, false, false, false, false, ref errors);
            }
        }

        private void CreateNode(out ushort startNode, ref Randomizer rand, NetInfo netInfo, Vector3 oldPos)
        {
            var pos = new Vector3(oldPos.x, 0, oldPos.z);
            pos.y = Singleton<TerrainManager>.instance.SampleRawHeightSmoothWithWater(pos, false, 0f);
            var nm = Singleton<NetManager>.instance;
            nm.CreateNode(out startNode, ref rand, netInfo, pos,
                Singleton<SimulationManager>.instance.m_currentBuildIndex);
            Singleton<SimulationManager>.instance.m_currentBuildIndex += 1u;
        }

        public Vector3 GetNodeDir(ushort node)
        {
            var nm = Singleton<NetManager>.instance;
            var m_node = nm.m_nodes.m_buffer[node];
            if (m_node.m_segment0 !=0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment0].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment0].m_startDirection;
                }
                else if(Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment0].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment0].m_endDirection;
                }
            }

            if (m_node.m_segment1 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment1].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment1].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment1].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment1].m_endDirection;
                }
            }

            if (m_node.m_segment2 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment2].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment2].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment2].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment2].m_endDirection;
                }
            }

            if (m_node.m_segment3 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment3].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment3].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment3].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment3].m_endDirection;
                }
            }

            if (m_node.m_segment4 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment4].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment4].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment4].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment4].m_endDirection;
                }
            }

            if (m_node.m_segment5 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment5].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment5].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment5].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment5].m_endDirection;
                }
            }

            if (m_node.m_segment6 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment6].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment6].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment6].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment6].m_endDirection;
                }
            }

            if (m_node.m_segment7 != 0)
            {
                if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment7].m_startNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment7].m_startDirection;
                }
                else if (Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment7].m_endNode == node)
                {
                    return Singleton<NetManager>.instance.m_segments.m_buffer[m_node.m_segment7].m_endDirection;
                }
            }

            return Vector3.zero;
        }

        public bool isRightTurn(Bezier3 part, Vector3 pos)
        {
            for (int i= 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                if (Vector2.Distance(VectorUtils.XZ(pos), VectorUtils.XZ(part.Position(p))) < 1f)
                {
                    return true;
                }
            }
            return false;
        }

        public void Build3RoundRoad(bool onlyShow, RenderManager.CameraInfo cameraInfo)
        {
            Bezier3 partA = default(Bezier3);
            Bezier3 partB = default(Bezier3);
            Bezier3 partC = default(Bezier3);
            Bezier3 partD = default(Bezier3);
            Bezier3 partE = default(Bezier3);
            var m_elevation = (roadType == 0) ? 0f : 8f;
            GetRound(pos1, radius * 8f, ref partA, ref partB, ref partC, ref partD);
            FindNodeA(true, pos0, GetNodeDir(node0), partA, partB, partC, partD, out Vector3 NodeA1, out Vector3 NodeA1Dir, out Vector3 startDir);
            FindNodeB(pos0, startDir, partA, partB, partC, partD, out Vector3 NodeB1, out Vector3 NodeB1Dir);
            FindNodeA(false, pos2, GetNodeDir(node2), partA, partB, partC, partD, out Vector3 NodeA2, out Vector3 NodeA2Dir,out Vector3 endDir);
            endDir = -endDir;
            FindNodeB(pos2, endDir, partA, partB, partC, partD, out Vector3 NodeB2, out Vector3 NodeB2Dir);
            bool rightTurn = false;

            partA.a = pos0;
            partA.d = NodeA1;
            CustomNetSegment.CalculateMiddlePoints(pos0, startDir, NodeA1, -VectorUtils.NormalizeXZ(NodeA1Dir), true, true, out partA.b, out partA.c);
            partB.a = NodeA1;
            partB.d = NodeB1;
            CustomNetSegment.CalculateMiddlePoints(NodeA1, VectorUtils.NormalizeXZ(NodeA1Dir), NodeB1, -VectorUtils.NormalizeXZ(NodeB1Dir), true, true, out partB.b, out partB.c);
            rightTurn = isRightTurn(partB, NodeA1);
            if (!rightTurn)
            {
                partC.a = NodeB1;
                partC.d = NodeB2;
                CustomNetSegment.CalculateMiddlePoints(NodeB1, VectorUtils.NormalizeXZ(NodeB1Dir), NodeB2, -VectorUtils.NormalizeXZ(NodeB2Dir), true, true, out partC.b, out partC.c);
                partD.a = NodeB2;
                partD.d = NodeA2;
                CustomNetSegment.CalculateMiddlePoints(NodeB2, VectorUtils.NormalizeXZ(NodeB2Dir), NodeA2, -VectorUtils.NormalizeXZ(NodeA2Dir), true, true, out partD.b, out partD.c);
                partE.a = NodeA2;
                partE.d = pos2;
                CustomNetSegment.CalculateMiddlePoints(NodeA2, VectorUtils.NormalizeXZ(NodeA2Dir), pos2, -endDir, true, true, out partE.b, out partE.c);
            } 
            else
            {
                partB.a = NodeA1;
                partB.d = NodeA2;
                CustomNetSegment.CalculateMiddlePoints(NodeA1, VectorUtils.NormalizeXZ(NodeA1Dir), NodeA2, -VectorUtils.NormalizeXZ(NodeA2Dir), true, true, out partB.b, out partB.c);
                partE.a = NodeA2;
                partE.d = pos2;
                CustomNetSegment.CalculateMiddlePoints(NodeA2, VectorUtils.NormalizeXZ(NodeA2Dir), pos2, -endDir, true, true, out partE.b, out partE.c);
            }

            if (!onlyShow)
            {
                if (NodeA1 == Vector3.zero) DebugLog.LogToFileOnly("NodeA1 not found");
                if (NodeB1 == Vector3.zero) DebugLog.LogToFileOnly("NodeB1 not found");
                if (NodeB2 == Vector3.zero) DebugLog.LogToFileOnly("NodeB2 not found");
                if (NodeA2 == Vector3.zero) DebugLog.LogToFileOnly("NodeA2 not found");
                DebugLog.LogToFileOnly(pos0.ToString());
                DebugLog.LogToFileOnly(pos1.ToString());
                DebugLog.LogToFileOnly(pos2.ToString());

                DebugLog.LogToFileOnly(NodeA1.ToString());
                DebugLog.LogToFileOnly(NodeB1.ToString());
                DebugLog.LogToFileOnly(NodeB2.ToString());
                DebugLog.LogToFileOnly(NodeA2.ToString());

                DebugLog.LogToFileOnly(NodeA1Dir.ToString());
                DebugLog.LogToFileOnly(NodeB1Dir.ToString());
                DebugLog.LogToFileOnly(NodeB2Dir.ToString());
                DebugLog.LogToFileOnly(NodeA2Dir.ToString());


                var rand = new Randomizer(0u);
                //var m_currentModule = Parser.ModuleNameFromUI(MainUI.fromSelected, MainUI.toSelected, MainUI.symmetry, MainUI.uturnLane, MainUI.hasSidewalk, MainUI.hasBike);
                //DebugLog.LogToFileOnly(m_netInfo.name);
                var m_prefab = m_netInfo;
                ToolErrors errors = default(ToolErrors);
                var netInfo = m_prefab.m_netAI.GetInfo(m_elevation, m_elevation, 5, false, false, false, false, ref errors);

                int m_nodeNum = 0;
                int partANum = (int)(Vector2.Distance(VectorUtils.XZ(pos0), VectorUtils.XZ(NodeA1)) / 64f);
                int partBNum = (int)(Vector2.Distance(VectorUtils.XZ(NodeA1), VectorUtils.XZ(NodeB1)) / 64f);
                int partCNum = (int)(Vector2.Distance(VectorUtils.XZ(NodeB1), VectorUtils.XZ(NodeB2)) / 64f);
                int partDNum = (int)(Vector2.Distance(VectorUtils.XZ(NodeB2), VectorUtils.XZ(NodeA2)) / 64f);
                int partENum = (int)(Vector2.Distance(VectorUtils.XZ(NodeA2), VectorUtils.XZ(pos2)) / 64f);

                m_nodeNum += partANum + 1;
                m_nodeNum += partBNum + 1;
                if (!rightTurn)
                {
                    m_nodeNum += partCNum + 1;
                    m_nodeNum += partDNum + 1;
                }
                m_nodeNum += partENum;
                ushort[] node = new ushort[m_nodeNum];
                ushort[] segment = new ushort[m_nodeNum];

                for (int i = 0; i <= partANum; i++)
                {
                    float p1 = (float)(i + 1) / (float)(partANum + 1);
                    float p2 = (float)(i) / (float)(partANum +1);
                    CreateNode(out node[i], ref rand, netInfo, partA.Position(p1));
                    if (i == 0)
                    {
                        if (Singleton<NetManager>.instance.CreateSegment(out segment[i], ref rand, netInfo, node0, node[i], startDir, -VectorUtils.NormalizeXZ(partA.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                            Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                    }
                    else
                    {
                        if (Singleton<NetManager>.instance.CreateSegment(out segment[i], ref rand, netInfo, node[i - 1], node[i], VectorUtils.NormalizeXZ(partA.Tangent(p2)), -VectorUtils.NormalizeXZ(partA.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                            Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                    }
                }

                for (int i = 0; i <= partBNum; i++)
                {
                    float p1 = (float)(i+1) / (float)(partBNum+1);
                    float p2 = (float)i / (float)(partBNum+1);
                    CreateNode(out node[i + partANum + 1], ref rand, netInfo, partB.Position(p1));
                    if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + 1], ref rand, netInfo, node[i + partANum], node[i + partANum + 1], VectorUtils.NormalizeXZ(partB.Tangent(p2)), -VectorUtils.NormalizeXZ(partB.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                        Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                }

                if (!rightTurn)
                {
                    for (int i = 0; i <= partCNum; i++)
                    {
                        float p1 = (float)(i + 1) / (float)(partCNum + 1);
                        float p2 = (float)(i) / (float)(partCNum + 1);
                        CreateNode(out node[i + partANum + partBNum + 2], ref rand, netInfo, partC.Position(p1));
                        if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + 2], ref rand, netInfo, node[i + partANum + partBNum + 1], node[i + partANum + partBNum + 2], VectorUtils.NormalizeXZ(partC.Tangent(p2)), -VectorUtils.NormalizeXZ(partC.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                            Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                    }


                    for (int i = 0; i <= partDNum; i++)
                    {
                        float p1 = (float)(i + 1) / (float)(partDNum + 1);
                        float p2 = (float)(i) / (float)(partDNum + 1);
                        CreateNode(out node[i + partANum + partBNum + partCNum + 3], ref rand, netInfo, partD.Position(p1));
                        if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + partCNum + 3], ref rand, netInfo, node[i + partANum + partBNum + partCNum + 2], node[i + partANum + partBNum + partCNum + 3], VectorUtils.NormalizeXZ(partD.Tangent(p2)), -VectorUtils.NormalizeXZ(partD.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                            Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                    }

                    if (partENum > 0)
                    {
                        for (int i = 1; i <= partENum; i++)
                        {
                            float p1 = (float)i / (float)(partENum + 1);
                            float p2 = (float)(i - 1) / (float)(partENum + 1);
                            CreateNode(out node[i + partANum + partBNum + partCNum + partDNum + 3], ref rand, netInfo, partE.Position(p1));
                            if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + partCNum + partDNum + 3], ref rand, netInfo, node[i + partANum + partBNum + partCNum + partDNum + 2], node[i + partANum + partBNum + partCNum + partDNum + 3], VectorUtils.NormalizeXZ(partE.Tangent(p2)), -VectorUtils.NormalizeXZ(partE.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                                Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                        }
                    }

                    ushort segmentId;
                    float tmp = (float)partENum / (float)(partENum + 1);
                    if (Singleton<NetManager>.instance.CreateSegment(out segmentId, ref rand, netInfo, node[partANum + partBNum + partCNum + partDNum + partENum + 3], node2, VectorUtils.NormalizeXZ(partE.Tangent(tmp)), -endDir, Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                        Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                } 
                else
                {
                    if (partENum > 0)
                    {
                        for (int i = 1; i <= partENum; i++)
                        {
                            float p1 = (float)i / (float)(partENum + 1);
                            float p2 = (float)(i - 1) / (float)(partENum + 1);
                            CreateNode(out node[i + partANum + partBNum + 1], ref rand, netInfo, partE.Position(p1));
                            if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + 1], ref rand, netInfo, node[i + partANum + partBNum], node[i + partANum + partBNum + 1], VectorUtils.NormalizeXZ(partE.Tangent(p2)), -VectorUtils.NormalizeXZ(partE.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                                Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                        }
                    }

                    ushort segmentId;
                    float tmp = (float)partENum / (float)(partENum + 1);
                    if (Singleton<NetManager>.instance.CreateSegment(out segmentId, ref rand, netInfo, node[partANum + partBNum + partENum + 1], node2, VectorUtils.NormalizeXZ(partE.Tangent(tmp)), -endDir, Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                        Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                }
            }
            else
            {
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partA, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partB, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                if (!rightTurn)
                {
                    Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partC, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                    Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partD, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                }
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partE, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
            }
        }

        public void GetRound(Vector3 centerPos, float radius, ref Bezier3 partA, ref Bezier3 partB, ref Bezier3 partC, ref Bezier3 partD)
        {
            Vector3 controlP1 = centerPos + new Vector3(radius, 0, 0);
            Vector3 direction1 = VectorUtils.NormalizeXZ(new Vector3(0, 0, radius));
            Vector3 controlP2 = centerPos + new Vector3(0, 0, radius);
            Vector3 direction2 = VectorUtils.NormalizeXZ(new Vector3(-radius, 0, 0));
            Vector3 controlP3 = centerPos + new Vector3(-radius, 0, 0);
            Vector3 direction3 = VectorUtils.NormalizeXZ(new Vector3(0, 0, -radius));
            Vector3 controlP4 = centerPos + new Vector3(0, 0, -radius);
            Vector3 direction4 = VectorUtils.NormalizeXZ(new Vector3(radius, 0, 0));
            partA.a = controlP1;
            partA.d = controlP2;
            CustomNetSegment.CalculateMiddlePoints(controlP1, direction1, controlP2, -direction2, true, true, out partA.b, out partA.c);
            partB.a = controlP2;
            partB.d = controlP3;
            CustomNetSegment.CalculateMiddlePoints(controlP2, direction2, controlP3, -direction3, true, true, out partB.b, out partB.c);
            partC.a = controlP3;
            partC.d = controlP4;
            CustomNetSegment.CalculateMiddlePoints(controlP3, direction3, controlP4, -direction4, true, true, out partC.b, out partC.c);
            partD.a = controlP4;
            partD.d = controlP1;
            CustomNetSegment.CalculateMiddlePoints(controlP4, direction4, controlP1, -direction1, true, true, out partD.b, out partD.c);
        }

        public string isRound(Vector3 startPos, Vector3 startDir, Vector3 endPos, Vector3 endDir, out float diff)
        {
            diff = 100f;
            float num = startDir.x * endDir.x + startDir.z * endDir.z;
            if (num >= -0.999f && Line2.Intersect(VectorUtils.XZ(startPos), VectorUtils.XZ(startPos + startDir), VectorUtils.XZ(endPos), VectorUtils.XZ(endPos + endDir), out float u, out float v))
            {
                if(u > 0 && v > 0 && (u < radius*10) && (v < radius * 10))
                {
                    if (u == v)
                    {
                        return "Yes";
                    }
                    else if (((u / v) <= 1.1f) && ((u / v) >= 0.9f) && (Math.Abs(u - v) <= 5f))
                    {
                        diff = Math.Abs(50*u / v - 50) + Math.Abs(u - v);
                        return "Maybe";
                    }
                }
            }

            /*diff = 100f;
            float a = (startDir.x - endDir.x) * (startPos.z - endPos.z);
            float b = (startDir.z - endDir.z) * (startPos.x - endPos.x);
            if (a == b && !(((startDir.x - endDir.x) == 0) && (startDir.z - endDir.z) == 0))
            {
                return "Yes";
            }
            else if ((a > 0 && b > 0) || (a < 0 && b < 0))
            {
                if (b != 0)
                {
                    if (((a / b) <= 1.1f) && ((a / b) >= 0.9f) && (Math.Abs(a - b) < 5f))
                    {
                        diff = Math.Abs(a / b - 1) + Math.Abs(a - b);
                        return "Maybe";
                    }
                }
            }*/
            return "No";
        }
        public void FindNodeA(bool isStart, Vector3 startPos, Vector3 startDir, Bezier3 partA, Bezier3 partB, Bezier3 partC, Bezier3 partD, out Vector3 NodeA, out Vector3 NodeADir, out Vector3 startDirFix)
        {
            NodeA = Vector3.zero;
            NodeADir = Vector3.zero;
            startDirFix = startDir;
            var tmpNodeA = Vector3.zero;
            var tmpNodeADir = Vector3.zero;
            var tmpDiff = 100f;
            var tmpDiff1 = 100f;
            //partA
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partA.Tangent(p)) : VectorUtils.NormalizeXZ(partA.Tangent(p));
                string result = isRound(startPos, startDir, partA.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partA.Position(p);
                    NodeADir = partA.Tangent(p);
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff)
                    {
                        tmpNodeA = partA.Position(p);
                        tmpNodeADir = partA.Tangent(p);
                        tmpDiff = diff;
                    }
                }
            }
            //partB
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partB.Tangent(p)) : VectorUtils.NormalizeXZ(partB.Tangent(p));
                string result = isRound(startPos, startDir, partB.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partB.Position(p);
                    NodeADir = partB.Tangent(p);
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff)
                    {
                        tmpNodeA = partB.Position(p);
                        tmpNodeADir = partB.Tangent(p);
                        tmpDiff = diff;
                    }
                }
            }
            //partC
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partC.Tangent(p)) : VectorUtils.NormalizeXZ(partC.Tangent(p));
                string result = isRound(startPos, startDir, partC.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partC.Position(p);
                    NodeADir = partC.Tangent(p);
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff)
                    {
                        tmpNodeA = partC.Position(p);
                        tmpNodeADir = partC.Tangent(p);
                        tmpDiff = diff;
                    }
                }
            }
            //partD
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partD.Tangent(p)) : VectorUtils.NormalizeXZ(partD.Tangent(p));
                string result = isRound(startPos, startDir, partD.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partD.Position(p);
                    NodeADir = partD.Tangent(p);
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff)
                    {
                        tmpNodeA = partD.Position(p);
                        tmpNodeADir = partD.Tangent(p);
                        tmpDiff = diff;
                    }
                }
            }

            if (tmpDiff != 100f)
            {
                NodeA = tmpNodeA;
                NodeADir = tmpNodeADir;
                tmpDiff1 = tmpDiff;
            }

            startDir = -startDir;
            //partA
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partA.Tangent(p)) : VectorUtils.NormalizeXZ(partA.Tangent(p));
                string result = isRound(startPos, startDir, partA.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partA.Position(p);
                    NodeADir = partA.Tangent(p);
                    startDirFix = startDir;
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff1)
                    {
                        tmpNodeA = partA.Position(p);
                        tmpNodeADir = partA.Tangent(p);
                        tmpDiff1 = diff;
                    }
                }
            }
            //partB
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partB.Tangent(p)) : VectorUtils.NormalizeXZ(partB.Tangent(p));
                string result = isRound(startPos, startDir, partB.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partB.Position(p);
                    NodeADir = partB.Tangent(p);
                    startDirFix = startDir;
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff1)
                    {
                        tmpNodeA = partB.Position(p);
                        tmpNodeADir = partB.Tangent(p);
                        tmpDiff1 = diff;
                    }
                }
            }
            //partC
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partC.Tangent(p)) : VectorUtils.NormalizeXZ(partC.Tangent(p));
                string result = isRound(startPos, startDir, partC.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partC.Position(p);
                    NodeADir = partC.Tangent(p);
                    startDirFix = startDir;
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff1)
                    {
                        tmpNodeA = partC.Position(p);
                        tmpNodeADir = partC.Tangent(p);
                        tmpDiff1 = diff;
                    }
                }
            }
            //partD
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart ? -VectorUtils.NormalizeXZ(partD.Tangent(p)) : VectorUtils.NormalizeXZ(partD.Tangent(p));
                string result = isRound(startPos, startDir, partD.Position(p), dir, out float diff);
                if (result == "Yes")
                {
                    NodeA = partD.Position(p);
                    NodeADir = partD.Tangent(p);
                    startDirFix = startDir;
                    return;
                }
                else if (result == "Maybe")
                {
                    if (diff < tmpDiff1)
                    {
                        tmpNodeA = partD.Position(p);
                        tmpNodeADir = partD.Tangent(p);
                        tmpDiff1 = diff;
                    }
                }
            }

            if (tmpDiff != tmpDiff1)
            {
                NodeA = tmpNodeA;
                NodeADir = tmpNodeADir;
                startDirFix = startDir;
            }
        }
        public void FindNodeB(Vector3 startPos, Vector3 startDir, Bezier3 partA, Bezier3 partB, Bezier3 partC, Bezier3 partD, out Vector3 NodeB, out Vector3 NodeBDir)
        {
            NodeB = Vector3.zero;
            NodeBDir = Vector3.zero;
            startDir.y = 0;
            var tmpDistance = 100f;
            var tmpNodeB = Vector3.zero;
            var tmpNodeBDir = Vector3.zero;
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = VectorUtils.NormalizeXZ(partA.Tangent(p));
                dir.y = 0;
                var distance = Vector3.Distance(startDir, dir);
                if (distance < 0.1f)
                {
                    if (distance < tmpDistance)
                    {
                        tmpNodeB = partA.Position(p);
                        tmpNodeBDir = partA.Tangent(p);
                        tmpDistance = distance;
                    }
                }
            }

            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = VectorUtils.NormalizeXZ(partB.Tangent(p));
                dir.y = 0;
                var distance = Vector3.Distance(startDir, dir);
                if (distance < 0.1f)
                {
                    if (distance < tmpDistance)
                    {
                        tmpNodeB = partB.Position(p);
                        tmpNodeBDir = partB.Tangent(p);
                        tmpDistance = distance;
                    }
                }
            }

            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = VectorUtils.NormalizeXZ(partC.Tangent(p));
                dir.y = 0;
                var distance = Vector3.Distance(startDir, dir);
                if (distance < 0.1f)
                {
                    if (distance < tmpDistance)
                    {
                        tmpNodeB = partC.Position(p);
                        tmpNodeBDir = partC.Tangent(p);
                        tmpDistance = distance;
                    }
                }
            }

            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = VectorUtils.NormalizeXZ(partD.Tangent(p));
                dir.y = 0;
                var distance = Vector3.Distance(startDir, dir);
                if (distance < 0.1f)
                {
                    if (distance < tmpDistance)
                    {
                        tmpNodeB = partD.Position(p);
                        tmpNodeBDir = partD.Tangent(p);
                        tmpDistance = distance;
                    }
                }
            }

            if (tmpDistance != 100f)
            {
                NodeB = tmpNodeB;
                NodeBDir = tmpNodeBDir;
            }
        }

        protected void CustomShowToolInfo(bool show, string text, Vector3 worldPos)
        {
            if (cursorInfoLabel == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(text) && show)
            {
                cursorInfoLabel.isVisible = true;
                UIView uIView = cursorInfoLabel.GetUIView();
                Vector2 vector = (!(fullscreenContainer != null)) ? uIView.GetScreenResolution() : fullscreenContainer.size;
                Vector3 v = Camera.main.WorldToScreenPoint(worldPos);
                v /= uIView.inputScale;
                Vector3 vector2 = cursorInfoLabel.pivot.UpperLeftToTransform(cursorInfoLabel.size, cursorInfoLabel.arbitraryPivotOffset);
                Vector3 relativePosition = uIView.ScreenPointToGUI(v) + new Vector2(vector2.x, vector2.y);
                cursorInfoLabel.text = text;
                if (relativePosition.x < 0f)
                {
                    relativePosition.x = 0f;
                }
                if (relativePosition.y < 0f)
                {
                    relativePosition.y = 0f;
                }
                if (relativePosition.x + cursorInfoLabel.width > vector.x)
                {
                    relativePosition.x = vector.x - cursorInfoLabel.width;
                }
                if (relativePosition.y + cursorInfoLabel.height > vector.y)
                {
                    relativePosition.y = vector.y - cursorInfoLabel.height;
                }
                cursorInfoLabel.relativePosition = relativePosition;
            }
            else
            {
                cursorInfoLabel.isVisible = false;
            }
        }

        public void FindRound(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 endir, float radius, out Vector3 startPos1, out Vector3 startDir1, out Vector3 startPos2, out Vector3 startDir2, out Vector3 endPos1, out Vector3 endDir1, out Vector3 endPos2, out Vector3 endDir2, out Vector3 endirFix)
        {
            var dir1 = VectorUtils.NormalizeXZ(pos2 - pos1);
            var dir2 = endir;
            endirFix = endir;
            Vector3 dir3 = new Vector3(dir1.z, 0, -dir1.x);
            Vector3 dir4 = new Vector3(-dir2.z, 0, dir2.x);
            var tmpDistance = 100f;
            var tmpDistance1 = 100f;
            startPos1 = Vector3.zero;
            startDir1 = Vector3.zero;
            startPos2 = Vector3.zero;
            startDir2 = Vector3.zero;
            endPos1 = Vector3.zero;
            endDir1 = Vector3.zero;
            endPos2 = Vector3.zero;
            endDir2 = Vector3.zero;
            var tmpStartPos1 = Vector3.zero;
            var tmpEndPos2 = Vector3.zero;
            //return (startPos + point * 2 * dir);
            for (int i = 0; i < 255; i++)
            {
                var point = pos1 + 2 * i * dir1;
                var point1 = point + (8 * radius * dir3) + (8 * radius * dir4);
                var tmpDir = VectorUtils.NormalizeXZ(pos3 - point1);
                var distance = Vector3.Distance(tmpDir, dir2);
                if (distance < 0.1f)
                {
                    if (distance < tmpDistance)
                    {
                        tmpStartPos1 = point;
                        tmpEndPos2 = point1;
                        tmpDistance = distance;
                    }
                }
            }

            dir2 = -endir;
            tmpDistance1 = tmpDistance;
            dir3 = new Vector3(dir1.z, 0, -dir1.x);
            dir4 = new Vector3(-dir2.z, 0, dir2.x);

            for (int i = 0; i < 255; i++)
            {
                var point = pos1 + 2 * i * dir1;
                var point1 = point + (8 * radius * dir3) + (8 * radius * dir4);
                var tmpDir = VectorUtils.NormalizeXZ(pos3 - point1);
                var distance = Vector3.Distance(tmpDir, dir2);
                if (distance < 0.1f)
                {
                    if (distance < tmpDistance1)
                    {
                        tmpStartPos1 = point;
                        tmpEndPos2 = point1;
                        tmpDistance1 = distance;
                    }
                }
            }

            if (tmpDistance != tmpDistance1)
            {
                startPos1 = tmpStartPos1;
                startDir1 = dir1;
                endPos2 = tmpEndPos2;
                endDir2 = dir2;
                startPos2 = (16 * radius * -dir4) + endPos2;
                startDir2 = -dir2;
                endPos1 = (16 * radius * dir3) + startPos1; ;
                endDir1 = -dir1;
                endirFix = -endir;
            }
        }

        public void Build1RoundRoad(bool onlyShow, RenderManager.CameraInfo cameraInfo)
        {
            Bezier3 partA = default(Bezier3);
            Bezier3 partB = default(Bezier3);
            Bezier3 partC = default(Bezier3);
            Bezier3 partD = default(Bezier3);
            Bezier3 partE = default(Bezier3);
            var m_elevation = (roadType == 0) ? 0f : 8f;
            FindRound(pos0, pos1, pos2, GetNodeDir(node2), radius, out Vector3 NodeA1, out Vector3 NodeA1Dir, out Vector3 NodeB1, out Vector3 NodeB1Dir, out Vector3 NodeB2, out Vector3 NodeB2Dir, out Vector3 NodeA2, out Vector3 NodeA2Dir, out Vector3 endirFix);
            partA.a = pos1;
            partA.d = NodeA1;
            CustomNetSegment.CalculateMiddlePoints(pos1, VectorUtils.NormalizeXZ(pos1 - pos0), NodeA1, -VectorUtils.NormalizeXZ(NodeA1Dir), true, true, out partA.b, out partA.c);
            partB.a = NodeA1;
            partB.d = NodeB1;
            CustomNetSegment.CalculateMiddlePoints(NodeA1, VectorUtils.NormalizeXZ(NodeA1Dir), NodeB1, -VectorUtils.NormalizeXZ(NodeB1Dir), true, true, out partB.b, out partB.c);
            partC.a = NodeB1;
            partC.d = NodeB2;
            CustomNetSegment.CalculateMiddlePoints(NodeB1, VectorUtils.NormalizeXZ(NodeB1Dir), NodeB2, -VectorUtils.NormalizeXZ(NodeB2Dir), true, true, out partC.b, out partC.c);
            partD.a = NodeB2;
            partD.d = NodeA2;
            CustomNetSegment.CalculateMiddlePoints(NodeB2, VectorUtils.NormalizeXZ(NodeB2Dir), NodeA2, -VectorUtils.NormalizeXZ(NodeA2Dir), true, true, out partD.b, out partD.c);
            partE.a = NodeA2;
            partE.d = pos2;
            CustomNetSegment.CalculateMiddlePoints(NodeA2, VectorUtils.NormalizeXZ(NodeA2Dir), pos2, -endirFix, true, true, out partE.b, out partE.c);

            if (!onlyShow)
            {
                if (NodeA1 == Vector3.zero) DebugLog.LogToFileOnly("NodeA1 not found");
                if (NodeB1 == Vector3.zero) DebugLog.LogToFileOnly("NodeB1 not found");
                if (NodeB2 == Vector3.zero) DebugLog.LogToFileOnly("NodeB2 not found");
                if (NodeA2 == Vector3.zero) DebugLog.LogToFileOnly("NodeA2 not found");
                DebugLog.LogToFileOnly(pos0.ToString());
                DebugLog.LogToFileOnly(pos1.ToString());
                DebugLog.LogToFileOnly(pos2.ToString());

                DebugLog.LogToFileOnly(NodeA1.ToString());
                DebugLog.LogToFileOnly(NodeB1.ToString());
                DebugLog.LogToFileOnly(NodeB2.ToString());
                DebugLog.LogToFileOnly(NodeA2.ToString());

                DebugLog.LogToFileOnly(NodeA1Dir.ToString());
                DebugLog.LogToFileOnly(NodeB1Dir.ToString());
                DebugLog.LogToFileOnly(NodeB2Dir.ToString());
                DebugLog.LogToFileOnly(NodeA2Dir.ToString());

                var rand = new Randomizer(0u);
                //var m_currentModule = Parser.ModuleNameFromUI(MainUI.fromSelected, MainUI.toSelected, MainUI.symmetry, MainUI.uturnLane, MainUI.hasSidewalk, MainUI.hasBike);
                DebugLog.LogToFileOnly(m_netInfo.name);
                var m_prefab = m_netInfo;
                ToolErrors errors = default(ToolErrors);
                var netInfo = m_prefab.m_netAI.GetInfo(m_elevation, m_elevation, 5, false, false, false, false, ref errors);

                int m_nodeNum = 0;
                int partANum = 0;
                if (Vector2.Distance(VectorUtils.XZ(pos1 - pos0), VectorUtils.XZ(NodeA1 - pos1)) == 0)
                {
                    partANum = (int)(Vector2.Distance(VectorUtils.XZ(pos1), VectorUtils.XZ(NodeA1)) / 64f);
                }
                else
                {
                    partANum = -1;
                }
                int partBNum = (int)(Vector2.Distance(VectorUtils.XZ(NodeA1), VectorUtils.XZ(NodeB1)) / 64f);
                int partCNum = (int)(Vector2.Distance(VectorUtils.XZ(NodeB1), VectorUtils.XZ(NodeB2)) / 64f);
                int partDNum = (int)(Vector2.Distance(VectorUtils.XZ(NodeB2), VectorUtils.XZ(NodeA2)) / 64f);
                int partENum = (int)(Vector2.Distance(VectorUtils.XZ(NodeA2), VectorUtils.XZ(pos2)) / 64f);

                m_nodeNum += partANum + 1;
                m_nodeNum += partBNum + 1;
                m_nodeNum += partCNum + 1;
                m_nodeNum += partDNum + 1;
                m_nodeNum += partENum;
                ushort[] node = new ushort[m_nodeNum];
                ushort[] segment = new ushort[m_nodeNum];

                if (partANum >= 0)
                {
                    for (int i = 0; i <= partANum; i++)
                    {
                        float p1 = (float)(i + 1) / (float)(partANum + 1);
                        float p2 = (float)(i) / (float)(partANum + 1);
                        CreateNode(out node[i], ref rand, netInfo, partA.Position(p1));
                        if (i == 0)
                        {
                            if (Singleton<NetManager>.instance.CreateSegment(out segment[i], ref rand, netInfo, node1, node[i], VectorUtils.NormalizeXZ(pos1 - pos0), -VectorUtils.NormalizeXZ(partA.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                                Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                        }
                        else
                        {
                            if (Singleton<NetManager>.instance.CreateSegment(out segment[i], ref rand, netInfo, node[i - 1], node[i], VectorUtils.NormalizeXZ(partA.Tangent(p2)), -VectorUtils.NormalizeXZ(partA.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                                Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                        }
                    }
                }

                for (int i = 0; i <= partBNum; i++)
                {
                    float p1 = (float)(i + 1) / (float)(partBNum + 1);
                    float p2 = (float)(i) / (float)(partBNum + 1);
                    CreateNode(out node[i + partANum + 1], ref rand, netInfo, partB.Position(p1));
                    if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + 1], ref rand, netInfo, node[i + partANum], node[i + partANum + 1], VectorUtils.NormalizeXZ(partB.Tangent(p2)), -VectorUtils.NormalizeXZ(partB.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                        Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                }


                for (int i = 0; i <= partCNum; i++)
                {
                    float p1 = (float)(i + 1) / (float)(partCNum + 1);
                    float p2 = (float)(i) / (float)(partCNum + 1);
                    CreateNode(out node[i + partANum + partBNum + 2], ref rand, netInfo, partC.Position(p1));
                    if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + 2], ref rand, netInfo, node[i + partANum + partBNum + 1], node[i + partANum + partBNum + 2], VectorUtils.NormalizeXZ(partC.Tangent(p2)), -VectorUtils.NormalizeXZ(partC.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                        Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                }


                for (int i = 0; i <= partDNum; i++)
                {
                    float p1 = (float)(i + 1) / (float)(partDNum + 1);
                    float p2 = (float)(i) / (float)(partDNum + 1);
                    CreateNode(out node[i + partANum + partBNum + partCNum + 3], ref rand, netInfo, partD.Position(p1));
                    if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + partCNum + 3], ref rand, netInfo, node[i + partANum + partBNum + partCNum + 2], node[i + partANum + partBNum + partCNum + 3], VectorUtils.NormalizeXZ(partD.Tangent(p2)), -VectorUtils.NormalizeXZ(partD.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                        Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                }

                if (partENum > 0)
                {
                    for (int i = 1; i <= partENum; i++)
                    {
                        float p1 = (float)i / (float)(partENum + 1);
                        float p2 = (float)(i - 1) / (float)(partENum + 1);
                        CreateNode(out node[i + partANum + partBNum + partCNum + partDNum + 3], ref rand, netInfo, partE.Position(p1));
                        if (Singleton<NetManager>.instance.CreateSegment(out segment[i + partANum + partBNum + partCNum + partDNum + 3], ref rand, netInfo, node[i + partANum + partBNum + partCNum + partDNum + 2], node[i + partANum + partBNum + partCNum + partDNum + 3], VectorUtils.NormalizeXZ(partE.Tangent(p2)), -VectorUtils.NormalizeXZ(partE.Tangent(p1)), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                            Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                    }
                }

                ushort segmentId;
                float tmp = (float)partENum / (float)(partENum + 1);
                if (Singleton<NetManager>.instance.CreateSegment(out segmentId, ref rand, netInfo, node[partANum + partBNum + partCNum + partDNum + partENum + 3], node2, VectorUtils.NormalizeXZ(partE.Tangent(tmp)), -endirFix, Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
            }
            else
            {
                if (Vector2.Distance(VectorUtils.XZ(pos1 - pos0), VectorUtils.XZ(NodeA1 - pos1)) == 0)
                {
                    Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partA, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                }
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partB, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partC, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partD, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partE, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
            }
        }
    }
}