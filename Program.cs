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
				Application.Run(new MainWindow());
			}
			
		}
	}
}
