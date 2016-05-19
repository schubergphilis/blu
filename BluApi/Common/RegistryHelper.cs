using System;
using Microsoft.Win32;

namespace BluApi.Common
{
    /// <summary>
    /// Win32 registy helper class
    /// </summary>
    public class RegistryHelper
    {
        /// <summary>
        /// Subkey, default to HKLM\Software\Blu
        /// </summary>
        private string _subKey = "SOFTWARE\\Blu";
        public string SubKey
        {
            get { return _subKey; }
            set { _subKey = value; }
        }
        private RegistryKey _baseRegistryKey = Registry.LocalMachine;
        public RegistryKey BaseRegistryKey
        {
            get { return _baseRegistryKey; }
            set { _baseRegistryKey = value; }
        }
        /// <summary>
        /// Reads a registry value
        /// </summary>
        /// <param name="keyName">registy key as string</param>
        /// <returns></returns>
        public string Read(string keyName)
        {
            RegistryKey rk = _baseRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(_subKey);
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)sk1.GetValue(keyName);
                }
                catch (Exception ex)
                {
                    Logger.log("error", "Error reading registry key " + keyName + " - " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Writes a value to registry
        /// </summary>
        /// <param name="keyName">registry key as string</param>
        /// <param name="value">registry value as object</param>
        /// <returns></returns>
        public bool Write(string keyName, object value)
        {
            try
            {
                RegistryKey rk = _baseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(_subKey);
                if (sk1 != null) sk1.SetValue(keyName, value);
                return true;
            }
            catch (Exception ex)
            {
                Logger.log("error", "Error writing registry key " + keyName + " - " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes a subkey from registry
        /// </summary>
        /// <returns>true if success, false if failed</returns>
        public bool DeleteSubKeyTree()
        {
            try
            {
                RegistryKey rk = _baseRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(_subKey);
                if (sk1 != null) rk.DeleteSubKeyTree(_subKey);
                return true;
            }
            catch (Exception ex)
            {
                Logger.log("error", "Error deleting registry key " + _subKey + " - " + ex.Message);
                return false;
            }
        }
    }
}
