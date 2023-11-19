using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot_CRM.Architecture.Configurations;
using TelegramBot_CRM.Architecture.Enums;
using TelegramBot_CRM.Architecture.Interfaces;
namespace TelegramBot_CRM;
public class TelegramBot
{
    private readonly ITelegramBotClient bot;
    private readonly IInnApi _innApi;
    public TelegramBot(TelegramBotConfiguration configuration, IInnApi innApi)
    {
        _innApi = innApi;
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
            if (message != null && message.Text != null && message.Text != "")
            {
                var words = message.Text.Split(' ');
                if(words[0] == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Инициализация пройдена");
                }
                else if(words[0] == "/inn")
                {
                    var text = "";
                    for(int i = 1; i < words.Length; i++)
                    {
                        var client = await _innApi.GetUserByInn(words[i]);
                        if(client.Status == GetUserByInnStatus.InvalidInn)
                            text += $"{words[i]} не является ИНН";
                        else if(client.Status == GetUserByInnStatus.NotFound)
                        {
                            text += $"{words[i]} не найден";
                        }
                        else
                        {
                            text += $"{client.Client!.Name}\n{client.Client!.Address}"; 
                        }
                        if(i + 1 < words.Length)
                            text += "\n\n";
                    }
                    if(words.Length == 1)
                        text = "Вы не ввели ни одного ИНН";
                    await botClient.SendTextMessageAsync(message.Chat, text);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда");
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