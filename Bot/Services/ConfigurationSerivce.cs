using System.Text.Json;
using TelegramBot_CRM.Architecture.Configurations;

namespace TelegramBot_CRM.Services;
public class ConfigurationService
{
    private readonly string _telegramBotConfigJson;
    public TelegramBotConfiguration TelegramBotConfiguration
    {
        get
        {
            var result = JsonSerializer.Deserialize<TelegramBotConfiguration>(_telegramBotConfigJson);
            return result!;
        }
    }
    public ConfigurationService()
    {
        if(File.Exists("./Configs/TelegramBotConfiguration.json"))
            _telegramBotConfigJson = File.ReadAllText("./Configs/TelegramBotConfiguration.json");
        else
            throw new Exception("TelegramBotConfiguration.json not exists");
        var telegramBotConfigChecker = JsonSerializer.Deserialize<TelegramBotConfiguration>(_telegramBotConfigJson) ?? throw new Exception("TelegramBotConfig.json is null");
    }
}