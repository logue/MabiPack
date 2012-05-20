using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MabinogiResource;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;

namespace MabiPacker
{
	class Utility
	{
		public class FileAssociation
		{
			[DllImport("Kernel32.dll")]
			private static extern uint GetShortPathName(string lpszLongPath,
				[Out] StringBuilder lpszShortPath, uint cchBuffer);
			
			/// <summary>
			/// Associate file extension with progID, description, icon and application
			/// </summary>
			public static void Associate(string extension,
				   string progID, string description, string icon, string application)
			{
				Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
				if (progID != null && progID.Length > 0)
					using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
					{
						if (description != null)
							key.SetValue("", description);
						if (icon != null)
							key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
						if (application != null)
							key.CreateSubKey(@"Shell\Open\Command").SetValue("",ToShortPathName(application) + " \"%1\"");
					}
			}
			/// <summary>
			/// Return true if extension already associated in registry
			/// </summary>
			public static bool IsAssociated(string extension)
			{
				return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
			}
			/// <summary>
			/// Return short path format of a file name
			/// </summary>
			private static string ToShortPathName(string longName)
			{
				StringBuilder s = new StringBuilder(1000);
				uint iSize = (uint)s.Capacity;
				uint iRet = GetShortPathName(longName, s, iSize);
				return s.ToString();
			}
		}
		public class Process{
			private ProgressDialog pd;
			private PackResourceSetCreater m_Pack;
			private PackResourceSet m_Unpack;
			private IntPtr handle;
			private bool isVista;
			/// <summary>
			/// Initialize Worker Progress Window.
			/// </summary>
			/// <param name="ShowDoneMsg">Show MessageBox and Progress. (default is true)</param>
			public Process (IntPtr h){
				this.handle = h;
				OperatingSystem osInfo = Environment.OSVersion;
				this.isVista = (osInfo.Version.Major >= 6) ? true : false;
				if (this.handle != IntPtr.Zero){
					// Set PrgressDialog
					this.pd = new ProgressDialog(h);
					this.pd.Title = "MabiPacker";
					this.pd.Value = 0;
					this.pd.Line1 = Properties.Resources.Str_Initialize;
					this.pd.CancelMessage = Properties.Resources.Str_CancelMsg;
				}
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
				if (this.handle != IntPtr.Zero)
				{
					this.pd.ShowDialog(ProgressDialog.PROGDLG.Normal);
					this.pd.Line1 = Properties.Resources.Str_Pack;
				}else{
					Console.WriteLine(Properties.Resources.Str_Pack);
				}
				String internal_filename = "";
				if (File.Exists(OutputFile))
				{
					System.IO.File.Delete(@OutputFile);
				}
				// Get Filelist
				string[] filelist = Directory.GetFiles(InputDir, "*", SearchOption.AllDirectories);
				Array.Sort(filelist);

				if (this.handle != IntPtr.Zero)
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
					if (this.handle != IntPtr.Zero)
					{
						this.pd.Value = v;
						this.pd.Line2 = String.Format(Properties.Resources.Str_Packing, v, filelist.Length);
						this.pd.Line3 = internal_filename;
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
				if (this.handle != IntPtr.Zero)
				{
					this.pd.Line1 = Properties.Resources.Str_Packing;
				}else{
					Console.WriteLine(Properties.Resources.Str_Packing);
				}
				m_Pack.CreatePack(OutputFile);
				m_Pack.Dispose();
				Done();
			}
			/// <summary>
			/// Unpacking Package file process.
			/// </summary>
			/// <param name="InputFile">Set filename of unpack file..</param>
			/// <param name="OutputDir">Set output distnation of Unpacked files.</param>
			public void Unpack(string InputFile, string OutputDir)
			{
				if (this.handle != IntPtr.Zero)
				{
					this.pd.ShowDialog(ProgressDialog.PROGDLG.Normal);
					this.pd.Line1 = Properties.Resources.Str_Unpack;
				}else{
					Console.WriteLine(Properties.Resources.Str_Unpack);
				}
				
				m_Unpack = PackResourceSet.CreateFromFile(InputFile);

				uint packed_files = m_Unpack.GetFileCount();
				if (this.handle != IntPtr.Zero)
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

					if (this.handle != IntPtr.Zero)
					{
						this.pd.Line2 = String.Format(Properties.Resources.Str_Unpacking, i, packed_files);
						this.pd.Line3 = InternalName;
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
					try
					{
						// loading file content.
						byte[] buffer = new byte[Res.GetSize()];
						Res.GetData(buffer);
						Res.Close();

						// Get output Directory Name
						String outputPath = @OutputDir + "\\data\\" + InternalName;

						// Create directory
						String DirPath = System.Text.RegularExpressions.Regex.Replace(outputPath, @"([^\\]*?)$", "");
						if (!Directory.Exists(DirPath))
						{
							Directory.CreateDirectory(DirPath);
						}

						// Delete old
						if (File.Exists(outputPath))
						{
							System.IO.File.Delete(@outputPath);
						}
						if (Directory.Exists(outputPath))
						{
							System.IO.Directory.Delete(@outputPath);
						}
						// Write to file.
						System.IO.FileStream fs = new System.IO.FileStream(outputPath, System.IO.FileMode.Create);
						fs.Write(buffer, 0, buffer.Length);
						fs.Close();

						// Modify File time
						System.IO.File.SetCreationTime(outputPath, Res.GetCreated());
						System.IO.File.SetLastAccessTime(outputPath, Res.GetAccessed());
						System.IO.File.SetLastWriteTime(outputPath, Res.GetModified());
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						/*
						MessageBox.Show(Properties.Resources.Str_Error + "\r\n" + InternalName,
							Properties.Resources.Error,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1);
						 */
						continue;
					}
				}
				m_Unpack.Dispose();
				Done();
			}
			/// <summary>
			/// Unpacking file
			/// </summary>
			/// <param name="Res">PackResource </param>
			public bool UnpackFile(PackResource Res){
				if (this.handle != IntPtr.Zero)
				{
					this.pd.ShowDialog(ProgressDialog.PROGDLG.Normal);
					this.pd.Line1 = Properties.Resources.Str_Unpack;
				}
				else
				{
					Console.WriteLine(Properties.Resources.Str_Unpack);
				}
				try{
					String InternalName = Res.GetName();
					SaveFileDialog dSaveAs = new SaveFileDialog();
					dSaveAs.FileName = Path.GetFileName(InternalName);

					if (dSaveAs.ShowDialog() == DialogResult.OK)
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
					Done();
				}
			}
			/// <summary>
			/// Show message box when process done.
			/// </summary>
			private void Done(){
				if (this.handle != IntPtr.Zero)
				{
					this.pd.Line1 = Properties.Resources.Str_Finish;
					this.pd.Line2 = Properties.Resources.Str_Done;
					this.pd.CloseDialog();
					if (this.isVista){
						TaskDialog td = new TaskDialog();
						TaskDialogStandardButtons button = TaskDialogStandardButtons.Yes;
						button |= TaskDialogStandardButtons.No;
						td.Icon = TaskDialogStandardIcon.Warning;
						td.StandardButtons = button;
						td.InstructionText = Properties.Resources.Info;
						td.Caption = Properties.Resources.Info;
						td.Text = Properties.Resources.Confirm;
						TaskDialogResult res = td.Show();
					}else{
						MessageBox.Show(
							Properties.Resources.Str_Done,
							Properties.Resources.Info,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information,
							MessageBoxDefaultButton.Button1
						);
					}
				}else{
					Console.WriteLine(Properties.Resources.Str_Finish);
				}
			}
			/// <summary>
			/// Show message box when process aborted.
			/// </summary>
			private void Interrupt(){
				if (this.handle != IntPtr.Zero)
				{
					this.pd.CloseDialog();
					if (this.isVista){
						TaskDialog td = new TaskDialog();
						TaskDialogStandardButtons button = TaskDialogStandardButtons.Close;
						td.Icon = TaskDialogStandardIcon.Information;
						td.StandardButtons = button;
						td.InstructionText = Properties.Resources.Info;
						td.Caption = Properties.Resources.Info;
						td.Text = Properties.Resources.Str_Interrupt;
						TaskDialogResult res = td.Show();
					}else{
						MessageBox.Show(
							Properties.Resources.Str_Interrupt,
							Properties.Resources.Info,
							MessageBoxButtons.OK,
							MessageBoxIcon.Hand,
							MessageBoxDefaultButton.Button1
						);
					}
				}else{
					Console.WriteLine("Interrupted!");
				}
			}
		}
		public class Dialogs{
			private bool isVista;
			private IntPtr Handle;
			private Process w;
			public Dialogs (IntPtr h){
				this.Handle = h;
				this.w = new Process(h);
				OperatingSystem osInfo = Environment.OSVersion;
				this.isVista = (osInfo.Version.Major >= 6) ? true : false;
			}
			private bool ConfirmOverwrite(){
				if (this.isVista){
					TaskDialog td = new TaskDialog();
					TaskDialogStandardButtons button = TaskDialogStandardButtons.Yes;
					button |= TaskDialogStandardButtons.No;
					td.Icon = TaskDialogStandardIcon.Warning;
					td.StandardButtons = button;
					td.InstructionText = Properties.Resources.Confirm;
					td.Caption = Properties.Resources.Confirm;
					td.Text = Properties.Resources.Str_Overwrite;
					TaskDialogResult res = td.Show();

					if (res.ToString() == "No")
					{
						return false;
					}
				}else{
					DialogResult overwrite = MessageBox.Show(
						Properties.Resources.Str_Overwrite,
						Properties.Resources.Confirm,
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Warning,
						MessageBoxDefaultButton.Button1
					);
					if (overwrite == DialogResult.No)
					{
						return false;
					}
				}
				return true;
			}
			private bool ConfirmDialog(){
				if (this.isVista){
					TaskDialog td = new TaskDialog();
					TaskDialogStandardButtons button = TaskDialogStandardButtons.Yes;
					button |= TaskDialogStandardButtons.No;
					td.Icon = TaskDialogStandardIcon.Information;
					td.StandardButtons = button;
					td.InstructionText = Properties.Resources.Confirm;
					td.Caption = Properties.Resources.Confirm;
					td.Text = Properties.Resources.Str_Confirm;
					TaskDialogResult res = td.Show();

					if (res.ToString() != "Yes")
					{
						return false;
					}
				}else{
					DialogResult result = MessageBox.Show(
					Properties.Resources.Str_Confirm,
					Properties.Resources.Confirm,
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1);
					if (result != DialogResult.OK)
					{
						return false;
					}
				}
				return true;
			}
			public void Unpack(string InputFile, string dest = "", bool confirm = true){

				// Check output directory exsists.
				if (!Directory.Exists(dest))
				{
					if (this.isVista)
					{
						CommonOpenFileDialog cfdExtractTo = new CommonOpenFileDialog();
						cfdExtractTo.EnsureReadOnly = true;
						cfdExtractTo.IsFolderPicker = true;
						cfdExtractTo.AllowNonFileSystemItems = true;
						cfdExtractTo.InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
						cfdExtractTo.Title = Properties.Resources.Str_ExtractTo;
						if (cfdExtractTo.ShowDialog() != CommonFileDialogResult.Ok)
						{
							return;
						}
						dest = cfdExtractTo.FileName;
					}else{
						FolderBrowserDialog ExtractTo = new FolderBrowserDialog();
						ExtractTo.Description = Properties.Resources.Str_ExtractTo;
						if (ExtractTo.ShowDialog() != DialogResult.OK){
							return;
						}
						dest = ExtractTo.SelectedPath;
					}
				}else if (Directory.Exists(dest + "\\data")){
					if (!ConfirmOverwrite()) return;
				}

				if (confirm &&!ConfirmDialog()) return;

				this.w.Unpack(InputFile, dest);
			}
			public void Pack(string InputDir, string SaveAs, uint Version, int Level = -1, bool precheck= false)
			{
				#region Pack precheck
				// Check directory exsists
				if (!Directory.Exists(InputDir))
				{
					MessageBox.Show(Properties.Resources.Str_DataDirNotExists, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
					return;
				}

				// Check output file exsists.
				if (File.Exists(SaveAs))
				{
					DialogResult overwrite = MessageBox.Show(Properties.Resources.Str_Overwrite, Properties.Resources.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
					if (overwrite == DialogResult.No)
					{
						return;
					}
				}
				#endregion
				DialogResult result = MessageBox.Show(
					Properties.Resources.Str_Confirm,
					Properties.Resources.Confirm,
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1);

				if (result == DialogResult.OK)
				{
					this.w.Pack(InputDir, SaveAs, Version, Level);
				}

			}
		}
	}
}
