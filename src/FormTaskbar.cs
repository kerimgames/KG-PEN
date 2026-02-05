using System;
using System.Drawing;
using System.Windows.Forms;

namespace gInk
{
	public class FormTaskbar : Form
	{
		private readonly Root Root;

		public FormTaskbar(Root root)
		{
			Root = root;

			Text = Root.AppDisplayName;
			try
			{
				if (System.IO.File.Exists("icon_kgink.ico"))
					Icon = new Icon("icon_kgink.ico");
			}
			catch { }
			ShowInTaskbar = true;
			ShowIcon = true;
			WindowState = FormWindowState.Minimized;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(-2000, -2000);
			Size = new Size(320, 200);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			WindowState = FormWindowState.Minimized;
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			Root.StartInk();
			WindowState = FormWindowState.Minimized;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (e.Button == MouseButtons.Left)
			{
				Root.StartInk();
				WindowState = FormWindowState.Minimized;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (WindowState != FormWindowState.Minimized)
				WindowState = FormWindowState.Minimized;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			Root.ExitApp();
		}
	}
}
