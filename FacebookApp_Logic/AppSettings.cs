using System.Xml.Serialization;
using System.IO;
using System.Drawing;

namespace FacebookApp_Logic
{
    public class AppSettings
    {
        private const string m_AppSettingsFileName = "appSettings.xml";
        private static AppSettings m_Instance;

        public static AppSettings LoadFromFile()
        {
            if (m_Instance == null)
            {
                FileStream fileStream;

                if (File.Exists(m_AppSettingsFileName))
                {
                    using (fileStream = new FileStream(m_AppSettingsFileName, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                        m_Instance = serializer.Deserialize(fileStream) as AppSettings;
                    }
                }
                else
                {
                    m_Instance = new AppSettings();
                }
            }

            return m_Instance;
        }

        public string AccessToken { get; set; }

        public Point LastWindowLocation { get; set; }

        public string LastSelectedThemeURL { get; set; } = "ThemeFeathers.jpg";

        private AppSettings()
        {
        }
  
        public void SaveToFile()
        {
            FileStream fileStream = null;

            try
            {
                if (File.Exists(m_AppSettingsFileName))
                {
                    fileStream = new FileStream(m_AppSettingsFileName, FileMode.Truncate);
                }
                else
                {
                    fileStream = new FileStream(m_AppSettingsFileName, FileMode.Create);
                }

                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(fileStream, this);
            }
            finally
            {
                if(fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }
    }
}
