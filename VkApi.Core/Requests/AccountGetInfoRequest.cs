namespace VkApi.Core.Requests;

public partial record AccountGetInfoRequest
{
    public AccountInfoField? Fields { get; init; }
}