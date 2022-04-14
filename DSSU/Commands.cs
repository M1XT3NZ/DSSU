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
            string ServerIP, string ServerInformation = ""
           )
        {
            if (Context.Channel.Id != 829380382104617050 || Context.Channel.Id != 964301577877864509)
                return;
            if (ServerIP == null)
                return;
            if (string.IsNullOrEmpty(ServerInformation))
                ServerInformation = "No Information Available Right Now";
            var serverIP = ServerIP.Trim();
            var info = Steam.IGameServersService.CSERVER(ServerIP);

            embed = Builder(embed, info, serverIP);

            // var builder = new ComponentBuilder()
            //     .WithButton("Start DAYZ", style: ButtonStyle.Link, url: "steam://rungameid/427520");

            var t = await ReplyAsync(embed: embed.Build()/*, components: builder.Build()*/);

            DiscordMessages ms = new DiscordMessages()
            {
                EmbedBuilder = embed,
                Info = info,
                IP = serverIP,
            };

            mymessages.Add(t, ms);
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