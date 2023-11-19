using TelegramBot_CRM.Architecture.DataModels;
using TelegramBot_CRM.Architecture.Enums;

namespace TelegramBot_CRM.Architecture.Interfaces;

public interface IInnApi
{
    public Task<(GetUserByInnStatus Status, Client? Client)> GetUserByInn(string inn);
}