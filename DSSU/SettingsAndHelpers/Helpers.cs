using Discord;
using Discord.WebSocket;

namespace DSSU.SettingsAndHelpers.Helpers
{
    public static class Helpers
    {
        public static bool HasManageServerPermission(SocketGuildUser user)
        {
            return user.GuildPermissions.ManageGuild;
        }

        public static async void LoadMessage(ulong messageid, ulong textchannel, ulong GuildID)
        {
            await Task.Delay(10000);//Delaying to wait For the Data to load
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

        public static bool IsTimeLessThan3Hours(TimeSpan[] time)
        {
            foreach (var item in time)
            {
                var _time = item;
                _time -= DateTime.Now.TimeOfDay;
                switch (Math.Round(_time.TotalHours))
                {
                    case 0:

                        return true;

                    case 1:
                        return true;

                    case 2:

                        return true;
                }
            }
            return false;
        }

        //This should return the Timespan of the array which is
        public static TimeSpan? GetTimeWhichIsLessThan3Hours(TimeSpan[] time)
        {
            foreach (var item in time)
            {
                var _time = item;
                var newtime = _time.Subtract(DateTime.Now.TimeOfDay);
                if (Math.Round(newtime.TotalHours, 0, MidpointRounding.ToZero) >= 0)

                    switch (Math.Round(newtime.TotalHours, 0, MidpointRounding.ToZero))
                    {
                        case 0:

                            Console.WriteLine(newtime.TotalHours);
                            return newtime;

                        case 1:

                            return newtime;

                        case 2:

                            return newtime;

                        default:
                            break;
                    }
            }
            return null;
        }

        public static string GetRestartTime() => HowMuchTimeTillRestart(ChernarusRestartTime);

        //Converts to local time and then adding one hour, Timezones be damned
        public static TimeSpan ConvertToLocalTime(string time)
        {
            return TimeSpan.Parse(time).Add(TimeSpan.FromHours(1));
        }

        public static string HowMuchTimeTillRestart(TimeSpan[] restarttimes)
        {
            if (IsTimeLessThan3Hours(restarttimes))
            {
                var t = GetTimeWhichIsLessThan3Hours(restarttimes);
                if (t == null)
                    return String.Empty;
                if (Math.Round(t.Value.TotalHours, 0, MidpointRounding.ToZero) == 0)
                    return $"Time Till Restart {t:%m} Minutes";
                else
                    return $"Time Till Restart {t:%h}:{t:%m} Hours";
            }
            else
                return "Couldnt Get Restart Time";
        }

        public static TimeSpan[] ChernarusRestartTime { get; set; } = {ConvertToLocalTime("00:00"), ConvertToLocalTime("03:00"),//ConvertToLocalTime("11:00"),

            ConvertToLocalTime("06:00"), ConvertToLocalTime("09:00"),
            ConvertToLocalTime("12:00"), ConvertToLocalTime("15:00"), ConvertToLocalTime("18:00"), ConvertToLocalTime("21:00")
        };
    }
}