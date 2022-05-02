using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DSSU.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace DSSU
{
    public class Program
    {
        private ObservableCollection<Task> tasks = new ObservableCollection<Task>();

        private Timer? _timer;
        private static bool IsReady = false;

        private readonly IServiceProvider _services;
        public static DiscordSocketClient _client;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        };

        public Program()
        {
            _services = new ServiceCollection()
                .AddSingleton(_socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
            _client = _services.GetRequiredService<DiscordSocketClient>();
        }

        private static void Main(string[] args)
            => new Program().MainAsync()
                .GetAwaiter()
                .GetResult();

        public async Task MainAsync()
        {
            //var autoEvent = new AutoResetEvent(false);
            //if (!XmlHelper.DoesSettingsFileExist())
            //{
            //    XmlHelper.CreateSettingsFile();
            //    Logger.Log("Please Fill in the Token and API Key in the Settings.xml");
            //    Logger.Log("After you have entered both the Token and the API Key press any key");
            //    Console.Read();
            //}
            //XmlHelper.CheckIfSettingsExists();
            //XmlHelper.LoadSettings();

            //_timer = new Timer(ServerStatusCheck, autoEvent, 0, 300000);

            //_client = _services.GetRequiredService<DiscordSocketClient>();

            //_client.Log += Log;
            //_client.Ready += _client_Ready;
            //_client.Disconnected += _client_Disconnected;

            //// Here we can initialize the service that will register and execute our commands
            //await _services.GetRequiredService<InteractionHandler>()
            //    .InitializeAsync();

            //// Bot token can be provided from the Configuration object we set up earlier
            //await _client.LoginAsync(TokenType.Bot, SettingsAndHelpers.Settings.DiscordToken);
            //await _client.StartAsync();
            SettingsAndHelpers.Helpers.Helpers.HowMuchTimeTillRestart(SettingsAndHelpers.Helpers.Helpers.ChernarusRestartTime, false);
            //Console.WriteLine("test " + SettingsAndHelpers.Helpers.Helpers.restart);

            // Never quit the program until manually forced to.
            await Task.Delay(Timeout.Infinite);

            //_timer = new Timer(ServerStatusCheck, autoEvent, 0, 300000);
            //_client = new DiscordSocketClient();
            //_commands = new CommandService(new CommandServiceConfig
            //{
            //    CaseSensitiveCommands = false,
            //});
            //_client.Log += Log;
            //_commands.Log += Log;

            //await InstallCommandsAsync();

            //await _client.LoginAsync(TokenType.Bot, SettingsAndHelpers.Settings.DiscordToken);

            //await _client.StartAsync();
            //_client.Ready += () =>
            //{
            //    Logger.Log("Bot is connected!");
            //    return Task.CompletedTask;
            //};

            //_client.Ready += _client_Ready;
            //_client.Disconnected += _client_Disconnected;
            //while (!IsReady)
            //{
            //    Task.Delay(1000);
            //}
            //await SetupCommands();
            //_client.SlashCommandExecuted += Commands.ServerInfoEmbed._client_SlashCommandExecuted;
            // Block this task until the program is closed.
        }

        private Task _client_Disconnected(Exception arg)
        {
            IsReady = false;
            return Task.CompletedTask;
        }

        private async Task<Task> _client_Ready()
        {
            IsReady = true;
            JsonHelper.Json.GetJson();
            await _client.SetGameAsync("Dayz on KarmaKrew Servers", type: ActivityType.Playing);
            return Task<Task>.CompletedTask;
        }

        private async Task SetupCommands()
        {
            var globalcommand = new SlashCommandBuilder();
            globalcommand.WithName("getserverinfo")
                .WithDescription("This will create an embed of the specified server that gets refreshed every 5 minutes");
            await _client.CreateGlobalApplicationCommandAsync(globalcommand.Build());
            globalcommand.WithName("server")
                .WithDescription("Shows an embed of the current Server POP/restart time")
                .AddOption(new SlashCommandOptionBuilder()
                .WithName("servername")
                .WithDescription("The name of the servermap")
                .WithRequired(true)
                .AddChoice("Chernarus", Servers.CString)
                .AddChoice("Namalsk", Servers.NString)
                .WithType(ApplicationCommandOptionType.String)

                );
            await _client.CreateGlobalApplicationCommandAsync(globalcommand.Build());
            globalcommand.WithName("saveserverinfo")
                .WithDescription("Saves ServerInfo to File. (Admins Only)");
            await _client.CreateGlobalApplicationCommandAsync(globalcommand.Build());
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Logger.Log(msg);
                    break;

                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Logger.Log(msg);
                    break;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Logger.Log(msg);
                    break;

                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Logger.Log(msg);
                    break;

                case LogSeverity.Verbose:
                    break;

                case LogSeverity.Debug:
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    Logger.Log(msg);
                    break;
            }

            return Task.CompletedTask;
        }

        public async void ServerStatusCheck(Object stateInfo)
        {
            if (InteractionHelp.mymessages == null)
                return;
            Logger.Log("Checking Server Status");
            try
            {
                foreach (var item in InteractionHelp.mymessages)
                {
                    Console.WriteLine("this works");
                    var message = item.Key;
                    var embed = item.Value.EmbedBuilder;
                    var fieldbuiler = item.Value.EmbedFieldBuilder;
                    var info = Steam.IGameServersService.CSERVER(item.Value.IP);
                    Logger.Log(message);
                    Logger.Log(embed);
                    await InteractionHelp.GetCorrectRestartTimes(item.Value.MapName);
                    if (item.Value.MapName.Contains("Namalsk"))
                        InteractionHelp.is4hours = true;
                    else
                        InteractionHelp.is4hours = false;
                    fieldbuiler.WithValue(SettingsAndHelpers.Helpers.Helpers.HowMuchTimeTillRestart(InteractionHelp.CurrentTimeSpans, InteractionHelp.is4hours));
                    if (info == null)
                    {
                        Logger.Log($"Server with IP:{embed.Description} is offline");
                        info = new Steam.Server()
                        {
                            name = $"{embed.Author} is Currently Offline",
                            players = 0,
                            max_players = 0,
                            addr = embed.Description
                        };
                        item.Value.Offline = true;
                    }
                    if (item.Value.Offline)
                    {
                        Logger.Log($"{embed.Description} is offline checking if it has a player account");
                        if (info.max_players >= 0)
                        {
                            embed = InteractionHelp.Builder(embed, fieldbuiler, info, item.Value.IP);
                            item.Value.Offline = false;
                        }
                    }
                    embed.Author.Name = info.name;
                    embed = InteractionHelp.Builder(embed, fieldbuiler, info, item.Value.IP);
                    await message.ModifyAsync(x => x.Embed = embed.Build());
                    Logger.Log($"Updated Server Info of {info.name}");
                    await Task.Delay(3000);
                }
            }
            catch (Exception g)
            {
                var ms = await _client.GetUserAsync(221467177302097931);
                var dm = await ms.CreateDMChannelAsync();
                await dm.SendMessageAsync(text: g.Message);
                Logger.Log(g.Message);
            }
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}