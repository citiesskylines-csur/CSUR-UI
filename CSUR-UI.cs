using ICities;
using System.IO;
using ColossalFramework.UI;

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
            get { return "UI for CSUR"; }
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
    }
}
