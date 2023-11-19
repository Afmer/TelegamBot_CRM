using TelegramBot_CRM.Services;

namespace TelegramBot_CRM;
public static class Program
{
    static void Main()
    {
        var config = new ConfigurationService();
        var innApi = new DaDataService(config.DaDataConfiguration);
        var bot = new TelegramBot(config.TelegramBotConfiguration, innApi);
        Console.ReadLine();
    }
}