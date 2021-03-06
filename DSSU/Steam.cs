using System.Text.Json;

namespace Steam
{
    public class IGameServersService
    {
        //Steam Web API Key: https://steamcommunity.com/dev/apikey

        public static async Task<Server[]> GetServerList(string url)
        {
            HttpClient client = new HttpClient();
            string data = await client.GetStringAsync(url);
            Query servers = JsonSerializer.Deserialize<Query>(data);
            return servers.response.servers;
        }

        public static Server? CSERVER(string serverIP)
        {
            Server server = new();
            try
            {
                var servers = GetServerList($@"https://api.steampowered.com/IGameServersService/GetServerList/v1/?key={DSSU.SettingsAndHelpers.Settings.SteamAPIKEY}&filter=\gameaddr\{serverIP}&limit=6320");
                if (servers == null)
                    return null;
                var info = servers.Result.FirstOrDefault();

                if (info == null) return null;
                server = new Server()
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
            }
            catch (Exception)
            {
                return null;
            }

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