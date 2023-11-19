namespace TelegramBot_CRM.Architecture.Interfaces;

public interface IStateMachine
{
    public void SetLastMessageState(string username, string lastCommand);
    public string? GetLastMessageState(string username);
}