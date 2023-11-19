using System.Text.Json;
using TelegramBot_CRM.Architecture.Configurations;

namespace TelegramBot_CRM.Services;
public class ConfigurationService
{
    private readonly string _telegramBotConfigJson;
    private readonly string _daDataConfigJson;
    public TelegramBotConfiguration TelegramBotConfiguration
    {
        get
        {
            var result = JsonSerializer.Deserialize<TelegramBotConfiguration>(_telegramBotConfigJson);
            return result!;
        }
    }
    public DaDataConfiguration DaDataConfiguration
    {
        get
        {
            var result = JsonSerializer.Deserialize<DaDataConfiguration>(_daDataConfigJson);
            return result!;
        }
    }
    public ConfigurationService()
    {
        if(File.Exists("./Configs/TelegramBotConfiguration.json"))
            _telegramBotConfigJson = File.ReadAllText("./Configs/TelegramBotConfiguration.json");
        else
            throw new Exception("TelegramBotConfiguration.json not exists");
        _ = JsonSerializer.Deserialize<TelegramBotConfiguration>(_telegramBotConfigJson)
            ?? throw new Exception("TelegramBotConfig.json is null");


        if (File.Exists("./Configs/DaDataConfig.json"))
            _daDataConfigJson = File.ReadAllText("./Configs/DaDataConfig.json");
        else
            throw new Exception("DaDataConfig.json not exists");
        _ = JsonSerializer.Deserialize<DaDataConfiguration>(_telegramBotConfigJson)
            ?? throw new Exception("TelegramBotConfig.json is null");
    }
}