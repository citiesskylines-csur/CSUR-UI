﻿using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CSUR_UI.UI
{
    public class OptionUI : MonoBehaviour
    {
        public static bool isShortCutsToPanel = false;
        public static bool isMutuallyExclude = false;
        public static void makeSettings(UIHelperBase helper)
        {
            // tabbing code is borrowed from RushHour mod
            // https://github.com/PropaneDragon/RushHour/blob/release/RushHour/Options/OptionHandler.cs
            LoadSetting();
            UIHelper actualHelper = helper as UIHelper;
            UIComponent container = actualHelper.self as UIComponent;

            UITabstrip tabStrip = container.AddUIComponent<UITabstrip>();
            tabStrip.relativePosition = new Vector3(0, 0);
            tabStrip.size = new Vector2(container.width - 20, 40);

            UITabContainer tabContainer = container.AddUIComponent<UITabContainer>();
            tabContainer.relativePosition = new Vector3(0, 40);
            tabContainer.size = new Vector2(container.width - 20, container.height - tabStrip.height - 20);
            tabStrip.tabPages = tabContainer;

            int tabIndex = 0;
            // Lane_ShortCut

            AddOptionTab(tabStrip, "Lane ShortCut");
            tabStrip.selectedIndex = tabIndex;

            UIPanel currentPanel = tabStrip.tabContainer.components[tabIndex] as UIPanel;
            currentPanel.autoLayout = true;
            currentPanel.autoLayoutDirection = LayoutDirection.Vertical;
            currentPanel.autoLayoutPadding.top = 5;
            currentPanel.autoLayoutPadding.left = 10;
            currentPanel.autoLayoutPadding.right = 10;

            UIHelper panelHelper = new UIHelper(currentPanel);

            var generalGroup = panelHelper.AddGroup("Lane Button ShortCut") as UIHelper;
            var panel = generalGroup.self as UIPanel;

            panel.gameObject.AddComponent<OptionsKeymappingLane>();

            var generalGroup1 = panelHelper.AddGroup("ShortCuts Control") as UIHelper;
            generalGroup1.AddCheckbox("ShortCuts will be used for ToPanel Button", isShortCutsToPanel, (index) => isShortCutsToPanelEnable(index));
            generalGroup1.AddCheckbox("N/NP/N+1 is mutually excluded(select this if you never use ShortCuts)", isMutuallyExclude, (index) => isMutuallyExcludeEnable(index));
            SaveSetting();

            // Function_ShortCut
            ++tabIndex;

            AddOptionTab(tabStrip, "Function ShortCut");
            tabStrip.selectedIndex = tabIndex;

            currentPanel = tabStrip.tabContainer.components[tabIndex] as UIPanel;
            currentPanel.autoLayout = true;
            currentPanel.autoLayoutDirection = LayoutDirection.Vertical;
            currentPanel.autoLayoutPadding.top = 5;
            currentPanel.autoLayoutPadding.left = 10;
            currentPanel.autoLayoutPadding.right = 10;

            panelHelper = new UIHelper(currentPanel);

            generalGroup = panelHelper.AddGroup("Function Button ShortCut") as UIHelper;
            panel = generalGroup.self as UIPanel;

            panel.gameObject.AddComponent<OptionsKeymappingFunction>();

            // Raod Tool ShortCut
            ++tabIndex;

            AddOptionTab(tabStrip, "Road Tool ShortCut");
            tabStrip.selectedIndex = tabIndex;

            currentPanel = tabStrip.tabContainer.components[tabIndex] as UIPanel;
            currentPanel.autoLayout = true;
            currentPanel.autoLayoutDirection = LayoutDirection.Vertical;
            currentPanel.autoLayoutPadding.top = 5;
            currentPanel.autoLayoutPadding.left = 10;
            currentPanel.autoLayoutPadding.right = 10;

            panelHelper = new UIHelper(currentPanel);

            generalGroup = panelHelper.AddGroup("Advanced Road Tool(BETA) ShortCut") as UIHelper;
            panel = generalGroup.self as UIPanel;

            panel.gameObject.AddComponent<OptionsKeymappingRoadTool>();
        }
        private static UIButton AddOptionTab(UITabstrip tabStrip, string caption)
        {
            UIButton tabButton = tabStrip.AddTab(caption);

            tabButton.normalBgSprite = "SubBarButtonBase";
            tabButton.disabledBgSprite = "SubBarButtonBaseDisabled";
            tabButton.focusedBgSprite = "SubBarButtonBaseFocused";
            tabButton.hoveredBgSprite = "SubBarButtonBaseHovered";
            tabButton.pressedBgSprite = "SubBarButtonBasePressed";

            tabButton.textPadding = new RectOffset(10, 10, 10, 10);
            tabButton.autoSize = true;
            tabButton.tooltip = caption;

            return tabButton;
        }

        public static void SaveSetting()
        {
            //save langugae
            FileStream fs = File.Create("CSUR_UI_setting.txt");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine(isShortCutsToPanel);
            streamWriter.WriteLine(isMutuallyExclude);
            streamWriter.Flush();
            fs.Close();
        }

        public static void LoadSetting()
        {
            if (File.Exists("CSUR_UI_setting.txt"))
            {
                FileStream fs = new FileStream("CSUR_UI_setting.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string strLine = sr.ReadLine();

                if (strLine == "True")
                {
                    isShortCutsToPanel = true;
                }
                else
                {
                    isShortCutsToPanel = false;
                }

                strLine = sr.ReadLine();
                if (strLine == "True")
                {
                    isMutuallyExclude = true;
                }
                else
                {
                    isMutuallyExclude = false;
                }
                sr.Close();
                fs.Close();
            }
        }
        public static void isShortCutsToPanelEnable(bool index)
        {
            isShortCutsToPanel = index;
            SaveSetting();
        }
        public static void isMutuallyExcludeEnable(bool index)
        {
            isMutuallyExclude = index;
            SaveSetting();
        }
    }
}
