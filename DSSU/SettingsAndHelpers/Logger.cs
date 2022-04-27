namespace DSSU
{
    public static class Logger
    {
        public static void Log(object Message)
        {
            if (Console.ForegroundColor != ConsoleColor.White)
                Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now}: {Message}");
        }
    }
}