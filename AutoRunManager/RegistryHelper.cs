using Microsoft.Win32;

namespace AutoRunManager;

public class RegistryHelper
{
    public static string[] ReadValueNames(string keyPath)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key?.GetValueNames();
        }
    }

    public static object ReadValue(string keyPath, string valueName)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key?.GetValue(valueName);
        }
    }

    public static void WriteValue(string keyPath, string valueName, object value)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
        {
            key?.SetValue(valueName, value);
        }
    }

    public static void DeleteValue(string keyPath, string valueName)
    {
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
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        {
            return key != null;
        }
    }

    public static void DeleteKey(string keyPath)
    {
        using (RegistryKey parentKey = Registry.CurrentUser.OpenSubKey(Path.GetDirectoryName(keyPath), true))
        {
            parentKey?.DeleteSubKey(Path.GetFileName(keyPath), false);
        }
    }
}