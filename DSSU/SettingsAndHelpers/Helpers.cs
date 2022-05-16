using Discord;
using Discord.WebSocket;
using DSSU.Interactions;

namespace DSSU.SettingsAndHelpers.Helpers
{
    public static class Helpers
    {
        public static bool HasManageServerPermission(SocketGuildUser user)
        {
            return user.GuildPermissions.ManageGuild;
        }

        public static bool IsOffline = false;

        public static async void LoadMessage(ulong messageid, ulong textchannel, ulong GuildID, string serverip, string mapname)
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
                    var info = Steam.IGameServersService.CSERVER(serverip);
                    if (info == null)
                    {
                        info = new Steam.Server()
                        {
                            name = $"{InteractionHelp.embed.Author} is Currently Offline",
                            players = 0,
                            max_players = 0,
                            addr = InteractionHelp.embed.Description
                        };
                        IsOffline = true;
                    }
                    IsOffline = false;
                    InteractionHelp.mymessages.Add(message, InteractionHelp.GetDiscordMessage(InteractionHelp.Builder(InteractionHelp.embed, InteractionHelp.embedField, info, item.Description), InteractionHelp.embedField, info, item.Description, mapname, IsOffline));
                    Logger.Log("Added Message");
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

                    case 3:

                        return true;
                }
            }
            return false;
        }

        //This should return the Timespan of the array which is
        public static TimeSpan? GetTimeWhichIsLessThan3Hours(TimeSpan[] time, bool Is4Hours = false)
        {
            foreach (var item in time)
            {
                var _time = item;

                Console.WriteLine("Server Restart Time: " + _time);
                var t = DateTime.Now.TimeOfDay;
                Console.WriteLine($"Current Time {t}");
                //var newtime = t.Subtract(_time);
                var newtime = _time.Subtract(DateTime.Now.TimeOfDay);
                Console.WriteLine("Time when the next restart is: " + newtime);
                if (Math.Round(t.TotalHours, 0, MidpointRounding.ToZero) == item.TotalHours)
                    goto Here;
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

                        case 3:
                            if (Is4Hours)
                            {
                                Console.WriteLine(nameof(time));
                                return newtime;
                            }
                            break;

                        default:
                            break;
                    }
                Here:;
            }
            return null;
        }

        //Converts to local time and then adding one hour, Timezones be damned
        public static TimeSpan ConvertToLocalTime(string time)
        {
            if (time == "00:00" && DateTime.Now.Hour != 0)
            {
                Console.WriteLine(1);
                return TimeSpan.Parse(time).Add(TimeSpan.FromDays(1));
            }
            else if (time == "00:00" && DateTime.Now.Hour == 0)
            {
                Console.WriteLine(2);
                return TimeSpan.Parse(time);
            }

            return TimeSpan.Parse(time);
        }

        private static TimeSpan dt;

        public static string HowMuchTimeTillRestart(TimeSpan[] restarttimes, bool is4hourrestart)
        {
            if (IsTimeLessThan3Hours(restarttimes))
            {
                var t = GetTimeWhichIsLessThan3Hours(restarttimes, is4hourrestart);
                if (t == null)
                    return String.Empty;
                Console.WriteLine(Math.Round(t.Value.TotalHours));
                if (Math.Round(t.Value.TotalHours, 0, MidpointRounding.ToZero) == 0)
                {
                    return $"{t:%m} Minute(s).";
                }
                else
                {
                    dt += (TimeSpan)t;
                    return $"{t:%h} hour(s) and {t:mm} minute(s).";
                }
            }
            else
                return "Couldnt Get Restart Time";
        }

        public static TimeSpan[] ChernarusRestartTime { get; } ={
            ConvertToLocalTime("00:00"), ConvertToLocalTime("03:00"),
            ConvertToLocalTime("06:00"), ConvertToLocalTime("09:00"),
            ConvertToLocalTime("12:00"), ConvertToLocalTime("15:00"),
            ConvertToLocalTime("18:00"), ConvertToLocalTime("21:00")
        };

        public static TimeSpan[] NamalskRestartTime { get; } = {
            ConvertToLocalTime("00:00"), ConvertToLocalTime("04:00"),
            ConvertToLocalTime("08:00"), ConvertToLocalTime("12:00"),
            ConvertToLocalTime("16:00"), ConvertToLocalTime("20:00")
        };
    }
}