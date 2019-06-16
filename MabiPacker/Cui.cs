using MabiPacker.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        };
        public Cui(string[] args)
        {
            Console.Title = "MabiPacker";
            // Console Mode
            StreamWriter stdout = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(stdout);
            Console.WriteLine("\r\n*** MabiPacker Console Mode ***\r\n");

            string key = null;
            Dictionary<string, string> result = args
                .GroupBy(s => Options.Contains(s) ? key = s : key)
                .ToDictionary(g => g.Key, g => g.Skip(1).FirstOrDefault());

            if (result.ContainsKey("/input") == false)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("   /input    (Required.) When this value is file, MabiPacker's");
                Console.WriteLine("             judgment is Unpack. Otherwise Pack.");
                Console.WriteLine("   /output   (When packing is required.) Output directory. ");
                Console.WriteLine("             If you omit this when you unpack, you are in the ");
                Console.WriteLine("             Package output directory of Mabinogi.");
                Console.WriteLine("   /version  (When packing is required.) Package file version.");
                Console.WriteLine("   /level    Compression Level. Packing only. default value is -1 (Auto)");
                Console.ResetColor();
                return;
            }
            if (File.Exists(result["/input"]))
            {
                // Unpack mode
                if (result.ContainsKey("/output") == false)
                {
                    MabiEnvironment u = new MabiEnvironment();
                    result["/output"] = u.MabinogiDir;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Unpack Mode.\r\n");
                Console.ResetColor();

                Unpacker unpacker = new Unpacker(result["/input"], result["/output"]);
                Progress<Entry> p = new Progress<Entry>((Entry entry) =>
                {
                    Console.WriteLine(
                        string.Format("{1}/{2}: {0}", entry.Name, entry.Index, unpacker.Count())
                    );
                });
                Task.Run(() => unpacker.Unpack(p));
            }
            else if (Directory.Exists(result["/input"]))
            {
                if (result.ContainsKey("/version") == false || result.ContainsKey("/output") == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: If /input value is directory, /version and /output value is required.");
                    Console.ResetColor();
                    return;
                }
                if (result.ContainsKey("/level") == false)
                {
                    result["/level"] = "-1";
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Pack Mode.\r\n");
                Console.ResetColor();
                // Pack mode
                Packer packer = new Packer(result["/input"], result["/output"], uint.Parse(result["/version"]), int.Parse(result["/level"]));

                Progress<Entry> p = new Progress<Entry>((Entry entry) =>
                {
                    Console.WriteLine(
                        string.Format("{1}/{2}: {0}", entry.Name, entry.Index, packer.Count())
                    );
                });
                Task.Run(() => packer.Pack(p));
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\r\nFinish.\r\n");
            Console.ResetColor();
        }
    }
}
