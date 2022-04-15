using System.Text.Json;
using System.Net;

namespace Steam
{
    public class IGameServersService
    {
        //Steam Web API Key: https://steamcommunity.com/dev/apikey

        public static Server[] GetServerList(string url)
        {
            WebClient wc = new WebClient();
            string data = wc.DownloadString(url);
            Query servers = JsonSerializer.Deserialize<Query>(data);
            return servers.response.servers;
        }

        public static Server CSERVER(string serverIP)
        {
            var info = GetServerList($@"https://api.steampowered.com/IGameServersService/GetServerList/v1/?key={DSSU.XmlHelper.STEAM_API_KEY}&filter=\gameaddr\{serverIP}&limit=6320").First();
            Server server = new Server()
            {
                players = info.players,
                max_players = info.max_players,
                addr = info.addr,
                secure = info.secure,
                steamid = info.steamid,
                appid = info.appid,
                bots = info.bots,
                dedicated = info.dedicated,
                gamedir = info.gamedir,
                gametype = info.gametype,
                map = info.map,
                name = info.name,
                os = info.os,
                port = info.port,
                product = info.product,
                region = info.region,
                version = info.version
            };
            return server;
        }
    }

    public class Query
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public Server[] servers { get; set; }
    }

    public class Server
    {
        //Need to make this look nicer
        public string addr { get; set; }

        public ushort port { get; set; }
        public string steamid { get; set; }
        public string name { get; set; }
        public int appid { get; set; }
        public string gamedir { get; set; }
        public string version { get; set; }
        public string product { get; set; }
        public int region { get; set; }
        public int players { get; set; }
        public int max_players { get; set; }
        public int bots { get; set; }
        public string map { get; set; }
        public bool secure { get; set; }
        public bool dedicated { get; set; }
        public string os { get; set; }
        public string gametype { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}