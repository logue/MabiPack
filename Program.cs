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
#region Drag & Drop Mode
				Utility u = new Utility();
				u.GetMabiEnv();
				string Query = String.Join(" ", args);
				if (File.Exists(Query) && System.IO.Path.GetExtension(@Query) == ".pack")
				{
					// Unpack
					FolderBrowserDialog ExtractTo = new FolderBrowserDialog();
					ExtractTo.Description = "Select the directory where the extracted.";
					if (ExtractTo.ShowDialog() == DialogResult.OK)
					{
						WorkerWindow w = new WorkerWindow();
						w.Show();
						w.Unpack(Query, ExtractTo.SelectedPath);
						w.Dispose();
					}
				}
				else if (Directory.Exists(Query))
				{
					// Pack
					SaveFileDialog dSaveAs = new SaveFileDialog();
					dSaveAs.DefaultExt = "*.pack";
					dSaveAs.Filter = Properties.Resources.PackFileFilter;
					dSaveAs.InitialDirectory = u.MabiDir + "\\package\\";

					if (dSaveAs.ShowDialog() == DialogResult.OK)
					{
						MabiPacker.Forms.PackOption o = new MabiPacker.Forms.PackOption();
						MabiPacker.Forms.PackOption.Instance = o;
						if (o.ShowDialog() == DialogResult.OK)
						{
							WorkerWindow w = new WorkerWindow();
							w.Show();
							w.Pack(Query, dSaveAs.FileName, o.Version_Value, o.Level_Value);
							w.Dispose();
						}
					}
				}
#endregion
			}
#region Console Mode
			if (Win32.AttachConsole(System.UInt32.MaxValue) ){

				// Console Mode
				StreamWriter stdout = new StreamWriter(Console.OpenStandardOutput());
				stdout.AutoFlush = true;
				Console.SetOut(stdout);
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("\r\nMabiPacker Console Mode.");
				
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
					Application.Exit();
				}
				if (File.Exists(result["/input"]))
				{
					// Unpack mode
					Utility u = new Utility();
					u.GetMabiEnv();
					if (result.ContainsKey("/output") == false)
						result["/output"] = u.MabiDir;

					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("Unpacking...");
					// Pack mode
					WorkerWindow w = new WorkerWindow();
					w.Show();
					w.Unpack(result["/input"], result["/output"]);
					w.Dispose();
				}
				else if (Directory.Exists(result["/input"]) )
				{
					if (result.ContainsKey("/version") == false || result.ContainsKey("/output") == false){
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Error : If /input value is directory, /version and /output value is required.");
						Console.ResetColor();
						Application.Exit();
					}
					if (result.ContainsKey("/level") == false)
						result["/level"] = "-1";

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Packing...");
					// Pack mode
					WorkerWindow w = new WorkerWindow();
					w.Show();
					w.Pack(result["/input"], result["/output"], uint.Parse(result["/version"]), int.Parse(result["/level"]));
					w.Dispose();
				}
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("Finish.");
				Console.ResetColor();

				Win32.FreeConsole();
			}
#endregion
			Application.Exit();
		}
	}
	public class Win32{
		[DllImport("kernel32.dll")]
		public static extern Boolean AttachConsole(uint dwProcessId);
		[DllImport("kernel32.dll")]
		public static extern Boolean FreeConsole();
	}
}
