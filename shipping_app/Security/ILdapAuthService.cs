using System.Threading.Tasks;

namespace shipping_app.Security
{
    public interface ILdapAuthService
    {
        /// <summary>/// Valida credenciales contra AD y devuelve la identidad (UPN o SAM) si son válidas./// </summary>
        Task<(bool ok, string? identity, string? error)> ValidateAsync(string username, string password);
    }
}
