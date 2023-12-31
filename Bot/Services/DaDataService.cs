using Dadata;
using TelegramBot_CRM.Architecture.Configurations;
using TelegramBot_CRM.Architecture.DataModels;
using TelegramBot_CRM.Architecture.Enums;
using TelegramBot_CRM.Architecture.Interfaces;

namespace TelegramBot_CRM.Services;

public class DaDataService : IInnApi
{
    private readonly SuggestClientAsync _api;
    public DaDataService(DaDataConfiguration configuration)
    {
        _api = new SuggestClientAsync(configuration.ApiKey);
    }
    public async Task<(GetUserByInnStatus Status, Client? Client)> GetUserByInn(string inn)
    {
        if(inn.Length != 10)
            return (GetUserByInnStatus.InvalidInn, null);
        for(int i = 0; i < inn.Length; i++)
            if(!char.IsDigit(inn[i]))
                return(GetUserByInnStatus.InvalidInn, null);
        var response = await _api.FindParty(inn);
        if(response.suggestions.Count == 0)
            return (GetUserByInnStatus.NotFound, null);
        var party = response.suggestions[0];
        var client = new Client(party.value, party.data.address.value);
        return (GetUserByInnStatus.Success, client);
    }
}