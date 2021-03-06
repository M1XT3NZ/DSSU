using Discord.Interactions;
using DSSU.SettingsAndHelpers.Helpers;

namespace DSSU.Interactions.CommandInteractions
{
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }

        private InteractionHandler _handler;

        // Constructor injection is also a valid way to access the dependencies
        public AdminCommands(InteractionHandler handler)
        {
            _handler = handler;
        }

        [DefaultMemberPermissions(Discord.GuildPermission.Administrator)]
        [SlashCommand("serverinfo", "Creates a Serverinfo Embed that updates every 5 minutes")]
        public async Task MServerInfoAsync(
        [Choice("Chernarus", Servers.CString)]
       // [Choice("Namalsk",Servers.NString)]
        [Choice("Esseker",Servers.EString)]
        string ServerName
   )
        {
            var MapName = "";
            switch (ServerName)
            {
                case string str when str.Equals(Servers.NString, StringComparison.InvariantCultureIgnoreCase):
                    await InteractionHelp.GetCorrectRestartTimes(nameof(Servers.Namalsk));
                    MapName = Servers.NString;
                    InteractionHelp.is4hours = true;
                    InteractionHelp.ServerIP = Servers.Namalsk.Trim();
                    InteractionHelp.Info = Steam.IGameServersService.CSERVER(Servers.Namalsk);
                    break;

                case string str when str.Equals(Servers.CString, StringComparison.InvariantCultureIgnoreCase):
                    MapName = Servers.CString;
                    InteractionHelp.is4hours = false;
                    await InteractionHelp.GetCorrectRestartTimes(nameof(Servers.Chernarus));
                    InteractionHelp.ServerIP = Servers.Chernarus.Trim();
                    InteractionHelp.Info = Steam.IGameServersService.CSERVER(Servers.Chernarus);
                    break;

                case string str when str.Equals(Servers.EString, StringComparison.InvariantCultureIgnoreCase):
                    MapName = Servers.CString;
                    InteractionHelp.is4hours = true;
                    InteractionHelp.Info = Steam.IGameServersService.CSERVER(Servers.Esseker);

                    await InteractionHelp.GetCorrectRestartTimes(nameof(Servers.Esseker));
                    InteractionHelp.ServerIP = Servers.Esseker.Trim();

                    //Info = Steam.IGameServersService.CSERVER(Servers.Esseker);
                    break;

                default:
                    Logger.Log($"Returned since the Person Wrote Gibberish\n" +
                        $"The Person Wrote: {ServerName}");
                    return;
            }
            var serverIP = InteractionHelp.ServerIP.Trim();
            InteractionHelp.Info = Steam.IGameServersService.CSERVER(InteractionHelp.ServerIP);
            if (InteractionHelp.Info == null)
            {
                InteractionHelp.Info = new Steam.Server()
                {
                    name = "Currently Offline",
                    players = 0,
                    max_players = 0,
                    addr = serverIP
                };
                InteractionHelp.IsOffline = true;
            }
            else
                InteractionHelp.IsOffline = false;

            InteractionHelp.embedField = InteractionHelp.Builder(InteractionHelp.embedField, InteractionHelp.CurrentTimeSpans, InteractionHelp.is4hours);

            InteractionHelp.embed = InteractionHelp.Builder(InteractionHelp.embed, InteractionHelp.embedField, InteractionHelp.Info, serverIP);
            InteractionHelp.ms = InteractionHelp.GetDiscordMessage(InteractionHelp.embed, InteractionHelp.embedField, InteractionHelp.Info, serverIP, MapName, InteractionHelp.IsOffline);

            //This is not going to be possible unless i actually get all of the mods manually every time anything changes with the mods
            /// Thinking about how I do it, since steam://rungameid/427420 is not a valid url
            /// var builder = new ComponentBuilder()
            ///     .WithButton("Start DAYZ", style: ButtonStyle.Link, url: "steam://rungameid/427520");
            await DeferAsync();

            var t = await ReplyAsync(embed: InteractionHelp.embed.Build()/*, components: builder.Build()*/);
            await DeleteOriginalResponseAsync();
            foreach (var item in t.Embeds)
            {
                Logger.Log(item.Author);
            }
            InteractionHelp.mymessages.Add(t, InteractionHelp.ms);

            Messageid messageid = new Messageid()
            {
                Name = InteractionHelp.Info.name,
                Id = t.Id,
                MapName = MapName,
                IP = serverIP,
            };
            InteractionHelp.MessagIDs.Add(messageid);
        }

        [DefaultMemberPermissions(Discord.GuildPermission.Administrator)]
        [SlashCommand("saveserverinfo", "This will save the current servers of serverinfo")]
        public async Task SaveServerInfoAsync()
        {
            JsonHelper.Json.SetJson();
        }
    }
}