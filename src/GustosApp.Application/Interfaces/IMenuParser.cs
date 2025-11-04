namespace GustosApp.Application.Interfaces
{
    public interface IMenuParser
    {
        Task<string> ParsearAsync(string texto, string monedaPorDefecto = "ARS", CancellationToken ct = default);
    }
}
