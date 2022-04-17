namespace DSSU
{
    public static class Logger
    {
        public static void Log(object Message)
        {
            Console.WriteLine($"{DateTime.Now}: {Message}");
        }
    }
}