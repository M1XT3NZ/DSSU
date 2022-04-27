using DSSU.Commands;
using DSSU.Commands.Classes;
using System.Collections.ObjectModel;
using System.Text.Json;
using DSSU.SettingsAndHelpers.Helpers;

namespace DSSU.JsonHelper
{
    public class Json
    {
        private static string JsonFile = @$"{System.IO.Directory.GetCurrentDirectory()}/ids.json";
        private static ObservableCollection<string> JsonFiles = new ObservableCollection<string>();
        private static List<Messageid> JsonMessages = new List<Messageid>();

        public static async void GetJson()
        {
            try
            {
                if (!File.Exists(JsonFile))
                    return;
                var JsonText = File.ReadAllText(JsonFile);
                var t = JsonSerializer.Deserialize<List<Messageid>>(JsonText);
                foreach (var item in t)
                {
                    if (item.IP == null)
                        throw new Exception("IP in the IDS.JSON was Misconfigured and had no ip");
                    foreach (var guilds in Program._client.Guilds)
                    {
                        Task.Delay(2000);
                        switch (guilds.Name)
                        {
                            case "KarmaKrew - DayZ Modded":
                                Helpers.LoadMessage(item.Id, SettingsAndHelpers.Settings.KarmaKrewTextChannelID, SettingsAndHelpers.Settings.KarmaKrewGuildId, item.IP);
                                Logger.Log("KarmaKrew Discord");
                                break;

                            case "Testing":
                                Helpers.LoadMessage(item.Id, SettingsAndHelpers.Settings.PrivateTextChannelID, SettingsAndHelpers.Settings.PrivateGuildId, item.IP);
                                Logger.Log("Testing Discrod");
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception g)
            {
                var ms = await Program._client.GetUserAsync(221467177302097931);
                var dm = await ms.CreateDMChannelAsync();
                await dm.SendMessageAsync(text: g.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.Log("Hey we have a Invalid Json file please redo the messages and save them");
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