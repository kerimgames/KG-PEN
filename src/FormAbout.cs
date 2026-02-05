using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace gInk
{
	public partial class FormAbout : Form
	{
		public FormAbout()
		{
			InitializeComponent();
		}

		private void FormAbout_Load(object sender, EventArgs e)
		{
			this.Icon = gInk.Properties.Resources.icon_red;
			string version = Application.ProductVersion.Substring(0, Application.ProductVersion.Length - 2);
			string about = "kg pen v" + version + "\r\n";
			about += "\r\n";
			about += "YAPIMCI: kerimgames\r\n";
			about += "FİKİR ÜRETEN: MUSA HIDIR\r\n";
			about += "KODLARI YAZAN: Kerim ve MARO PASHA\r\n";
			about += "\r\n";
			about += "TEŞEKKÜRLER\r\n";
			about += "Buğra Eymen Şimşek\r\n";
			about += "glınk\r\n";
			about += "\r\n";
			about += "2023-2027 kerimgames tüm hakları saklıdır\r\n";
			textBox1.Text = about;
			textBox1.Select(textBox1.Text.Length, 0);
		}
	}
}
