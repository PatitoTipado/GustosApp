namespace GustosApp.Application.Interfaces
{
    public interface IOcrService
    {
        Task<string> ReconocerTextoAsync(IEnumerable<Stream> imagenes, string languages = "spa+eng", CancellationToken ct = default);
    }
}
