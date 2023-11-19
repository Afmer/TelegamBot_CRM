using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot_CRM.Architecture.Configurations;
namespace TelegramBot_CRM;
public class TelegramBot
{
    private readonly ITelegramBotClient bot;
    public TelegramBot(TelegramBotConfiguration configuration)
    {
        bot = new TelegramBotClient(configuration.Token);
        try
        {
            var name = bot.GetMeAsync().Result.FirstName;
        }
        catch(ArgumentException e)
        {
            bot = null!;
            throw new ArgumentException(e.Message);
        }
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { },
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
    }
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            if (message != null && message.Text != null)
            {
                if(message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Инициализация пройдена");
                }
            }
        }
    }
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
}