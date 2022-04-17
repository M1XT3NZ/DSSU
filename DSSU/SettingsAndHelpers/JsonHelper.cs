using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using DSSU.Commands;
using DSSU.Commands.Classes;

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
            // JsonSerializer.Deserialize<List<Messageid>>(JsonText);
            var t = JsonSerializer.Deserialize<List<Messageid>>(JsonText);
            foreach (var item in t)
            {
                Console.WriteLine(item.Name);
                Console.WriteLine(item.Id);
                SettingsAndHelpers.Helpers.LoadMessage(item.Id);
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