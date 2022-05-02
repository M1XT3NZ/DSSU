using Discord.Interactions;
using DSSU.RateLimit;

namespace DSSU.Interactions.CommandInteractions
{
    public class UserCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private InteractionHandler _handler;

        public UserCommands(InteractionHandler handler)
        {
            _handler = handler;
        }

        [DefaultMemberPermissions(Discord.GuildPermission.EmbedLinks)]
        [SlashCommand("server", "Shows Current Info of the server and an Estimated Server Restart time.")]
        public async Task SingleServerInfoAsync(
        [Choice("Chernarus", Servers.CString)]
        [Choice("Namalsk",Servers.NString)]
        string ServerName)
        {
            //Checks if the Channel is either the Karma Krew General channel or the one in my private testing area

            if (Context.Channel.Id != 572827981932789760 && Context.Channel.Id != 927187571744866374 && Context.Channel.Id != 761993118181621812)
            {
                await DeferAsync();
                await DeleteOriginalResponseAsync();
                return;
            }
            if (ServerName == null)//Could do both in the same line but looks cleaner imo
                return;
            switch (ServerName)
            {
                case string str when str.Equals(Servers.NString, StringComparison.InvariantCultureIgnoreCase):
                    await InteractionHelp.GetCorrectRestartTimes(Servers.NString);
                    InteractionHelp.is4hours = true;
                    InteractionHelp.ServerIP = Servers.Namalsk.Trim();
                    InteractionHelp.Info = Steam.IGameServersService.CSERVER(Servers.Namalsk);
                    break;

                case string str when str.Equals(Servers.CString, StringComparison.InvariantCultureIgnoreCase):
                    InteractionHelp.is4hours = false;
                    await InteractionHelp.GetCorrectRestartTimes(Servers.CString);
                    InteractionHelp.ServerIP = Servers.Chernarus.Trim();
                    InteractionHelp.Info = Steam.IGameServersService.CSERVER(Servers.Chernarus);
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
            if (InteractionHelp.ServerIP == null)
                return;
            if (InteractionHelp.Info == null)
            {
                InteractionHelp.Info = new Steam.Server()
                {
                    name = "Currently Offline",
                    players = 0,
                    max_players = 0,
                    addr = InteractionHelp.ServerIP
                };
            }
            Console.WriteLine(InteractionHelp.CurrentTimeSpans);
            InteractionHelp.embedField = InteractionHelp.Builder(InteractionHelp.embedField, InteractionHelp.CurrentTimeSpans, InteractionHelp.is4hours);
            InteractionHelp.embed = InteractionHelp.Builder(InteractionHelp.embed, InteractionHelp.embedField, InteractionHelp.Info, InteractionHelp.ServerIP);

            await DeferAsync();
            var t = await ReplyAsync(embed: InteractionHelp.embed.Build()/*, components: builder.Build()*/);
            await DeleteOriginalResponseAsync();
            //await ReplyAsync(embed: embed.Build());
        }
    }
}