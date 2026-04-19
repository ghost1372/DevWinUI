using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

namespace DevWinUI;
public static partial class CredentialHelper
{
    private static async Task<CredentialPickerResults> PickCredentialBase(string caption, string message, CredentialSaveOption credentialSaveOption, AuthenticationProtocol authenticationProtocol, bool alwaysDisplayDialog)
    {
        uint MAX_Length = 260;
        Span<char> buffer = stackalloc char[(int)MAX_Length];

        string computerName = string.Empty;

        int result = PInvoke.GetComputerName(buffer, ref MAX_Length);
        if (result > 0)
        {
            computerName = new string(buffer.Slice(0, result));
        }

        var options = new CredentialPickerOptions()
        {
            TargetName = computerName,
            Caption = caption,
            Message = message,
            CredentialSaveOption = credentialSaveOption,
            AuthenticationProtocol = authenticationProtocol,
            AlwaysDisplayDialog = alwaysDisplayDialog
        };
        return await CredentialPicker.PickAsync(options);
    }

    /// <summary>
    /// Prompts the user to select or enter credentials for authentication.
    /// and behavior.
    /// </summary>
    /// <param name="caption">Sets the title displayed at the top of the credential dialog.</param>
    /// <param name="message">Provides a message to guide the user on what credentials to enter.</param>
    /// <param name="credentialSaveOption">Determines whether the entered credentials can be saved for future use.</param>
    /// <param name="authenticationProtocol">Specifies the authentication method to be used for the credential request.</param>
    /// <param name="alwaysDisplayDialog">Indicates if the dialog should be shown every time, regardless of saved credentials.</param>
    /// <returns>Returns the results of the credential selection process.</returns>
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message, CredentialSaveOption credentialSaveOption, AuthenticationProtocol authenticationProtocol, bool alwaysDisplayDialog)
    {
        return await PickCredentialBase(caption, message, credentialSaveOption, authenticationProtocol, alwaysDisplayDialog);
    }

    /// <summary>
    /// Prompts the user to select or enter credentials for authentication.
    /// </summary>
    /// <param name="caption">Sets the title displayed at the top of the credential dialog.</param>
    /// <param name="message">Provides a message to guide the user on what credentials to enter.</param>
    /// <param name="credentialSaveOption">Determines whether the entered credentials can be saved for future use.</param>
    /// <param name="authenticationProtocol">Specifies the authentication method to be used for the credential request.</param>
    /// <returns>Returns the results of the credential selection process.</returns>
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message, CredentialSaveOption credentialSaveOption, AuthenticationProtocol authenticationProtocol)
    {
        return await PickCredentialBase(caption, message, credentialSaveOption, authenticationProtocol, true);
    }

    /// <summary>
    /// Prompts the user to select or enter credentials for authentication.
    /// </summary>
    /// <param name="caption">Sets the title displayed at the top of the credential dialog.</param>
    /// <param name="message">Provides a message to guide the user on what credentials to enter.</param>
    /// <param name="credentialSaveOption">Determines whether the entered credentials can be saved for future use.</param>
    /// <returns>Returns the results of the credential selection process.</returns>
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message, CredentialSaveOption credentialSaveOption)
    {
        return await PickCredentialBase(caption, message, credentialSaveOption, AuthenticationProtocol.Basic, true);
    }

    /// <summary>
    /// Prompts the user to select or enter credentials for authentication.
    /// </summary>
    /// <param name="caption">Sets the title displayed at the top of the credential dialog.</param>
    /// <param name="message">Provides a message to guide the user on what credentials to enter.</param>
    /// <returns>Returns the results of the credential selection process.</returns>
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message)
    {
        return await PickCredentialBase(caption, message, CredentialSaveOption.Selected, AuthenticationProtocol.Basic, true);
    }

    /// <summary>
    /// Retrieves a password credential associated with a specific resource and username from a password vault.
    /// </summary>
    /// <param name="resource">Specifies the resource for which the password credential is being retrieved.</param>
    /// <param name="username">Indicates the user for whom the password credential is being fetched.</param>
    /// <returns>Returns the password credential if found, otherwise returns null.</returns>
    public static PasswordCredential GetPasswordCredential(string resource, string username)
    {
        if (string.IsNullOrEmpty(resource) || string.IsNullOrEmpty(username))
        {
            return null;
        }

        try
        {
            var vault = new PasswordVault();
            return vault.Retrieve(resource, username);
        }
        catch (Exception)
        {
        }

        return null;
    }

    /// <summary>
    /// Adds a password credential to a secure vault if all required information is provided.
    /// </summary>
    /// <param name="resource">Specifies the location or service for which the credential is being stored.</param>
    /// <param name="username">Identifies the user associated with the credential being added.</param>
    /// <param name="password">Represents the secret key used for authentication with the specified user.</param>
    public static void AddPasswordCredential(string resource, string username, string password)
    {
        if (string.IsNullOrEmpty(resource) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }
        var vault = new PasswordVault();
        var credential = new PasswordCredential(resource, username, password);
        vault.Add(credential);
    }

    /// <summary>
    /// Removes a password credential associated with a specific resource and username from the password vault.
    /// </summary>
    /// <param name="resource">Identifies the resource for which the password credential is being removed.</param>
    /// <param name="username">Specifies the user account associated with the password credential to be removed.</param>
    public static void RemovePasswordCredential(string resource, string username)
    {
        if (string.IsNullOrEmpty(resource) || string.IsNullOrEmpty(username))
        {
            return;
        }
        var vault = new PasswordVault();
        var credential = vault.Retrieve(resource, username);
        vault.Remove(credential);
    }

    /// <summary>
    /// Requests verification of a Windows Hello PIN using a specified message. Returns a boolean indicating success or
    /// failure of the verification.
    /// </summary>
    /// <param name="message">Provides a message to guide the user on what credentials to enter.</param>
    /// <returns>A boolean indicating whether the Windows Hello PIN was successfully verified.</returns>
    public static async Task<bool> RequestWindowsPIN(string message)
    {
        // Check if Windows Hello is available
        if (await UserConsentVerifier.CheckAvailabilityAsync() != UserConsentVerifierAvailability.Available)
        {
            return false;
        }

        var consentResult = await UserConsentVerifier.RequestVerificationAsync(message);

        if (consentResult == UserConsentVerificationResult.Verified)
        {
            // Windows Hello PIN was successfully verified
            return true;
        }
        else
        {
            // Verification failed or was canceled
            return false;
        }
    }
}
