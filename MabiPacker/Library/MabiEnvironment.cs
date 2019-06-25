// Mabinogi Environment Class
// Copyright (c) 2012, 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        // Define
        private const uint Code = 1622;
        private const uint LoginPort = 11000;
        private readonly string[] RegistoryKeys =
        {
            // Default Mabinogi Location
            @"Software\Nexon\Mabinogi",
            // For Test Client
            @"Software\Nexon\Mabinogi_test",
            // For Hangame Client (JP only)
            @"Software\Nexon\Mabinogi_hangame",
        };
        private readonly string[] RegistoryValues =
        {
            "ExecutablePath",
            "LauncherPath",
            ""
        };
        /// <summary>
        /// Constructor
        /// </summary>
        public MabiEnvironment()
        {
            isDownloadable = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">Url to Patch.txt</param>
        public MabiEnvironment(string url)
        {
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                using (HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse())
                {
                    using (Stream st = webres.GetResponseStream())
                    {
                        Dictionary<string, string> p = ParsePatchText(st);

                        isDownloadable = (p["patch_accept"] == "0") ? false : true;
                        ServerVersion = uint.Parse(p["main_version"]);
                        Arg = p["arg"];
                        LoginIP = p["login"];
                        LangPack = p.ContainsKey("lang") ? p["lang"] : ""; // language.pack

                        // Maybe Korean server only.
                        Fullver = p.ContainsKey("main_fullversion") ? uint.Parse(p["main_fullversion"]) : 0;
                        PatchServer = new Uri(p["main_ftp"]);
                    }
                }
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
        public string MabinogiDir
        {
            get
            {
                // Get Mabinogi Directory from Registory
                foreach (string key in RegistoryKeys)
                {
                    using (RegistryKey regkey = Registry.CurrentUser.OpenSubKey(key, false))
                    {
                        if (regkey != null)
                        {
                            // Get registoy value.
                            foreach (string value in RegistoryValues)
                            {
                                string path = (string)regkey.GetValue(value);
                                if (path != null)
                                {
                                    return path;
                                }
                            }
                        }
                    }
                }
                throw new WarningException("Could not detect Mabinogi Directory. Mabinogi is installed collectly?");
            }
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
        /// <param name="st">Stream of patch.txt</param>
        /// <returns>Key-value data of patch.txt.</returns>
        private Dictionary<string, string> ParsePatchText(Stream st)
        {
            // Fetch patch.txt
            Encoding enc = Encoding.GetEncoding("UTF-8");   // assume UTF-8
            using (StreamReader sr = new StreamReader(st, enc))
            {
                string line;
                Dictionary<string, string> data = new Dictionary<string, string>();
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0 || line[0].Equals('#'))
                    {
                        // skip comment line
                        continue;
                    }
                    string[] result = line.Split(new char[] { '=' }, 2);
                    if (result.Length == 2)
                    {
                        data.Add(result[0], result[1]);
                    }
                }
                return data;
            }
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

            if (!File.Exists("client.exe"))
            {
                throw new FileNotFoundException("Could not detect client.exe. This program must be Mabinogi.exe same directory.");
            }

            Console.WriteLine("Command Line: client.exe " + cArgs);
            Console.WriteLine("Launch Mabinogi client.");

            // Multiple launch client is not checked. :)
            return RunElevated("client.exe", cArgs, false);
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

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = fileName,
                    Verb = "runas",
                    Arguments = arguments
                };

                using (Process p = Process.Start(psi))
                {
                    if (waitExit)
                    {
                        p.WaitForExit();
                    }
                }

            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}