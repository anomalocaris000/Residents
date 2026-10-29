namespace Residents.Models;

public class Bill
{
    public int Id { get; set; }
    public int FlatId { get; set; }
    public string Period { get; set; } = "";
    public string ServiceName { get; set; } = "";
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
}
