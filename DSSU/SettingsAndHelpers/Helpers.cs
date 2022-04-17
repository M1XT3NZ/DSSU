using Discord;
using Discord.WebSocket;

namespace DSSU.SettingsAndHelpers
{
    public class Helpers
    {
        public static bool HasManageServerPermission(SocketGuildUser user)
        {
            return user.GuildPermissions.ManageGuild;
        }

        public static async void LoadMessage(ulong messageid, ulong textchannel, ulong GuildID)
        {
            //var guild = Program._client.GetGuild(567699721343336448);
            //var t = guild.GetTextChannel(829380382104617050);
            //t.GetMessagesAsync(2, dir: Direction.Before);
            await Task.Delay(10000);
            var guild = Program._client.GetGuild(GuildID);
            var t = guild.GetTextChannel(textchannel);
            var g = await t.GetMessageAsync(messageid);
            if (g is IUserMessage message)
            {
                foreach (var item in g.Embeds)
                {
                    Logger.Log($"Found Message");
                    Logger.Log(item.Title);
                    Logger.Log(item.Description);
                    var info = Steam.IGameServersService.CSERVER(item.Description);
                    Commands.ServerInfoEmbed.Builder(Commands.ServerInfoEmbed.embed, info, item.Description);
                    Commands.ServerInfoEmbed.mymessages.Add(message, Commands.ServerInfoEmbed.GetDiscordMessage(Commands.ServerInfoEmbed.embed, info, item.Description));
                }
            }
        }
    }
}