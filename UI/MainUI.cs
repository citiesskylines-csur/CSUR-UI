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
        private static readonly float WIDTH = 640f;
        private static readonly float HEIGHT = 420f;
        private static readonly float HEADER = 40f;
        private static readonly float SPACING = 25f;
        private static readonly float SPACING2 = 50f;
        private UIDragHandle m_DragHandler;
        private UIButton m_closeButton;
        private UILabel m_title;
        private UIButton m_okButton;
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

        private UILabel m_result;

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
            m_1pLButton = AddUIComponent<UIButton>();
            m_1pLButton.playAudioEvents = true;
            m_1pLButton.relativePosition = new Vector3(SPACING2, SPACING2);
            m_1pLButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1pLButton.normalBgSprite = "1P_L";
            m_1pLButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1pLButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 1) == 0) ? (fromSelected | 1) : (fromSelected & ~(1));
                RefreshData();
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
                fromSelected = ((fromSelected & 2) == 0) ? (fromSelected | 2) : (fromSelected & ~(2));
                RefreshData();
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
                fromSelected = ((fromSelected & 4) == 0) ? (fromSelected | 4) : (fromSelected & ~(4));
                RefreshData();
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
                fromSelected = ((fromSelected & 8) == 0) ? (fromSelected | 8) : (fromSelected & ~(8));
                RefreshData();
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
                fromSelected = ((fromSelected & 16) == 0) ? (fromSelected | 16) : (fromSelected & ~(16));
                RefreshData();
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
                fromSelected = ((fromSelected & 32) == 0) ? (fromSelected | 32) : (fromSelected & ~(32));
                RefreshData();
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
                fromSelected = ((fromSelected & 64) == 0) ? (fromSelected | 64) : (fromSelected & ~(64));
                RefreshData();
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
                fromSelected = ((fromSelected & 128) == 0) ? (fromSelected | 128) : (fromSelected & ~(128));
                RefreshData();
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
                fromSelected = ((fromSelected & 256) == 0) ? (fromSelected | 256) : (fromSelected & ~(256));
                RefreshData();
            };

            m_2LButton = AddUIComponent<UIButton>();
            m_2LButton.playAudioEvents = true;
            m_2LButton.relativePosition = new Vector3(SPACING, m_1pLButton.relativePosition.y + SPACING2);
            m_2LButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_2LButton.normalBgSprite = "2_L";
            m_2LButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_2LButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 512) == 0) ? (fromSelected | 512) : (fromSelected & ~(512));
                RefreshData();
            };

            m_1LButton = AddUIComponent<UIButton>();
            m_1LButton.playAudioEvents = true;
            m_1LButton.relativePosition = new Vector3(m_2LButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_1LButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1LButton.normalBgSprite = "1_L";
            m_1LButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1LButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 1024) == 0) ? (fromSelected | 1024) : (fromSelected & ~(1024));
                RefreshData();
            };

            m_CButton = AddUIComponent<UIButton>();
            m_CButton.playAudioEvents = true;
            m_CButton.relativePosition = new Vector3(m_1LButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_CButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_CButton.normalBgSprite = "C_C";
            m_CButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_CButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 2048) == 0) ? (fromSelected | 2048) : (fromSelected & ~(2048));
                RefreshData();
            };

            m_1RButton = AddUIComponent<UIButton>();
            m_1RButton.playAudioEvents = true;
            m_1RButton.relativePosition = new Vector3(m_CButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_1RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1RButton.normalBgSprite = "1_R";
            m_1RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 4096) == 0) ? (fromSelected | 4096) : (fromSelected & ~(4096));
                RefreshData();
            };

            m_2RButton = AddUIComponent<UIButton>();
            m_2RButton.playAudioEvents = true;
            m_2RButton.relativePosition = new Vector3(m_1RButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_2RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_2RButton.normalBgSprite = "2_R";
            m_2RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_2RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 8192) == 0) ? (fromSelected | 8192) : (fromSelected & ~(8192));
                RefreshData();
            };

            m_3RButton = AddUIComponent<UIButton>();
            m_3RButton.playAudioEvents = true;
            m_3RButton.relativePosition = new Vector3(m_2RButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_3RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_3RButton.normalBgSprite = "3_R";
            m_3RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_3RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 16384) == 0) ? (fromSelected | 16384) : (fromSelected & ~(16384));
                RefreshData();
            };

            m_4RButton = AddUIComponent<UIButton>();
            m_4RButton.playAudioEvents = true;
            m_4RButton.relativePosition = new Vector3(m_3RButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_4RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_4RButton.normalBgSprite = "4_R";
            m_4RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_4RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 32768) == 0) ? (fromSelected | 32768) : (fromSelected & ~(32768));
                RefreshData();
            };

            m_5RButton = AddUIComponent<UIButton>();
            m_5RButton.playAudioEvents = true;
            m_5RButton.relativePosition = new Vector3(m_4RButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_5RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_5RButton.normalBgSprite = "5_R";
            m_5RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_5RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 65536) == 0) ? (fromSelected | 65536) : (fromSelected & ~(65536));
                RefreshData();
            };

            m_6RButton = AddUIComponent<UIButton>();
            m_6RButton.playAudioEvents = true;
            m_6RButton.relativePosition = new Vector3(m_5RButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_6RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_6RButton.normalBgSprite = "6_R";
            m_6RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_6RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 131072) == 0) ? (fromSelected | 131072) : (fromSelected & ~(131072));
                RefreshData();
            };

            m_7RButton = AddUIComponent<UIButton>();
            m_7RButton.playAudioEvents = true;
            m_7RButton.relativePosition = new Vector3(m_6RButton.relativePosition.x + SPACING2, m_2LButton.relativePosition.y);
            m_7RButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_7RButton.normalBgSprite = "7_R";
            m_7RButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_7RButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = ((fromSelected & 262144) == 0) ? (fromSelected | 262144) : (fromSelected & ~(262144));
                RefreshData();
            };

            //TODO, there should be a text lable
            m_result = AddUIComponent<UILabel>();
            m_result.text = "";
            m_result.textScale = 2f;
            m_result.relativePosition = new Vector3(2f*SPACING2 , 1.5f * SPACING2 + m_2LButton.relativePosition.y);
            m_result.autoSize = true;

            m_2LDButton = AddUIComponent<UIButton>();
            m_2LDButton.playAudioEvents = true;
            m_2LDButton.relativePosition = new Vector3(m_2LButton.relativePosition.x, m_2LButton.relativePosition.y + 3*SPACING2);
            m_2LDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_2LDButton.normalBgSprite = "2_L";
            m_2LDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_2LDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 512) == 0) ? (toSelected | 512) : (toSelected & ~(512));
                RefreshData();
            };

            m_1LDButton = AddUIComponent<UIButton>();
            m_1LDButton.playAudioEvents = true;
            m_1LDButton.relativePosition = new Vector3(m_2LDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_1LDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1LDButton.normalBgSprite = "1_L";
            m_1LDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1LDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 1024) == 0) ? (toSelected | 1024) : (toSelected & ~(1024));
                RefreshData();
            };

            m_CDButton = AddUIComponent<UIButton>();
            m_CDButton.playAudioEvents = true;
            m_CDButton.relativePosition = new Vector3(m_1LDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_CDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_CDButton.normalBgSprite = "C_C";
            m_CDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_CDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 2048) == 0) ? (toSelected | 2048) : (toSelected & ~(2048));
                RefreshData();
            };

            m_1RDButton = AddUIComponent<UIButton>();
            m_1RDButton.playAudioEvents = true;
            m_1RDButton.relativePosition = new Vector3(m_CDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_1RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1RDButton.normalBgSprite = "1_R";
            m_1RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 4096) == 0) ? (toSelected | 4096) : (toSelected & ~(4096));
                RefreshData();
            };

            m_2RDButton = AddUIComponent<UIButton>();
            m_2RDButton.playAudioEvents = true;
            m_2RDButton.relativePosition = new Vector3(m_1RDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_2RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_2RDButton.normalBgSprite = "2_R";
            m_2RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_2RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 8192) == 0) ? (toSelected | 8192) : (toSelected & ~(8192));
                RefreshData();
            };

            m_3RDButton = AddUIComponent<UIButton>();
            m_3RDButton.playAudioEvents = true;
            m_3RDButton.relativePosition = new Vector3(m_2RDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_3RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_3RDButton.normalBgSprite = "3_R";
            m_3RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_3RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 16384) == 0) ? (toSelected | 16384) : (toSelected & ~(16384));
                RefreshData();
            };

            m_4RDButton = AddUIComponent<UIButton>();
            m_4RDButton.playAudioEvents = true;
            m_4RDButton.relativePosition = new Vector3(m_3RDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_4RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_4RDButton.normalBgSprite = "4_R";
            m_4RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_4RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 32768) == 0) ? (toSelected | 32768) : (toSelected & ~(32768));
                RefreshData();
            };

            m_5RDButton = AddUIComponent<UIButton>();
            m_5RDButton.playAudioEvents = true;
            m_5RDButton.relativePosition = new Vector3(m_4RDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_5RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_5RDButton.normalBgSprite = "5_R";
            m_5RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_5RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 65536) == 0) ? (toSelected | 65536) : (toSelected & ~(65536));
                RefreshData();
            };

            m_6RDButton = AddUIComponent<UIButton>();
            m_6RDButton.playAudioEvents = true;
            m_6RDButton.relativePosition = new Vector3(m_5RDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_6RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_6RDButton.normalBgSprite = "6_R";
            m_6RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_6RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 131072) == 0) ? (toSelected | 131072) : (toSelected & ~(131072));
                RefreshData();
            };

            m_7RDButton = AddUIComponent<UIButton>();
            m_7RDButton.playAudioEvents = true;
            m_7RDButton.relativePosition = new Vector3(m_6RDButton.relativePosition.x + SPACING2, m_2LDButton.relativePosition.y);
            m_7RDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_7RDButton.normalBgSprite = "7_R";
            m_7RDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_7RDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 262144) == 0) ? (toSelected | 262144) : (toSelected & ~(262144));
                RefreshData();
            };

            m_1pLDButton = AddUIComponent<UIButton>();
            m_1pLDButton.playAudioEvents = true;
            m_1pLDButton.relativePosition = new Vector3(m_1pLButton.relativePosition.x, m_1pLButton.relativePosition.y + 5*SPACING2);
            m_1pLDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1pLDButton.normalBgSprite = "1P_L";
            m_1pLDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1pLDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 1) == 0) ? (toSelected | 1) : (toSelected & ~(1));
                RefreshData();
            };

            m_0pLDButton = AddUIComponent<UIButton>();
            m_0pLDButton.playAudioEvents = true;
            m_0pLDButton.relativePosition = new Vector3(m_1pLDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_0pLDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_0pLDButton.normalBgSprite = "0P_L";
            m_0pLDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_0pLDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 2) == 0) ? (toSelected | 2) : (toSelected & ~(2));
                RefreshData();
            };

            m_0pRDButton = AddUIComponent<UIButton>();
            m_0pRDButton.playAudioEvents = true;
            m_0pRDButton.relativePosition = new Vector3(m_0pLDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_0pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_0pRDButton.normalBgSprite = "0P_R";
            m_0pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_0pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 4) == 0) ? (toSelected | 4) : (toSelected & ~(4));
                RefreshData();
            };

            m_1pRDButton = AddUIComponent<UIButton>();
            m_1pRDButton.playAudioEvents = true;
            m_1pRDButton.relativePosition = new Vector3(m_0pRDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_1pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_1pRDButton.normalBgSprite = "1P_R";
            m_1pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_1pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 8) == 0) ? (toSelected | 8) : (toSelected & ~(8));
                RefreshData();
            };

            m_2pRDButton = AddUIComponent<UIButton>();
            m_2pRDButton.playAudioEvents = true;
            m_2pRDButton.relativePosition = new Vector3(m_1pRDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_2pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_2pRDButton.normalBgSprite = "2P_R";
            m_2pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_2pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 16) == 0) ? (toSelected | 16) : (toSelected & ~(16));
                RefreshData();
            };

            m_3pRDButton = AddUIComponent<UIButton>();
            m_3pRDButton.playAudioEvents = true;
            m_3pRDButton.relativePosition = new Vector3(m_2pRDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_3pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_3pRDButton.normalBgSprite = "3P_R";
            m_3pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_3pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 32) == 0) ? (toSelected | 32) : (toSelected & ~(32));
                RefreshData();
            };

            m_4pRDButton = AddUIComponent<UIButton>();
            m_4pRDButton.playAudioEvents = true;
            m_4pRDButton.relativePosition = new Vector3(m_3pRDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_4pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_4pRDButton.normalBgSprite = "4P_R";
            m_4pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_4pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 64) == 0) ? (toSelected | 64) : (toSelected & ~(64));
                RefreshData();
            };

            m_5pRDButton = AddUIComponent<UIButton>();
            m_5pRDButton.playAudioEvents = true;
            m_5pRDButton.relativePosition = new Vector3(m_4pRDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_5pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_5pRDButton.normalBgSprite = "5P_R";
            m_5pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_5pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 128) == 0) ? (toSelected | 128) : (toSelected & ~(128));
                RefreshData();
            };

            m_6pRDButton = AddUIComponent<UIButton>();
            m_6pRDButton.playAudioEvents = true;
            m_6pRDButton.relativePosition = new Vector3(m_5pRDButton.relativePosition.x + SPACING2, m_1pLDButton.relativePosition.y);
            m_6pRDButton.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            m_6pRDButton.normalBgSprite = "6P_R";
            m_6pRDButton.size = new Vector2(39f, 39f);
            zOrder = 11;
            m_6pRDButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                toSelected = ((toSelected & 256) == 0) ? (toSelected | 256) : (toSelected & ~(256));
                RefreshData();
            };

            m_copyButton = AddUIComponent<UIButton>();
            m_copyButton.normalBgSprite = "ToolbarIconGroup1Nomarl";
            m_copyButton.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            m_copyButton.focusedBgSprite = "ToolbarIconGroup1Focused";
            m_copyButton.pressedBgSprite = "ToolbarIconGroup1Pressed";
            m_copyButton.playAudioEvents = true;
            m_copyButton.text = "Copy";
            m_copyButton.relativePosition = new Vector3(m_6pRButton.relativePosition.x + 2*SPACING2, m_6pRButton.relativePosition.y + 2*SPACING2);
            m_copyButton.autoSize = true;
            m_copyButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                fromSelected = toSelected;
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

            asym0CheckBox = base.AddUIComponent<UICheckBox>();
            asym0CheckBox.relativePosition = new Vector3(m_1pLDButton.relativePosition.x, m_1pLDButton.relativePosition.y + SPACING2);
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
                symmetry = 255;
                uturnLane = false;
                hasSidewalk = false;
                asym0CheckBox.isChecked = false;
                asym1CheckBox.isChecked = false;
                asym2CheckBox.isChecked = false;
                uturnLaneCheckBox.isChecked = false;
                hasSideWalkCheckBox.isChecked = false;
                RefreshData();
            };

            m_okButton = AddUIComponent<UIButton>();
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
                Hide();
            };
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
        public void BuildRoad()
        {
            m_netTool = ToolsModifierControl.SetTool<NetTool>();
            m_netTool.m_prefab = PrefabCollection<NetInfo>.FindLoaded("CSUR 1R_Data");
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
                    m_title.text = "CSUR_UI";
                    //temp
                    m_result.text = "fromSelected = " + fromSelected.ToString() + " toSelected = " + toSelected.ToString() + " symmetry = " + symmetry.ToString() + " uturnLane: " + uturnLane.ToString() + " hasSidewalk: " + hasSidewalk.ToString();
                    m_result.textScale = 0.7f;
                    m_result.relativePosition = new Vector3(SPACING, 1.5f * SPACING2 + m_2LButton.relativePosition.y);
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

        private void RefreshData()
        {
            m_1pLButton.normalBgSprite = ((fromSelected & 1) == 0) ? "1P_L" : "1P_S";
            m_0pLButton.normalBgSprite = ((fromSelected & 2) == 0) ? "0P_L" : "0P_S";
            m_0pRButton.normalBgSprite = ((fromSelected & 4) == 0) ? "0P_R" : "0P_S";
            m_1pRButton.normalBgSprite = ((fromSelected & 8) == 0) ? "1P_R" : "1P_S";
            m_2pRButton.normalBgSprite = ((fromSelected & 16) == 0) ? "2P_R" : "2P_S";
            m_3pRButton.normalBgSprite = ((fromSelected & 32) == 0) ? "3P_R" : "3P_S";
            m_4pRButton.normalBgSprite = ((fromSelected & 64) == 0) ? "4P_R" : "4P_S";
            m_5pRButton.normalBgSprite = ((fromSelected & 128) == 0) ? "5P_R" : "5P_S";
            m_6pRButton.normalBgSprite = ((fromSelected & 256) == 0) ? "6P_R" : "6P_S";

            m_2LButton.normalBgSprite = ((fromSelected & 512) == 0) ? "2_L" : "2_S";
            m_1LButton.normalBgSprite = ((fromSelected & 1024) == 0) ? "1_L" : "1_S";
            m_CButton.normalBgSprite = ((fromSelected & 2048) == 0) ? "C_C" : "C_S";
            m_1RButton.normalBgSprite = ((fromSelected & 4096) == 0) ? "1_R" : "1_S";
            m_2RButton.normalBgSprite = ((fromSelected & 8192) == 0) ? "2_R" : "2_S";
            m_3RButton.normalBgSprite = ((fromSelected & 16384) == 0) ? "3_R" : "3_S";
            m_4RButton.normalBgSprite = ((fromSelected & 32768) == 0) ? "4_R" : "4_S";
            m_5RButton.normalBgSprite = ((fromSelected & 65536) == 0) ? "5_R" : "5_S";
            m_6RButton.normalBgSprite = ((fromSelected & 131072) == 0) ? "6_R" : "6_S";
            m_7RButton.normalBgSprite = ((fromSelected & 262144) == 0) ? "7_R" : "7_S";

            m_1pLDButton.normalBgSprite = ((toSelected & 1) == 0) ? "1P_L" : "1P_S";
            m_0pLDButton.normalBgSprite = ((toSelected & 2) == 0) ? "0P_L" : "0P_S";
            m_0pRDButton.normalBgSprite = ((toSelected & 4) == 0) ? "0P_R" : "0P_S";
            m_1pRDButton.normalBgSprite = ((toSelected & 8) == 0) ? "1P_R" : "1P_S";
            m_2pRDButton.normalBgSprite = ((toSelected & 16) == 0) ? "2P_R" : "2P_S";
            m_3pRDButton.normalBgSprite = ((toSelected & 32) == 0) ? "3P_R" : "3P_S";
            m_4pRDButton.normalBgSprite = ((toSelected & 64) == 0) ? "4P_R" : "4P_S";
            m_5pRDButton.normalBgSprite = ((toSelected & 128) == 0) ? "5P_R" : "5P_S";
            m_6pRDButton.normalBgSprite = ((toSelected & 256) == 0) ? "6P_R" : "6P_S";

            m_2LDButton.normalBgSprite = ((toSelected & 512) == 0) ? "2_L" : "2_S";
            m_1LDButton.normalBgSprite = ((toSelected & 1024) == 0) ? "1_L" : "1_S";
            m_CDButton.normalBgSprite = ((toSelected & 2048) == 0) ? "C_C" : "C_S";
            m_1RDButton.normalBgSprite = ((toSelected & 4096) == 0) ? "1_R" : "1_S";
            m_2RDButton.normalBgSprite = ((toSelected & 8192) == 0) ? "2_R" : "2_S";
            m_3RDButton.normalBgSprite = ((toSelected & 16384) == 0) ? "3_R" : "3_S";
            m_4RDButton.normalBgSprite = ((toSelected & 32768) == 0) ? "4_R" : "4_S";
            m_5RDButton.normalBgSprite = ((toSelected & 65536) == 0) ? "5_R" : "5_S";
            m_6RDButton.normalBgSprite = ((toSelected & 131072) == 0) ? "6_R" : "6_S";
            m_7RDButton.normalBgSprite = ((toSelected & 262144) == 0) ? "7_R" : "7_S";

            refeshOnce = true;
        }

    }
}
