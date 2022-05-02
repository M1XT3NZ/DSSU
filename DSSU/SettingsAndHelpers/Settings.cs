namespace DSSU.SettingsAndHelpers
{
    public class Settings
    {
        public static string SteamAPIKEY { get; set; } = "YOUR_API_KEY_HERE";
        public static string DiscordToken { get; set; } = "YOUR_TOKEN_HERE";
        public static int HelpCommandTimeout { get; set; } = 20;//20 minutes
        public static int ServerCommandTimeout { get; set; } = 5;//5 minutes

        public static ulong KarmaKrewGuildId { get; set; } = 567699721343336448;
        public static ulong KarmaKrewTextChannelID { get; set; } = 829380382104617050;
        public static ulong PrivateGuildId { get; set; } = 761993117662052383;
        public static ulong PrivateTextChannelID { get; set; } = 964301577877864509;
    }
}