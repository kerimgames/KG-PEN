using System;
using System.Drawing;
using System.Windows.Forms;

namespace gInk
{
	public class FormColorWheel : Form
	{
		private const int WheelSize = 140;
		private const int WheelPadding = 8;
		private const int SliderWidth = 16;

		private Bitmap wheelBitmap;
		private Point wheelCenter;
		private float wheelRadius;
		private float hue;
		private float sat;
		private float val;

		private Panel preview;
		private TrackBar valueSlider;

		private bool dragging = false;

		public Color SelectedColor { get; private set; }
		public event Action<Color> ColorChanged;

		public FormColorWheel(Color current, Point location)
		{
			FormBorderStyle = FormBorderStyle.FixedSingle;
			StartPosition = FormStartPosition.Manual;
			Location = location;
			ClientSize = new Size(WheelSize + SliderWidth + WheelPadding * 3, WheelSize + WheelPadding * 2 + 40);
			MaximizeBox = false;
			MinimizeBox = false;
			ShowInTaskbar = false;
			TopMost = true;
			KeyPreview = true;
			Text = "Renk";

			wheelCenter = new Point(WheelPadding + WheelSize / 2, WheelPadding + WheelSize / 2);
			wheelRadius = WheelSize / 2 - 2;
			wheelBitmap = BuildWheelBitmap(WheelSize);

			valueSlider = new TrackBar();
			valueSlider.Orientation = Orientation.Vertical;
			valueSlider.Minimum = 0;
			valueSlider.Maximum = 100;
			valueSlider.TickStyle = TickStyle.None;
			valueSlider.Left = WheelPadding + WheelSize + WheelPadding;
			valueSlider.Top = WheelPadding;
			valueSlider.Height = WheelSize;
			valueSlider.Width = SliderWidth;
			valueSlider.ValueChanged += ValueSlider_ValueChanged;
			Controls.Add(valueSlider);

			preview = new Panel();
			preview.Left = WheelPadding;
			preview.Top = WheelPadding + WheelSize + 8;
			preview.Width = 80;
			preview.Height = 20;
			preview.BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(preview);

			Button cancel = new Button();
			cancel.Text = "Kapat";
			cancel.Left = WheelPadding + 90;
			cancel.Top = WheelPadding + WheelSize + 6;
			cancel.Width = 60;
			cancel.Click += (s, e) => Close();
			Controls.Add(cancel);

			CancelButton = cancel;
			cancel.TabStop = true;
			cancel.TabIndex = 0;

			SetFromColor(current);
			UpdatePreview();
			valueSlider.Value = (int)(val * 100);

			valueSlider.BringToFront();
			preview.BringToFront();
			cancel.BringToFront();

			MouseDown += FormColorWheel_MouseDown;
			MouseMove += FormColorWheel_MouseMove;
			MouseUp += (s, e) => dragging = false;
			KeyDown += FormColorWheel_KeyDown;
		}

		private void FormColorWheel_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}

		private void ValueSlider_ValueChanged(object sender, EventArgs e)
		{
			val = valueSlider.Value / 100f;
			UpdatePreview();
			Invalidate(new Rectangle(WheelPadding, WheelPadding, WheelSize, WheelSize));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawImage(wheelBitmap, WheelPadding, WheelPadding);
			DrawKnob(e.Graphics);
		}

		private void DrawKnob(Graphics g)
		{
			Point p = HSVToPoint(hue, sat);
			int r = 6;
			Rectangle rect = new Rectangle(p.X - r, p.Y - r, r * 2, r * 2);
			g.DrawEllipse(Pens.Black, rect);
			g.DrawEllipse(Pens.White, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
		}

		private void FormColorWheel_MouseDown(object sender, MouseEventArgs e)
		{
			if (IsInsideWheel(e.Location))
			{
				dragging = true;
				UpdateFromPoint(e.Location);
			}
		}

		private void FormColorWheel_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragging)
			{
				UpdateFromPoint(e.Location);
			}
		}

		private void UpdateFromPoint(Point p)
		{
			PointF v = new PointF(p.X - wheelCenter.X, p.Y - wheelCenter.Y);
			float dist = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
			if (dist > wheelRadius)
			{
				v.X = v.X * wheelRadius / dist;
				v.Y = v.Y * wheelRadius / dist;
				dist = wheelRadius;
			}
			sat = dist / wheelRadius;
			hue = (float)(Math.Atan2(v.Y, v.X) * 180.0 / Math.PI);
			if (hue < 0) hue += 360f;
			UpdatePreview();
			Invalidate(new Rectangle(WheelPadding, WheelPadding, WheelSize, WheelSize));
		}

		private bool IsInsideWheel(Point p)
		{
			float dx = p.X - wheelCenter.X;
			float dy = p.Y - wheelCenter.Y;
			return (dx * dx + dy * dy) <= wheelRadius * wheelRadius;
		}

		private void UpdatePreview()
		{
			SelectedColor = ColorFromHSV(hue, sat, val);
			preview.BackColor = SelectedColor;
			if (ColorChanged != null)
			{
				ColorChanged(SelectedColor);
			}
		}

		private Point HSVToPoint(float h, float s)
		{
			double rad = h * Math.PI / 180.0;
			float r = s * wheelRadius;
			int x = (int)(wheelCenter.X + r * Math.Cos(rad));
			int y = (int)(wheelCenter.Y + r * Math.Sin(rad));
			return new Point(x, y);
		}

		private void SetFromColor(Color c)
		{
			ColorToHSV(c, out hue, out sat, out val);
		}

		private static void ColorToHSV(Color c, out float h, out float s, out float v)
		{
			float r = c.R / 255f;
			float g = c.G / 255f;
			float b = c.B / 255f;
			float max = Math.Max(r, Math.Max(g, b));
			float min = Math.Min(r, Math.Min(g, b));
			v = max;

			float delta = max - min;
			if (max == 0 || delta == 0)
			{
				s = 0;
				h = 0;
				return;
			}
			s = delta / max;

			if (r == max)
				h = (g - b) / delta;
			else if (g == max)
				h = 2 + (b - r) / delta;
			else
				h = 4 + (r - g) / delta;
			h *= 60;
			if (h < 0) h += 360;
		}

		private static Color ColorFromHSV(float h, float s, float v)
		{
			int i;
			float f, p, q, t;
			if (s == 0)
				return Color.FromArgb((int)(v * 255), (int)(v * 255), (int)(v * 255));

			h /= 60;
			i = (int)Math.Floor(h);
			f = h - i;
			p = v * (1 - s);
			q = v * (1 - s * f);
			t = v * (1 - s * (1 - f));

			float r = 0, g = 0, b = 0;
			switch (i % 6)
			{
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				case 5: r = v; g = p; b = q; break;
			}
			return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
		}

		private Bitmap BuildWheelBitmap(int size)
		{
			Bitmap bmp = new Bitmap(size, size);
			Point center = new Point(size / 2, size / 2);
			float radius = size / 2f - 1;
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float dx = x - center.X;
					float dy = y - center.Y;
					float dist = (float)Math.Sqrt(dx * dx + dy * dy);
					if (dist > radius)
					{
						bmp.SetPixel(x, y, Color.Transparent);
						continue;
					}
					float satLocal = dist / radius;
					float hueLocal = (float)(Math.Atan2(dy, dx) * 180.0 / Math.PI);
					if (hueLocal < 0) hueLocal += 360f;
					Color c = ColorFromHSV(hueLocal, satLocal, 1f);
					bmp.SetPixel(x, y, c);
				}
			}
			return bmp;
		}
	}
}
