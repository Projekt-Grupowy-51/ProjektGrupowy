public interface IOwnedEntity
{
    string CreatedById { get; set; }
    DateTime? DelDate { get; set; }
}