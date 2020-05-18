using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace DawnLauncher
{
    public class Settings
    {
        string appDataDir;
        string jsonPath;
        #region def des settings
        private string m_gamDir;
        public string gameDir
        {
            get => m_gamDir;
            set
            {
                m_gamDir = value;
                updateSettings();
            }
        }
        private string m_savedLogin;
        public string savedLogin
        {
            get => m_savedLogin;
            set
            {
                m_savedLogin = value;
                updateSettings();
            }
        }
        private string m_gameVersion;
        public string gameVersion
        {
            get => m_gameVersion;
            set
            {
                m_gameVersion = value;
                updateSettings();
            }
        }
        #endregion 
        public Settings()
        {
            appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Dawn");
            jsonPath = appDataDir + "\\settings.json";
            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);
            if (!File.Exists(jsonPath))
                updateSettings();
            readSettings();

        }
        public void resetSettings()
        {
            File.Delete(jsonPath);
        }

        ////////////////////
        private void updateSettings()
        {
            SettingObject obj = new SettingObject
            {
                gameDir = gameDir,
                savedLogin = savedLogin,
                gameVersion = gameVersion
            };
            string text = JsonConvert.SerializeObject(obj);
            File.WriteAllText(jsonPath, text);
        }
        private void readSettings()
        {
            dynamic result;
            try
            {
                result = JsonConvert.DeserializeObject(File.ReadAllText(jsonPath));
            }
            catch
            {
                result = JsonConvert.DeserializeObject("{}");
            }
            gameDir = result.gameDir;
            savedLogin = result.savedLogin;
            gameVersion = result.gameVersion;
        }

        private class SettingObject
        {
            public string gameDir;
            public string savedLogin;
            public string gameVersion;
        }
    }

}
