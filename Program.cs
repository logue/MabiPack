using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
			if (args.Length == 0){
				Application.Run(new MainWindow());
			}else{
				// Drag & Drop Mode
				string Query = String.Join(" ",args);
#region Unpack
				if (File.Exists(Query) && System.IO.Path.GetExtension(@Query) == ".pack"){
					FolderBrowserDialog ExtractTo = new FolderBrowserDialog();
					ExtractTo.Description = "Select the directory where the extracted.";
					ExtractTo.RootFolder = System.Environment.SpecialFolder.Personal;
					if (ExtractTo.ShowDialog() == DialogResult.OK){
						WorkerWindow w = new WorkerWindow();
						w.Show();
						w.Unpack(Query, ExtractTo.SelectedPath, false);
						w.Dispose();
					}
				}
#endregion
				/*
				if (Directory.Exists(Query)){
					SaveFileDialog SaveAs = new SaveFileDialog();
					SaveAs.DefaultExt = "*.pack";
					SaveAs.InitialDirectory = "C:\\Nexon\\Mabinogi\\Package";

					if (SaveAs.ShowDialog() == DialogResult.OK){
						Form o = new MabiPacker.Forms.PackOption();
						o.ShowDialog();

						WorkerWindow w = new WorkerWindow();
						w.Show();
						w.Pack(Query, SaveAs.FileName, o.Level.);
						w.Dispose();
				}
				 */
			}
			
		}
	}
}
