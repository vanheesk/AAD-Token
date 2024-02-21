using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using Azure.Core;
using Azure.Identity;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using GetTokenFromAzureEntra.Models;

namespace GetTokenFromAzureEntra.Commands.Get;

internal class GetTokenCommand: Command<GetTokenCommand.Settings>
{
    public class Settings : GetSettings
    {

        [CommandOption("-c|--clientid <CLIENT-ID>")]
        [Description("The client/application id to query")]
        public string ClientId { get; set; }

        [CommandOption("-t|--tenantid <TENANT-ID>")]
        [Description("The tenant id to query")]
        public string TenantId { get; set; }

        [CommandOption("-r|--redirect-url <REDIRECT-URL>")]
        [Description("The callback url used after a succesful login.")]
        public string RedirectUri { get; set; } = "http://localhost:5000/callback";

    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var exitCode = 0;

        try
        {
            string clientId = settings.ClientId;
            string tenantId = settings.TenantId;

            // Define the authority, reply URL, and scopes
            var tenantIdAuthority = $"https://login.microsoftonline.com/{tenantId}";
            var interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions
            {
                ClientId = clientId,
                TenantId = tenantId,
                RedirectUri = new Uri(settings.RedirectUri), // Make sure this redirect URI is registered in your Azure AD app
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // Create an InteractiveBrowserCredential with the specified options
            var credential = new InteractiveBrowserCredential(interactiveBrowserCredentialOptions);

            // Request a token to authenticate
            var scope = new[] { "User.Read" }; // Specify the scopes your application requires
            var tokenRequestContext = new TokenRequestContext(scope);

            try
            {
                //var authResult = credential.Authenticate(tokenRequestContext);
                var authResult = credential.GetToken(tokenRequestContext);
                Console.WriteLine($"Token:");
                Console.WriteLine($"{authResult.Token}");
                
                Console.WriteLine("Decoded token:");
                var decodedToken = ConvertJwtStringToJwtSecurityToken(authResult.Token);
                Console.WriteLine(DecodeJwt(decodedToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            exitCode = -1;
        }

        return exitCode;
    }

    private JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(jwt) as JwtSecurityToken;

        return token;
    }

    private DecodedToken DecodeJwt(JwtSecurityToken token)
    {
        var decodedToken = new DecodedToken(
            token.Header.Kid,
            token.Issuer,
            token.Audiences.ToList(),
            token.Claims.Select(c => (c.Type, c.Value)).ToList(),
            token.ValidTo,
            token.SignatureAlgorithm,
            token.RawData,
            token.Subject,
            token.ValidFrom,
            token.EncodedHeader,
            token.EncodedPayload);

        return decodedToken;
    }

}
