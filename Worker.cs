using System;
using System.IO;
using System.Text.RegularExpressions;
using MabinogiResource;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MabiPacker
{
	class Worker{
		private ProgressDialog pd;
		private PackResourceSetCreater m_Pack;
		private PackResourceSet m_Unpack;
		private MabiEnvironment env;
		private string MabiDir;
		private bool isVista;
		private bool isCLI;
		/// <summary>
		/// Initialize Worker Progress Window.
		/// </summary>
		/// <param name="ShowDoneMsg">Show MessageBox and Progress. (default is true)</param>
		public Worker (bool isCLI){
			this.isCLI = isCLI;
			this.env = new MabiEnvironment();
			this.MabiDir = env.MabinogiDir;
			this.isVista = (Environment.OSVersion.Version.Major >= 6) ? true : false;

			if (!this.isCLI){
				// Set PrgressDialog
				this.pd = new ProgressDialog();
				this.pd.Title = "MabiPacker";
				this.pd.Value = 0;
				this.pd.Caption = Properties.Resources.Str_Initialize;
			}
		}
		public Worker()
		{
			this.isCLI = true;
		}
		/// <summary>
		/// Packing Package file process.
		/// </summary>
		/// <param name="InputDir">Set distnation of data directory for pack.</param>
		/// <param name="OutputFile">Set filename of outputted *.pack file, with path.</param>
		/// <param name="OutputVer">Set version of *.pack file.</param>
		/// <param name="OutputVer">Set compress level of *.pack file.</param>
		public void Pack(string InputDir, string OutputFile, uint OutputVer, int Level=-1)
		{
			if (!isCLI)
			{
				this.pd.ShowDialog(ProgressDialog.PROGDLG.Normal);
				this.pd.Caption = Properties.Resources.Str_Pack;

			}else{
				Console.WriteLine("Pack");
			}
			String internal_filename = "";
			if (File.Exists(OutputFile))
			{
				System.IO.File.Delete(@OutputFile);
			}
			// Get Filelist
			string[] filelist = Directory.GetFiles(InputDir, "*", SearchOption.AllDirectories);
			Array.Sort(filelist);

			if (!isCLI)
			{
				this.pd.Maximum = (uint)filelist.Length;
				this.pd.Value = 0;
				if (this.pd.HasUserCancelled)
				{
					Interrupt();
					return;
				}
			}
			// Instance
			m_Pack = new PackResourceSetCreater(OutputVer, Level);
			uint v=0;
			// store file list for pack
			foreach (string path in filelist)
			{
					
				internal_filename = path.Replace(InputDir + "\\", "");
				m_Pack.AddFile(internal_filename, path);
				if (!isCLI)
				{
					this.pd.Value = v;
					this.pd.Message = String.Format("Now checking file...({0} / {1})", v, filelist.Length);
					this.pd.Detail = internal_filename;
					if (this.pd.HasUserCancelled)
					{
						m_Pack.Dispose();
						Interrupt();
						return;
					}
				}else{
					Console.WriteLine( String.Format("{0} / {1} {2}", v, filelist.Length, internal_filename));
				}
				v++;
			}
			// Start packing
			if (!isCLI)
			{
				this.pd.Message = Properties.Resources.Str_Packing;
			}else{
				Console.WriteLine("Now Packing...");
			}
			m_Pack.CreatePack(OutputFile);
			m_Pack.Dispose();
			if (!isCLI)
			{
				this.pd.CloseDialog();
			}
			else
			{
				Console.WriteLine("Finish.");
			}
			
		}
		/// <summary>
		/// Unpacking Package file process.
		/// </summary>
		/// <param name="InputFile">Set filename of unpack file..</param>
		/// <param name="OutputDir">Set output distnation of Unpacked files.</param>
		public void Unpack(string InputFile , string OutputDir)
		{
			if (!isCLI)
			{
				this.pd.ShowDialog(ProgressDialog.PROGDLG.Normal);
				this.pd.Caption = Properties.Resources.Str_Unpack;
			}else{
				Console.WriteLine("Unpack");
			}
				
			m_Unpack = PackResourceSet.CreateFromFile(InputFile);

			uint packed_files = m_Unpack.GetFileCount();
			if (!isCLI)
			{
				pd.Maximum = packed_files;
				if (this.pd.HasUserCancelled)
				{
					m_Unpack.Dispose();
					this.pd.CloseDialog();
					return;
				}
			}

			for (uint i = 0; i < packed_files; ++i)
			{
				PackResource Res = m_Unpack.GetFileByIndex(i);
				String InternalName = Res.GetName();

				if (!isCLI)
				{
					this.pd.Message = String.Format(Properties.Resources.Str_Unpacking, i, packed_files);
					this.pd.Detail = InternalName;
					this.pd.Value = i;
					if (pd.HasUserCancelled)
					{
						m_Unpack.Dispose();
						Interrupt();
						return;
					}
				}else{
					Console.WriteLine(String.Format("{0}/{1} {2}", i, packed_files, InternalName));
				}
				// loading file content.
				byte[] buffer = new byte[Res.GetSize()];
				Res.GetData(buffer);
				Res.Close();

				// Get output Directory Name
				String outputPath = @OutputDir + "\\data\\" + InternalName;

				// Create directory
				String DirPath = Regex.Replace(outputPath, @"([^\\]*?)$", "");
				if (!Directory.Exists(DirPath))
				{
					Directory.CreateDirectory(DirPath);
				}

				// Delete old
				if (File.Exists(outputPath))
				{
					File.Delete(@outputPath);
				}
				if (Directory.Exists(outputPath))
				{
					Directory.Delete(@outputPath);
				}
				// Write to file.
				FileStream fs = new FileStream(outputPath, System.IO.FileMode.Create);
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();

				// Modify File time
				File.SetCreationTime(outputPath, Res.GetCreated());
				File.SetLastAccessTime(outputPath, Res.GetAccessed());
				File.SetLastWriteTime(outputPath, Res.GetModified());
			}
			m_Unpack.Dispose();
			Console.WriteLine("Finish.");
		}
		/// <summary>
		/// Unpacking file
		/// </summary>
		/// <param name="Res">PackResource </param>
		public bool UnpackFile(PackResource Res){
			if (!isCLI)
			{
				this.pd.ShowDialog(ProgressDialog.PROGDLG.Normal);
				this.pd.Caption = Properties.Resources.Str_Unpack;
			}
			else
			{
				Console.WriteLine("Unpack");
			}
			try{
				String InternalName = Res.GetName();
				CommonSaveFileDialog dSaveAs = new CommonSaveFileDialog();
				dSaveAs.DefaultFileName = Path.GetFileName(InternalName);

				if (dSaveAs.ShowDialog() == CommonFileDialogResult.Ok)
				{
					// loading file content.
					byte[] buffer = new byte[Res.GetSize()];
					Res.GetData(buffer);
					Res.Close();

					// Delete old
					if (File.Exists(dSaveAs.FileName))
					{
						File.Delete(dSaveAs.FileName);
					}
					// Write to file.
					FileStream fs = new FileStream(dSaveAs.FileName, System.IO.FileMode.Create);
					fs.Write(buffer, 0, buffer.Length);
					fs.Close();

					// Modify File time
					File.SetCreationTime(dSaveAs.FileName, Res.GetCreated());
					File.SetLastAccessTime(dSaveAs.FileName, Res.GetAccessed());
					File.SetLastWriteTime(dSaveAs.FileName, Res.GetModified());
				}
				return true;

			}catch(Exception){
				return false;
			}finally{
				Res.Close();
				Console.WriteLine("Finish.");
			}
		}
			
		/// <summary>
		/// Show message box when process aborted.
		/// </summary>
		private void Interrupt(){
			if (!isCLI)
			{
				this.pd.CloseDialog();
				/*
				TaskDialog td = new TaskDialog();
				TaskDialogStandardButtons button = TaskDialogStandardButtons.Close;
				td.Icon = TaskDialogStandardIcon.Information;
				td.StandardButtons = button;
				td.InstructionText = Properties.Resources.Info;
				td.Caption = Properties.Resources.Info;
				td.Text = Properties.Resources.Str_Interrupt;
				TaskDialogResult res = td.Show();
					*/ 
			}else{
				Console.WriteLine("Interrupted!");
			}
		}
	}
}
