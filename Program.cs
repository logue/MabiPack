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
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainWindow());

		}
	}
}
