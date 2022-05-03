using Discord;
using DSSU.SettingsAndHelpers.Helpers;
using System.Collections.ObjectModel;

namespace DSSU.Interactions
{
    public class InteractionHelp
    {
        public static TimeSpan[] CurrentTimeSpans { get; set; }

        public static EmbedBuilder embed = new EmbedBuilder();

        public static EmbedFieldBuilder embedField = new EmbedFieldBuilder();

        public static Dictionary<IUserMessage, DiscordMessages> mymessages = new Dictionary<IUserMessage, DiscordMessages>();

        public static ObservableCollection<Messageid> MessagIDs = new ObservableCollection<Messageid>();

        public static bool is4hours;
        public static Steam.Server? Info = new Steam.Server();
        public static String? ServerIP;
        public static bool IsOffline = false;
        public static DiscordMessages? ms;

        public static DiscordMessages GetDiscordMessage(EmbedBuilder embed, EmbedFieldBuilder fieldbuilder, Steam.Server Info, string IP, string name, bool IsOffline = false)
        {
            DiscordMessages ms = new DiscordMessages()
            {
                EmbedBuilder = embed,
                EmbedFieldBuilder = fieldbuilder,
                MapName = name,
                Info = Info,
                IP = IP,
                Offline = IsOffline
            };
            return ms;
        }

        public static EmbedBuilder Builder(EmbedBuilder embed, EmbedFieldBuilder embedfield, Steam.Server info, string ip)
        {
            embed = new EmbedBuilder();
            embed.WithAuthor(info.name)
            .WithTitle($"Currently there are {info.players}/{info.max_players} playing.")
            .WithDescription($"{ip}")
            .WithFields(embedfield)
            .WithThumbnailUrl("https://pbs.twimg.com/profile_images/1087807951648231426/VqouJGUB_400x400.jpg")
            .WithCurrentTimestamp();

            return embed;
        }

        public static EmbedFieldBuilder Builder(EmbedFieldBuilder embed, TimeSpan[] span, bool is4hours)
        {
            var s = Helpers.HowMuchTimeTillRestart(span, is4hours);
            Console.WriteLine(s);
            Task.Delay(1000);
            embed.WithName("Next Restart In")
                .WithValue(s)
                .WithIsInline(true);
            return embed;
        }

        public static async Task GetCorrectRestartTimes(string servername)
        {
            switch (servername)
            {
                case string str when str.Equals(Servers.NString, StringComparison.InvariantCultureIgnoreCase):
                    CurrentTimeSpans = Helpers.NamalskRestartTime;
                    break;

                case string str when str.Equals(Servers.CString, StringComparison.InvariantCultureIgnoreCase):

                    CurrentTimeSpans = Helpers.ChernarusRestartTime;
                    break;

                case string str when str.Equals(Servers.EString, StringComparison.InvariantCultureIgnoreCase):

                    Logger.Log("Well that doesnt exist yet ^_____^");

                    break;
            }
        }
    }

    public class DiscordMessages
    {
        public EmbedBuilder EmbedBuilder { get; set; }
        public EmbedFieldBuilder? EmbedFieldBuilder { get; set; }
        public string MapName { get; set; }
        public Steam.Server? Info { get; set; }
        public string? IP { get; set; }
        public bool Offline { get; set; }
    }

    public class Messageid
    {
        public string? Name { get; set; }
        public ulong Id { get; set; }
        public string MapName { set; get; }

        public string IP { get; set; }
    }
}