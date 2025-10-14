namespace DreamLuso.Domain.Model;

public class Name
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();

    public Name()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public Name(string firstName, string lastName)
    {
        FirstName = firstName?.Trim() ?? string.Empty;
        LastName = lastName?.Trim() ?? string.Empty;
    }
}

