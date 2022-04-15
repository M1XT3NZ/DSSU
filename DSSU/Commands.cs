using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SteamQueryNet;
using SteamQueryNet.Interfaces;
using SteamQueryNet.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DSSU
{
    public class ServerInfoEmbed : ModuleBase<SocketCommandContext>
    {
        private EmbedBuilder embed = new EmbedBuilder();

        public static Dictionary<IUserMessage, DiscordMessages> mymessages = new Dictionary<IUserMessage, DiscordMessages>();

        [Command("ServerInfo")]
        public async Task MServerInfoAsync(
            string ServerIP
           )
        {
            //Checks if the Channel is either the Karma Krew server-status channel or the one in my private testing area
            if (Context.Channel.Id != 829380382104617050 && Context.Channel.Id != 964301577877864509)
                return;
            if (ServerIP == null)//Could do both in the same line but looks cleaner imo
                return;
            var serverIP = ServerIP.Trim();
            var info = Steam.IGameServersService.CSERVER(ServerIP);

            embed = Builder(embed, info, serverIP);

            /// Thinking about how I do it, since steam://rungameid/427420 is not a valid url
            // var builder = new ComponentBuilder()
            //     .WithButton("Start DAYZ", style: ButtonStyle.Link, url: "steam://rungameid/427520");

            var t = await ReplyAsync(embed: embed.Build()/*, components: builder.Build()*/);
            Console.WriteLine(t.ToString());
            DiscordMessages ms = new DiscordMessages()
            {
                EmbedBuilder = embed,
                Info = info,
                IP = serverIP,
            };

            mymessages.Add(t, ms);
        }

        //This is for the General Channel. This is only a single update (The most Current)

        private static Steam.Server? Info = new Steam.Server();
        private static String? ServerIP;

        [Command("Server")]
        public async Task SingleServerInfoAsync(string ServerName
            )
        {
            //    Console.WriteLine($"This is the current Datetime {DateTime.Now}\n" +
            //       $"This is the old Datetime {Program.dt}");
            if (Program.dt >= DateTime.Now)
                return;

            //Checks if the Channel is either the Karma Krew General channel or the one in my private testing area
            if (Context.Channel.Id != 572827981932789760 && Context.Channel.Id != 761993118181621812)
                return;
            if (ServerName == null)//Could do both in the same line but looks cleaner imo
                return;

            switch (ServerName)
            {
                case string str when str.Equals(Servers.NString, StringComparison.InvariantCultureIgnoreCase):

                    ServerIP = Servers.Namalsk.Trim();
                    Info = Steam.IGameServersService.CSERVER(Servers.Namalsk);
                    Console.WriteLine("im a testing whore");
                    break;

                case string str when str.Equals(Servers.CString, StringComparison.InvariantCultureIgnoreCase):

                    ServerIP = Servers.Namalsk.Trim();
                    Info = Steam.IGameServersService.CSERVER(Servers.Chernarus);
                    break;

                case string str when str.Equals(Servers.EString, StringComparison.InvariantCultureIgnoreCase):

                    Console.WriteLine("Well that doesnt exist yet ^_____^");
                    //Info = Steam.IGameServersService.CSERVER(Servers.Esseker);
                    break;

                default:
                    Console.WriteLine("Returned since");
                    return;
            }
            if (ServerIP == null || Info == null)
                return;
            Program.dt = DateTime.Now + TimeSpan.FromMinutes(5);
            embed = Builder(embed, Info, ServerIP);

            await ReplyAsync(embed: embed.Build());
        }

        public static EmbedBuilder Builder(EmbedBuilder embed, Steam.Server info, string ip)
        {
            embed.WithAuthor(info.name)
            .WithTitle($"Currently there are {info.players}/{info.max_players} playing.")
            .WithDescription($"{ip}")
            .WithThumbnailUrl("https://pbs.twimg.com/profile_images/1087807951648231426/VqouJGUB_400x400.jpg")
            .WithCurrentTimestamp();

            return embed;
        }

        public class DiscordMessages
        {
            public EmbedBuilder EmbedBuilder { get; set; }
            public Steam.Server Info { get; set; }
            public string IP { get; set; }
        }
    }
}