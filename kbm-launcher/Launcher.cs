// chiaki-ng (mouse) launcher: set keys, mouse and stream settings, then start chiaki-ng.
// Settings go to HKCU\Software\Chiaki\Chiaki, where chiaki-ng reads them.
// C# 5 (built with the .NET Framework compiler that ships with Windows).
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

[assembly: System.Reflection.AssemblyTitle("GTA Klavye Mouse Baslatici")]

namespace ChiakiMouseLauncher
{
	static class Theme
	{
		// Chinese needs a font with CJK glyphs; GDI+ doesn't fall back on its own
		public static string Ui = "Segoe UI";
		public static string Semi = "Segoe UI Semibold";
		public static void ForLanguage(string code)
		{
			if (code == "zh") { Ui = "Microsoft YaHei UI"; Semi = "Microsoft YaHei UI"; }
			else { Ui = "Segoe UI"; Semi = "Segoe UI Semibold"; }
		}

		public static readonly Color Bg = Color.FromArgb(18, 19, 23);
		public static readonly Color Panel = Color.FromArgb(27, 28, 34);
		public static readonly Color Row = Color.FromArgb(34, 35, 43);
		public static readonly Color RowHover = Color.FromArgb(44, 46, 56);
		public static readonly Color Accent = Color.FromArgb(47, 124, 246);
		public static readonly Color AccentSoft = Color.FromArgb(38, 63, 112);
		public static readonly Color Text = Color.FromArgb(236, 237, 240);
		public static readonly Color Muted = Color.FromArgb(150, 156, 168);
		public static readonly Color Keycap = Color.FromArgb(56, 59, 72);
		public static readonly Color KeycapEdge = Color.FromArgb(80, 84, 100);
		public static readonly Color Pink = Color.FromArgb(232, 92, 160);

		public static GraphicsPath Round(RectangleF r, float rad)
		{
			GraphicsPath p = new GraphicsPath();
			float d = rad * 2;
			if (d > r.Height) d = r.Height;
			if (d > r.Width) d = r.Width;
			p.AddArc(r.X, r.Y, d, d, 180, 90);
			p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
			p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
			p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
			p.CloseFigure();
			return p;
		}

		public static string Pretty(string key)
		{
			if (string.IsNullOrEmpty(key)) return "—";
			switch (key)
			{
				case "Up": return "↑";
				case "Down": return "↓";
				case "Left": return "←";
				case "Right": return "→";
				case "Return": return "Enter";
				case "PgUp": return "Page Up";
				case "PgDown": return "Page Down";
			}
			return key;
		}
	}

	class Binding
	{
		public string RegName;   // value name under keymap
		public string Label;     // what it does in the game
		public string Hint;      // small grey text
		public string Ps;        // controller id used by the drawing (L1, CROSS, LS...)
		public string PsText;    // shown in the badge
		public string DefaultKey;
		public string Key;
		public string Section;
	}

	// One clickable line: "Koş   [L3]   [Shift]"
	class ActionRow : Control
	{
		public readonly Binding B;
		public bool Capturing;
		bool hover;
		readonly MainForm form;

		public ActionRow(MainForm f, Binding b)
		{
			form = f; B = b;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, false);
			Height = 46;
			Cursor = Cursors.Hand;
			Margin = new Padding(0, 0, 0, 6);
		}

