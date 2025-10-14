using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class PropertyImage : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public required Property Property { get; set; }
    public required string ImageUrl { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; } = false;
    public ImageType Type { get; set; }

    public PropertyImage()
    {
        Id = Guid.NewGuid();
    }

    public PropertyImage(Guid propertyId, string imageUrl, int displayOrder = 0)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
    }
}

public enum ImageType
{
    Exterior,       // Exterior
    Interior,       // Interior
    Kitchen,        // Cozinha
    Bedroom,        // Quarto
    Bathroom,       // Casa de banho
    LivingRoom,     // Sala de estar
    Garden,         // Jardim
    Pool,           // Piscina
    View,           // Vista
    FloorPlan,      // Planta
    Other
}

