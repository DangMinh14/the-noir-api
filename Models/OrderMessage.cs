namespace TheNoir.Api.Models;

public static class OrderMessageSenders
{
    public const string Staff = "Staff";
    public const string Customer = "Customer";
}

// A quick back-and-forth thread on one order, e.g. staff asking "paper cup
// or ice?" mid-prep and the customer replying. Guest orders (no UserId)
// can't use it since there's no authenticated identity to reply as.
public class OrderMessage
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public required string SenderRole { get; set; }
    public required string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}