		protected override void OnMouseEnter(EventArgs e) { hover = true; form.SetHighlight(B.Ps); Invalidate(); base.OnMouseEnter(e); }
		protected override void OnMouseLeave(EventArgs e) { hover = false; form.SetHighlight(null); Invalidate(); base.OnMouseLeave(e); }
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) form.ClearBinding(B);
			else form.BeginCapture(this);
			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			g.Clear(Parent != null ? Parent.BackColor : Theme.Panel);

			RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1, Height - 1);
			Color bg = Capturing ? Theme.AccentSoft : (hover ? Theme.RowHover : Theme.Row);
			using (GraphicsPath p = Theme.Round(r, 8))
			{
				using (SolidBrush b = new SolidBrush(bg)) g.FillPath(b, p);
				if (Capturing) using (Pen pen = new Pen(Theme.Accent, 1.5f)) g.DrawPath(pen, p);
			}

			// Action name + hint
			using (Font f = new Font(Theme.Semi, 10.5f))
			using (SolidBrush tb = new SolidBrush(Theme.Text))
				g.DrawString(B.Label, f, tb, 14, B.Hint == null ? 13 : 5);
			if (B.Hint != null)
				using (Font f = new Font(Theme.Ui, 8.5f))
				using (SolidBrush mb = new SolidBrush(Theme.Muted))
					g.DrawString(B.Hint, f, mb, 15, 25);

			// Keycap (right)
			string keyText = Capturing ? Strings.T("press_key") : Theme.Pretty(B.Key);
			using (Font kf = new Font(Theme.Semi, 10f))
			{
				SizeF ks = g.MeasureString(keyText, kf);
				float kw = Math.Max(64, ks.Width + 22);
				RectangleF kr = new RectangleF(Width - kw - 10, 8, kw, Height - 16);
				using (GraphicsPath kp = Theme.Round(kr, 6))
				{
					using (SolidBrush kb = new SolidBrush(Capturing ? Theme.Accent : Theme.Keycap)) g.FillPath(kb, kp);
					using (Pen ep = new Pen(Capturing ? Theme.Accent : Theme.KeycapEdge)) g.DrawPath(ep, kp);
				}
				using (SolidBrush tb = new SolidBrush(string.IsNullOrEmpty(B.Key) && !Capturing ? Theme.Muted : Color.White))
				{
					StringFormat sf = new StringFormat(); sf.Alignment = StringAlignment.Center; sf.LineAlignment = StringAlignment.Center;
					g.DrawString(keyText, kf, tb, kr, sf);
				}

				// PS badge (left of keycap)
				if (B.PsText != null)
				{
					using (Font bf = new Font("Segoe UI Symbol", 9f, FontStyle.Bold))
					{
						SizeF bs = g.MeasureString(B.PsText, bf);
						float bw = Math.Max(34, bs.Width + 14);
						RectangleF br = new RectangleF(kr.X - bw - 12, 12, bw, Height - 24);
						using (GraphicsPath bp = Theme.Round(br, (Height - 24) / 2f))
						using (Pen pp = new Pen(Theme.Muted))
							g.DrawPath(pp, bp);
						StringFormat sf = new StringFormat(); sf.Alignment = StringAlignment.Center; sf.LineAlignment = StringAlignment.Center;
						using (SolidBrush mb = new SolidBrush(Theme.Muted)) g.DrawString(B.PsText, bf, mb, br, sf);
					}
				}
			}
		}
	}

	class MainForm : Form
	{
		const string RegRoot = @"Software\Chiaki\Chiaki";
		const string LauncherVersion = "6";
		static readonly string[] PsButtonNames = { "none", "l1", "r1", "l2", "r2", "l3", "r3", "cross", "moon", "pyramid", "box" };
		static readonly string[] PsButtonLabels = { "—", "L1", "R1", "L2", "R2", "L3", "R3", "✕", "○", "△", "▢" };
		CheckBox vehAuto, vehBeep, vehExitE, vehExitAim;
		ComboBox vehLeft, vehRight;
		readonly List<Binding> bindings = new List<Binding>();
		readonly List<ActionRow> rows = new List<ActionRow>();
		ActionRow capturing;
		ControllerView pad;
		TrackBar sensitivity, deadzone, bitrate;
		Label sensValue, deadValue, bitValue, status;
		CheckBox invertY, mouseOnAtStart;
		Button fps30, fps60;
		int fpsValue = 60;

		public MainForm()
		{
			Text = Strings.T("title");
			Font = new Font(Theme.Ui, 10f);
			StartPosition = FormStartPosition.CenterScreen;
			ClientSize = new Size(1240, 820);
			MinimumSize = new Size(980, 620);
			KeyPreview = true;
			BackColor = Theme.Bg;
			ForeColor = Theme.Text;

			// Defaults follow GTA 6's "Default" control scheme (on foot), with usual PC keys.
			Sec("sec_move");
			Add("left_stick_up", "a_forward", "h_forward", "LS", "L ↑", "W");
			Add("left_stick_down", "a_back", "h_back", "LS", "L ↓", "S");
			Add("left_stick_left", "a_left", null, "LS", "L ←", "A");
			Add("left_stick_right", "a_right", null, "LS", "L →", "D");
			Add("l3", "a_run", null, "L3", "L3", "Shift");
			Add("cross", "a_jump", null, "CROSS", "✕", "Space");
			Add("moon", "a_crouch", null, "CIRCLE", "○", "C");
			Add("r1", "a_cover", null, "R1", "R1", "Q");

			Sec("sec_combat");
			Add("l2", "a_aim", "h_aim", "L2", "L2", "Z");
			Add("r2", "a_shoot", "h_shoot", "R2", "R2", "");
			Add("box", "a_melee", null, "SQUARE", "▢", "R");
			Add("l1", "a_holster", null, "L1", "L1", "Tab");
			Add("r3", "a_lookbehind", "h_middle", "R3", "R3", "B");
			Add("focus_mode", "a_focus", "h_focus", "L3R3", "L3+R3", "CapsLock");

			Sec("sec_interact");
			Add("pyramid", "a_interact", null, "TRIANGLE", "△", "E");
			Add("touchpad", "a_map", null, "TOUCHPAD", "Touchpad", "M");
			Add("options", "a_pause", null, "OPTIONS", "Options", "Esc");
			Add("d-pad_up", "a_phone", null, "DUP", "D-pad ↑", "Up");
			Add("d-pad_down", "a_dpad_down", "h_dpad", "DDOWN", "D-pad ↓", "Down");
			Add("d-pad_left", "a_dpad_left", "h_dpad", "DLEFT", "D-pad ←", "Left");
			Add("d-pad_right", "a_dpad_right", "h_dpad", "DRIGHT", "D-pad →", "Right");

			Sec("sec_menus");
			Add("cross_alt", "a_confirm", "h_confirm", "CROSS", "✕", "Return");
			Add("moon_alt", "a_cancel", "h_cancel", "CIRCLE", "○", "X");

			Sec("sec_vehicle");
			Add("drive_mode_toggle", "a_drive", "h_drive", null, null, "V");

			Sec("sec_other");
			Add("ps", "a_ps", null, "PS", "PS", "Home");
			Add("share", "a_create", null, "SHARE", "Create", "F1");
			Add("right_stick_up", "a_cam_up", "h_cam", "RS", "R ↑", "I");
			Add("right_stick_down", "a_cam_down", "h_cam", "RS", "R ↓", "K");
			Add("right_stick_left", "a_cam_left", "h_cam", "RS", "R ←", "J");
			Add("right_stick_right", "a_cam_right", "h_cam", "RS", "R →", "L");

			BuildUi();
			LoadSettings();
		}

		string currentSection;
		void Sec(string key) { currentSection = Strings.T(key); }

		void Add(string reg, string labelKey, string hintKey, string ps, string psText, string def)
		{
			Binding b = new Binding();
			b.RegName = reg; b.Label = Strings.T(labelKey); b.Hint = hintKey == null ? null : Strings.T(hintKey);
			b.Ps = ps; b.PsText = psText;
			b.DefaultKey = def; b.Key = def; b.Section = currentSection;
			bindings.Add(b);
		}

		public void SetHighlight(string ps)
		{
			if (capturing != null) ps = capturing.B.Ps;
			pad.Highlight = ps;
			pad.Invalidate();
		}

		public void BeginCapture(ActionRow row)
		{
			if (capturing == row) { EndCapture(); return; }
			if (capturing != null) { capturing.Capturing = false; capturing.Invalidate(); }
			capturing = row;
			row.Capturing = true;
			row.Invalidate();
			status.Text = Strings.F("capture_prompt", row.B.Label);
			SetHighlight(row.B.Ps);
			ActiveControl = null;
		}

		void EndCapture()
		{
			if (capturing == null) return;
			capturing.Capturing = false;
			capturing.Invalidate();
			capturing = null;
			status.Text = "";
		}

		public void ClearBinding(Binding b)
		{
			b.Key = "";
			EndCapture();
			RefreshAll();
			status.Text = Strings.F("cleared", b.Label);
		}

		void RefreshAll()
		{
			foreach (ActionRow r in rows) r.Invalidate();
			pad.Invalidate();
		}

		string KeyForPs(string ps)
		{
			// The mouse buttons come first where they are the main control
			if (ps == "R2") return "LMB";
			if (ps == "L2") return "RMB";
			if (ps == "LS") return "WASD";
			if (ps == "RS") return null;
			foreach (Binding b in bindings)
				if (b.Ps == ps && !string.IsNullOrEmpty(b.Key)) return Theme.Pretty(b.Key);
			return null;
		}

		// ---------- layout ----------

		Label Text_(string text, float size, bool bold, Color color)
		{
			Label l = new Label();
			l.Text = text;
			l.AutoSize = true;
			l.UseMnemonic = false; // show "&" as text
			l.ForeColor = color;
			l.BackColor = Color.Transparent;
			l.Font = new Font(bold ? Theme.Semi : Theme.Ui, size);
			return l;
		}

		void BuildUi()
		{
			// Header
			Panel header = new Panel();
			header.Dock = DockStyle.Top;
			header.Height = 74;
			header.BackColor = Theme.Bg;
			header.Padding = new Padding(24, 14, 24, 0);
			Label title = Text_(Strings.T("title"), 17f, true, Theme.Text);
			title.Location = new Point(22, 10);
			Label sub = Text_(Strings.T("subtitle"), 9.5f, false, Theme.Muted);
			sub.Location = new Point(25, 44);
			header.Controls.Add(title); header.Controls.Add(sub);

			// Language picker (top right)
			ComboBox langBox = new ComboBox();
			langBox.DropDownStyle = ComboBoxStyle.DropDownList;
			langBox.FlatStyle = FlatStyle.Flat;
			langBox.BackColor = Theme.Keycap;
			langBox.ForeColor = Theme.Text;
			langBox.Width = 150;
			langBox.TabStop = false;
			langBox.Font = new Font("Segoe UI", 10f);
			foreach (string n in Strings.Names) langBox.Items.Add(n);
			langBox.SelectedIndex = Array.IndexOf(Strings.Codes, Strings.Code);
			langBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Label langLabel = Text_(Strings.T("language"), 9.5f, false, Theme.Muted);
			langLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			header.Controls.Add(langBox); header.Controls.Add(langLabel);
			header.Resize += delegate
			{
				langBox.Location = new Point(header.Width - langBox.Width - 24, 20);
				langLabel.Location = new Point(langBox.Left - langLabel.Width - 8, 24);
			};
			langBox.SelectedIndexChanged += delegate
			{
				string code = Strings.Codes[langBox.SelectedIndex];
				if (code == Strings.Code) return;
				SaveLanguage(code);
				Program.RestartWith = code;
				Close();
			};

			// Bottom bar
			Panel bottom = new Panel();
			bottom.Dock = DockStyle.Bottom;
			bottom.Height = 72;
			bottom.BackColor = Theme.Panel;
			Button start = Action_(Strings.T("start"), true, 170);
			start.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			start.Click += delegate { if (Save()) StartChiaki(); };
			Button save = Action_(Strings.T("save"), false, 120);
			save.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			save.Click += delegate { if (Save()) status.Text = Strings.T("saved"); };
			Button defaults = Action_(Strings.T("defaults"), false, 150);
			defaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			defaults.Click += delegate { ResetDefaults(); };
			status = Text_("", 9.5f, false, Theme.Muted);
			status.AutoSize = false;
			status.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			bottom.Controls.Add(start); bottom.Controls.Add(save); bottom.Controls.Add(defaults); bottom.Controls.Add(status);
			bottom.Resize += delegate
			{
				start.Location = new Point(bottom.Width - start.Width - 24, 16);
				save.Location = new Point(start.Left - save.Width - 10, 16);
				defaults.Location = new Point(24, 16);
				status.Location = new Point(defaults.Right + 16, 26);
				status.Size = new Size(Math.Max(10, save.Left - status.Left - 12), 22);
			};

			// Left: action list
			Panel listHost = new Panel();
			listHost.Dock = DockStyle.Fill;
			listHost.BackColor = Theme.Bg;
			listHost.Padding = new Padding(24, 4, 8, 12);

			FlowLayoutPanel list = new FlowLayoutPanel();
			list.Dock = DockStyle.Fill;
			list.FlowDirection = FlowDirection.TopDown;
			list.WrapContents = false;
			list.AutoScroll = true;
			list.BackColor = Theme.Bg;
			list.Padding = new Padding(0, 0, 14, 0);
			listHost.Controls.Add(list);

			string lastSection = null;
			foreach (Binding b in bindings)
			{
				if (b.Section != lastSection)
				{
					Label sh = Text_(b.Section.ToUpper(Strings.Code == "tr" ? new CultureInfo("tr-TR") : CultureInfo.InvariantCulture), 9f, true, Theme.Pink);
					sh.Margin = new Padding(2, lastSection == null ? 6 : 18, 0, 8);
					list.Controls.Add(sh);
					lastSection = b.Section;
				}
				ActionRow row = new ActionRow(this, b);
				rows.Add(row);
				list.Controls.Add(row);
			}
			list.Resize += delegate
			{
				int w = list.ClientSize.Width - list.Padding.Horizontal - 2;
				foreach (ActionRow r in rows) r.Width = w;
			};

			// Right: controller + mouse + stream
			Panel side = new Panel();
			side.Dock = DockStyle.Right;
			side.Width = 580;
			side.BackColor = Theme.Bg;
			side.Padding = new Padding(8, 4, 24, 12);

			Panel card = new Panel();
			card.Dock = DockStyle.Fill;
			card.BackColor = Theme.Panel;
			card.Padding = new Padding(18, 12, 18, 12);
			side.Controls.Add(card);

			pad = new ControllerView();
			pad.BackColor = Theme.Panel;
			pad.Dock = DockStyle.Top;
			pad.Height = 300;
			pad.KeyFor = KeyForPs;

			FlowLayoutPanel opts = new FlowLayoutPanel();
			opts.Dock = DockStyle.Fill;
			opts.FlowDirection = FlowDirection.TopDown;
			opts.WrapContents = false;
			opts.AutoScroll = true;
			opts.BackColor = Theme.Panel;

			Label mh = Text_(Strings.T("mouse"), 9f, true, Theme.Pink); mh.Margin = new Padding(0, 8, 0, 4);
			opts.Controls.Add(mh);
			Label mInfo = Text_(Strings.T("mouse_info"), 8.8f, false, Theme.Muted);
			mInfo.MaximumSize = new Size(410, 0);
			mInfo.Margin = new Padding(0, 0, 0, 8);
			opts.Controls.Add(mInfo);

			sensitivity = Slider(1, 40, 20);
			sensValue = Text_("", 10f, true, Theme.Text);
			sensitivity.ValueChanged += delegate { sensValue.Text = (sensitivity.Value / 2.0).ToString("0.0", CultureInfo.InvariantCulture); };
			opts.Controls.Add(SliderRow(Strings.T("sensitivity"), sensitivity, sensValue));

			deadzone = Slider(0, 50, 20);
			deadValue = Text_("", 10f, true, Theme.Text);
			deadzone.ValueChanged += delegate { deadValue.Text = (deadzone.Value / 100.0).ToString("0.00", CultureInfo.InvariantCulture); };
			opts.Controls.Add(SliderRow(Strings.T("deadzone"), deadzone, deadValue));

			invertY = Check(Strings.T("invert_y"));
			opts.Controls.Add(invertY);
			mouseOnAtStart = Check(Strings.T("mouse_on"));
			opts.Controls.Add(mouseOnAtStart);

			Label vhh = Text_(Strings.T("sec_vehicle").ToUpper(Strings.Code == "tr" ? new CultureInfo("tr-TR") : CultureInfo.InvariantCulture), 9f, true, Theme.Pink);
			vhh.Margin = new Padding(0, 16, 0, 4);
			opts.Controls.Add(vhh);
			Label vInfo = Text_(Strings.T("veh_info"), 8.8f, false, Theme.Muted);
			vInfo.MaximumSize = new Size(480, 0);
			vInfo.Margin = new Padding(0, 0, 0, 6);
			opts.Controls.Add(vInfo);
			// The automatic switch on △ guessed wrong too often (△ does much more than enter vehicles);
			// it stays off and isn't offered in the UI.
			vehAuto = new CheckBox();
			vehBeep = new CheckBox();
			// Turning vehicle mode OFF automatically is safe: △ in a vehicle only exits it,
			// and right click in a vehicle does nothing, so on foot it means aiming.
			vehExitE = Check(Strings.T("veh_exit_e"));
			opts.Controls.Add(vehExitE);
			// Not offered: right click in a vehicle is how people try to aim after picking a weapon,
			// and turning vehicle mode off then made left click accelerate and right click brake.
			vehExitAim = new CheckBox();
			vehRight = ButtonPicker();
			opts.Controls.Add(PickerRow(Strings.T("veh_right"), vehRight));
			vehLeft = ButtonPicker();
			opts.Controls.Add(PickerRow(Strings.T("veh_left"), vehLeft));

			Label vh = Text_(Strings.T("stream"), 9f, true, Theme.Pink); vh.Margin = new Padding(0, 16, 0, 6);
			opts.Controls.Add(vh);
			FlowLayoutPanel fpsRow = new FlowLayoutPanel();
			fpsRow.AutoSize = true; fpsRow.BackColor = Theme.Panel; fpsRow.Margin = new Padding(0, 0, 0, 6);
			Label fl = Text_("FPS", 10f, false, Theme.Text); fl.Margin = new Padding(0, 7, 8, 0); fl.Width = 150; fl.AutoSize = false;
			fps30 = Action_("30", false, 70); fps60 = Action_("60", false, 70);
			fps30.Height = 32; fps60.Height = 32;
			fps30.Click += delegate { SetFps(30); };
			fps60.Click += delegate { SetFps(60); };
			fpsRow.Controls.Add(fl); fpsRow.Controls.Add(fps30); fpsRow.Controls.Add(fps60);
			opts.Controls.Add(fpsRow);
			Label fpsInfo = Text_(Strings.T("fps_info"), 8.8f, false, Theme.Muted);
			fpsInfo.Margin = new Padding(0, 0, 0, 6);
			opts.Controls.Add(fpsInfo);

			bitrate = Slider(1, 20, 10);
			bitValue = Text_("", 10f, true, Theme.Text);
			bitrate.ValueChanged += delegate { bitValue.Text = (bitrate.Value * 5) + " Mbps"; };
			opts.Controls.Add(SliderRow("Bitrate", bitrate, bitValue));

			card.Controls.Add(opts);
			card.Controls.Add(pad);

			Controls.Add(listHost);
			Controls.Add(side);
			Controls.Add(bottom);
			Controls.Add(header);

			// Clicking anywhere else stops waiting for a key
			MouseDown += delegate { EndCapture(); };
		}

		Button Action_(string text, bool primary, int width)
		{
			Button b = new Button();
			b.Text = text;
			b.Width = width;
			b.Height = 40;
			b.FlatStyle = FlatStyle.Flat;
			b.FlatAppearance.BorderSize = 0;
			b.TabStop = false;
			b.Cursor = Cursors.Hand;
			b.Font = new Font(Theme.Semi, primary ? 11f : 10f);
			b.BackColor = primary ? Theme.Accent : Theme.Keycap;
			b.ForeColor = Color.White;
			b.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(70, 142, 255) : Theme.RowHover;
			return b;
		}

		TrackBar Slider(int min, int max, int val)
		{
			TrackBar t = new TrackBar();
			t.Minimum = min; t.Maximum = max; t.Value = val;
			t.TickStyle = TickStyle.None;
			t.Width = 220;
			t.BackColor = Theme.Panel;
			t.TabStop = false;
			return t;
		}

		Control SliderRow(string label, TrackBar t, Label value)
		{
			FlowLayoutPanel p = new FlowLayoutPanel();
			p.AutoSize = true;
			p.WrapContents = false;
			p.BackColor = Theme.Panel;
			p.Margin = new Padding(0, 0, 0, 2);
			t.Width = 170;
			Label l = Text_(label, 10f, false, Theme.Text);
			l.AutoSize = false; l.Width = 150; l.Height = 24; l.Margin = new Padding(0, 6, 8, 0);
			value.Margin = new Padding(4, 6, 0, 0);
			p.Controls.Add(l); p.Controls.Add(t); p.Controls.Add(value);
			return p;
		}

		ComboBox ButtonPicker()
		{
			ComboBox c = new ComboBox();
			c.DropDownStyle = ComboBoxStyle.DropDownList;
			c.FlatStyle = FlatStyle.Flat;
			c.BackColor = Theme.Keycap;
			c.ForeColor = Theme.Text;
			c.Width = 90;
			c.TabStop = false;
			c.Font = new Font("Segoe UI Symbol", 10f);
			foreach (string l in PsButtonLabels) c.Items.Add(l);
			return c;
		}

		Control PickerRow(string label, ComboBox c)
		{
			FlowLayoutPanel p = new FlowLayoutPanel();
			p.AutoSize = true;
			p.WrapContents = false;
			p.BackColor = Theme.Panel;
			p.Margin = new Padding(0, 2, 0, 2);
			Label l = Text_(label, 10f, false, Theme.Text);
			l.AutoSize = false; l.Width = 250; l.Height = 24; l.Margin = new Padding(0, 6, 8, 0);
			p.Controls.Add(l); p.Controls.Add(c);
			return p;
		}

		static void SelectButton(ComboBox c, string name, string fallback)
		{
			int i = Array.IndexOf(PsButtonNames, (name ?? "").Trim().ToLowerInvariant());
			if (i < 0) i = Array.IndexOf(PsButtonNames, fallback);
			c.SelectedIndex = i;
		}

		CheckBox Check(string text)
		{
			CheckBox c = new CheckBox();
			c.Text = text;
			c.AutoSize = true;
			c.ForeColor = Theme.Text;
			c.BackColor = Theme.Panel;
			c.Margin = new Padding(0, 4, 0, 4);
			c.TabStop = false;
			return c;
		}

		void SetFps(int v)
		{
			fpsValue = v;
			fps30.BackColor = v == 30 ? Theme.Accent : Theme.Keycap;
			fps60.BackColor = v == 60 ? Theme.Accent : Theme.Keycap;
		}

		// ---------- key capture ----------

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (capturing != null)
			{
				Assign(keyData & Keys.KeyCode);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		void Assign(Keys code)
		{
			string name = QtName(code);
			if (name == null)
			{
				if (code == Keys.F8 || code == Keys.F11)
					status.Text = Strings.T("reserved");
				else
					status.Text = Strings.T("unsupported");
				return;
			}
			Binding target = capturing.B;
			// Same key on another action? Swap them so nothing is bound twice.
			foreach (Binding other in bindings)
				if (other != target && other.Key == name)
					other.Key = target.Key;
			target.Key = name;
			EndCapture();
			status.Text = name == "Ctrl" ? Strings.T("ctrl_note") : "\"" + target.Label + "\" → " + Theme.Pretty(name);
			RefreshAll();
		}

		static string QtName(Keys k)
		{
			if (k >= Keys.A && k <= Keys.Z) return ((char)('A' + (k - Keys.A))).ToString();
			if (k >= Keys.D0 && k <= Keys.D9) return ((char)('0' + (k - Keys.D0))).ToString();
			if (k >= Keys.NumPad0 && k <= Keys.NumPad9) return ((char)('0' + (k - Keys.NumPad0))).ToString();
			if (k >= Keys.F1 && k <= Keys.F24)
			{
				if (k == Keys.F8 || k == Keys.F11) return null;
				return "F" + (1 + (k - Keys.F1));
			}
			switch (k)
			{
				case Keys.Space: return "Space";
				case Keys.Tab: return "Tab";
				case Keys.Escape: return "Esc";
				case Keys.Return: return "Return";
				case Keys.Back: return "Backspace";
				case Keys.Up: return "Up";
				case Keys.Down: return "Down";
				case Keys.Left: return "Left";
				case Keys.Right: return "Right";
				case Keys.Home: return "Home";
				case Keys.End: return "End";
				case Keys.PageUp: return "PgUp";
				case Keys.PageDown: return "PgDown";
				case Keys.Insert: return "Ins";
				case Keys.Delete: return "Del";
				case Keys.CapsLock: return "CapsLock";
				case Keys.ShiftKey: case Keys.LShiftKey: case Keys.RShiftKey: return "Shift";
				case Keys.ControlKey: case Keys.LControlKey: case Keys.RControlKey: return "Ctrl";
				case Keys.Menu: case Keys.LMenu: case Keys.RMenu: return "Alt";
			}
			return null;
		}

		// ---------- settings ----------

		static object Get(RegistryKey k, string name) { return k == null ? null : k.GetValue(name); }

		static double ToDouble(object v, double def)
		{
			if (v == null) return def;
			double d;
			return double.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : def;
		}

		static int ToInt(object v, int def)
		{
			if (v == null) return def;
			int i;
			return int.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), out i) ? i : def;
		}

		static bool ToBool(object v, bool def)
		{
			if (v == null) return def;
			string s = Convert.ToString(v, CultureInfo.InvariantCulture).Trim().ToLowerInvariant();
			if (s == "true" || s == "1") return true;
			if (s == "false" || s == "0") return false;
			return def;
		}

		static int Clamp(int v, TrackBar t) { return Math.Max(t.Minimum, Math.Min(t.Maximum, v)); }

		void LoadSettings()
		{
			using (RegistryKey s = Registry.CurrentUser.OpenSubKey(RegRoot + @"\settings"))
			{
				// New version of the default layout: start from it once instead of older leftovers
				if (Program.ShowDefaults || Convert.ToString(Get(s, "mouselauncher_version")) != LauncherVersion)
				{
					ResetDefaults();
					status.Text = Strings.T("first_run");
					return;
				}
				sensitivity.Value = Clamp((int)Math.Round(ToDouble(Get(s, "mouselook_sensitivity"), 10) * 2), sensitivity);
				deadzone.Value = Clamp((int)Math.Round(ToDouble(Get(s, "mouselook_deadzone"), 0.2) * 100), deadzone);
				invertY.Checked = ToBool(Get(s, "mouselook_invert_y"), false);
				mouseOnAtStart.Checked = ToBool(Get(s, "mouselook_enabled"), true);
				vehAuto.Checked = false;
				vehExitE.Checked = ToBool(Get(s, "vehicle_exit_on_interact"), true);
				vehExitAim.Checked = false;
				vehBeep.Checked = false;
				SelectButton(vehLeft, Convert.ToString(Get(s, "vehicle_mouse_left")), "r1");
				SelectButton(vehRight, Convert.ToString(Get(s, "vehicle_mouse_right")), "none");
				SetFps(ToInt(Get(s, "fps_local_ps5"), 60) == 30 ? 30 : 60);
				int kbps = ToInt(Get(s, "bitrate_local_ps5"), 50000);
				bitrate.Value = Clamp((int)Math.Round(kbps / 5000.0), bitrate);
			}
			using (RegistryKey km = Registry.CurrentUser.OpenSubKey(RegRoot + @"\keymap"))
			{
				foreach (Binding b in bindings)
				{
					object v = Get(km, b.RegName);
					if (v != null) b.Key = Convert.ToString(v).Trim();
				}
			}
			UpdateValueLabels();
			RefreshAll();
		}

		void UpdateValueLabels()
		{
			sensValue.Text = (sensitivity.Value / 2.0).ToString("0.0", CultureInfo.InvariantCulture);
			deadValue.Text = (deadzone.Value / 100.0).ToString("0.00", CultureInfo.InvariantCulture);
			bitValue.Text = (bitrate.Value * 5) + " Mbps";
		}

		void ResetDefaults()
		{
			foreach (Binding b in bindings) b.Key = b.DefaultKey;
			sensitivity.Value = 20; deadzone.Value = 20; invertY.Checked = false; mouseOnAtStart.Checked = true;
			SetFps(60); bitrate.Value = 10;
			vehAuto.Checked = false; vehBeep.Checked = false;
			vehExitE.Checked = true; vehExitAim.Checked = false;
			SelectButton(vehLeft, "r1", "r1");
			// GTA VI vehicles: R1 shoots, L1 is holster / weapon select; there is no separate aim
			SelectButton(vehRight, "none", "none");
			EndCapture();
			UpdateValueLabels();
			RefreshAll();
			status.Text = Strings.T("defaults_loaded");
		}

		bool Save()
		{
			EndCapture();
			try
			{
				using (RegistryKey km = Registry.CurrentUser.CreateSubKey(RegRoot + @"\keymap"))
					foreach (Binding b in bindings)
						km.SetValue(b.RegName, b.Key ?? "", RegistryValueKind.String);
				using (RegistryKey s = Registry.CurrentUser.CreateSubKey(RegRoot + @"\settings"))
				{
					CultureInfo inv = CultureInfo.InvariantCulture;
					s.SetValue("mouselook_sensitivity", (sensitivity.Value / 2.0).ToString(inv), RegistryValueKind.String);
					s.SetValue("mouselook_deadzone", (deadzone.Value / 100.0).ToString(inv), RegistryValueKind.String);
					s.SetValue("mouselook_invert_y", invertY.Checked ? "true" : "false", RegistryValueKind.String);
					s.SetValue("mouselook_enabled", mouseOnAtStart.Checked ? "true" : "false", RegistryValueKind.String);
					s.SetValue("vehicle_auto", vehAuto.Checked ? "true" : "false", RegistryValueKind.String);
					s.SetValue("vehicle_exit_on_interact", vehExitE.Checked ? "true" : "false", RegistryValueKind.String);
					s.SetValue("vehicle_exit_on_aim", vehExitAim.Checked ? "true" : "false", RegistryValueKind.String);
					s.SetValue("vehicle_beep", vehBeep.Checked ? "true" : "false", RegistryValueKind.String);
					s.SetValue("vehicle_mouse_left", PsButtonNames[Math.Max(0, vehLeft.SelectedIndex)], RegistryValueKind.String);
					s.SetValue("vehicle_mouse_right", PsButtonNames[Math.Max(0, vehRight.SelectedIndex)], RegistryValueKind.String);
					s.SetValue("fps_local_ps5", fpsValue, RegistryValueKind.DWord);
					s.SetValue("bitrate_local_ps5", bitrate.Value * 5000, RegistryValueKind.DWord);
					s.SetValue("mouselauncher_version", LauncherVersion, RegistryValueKind.String);
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, Strings.T("save_failed") + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		public static string LoadLanguage()
		{
			using (RegistryKey s = Registry.CurrentUser.OpenSubKey(RegRoot + @"\settings"))
			{
				object v = Get(s, "mouselauncher_lang");
				string code = v == null ? null : Convert.ToString(v);
				return Array.IndexOf(Strings.Codes, code) >= 0 ? code : "en";
			}
		}

		static void SaveLanguage(string code)
		{
			try
			{
				using (RegistryKey s = Registry.CurrentUser.CreateSubKey(RegRoot + @"\settings"))
					s.SetValue("mouselauncher_lang", code, RegistryValueKind.String);
			}
			catch (Exception) { }
		}

		void StartChiaki()
		{
			string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chiaki-ng-Win");
			string exe = Path.Combine(dir, "chiaki.exe");
			if (!File.Exists(exe))
			{
				MessageBox.Show(this, Strings.T("exe_missing") + exe, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (Process.GetProcessesByName("chiaki").Length > 0)
			{
				DialogResult r = MessageBox.Show(this, Strings.T("already_open"),
					Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (r == DialogResult.Cancel) return;
				if (r == DialogResult.No) { Close(); return; }
				foreach (Process p in Process.GetProcessesByName("chiaki"))
				{
					try { p.CloseMainWindow(); if (!p.WaitForExit(5000)) p.Kill(); } catch (Exception) { }
				}
			}
			ProcessStartInfo psi = new ProcessStartInfo(exe);
			psi.WorkingDirectory = dir;
			psi.UseShellExecute = true;
			Process.Start(psi);
			Close();
		}
	}

	static class Program
	{
		// Set when the language changes: the window is rebuilt in the new language
		public static string RestartWith;
		// --defaults: open with the default layout instead of the saved keys (nothing is saved until you press Save / START)
		public static bool ShowDefaults;

		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			string lang = MainForm.LoadLanguage();
			// --lang xx: open in that language this time (en, tr, es, pt, fr, de, ru, zh)
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "--defaults") ShowDefaults = true;
				else if (args[i] == "--lang" && i + 1 < args.Length && Array.IndexOf(Strings.Codes, args[i + 1]) >= 0) lang = args[++i];
			}
			while (true)
			{
				Strings.SetLanguage(lang);
				Theme.ForLanguage(lang);
				RestartWith = null;
				Application.Run(new MainForm());
				if (RestartWith == null) break;
				lang = RestartWith;
			}
		}
	}
}
