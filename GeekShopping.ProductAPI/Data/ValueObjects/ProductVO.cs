namespace GeekShopping.ProductAPI.Data.ValueObjects;

public class ProductVO
{
    public long Id { get; set; }
    public string Name { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }

    public string Category_Name { get; set; }

    public string Image_URL { get; set; }
}

