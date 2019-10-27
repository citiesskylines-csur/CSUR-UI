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
        private static readonly float WIDTH = 620f;
        private static readonly float HEIGHT = 420f;
        private static readonly float HEADER = 40f;
        private static readonly float SPACING = 25f;
        private static readonly float SPACING2 = 50f;
        private static readonly float BTN_SIZE = 39f;

        private static readonly int N_POS_INT = 10;
        private static readonly int CENTER = 2;

        private static readonly string[] labelsHalf = {"1P", "0P", "0P", "1P", "2P", "3P", "4P", "5P", "6P"};
        private static readonly string[] labelsInt = {"2", "1", "C", "1", "2", "3", "4", "5", "6", "7" };
        private UIDragHandle m_DragHandler;
        private UIButton m_closeButton;
        private UILabel m_title;
        //private UIButton m_okButton;
        private UIButton m_copyButton;
        private UIButton m_swapButton;
        private UIButton m_clearButton;

        public UICheckBox asym0CheckBox;
        public UICheckBox asym1CheckBox;
        public UICheckBox asym2CheckBox;
        public UICheckBox uturnLaneCheckBox;
        public UICheckBox hasSideWalkCheckBox;
        private UILabel asym0CheckBoxText;
        private UILabel asym1CheckBoxText;
        private UILabel asym2CheckBoxText;
        private UILabel uturnLaneCheckBoxText;
        private UILabel hasSideWalkCheckBoxText;

        private UIButton[] m_toHalfButtons = new UIButton[N_POS_INT - 1];
        private UIButton[] m_toIntButtons = new UIButton[N_POS_INT];

        private UIButton[] m_fromHalfButtons = new UIButton[N_POS_INT - 1];
        private UIButton[] m_fromIntButtons = new UIButton[N_POS_INT];


        /*
        private UIButton m_1pLButton;
        private UIButton m_0pLButton;
        private UIButton m_0pRButton;
        private UIButton m_1pRButton;
        private UIButton m_2pRButton;
        private UIButton m_3pRButton;
        private UIButton m_4pRButton;
        private UIButton m_5pRButton;
        private UIButton m_6pRButton;

        private UIButton m_2LButton;
        private UIButton m_1LButton;
        private UIButton m_CButton;
        private UIButton m_1RButton;
        private UIButton m_2RButton;
        private UIButton m_3RButton;
        private UIButton m_4RButton;
        private UIButton m_5RButton;
        private UIButton m_6RButton;
        private UIButton m_7RButton;


        private UIButton m_1pLDButton;
        private UIButton m_0pLDButton;
        private UIButton m_0pRDButton;
        private UIButton m_1pRDButton;
        private UIButton m_2pRDButton;
        private UIButton m_3pRDButton;
        private UIButton m_4pRDButton;
        private UIButton m_5pRDButton;
        private UIButton m_6pRDButton;

        private UIButton m_2LDButton;
        private UIButton m_1LDButton;
        private UIButton m_CDButton;
        private UIButton m_1RDButton;
        private UIButton m_2RDButton;
        private UIButton m_3RDButton;
        private UIButton m_4RDButton;
        private UIButton m_5RDButton;
        private UIButton m_6RDButton;
        private UIButton m_7RDButton;
        */

        private UILabel m_result;

        public NetInfo m_netInfo;
        public NetTool m_netTool;
        // road cache
        public List<string> NETPICKER_ROADCACHE_STRINGS = new List<string>();
        public List<List<UIComponent>> NETPICKER_ROADCACHE_DICTIONARY = new List<List<UIComponent>>();
        public static bool refeshOnce = false;
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
            m_DragHandler = AddUIComponent<UIDragHandle>();
            m_DragHandler.target = this;
            m_title = AddUIComponent<UILabel>();
            m_title.text = "CSUR_UI";
            m_title.relativePosition = new Vector3(WIDTH / 2f - m_title.width / 2f - 25f, HEADER / 2f - m_title.height / 2f);
            m_title.textAlignment = UIHorizontalAlignment.Center;
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
            float currentX = 0, currentY = SPACING2;
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
            currentX = SPACING - SPACING2;
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
            currentY = m_toIntButtons[0].relativePosition.y + 3 * SPACING2;
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
            currentY = m_toHalfButtons[0].relativePosition.y + 5 * SPACING2;
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
            m_copyButton.normalBgSprite = "ToolbarIconGroup1Nomarl";
            m_copyButton.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            m_copyButton.focusedBgSprite = "ToolbarIconGroup1Focused";
            m_copyButton.pressedBgSprite = "ToolbarIconGroup1Pressed";
            m_copyButton.playAudioEvents = true;
            m_copyButton.text = "Copy";
            m_copyButton.relativePosition = new Vector3(m_toHalfButtons[N_POS_INT - 2].relativePosition.x + 2*SPACING2, 
                                                        m_toHalfButtons[N_POS_INT - 2].relativePosition.y + 2*SPACING2);
            m_copyButton.autoSize = true;
            m_copyButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = fromSelected;
                RefreshData();
            };

            m_swapButton = AddUIComponent<UIButton>();
            m_swapButton.normalBgSprite = "ToolbarIconGroup1Nomarl";
            m_swapButton.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            m_swapButton.focusedBgSprite = "ToolbarIconGroup1Focused";
            m_swapButton.pressedBgSprite = "ToolbarIconGroup1Pressed";
            m_swapButton.playAudioEvents = true;
            m_swapButton.text = "Swap";
            m_swapButton.relativePosition = new Vector3(m_copyButton.relativePosition.x, m_copyButton.relativePosition.y + SPACING2);
            m_swapButton.autoSize = true;
            m_swapButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                int temp = fromSelected;
                fromSelected = toSelected;
                toSelected = temp;
                RefreshData();
            };

            //TODO, there should be a text label showing the module name and whether it exists
            m_result = AddUIComponent<UILabel>();
            m_result.text = "";
            m_result.textScale = 2f; 
            m_result.relativePosition = new Vector3(SPACING, 1.5f * SPACING2 + m_toIntButtons[0].relativePosition.y);
            m_result.autoSize = true;

            asym0CheckBox = base.AddUIComponent<UICheckBox>();
            asym0CheckBox.relativePosition = new Vector3(m_fromHalfButtons[0].relativePosition.x, m_fromHalfButtons[0].relativePosition.y + SPACING2);
            this.asym0CheckBoxText = base.AddUIComponent<UILabel>();
            this.asym0CheckBoxText.relativePosition = new Vector3(asym0CheckBox.relativePosition.x + asym0CheckBox.width + 20f, asym0CheckBox.relativePosition.y);
            asym0CheckBox.height = 16f;
            asym0CheckBox.width = 16f;
            asym0CheckBox.label = this.asym0CheckBoxText;
            asym0CheckBox.text = "Sym+0";
            UISprite uISprite0 = asym0CheckBox.AddUIComponent<UISprite>();
            uISprite0.height = 16f;
            uISprite0.width = 16f;
            uISprite0.relativePosition = new Vector3(0f, 0f);
            uISprite0.spriteName = "check-unchecked";
            uISprite0.isVisible = true;
            UISprite uISprite1 = asym0CheckBox.AddUIComponent<UISprite>();
            uISprite1.height = 16f;
            uISprite1.width = 16f;
            uISprite1.relativePosition = new Vector3(0f, 0f);
            uISprite1.spriteName = "check-checked";
            asym0CheckBox.checkedBoxObject = uISprite1;
            asym0CheckBox.isChecked = (symmetry == 0) ? true : false;
            asym0CheckBox.isEnabled = true;
            asym0CheckBox.isVisible = true;
            asym0CheckBox.canFocus = true;
            asym0CheckBox.isInteractive = true;
            asym0CheckBox.eventClicked += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                asym0CheckBox_OnCheckChanged();
                refeshOnce = true;
            };

            asym1CheckBox = base.AddUIComponent<UICheckBox>();
            asym1CheckBox.relativePosition = new Vector3(asym0CheckBox.relativePosition.x + 3.5f * SPACING2, asym0CheckBox.relativePosition.y);
            this.asym1CheckBoxText = base.AddUIComponent<UILabel>();
            this.asym1CheckBoxText.relativePosition = new Vector3(asym1CheckBox.relativePosition.x + asym1CheckBox.width + 20f, asym1CheckBox.relativePosition.y);
            asym1CheckBox.height = 16f;
            asym1CheckBox.width = 16f;
            asym1CheckBox.label = this.asym1CheckBoxText;
            asym1CheckBox.text = "Sym+1";
            UISprite uISprite2 = asym1CheckBox.AddUIComponent<UISprite>();
            uISprite2.height = 16f;
            uISprite2.width = 16f;
            uISprite2.relativePosition = new Vector3(0f, 0f);
            uISprite2.spriteName = "check-unchecked";
            uISprite2.isVisible = true;
            UISprite uISprite3 = asym1CheckBox.AddUIComponent<UISprite>();
            uISprite3.height = 16f;
            uISprite3.width = 16f;
            uISprite3.relativePosition = new Vector3(0f, 0f);
            uISprite3.spriteName = "check-checked";
            asym1CheckBox.checkedBoxObject = uISprite3;
            asym1CheckBox.isChecked = (symmetry == 1) ? true : false;
            asym1CheckBox.isEnabled = true;
            asym1CheckBox.isVisible = true;
            asym1CheckBox.canFocus = true;
            asym1CheckBox.isInteractive = true;
            asym1CheckBox.eventClicked += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                asym1CheckBox_OnCheckChanged();
                refeshOnce = true;
            };

            asym2CheckBox = base.AddUIComponent<UICheckBox>();
            asym2CheckBox.relativePosition = new Vector3(asym1CheckBox.relativePosition.x + 3.5f * SPACING2, asym1CheckBox.relativePosition.y);
            this.asym2CheckBoxText = base.AddUIComponent<UILabel>();
            this.asym2CheckBoxText.relativePosition = new Vector3(asym2CheckBox.relativePosition.x + asym2CheckBox.width + 20f, asym2CheckBox.relativePosition.y);
            asym2CheckBox.height = 16f;
            asym2CheckBox.width = 16f;
            asym2CheckBox.label = this.asym2CheckBoxText;
            asym2CheckBox.text = "Sym+2";
            UISprite uISprite4 = asym2CheckBox.AddUIComponent<UISprite>();
            uISprite4.height = 16f;
            uISprite4.width = 16f;
            uISprite4.relativePosition = new Vector3(0f, 0f);
            uISprite4.spriteName = "check-unchecked";
            uISprite4.isVisible = true;
            UISprite uISprite5 = asym2CheckBox.AddUIComponent<UISprite>();
            uISprite5.height = 16f;
            uISprite5.width = 16f;
            uISprite5.relativePosition = new Vector3(0f, 0f);
            uISprite5.spriteName = "check-checked";
            asym2CheckBox.checkedBoxObject = uISprite5;
            asym2CheckBox.isChecked = (symmetry == 2) ? true : false;
            asym2CheckBox.isEnabled = true;
            asym2CheckBox.isVisible = true;
            asym2CheckBox.canFocus = true;
            asym2CheckBox.isInteractive = true;
            asym2CheckBox.eventClicked += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                asym2CheckBox_OnCheckChanged();
                refeshOnce = true;
            };

            uturnLaneCheckBox = base.AddUIComponent<UICheckBox>();
            uturnLaneCheckBox.relativePosition = new Vector3(asym0CheckBox.relativePosition.x, asym0CheckBox.relativePosition.y + SPACING);
            this.uturnLaneCheckBoxText = base.AddUIComponent<UILabel>();
            this.uturnLaneCheckBoxText.relativePosition = new Vector3(uturnLaneCheckBox.relativePosition.x + uturnLaneCheckBox.width + 20f, uturnLaneCheckBox.relativePosition.y);
            uturnLaneCheckBox.height = 16f;
            uturnLaneCheckBox.width = 16f;
            uturnLaneCheckBox.label = this.uturnLaneCheckBoxText;
            uturnLaneCheckBox.text = "UTurn";
            UISprite uISprite6 = uturnLaneCheckBox.AddUIComponent<UISprite>();
            uISprite6.height = 16f;
            uISprite6.width = 16f;
            uISprite6.relativePosition = new Vector3(0f, 0f);
            uISprite6.spriteName = "check-unchecked";
            uISprite6.isVisible = true;
            UISprite uISprite7 = uturnLaneCheckBox.AddUIComponent<UISprite>();
            uISprite7.height = 16f;
            uISprite7.width = 16f;
            uISprite7.relativePosition = new Vector3(0f, 0f);
            uISprite7.spriteName = "check-checked";
            uturnLaneCheckBox.checkedBoxObject = uISprite7;
            uturnLaneCheckBox.isChecked = uturnLane ? true : false;
            uturnLaneCheckBox.isEnabled = true;
            uturnLaneCheckBox.isVisible = true;
            uturnLaneCheckBox.canFocus = true;
            uturnLaneCheckBox.isInteractive = true;
            uturnLaneCheckBox.eventClicked += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                uturnLaneCheckBox_OnCheckChanged();
                refeshOnce = true;
            };

            hasSideWalkCheckBox = base.AddUIComponent<UICheckBox>();
            hasSideWalkCheckBox.relativePosition = new Vector3(uturnLaneCheckBox.relativePosition.x + 3.5f * SPACING2, uturnLaneCheckBox.relativePosition.y);
            this.hasSideWalkCheckBoxText = base.AddUIComponent<UILabel>();
            this.hasSideWalkCheckBoxText.relativePosition = new Vector3(hasSideWalkCheckBox.relativePosition.x + hasSideWalkCheckBox.width + 20f, hasSideWalkCheckBox.relativePosition.y);
            hasSideWalkCheckBox.height = 16f;
            hasSideWalkCheckBox.width = 16f;
            hasSideWalkCheckBox.label = this.hasSideWalkCheckBoxText;
            hasSideWalkCheckBox.text = "SideWalk";
            UISprite uISprite8 = hasSideWalkCheckBox.AddUIComponent<UISprite>();
            uISprite8.height = 16f;
            uISprite8.width = 16f;
            uISprite8.relativePosition = new Vector3(0f, 0f);
            uISprite8.spriteName = "check-unchecked";
            uISprite8.isVisible = true;
            UISprite uISprite9 = hasSideWalkCheckBox.AddUIComponent<UISprite>();
            uISprite9.height = 16f;
            uISprite9.width = 16f;
            uISprite9.relativePosition = new Vector3(0f, 0f);
            uISprite9.spriteName = "check-checked";
            hasSideWalkCheckBox.checkedBoxObject = uISprite9;
            hasSideWalkCheckBox.isChecked = hasSidewalk ? true : false;
            hasSideWalkCheckBox.isEnabled = true;
            hasSideWalkCheckBox.isVisible = true;
            hasSideWalkCheckBox.canFocus = true;
            hasSideWalkCheckBox.isInteractive = true;
            hasSideWalkCheckBox.eventClicked += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                hasSideWalkCheckBox_OnCheckChanged();
                refeshOnce = true;
            };

            m_clearButton = AddUIComponent<UIButton>();
            m_clearButton.normalBgSprite = "ToolbarIconGroup1Nomarl";
            m_clearButton.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            m_clearButton.focusedBgSprite = "ToolbarIconGroup1Focused";
            m_clearButton.pressedBgSprite = "ToolbarIconGroup1Pressed";
            m_clearButton.playAudioEvents = true;
            m_clearButton.text = "Clear";
            m_clearButton.relativePosition = new Vector3(m_swapButton.relativePosition.x, m_swapButton.relativePosition.y + SPACING2);
            m_clearButton.autoSize = true;
            m_clearButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = 0;
                toSelected = 0;
                //symmetry = 255;
                //uturnLane = false;
                //hasSidewalk = false;
                //asym0CheckBox.isChecked = false;
                //asym1CheckBox.isChecked = false;
                //asym2CheckBox.isChecked = false;
                //uturnLaneCheckBox.isChecked = false;
                //hasSideWalkCheckBox.isChecked = false;
                RefreshData();
            };

            /*m_okButton = AddUIComponent<UIButton>();
            m_okButton.normalBgSprite = "ToolbarIconGroup1Nomarl";
            m_okButton.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            m_okButton.focusedBgSprite = "ToolbarIconGroup1Focused";
            m_okButton.pressedBgSprite = "ToolbarIconGroup1Pressed";
            m_okButton.playAudioEvents = true;
            m_okButton.text = "OK";
            m_okButton.textScale = 1.5f;
            m_okButton.relativePosition = new Vector3(m_clearButton.relativePosition.x + SPACING, m_clearButton.relativePosition.y + 2 * SPACING2);
            m_okButton.autoSize = true;
            m_okButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                BuildRoad();
            };*/
        }

        public void asym0CheckBox_OnCheckChanged()
        {
            if (symmetry!=0)
            {
                symmetry = 0;
                asym2CheckBox.isChecked = false;
                asym1CheckBox.isChecked = false;
                asym0CheckBox.isChecked = true;
                //DebugLog.LogToFileOnly("case1" + symmetry.ToString());
            }
            else
            {
                symmetry = 255;
                asym2CheckBox.isChecked = false;
                asym1CheckBox.isChecked = false;
                asym0CheckBox.isChecked = false;
                //DebugLog.LogToFileOnly("case2" + symmetry.ToString());
            }
        }

        public void asym1CheckBox_OnCheckChanged()
        {
            if (symmetry != 1)
            {
                symmetry = 1;
                asym2CheckBox.isChecked = false;
                asym1CheckBox.isChecked = true;
                asym0CheckBox.isChecked = false;
                //DebugLog.LogToFileOnly("case3" + symmetry.ToString());
            }
            else
            {
                symmetry = 255;
                asym2CheckBox.isChecked = false;
                asym1CheckBox.isChecked = false;
                asym0CheckBox.isChecked = false;
                //DebugLog.LogToFileOnly("case4" + symmetry.ToString());
            }
        }

        public void asym2CheckBox_OnCheckChanged()
        {
            if (symmetry != 2)
            {
                symmetry = 2;
                asym2CheckBox.isChecked = true;
                asym1CheckBox.isChecked = false;
                asym0CheckBox.isChecked = false;
                //DebugLog.LogToFileOnly("case5" + symmetry.ToString());
            }
            else
            {
                symmetry = 255;
                asym2CheckBox.isChecked = false;
                asym1CheckBox.isChecked = false;
                asym0CheckBox.isChecked = false;
                //DebugLog.LogToFileOnly("case6" + symmetry.ToString());
            }
        }
        public void hasSideWalkCheckBox_OnCheckChanged()
        {
            if (!hasSidewalk)
            {
                hasSidewalk = true;
                hasSideWalkCheckBox.isChecked = true;
            }
            else
            {
                hasSidewalk = false;
                hasSideWalkCheckBox.isChecked = false;
            }
        }
        public void uturnLaneCheckBox_OnCheckChanged()
        {
            if (!uturnLane)
            {
                uturnLane = true;
                uturnLaneCheckBox.isChecked = true;
            }
            else
            {
                uturnLane = false;
                uturnLaneCheckBox.isChecked = false;
            }
        }
        /*public void BuildRoad()
        {
            m_netTool = ToolsModifierControl.SetTool<NetTool>();
            var m_currentModule = Parser.ModuleNameFromUI(fromSelected, toSelected, symmetry, uturnLane, hasSidewalk);
            var m_prefab = PrefabCollection<NetInfo>.FindLoaded(m_currentModule + "_data");
            m_netTool.m_prefab = PrefabCollection<NetInfo>.FindLoaded(m_currentModule + "_data");
            if (m_prefab != null)
            {
                var reveal = FindRoadInPanel(m_netTool.m_prefab.name);
                if (reveal != null)
                {
                    UIView.Find("TSCloseButton").SimulateClick();
                    DebugLog.LogToFileOnly("[CSUR-UI] Attempting to open panel " + reveal[1].parent.parent.parent.parent.name.Replace("Panel", ""));
                    UIButton rb = UIView.Find("MainToolstrip").Find<UIButton>(reveal[1].parent.parent.parent.parent.name.Replace("Panel", ""));
                    rb.SimulateClick();
                    reveal[0].SimulateClick();
                    reveal[1].SimulateClick();
                    if (!UIView.Find("TSCloseButton").isVisible) DebugLog.LogToFileOnly("Failed");
                }
            }
        }*/

        public List<UIComponent> FindRoadInPanel(string name)
        {
            return FindRoadInPanel(name, 0);
        }

        public List<UIComponent> FindRoadInPanel(string name, int attemptNumber)
        {
            if (NETPICKER_ROADCACHE_STRINGS.Contains(name)) return NETPICKER_ROADCACHE_DICTIONARY[NETPICKER_ROADCACHE_STRINGS.IndexOf(name)];

            List<UIComponent> result = new List<UIComponent>();
            string[] panels = { "RoadsPanel", "PublicTransportPanel", "BeautificationPanel", "LandscapingPanel", "ElectricityPanel" };

            // If this isn't the first attempt at finding the network (in RoadsPanel) then 
            if (attemptNumber > 0) UIView.Find(panels[attemptNumber - 1]).Hide();

            UIView.Find(panels[attemptNumber]).Show();
            DebugLog.LogToFileOnly(panels[attemptNumber]);
            List<UIButton> hide = UIView.Find(panels[attemptNumber]).GetComponentsInChildren<UITabstrip>()[0].GetComponentsInChildren<UIButton>().ToList();

            for (var i = 0; i < hide.Count; i++)
            {
                hide[i].SimulateClick();

                UIPanel testedPanel = null;
                UIComponent GTSContainer = UIView.Find(panels[attemptNumber]).GetComponentsInChildren<UITabContainer>()[0];
                for (var k = 0; k < GTSContainer.GetComponentsInChildren<UIPanel>().ToList().Count; k++)
                {
                    UIPanel t = GTSContainer.GetComponentsInChildren<UIPanel>()[k];
                    if (t.isVisible)
                    {
                        testedPanel = t;
                        break;
                    }
                }
                if (testedPanel == null) return null;

                for (var j = 0; j < testedPanel.GetComponentsInChildren<UIButton>().ToList().Count; j++)
                {
                    UIButton button = testedPanel.GetComponentsInChildren<UIButton>().ToList()[j];
                    //DebugLog.LogToFileOnly("[CSUR-UI] Looking for " + name + " ?= " + button.name + " [" + testedPanel.name + "]");
                    if (!NETPICKER_ROADCACHE_STRINGS.Contains(button.name))
                    {
                        List<UIComponent> cacheBuilder = new List<UIComponent>();
                        cacheBuilder.Add(hide[i]);
                        cacheBuilder.Add(button);
                        NETPICKER_ROADCACHE_STRINGS.Add(button.name);
                        NETPICKER_ROADCACHE_DICTIONARY.Add(cacheBuilder);
                    }
                    if (button.name == name)
                    {
                        result.Add(hide[i]);
                        result.Add(button);
                        UIView.Find(panels[attemptNumber]).Hide();
                        return result;
                    }
                }
            }
            attemptNumber++;
            if (attemptNumber < 5)
            {
                return FindRoadInPanel(name, attemptNumber);
            }
            else
            {
                return null;
            }
        }

        private void RefreshDisplayData()
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            uint num2 = currentFrameIndex & 255u;
            if (refeshOnce)
            {
                if (isVisible)
                {
                    m_title.text = "CSUR_UI";
                    //temp
                    //m_result.text = "fromSelected = " + fromSelected.ToString() + " toSelected = " + toSelected.ToString() + " symmetry = " + symmetry.ToString() + " uturnLane: " + uturnLane.ToString() + " hasSidewalk: " + hasSidewalk.ToString();

                    var m_currentModule = Parser.ModuleNameFromUI(fromSelected, toSelected, symmetry, uturnLane, hasSidewalk);
                    var m_prefab = PrefabCollection<NetInfo>.FindLoaded(m_currentModule + "_Data");
                    if (m_prefab != null)
                    {
                        var reveal = FindRoadInPanel(m_prefab.name);
                        if (reveal != null)
                        {
                            if (!(ToolsModifierControl.toolController.CurrentTool is NetTool))
                            {
                                m_netTool = ToolsModifierControl.SetTool<NetTool>();
                                m_result.text = m_currentModule + "(Can Build)";
                                m_netTool.m_prefab = m_prefab;
                                UIView.Find("TSCloseButton").SimulateClick();
                                //DebugLog.LogToFileOnly("[CSUR-UI] Attempting to open panel " + reveal[1].parent.parent.parent.parent.name.Replace("Panel", ""));
                                UIButton rb = UIView.Find("MainToolstrip").Find<UIButton>(reveal[1].parent.parent.parent.parent.name.Replace("Panel", ""));
                                rb.SimulateClick();
                                reveal[0].SimulateClick();
                                reveal[1].SimulateClick();
                                if (!UIView.Find("TSCloseButton").isVisible) DebugLog.LogToFileOnly("Failed");
                            }
                            else
                            {
                                m_netTool = (NetTool)ToolsModifierControl.toolController.CurrentTool;
                                m_result.text = m_currentModule + "(Can Build)";
                                m_netTool.m_prefab = m_prefab;
                            }
                        }
                        else
                        {
                            ToolsModifierControl.SetTool<DefaultTool>();
                            m_result.text = m_currentModule + "(Cant Find Road)";
                        }
                    }
                    else
                    {
                        ToolsModifierControl.SetTool<DefaultTool>();
                        m_result.text = m_currentModule + "(Lack of Assets)";
                    }
                    m_result.textScale = 1.5f;
                    m_result.relativePosition = new Vector3(SPACING2, 1.5f * SPACING2 + m_toIntButtons[0].relativePosition.y);
                    refeshOnce = false;
                }
            }
        }

        /*private void ProcessVisibility()
        {
            if (base.isVisible)
            {
                Show();
            }
            else if (isBuildingRoad && !(ToolsModifierControl.toolController.CurrentTool is NetTool))
            {
                isBuildingRoad = false;
                Show();
            }
            else
            {
                Hide();
            }
        }*/

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

            refeshOnce = true;
        }

    }
}
