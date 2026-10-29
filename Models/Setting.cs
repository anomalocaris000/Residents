namespace Residents.Models;

public class Setting
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int FlatId { get; set; }
    public DateTime MoveInDate { get; set; }
    public DateTime? MoveOutDate { get; set; }
    public bool IsActive => MoveOutDate == null;
}
