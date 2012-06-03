using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MabiPacker
{
	public class Dialogs{
		private bool isVista;
		private Worker w;
		private string MabiDir;
		public Dialogs (string MabiDir = "C:\\Nexon\\Mabinogi"){
			this.MabiDir = MabiDir;
			this.w = new Worker();
			this.isVista = (Environment.OSVersion.Version.Major >= 6) ? true : false;
		}
		/* File Open Dialogs */
		public string InputFile(string InputFile = ""){
			string caption = Properties.Resources.ChoosePackDir;
			if (this.isVista){
				CommonOpenFileDialog dInputFile = new CommonOpenFileDialog();
				dInputFile.Title = caption;
				dInputFile.DefaultExtension = ".pack";
				dInputFile.Filters.Add(new CommonFileDialogFilter(Properties.Resources.PackFileDesc, "*.pack"));
				dInputFile.InitialDirectory = MabiDir + "\\Package\\";
				dInputFile.IsFolderPicker = false;
				dInputFile.Multiselect = false;
					
				if (dInputFile.ShowDialog() == CommonFileDialogResult.Ok)
				{
					InputFile = dInputFile.FileName;
				}
			}else{
				OpenFileDialog dInputFile = new OpenFileDialog();
				dInputFile.Title = caption;
				dInputFile.DefaultExt = ".pack";
				dInputFile.InitialDirectory = MabiDir + "\\Package\\";
				dInputFile.Filter = Properties.Resources.PackFileDesc + "|(*.pack)";
				if (dInputFile.ShowDialog() == DialogResult.OK){
					InputFile = dInputFile.FileName;
				}
			}
			return InputFile;
		}
		public string OutputFile(string OutputFile = "")
		{
			string caption = Properties.Resources.ChooseUnpackFile;
			if (this.isVista){
				CommonSaveFileDialog dOutputFile = new CommonSaveFileDialog();
				dOutputFile.Title = caption;
				dOutputFile.DefaultExtension = ".pack";
				dOutputFile.Filters.Add(new CommonFileDialogFilter(Properties.Resources.PackFileDesc, "*.pack"));
				dOutputFile.InitialDirectory = MabiDir + "\\Package\\";

				if (dOutputFile.ShowDialog() == CommonFileDialogResult.Ok)
				{
					OutputFile = dOutputFile.FileName;
				}
			}else{
				SaveFileDialog dOutputFile = new SaveFileDialog();
				dOutputFile.InitialDirectory = OutputFile;
				dOutputFile.Title = caption;
				dOutputFile.DefaultExt = ".pack";
				dOutputFile.Filter = Properties.Resources.PackFileDesc +  "|(*.pack)";
				if (dOutputFile.ShowDialog() == DialogResult.OK)
				{
					OutputFile = dOutputFile.FileName;
				}

			}
			return OutputFile;
		}
/*
 public string OutputFile(string OutputFile = "", uint version=120527 , int level=-1 )
		{	
				CommonFileDialog dOutputFile = new CommonSaveFileDialog();
				dOutputFile.Title = "Choose save *.pack file to.";
				dOutputFile.DefaultExtension = ".pack";
				dOutputFile.Filters.Add(new CommonFileDialogFilter((string)Application.Current.FindResource("MabinogiPackageFile") , "*.pack"));

				// Add Version Option
				dOutputFile.Controls.Add(new CommonFileDialogLabel("Version :"));
				dOutputFile.Controls.Add(new CommonFileDialogTextBox("tVersion", version.ToString()));

				// Add a Compression level
				dOutputFile.Controls.Add(new CommonFileDialogLabel("Compression Level :"));
				CommonFileDialogComboBox cLevel = new CommonFileDialogComboBox("cLevel");
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("Auto"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("0 - No Compression"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("1 - Fast"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("2"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("3"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("4"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("5 - Middle Compression"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("6"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("7"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("8"));
				cLevel.Items.Add(new CommonFileDialogComboBoxItem("9 - Best Compression"));
				cLevel.SelectedIndex = 0;
				dOutputFile.Controls.Add(cLevel);

				if (dOutputFile.ShowDialog() == CommonFileDialogResult.Ok)
				{
					OutputFile = dOutputFile.FileName;
				}
			return OutputFile;
		}
 */
		public string InputDir(string InputDir = "")
		{
			string caption = Properties.Resources.ChoosePackDir;
			if (this.isVista){
				CommonOpenFileDialog dInputDir = new CommonOpenFileDialog();
				dInputDir.IsFolderPicker = true;
				dInputDir.Title = caption;
				dInputDir.Multiselect = false;
				dInputDir.InitialDirectory = InputDir;
				if (dInputDir.ShowDialog() == CommonFileDialogResult.Ok)
				{
					InputDir = dInputDir.FileName;
				}
			}else{
				FolderBrowserDialog dInputDir = new FolderBrowserDialog();
				dInputDir.SelectedPath = InputDir;
				dInputDir.Description = caption;
				if (dInputDir.ShowDialog() == DialogResult.OK)
				{
					InputDir = dInputDir.SelectedPath;
				}
			}
			return InputDir;
		}
		public string OutputDir(string OutputDir = ""){
		string caption = Properties.Resources.ChooseUnpackDir;
			if (isVista){
				CommonOpenFileDialog dOutputDir = new CommonOpenFileDialog();
				dOutputDir.IsFolderPicker = true;
				dOutputDir.Title = caption;
				dOutputDir.Multiselect = false;
				if (dOutputDir.ShowDialog() == CommonFileDialogResult.Ok)
				{
					OutputDir = dOutputDir.FileName;
				}
			}else{
				OpenFileDialog dOutputDir = new OpenFileDialog();
				dOutputDir.InitialDirectory = OutputDir;
			}
			return OutputDir;
		}
		public void Pack(string InputDir, string OutputFile, uint OutputVer, int Level){
			string TaskName = Properties.Resources.Str_Pack;
			if (Confirm(TaskName) !=false){
				try{
					w.Pack(InputDir,OutputFile,OutputVer,Level);
				}catch (Exception e){
					Console.WriteLine(e);
					Error(e,TaskName);
				}
			}
		}
		public void Unpack(string InputFile, string OutputDir)
		{
			string TaskName = Properties.Resources.Str_Unpack;
			if (Confirm(TaskName) != false)
			{
				try
				{
					w.Unpack(InputFile, OutputDir);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					Error(e, TaskName);
				}
			}
		}
		private bool Confirm(string TaskName){
			if (isVista){
				TaskDialog td = new TaskDialog();
				TaskDialogStandardButtons button = TaskDialogStandardButtons.Yes;
				button |= TaskDialogStandardButtons.No;
				td.Icon = TaskDialogStandardIcon.Information;
				td.StandardButtons = button;
				td.InstructionText = TaskName;
				td.Caption = TaskName;
				td.Text = Properties.Resources.Str_Confirm;
				TaskDialogResult res = td.Show();

				if (res.ToString() != "Yes"){
					return false;
				}
			}else{
				DialogResult result = MessageBox.Show(
				Properties.Resources.Str_Confirm,
				Properties.Resources.Confirm,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1);
				if (result != DialogResult.Yes){
					return false;
				}
			}
			return true;
		}
		private void Done(string TaskName){
			TaskDialog td = new TaskDialog();
			TaskDialogStandardButtons button = TaskDialogStandardButtons.Ok;
			td.Icon = TaskDialogStandardIcon.Information;
			td.StandardButtons = button;
			td.InstructionText = TaskName;
			td.Caption = TaskName;
			td.Text = Properties.Resources.Complete;
			TaskDialogResult res = td.Show();
		}
		public void Error(Exception e, string TaskName){
			// Error dialog
			TaskDialog tdError = new TaskDialog();
			TaskDialogStandardButtons button = TaskDialogStandardButtons.Ok;
			tdError.StandardButtons = button;
			tdError.DetailsExpanded = false;
			tdError.Cancelable = true;
			tdError.Icon = TaskDialogStandardIcon.Error;

			tdError.Caption = TaskName;
			tdError.InstructionText = Properties.Resources.ErrorMsg;
			tdError.DetailsExpandedText = e.ToString();

			tdError.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;

			TaskDialogResult res = tdError.Show();
		}
	}
}
