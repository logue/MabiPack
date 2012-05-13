using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MabiPacker
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Standard Mode
			if (args.Length == 0)
			{
				Application.Run(new MainWindow());
			}
			else 
			{
				if (Win32.AttachConsole(System.UInt32.MaxValue)){
					Utility.Worker w = new Utility.Worker(IntPtr.Zero);
					Console.Title = "MabiPacker";

					// Console Mode
					StreamWriter stdout = new StreamWriter(Console.OpenStandardOutput());
					stdout.AutoFlush = true;

					Console.SetOut(stdout);
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine("\r\n*** MabiPacker Console Mode ***");

					// Parse query strings
					var options = new HashSet<string> {
						"/input",	// input path
						"/output",	// output path
						"/version",	// version
						"/level"	// Compress level (optional, default=-1)
					};
					string key = null;
					var result = args
						.GroupBy(s => options.Contains(s) ? key = s : key)
						.ToDictionary(g => g.Key, g => g.Skip(1).FirstOrDefault());

					if (result.ContainsKey("/input") == false)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Error: /input value is always required!");
						Console.ResetColor();
						Application.Exit();
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
						// Pack mode
						w.Unpack(result["/input"], result["/output"]);
					}
					else if (Directory.Exists(result["/input"]))
					{
						if (result.ContainsKey("/version") == false || result.ContainsKey("/output") == false)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Error : If /input value is directory, /version and /output value is required.");
							Console.ResetColor();
							Application.Exit();
						}
						if (result.ContainsKey("/level") == false)
							result["/level"] = "-1";

						Console.ForegroundColor = ConsoleColor.Green;
						// Pack mode
						w.Pack(result["/input"], result["/output"], uint.Parse(result["/version"]), int.Parse(result["/level"]));
					}
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine("Finish.");
					Console.ResetColor();
					Win32.FreeConsole();
				}else{
					string Query = String.Join(" ", args);
					if (File.Exists(Query) && System.IO.Path.GetExtension(@Query) == ".pack")
					{
						Application.Run(new PackBrowser(Query));
					}
				}
			}
		}
		private class Win32
		{
			[DllImport("kernel32.dll")]
			public static extern Boolean AttachConsole(uint dwProcessId);
			[DllImport("kernel32.dll")]
			public static extern Boolean FreeConsole();
		}
	}
}
