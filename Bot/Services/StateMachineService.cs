using TelegramBot_CRM.Architecture.DataModels;
using TelegramBot_CRM.Architecture.Interfaces;

namespace TelegramBot_CRM.Services;

public class StateMachineService : IStateMachine
{
    private Dictionary<string, UserState> _userStates = new();
    public void SetLastMessageState(string username, string lastCommand)
    {
        if(_userStates.ContainsKey(username))
            _userStates[username].LastCommand = lastCommand;
        else
        {
            var state = new UserState();
            state.LastCommand = lastCommand;
            _userStates.Add(username, state);
        }
    }
    public string? GetLastMessageState(string username)
    {
        if(_userStates.ContainsKey(username) && _userStates[username].LastCommand != null)
            return new string(_userStates[username].LastCommand);
        else
            return null;
    }
}