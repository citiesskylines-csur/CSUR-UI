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
        Vector3 pos3;
        Vector3 pos4;
        ushort node0;
        ushort node1;
        ushort node2;
        ushort node3;
        byte radius;
        byte rampMode;
        byte roadType;

        Vector3 debugA1pos;
        Vector3 debugA2pos;
        Vector3 debugB1pos;
        Vector3 debugB2pos;

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
            }

            if (m_step != 6)
            {
                switch (m_step)
                {
                    case 0: CustomShowToolInfo(true, "Please use ShotCut key to select ramp type", output.m_hitPos); break;
                    case 1: CustomShowToolInfo(true, "Please use ShotCut key to select road type", output.m_hitPos); break;
                    default: break;
                }

                if (determineHoveredElements())
                {
                    switch (m_step)
                    {
                        case 2:
                            pos0 = GetNode(m_hover).m_position; node0 = m_hover; CustomShowToolInfo(true, "Please select pre-start node", output.m_hitPos); break;
                        case 3:
                            pos1 = GetNode(m_hover).m_position; node1 = m_hover; CustomShowToolInfo(true, "Please select start node", output.m_hitPos); break;
                        case 4:
                            pos2 = GetNode(m_hover).m_position; node2 = m_hover; CustomShowToolInfo(true, "Please select end node", output.m_hitPos); break;
                        case 5:
                            pos3 = GetNode(m_hover).m_position; node3 = m_hover; CustomShowToolInfo(true, "Please select post-end node", output.m_hitPos); break;
                        default: break;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (m_step > 1 && m_step < 6)
                    {
                        m_step++;
                    }
                }
            }
            else
            {
                CustomShowToolInfo(true, "Please select Round centre and adjust radius " + radius.ToString() + "\n Click mouse to build", output.m_hitPos);
                pos4 = output.m_hitPos;
            }
        }

        protected override void OnToolGUI(Event e)
        {
            if (enabled == true)
            {
                if (OptionsKeymappingRoadTool.m_add.IsPressed(e)) radius = (byte)COMath.Clamp(radius + 1, 10, 250);
                if (OptionsKeymappingRoadTool.m_addPlus.IsPressed(e)) radius = (byte)COMath.Clamp(radius + 10, 10, 250);
                if (OptionsKeymappingRoadTool.m_minus.IsPressed(e)) radius = (byte)COMath.Clamp(radius - 1, 10, 250);
                if (OptionsKeymappingRoadTool.m_minusPlus.IsPressed(e)) radius = (byte)COMath.Clamp(radius -10, 10, 250);
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
                if (OptionsKeymappingRoadTool.m_rightRamp3Round.IsPressed(e))
                {
                    if (m_step == 0) m_step++;
                    rampMode = 0;
                }
                if (OptionsKeymappingRoadTool.m_build.IsPressed(e))
                {
                    if (m_step == 6)
                    {
                        if (rampMode == 0)
                        {
                            BuildLeft3RoundRoad(false, null);
                        }
                        m_step = 0;
                        CustomShowToolInfo(show: false, null, Vector3.zero);
                        ToolsModifierControl.SetTool<DefaultTool>();
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
                if (m_hover != 0 && (m_step != 6))
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
                if (m_step == 6)
                {
                    BuildLeft3RoundRoad(true, cameraInfo);
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

        private Vector3 CreateNodePosition(Vector3 oldPos)
        {
            var pos = new Vector3(oldPos.x, 0, oldPos.z);
            pos.y = Singleton<TerrainManager>.instance.SampleRawHeightSmoothWithWater(pos, false, 0f);
            return pos;
        }

        public void BuildLeft3RoundRoad(bool onlyShow, RenderManager.CameraInfo cameraInfo)
        {
            Bezier3 partA = default(Bezier3);
            Bezier3 partB = default(Bezier3);
            Bezier3 partC = default(Bezier3);
            Bezier3 partD = default(Bezier3);
            Bezier3 partE = default(Bezier3);
            var m_elevation = (roadType == 0) ? 0f : 8f;
            GetRound(pos4, radius*8f, ref partA, ref partB, ref partC, ref partD);
            FindNodeA(true, pos1, VectorUtils.NormalizeXZ(pos1 - pos0), partA, partB, partC, partD, out Vector3 NodeA1, out Vector3 NodeA1Dir);
            FindNodeB(pos1, VectorUtils.NormalizeXZ(pos1 - pos0), partA, partB, partC, partD, out Vector3 NodeB1, out Vector3 NodeB1Dir);
            FindNodeB(pos2, VectorUtils.NormalizeXZ(pos3 - pos2), partA, partB, partC, partD, out Vector3 NodeB2, out Vector3 NodeB2Dir);
            FindNodeA(false, pos2, VectorUtils.NormalizeXZ(pos3 - pos2), partA, partB, partC, partD, out Vector3 NodeA2, out Vector3 NodeA2Dir); ; ;

            if (NodeA1 == Vector3.zero) DebugLog.LogToFileOnly("NodeA1 not found");
            if (NodeB1 == Vector3.zero) DebugLog.LogToFileOnly("NodeB1 not found");
            if (NodeB2 == Vector3.zero) DebugLog.LogToFileOnly("NodeB2 not found");
            if (NodeA2 == Vector3.zero) DebugLog.LogToFileOnly("NodeA2 not found");

            if (!onlyShow)
            {
                DebugLog.LogToFileOnly(pos0.ToString());
                DebugLog.LogToFileOnly(pos1.ToString());
                DebugLog.LogToFileOnly(pos2.ToString());
                DebugLog.LogToFileOnly(pos3.ToString());
                DebugLog.LogToFileOnly(pos4.ToString());

                DebugLog.LogToFileOnly(NodeA1.ToString());
                DebugLog.LogToFileOnly(NodeB1.ToString());
                DebugLog.LogToFileOnly(NodeB2.ToString());
                DebugLog.LogToFileOnly(NodeA2.ToString());

                DebugLog.LogToFileOnly(NodeA1Dir.ToString());
                DebugLog.LogToFileOnly(NodeB1Dir.ToString());
                DebugLog.LogToFileOnly(NodeB2Dir.ToString());
                DebugLog.LogToFileOnly(NodeA2Dir.ToString());

                debugA1pos = NodeA1;
                debugA2pos = NodeA2;
                debugB1pos = NodeB1;
                debugB2pos = NodeB2;

                var rand = new Randomizer(0u);
                //var m_currentModule = Parser.ModuleNameFromUI(MainUI.fromSelected, MainUI.toSelected, MainUI.symmetry, MainUI.uturnLane, MainUI.hasSidewalk, MainUI.hasBike);
                DebugLog.LogToFileOnly(m_netInfo.name);
                var m_prefab = m_netInfo;
                ToolErrors errors = default(ToolErrors);
                var netInfo = m_prefab.m_netAI.GetInfo(m_elevation, m_elevation, 5, false, false, false, false, ref errors);

                CreateNode(out ushort nodeA1, ref rand, netInfo, NodeA1);
                AdjustElevation(nodeA1, m_elevation);
                CreateNode(out ushort nodeB1, ref rand, netInfo, NodeB1);
                AdjustElevation(nodeB1, m_elevation);
                CreateNode(out ushort nodeB2, ref rand, netInfo, NodeB2);
                AdjustElevation(nodeB2, m_elevation);
                CreateNode(out ushort nodeA2, ref rand, netInfo, NodeA2);
                AdjustElevation(nodeA2, m_elevation);

                ushort segmentId0;
                if (Singleton<NetManager>.instance.CreateSegment(out segmentId0, ref rand, netInfo, node1, nodeA1, VectorUtils.NormalizeXZ(pos1 - pos0), -VectorUtils.NormalizeXZ(NodeA1Dir), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                ushort segmentId1;
                if (Singleton<NetManager>.instance.CreateSegment(out segmentId1, ref rand, netInfo, nodeA1, nodeB1, VectorUtils.NormalizeXZ(NodeA1Dir), -VectorUtils.NormalizeXZ(NodeB1Dir), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                ushort segmentId2;
                if (Singleton<NetManager>.instance.CreateSegment(out segmentId2, ref rand, netInfo, nodeB1, nodeB2, VectorUtils.NormalizeXZ(NodeB1Dir), -VectorUtils.NormalizeXZ(NodeB2Dir), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                ushort segmentId3;
                if (Singleton<NetManager>.instance.CreateSegment(out segmentId3, ref rand, netInfo, nodeB2, nodeA2, VectorUtils.NormalizeXZ(NodeB2Dir), -VectorUtils.NormalizeXZ(NodeA2Dir), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
                ushort segmentId4;
                if (Singleton<NetManager>.instance.CreateSegment(out segmentId4, ref rand, netInfo, nodeA2, node2, VectorUtils.NormalizeXZ(NodeA2Dir), -VectorUtils.NormalizeXZ(pos3 - pos2), Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
                    Singleton<SimulationManager>.instance.m_currentBuildIndex += 2u;
            } 
            else
            {
                partA.a = pos1;
                partA.d = NodeA1;
                NetSegment.CalculateMiddlePoints(pos1, VectorUtils.NormalizeXZ(pos1 - pos0), NodeA1, -VectorUtils.NormalizeXZ(NodeA1Dir), true, true, out partA.b, out partA.c);
                partB.a = NodeA1;
                partB.d = NodeB1;
                NetSegment.CalculateMiddlePoints(NodeA1, VectorUtils.NormalizeXZ(NodeA1Dir), NodeB1, -VectorUtils.NormalizeXZ(NodeB1Dir), true, true, out partB.b, out partB.c);
                partC.a = NodeB1;
                partC.d = NodeB2;
                NetSegment.CalculateMiddlePoints(NodeB1, VectorUtils.NormalizeXZ(NodeB1Dir), NodeB2, -VectorUtils.NormalizeXZ(NodeB2Dir), true, true, out partC.b, out partC.c);
                partD.a = NodeB2;
                partD.d = NodeA2;
                NetSegment.CalculateMiddlePoints(NodeB2, VectorUtils.NormalizeXZ(NodeB2Dir), NodeA2, -VectorUtils.NormalizeXZ(NodeA2Dir), true, true, out partD.b, out partD.c);
                partE.a = NodeA2;
                partE.d = pos2;
                NetSegment.CalculateMiddlePoints(NodeA2, VectorUtils.NormalizeXZ(NodeA2Dir), pos2, -VectorUtils.NormalizeXZ(pos3 - pos2), true, true, out partE.b, out partE.c);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partA, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partB, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partC, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, m_validColorInfo, partD, 16f, -100000f, -100000f, -1f, 1280f, renderLimits: false, alphaBlend: false);
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
            NetSegment.CalculateMiddlePoints(controlP1, direction1, controlP2, -direction2, true, true, out partA.b, out partA.c);
            partB.a = controlP2;
            partB.d = controlP3;
            NetSegment.CalculateMiddlePoints(controlP2, direction2, controlP3, -direction3, true, true, out partB.b, out partB.c);
            partC.a = controlP3;
            partC.d = controlP4;
            NetSegment.CalculateMiddlePoints(controlP3, direction3, controlP4, -direction4, true, true, out partC.b, out partC.c);
            partD.a = controlP4;
            partD.d = controlP1;
            NetSegment.CalculateMiddlePoints(controlP4, direction4, controlP1, -direction1, true, true, out partD.b, out partD.c);
        }

        public string isRound(Vector3 startPos, Vector3 startDir, Vector3 endPos, Vector3 endDir, out float diff)
        {
            diff = 100f;
            float a = (startDir.x - endDir.x) * (startPos.z - endPos.z);
            float b = (startDir.z - endDir.z) * (startPos.x - endPos.x);
            if (a == b && !(((startDir.x - endDir.x) ==0) && (startDir.z - endDir.z) == 0))
            {
                return "Yes";
            }
            else if ((a > 0 && b > 0) || (a < 0 && b < 0))
            {
                if (b!= 0)
                {
                    if (((a / b) <= 1.1f) && ((a / b) >= 0.9f) && (Math.Abs(a - b) < 5f))
                    {
                        diff = Math.Abs(a / b - 1) + Math.Abs(a - b);
                        return "Maybe";
                    }
                }
            }
            return "No";
        }
        public void FindNodeA(bool isStart, Vector3 startPos, Vector3 startDir, Bezier3 partA, Bezier3 partB, Bezier3 partC, Bezier3 partD, out Vector3 NodeA, out Vector3 NodeADir)
        {
            NodeA = Vector3.zero;
            NodeADir = Vector3.zero;
            startDir = isStart ? startDir : -startDir;
            var tmpNodeA = Vector3.zero;
            var tmpNodeADir = Vector3.zero;
            var tmpDiff = 1000f;
            //partA
            for (int i = 0; i < 255; i++)
            {
                float p = (float)i / (float)(255);
                var dir = isStart? -VectorUtils.NormalizeXZ(partA.Tangent(p)) : VectorUtils.NormalizeXZ(partA.Tangent(p));
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

            if (tmpDiff!=1000f)
            {
                NodeA = tmpNodeA;
                NodeADir = tmpNodeADir;
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
    }
}