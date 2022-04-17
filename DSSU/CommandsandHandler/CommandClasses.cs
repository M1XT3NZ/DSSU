using Discord;

namespace DSSU.Commands.Classes
{
    public class DiscordMessages
    {
        public EmbedBuilder? EmbedBuilder { get; set; }
        public Steam.Server? Info { get; set; }
        public string? IP { get; set; }
        public bool Offline { get; set; }
    }

    public class Messageid
    {
        public string? Name { get; set; }
        public ulong Id { get; set; }
    }
}