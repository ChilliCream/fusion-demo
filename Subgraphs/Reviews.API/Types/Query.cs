namespace Reviews.API.Types;

[QueryType]
public static partial class Query
{
    [Lookup]
    [Internal]
    public static Product ProductById(int id)
        => new Product(id);
}

public record Product(int Id)
{
    public string NameAndId([Require] string name)
        => $"{Id} {name}";
}
