using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace AutoRunManager;

public static class RegistryHelper
{
    public static bool IsWindows()
    {
        return OperatingSystem.IsWindows();
    }

    public static string[] ReadValueNames(string keyPath)
    {
        if (!IsWindows()) return Array.Empty<string>();
        
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key?.GetValueNames() ?? Array.Empty<string>();
        }
    }

    public static object ReadValue(string keyPath, string valueName)
    {
        if (!IsWindows()) return null;
        
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key?.GetValue(valueName);
        }
    }

    public static void WriteValue(string keyPath, string valueName, object value)
    {
        if (!IsWindows()) return;
        
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
        {
            key?.SetValue(valueName, value);
        }
    }

    public static void DeleteValue(string keyPath, string valueName)
    {
        if (!IsWindows()) return;
        
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
        {
            if (key?.GetValue(valueName) != null)
            {
                key.DeleteValue(valueName);
            }
        }
    }

    public static bool KeyExists(string keyPath)
    {
        if (!IsWindows()) return false;
        
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key != null;
        }
    }

    public static void DeleteKey(string keyPath)
    {
        if (!IsWindows()) return;
        
        using (RegistryKey parentKey = Registry.CurrentUser.OpenSubKey(Path.GetDirectoryName(keyPath), true))
        {
            parentKey?.DeleteSubKey(Path.GetFileName(keyPath), false);
        }
    }

    public static string GetValueKind(string keyPath, string valueName)
    {
        if (!IsWindows()) return string.Empty;
        
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key?.GetValueKind(valueName).ToString() ?? string.Empty;
        }
    }

    public static bool SetRegistryKeyAccessPermissions(string keyPath)
    {
        if (!IsWindows()) return false;

        var user = Path.Combine(Environment.UserDomainName, Environment.UserName);
        var registrySecurity = new RegistrySecurity();

        var accessRule = new RegistryAccessRule(user,
            RegistryRights.ReadKey | RegistryRights.WriteKey,
            InheritanceFlags.None,
            PropagationFlags.None,
            AccessControlType.Allow);

        registrySecurity.AddAccessRule(accessRule);

        using var subKey = Registry.CurrentUser.CreateSubKey(keyPath, RegistryKeyPermissionCheck.Default, registrySecurity);

        if (subKey == null) return false;

        var isAdded = false;
        var accessControl = subKey.GetAccessControl();
        var accessRules = accessControl.GetAccessRules(true, true, typeof(NTAccount));

        foreach (RegistryAccessRule rule in accessRules)
        {
            if (rule.IdentityReference.Value == user)
            {
                isAdded = true;
                break;
            }
        }

        return isAdded;
    }

    public static bool OpenRemoteBaseKey(string machineName)
    {
        if (!IsWindows()) return false;

        try
        {
            var remoteBaseKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, machineName);
            return remoteBaseKey != null;
        }
        catch
        {
            return false;
        }
    }
}