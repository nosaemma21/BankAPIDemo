using BankAccountManager.Enums;

namespace BankAccountManager.Models;

public class Account
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountTypes AccountType { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
}
