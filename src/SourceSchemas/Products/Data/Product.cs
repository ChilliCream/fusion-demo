namespace Demo.Products.Data;

public class Product
{
    // [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string? Name { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int Weight { get; set; }
    
    [Required]
    public double Length { get; set; }

    [Required]
    public double Width { get; set; }

    [Required]
    public double Height { get; set; }

    public string? PictureFileName { get; set; }
}
