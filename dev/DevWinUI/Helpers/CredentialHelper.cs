using Windows.Security.Credentials.UI;
using Windows.Security.Credentials;

namespace DevWinUI;
public static partial class CredentialHelper
{
    private static async Task<CredentialPickerResults> PickCredentialBase(string caption, string message, CredentialSaveOption credentialSaveOption, AuthenticationProtocol authenticationProtocol, bool alwaysDisplayDialog)
    {
        uint MAX_Length = 260;
        Span<char> buffer = stackalloc char[(int)MAX_Length];

        string computerName = string.Empty;

        unsafe
        {
            fixed (char* pBuffer = buffer)
            {
                int result = PInvoke.GetComputerName(pBuffer, ref MAX_Length);
                if (result > 0)
                {
                    computerName = new string(pBuffer, 0, result);
                }
            }
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
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message, CredentialSaveOption credentialSaveOption, AuthenticationProtocol authenticationProtocol, bool alwaysDisplayDialog)
    {
        return await PickCredentialBase(caption, message, credentialSaveOption, authenticationProtocol, alwaysDisplayDialog);
    }
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message, CredentialSaveOption credentialSaveOption, AuthenticationProtocol authenticationProtocol)
    {
        return await PickCredentialBase(caption, message, credentialSaveOption, authenticationProtocol, true);
    }
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message, CredentialSaveOption credentialSaveOption)
    {
        return await PickCredentialBase(caption, message, credentialSaveOption, AuthenticationProtocol.Basic, true);
    }
    public static async Task<CredentialPickerResults> PickCredential(string caption, string message)
    {
        return await PickCredentialBase(caption, message, CredentialSaveOption.Selected, AuthenticationProtocol.Basic, true);
    }

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
