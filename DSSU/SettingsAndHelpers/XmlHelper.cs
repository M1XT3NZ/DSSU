using System.Text;
using System.Xml.Linq;
using DSSU.SettingsAndHelpers;

namespace DSSU
{
    internal class XmlHelper
    {
        private static string XmlPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.xml");

        private static string Application = "Application";

        private static XDocument doc;

        private static readonly List<string> ApplicationSettings = new List<string> { nameof(Settings.SteamAPIKEY), nameof(Settings.DiscordToken),
            nameof(Settings.HelpCommandTimeout), nameof(Settings.ServerCommandTimeout)
        };

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

        //public static void SaveAllSettings()
        //{
        //    if (!DoesSettingsFileExist())
        //        CreateSettingsFile();

        //    foreach (var item in ApplicationSettings)
        //    {
        //        switch (item)
        //        {
        //            case nameof(Settings.SteamAPIKEY):
        //                CreateSettingIfDoesNotExist(item);
        //                doc.Save(XmlPath);
        //                break;

        //            case nameof(Settings.DiscordToken):
        //                CreateSettingIfDoesNotExist(item);
        //                doc.Save(XmlPath);
        //                break;

        //            case nameof(Settings.HelpCommandTimeout):
        //                CreateSettingIfDoesNotExist(item);
        //                doc.Save(XmlPath);
        //                break;

        //            case nameof(Settings.ServerCommandTimeout):
        //                CreateSettingIfDoesNotExist(item);
        //                doc.Save(XmlPath);
        //                break;
        //        }
        //    }
        //    doc.Save(XmlPath);
        //}

        public static void CreateSettingsFile()
        {
            doc =
                new XDocument(
                    new XDeclaration("1.0", Encoding.UTF8.HeaderName, string.Empty),
                    new XElement("Settings",
                        new XElement("Application",
                            new XElement(nameof(Settings.SteamAPIKEY), Settings.SteamAPIKEY),
                            new XComment("//Steam Web API Key: https://steamcommunity.com/dev/apikey"),
                            new XComment("DiscordToken is the Token from your bot: https://discord.com/developers/applications "),
                            new XElement(nameof(Settings.DiscordToken), Settings.DiscordToken),
                            new XComment("Value for the Timeout is in minutes so, 5 is 5 Minutes"),
                            new XElement(nameof(Settings.HelpCommandTimeout), Settings.HelpCommandTimeout),
                            new XElement(nameof(Settings.ServerCommandTimeout), Settings.ServerCommandTimeout)
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
        public static object? GetApplicationSetting(string ChildElement)
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
            CreateSettingIfDoesNotExist(ChildElement, Value);  //Creates the ChildElement setting if it doesnt exist
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

        public static void CreateSettingIfDoesNotExist(string ChildElement, object value)
        {
            doc = XDocument.Load(XmlPath);
            XElement? Element = doc.Root?.Element(Application)?.Elements().FirstOrDefault(p => p.Name == ChildElement);
            if (Element == null)
            {
                doc.Root?.Element(Application)?.Add(new XElement(ChildElement, value));
                doc.Save(XmlPath);
            }
        }

        public static void CheckIfSettingsExists()
        {
            if (!DoesSettingsFileExist())
                CreateSettingsFile();

            foreach (var item in ApplicationSettings)
            {
                switch (item)
                {
                    case nameof(Settings.SteamAPIKEY):
                        CreateSettingIfDoesNotExist(item, Settings.SteamAPIKEY);
                        doc.Save(XmlPath);
                        break;

                    case nameof(Settings.DiscordToken):
                        CreateSettingIfDoesNotExist(item, Settings.DiscordToken);
                        doc.Save(XmlPath);
                        break;

                    case nameof(Settings.HelpCommandTimeout):
                        CreateSettingIfDoesNotExist(item, Settings.HelpCommandTimeout);
                        doc.Save(XmlPath);
                        break;

                    case nameof(Settings.ServerCommandTimeout):
                        CreateSettingIfDoesNotExist(item, Settings.ServerCommandTimeout);
                        doc.Save(XmlPath);
                        break;
                }
            }
            doc.Save(XmlPath);
        }

        public static void LoadSettings()
        {
            if ((string)GetApplicationSetting(nameof(Settings.SteamAPIKEY)) == Settings.SteamAPIKEY)
            {
                Console.WriteLine("This shouldnt have happened :(");
                return;
            }
            Settings.DiscordToken = (string)GetApplicationSetting(nameof(Settings.DiscordToken));
            Settings.SteamAPIKEY = (string)GetApplicationSetting(nameof(Settings.SteamAPIKEY));
            Settings.HelpCommandTimeout = int.Parse((string)GetApplicationSetting(nameof(Settings.HelpCommandTimeout)));
            Settings.ServerCommandTimeout = int.Parse((string)GetApplicationSetting(nameof(Settings.ServerCommandTimeout)));
        }
    }
}