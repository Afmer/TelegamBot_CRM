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
    private readonly IStateMachine _stateMachine;
    public TelegramBot(TelegramBotConfiguration configuration, IInnApi innApi, IStateMachine stateMachine)
    {
        _innApi = innApi;
        _stateMachine = stateMachine;
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
                await CommandHandler(message.Text, botClient, message, false);
            }
        }
    }
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
    private async Task CommandHandler(string command, ITelegramBotClient botClient, Message message, bool isLastCommandExecute)
    {
        bool unknownCommand = false;
        var words = command.Split(' ');
        if(words[0] == "/start")
        {
            await StartCommand(botClient, message);
        }
        else if(words[0] == "/inn")
        {
            await InnCommand(words, botClient, message);
        }
        else if(words[0] == "/last")
        {
            await LastCommand(botClient, message, isLastCommandExecute);
        }
        else if(words[0] == "/help")
        {
            await HelpCommand(botClient, message);
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда");
            unknownCommand = true;
        }
        if(words[0] != "/last" && !unknownCommand && !isLastCommandExecute)
        {
            string username = message.From!.Username!;
            _stateMachine.SetLastMessageState(username, message.Text!);
        }
    }

    private async Task StartCommand(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat, "Приветствую вас! Напишите /help для более подробной информации о моих возможностях");
    }
    private async Task InnCommand(string[] words, ITelegramBotClient botClient, Message message)
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
    private async Task LastCommand(ITelegramBotClient botClient, Message message, bool isLastCommandExecute)
    {
        string username = message.From!.Username!;
        var commandState = _stateMachine.GetLastMessageState(username);
        if(commandState == null)
            return;
        await CommandHandler(commandState, botClient, message, true);
    }
    private async Task HelpCommand(ITelegramBotClient botClient, Message message)
    {
        string text = "Введите \"/inn [ИНН 1] [ИНН 2] ... [ИНН n]\", чтобы получить компании, которым принадлежат введенные вами ИНН" +
            "\n\nВведите \"/last\", чтобы выполнить последнюю введенную вами команду";
        await botClient.SendTextMessageAsync(message.Chat, text);
    }
}