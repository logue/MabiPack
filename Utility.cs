using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace MabiPacker
{
	class Utility
	{
        public class MabinogiEnv
        {
            public string MabiDir;
            public int MabiVer;

            public void GetMabiEnv()
            {
                this.MabiDir = GetMabiDir();
                this.MabiVer = GetMabiVer();
            }
            private String GetMabiDir()
            {
                // Get Mabinogi Directory from Registory
                Microsoft.Win32.RegistryKey regkey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi", false);
                if (regkey == null)
                {
                    regkey =
                        Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_test", false);
                    if (regkey == null)
                    {
                        Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_hangame", false);
                        if (regkey == null) return "";
                    }
                }
                string reg = (string)regkey.GetValue("");	// Returns Mabinogi Directory
                regkey.Close();
                return reg;
            }
            private int GetMabiVer()
            {
                String MabiDir = this.GetMabiDir();
                // Get Client Version from version.dat
                string version_dat = MabiDir + "\\version.dat";
                if (File.Exists(version_dat))
                {
                    byte[] data = File.ReadAllBytes(version_dat);
                    return BitConverter.ToInt32(data, 0);
                }
                else
                {
                    return 0;
                }
            }
        }
        public class FileAssociation
        {
            // Associate file extension with progID, description, icon and application
            public static void Associate(string extension,
                   string progID, string description, string icon, string application)
            {
                Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
                if (progID != null && progID.Length > 0)
                    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
                    {
                        if (description != null)
                            key.SetValue("", description);
                        if (icon != null)
                            key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
                        if (application != null)
                            key.CreateSubKey(@"Shell\Open\Command").SetValue("",ToShortPathName(application) + " \"%1\"");
                    }
            }

            // Return true if extension already associated in registry
            public static bool IsAssociated(string extension)
            {
                return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
            }

            [DllImport("Kernel32.dll")]
            private static extern uint GetShortPathName(string lpszLongPath,
                [Out] StringBuilder lpszShortPath, uint cchBuffer);

            // Return short path format of a file name
            private static string ToShortPathName(string longName)
            {
                StringBuilder s = new StringBuilder(1000);
                uint iSize = (uint)s.Capacity;
                uint iRet = GetShortPathName(longName, s, iSize);
                return s.ToString();
            }
        }
	}
}
