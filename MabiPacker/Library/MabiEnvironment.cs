/*!
 * Mabinogi Environment Class
 * Copyright (C) 2012,2019 by Logue <http://logue.be/>
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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace MabiPacker.Library
{
    internal class MabiEnvironment
    {
        // Server Enviroment
        public bool isDownloadable = false;
        public uint ServerVersion = 0;
        public string Arg = null;
        public string LoginIP = "127.0.0.1";
        public Uri PatchServer = null;
        public uint Fullver = 0;
        public string LangPack = null;
        public string MabinogiDir = null;
        // Define
        private readonly uint Code = 1622;
        private readonly uint LoginPort = 11000;
        private readonly string CrackShieldBinName = "Solaris.exe";

        /// <summary>
        /// Get Mabinogi Environment
        /// </summary>
        public MabiEnvironment()
        {
            MabinogiDir = GetMabinogiDir();
            isDownloadable = false;
        }
        /// <summary>
        /// Get Mabinogi Environment
        /// </summary>
        /// <param name="url">Url to Patch.txt</param>
        public MabiEnvironment(string url)
        {
            MabinogiDir = GetMabinogiDir();
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
                Stream st = webres.GetResponseStream();
                Dictionary<string, string> p = ParsePatchText(st);

                isDownloadable = (p["patch_accept"] == "0") ? false : true;
                ServerVersion = uint.Parse(p["main_version"]);
                Arg = p["arg"];
                LoginIP = p["login"];
                LangPack = p.ContainsKey("lang") ? p["lang"] : ""; // language.pack

                // Maybe Korean server only.
                Fullver = p.ContainsKey("main_fullversion") ? uint.Parse(p["main_fullversion"]) : 0;
                PatchServer = new Uri(p["main_ftp"]);
                webres.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                isDownloadable = false;
            }
        }
        /// <summary>
        /// Get Mabinogi installed directory from Registory.
        /// </summary>
        /// <returns>Fullpath of Mabinogi directory</returns>
        private string GetMabinogiDir()
        {
            // Get Mabinogi Directory from Registory
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi", false);
            if (regkey == null)
            {
                regkey = Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_test", false);
                if (regkey == null)
                {
                    Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_hangame", false);
                    if (regkey == null)
                    {
                        return "C:\\Nexon\\Mabinogi";
                    }
                }
            }
            string reg = (string)regkey.GetValue("");   // Returns Mabinogi Directory
            regkey.Close();
            return reg;

        }
        /// <summary>
        /// Read Mabinogi Version from version.dat
        /// </summary>
        /// <returns>Local Version</returns>
        public uint LocalVersion
        {
            get
            {
                // Get Client Version from version.dat
                //string version_dat = MabiDir + "\\version.dat";
                string VersionDat = "version.dat";
                if (File.Exists(VersionDat))
                {
                    byte[] data = File.ReadAllBytes(VersionDat);
                    return BitConverter.ToUInt32(data, 0);
                }
                else if (File.Exists(MabinogiDir + "\\" + VersionDat))
                {
                    // binary is not exists same directory,
                    // load registory.
                    byte[] data = File.ReadAllBytes(MabinogiDir + "\\" + VersionDat);
                    return BitConverter.ToUInt32(data, 0);
                }
                return 0;
            }
        }

        /// <summary>
        /// Fetch and parse patch.txt
        /// </summary>
        /// <param name="url">URL to patch text.</param>
        /// <returns>Key-value data of patch.txt.</returns>
        private Dictionary<string, string> ParsePatchText(Stream st)
        {
            string line;
            // Fetch patch.txt
            Encoding enc = Encoding.GetEncoding("UTF-8");   // assume UTF-8
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
            sr.Close();
            return data;
        }
        /// <summary>
        /// Launch Mabinogi client. If Crackshild is detected, launch crackshield.
        /// Notice : The parent program MUST be put on the same directory as client.exe. 
        /// </summary>
        /// <param name="form">The window of a parent program.</param>
        /// <returns>When client is launched returns true, otherwise returns false.</returns>
        public bool Launch(string[] args)
        {
            string cArgs = "code:" + Code + " ver:" + LocalVersion +
                " logip:" + LoginIP + " logport:" + LoginPort + " " + Arg + " " + string.Join(" ", args);

            if (File.Exists("client.exe"))
            {
                if (File.Exists(CrackShieldBinName) &&
                    File.Exists("dinput8.dll") &&
                    Process.GetProcessesByName(CrackShieldBinName).Length == 0)
                {
                    Console.WriteLine("Detect CrackSheild. Launch CrackShield first...");
                    RunElevated(CrackShieldBinName, "", false);
                }

                Console.WriteLine("Command Line: client.exe " + cArgs);
                Console.WriteLine("Launch Mabinogi client.");

                // Multiple launch client is not checked. :)
                return RunElevated("client.exe", cArgs, false);
            }
            else
            {
                // Whwn detect CrackShield, launch CrackShield first.
                // If CrackShield process detected, ignore launch code.
                if (File.Exists(MabinogiDir + "\\" + CrackShieldBinName) &&
                    File.Exists(MabinogiDir + "\\dinput8.dll") &&
                    Process.GetProcessesByName(CrackShieldBinName).Length == 0)
                {
                    Console.WriteLine("Detect CrackSheild. Launch CrackShield first...");
                    RunElevated(MabinogiDir + "\\" + CrackShieldBinName, "", false);
                }
                Console.WriteLine("Command Line:" + MabinogiDir + "\\client.exe " + cArgs);
                Console.WriteLine("Launch Mabinogi client.");

                // Multiple launch client is not checked. :)
                if (File.Exists(MabinogiDir + "\\" + CrackShieldBinName))
                {
                    return RunElevated(MabinogiDir + "\\client.exe", cArgs, false);
                }
            }
            return false;
        }
        /// <summary>
        /// Launch other program as Administrator.
        /// </summary>
        /// <param name="fileName">Fullpath of program you would launch</param>
        /// <param name="arguments">Argument of program.</param>
        /// <param name="waitExit">Wait for program exit.</param>
        /// <returns>When launch succeed returns true, cancelled by UAC returns false.</returns>
        private static bool RunElevated(string fileName, string arguments, bool waitExit)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fileName,
                //動詞に「runas」をつける
                Verb = "runas",
                Arguments = arguments
            };

            try
            {
                Process p = Process.Start(psi);
                if (waitExit)
                {
                    p.WaitForExit();
                }
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}