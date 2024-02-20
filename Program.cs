using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using System;

namespace GetTokenFromAzureEntra;

class Program
{
    
    static void Main(string[] args)
    {
       // Check if a client ID is passed as an argument
       if (args.Length != 1)
       {
           Console.WriteLine("Usage: AzureADAuthApp <client-id>");
           return;
       }

       string clientId = args[0];
       string tenantId = "3d4d17ea-1ae4-4705-947e-51369c5a5f79"; // Replace with your Azure AD tenant ID

       // Define the authority, reply URL, and scopes
       var tenantIdAuthority = $"https://login.microsoftonline.com/{tenantId}";
       var interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions
       {
           ClientId = clientId,
           TenantId = tenantId,                 
           RedirectUri = new Uri("http://localhost:5000/callback"), // Make sure this redirect URI is registered in your Azure AD app
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
           Console.WriteLine($"Token: {authResult.Token}");
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Authentication failed: {ex.Message}");
       }
    }

    // TEST2
    // static async Task Main(string[] args)
    // {
    //     // Check if a client ID is passed as an argument
    //     if (args.Length != 1)
    //     {
    //         Console.WriteLine("Usage: AzureADAuthApp <client-id>");
    //         return;
    //     }

    //     string clientId = args[0];
    //     string tenantId = "3d4d17ea-1ae4-4705-947e-51369c5a5f79"; // Replace with your Azure AD tenant ID

    //     string redirectUri = "http://localhost:5000/callback"; // Your configured redirect URI
    //     string authority = $"https://login.microsoftonline.com/{tenantId}"; // Replace with your Azure AD tenant ID

    //     var app = PublicClientApplicationBuilder.Create(clientId)
    //         .WithAuthority(authority)
    //         .WithRedirectUri(redirectUri)
    //         .Build();

    //     string[] scopes = { "User.Read" }; // Customize based on your app's requirements

    //     try
    //     {
    //         var result = await app.AcquireTokenInteractive(scopes)
    //             .ExecuteAsync();

    //         Console.WriteLine($"Access token: {result.AccessToken}");
    //     }
    //     catch (MsalException ex)
    //     {
    //         Console.WriteLine($"Error acquiring token: {ex.Message}");
    //     }
    // }

}

