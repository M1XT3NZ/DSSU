using Discord;
using Discord.Commands;
using DSSU.Commands.Classes;
using DSSU.RateLimit;
using System.Collections.ObjectModel;
using DSSU.SettingsAndHelpers.Helpers;

namespace DSSU.Commands
{
    public class ServerInfoEmbed : ModuleBase<SocketCommandContext>
    {
        public static EmbedBuilder embed = new EmbedBuilder();

        public static Dictionary<IUserMessage, DiscordMessages> mymessages = new Dictionary<IUserMessage, DiscordMessages>();
        public static ObservableCollection<Messageid> MessagIDs = new ObservableCollection<Messageid>();

        private static Steam.Server? Info = new Steam.Server();
        private static String? ServerIP;
        private static DiscordMessages? ms;
        private static bool IsOffline = false;

        [Command("GetServerInfo")]
        public async Task MServerInfoAsync(
            string ServerIP
           )
        {
            if (!Helpers.HasManageServerPermission(Context.Guild.GetUser(Context.User.Id)))
                return;

            //Checks if the Channel is either the Karma Krew server-status channel or the one in my private testing area
            if (Context.Channel.Id != 829380382104617050 && Context.Channel.Id != 964301577877864509)
                return;
            if (ServerIP == null)//Could do both in the same line but looks cleaner imo
                return;
            var serverIP = ServerIP.Trim();
            Info = Steam.IGameServersService.CSERVER(ServerIP);
            if (Info == null)
            {
                Info = new Steam.Server()
                {
                    name = "Currently Offline",
                    players = 0,
                    max_players = 0,
                    addr = serverIP
                };
                IsOffline = true;
            }
            else
                IsOffline = false;

            embed = Builder(embed, Info, serverIP);
            ms = GetDiscordMessage(embed, Info, serverIP, IsOffline);

            //This is not going to be possible unless i actually get all of the mods manually every time anything changes with the mods
            /// Thinking about how I do it, since steam://rungameid/427420 is not a valid url
            /// var builder = new ComponentBuilder()
            ///     .WithButton("Start DAYZ", style: ButtonStyle.Link, url: "steam://rungameid/427520");

            var t = await ReplyAsync(embed: embed.Build()/*, components: builder.Build()*/);

            mymessages.Add(t, ms);

            Messageid messageid = new Messageid()
            {
                Name = Info.name,
                Id = t.Id,
                IP = serverIP,
            };
            MessagIDs.Add(messageid);
        }

        [Command("SaveServerInfo", RunMode = RunMode.Async)]
        public async Task SaveServerInfoAsync()
        {
            if (!Helpers.HasManageServerPermission(Context.Guild.GetUser(Context.User.Id)))
                return;
            JsonHelper.Json.SetJson();
        }

        public static DiscordMessages GetDiscordMessage(EmbedBuilder embed, Steam.Server Info, string IP, bool IsOffline = false)
        {
            DiscordMessages ms = new DiscordMessages()
            {
                EmbedBuilder = embed,
                Info = Info,
                IP = IP,
                Offline = IsOffline
            };
            return ms;
        }

        //This is for the General Channel. This is only a single update (The most Current)
        //Only useable every 5 Minutes

        [Command("Server")]
        //User Can Issue the command once every 5 minutes.
        [Ratelimit(1, 5, Measure.Minutes)]
        public async Task SingleServerInfoAsync(string ServerName
            )
        {
            //Checks if the Channel is either the Karma Krew General channel or the one in my private testing area
            if (Context.Channel.Id != 572827981932789760 && Context.Channel.Id != 927187571744866374 && Context.Channel.Id != 761993118181621812)
                return;
            if (ServerName == null)//Could do both in the same line but looks cleaner imo
                return;

            switch (ServerName)
            {
                case string str when str.Equals(Servers.NString, StringComparison.InvariantCultureIgnoreCase):

                    ServerIP = Servers.Namalsk.Trim();
                    Info = Steam.IGameServersService.CSERVER(Servers.Namalsk);
                    break;

                case string str when str.Equals(Servers.CString, StringComparison.InvariantCultureIgnoreCase):

                    ServerIP = Servers.Chernarus.Trim();
                    Info = Steam.IGameServersService.CSERVER(Servers.Chernarus);
                    break;

                case string str when str.Equals(Servers.EString, StringComparison.InvariantCultureIgnoreCase):

                    Logger.Log("Well that doesnt exist yet ^_____^");
                    //Info = Steam.IGameServersService.CSERVER(Servers.Esseker);
                    break;

                default:
                    Logger.Log($"Returned since the Person Wrote Gibberish\n" +
                        $"The Person Wrote: {ServerName}");
                    return;
            }
            if (ServerIP == null)
                return;
            if (Info == null)
            {
                Info = new Steam.Server()
                {
                    name = "Currently Offline",
                    players = 0,
                    max_players = 0,
                    addr = ServerIP
                };
            }

            embed = Builder(embed, Info, ServerIP);

            await ReplyAsync(embed: embed.Build());
        }

        [Ratelimit(1, 20, Measure.Minutes)]
        [Command("Help")]
        public async Task ShowHelp()
        {
            await ReplyAsync("The Current Commands Are\n" +
                "!ServerInfo <IP> (Admins Only)\n" +
                "!SaveServerInfo (Admins Only)" +
                "!Server <NameOfTheServer> for example !Server namalsk\n" +
                "Have a Wonderful day :)");
        }

        public static EmbedBuilder Builder(EmbedBuilder embed, Steam.Server info, string ip)
        {
            embed.WithAuthor(info.name)
            .WithTitle($"Currently there are {info.players}/{info.max_players} playing.")
            .WithDescription($"{ip}")
            //.WithFields(new EmbedFieldBuilder
            //{
            //    Name = "Next Restart:",
            //    Value = Helpers.GetRestartTime(),
            //})
            .WithThumbnailUrl("https://pbs.twimg.com/profile_images/1087807951648231426/VqouJGUB_400x400.jpg")
            .WithCurrentTimestamp();

            return embed;
        }
    }
}