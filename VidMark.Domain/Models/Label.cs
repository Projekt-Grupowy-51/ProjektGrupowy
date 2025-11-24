using System.Text.Json;

namespace VidMark.Domain.Models;

public class Label : BaseEntity, IOwnedEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ColorHex { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty; // consider enum, ask about it, in the json given to us all types are range

    public char? Shortcut { get; set; } = null;

    
    public virtual Subject Subject { get; set; } = default!;

    public virtual ICollection<AssignedLabel> AssignedLabels { get; set; } = new List<AssignedLabel>();
    public string CreatedById { get; set; } = string.Empty;

    
    public virtual User CreatedBy { get; set; } = default!;
    public DateTime? DelDate { get; set; } = null;


    public string ToJson()
    {
        var jsonObject = new
        {
            label = new
            {
                name = Name,
                color = ColorHex,
                type = Type,
                shortcut = Shortcut.ToString()
            }
        };

        return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public static Label Create(string name, string colorHex, string type, char? shortcut, Subject subject, string createdById)
    {
        var label = new Label
        {
            Name = name,
            ColorHex = colorHex,
            Type = type,
            Shortcut = shortcut,
            Subject = subject,
            CreatedById = createdById
        };
        label.AddDomainEvent("Etykieta została dodana!", createdById);
        return label;
    }

    public void Update(string name, string colorHex, string type, char? shortcut, Subject subject, string userId)
    {
        Name = name;
        ColorHex = colorHex;
        Type = type;
        Shortcut = shortcut;
        Subject = subject;
        AddDomainEvent("Etykieta została zaktualizowana!", userId);
    }
}