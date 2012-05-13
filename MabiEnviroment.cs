/*!
 * Mabinogi Environment Class
 * Copyright (C) 2012 by Logue <http://logue.be/>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License version 3
 * as published by the Free Software Foundation.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;

namespace MabiPacker
{
	class MabiEnvironment
	{
		// Local Enviroment
		public string MabinogiDir;
		public uint LocalVersion;
		// Server Enviroment
		public bool isDownloadable;
		public uint Version;
		public string Arg;
		public string LoginIP;
		public Uri PatchServer;
		public uint Fullver;
		public string LangPack;

		/// <summary>
		/// Get Mabinogi Environment
		/// </summary>
		/// <param name="url">Url to Patch.txt</param>
		public MabiEnvironment()
		{
			this.MabinogiDir = GetMabiDir();
			this.LocalVersion = GetMabiVer();

			Dictionary<string, string> p = PatchText(Properties.Resources.Uri_PatchTxt);
			this.isDownloadable = (p["patch_accept"] == "0") ? false : true;
			this.Version = uint.Parse(p["main_version"]);
			this.Arg = p["arg"];
			this.LoginIP = p["login"];
			this.Fullver = p.ContainsKey("main_fullversion") ? uint.Parse(p["main_fullversion"]) : 0;	// Maybe Korean server only.
			this.LangPack = p.ContainsKey("lang") ? p["lang"] : "";
			// Notice:
			// * US server seems not read main_ftp value.
			// * In some countries in the HTTP patch to drop, in addition to the beginning of the address http://, to pass to the URI object.
			// * Whether FTP is to be determined by the port.
			this.PatchServer = new Uri("http://" + (p["main_ftp"] == "mabipatch.nexon.net/game/" ? "mabipatch.nexon.net" : p["main_ftp"]) );
		}
		/// <summary>
		/// Get Mabinogi installed directory from Registory.
		/// </summary>
		/// <returns>Fullpath of Mabinogi directory</returns>
		private String GetMabiDir()
		{
			// Get Mabinogi Directory from Registory
			RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi", false);
			if (regkey == null)
			{
				regkey = Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_test", false);
				if (regkey == null)
				{
					Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_hangame", false);
					if (regkey == null) return "";
				}
			}
			string reg = (string)regkey.GetValue("");	// Returns Mabinogi Directory
			regkey.Close();
			return reg;
		}
		/// <summary>
		/// Read Mabinogi Version from version.dat
		/// </summary>
		/// <returns>Local Version</returns>
		private uint GetMabiVer()
		{
			String MabiDir = this.GetMabiDir();
			// Get Client Version from version.dat
			string version_dat = MabiDir + "\\version.dat";
			if (File.Exists(version_dat))
			{
				byte[] data = File.ReadAllBytes(version_dat);
				return BitConverter.ToUInt32(data, 0);
			}
			else
			{
				return 0;
			}
		}
		/// <summary>
		/// Fetch and parse patch.txt
		/// </summary>
		/// <param name="url">URL to patch text.</param>
		/// <returns>Key-value data of patch.txt.</returns>
		private Dictionary<string, string> PatchText(string url)
		{
			string line = "";
			ArrayList al = new ArrayList();
			HttpWebRequest webreq = (HttpWebRequest) WebRequest.Create(url);
			HttpWebResponse webres = (HttpWebResponse) webreq.GetResponse();
			Stream st = webres.GetResponseStream();

			// Fetch patch.txt
			Encoding enc = Encoding.GetEncoding("UTF-8");	// assume UTF-8
			StreamReader sr = new StreamReader(st, enc);

			Dictionary<string, string> data = new Dictionary<string, string>();
			while ((line = sr.ReadLine()) != null)
			{
				if (line.Trim().Length == 0 || line[0].Equals('#'))
				{
					continue;
				}
				string[] result = line.Split(new char[] { '=' }, 2);
				if (result.Length == 2)
				{
					data.Add(result[0], result[1]);
				}
			}
			webres.Close();
			sr.Close();
			return data;
		}
		/// <summary>
		/// Launch Mabinogi client. If Crackshild is detected, launch crackshield.
		/// Notice : The parent program MUST be put on the same directory as client.exe. 
		/// </summary>
		/// <param name="form">The window of a parent program.</param>
		/// <returns>When client is launched returns true, otherwise returns false.</returns>
		public bool Launch(Form form){
			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
			if (System.IO.File.Exists(this.MabinogiDir + "\\HSLaunch.exe") && System.Diagnostics.Process.GetProcessesByName("HSLaunch.exe").Length == 0)
			{
				Console.WriteLine("Detect CrackSheild. Launch CrackShield first...");
				RunElevated(this.MabinogiDir + "\\HSLaunch.exe", "", form, false);
			}
			String cArgs = "code:1622 ver:" + this.LocalVersion + " logip:" + this.LoginIP + " logport:11000 " + this.Arg;
			Console.WriteLine("Command Line:" + MabinogiDir + "\\client.exe " + cArgs);
			Console.WriteLine("Launch Mabinogi client.");
			return RunElevated(this.MabinogiDir + "\\client.exe", cArgs, form, false);
		}
		/// <summary>
		/// Launch other program as Administrator.
		/// </summary>
		/// <param name="fileName">Fullpath of program you would launch</param>
		/// <param name="arguments">Argument of program.</param>
		/// <param name="parentForm">The window of a parent program.</param>
		/// <param name="waitExit">Wait for program exit.</param>
		/// <returns>When launch succeed returns true, cancelled by UAC returns false.</returns>
		private static bool RunElevated(string fileName, string arguments, Form parentForm, bool waitExit)
		{
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException();
			}

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.UseShellExecute = true;
			psi.FileName = fileName;
			//動詞に「runas」をつける
			psi.Verb = "runas";
			psi.Arguments = arguments;

			if (parentForm != null)
			{
				psi.ErrorDialog = true;
				psi.ErrorDialogParentHandle = parentForm.Handle;
			}

			try
			{
				Process p = Process.Start(psi);
				if (waitExit)
				{
					p.WaitForExit();
				}
			}
			catch (System.ComponentModel.Win32Exception)
			{
				return false;
			}

			return true;
		}
	}
}