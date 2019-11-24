using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CSUR_UI.NewData;
using CSUR_UI.UI;
using CSUR_UI.Util;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CSUR_UI
{
    public class Loader : LoadingExtensionBase
    {
        public static UIView parentGuiView;
        public static MainUI mainUI;
        public static LoadMode CurrentLoadMode;
        public static bool isGuiRunning = false;
        public static MainButton mainButton;
        public static string m_atlasName = "CSUR_UI";
        public static string m_atlasName1 = "CSUR_UI1";
        public static string m_atlasName2 = "CSUR_UI2";
        public static string m_atlasNameHeader = "CSUR_UI_Header";
        public static string m_atlasNameBg = "CSUR_UI_Bg";
        public static string m_atlasNameNoAsset = "CSUR_UI_NoAssert";
        public static bool m_atlasLoaded;
        public static bool Done { get; private set; } // Only one Assets installation throughout the application


        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
        }
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CurrentLoadMode = mode;
            if (CSUR_UI.IsEnabled)
            {
                if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewMap || mode == LoadMode.LoadMap || mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
                {
                    OptionUI.LoadSetting();
                    SetupGui();
                    DebugLog.LogToFileOnly("OnLevelLoaded");
                    if (mode == LoadMode.NewGame)
                    {
                        //InitData();
                        DebugLog.LogToFileOnly("InitData");
                    }
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (CurrentLoadMode == LoadMode.LoadGame || CurrentLoadMode == LoadMode.NewGame || CurrentLoadMode == LoadMode.LoadMap || CurrentLoadMode == LoadMode.NewMap || CurrentLoadMode == LoadMode.LoadAsset || CurrentLoadMode == LoadMode.NewAsset)
            {
                if (CSUR_UI.IsEnabled && isGuiRunning)
                {
                    RemoveGui();
                }
            }
        }

        private static void LoadSprites()
        {
            if (SpriteUtilities.GetAtlas(m_atlasName) != null) return;
            var modPath = PluginManager.instance.FindPluginInfo(Assembly.GetExecutingAssembly()).modPath;
            m_atlasLoaded = SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Resources/CSUR.png"), m_atlasName);
            m_atlasLoaded &= SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Resources/CSUR1.png"), m_atlasName1);
            m_atlasLoaded &= SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Resources/CSUR2.png"), m_atlasName2);
            m_atlasLoaded &= SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Resources/UIBG.png"), m_atlasNameBg);
            m_atlasLoaded &= SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Resources/UITOP.png"), m_atlasNameHeader);
            m_atlasLoaded &= SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Resources/Notfound.png"), m_atlasNameNoAsset);
            if (m_atlasLoaded)
            {
                var spriteSuccess = true;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 2), new Vector2(30, 30)), "0P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(34, 2), new Vector2(30, 30)), "0P_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(66, 2), new Vector2(30, 30)), "0P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(98, 2), new Vector2(30, 30)), "1_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(130, 2), new Vector2(30, 30)), "1_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(162, 2), new Vector2(30, 30)), "1_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(194, 2), new Vector2(30, 30)), "1P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(226, 2), new Vector2(30, 30)), "1P_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(258, 2), new Vector2(30, 30)), "1P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(290, 2), new Vector2(30, 30)), "2_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(322, 2), new Vector2(30, 30)), "2_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(354, 2), new Vector2(30, 30)), "2_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(386, 2), new Vector2(30, 30)), "2P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(418, 2), new Vector2(30, 30)), "2P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(452, 2), new Vector2(30, 30)), "3_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(482, 2), new Vector2(30, 30)), "3_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(514, 2), new Vector2(30, 30)), "3P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(546, 2), new Vector2(30, 30)), "3P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(578, 2), new Vector2(30, 30)), "4_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(610, 2), new Vector2(30, 30)), "4_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(642, 2), new Vector2(30, 30)), "4P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(674, 2), new Vector2(30, 30)), "4P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(706, 2), new Vector2(30, 30)), "5_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(738, 2), new Vector2(30, 30)), "5_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(770, 2), new Vector2(30, 30)), "5P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(802, 2), new Vector2(30, 30)), "5P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(834, 2), new Vector2(30, 30)), "6_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(866, 2), new Vector2(30, 30)), "6_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(898, 2), new Vector2(30, 30)), "6P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(930, 2), new Vector2(30, 30)), "6P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(962, 2), new Vector2(30, 30)), "7_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(994, 2), new Vector2(30, 30)), "7_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1026, 2), new Vector2(30, 30)), "C_C", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1058, 2), new Vector2(30, 30)), "C_S", m_atlasName)
                             && spriteSuccess;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 2), new Vector2(30, 30)), "+0", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(34, 2), new Vector2(30, 30)), "+1", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(66, 2), new Vector2(30, 30)), "+2", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(98, 2), new Vector2(30, 30)), "SWAP", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(130, 2), new Vector2(30, 30)), "SWAP_S", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(162, 2), new Vector2(30, 30)), "SIDEWALK", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(194, 2), new Vector2(30, 30)), "0_S", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(226, 2), new Vector2(30, 30)), "0", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(258, 2), new Vector2(30, 30)), "COPY", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(290, 2), new Vector2(30, 30)), "COPY_S", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(322, 2), new Vector2(30, 30)), "UTURN", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(354, 2), new Vector2(30, 30)), "UTURN_S", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(386, 2), new Vector2(30, 30)), "NOSIDEWALK", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(418, 2), new Vector2(30, 30)), "CLEAR", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(450, 2), new Vector2(30, 30)), "CLEAR_S", m_atlasName1)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(482, 2), new Vector2(30, 30)), "BIKE", m_atlasName1)
                             && spriteSuccess;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 2), new Vector2(60, 50)), "CSUR_BUTTON_S", m_atlasName2)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(64, 2), new Vector2(60, 50)), "CSUR_BUTTON", m_atlasName2)
                             && spriteSuccess;
                spriteSuccess &= SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(0, 0), new Vector2(566, 210)), "UIBG", m_atlasNameBg);
                spriteSuccess &= SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(0, 0), new Vector2(565, 35)), "UITOP", m_atlasNameHeader);
                spriteSuccess &= SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(0, 0), new Vector2(150, 150)), "NOASSET", m_atlasNameNoAsset);
                if (!spriteSuccess) DebugLog.LogToFileOnly("Error: Some sprites haven't been loaded. This is abnormal; you should probably report this to the mod creator.");
            }
            else DebugLog.LogToFileOnly("Error: The texture atlas (provides custom icons) has not loaded. All icons have reverted to text prompts.");
        }
        public static void SetupGui()
        {
            LoadSprites();
            if (m_atlasLoaded)
            {
                parentGuiView = null;
                parentGuiView = UIView.GetAView();

                if (mainUI == null)
                {
                    mainUI = (MainUI)parentGuiView.AddUIComponent(typeof(MainUI));
                }

                SetupMainButton();
                isGuiRunning = true;
            }
        }

        public static void SetupMainButton()
        {
            if (mainButton == null)
            {
                mainButton = (parentGuiView.AddUIComponent(typeof(MainButton)) as MainButton);
            }
            mainButton.Show();
        }

        public static void RemoveGui()
        {
            isGuiRunning = false;
            if (parentGuiView != null)
            {
                parentGuiView = null;
                UnityEngine.Object.Destroy(mainUI);
                UnityEngine.Object.Destroy(mainButton);
                mainUI = null;
                mainButton = null;
            }
        }
    }
}
