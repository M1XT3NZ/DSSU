﻿using System.Text;
using System.Xml.Linq;

namespace DSSU
{
    internal class XmlHelper
    {
        private static string XmlPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.xml");

        private static string Application = "Application";

        private static XDocument doc;

        private static readonly List<string> ApplicationSettings = new List<string> { "SteamAPIKEY", "DiscordToken"
        };

        public static string DISCORD_TOKEN = "YOUR_BOT_TOKEN";
        public static string STEAM_API_KEY = "YOUR_WEB_API_KEY";

        private XmlHelper()
        {
        }

        public static bool DoesSettingsFileExist()
        {
            try
            {
                if (File.Exists(XmlPath))
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static void SaveAllSettings()
        {
            if (!DoesSettingsFileExist())
                CreateSettingsFile();

            foreach (var item in ApplicationSettings)
            {
                switch (item)
                {
                    case "SteamAPIKEY":
                        CreateSettingIfDoesNotExist(item);
                        doc.Save(XmlPath);
                        break;

                    case "DiscordToken":
                        CreateSettingIfDoesNotExist(item);
                        doc.Save(XmlPath);
                        break;
                }
            }
            doc.Save(XmlPath);
        }

        public static void CreateSettingsFile()
        {
            doc =
                new XDocument(
                    new XDeclaration("1.0", Encoding.UTF8.HeaderName, string.Empty),
                    new XElement("Settings",
                        new XElement("Application",
                            new XElement("SteamAPIKEY", "YOUR_API_KEY_HERE"),
                            new XComment("//Steam Web API Key: https://steamcommunity.com/dev/apikey"),
                            new XComment("DiscordToken is the Token from your bot"),
                            new XElement("DiscordToken", "YOUR_TOKEN_HERE")

                        )
                    )
                );
            doc.Save(XmlPath);
        }

        /// <summary>
        /// Gets the Application setting in the settings.xml,Returns null if empty
        /// </summary>
        /// <param name="ChildElement">Element of which it should get the value from</param>
        /// <returns></returns>
        public static string? GetApplicationSetting(string ChildElement)
        {
            if (ChildElement == null)
                return null;

            var xElement = XDocument.Load(XmlPath).Root;
            if (xElement != null && xElement.Element(Application) != null)
            {
                var AppElement = xElement.Element(Application).Elements().FirstOrDefault(p => p.Name == ChildElement);
                ;
                if (AppElement != null && AppElement.Value != null)
                    return AppElement.Value;
            }

            SaveApplicationSetting(ChildElement, string.Empty);
            return null;
        }

        public static void SaveApplicationSetting(string ChildElement, object Value)
        {
            if (Value == null || ChildElement == null)
                return;
            CreateSettingIfDoesNotExist(ChildElement);  //Creates the ChildElement setting if it doesnt exist
            var doc = XDocument.Load(XmlPath).Root;
            if (doc != null && doc.Element(Application) != null)
            {
                var AppElement = doc.Element(Application)?.Elements().FirstOrDefault(p => p.Name == ChildElement)
    ;
                if (AppElement != null && AppElement.Value != null && Value != null && Value.ToString() != null)
                    AppElement.Value = Value.ToString();
                doc.Save(XmlPath);
            }
        }

        public static async void CreateSettingIfDoesNotExist(string ChildElement, bool IsApplicationSetting = false)
        {
            XElement? Element = doc.Root?.Element(Application)?.Elements().FirstOrDefault(p => p.Name == ChildElement);
            if (Element == null)
            {
                doc.Root?.Element(Application)?.Add(new XElement(ChildElement));
                doc.Save(XmlPath);
            }
        }
    }
}