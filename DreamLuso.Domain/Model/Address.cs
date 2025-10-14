namespace DreamLuso.Domain.Model;

public class Address
{
    public string Street { get; set; }
    public string Number { get; set; }
    public string Parish { get; set; }        // Freguesia
    public string Municipality { get; set; }  // Concelho
    public string District { get; set; }      // Distrito
    public string PostalCode { get; set; }
    public string? Complement { get; set; }   // Complemento (ex: Andar, Porta)

    public Address()
    {
        Street = string.Empty;
        Number = string.Empty;
        Parish = string.Empty;
        Municipality = string.Empty;
        District = string.Empty;
        PostalCode = string.Empty;
    }

    public Address(string street, string number, string parish, string municipality, 
                   string district, string postalCode, string? complement = null)
    {
        Street = street;
        Number = number;
        Parish = parish;
        Municipality = municipality;
        District = district;
        PostalCode = postalCode;
        Complement = complement;
    }

    public string FullAddress => 
        $"{Street}, {Number}{(string.IsNullOrWhiteSpace(Complement) ? "" : $", {Complement}")}, {PostalCode} {Parish}, {Municipality}";
}

