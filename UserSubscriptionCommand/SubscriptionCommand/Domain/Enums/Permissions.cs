namespace SubscriptionCommand.Domain.Enums;

[Flags]
public enum Permissions : long
{
    None = 0,
    Transfer = 1,
    PurchaseCards = 2,
    ManageDevices = 4
}