using System;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace shipping_app.Security
{

    public class LdapAuthService : ILdapAuthService
    {
        private readonly LdapSettings _cfg;
        public LdapAuthService(LdapSettings cfg) => _cfg = cfg;



        public Task<(bool ok, string? identity, string? error)> ValidateAsync(string username, string password)
        {
            username = (username ?? string.Empty).Trim();
            password = password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
                return Task.FromResult<(bool, string?, string?)>((false, null, "Empty User"));

            if (string.IsNullOrEmpty(password))
                return Task.FromResult<(bool, string?, string?)>((false, null, "Empty Password"));

            try
            {
                // Selección del contexto: Domain | Machine | ApplicationDirectory
                var raw = (_cfg.ValidateContext ?? "Domain").Trim().ToLowerInvariant();
                ContextType ctxType = raw switch
                {
                    "machine" => ContextType.Machine,
                    "applicationdirectory" => ContextType.ApplicationDirectory,
                    _ => ContextType.Domain
                };

                using var context = string.IsNullOrWhiteSpace(_cfg.Container)
                    ? new PrincipalContext(ctxType, _cfg.Domain)
                    : new PrincipalContext(ctxType, _cfg.Domain, _cfg.Container);

                bool isValid = context.ValidateCredentials(username, password);
                if (!isValid)
                    return Task.FromResult<(bool, string?, string?)>((false, null, "Invalid Credentials"));

                using var user = UserPrincipal.FindByIdentity(context, username);
                var identity = user?.UserPrincipalName ?? user?.SamAccountName ?? username;

                return Task.FromResult<(bool, string?, string?)>((true, identity, null));
            }
            catch (Exception ex)
            {
                return Task.FromResult<(bool, string?, string?)>((false, null, $"LDAP error: {ex.Message}"));
            }
        }

        private static PrincipalContext CreatePrincipalContext(ContextType type, string? domain, string? container)
        {
            // Sin credenciales administrativas: ValidateCredentials hace el bind con username/password del login
            // Puedes especificar el "server" (DC) si lo necesitas: new PrincipalContext(type, "DC01", container)
            if (!string.IsNullOrWhiteSpace(container))
                return new PrincipalContext(type, domain, container);
            return new PrincipalContext(type, domain);
        }


    }
}
