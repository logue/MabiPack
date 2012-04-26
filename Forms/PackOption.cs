using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MabiPacker.Forms
{
	public partial class PackOption : Form
	{
		public PackOption()
		{
			InitializeComponent();
            Utility.MabinogiEnv c = new Utility.MabinogiEnv();
			c.GetMabiEnv();
			PackageVersion.Minimum = c.MabiVer;
			PackageVersion.Value = (PackageVersion.Value > c.MabiVer) ? Properties.Settings.Default.LastPackVer : c.MabiVer;
			Level.SelectedIndex = Properties.Settings.Default.CompressLevel;
		}
		// Instance
		private static PackOption _Instance;
		public static PackOption Instance
		{
			get
			{
				return _Instance;
			}
			set
			{
				_Instance = value;
			}
		}
		private void PackOption_Load(object sender, System.EventArgs e)
		{
			PackOption.Instance = this;
		}
		// Get value
		public int Level_Value
		{
			get
			{
				return Level.SelectedIndex;
			}
			set
			{
				Level.SelectedIndex = value;
			}
		}

		public uint Version_Value
		{
			get
			{
				return (uint) PackageVersion.Value;
			}
			set
			{
				PackageVersion.Value = (decimal) value;
			}
		}
	}
}
