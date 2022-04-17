using DSSU.Commands;
using DSSU.Commands.Classes;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace DSSU.JsonHelper
{
    public class Json
    {
        private static string JsonFile = @$"{System.IO.Directory.GetCurrentDirectory()}/ids.json";
        private static ObservableCollection<string> JsonFiles = new ObservableCollection<string>();
        private static List<Messageid> JsonMessages = new List<Messageid>();

        public static void GetJson()
        {
            if (!File.Exists(JsonFile))
                return;
            var JsonText = File.ReadAllText(JsonFile);
            var t = JsonSerializer.Deserialize<List<Messageid>>(JsonText);
            foreach (var item in t)
            {
                foreach (var guilds in Program._client.Guilds)
                {
                    switch (guilds.Name)
                    {
                        case "KarmaKrew - DayZ Modded":
                            SettingsAndHelpers.Helpers.LoadMessage(item.Id, SettingsAndHelpers.Settings.KarmaKrewTextChannelID, SettingsAndHelpers.Settings.KarmaKrewGuildId);
                            Logger.Log("KarmaKrew Discord");
                            break;

                        case "Testing":
                            SettingsAndHelpers.Helpers.LoadMessage(item.Id, SettingsAndHelpers.Settings.PrivateTextChannelID, SettingsAndHelpers.Settings.PrivateGuildId);
                            Logger.Log("Testing Discrod");
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public static void SetJson()
        {
            foreach (var item in ServerInfoEmbed.MessagIDs)
            {
                JsonMessages.Add(item);
                //JsonSerializer.Serialize<Messageid>(item, new JsonSerializerOptions { WriteIndented = true });
            }
            var json = JsonSerializer.Serialize(JsonMessages, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(JsonFile, json);
        }
    }
}