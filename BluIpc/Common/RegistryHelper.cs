using System;
using Microsoft.Win32;

namespace Blu.core.common
{
       public static class RegistryUtil
        {
            private static readonly RegistryKey BaseRegistryKey = Registry.LocalMachine;
            private const string SubKey = "SOFTWARE\\Blu";

            public static string ReadRegistryKey(string keyName)
            {
                RegistryKey rk = BaseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(SubKey);

                if (sk1 == null) return null;

                try
                {
                    return (string)sk1.GetValue(keyName);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
}
