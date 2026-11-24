// DualSense diagram for the launcher: a clean controller in the middle, the key bound to each
// button in a callout on the left or right, joined by a line (like the game's own control screen).
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ChiakiMouseLauncher
{
	class ControllerView : Control
	{
		public string Highlight;
		public Func<string, string> KeyFor;

		// Drawing space; scaled to fit the control
		const float W = 640, H = 360;

		class Callout
		{
			public string Id, Glyph;
			public bool Left;
			public float Y;
			public PointF Target;
			public Callout(string id, string glyph, bool left, float y, float tx, float ty)
			{ Id = id; Glyph = glyph; Left = left; Y = y; Target = new PointF(tx, ty); }
		}

		static readonly Callout[] callouts =
		{
			new Callout("L2", "L2", true, 40, 205, 56),
			new Callout("L1", "L1", true, 68, 205, 72),
			new Callout("SHARE", "Create", true, 96, 242, 103),
			new Callout("DUP", "↑", true, 124, 195, 130),
			new Callout("DLEFT", "←", true, 150, 178, 145),
			new Callout("DRIGHT", "→", true, 176, 212, 145),
			new Callout("DDOWN", "↓", true, 202, 195, 160),
			new Callout("L3", "L3", true, 300, 280, 210),
			new Callout("R2", "R2", false, 40, 435, 56),
			new Callout("R1", "R1", false, 68, 435, 72),
			new Callout("OPTIONS", "Options", false, 96, 398, 103),
			new Callout("TRIANGLE", "△", false, 124, 445, 121),
			new Callout("CIRCLE", "○", false, 150, 469, 145),
			new Callout("CROSS", "✕", false, 176, 445, 169),
			new Callout("SQUARE", "▢", false, 202, 421, 145),
			new Callout("R3", "R3", false, 300, 360, 210),
		};

		public ControllerView()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, false);
		}

		bool On(string id)
		{
			if (Highlight == null) return false;
			if (Highlight == id) return true;
			if (Highlight == "L3R3" && (id == "L3" || id == "R3")) return true;
			return false;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			g.Clear(BackColor);

			float s = Math.Min(Width / W, Height / H);
			g.TranslateTransform((Width - W * s) / 2, (Height - H * s) / 2);
			g.ScaleTransform(s, s);

			DrawShoulders(g);
			DrawBody(g);
			DrawButtons(g);
			foreach (Callout c in callouts) DrawCallout(g, c);
			DrawCenterCallout(g, "TOUCHPAD", "Touchpad", 16, new PointF(320, 88), true);
			DrawCenterCallout(g, "PS", "PS", 344, new PointF(320, 228), false);
		}

		// ---------- controller ----------

		static GraphicsPath BodyPath()
		{
			GraphicsPath p = new GraphicsPath();
			p.AddLine(195, 82, 445, 82);
			p.AddBezier(445, 82, 490, 82, 508, 115, 510, 160);
			p.AddBezier(510, 160, 514, 220, 520, 282, 495, 310);
			p.AddBezier(495, 310, 475, 332, 435, 325, 420, 290);
			p.AddBezier(420, 290, 408, 260, 390, 242, 360, 240);
			p.AddLine(360, 240, 280, 240);
			p.AddBezier(280, 240, 250, 242, 232, 260, 220, 290);
			p.AddBezier(220, 290, 205, 325, 165, 332, 145, 310);
			p.AddBezier(145, 310, 120, 282, 126, 220, 130, 160);
			p.AddBezier(130, 160, 132, 115, 150, 82, 195, 82);
			p.CloseFigure();
			return p;
		}

		void DrawShoulders(Graphics g)
		{
			Shoulder(g, "L2", new RectangleF(170, 46, 70, 18), 9);
			Shoulder(g, "R2", new RectangleF(400, 46, 70, 18), 9);
			Shoulder(g, "L1", new RectangleF(160, 66, 88, 12), 6);
			Shoulder(g, "R1", new RectangleF(392, 66, 88, 12), 6);
		}

		void Shoulder(Graphics g, string id, RectangleF r, float rad)
		{
			using (GraphicsPath p = Theme.Round(r, rad))
			using (SolidBrush b = new SolidBrush(On(id) ? Theme.Accent : Color.FromArgb(70, 73, 86)))
				g.FillPath(b, p);
		}

		void DrawBody(Graphics g)
		{
			using (GraphicsPath body = BodyPath())
			{
				// soft shadow
				using (Matrix m = new Matrix())
				using (GraphicsPath shadow = (GraphicsPath)body.Clone())
				{
					m.Translate(0, 6);
					shadow.Transform(m);
					using (SolidBrush sb = new SolidBrush(Color.FromArgb(70, 0, 0, 0))) g.FillPath(sb, shadow);
				}
				using (LinearGradientBrush lb = new LinearGradientBrush(new RectangleF(120, 80, 400, 260),
					Color.FromArgb(246, 247, 250), Color.FromArgb(200, 203, 214), 90f))
					g.FillPath(lb, body);
				using (Pen edge = new Pen(Color.FromArgb(150, 154, 168), 1.2f)) g.DrawPath(edge, body);
			}

			// Black centre with the sticks
			using (GraphicsPath c = Theme.Round(new RectangleF(244, 178, 152, 60), 28))
			using (SolidBrush b = new SolidBrush(Color.FromArgb(30, 32, 40)))
				g.FillPath(b, c);

			// Touchpad
			using (GraphicsPath tp = Theme.Round(new RectangleF(262, 88, 116, 70), 10))
			{
				using (SolidBrush b = new SolidBrush(On("TOUCHPAD") ? Theme.Accent : Color.FromArgb(232, 234, 240))) g.FillPath(b, tp);
				using (Pen pen = new Pen(Color.FromArgb(170, 174, 188), 1f)) g.DrawPath(pen, tp);
			}
			// Light bars beside the touchpad
			using (Pen lb = new Pen(Color.FromArgb(90, 140, 255), 2f))
			{
				g.DrawLine(lb, 258, 92, 252, 150);
				g.DrawLine(lb, 382, 92, 388, 150);
			}
		}

		void DrawButtons(Graphics g)
		{
			Color dark = Color.FromArgb(52, 55, 66);

			// Create / Options pills
			Pill(g, "SHARE", new RectangleF(239, 94, 7, 18), dark);
			Pill(g, "OPTIONS", new RectangleF(394, 94, 7, 18), dark);

			// D-pad
			Pad(g, "DUP", new RectangleF(187, 120, 16, 18));
			Pad(g, "DDOWN", new RectangleF(187, 152, 16, 18));
			Pad(g, "DLEFT", new RectangleF(168, 137, 18, 16));
			Pad(g, "DRIGHT", new RectangleF(204, 137, 18, 16));

			// Face buttons
			Face(g, "TRIANGLE", 445, 121, "△");
			Face(g, "CIRCLE", 469, 145, "○");
			Face(g, "CROSS", 445, 169, "✕");
			Face(g, "SQUARE", 421, 145, "▢");

			// Sticks
			Stick(g, "L3", 280, 210, "WASD");
			Stick(g, "R3", 360, 210, "Mouse");

			// PS button
			using (SolidBrush b = new SolidBrush(On("PS") ? Theme.Accent : Color.FromArgb(70, 73, 86)))
				g.FillEllipse(b, 314, 222, 12, 12);
		}

		void Pill(Graphics g, string id, RectangleF r, Color c)
		{
			using (GraphicsPath p = Theme.Round(r, 3.5f))
			using (SolidBrush b = new SolidBrush(On(id) ? Theme.Accent : c))
				g.FillPath(b, p);
		}

		void Pad(Graphics g, string id, RectangleF r)
		{
			using (GraphicsPath p = Theme.Round(r, 4))
			using (SolidBrush b = new SolidBrush(On(id) ? Theme.Accent : Color.FromArgb(62, 65, 78)))
				g.FillPath(b, p);
		}

		void Face(Graphics g, string id, float cx, float cy, string glyph)
		{
			bool on = On(id);
			using (SolidBrush b = new SolidBrush(on ? Theme.Accent : Color.FromArgb(46, 49, 60)))
				g.FillEllipse(b, cx - 11, cy - 11, 22, 22);
			using (Font f = new Font("Segoe UI Symbol", 8.5f, FontStyle.Bold))
			using (SolidBrush t = new SolidBrush(Color.White))
			{
				StringFormat sf = new StringFormat(); sf.Alignment = StringAlignment.Center; sf.LineAlignment = StringAlignment.Center;
				g.DrawString(glyph, f, t, new RectangleF(cx - 11, cy - 11, 22, 22), sf);
			}
		}

		void Stick(Graphics g, string id, float cx, float cy, string label)
		{
			bool on = On(id) || (id == "L3" && On("LS")) || (id == "R3" && On("RS"));
			using (SolidBrush b = new SolidBrush(on ? Theme.Accent : Color.FromArgb(16, 17, 22)))
				g.FillEllipse(b, cx - 20, cy - 20, 40, 40);
			using (Pen p = new Pen(Color.FromArgb(on ? 220 : 80, 255, 255, 255), 1.5f))
				g.DrawEllipse(p, cx - 12, cy - 12, 24, 24);
			using (Font f = new Font(Theme.Semi, 6.5f))
			using (SolidBrush t = new SolidBrush(Color.FromArgb(200, 205, 215)))
			{
				StringFormat sf = new StringFormat(); sf.Alignment = StringAlignment.Center;
				g.DrawString(label, f, t, new RectangleF(cx - 30, cy + 22, 60, 12), sf);
			}
		}

		// ---------- callouts ----------

		void DrawCallout(Graphics g, Callout c)
		{
			bool on = On(c.Id);
			string key = KeyFor != null ? KeyFor(c.Id) : null;
			bool empty = string.IsNullOrEmpty(key);
			if (empty) key = "—";

			using (Font kf = new Font(Theme.Semi, on ? 9f : 8.5f))
			using (Font gf = new Font(c.Glyph.Length <= 2 ? "Segoe UI Symbol" : Theme.Ui, 7.5f, FontStyle.Bold))
			{
				SizeF ks = g.MeasureString(key, kf);
				SizeF gs = g.MeasureString(c.Glyph, gf);
				float chipW = Math.Max(30, ks.Width + 12), chipH = 20;
				RectangleF chip;
				float glyphX, lineStart;
				if (c.Left)
				{
					chip = new RectangleF(100 - chipW, c.Y - chipH / 2, chipW, chipH);
					glyphX = 104;
					lineStart = glyphX + gs.Width + 2;
				}
				else
				{
					chip = new RectangleF(540, c.Y - chipH / 2, chipW, chipH);
					glyphX = 536 - gs.Width;
					lineStart = glyphX - 2;
				}

				// Leader line: horizontal, then straight to the button
				Color lc = on ? Theme.Accent : Color.FromArgb(78, 82, 96);
				using (Pen lp = new Pen(lc, on ? 1.8f : 1f))
				{
					float elbowX = c.Left ? Math.Min(lineStart + 18, c.Target.X) : Math.Max(lineStart - 18, c.Target.X);
					g.DrawLine(lp, lineStart, c.Y, elbowX, c.Y);
					g.DrawLine(lp, elbowX, c.Y, c.Target.X, c.Target.Y);
				}
				using (SolidBrush db = new SolidBrush(lc)) g.FillEllipse(db, c.Target.X - 2.5f, c.Target.Y - 2.5f, 5, 5);

				// Glyph
				using (SolidBrush gb = new SolidBrush(on ? Color.White : Theme.Muted))
					g.DrawString(c.Glyph, gf, gb, glyphX, c.Y - gs.Height / 2);

				// Key chip
				using (GraphicsPath cp = Theme.Round(chip, 6))
				{
					using (SolidBrush cb = new SolidBrush(on ? Theme.Accent : Theme.Keycap)) g.FillPath(cb, cp);
					using (Pen ep = new Pen(on ? Theme.Accent : Theme.KeycapEdge)) g.DrawPath(ep, cp);
				}
				StringFormat sf = new StringFormat(); sf.Alignment = StringAlignment.Center; sf.LineAlignment = StringAlignment.Center;
				using (SolidBrush tb = new SolidBrush(empty ? Theme.Muted : Color.White))
					g.DrawString(key, kf, tb, chip, sf);
			}
		}

		void DrawCenterCallout(Graphics g, string id, string glyph, float y, PointF target, bool above)
		{
			bool on = On(id);
			string key = KeyFor != null ? KeyFor(id) : null;
			bool empty = string.IsNullOrEmpty(key);
			if (empty) key = "—";
			using (Font kf = new Font(Theme.Semi, on ? 9f : 8.5f))
			using (Font gf = new Font(Theme.Ui, 7.5f, FontStyle.Bold))
			{
				SizeF ks = g.MeasureString(key, kf);
				SizeF gs = g.MeasureString(glyph, gf);
				float chipW = Math.Max(30, ks.Width + 12);
				RectangleF chip = new RectangleF(320 - chipW / 2, y - 10, chipW, 20);
				Color lc = on ? Theme.Accent : Color.FromArgb(78, 82, 96);
				using (Pen lp = new Pen(lc, on ? 1.8f : 1f))
					g.DrawLine(lp, 320, above ? chip.Bottom : chip.Top, target.X, target.Y);
				using (SolidBrush gb = new SolidBrush(on ? Color.White : Theme.Muted))
					g.DrawString(glyph, gf, gb, chip.Right + 6, y - gs.Height / 2);
				using (GraphicsPath cp = Theme.Round(chip, 6))
				{
					using (SolidBrush cb = new SolidBrush(on ? Theme.Accent : Theme.Keycap)) g.FillPath(cb, cp);
					using (Pen ep = new Pen(on ? Theme.Accent : Theme.KeycapEdge)) g.DrawPath(ep, cp);
				}
				StringFormat sf = new StringFormat(); sf.Alignment = StringAlignment.Center; sf.LineAlignment = StringAlignment.Center;
				using (SolidBrush tb = new SolidBrush(empty ? Theme.Muted : Color.White))
					g.DrawString(key, kf, tb, chip, sf);
			}
		}
	}
}
