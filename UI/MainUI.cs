using ColossalFramework;
using ColossalFramework.UI;
using CSUR_UI.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSUR_UI.UI
{
    public class MainUI : UIPanel
    {
        public static readonly string cacheName = "MainUI";
        private static readonly float WIDTH = 731f;
        private static readonly float HEIGHT = 327f;
        private static readonly float SPACING2 = 45f;
        private static readonly float BTN_SIZE = 39f;

        private static readonly int N_POS_INT = 10;
        private static readonly int CENTER = 2;

        private static readonly string[] labelsHalf = {"1P", "0P", "0P", "1P", "2P", "3P", "4P", "5P", "6P"};
        private static readonly string[] labelsInt = {"2", "1", "C", "1", "2", "3", "4", "5", "6", "7" };
        private UIDragHandle m_DragHandler;
        private UIButton m_closeButton;
        private UISprite m_UIBG;
        private UISprite m_UITOP;
        private UIButton m_copyButton;
        private UIButton m_swapButton;
        private UIButton m_clearButton;

        private UIButton m_asym0Button;
        private UIButton m_asym1Button;
        private UIButton m_asym2Button;
        private UIButton m_uturnLaneButton;
        private UIButton m_hasSideWalkButton;

        private UIButton[] m_toHalfButtons = new UIButton[N_POS_INT - 1];
        private UIButton[] m_toIntButtons = new UIButton[N_POS_INT];

        private UIButton[] m_fromHalfButtons = new UIButton[N_POS_INT - 1];
        private UIButton[] m_fromIntButtons = new UIButton[N_POS_INT];

        private UILabel m_fromLabel;
        private UILabel m_toLabel;
        private UISprite m_result;

        public NetInfo m_netInfo;
        public NetTool m_netTool;
        // road cache
        public List<string> NETPICKER_ROADCACHE_STRINGS = new List<string>();
        public List<List<UIComponent>> NETPICKER_ROADCACHE_DICTIONARY = new List<List<UIComponent>>();
        public static bool refreshOnce = false;
        public static int fromSelected;
        public static int toSelected;
        public static byte symmetry;
        public static bool uturnLane;
        public static bool hasSidewalk;

        public override void Update()
        {
            RefreshDisplayData();
            base.Update();
        }

        public override void Start()
        {
            base.Start();
            //init data
            fromSelected = 0;
            toSelected = 0;
            symmetry = 255;
            uturnLane = false;
            hasSidewalk = false;
            //UI
            size = new Vector2(WIDTH, HEIGHT);
            backgroundSprite = "MenuPanel";
            canFocus = true;
            isInteractive = true;
            BringToFront();
            relativePosition = new Vector3((Loader.parentGuiView.fixedWidth / 2 + 20f), 170f);
            opacity = 1f;
            cachedName = cacheName;
            //UITOP
            m_DragHandler = AddUIComponent<UIDragHandle>();
            m_DragHandler.target = this;
            m_DragHandler.zOrder = 11;
            m_UITOP = m_DragHandler.AddUIComponent<UISprite>();
            m_UITOP.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasNameHeader);
            m_UITOP.spriteName = "UITOP";
            m_UITOP.relativePosition = new Vector3(0f, 0f);
            m_UITOP.width = WIDTH;
            m_UITOP.height = 50f;
            m_UITOP.zOrder = 12;
            //UIBG
            m_UIBG = AddUIComponent<UISprite>();
            m_UIBG.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasNameBg);
            m_UIBG.spriteName = "UIBG";
            m_UIBG.relativePosition = new Vector3(0f, 50f);
            m_UIBG.width = WIDTH;
            m_UIBG.height = HEIGHT -50f;
            m_UIBG.zOrder = 12;
            //result
            m_result = AddUIComponent<UISprite>();
            m_result.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasNameNoAsset);
            m_result.spriteName = "NOASSET";
            m_result.relativePosition = new Vector3(10f, 60f);
            m_result.width = 219f;
            m_result.height = 203f;
            m_result.zOrder = 11;
            //close
            m_closeButton = AddUIComponent<UIButton>();
            m_closeButton.normalBgSprite = "buttonclose";
            m_closeButton.hoveredBgSprite = "buttonclosehover";
            m_closeButton.pressedBgSprite = "buttonclosepressed";
            m_closeButton.relativePosition = new Vector3(WIDTH - 35f, 5f, 10f);
            m_closeButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                Hide();
            };
            Hide(); //dont show in the beginning
            DoOnStartup();
        }

        private void DoOnStartup()
        {
            ShowOnGui();
            RefreshData();
            RefreshDisplayData();
        }

        private void ShowOnGui()
        {
            float currentX = 260f, currentY = 65f;
            // Top (to) section, half positions
            for (int i = 0; i < N_POS_INT - 1; i++)
            {
                m_toHalfButtons[i] = AddUIComponent<UIButton>();
                m_toHalfButtons[i].playAudioEvents = true;
                m_toHalfButtons[i].relativePosition = new Vector3(currentX + SPACING2, currentY);
                m_toHalfButtons[i].atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
                m_toHalfButtons[i].normalBgSprite = labelsHalf[i] + ((i < CENTER) ? "_L" : "_R");
                m_toHalfButtons[i].size = new Vector2(BTN_SIZE, BTN_SIZE);
                m_toHalfButtons[i].zOrder = 11;
                m_toHalfButtons[i].stringUserData = $"ToLanePos_{2 * i + 1}";
                m_toHalfButtons[i].eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    byte myBit = System.Convert.ToByte(component.stringUserData.Substring(10));
                    toSelected ^= 1 << myBit;
                    toSelected &= ~(1 << myBit + 1);
                    if (myBit > 0) toSelected &= ~(1 << myBit - 1);
                    RefreshData();
                };
                currentX = m_toHalfButtons[i].relativePosition.x;
            }

            //  Top (to) section, integer positions
            currentY = SPACING2 + m_toHalfButtons[0].relativePosition.y;
            currentX = 237.5f;
            for (int i = 0; i < N_POS_INT; i++)
            {
                m_toIntButtons[i] = AddUIComponent<UIButton>();
                m_toIntButtons[i].playAudioEvents = true;
                m_toIntButtons[i].relativePosition = new Vector3(currentX + SPACING2, currentY);
                m_toIntButtons[i].atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
                m_toIntButtons[i].normalBgSprite = labelsInt[i] + ((i < CENTER) ? "_L" : (i > CENTER) ? "_R" : "_C");
                m_toIntButtons[i].size = new Vector2(BTN_SIZE, BTN_SIZE);
                m_toIntButtons[i].zOrder = 11;
                m_toIntButtons[i].stringUserData = $"ToLanePos_{2 * i}";
                m_toIntButtons[i].eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    byte myBit = System.Convert.ToByte(component.stringUserData.Substring(10));
                    toSelected ^= 1 << myBit;
                    toSelected &= ~(1 << myBit + 1);
                    if (myBit > 0) toSelected &= ~(1 << myBit - 1);
                    RefreshData();
                };
                currentX = m_toIntButtons[i].relativePosition.x;
            }

            // Bottom (from) section, integer positions
            currentX = m_toIntButtons[0].relativePosition.x - SPACING2;
            currentY = m_toIntButtons[0].relativePosition.y + 120f;
            for (int i = 0; i < N_POS_INT; i++)
            {
                m_fromIntButtons[i] = AddUIComponent<UIButton>();
                m_fromIntButtons[i].playAudioEvents = true;
                m_fromIntButtons[i].relativePosition = new Vector3(currentX + SPACING2, currentY);
                m_fromIntButtons[i].atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
                m_fromIntButtons[i].normalBgSprite = labelsInt[i] + ((i < CENTER) ? "_L" : (i > CENTER) ? "_R" : "_C");
                m_fromIntButtons[i].size = new Vector2(BTN_SIZE, BTN_SIZE);
                m_fromIntButtons[i].zOrder = 11;
                m_fromIntButtons[i].stringUserData = $"FromLanePos_{2 * i}";
                m_fromIntButtons[i].eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    byte myBit = System.Convert.ToByte(component.stringUserData.Substring(12));
                    fromSelected ^= 1 << myBit;
                    fromSelected &= ~(1 << myBit + 1);
                    if (myBit > 0) fromSelected &= ~(1 << myBit - 1);
                    RefreshData();
                };
                currentX = m_fromIntButtons[i].relativePosition.x;
            }


            // Bottom (from) section, half positions
            currentX = m_toHalfButtons[0].relativePosition.x - SPACING2;
            currentY = m_toHalfButtons[0].relativePosition.y + 210f;
            for (int i = 0; i < N_POS_INT - 1; i++)
            {
                m_fromHalfButtons[i] = AddUIComponent<UIButton>();
                m_fromHalfButtons[i].playAudioEvents = true;
                m_fromHalfButtons[i].relativePosition = new Vector3(currentX + SPACING2, currentY);
                m_fromHalfButtons[i].atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
                m_fromHalfButtons[i].normalBgSprite = labelsHalf[i] + ((i < CENTER) ? "_L" : "_R");
                m_fromHalfButtons[i].size = new Vector2(BTN_SIZE, BTN_SIZE);
                m_fromHalfButtons[i].zOrder = 11;
                m_fromHalfButtons[i].stringUserData = $"FromLanePos_{2 * i + 1}";
                m_fromHalfButtons[i].eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    byte myBit = System.Convert.ToByte(component.stringUserData.Substring(12));
                    fromSelected ^= 1 << myBit;
                    fromSelected &= ~(1 << myBit + 1);
                    if (myBit > 0) fromSelected &= ~(1 << myBit - 1);
                    RefreshData();
                };
                currentX = m_fromHalfButtons[i].relativePosition.x;
            }

            m_copyButton = AddUIComponent<UIButton>();
            m_copyButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_copyButton.normalBgSprite = "COPY";
            m_copyButton.hoveredBgSprite = "COPY_S";
            m_copyButton.playAudioEvents = true;
            m_copyButton.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_copyButton.relativePosition = new Vector3(m_toIntButtons[3].relativePosition.x + 10f, 
                                                        m_toIntButtons[3].relativePosition.y + 60f);
            m_copyButton.autoSize = true;
            m_copyButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = fromSelected;
                RefreshData();
            };

            m_swapButton = AddUIComponent<UIButton>();
            m_swapButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_swapButton.normalBgSprite = "SWAP";
            m_swapButton.hoveredBgSprite = "SWAP_S";
            m_swapButton.playAudioEvents = true;
            m_swapButton.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_swapButton.relativePosition = new Vector3(m_copyButton.relativePosition.x + 55f, m_copyButton.relativePosition.y);
            m_swapButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                int temp = fromSelected;
                fromSelected = toSelected;
                toSelected = temp;
                RefreshData();
            };

            m_asym0Button = AddUIComponent<UIButton>();
            m_asym0Button.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_asym0Button.normalBgSprite = "+0";
            m_asym0Button.playAudioEvents = true;
            m_asym0Button.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_asym0Button.relativePosition = new Vector3(10f, m_fromHalfButtons[0].relativePosition.y);
            m_asym0Button.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                asym0Button_OnCheckChanged();
                refreshOnce = true;
            };

            m_asym1Button = AddUIComponent<UIButton>();
            m_asym1Button.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_asym1Button.normalBgSprite = "+1";
            m_asym1Button.playAudioEvents = true;
            m_asym1Button.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_asym1Button.relativePosition = new Vector3(m_asym0Button.relativePosition.x + SPACING2, m_fromHalfButtons[0].relativePosition.y);
            m_asym1Button.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                asym1Button_OnCheckChanged();
                refreshOnce = true;
            };

            m_asym2Button = AddUIComponent<UIButton>();
            m_asym2Button.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_asym2Button.normalBgSprite = "+2";
            m_asym2Button.playAudioEvents = true;
            m_asym2Button.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_asym2Button.relativePosition = new Vector3(m_asym1Button.relativePosition.x + SPACING2, m_fromHalfButtons[0].relativePosition.y);
            m_asym2Button.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                asym2Button_OnCheckChanged();
                refreshOnce = true;
            };

            m_uturnLaneButton = AddUIComponent<UIButton>();
            m_uturnLaneButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_uturnLaneButton.normalBgSprite = "UTURN";
            m_uturnLaneButton.playAudioEvents = true;
            m_uturnLaneButton.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_uturnLaneButton.relativePosition = new Vector3(m_asym2Button.relativePosition.x + SPACING2, m_fromHalfButtons[0].relativePosition.y);
            m_uturnLaneButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                uturnLaneButton_OnCheckChanged();
                refreshOnce = true;
            };

            m_hasSideWalkButton = AddUIComponent<UIButton>();
            m_hasSideWalkButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_hasSideWalkButton.normalBgSprite = "SIDEWALK";
            m_hasSideWalkButton.playAudioEvents = true;
            m_hasSideWalkButton.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_hasSideWalkButton.relativePosition = new Vector3(m_uturnLaneButton.relativePosition.x + SPACING2, m_fromHalfButtons[0].relativePosition.y);
            m_hasSideWalkButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                hasSideWalkButton_OnCheckChanged();
                refreshOnce = true;
            };

            m_fromLabel = AddUIComponent<UILabel>();
            m_fromLabel.text = "From";
            m_fromLabel.textScale = 1f;
            m_fromLabel.textColor = new Color32(54, 54, 54, 54);
            m_fromLabel.relativePosition = new Vector3(m_toIntButtons[0].relativePosition.x, m_toIntButtons[0].relativePosition.y +70f);
            m_fromLabel.autoSize = true;

            m_toLabel = AddUIComponent<UILabel>();
            m_toLabel.text = "To";
            m_toLabel.textScale = 1f;
            m_toLabel.textColor = new Color32(54, 54, 54, 54);
            m_toLabel.relativePosition = new Vector3(m_toIntButtons[9].relativePosition.x, m_toIntButtons[9].relativePosition.y + 50f);

            m_clearButton = AddUIComponent<UIButton>();
            m_clearButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName1);
            m_clearButton.normalBgSprite = "CLEAR";
            m_clearButton.hoveredBgSprite = "CLEAR_S";
            m_clearButton.playAudioEvents = true;
            m_clearButton.size = new Vector2(BTN_SIZE, BTN_SIZE);
            m_clearButton.relativePosition = new Vector3(m_swapButton.relativePosition.x + 55f, m_swapButton.relativePosition.y);
            m_clearButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = 0;
                toSelected = 0;
                RefreshData();
            };
        }

        public void asym0Button_OnCheckChanged()
        {
            if (symmetry!=0)
            {
                symmetry = 0;
                m_asym0Button.normalBgSprite = "+0_S";
                m_asym1Button.normalBgSprite = "+1";
                m_asym2Button.normalBgSprite = "+2";
            }
            else
            {
                m_asym0Button.normalBgSprite = "+0";
                m_asym1Button.normalBgSprite = "+1";
                m_asym2Button.normalBgSprite = "+2";
                symmetry = 255;
            }
        }

        public void asym1Button_OnCheckChanged()
        {
            if (symmetry != 1)
            {
                m_asym0Button.normalBgSprite = "+0";
                m_asym1Button.normalBgSprite = "+1_S";
                m_asym2Button.normalBgSprite = "+2";
                symmetry = 1;
            }
            else
            {
                m_asym0Button.normalBgSprite = "+0";
                m_asym1Button.normalBgSprite = "+1";
                m_asym2Button.normalBgSprite = "+2";
                symmetry = 255;
            }
        }

        public void asym2Button_OnCheckChanged()
        {
            if (symmetry != 2)
            {
                m_asym0Button.normalBgSprite = "+0";
                m_asym1Button.normalBgSprite = "+1";
                m_asym2Button.normalBgSprite = "+2_S";
                symmetry = 2;
            }
            else
            {
                m_asym0Button.normalBgSprite = "+0";
                m_asym1Button.normalBgSprite = "+1";
                m_asym2Button.normalBgSprite = "+2";
                symmetry = 255;
            }
        }
        public void hasSideWalkButton_OnCheckChanged()
        {
            if (!hasSidewalk)
            {
                m_hasSideWalkButton.normalBgSprite = "SIDEWALK_S";
                hasSidewalk = true;
            }
            else
            {
                m_hasSideWalkButton.normalBgSprite = "SIDEWALK";
                hasSidewalk = false;
            }
        }
        public void uturnLaneButton_OnCheckChanged()
        {
            if (!uturnLane)
            {
                m_uturnLaneButton.normalBgSprite = "UTURN_S";
                uturnLane = true;
            }
            else
            {
                m_uturnLaneButton.normalBgSprite = "UTURN";
                uturnLane = false;
            }
        }

        private void RefreshDisplayData()
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            uint num2 = currentFrameIndex & 255u;
            if (refreshOnce)
            {
                if (isVisible)
                {
                    m_fromLabel.text = "From";
                    m_fromLabel.text = "From";
                    //DebugLog.LogToFileOnly("fromSelected = " + fromSelected.ToString() + " toSelected = " + toSelected.ToString() + " symmetry = " + symmetry.ToString() + " uturnLane: " + uturnLane.ToString() + " hasSidewalk: " + hasSidewalk.ToString());

                    var m_currentModule = Parser.ModuleNameFromUI(fromSelected, toSelected, symmetry, uturnLane, hasSidewalk);
                    var m_prefab = PrefabCollection<NetInfo>.FindLoaded(m_currentModule + "_Data");
                    if (m_prefab != null)
                    {
                        m_netTool = ToolsModifierControl.SetTool<NetTool>();
                        UIView.Find("RoadsPanel").Show();
                        m_netTool.m_prefab = m_prefab;
                        m_result.atlas = m_netTool.m_prefab.m_Atlas;
                        m_result.spriteName = m_netTool.m_prefab.m_Thumbnail;
                    }
                    else
                    {
                        ToolsModifierControl.SetTool<DefaultTool>();
                        m_result.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasNameNoAsset);
                        m_result.spriteName = "NOASSET";
                    }
                    refreshOnce = false;
                }
            }
        }

        private void RefreshData()
        {
            for (int i = 0; i < N_POS_INT; i++)
            {
                if ((toSelected & (1 << 2 * i)) == 0)
                {
                    // selected sprite
                    m_toIntButtons[i].normalBgSprite = labelsInt[i] + ((i < CENTER) ? "_L" : (i > CENTER) ? "_R" : "_C");
                } else
                {
                    // unselected sprite
                    m_toIntButtons[i].normalBgSprite = labelsInt[i] + "_S";
                }
                if ((fromSelected & (1 << 2 * i)) == 0)
                {
                    // selected sprite
                    m_fromIntButtons[i].normalBgSprite = labelsInt[i] + ((i < CENTER) ? "_L" : (i > CENTER) ? "_R" : "_C");
                }
                else
                {
                    // unselected sprite
                    m_fromIntButtons[i].normalBgSprite = labelsInt[i] + "_S";
                }
            }
            for (int i = 0; i < N_POS_INT - 1; i++)
            {
                if ((toSelected & (2 << 2 * i)) == 0)
                {
                    // selected sprite
                    m_toHalfButtons[i].normalBgSprite = labelsHalf[i] + ((i < CENTER) ? "_L" : "_R");
                }
                else
                {
                    // unselected sprite
                    m_toHalfButtons[i].normalBgSprite = labelsHalf[i] + "_S";
                }
                if ((fromSelected & (2 << 2 * i)) == 0)
                {
                    // selected sprite
                    m_fromHalfButtons[i].normalBgSprite = labelsHalf[i] + ((i < CENTER) ? "_L" : "_R");
                }
                else
                {
                    // unselected sprite
                    m_fromHalfButtons[i].normalBgSprite = labelsHalf[i] + "_S";
                }
            }

            refreshOnce = true;
        }

    }
}
