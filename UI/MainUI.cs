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
        private static readonly float WIDTH = 900f;
        private static readonly float HEIGHT = 780f;
        private static readonly float HEADER = 40f;
        private static readonly float SPACING = 25f;
        private static readonly float SPACING2 = 50f;
        private UIDragHandle m_DragHandler;
        private UIButton m_closeButton;
        private UILabel m_title;
        private UIButton m_okButton;
        private UIButton m_1pLButton;
        private UIButton m_0pLButton;
        private UIButton m_0pRButton;
        private UIButton m_1pRButton;
        private UIButton m_2pRButton;
        private UIButton m_3pRButton;
        private UIButton m_4pRButton;
        private UIButton m_5pRButton;
        private UIButton m_6pRButton;

        public NetInfo m_netInfo;
        public NetTool m_netTool;
        // road cache
        public List<string> NETPICKER_ROADCACHE_STRINGS = new List<string>();
        public List<List<UIComponent>> NETPICKER_ROADCACHE_DICTIONARY = new List<List<UIComponent>>();
        public static bool refeshOnce = false;

        public override void Update()
        {
            RefreshDisplayData();
            base.Update();
        }

        public override void Start()
        {
            base.Start();
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
            RefreshDisplayData();
        }

        private void ShowOnGui()
        {
            m_okButton = AddUIComponent<UIButton>();
            m_okButton.normalBgSprite = "ToolbarIconGroup1Nomarl";
            m_okButton.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            m_okButton.focusedBgSprite = "ToolbarIconGroup1Focused";
            m_okButton.pressedBgSprite = "ToolbarIconGroup1Pressed";
            m_okButton.playAudioEvents = true;
            m_okButton.text = "OK";
            m_okButton.relativePosition = new Vector3(SPACING, 500f);
            m_okButton.autoSize = true;
            m_okButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                BuildRoad();
                Hide();
            };

            m_1pLButton = AddUIComponent<UIButton>();
            m_1pLButton.playAudioEvents = true;
            m_1pLButton.relativePosition = new Vector3(SPACING2, SPACING2);
            m_1pLButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1pLButton.normalBgSprite = "1P_L";
            m_1pLButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1pLButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_1pLButton.normalBgSprite = "1P_S";
            };

            m_0pLButton = AddUIComponent<UIButton>();
            m_0pLButton.playAudioEvents = true;
            m_0pLButton.relativePosition = new Vector3(m_1pLButton.relativePosition.x + SPACING2, SPACING2);
            m_0pLButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_0pLButton.normalBgSprite = "0P_L";
            m_0pLButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_0pLButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_0pLButton.normalBgSprite = "0P_S";
            };

            m_0pRButton = AddUIComponent<UIButton>();
            m_0pRButton.playAudioEvents = true;
            m_0pRButton.relativePosition = new Vector3(m_0pLButton.relativePosition.x + SPACING2, SPACING2);
            m_0pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_0pRButton.normalBgSprite = "0P_R";
            m_0pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_0pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_0pRButton.normalBgSprite = "0P_S";
            };

            m_1pRButton = AddUIComponent<UIButton>();
            m_1pRButton.playAudioEvents = true;
            m_1pRButton.relativePosition = new Vector3(m_0pRButton.relativePosition.x + SPACING2, SPACING2);
            m_1pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1pRButton.normalBgSprite = "1P_R";
            m_1pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_1pRButton.normalBgSprite = "1P_S";
            };

            m_2pRButton = AddUIComponent<UIButton>();
            m_2pRButton.playAudioEvents = true;
            m_2pRButton.relativePosition = new Vector3(m_1pRButton.relativePosition.x + SPACING2, SPACING2);
            m_2pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_2pRButton.normalBgSprite = "2P_R";
            m_2pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_2pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_2pRButton.normalBgSprite = "2P_S";
            };

            m_3pRButton = AddUIComponent<UIButton>();
            m_3pRButton.playAudioEvents = true;
            m_3pRButton.relativePosition = new Vector3(m_2pRButton.relativePosition.x + SPACING2, SPACING2);
            m_3pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_3pRButton.normalBgSprite = "3P_R";
            m_3pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_3pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_3pRButton.normalBgSprite = "3P_S";
            };

            m_4pRButton = AddUIComponent<UIButton>();
            m_4pRButton.playAudioEvents = true;
            m_4pRButton.relativePosition = new Vector3(m_3pRButton.relativePosition.x + SPACING2, SPACING2);
            m_4pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_4pRButton.normalBgSprite = "4P_R";
            m_4pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_4pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_4pRButton.normalBgSprite = "4P_S";
            };

            m_5pRButton = AddUIComponent<UIButton>();
            m_5pRButton.playAudioEvents = true;
            m_5pRButton.relativePosition = new Vector3(m_4pRButton.relativePosition.x + SPACING2, SPACING2);
            m_5pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_5pRButton.normalBgSprite = "5P_R";
            m_5pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_5pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_5pRButton.normalBgSprite = "5P_S";
            };

            m_6pRButton = AddUIComponent<UIButton>();
            m_6pRButton.playAudioEvents = true;
            m_6pRButton.relativePosition = new Vector3(m_5pRButton.relativePosition.x + SPACING2, SPACING2);
            m_6pRButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_6pRButton.normalBgSprite = "6P_R";
            m_6pRButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_6pRButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                m_6pRButton.normalBgSprite = "6P_S";
            };
        }

        public void BuildRoad()
        {
            m_netTool = ToolsModifierControl.SetTool<NetTool>();
            m_netTool.m_prefab = PrefabCollection<NetInfo>.FindLoaded("CSUR St 2 1R1R_Data");
            var reveal = FindRoadInPanel(m_netTool.m_prefab.name);
            UIView.Find("TSCloseButton").SimulateClick();
            DebugLog.LogToFileOnly("[CSUR-UI] Attempting to open panel " + reveal[1].parent.parent.parent.parent.name.Replace("Panel", ""));
            UIButton rb = UIView.Find("MainToolstrip").Find<UIButton>(reveal[1].parent.parent.parent.parent.name.Replace("Panel", ""));
            rb.SimulateClick();
            reveal[0].SimulateClick();
            reveal[1].SimulateClick();
            if (!UIView.Find("TSCloseButton").isVisible) DebugLog.LogToFileOnly("Failed");
        }

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
                    DebugLog.LogToFileOnly("[CSUR-UI] Looking for " + name + " ?= " + button.name + " [" + testedPanel.name + "]");
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
                    process_data();
                    m_title.text = "CSUR_UI";
                    refeshOnce = false;
                }
            }
        }

        private void ProcessVisibility()
        {
            if (!isVisible)
            {
                refeshOnce = true;
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void process_data()
        {
        }
    }
}
