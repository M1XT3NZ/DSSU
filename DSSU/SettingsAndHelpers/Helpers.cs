using Discord.WebSocket;
using Discord;

namespace DSSU.SettingsAndHelpers
{
    public class Helpers
    {
        public static bool HasManageServerPermission(SocketGuildUser user)
        {
            return user.GuildPermissions.ManageGuild;
        }

        public static async void LoadMessage(ulong messageid)
        {
            //var guild = Program._client.GetGuild(567699721343336448);
            //var t = guild.GetTextChannel(829380382104617050);
            //t.GetMessagesAsync(2, dir: Direction.Before);
            await Task.Delay(10000);
            var guild = Program._client.GetGuild(761993117662052383);
            var t = guild.GetTextChannel(964301577877864509);
            var g = await t.GetMessageAsync(messageid);
            //IUserMessage message = (IUserMessage)g;
            if (g is IUserMessage message)
            {
                foreach (var item in g.Embeds)
                {
                    Console.WriteLine($"Found Message");
                    Console.WriteLine(item.Title);
                    Console.WriteLine(item.Description);
                    var info = Steam.IGameServersService.CSERVER(item.Description);
                    Commands.ServerInfoEmbed.Builder(Commands.ServerInfoEmbed.embed, info, item.Description);
                    Commands.ServerInfoEmbed.mymessages.Add(message, Commands.ServerInfoEmbed.GetDiscordMessage(Commands.ServerInfoEmbed.embed, info, item.Description));
                }
            }
        }
    }
}