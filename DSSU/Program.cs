using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DSSU
{
    public class Program
    {
        private ObservableCollection<Task> tasks = new ObservableCollection<Task>();
        private DiscordSocketClient _client;
        private CommandService _commands;
        private Timer? _timer;
        public static DateTime dt;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            dt = DateTime.Now;
            var autoEvent = new AutoResetEvent(false);
            if (!XmlHelper.DoesSettingsFileExist())
            {
                XmlHelper.CreateSettingsFile();
                Console.WriteLine("Please Fill in the Token and API Key in the Settings.xml");
                Console.WriteLine("After you have entered both the Token and the API Key press any key");
                Console.Read();
            }

            XmlHelper.DISCORD_TOKEN = XmlHelper.GetApplicationSetting("DiscordToken");
            XmlHelper.STEAM_API_KEY = XmlHelper.GetApplicationSetting("SteamAPIKEY");
            //5 minutes = 300000

            _timer = new Timer(ServerStatusCheck, autoEvent, 0, 300000);
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _client.Log += Log;
            _commands.Log += Log;

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, XmlHelper.DISCORD_TOKEN);

            await _client.StartAsync();
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public void ServerStatusCheck(Object stateInfo)
        {
            if (ServerInfoEmbed.mymessages == null)
                return;
            foreach (var item in ServerInfoEmbed.mymessages)
            {
                var message = item.Key;
                var embed = item.Value.EmbedBuilder;
                var info = Steam.IGameServersService.CSERVER(item.Value.IP);
                if (info == null) { return; }
                if (item.Value.Offline)
                {
                    Console.WriteLine("Offline = True");
                    if (info.max_players > 0)
                    {
                        embed = ServerInfoEmbed.Builder(embed, info, item.Value.IP);
                        item.Value.Offline = false;
                    }
                }

                embed = ServerInfoEmbed.Builder(embed, info, item.Value.IP);
                message.ModifyAsync(x => x.Embed = embed.Build());
            }
        }
    }
}