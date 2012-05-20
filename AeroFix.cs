using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace MabiPacker
{
	public static class GlassExtensions
	{
		public static void HookGlassRender(this Control control, int offsetX, int offsetY)
		{
			GlassRenderer.Hook(control, offsetX, offsetY);
		}

		public static void HookGlassRender(this TextBox control)
		{
			GlassRenderer.Hook(control, -1, -1);
		}

		public static void HookGlassRender(this ListBox control)
		{
			GlassRenderer.Hook(control, -1, -1);
		}

		public static void HookGlassRender(this TrackBar control)
		{
			GlassRenderer.Hook(control, 0, 0);
		}

		public static void HookGlassRender(this TreeView control)
		{
			GlassRenderer.Hook(control, -1, -1);
		}

		public static void HookGlassRender(this NumericUpDown control)
		{
			GlassRenderer.Hook(control, 0, 0);
			GlassRenderer.Hook(control.Controls[1], 0, 0);
		}
		public static void HookGlassRender(this ComboBox control)
		{
			GlassRenderer.Hook(control, 0, 0);
		}

		public static void UnhookGlassRender(this Control control)
		{
			GlassRenderer.Unhook(control);
		}
	}

	public class GlassRenderer : NativeWindow, IDisposable
	{
		private static Dictionary<Control, GlassRenderer> RegisteredControls = new Dictionary<Control, GlassRenderer>();

		public static void Hook(Control control, int offsetX, int offsetY)
		{
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				GlassRenderer.RegisteredControls.Add(control, new GlassRenderer(control, offsetX, offsetY));
			}
		}

		public static void Unhook(Control control)
		{
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				if (GlassRenderer.RegisteredControls.ContainsKey(control))
				{
					GlassRenderer.RegisteredControls[control].Dispose();
					GlassRenderer.RegisteredControls.Remove(control);
				}
			}
		}

		private Control Control;
		private Bitmap Bitmap;
		private Graphics ControlGraphics;
		private Point Offset;

		public bool HasDisposed { get; private set; }

		protected override void WndProc(ref Message m)
		{
			if (!this.HasDisposed)
			{
				switch (m.Msg)
				{
					case 0x14: // WM_ERASEBKGND
						this.CustomPaint();
						break;

					case 0x0F: // WM_PAINT
					case 0x85: // WM_NCPAINT

					case 0x100: // WM_KEYDOWN
					case 0x101: // WM_KEYUP
					case 0x102: // WM_CHAR

					case 0x200: // WM_MOUSEMOVE
					case 0x2A1: // WM_MOUSEHOVER
					case 0x201: // WM_LBUTTONDOWN
					case 0x202: // WM_LBUTTONUP
					case 0x285: // WM_IME_SELECT

					case 0x300: // WM_CUT
					case 0x301: // WM_COPY
					case 0x302: // WM_PASTE
					case 0x303: // WM_CLEAR
					case 0x304: // WM_UNDO
						base.WndProc(ref m);
						this.CustomPaint();
						break;

					default:
						base.WndProc(ref m);
						break;
				}
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		public GlassRenderer(Control control, int offsetX, int offsetY)
		{
			this.Offset = new Point(offsetX, offsetY);
			this.Control = control;
			this.Bitmap = new Bitmap(this.Control.Width, this.Control.Height);
			this.ControlGraphics = Graphics.FromHwnd(control.Handle);
			this.AssignHandle(control.Handle);
			control.Disposed += delegate { this.Dispose(); };
		}

		public void CustomPaint()
		{
			this.Control.DrawToBitmap(this.Bitmap, new Rectangle(0, 0, this.Control.Width, this.Control.Height));
			this.ControlGraphics.DrawImageUnscaled(this.Bitmap, this.Offset);
		}

		public void Dispose()
		{
			if (this.HasDisposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			this.HasDisposed = true;
			this.DestroyHandle();
		}
	}
}