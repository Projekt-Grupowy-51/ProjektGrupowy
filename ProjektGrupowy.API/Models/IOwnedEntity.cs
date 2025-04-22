public interface IOwnedEntity
{
    string OwnerId { get; set; }
    DateTime? DelDate { get; set; }
}