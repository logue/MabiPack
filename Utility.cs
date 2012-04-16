using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace MabiPacker
{
	class Utility
	{
		public string MabiDir;
		public int MabiVer;

		public void GetMabiEnv(){
			this.MabiDir = GetMabiDir();
			this.MabiVer = GetMabiVer();
		}
		private String GetMabiDir(){
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
					if (regkey == null) return  "";
				}
			}
			string reg = (string)regkey.GetValue("");	// Returns Mabinogi Directory
			regkey.Close();
			return reg;
		}
		private int GetMabiVer() {
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
}
