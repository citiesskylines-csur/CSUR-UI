using ICities;
using System.IO;
using ColossalFramework.UI;
using ColossalFramework;
using System;
using CSUR_UI.Util;
using CSUR_UI.UI;

namespace CSUR_UI
{
    public class CSUR_UI : IUserMod
    {
        public static bool IsEnabled = false;
        public string Name
        {
            get { return "CSUR UI"; }
        }
        public string Description
        {
            get { return "UI for CSUR Road Selector"; }
        }
        public void OnEnabled()
        {
            IsEnabled = true;
            FileStream fs = File.Create("CSUR_UI.txt");
            fs.Close();
        }
        public void OnDisabled()
        {
            IsEnabled = false;
        }
        public CSUR_UI()
        {
            try
            {
                if (GameSettings.FindSettingsFileByName("CSUR_UI_SETTING") == null)
                {
                    // Creating setting file 
                    GameSettings.AddSettingsFile(new SettingsFile { fileName = "CSUR_UI_SETTING" });
                }
            }
            catch (Exception)
            {
                DebugLog.LogToFileOnly("Could not load/create the setting file.");
            }
        }
        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionUI.makeSettings(helper);
        }
    }
}
