using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MabinogiResource;
using Microsoft.Win32;

namespace MabiPacker
{
	class Utility
	{
		public static void LoadAssembly()
		{
		/*
			AppDomain.CurrentDomain.AssemblyResolve += (sender, a) =>
			{
				Console.WriteLine(a.Name);
				String[] DLLs = new[] {"DevIL","MabinogiResource","Tao.DevIL", "MabinogiResource.net"};
				for(int i=0; i<DLLs.Length-1; ++i){
					if (a.Name.StartsWith(DLLs[i]))
					{
						
						byte[] b;
						using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MabiPacker." + DLLs[i] + ".dll"))
						{
							b = new byte[s.Length];
							s.Read(b, 0, (int)s.Length);
						}
						return System.Reflection.Assembly.Load(b);
					}
				}
				return null;
			};
		 */
		}
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
		public class Worker{
			private ProgressDialog pd;
			private PackResourceSetCreater m_Pack;
			private PackResourceSet m_Unpack;
			private IntPtr handle;
			/// <summary>
			/// Initialize Worker Progress Window.
			/// </summary>
			/// <param name="ShowDoneMsg">Show MessageBox and Progress. (default is true)</param>
			public Worker (IntPtr h){
				this.handle = h;
				if (this.handle != IntPtr.Zero){
					// Set PrgressDialog
					this.pd = new ProgressDialog(h);
					this.pd.Title = "MabiPacker";
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
						this.pd.Line1 = String.Format(Properties.Resources.Str_Packing, v, filelist.Length);
						this.pd.Line2 = internal_filename;
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
				this.pd.CloseDialog();

				// Start packing
				if (this.handle != IntPtr.Zero)
				{
					this.pd.Line1 = Properties.Resources.Str_Packing;
				}else{
					Console.WriteLine(Properties.Resources.Str_Packing);
				}
				m_Pack.CreatePack(OutputFile);
				m_Pack.Dispose();
				if (this.handle != IntPtr.Zero)
				{
					this.pd.Line1 = Properties.Resources.Str_Finish;
					this.pd.Line2 = Properties.Resources.Str_Done;
					this.pd.CloseDialog();
					DoneMsg();
				}
				else
				{
					Console.WriteLine(Properties.Resources.Str_Finish);
				}
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
						this.pd.Line1 = String.Format(Properties.Resources.Str_Unpacking, i, packed_files);
						this.pd.Line2 = InternalName;
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

				if (this.handle != IntPtr.Zero)
				{
					this.pd.Line1 = Properties.Resources.Str_Finish;
					this.pd.Line2 = Properties.Resources.Str_Done;
					this.pd.CloseDialog();
					DoneMsg();
				}else{
					Console.WriteLine(Properties.Resources.Str_Finish);
				}
			}
			/// <summary>
			/// Unpacking file
			/// </summary>
			/// <param name="Res">PackResource </param>
			public bool UnpackFile(PackResource Res){
				try{
					String InternalName = Res.GetName();
					SaveFileDialog dSaveAs = new SaveFileDialog();
					dSaveAs.FileName = System.IO.Path.GetFileName(InternalName);

					if (dSaveAs.ShowDialog() == DialogResult.OK)
					{
						// loading file content.
						byte[] buffer = new byte[Res.GetSize()];
						Res.GetData(buffer);
						Res.Close();

						// Delete old
						if (File.Exists(dSaveAs.FileName))
						{
							System.IO.File.Delete(dSaveAs.FileName);
						}
						// Write to file.
						System.IO.FileStream fs = new System.IO.FileStream(dSaveAs.FileName, System.IO.FileMode.Create);
						fs.Write(buffer, 0, buffer.Length);
						fs.Close();

						// Modify File time
						System.IO.File.SetCreationTime(dSaveAs.FileName, Res.GetCreated());
						System.IO.File.SetLastAccessTime(dSaveAs.FileName, Res.GetAccessed());
						System.IO.File.SetLastWriteTime(dSaveAs.FileName, Res.GetModified());
					}
					return true;
				}catch(Exception){
					return false;
				}
			}
			/// <summary>
			/// Show message box when process done.
			/// </summary>
			private void DoneMsg(){
				MessageBox.Show(Properties.Resources.Str_Done, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
			}
			/// <summary>
			/// Show message box when process aborted.
			/// </summary>
			private void Interrupt(){
				this.pd.CloseDialog();
				MessageBox.Show(Properties.Resources.Str_Interrupt, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
	}
}
