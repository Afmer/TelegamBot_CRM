using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
namespace TelegramBot_CRM;
public class TelegramBot
{
    private readonly ITelegramBotClient bot;
    public TelegramBot(string token)
    {
        bot = new TelegramBotClient(token);
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
        throw new NotImplementedException();
    }
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
}