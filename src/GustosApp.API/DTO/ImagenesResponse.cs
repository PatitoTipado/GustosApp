public class ImagenesResponse
{
    public int Total { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public List<RestauranteImagenDto> Items { get; set; } = new();
}