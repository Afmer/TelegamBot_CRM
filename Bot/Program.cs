using TelegramBot_CRM.Services;

namespace TelegramBot_CRM;
public static class Program
{
    static void Main()
    {
        var config = new ConfigurationService();
        var bot = new TelegramBot(config.TelegramBotConfiguration);
        Console.ReadLine();
    }
}