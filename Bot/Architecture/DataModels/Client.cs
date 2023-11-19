namespace TelegramBot_CRM.Architecture.DataModels;

public class Client
{
    public string Name {get; private set;}
    public string Address {get; private set;}
    public Client(string name, string address)
    {
        Name = name;
        Address = address;
    }
}