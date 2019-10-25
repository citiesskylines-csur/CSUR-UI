﻿using ColossalFramework.Plugins;
using ColossalFramework.UI;
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
                if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                {
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
            if (CurrentLoadMode == LoadMode.LoadGame || CurrentLoadMode == LoadMode.NewGame)
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
            if (m_atlasLoaded)
            {
                var spriteSuccess = true;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(2, 2), new Vector2(39, 39)), "0P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(43, 2), new Vector2(39, 39)), "0P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(84, 2), new Vector2(39, 39)), "0P_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(125, 2), new Vector2(39, 39)), "1_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(166, 2), new Vector2(39, 39)), "1_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(207, 2), new Vector2(39, 39)), "1_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(248, 2), new Vector2(39, 39)), "1P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(289, 2), new Vector2(39, 39)), "1P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(330, 2), new Vector2(39, 39)), "1P_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(371, 2), new Vector2(39, 39)), "2_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(412, 2), new Vector2(39, 39)), "2_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(453, 2), new Vector2(39, 39)), "2_L", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(494, 2), new Vector2(39, 39)), "2P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(535, 2), new Vector2(39, 39)), "2P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(576, 2), new Vector2(39, 39)), "3_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(617, 2), new Vector2(39, 39)), "3_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(658, 2), new Vector2(39, 39)), "3P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(699, 2), new Vector2(39, 39)), "3P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(740, 2), new Vector2(39, 39)), "4_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(781, 2), new Vector2(39, 39)), "4_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(822, 2), new Vector2(39, 39)), "4P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(863, 2), new Vector2(39, 39)), "4P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(904, 2), new Vector2(39, 39)), "5_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(945, 2), new Vector2(39, 39)), "5_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(986, 2), new Vector2(39, 39)), "5P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1027, 2), new Vector2(39, 39)), "5P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1068, 2), new Vector2(39, 39)), "6_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1109, 2), new Vector2(39, 39)), "6_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1150, 2), new Vector2(39, 39)), "6P_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1191, 2), new Vector2(39, 39)), "6P_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1232, 2), new Vector2(39, 39)), "7_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1274, 2), new Vector2(39, 39)), "7_R", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1315, 2), new Vector2(39, 39)), "C_S", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1355, 2), new Vector2(39, 39)), "C_C", m_atlasName)
                             && spriteSuccess;
                if (!spriteSuccess) DebugLog.LogToFileOnly("Some sprites haven't been loaded. This is abnormal; you should probably report this to the mod creator.");
            }
            else DebugLog.LogToFileOnly("The texture atlas (provides custom icons) has not loaded. All icons have reverted to text prompts.");
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