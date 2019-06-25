// MabiPacker
// Copyright (c) 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using MabiPacker.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MabiPacker
{
    internal class Cui
    {
        private readonly HashSet<string> Options = new HashSet<string>
        {
            "/input",   // input path
            "/output",  // output path
            "/version", // version
            "/level",   // Compress level (optional, default=-1)
            "/help"
        };
        private readonly Dictionary<string, string> result;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        public Cui(string[] args)
        {
            Console.Title = "MabiPacker";
            // Console Mode
            StreamWriter stdout = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(stdout);
            Console.Clear();
            Console.WriteLine("*** MabiPacker Console Mode ***\r\n");

            string key = "";
            result = args.GroupBy(s => Options.Contains(s) ? key = s : key).ToDictionary(g => g.Key, g => g.Skip(1).FirstOrDefault());

            //foreach (var r in result)
            //{
            //    Console.WriteLine(r.Key + ": " + r.Value);
            //}

            if (result.ContainsKey("/help") || result.ContainsKey("/input") == false)
            {
                Help();
            }
            else if (File.Exists(result["/input"]))
            {
                Console.WriteLine("Input:" + result["/input"]);
                Unpack();
            }
            else if (Directory.Exists(result["/input"]))
            {
                Console.WriteLine("Input:" + result["/input"]);
                Pack();
            }
            else
            {
                ErrorMessage("File or Directory is not found.");
            }
            FinishMessage();
        }
        private void FinishMessage()
        {
            Console.ResetColor();
            Console.WriteLine("\r\n*** Press any key to exit. ***");
        }
        /// <summary>
        /// Display Error Message
        /// </summary>
        /// <param name="message"></param>
        private void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message + "\r\n");
            Console.ResetColor();
        }
        /// <summary>
        /// Display Help
        /// </summary>
        private void Help()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Usage:");
            Console.WriteLine("   /input    (Required.) When this value is file, MabiPacker's judgment is Unpack. Otherwise Pack.");
            Console.WriteLine("   /output   (When packing is required.) Output directory. ");
            Console.WriteLine("             If you omit this when you unpack, you are in the Package output directory of Mabinogi.");
            Console.WriteLine("   /version  (When packing is required.) Package file version.");
            Console.WriteLine("   /level    Compression Level. Packing only. default value is -1 (Auto)");
            Console.WriteLine("   /help     Display this message.");
            Console.ResetColor();
        }
        /// <summary>
        /// Process Pack
        /// </summary>
        private void Pack()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pack Mode.\r\n");
            Console.ResetColor();

            if (result.ContainsKey("/version") == false)
            {
                ErrorMessage("/version value is required.");
                return;
            }
            if (result.ContainsKey("/level") == false)
            {
                result["/level"] = "-1";
            }
            if (result.ContainsKey("/output") == false)
            {
                MabiEnvironment u = new MabiEnvironment();
                result["/output"] = u.MabinogiDir + "\\package\\custom-" + result["/version"] + ".pack";
            }

            // Pack mode
            using (Packer packer = new Packer(result["/output"], result["/input"], uint.Parse(result["/version"]), int.Parse(result["/level"])))
            {
                Progress<Entry> p = new Progress<Entry>((Entry entry) =>
                {
                    Console.WriteLine(
                        string.Format("{1}/{2}: {0}", entry.Name, entry.Index, packer.Count())
                    );
                });
                packer.Pack(p);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\r\nFinish.\r\n");
            }

        }
        /// <summary>
        /// Process Unpack
        /// </summary>
        private void Unpack()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Unpack Mode.\r\n");
            Console.ResetColor();

            if (result.ContainsKey("/output") == false)
            {
                result["/output"] = Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0]));
            }

            using (Unpacker unpacker = new Unpacker(result["/input"], result["/output"]))
            {
                Progress<Entry> p = new Progress<Entry>((Entry entry) =>
                {
                    Console.WriteLine(
                        string.Format("{1}/{2}: {0}", entry.Name, entry.Index, unpacker.Count())
                    );
                });
                unpacker.Unpack(p);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\r\nFinish.\r\n");
            }

        }
    }
}
