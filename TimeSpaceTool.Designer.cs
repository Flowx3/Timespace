using MaterialSkin.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace TimeSpace
{
    partial class TimeSpaceTool
    {
        /// <summary>  
        ///  Required designer variable.  
        /// </summary>  
        private System.ComponentModel.IContainer components = null;

        /// <summary>  
        ///  Clean up any resources being used.  
        /// </summary>  
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>  
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code  

        /// <summary>  
        ///  Required method for Designer support - do not modify  
        ///  the contents of this method with the code editor.  
        /// </summary>  
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeSpaceTool));
            openFileDialog1 = new OpenFileDialog();
            tabPage3 = new TabPage();
            tabControl2 = new ModernTabControl();
            panelButtons = new Panel();
            button4 = new MaterialButton();
            button5 = new MaterialButton();
            button6 = new MaterialButton();
            IsHiddenLabel = new Label();
            IsHeroLabel = new Label();
            IsSpecialLabel = new Label();
            textBox7 = new ModernTextBox();
            tabPage1 = new TabPage();
            materialButton1 = new MaterialButton();
            modernTextBox1 = new ModernTextBox();
            button9 = new MaterialButton();
            textBox13 = new ModernTextBox();
            button8 = new MaterialButton();
            button7 = new MaterialButton();
            textBox12 = new ModernTextBox();
            textBox11 = new ModernTextBox();
            textBox1 = new ModernTextBox();
            button1 = new MaterialButton();
            modernTextBox2 = new ModernTextBox();
            materialButton2 = new MaterialButton();
            tabControl1 = new ModernTabControl();
            tabPage2 = new TabPage();
            textBox2 = new TextBox();
            label9 = new Label();
            label6 = new Label();
            modernTextBox6 = new ModernTextBox();
            modernTextBox4 = new ModernTextBox();
            modernTextBox5 = new ModernTextBox();
            label5 = new Label();
            textBox4 = new TextBox();
            modernTextBox3 = new TextBox();
            pictureBox2 = new PictureBox();
            textBox15 = new ModernTextBox();
            label4 = new Label();
            textBox9 = new TextBox();
            textBox14 = new TextBox();
            textBox10 = new TextBox();
            pictureBox5 = new PictureBox();
            textBox8 = new ModernTextBox();
            numericUpDown2 = new ModernNumericUpDown();
            textBox5 = new ModernTextBox();
            textBox3 = new ModernTextBox();
            label8 = new Label();
            label7 = new Label();
            label3 = new Label();
            label2 = new Label();
            objectivesPanel = new CollapsibleObjectivesPanel();
            headerLabel = new Label();
            isHeroCheckBox = new CheckBox();
            isSpecialCheckBox = new CheckBox();
            isHiddenCheckBox = new CheckBox();
            pictureBox3 = new PictureBox();
            pictureBox1 = new PictureBox();
            pictureBox4 = new PictureBox();
            minPlayersLabel = new Label();
            maxPlayersLabel = new Label();
            label1 = new Label();
            tabPage3.SuspendLayout();
            panelButtons.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            objectivesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            DrawItemSlot1 = new ItemSlot(config);
            DrawItemSlot2 = new ItemSlot(config);
            DrawItemSlot3 = new ItemSlot(config);
            DrawItemSlot4 = new ItemSlot(config);
            DrawItemSlot5 = new ItemSlot(config);
            SpecialItemSlot1 = new ItemSlot(config);
            SpecialItemSlot2 = new ItemSlot(config);
            BonusItemSlot1 = new ItemSlot(config);
            BonusItemSlot2 = new ItemSlot(config);
            BonusItemSlot3 = new ItemSlot(config);
            // 
            // DrawItemSlot1
            // 
            DrawItemSlot1.Location = new Point(299, 501);
            DrawItemSlot1.Name = "DrawItemSlot1";
            DrawItemSlot1.Size = new Size(105, 88);
            DrawItemSlot1.TabIndex = 111;
            DrawItemSlot1.TabStop = false;
            // 
            // DrawItemSlot2
            // 
            DrawItemSlot2.Location = new Point(431, 500);
            DrawItemSlot2.Name = "DrawItemSlot2";
            DrawItemSlot2.Size = new Size(105, 88);
            DrawItemSlot2.TabIndex = 112;
            DrawItemSlot2.TabStop = false;
            // 
            // DrawItemSlot3
            // 
            DrawItemSlot3.Location = new Point(563, 500);
            DrawItemSlot3.Name = "DrawItemSlot3";
            DrawItemSlot3.Size = new Size(105, 88);
            DrawItemSlot3.TabIndex = 113;
            DrawItemSlot3.TabStop = false;
            // 
            // DrawItemSlot4
            // 
            DrawItemSlot4.Location = new Point(695, 500);
            DrawItemSlot4.Name = "DrawItemSlot4";
            DrawItemSlot4.Size = new Size(105, 88);
            DrawItemSlot4.TabIndex = 114;
            DrawItemSlot4.TabStop = false;
            // 
            // DrawItemSlot5
            // 
            DrawItemSlot5.Location = new Point(827, 500);
            DrawItemSlot5.Name = "DrawItemSlot5";
            DrawItemSlot5.Size = new Size(105, 88);
            DrawItemSlot5.TabIndex = 115;
            DrawItemSlot5.TabStop = false;
            // 
            // SpecialItemSlot1
            // 
            SpecialItemSlot1.Location = new Point(209, 623);
            SpecialItemSlot1.Name = "SpecialItemSlot1";
            SpecialItemSlot1.Size = new Size(105, 88);
            SpecialItemSlot1.TabIndex = 116;
            SpecialItemSlot1.TabStop = false;
            // 
            // SpecialItemSlot2
            // 
            SpecialItemSlot2.Location = new Point(342, 623);
            SpecialItemSlot2.Name = "SpecialItemSlot2";
            SpecialItemSlot2.Size = new Size(105, 88);
            SpecialItemSlot2.TabIndex = 117;
            SpecialItemSlot2.TabStop = false;
            // 
            // BonusItemSlot1
            // 
            BonusItemSlot1.Location = new Point(677, 623);
            BonusItemSlot1.Name = "BonusItemSlot1";
            BonusItemSlot1.Size = new Size(105, 88);
            BonusItemSlot1.TabIndex = 118;
            BonusItemSlot1.TabStop = false;
            // 
            // BonusItemSlot2
            // 
            BonusItemSlot2.Location = new Point(809, 623);
            BonusItemSlot2.Name = "BonusItemSlot2";
            BonusItemSlot2.Size = new Size(105, 88);
            BonusItemSlot2.TabIndex = 119;
            BonusItemSlot2.TabStop = false;
            // 
            // BonusItemSlot3
            // 
            BonusItemSlot3.Location = new Point(941, 623);
            BonusItemSlot3.Name = "BonusItemSlot3";
            BonusItemSlot3.Size = new Size(105, 88);
            BonusItemSlot3.TabIndex = 120;
            BonusItemSlot3.TabStop = false;
            tabPage2.Controls.Add(BonusItemSlot3);
            tabPage2.Controls.Add(BonusItemSlot2);
            tabPage2.Controls.Add(BonusItemSlot1);
            tabPage2.Controls.Add(SpecialItemSlot2);
            tabPage2.Controls.Add(SpecialItemSlot1);
            tabPage2.Controls.Add(DrawItemSlot5);
            tabPage2.Controls.Add(DrawItemSlot4);
            tabPage2.Controls.Add(DrawItemSlot3);
            tabPage2.Controls.Add(DrawItemSlot2);
            tabPage2.Controls.Add(DrawItemSlot1);
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabPage3
            // 
            tabPage3.AutoScroll = true;
            tabPage3.BackColor = Color.FromArgb(45, 45, 48);
            tabPage3.Controls.Add(tabControl2);
            tabPage3.Controls.Add(panelButtons);
            tabPage3.ForeColor = Color.White;
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1823, 825);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "TimeSpace Editor";
            // 
            // tabControl2
            // 
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.ItemSize = new Size(75, 25);
            tabControl2.Location = new Point(3, 3);
            tabControl2.Name = "tabControl2";
            tabControl2.Padding = new Point(0, 0);
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(1817, 759);
            tabControl2.SizeMode = TabSizeMode.Fixed;
            tabControl2.TabIndex = 4;
            tabControl2.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            // 
            // panelButtons
            // 
            panelButtons.BackColor = Color.FromArgb(45, 45, 48);
            panelButtons.Controls.Add(button4);
            panelButtons.Controls.Add(button5);
            panelButtons.Controls.Add(button6);
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Location = new Point(3, 762);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(1817, 60);
            panelButtons.TabIndex = 5;
            // 
            // button4
            // 
            button4.AutoSize = false;
            button4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button4.BackColor = Color.FromArgb(28, 28, 28);
            button4.Density = MaterialButton.MaterialButtonDensity.Default;
            button4.Depth = 0;
            button4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button4.ForeColor = Color.White;
            button4.HighEmphasis = true;
            button4.Icon = null;
            button4.Location = new Point(10, 10);
            button4.Margin = new Padding(4, 6, 4, 6);
            button4.MouseState = MaterialSkin.MouseState.HOVER;
            button4.Name = "button4";
            button4.NoAccentTextColor = Color.Empty;
            button4.Size = new Size(100, 50);
            button4.TabIndex = 0;
            button4.Text = "Remove";
            button4.Type = MaterialButton.MaterialButtonType.Contained;
            button4.UseAccentColor = false;
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.AutoSize = false;
            button5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button5.BackColor = Color.FromArgb(28, 28, 28);
            button5.Density = MaterialButton.MaterialButtonDensity.Default;
            button5.Depth = 0;
            button5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button5.ForeColor = Color.White;
            button5.HighEmphasis = true;
            button5.Icon = null;
            button5.Location = new Point(120, 10);
            button5.Margin = new Padding(4, 6, 4, 6);
            button5.MouseState = MaterialSkin.MouseState.HOVER;
            button5.Name = "button5";
            button5.NoAccentTextColor = Color.Empty;
            button5.Size = new Size(100, 50);
            button5.TabIndex = 1;
            button5.Text = "Add";
            button5.Type = MaterialButton.MaterialButtonType.Contained;
            button5.UseAccentColor = false;
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.AutoSize = false;
            button6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button6.BackColor = Color.FromArgb(28, 28, 28);
            button6.Density = MaterialButton.MaterialButtonDensity.Default;
            button6.Depth = 0;
            button6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button6.ForeColor = Color.White;
            button6.HighEmphasis = true;
            button6.Icon = null;
            button6.Location = new Point(230, 10);
            button6.Margin = new Padding(4, 6, 4, 6);
            button6.MouseState = MaterialSkin.MouseState.HOVER;
            button6.Name = "button6";
            button6.NoAccentTextColor = Color.Empty;
            button6.Size = new Size(100, 50);
            button6.TabIndex = 2;
            button6.Text = "Generate";
            button6.Type = MaterialButton.MaterialButtonType.Contained;
            button6.UseAccentColor = false;
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // IsHiddenLabel
            // 
            IsHiddenLabel.Location = new Point(0, 0);
            IsHiddenLabel.Name = "IsHiddenLabel";
            IsHiddenLabel.Size = new Size(100, 23);
            IsHiddenLabel.TabIndex = 0;
            // 
            // IsHeroLabel
            // 
            IsHeroLabel.Location = new Point(0, 0);
            IsHeroLabel.Name = "IsHeroLabel";
            IsHeroLabel.Size = new Size(100, 23);
            IsHeroLabel.TabIndex = 0;
            // 
            // IsSpecialLabel
            // 
            IsSpecialLabel.Location = new Point(0, 0);
            IsSpecialLabel.Name = "IsSpecialLabel";
            IsSpecialLabel.Size = new Size(100, 23);
            IsSpecialLabel.TabIndex = 0;
            // 
            // textBox7
            // 
            textBox7.BackColor = Color.FromArgb(28, 28, 28);
            textBox7.BorderStyle = BorderStyle.FixedSingle;
            textBox7.Font = new Font("Segoe UI", 10F);
            textBox7.ForeColor = Color.White;
            textBox7.Location = new Point(0, 0);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(100, 25);
            textBox7.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.AutoScroll = true;
            tabPage1.BackColor = Color.FromArgb(45, 45, 45);
            tabPage1.Controls.Add(materialButton1);
            tabPage1.Controls.Add(modernTextBox1);
            tabPage1.Controls.Add(button9);
            tabPage1.Controls.Add(textBox13);
            tabPage1.Controls.Add(button8);
            tabPage1.Controls.Add(button7);
            tabPage1.Controls.Add(textBox12);
            tabPage1.Controls.Add(textBox11);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(modernTextBox2);
            tabPage1.Controls.Add(materialButton2);
            tabPage1.ForeColor = Color.White;
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1823, 825);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Settings";
            // 
            // materialButton1
            // 
            materialButton1.AutoSize = false;
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.BackColor = Color.FromArgb(28, 28, 28);
            materialButton1.Density = MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.Font = new Font("Segoe UI", 9F);
            materialButton1.ForeColor = Color.White;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.Location = new Point(662, 227);
            materialButton1.Margin = new Padding(4, 6, 4, 6);
            materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(75, 23);
            materialButton1.TabIndex = 11;
            materialButton1.Text = "Browse";
            materialButton1.Type = MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = false;
            materialButton1.Click += MaterialButton1_Click;
            // 
            // modernTextBox1
            // 
            modernTextBox1.BackColor = Color.FromArgb(28, 28, 28);
            modernTextBox1.BorderStyle = BorderStyle.None;
            modernTextBox1.Font = new Font("Segoe UI", 10F);
            modernTextBox1.ForeColor = Color.White;
            modernTextBox1.Location = new Point(8, 232);
            modernTextBox1.MaxLength = 50;
            modernTextBox1.Name = "modernTextBox1";
            modernTextBox1.ReadOnly = true;
            modernTextBox1.Size = new Size(647, 18);
            modernTextBox1.TabIndex = 10;
            modernTextBox1.Text = "Choose TimeSpace to Load";
            // 
            // button9
            // 
            button9.AutoSize = false;
            button9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button9.BackColor = Color.FromArgb(28, 28, 28);
            button9.Density = MaterialButton.MaterialButtonDensity.Default;
            button9.Depth = 0;
            button9.Font = new Font("Segoe UI", 9F);
            button9.ForeColor = Color.White;
            button9.HighEmphasis = true;
            button9.Icon = null;
            button9.Location = new Point(662, 139);
            button9.Margin = new Padding(4, 6, 4, 6);
            button9.MouseState = MaterialSkin.MouseState.HOVER;
            button9.Name = "button9";
            button9.NoAccentTextColor = Color.Empty;
            button9.Size = new Size(75, 23);
            button9.TabIndex = 9;
            button9.Text = "Browse";
            button9.Type = MaterialButton.MaterialButtonType.Contained;
            button9.UseAccentColor = false;
            button9.UseVisualStyleBackColor = false;
            button9.Click += button9_Click;
            // 
            // textBox13
            // 
            textBox13.BackColor = Color.FromArgb(28, 28, 28);
            textBox13.BorderStyle = BorderStyle.None;
            textBox13.Font = new Font("Segoe UI", 10F);
            textBox13.ForeColor = Color.White;
            textBox13.Location = new Point(8, 144);
            textBox13.MaxLength = 50;
            textBox13.Name = "textBox13";
            textBox13.ReadOnly = true;
            textBox13.Size = new Size(647, 18);
            textBox13.TabIndex = 8;
            // 
            // button8
            // 
            button8.AutoSize = false;
            button8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button8.BackColor = Color.FromArgb(28, 28, 28);
            button8.Density = MaterialButton.MaterialButtonDensity.Default;
            button8.Depth = 0;
            button8.Font = new Font("Segoe UI", 9F);
            button8.ForeColor = Color.White;
            button8.HighEmphasis = true;
            button8.Icon = null;
            button8.Location = new Point(662, 95);
            button8.Margin = new Padding(4, 6, 4, 6);
            button8.MouseState = MaterialSkin.MouseState.HOVER;
            button8.Name = "button8";
            button8.NoAccentTextColor = Color.Empty;
            button8.Size = new Size(75, 23);
            button8.TabIndex = 7;
            button8.Text = "Browse";
            button8.Type = MaterialButton.MaterialButtonType.Contained;
            button8.UseAccentColor = false;
            button8.UseVisualStyleBackColor = false;
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.AutoSize = false;
            button7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button7.BackColor = Color.FromArgb(28, 28, 28);
            button7.Density = MaterialButton.MaterialButtonDensity.Default;
            button7.Depth = 0;
            button7.Font = new Font("Segoe UI", 9F);
            button7.ForeColor = Color.White;
            button7.HighEmphasis = true;
            button7.Icon = null;
            button7.Location = new Point(662, 51);
            button7.Margin = new Padding(4, 6, 4, 6);
            button7.MouseState = MaterialSkin.MouseState.HOVER;
            button7.Name = "button7";
            button7.NoAccentTextColor = Color.Empty;
            button7.Size = new Size(75, 23);
            button7.TabIndex = 6;
            button7.Text = "Browse";
            button7.Type = MaterialButton.MaterialButtonType.Contained;
            button7.UseAccentColor = false;
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // textBox12
            // 
            textBox12.BackColor = Color.FromArgb(28, 28, 28);
            textBox12.BorderStyle = BorderStyle.None;
            textBox12.Font = new Font("Segoe UI", 10F);
            textBox12.ForeColor = Color.White;
            textBox12.Location = new Point(8, 100);
            textBox12.MaxLength = 50;
            textBox12.Name = "textBox12";
            textBox12.ReadOnly = true;
            textBox12.Size = new Size(647, 18);
            textBox12.TabIndex = 5;
            // 
            // textBox11
            // 
            textBox11.BackColor = Color.FromArgb(28, 28, 28);
            textBox11.BorderStyle = BorderStyle.None;
            textBox11.Font = new Font("Segoe UI", 10F);
            textBox11.ForeColor = Color.White;
            textBox11.Location = new Point(8, 56);
            textBox11.MaxLength = 50;
            textBox11.Name = "textBox11";
            textBox11.ReadOnly = true;
            textBox11.Size = new Size(647, 18);
            textBox11.TabIndex = 4;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(28, 28, 28);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Segoe UI", 10F);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(8, 12);
            textBox1.MaxLength = 50;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(647, 18);
            textBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.AutoSize = false;
            button1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button1.BackColor = Color.FromArgb(28, 28, 28);
            button1.Density = MaterialButton.MaterialButtonDensity.Default;
            button1.Depth = 0;
            button1.Font = new Font("Segoe UI", 9F);
            button1.ForeColor = Color.White;
            button1.HighEmphasis = true;
            button1.Icon = null;
            button1.Location = new Point(662, 7);
            button1.Margin = new Padding(4, 6, 4, 6);
            button1.MouseState = MaterialSkin.MouseState.HOVER;
            button1.Name = "button1";
            button1.NoAccentTextColor = Color.Empty;
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Browse";
            button1.Type = MaterialButton.MaterialButtonType.Contained;
            button1.UseAccentColor = false;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // modernTextBox2
            // 
            modernTextBox2.BackColor = Color.FromArgb(28, 28, 28);
            modernTextBox2.BorderStyle = BorderStyle.None;
            modernTextBox2.Font = new Font("Segoe UI", 10F);
            modernTextBox2.ForeColor = Color.White;
            modernTextBox2.Location = new Point(8, 188);
            modernTextBox2.MaxLength = 50;
            modernTextBox2.Name = "modernTextBox2";
            modernTextBox2.ReadOnly = true;
            modernTextBox2.Size = new Size(647, 18);
            modernTextBox2.TabIndex = 10;
            // 
            // materialButton2
            // 
            materialButton2.AutoSize = false;
            materialButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton2.BackColor = Color.FromArgb(28, 28, 28);
            materialButton2.Density = MaterialButton.MaterialButtonDensity.Default;
            materialButton2.Depth = 0;
            materialButton2.Font = new Font("Segoe UI", 9F);
            materialButton2.ForeColor = Color.White;
            materialButton2.HighEmphasis = true;
            materialButton2.Icon = null;
            materialButton2.Location = new Point(662, 183);
            materialButton2.Margin = new Padding(4, 6, 4, 6);
            materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton2.Name = "materialButton2";
            materialButton2.NoAccentTextColor = Color.Empty;
            materialButton2.Size = new Size(75, 23);
            materialButton2.TabIndex = 11;
            materialButton2.Text = "Browse";
            materialButton2.Type = MaterialButton.MaterialButtonType.Contained;
            materialButton2.UseAccentColor = false;
            materialButton2.UseVisualStyleBackColor = false;
            materialButton2.Click += materialButton2_Click;
            // 
            // objectivePanel
            // 
            objectivesPanel.Margin = new Padding(16);
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.ForeColor = Color.White;
            tabControl1.ItemSize = new Size(75, 25);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(0, 0);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1831, 858);
            tabControl1.SizeMode = TabSizeMode.FillToRight;
            tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.AutoScroll = true;
            tabPage2.BackColor = Color.FromArgb(45, 45, 48);
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(textBox2);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(modernTextBox6);
            tabPage2.Controls.Add(modernTextBox4);
            tabPage2.Controls.Add(modernTextBox5);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(textBox4);
            tabPage2.Controls.Add(modernTextBox3);
            tabPage2.Controls.Add(pictureBox2);
            tabPage2.Controls.Add(textBox15);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(textBox9);
            tabPage2.Controls.Add(textBox14);
            tabPage2.Controls.Add(textBox10);
            tabPage2.Controls.Add(pictureBox5);
            tabPage2.Controls.Add(textBox8);
            tabPage2.Controls.Add(numericUpDown2);
            tabPage2.Controls.Add(textBox5);
            tabPage2.Controls.Add(textBox3);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(objectivesPanel);
            tabPage2.Controls.Add(isHeroCheckBox);
            tabPage2.Controls.Add(isSpecialCheckBox);
            tabPage2.Controls.Add(isHiddenCheckBox);
            tabPage2.Controls.Add(pictureBox3);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Controls.Add(pictureBox4);
            tabPage2.ForeColor = Color.White;
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1823, 825);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "TimeSpace Configuration";
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.Black;
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.ForeColor = Color.White;
            textBox2.Location = new Point(1020, 10);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(55, 37);
            textBox2.TabIndex = 110;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.FromArgb(23, 23, 25);
            label9.ForeColor = Color.White;
            label9.Location = new Point(704, 452);
            label9.Name = "label9";
            label9.Size = new Size(41, 15);
            label9.TabIndex = 109;
            label9.Text = "Map Y";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.FromArgb(23, 23, 25);
            label6.ForeColor = Color.White;
            label6.Location = new Point(704, 425);
            label6.Name = "label6";
            label6.Size = new Size(41, 15);
            label6.TabIndex = 108;
            label6.Text = "Map X";
            // 
            // modernTextBox6
            // 
            modernTextBox6.BackColor = Color.FromArgb(23, 23, 25);
            modernTextBox6.BorderStyle = BorderStyle.None;
            modernTextBox6.Font = new Font("Segoe UI", 10F);
            modernTextBox6.ForeColor = Color.White;
            modernTextBox6.Location = new Point(757, 394);
            modernTextBox6.MaxLength = 50;
            modernTextBox6.Name = "modernTextBox6";
            modernTextBox6.PlaceholderText = "ID";
            modernTextBox6.Size = new Size(63, 18);
            modernTextBox6.TabIndex = 107;
            // 
            // modernTextBox4
            // 
            modernTextBox4.BackColor = Color.FromArgb(23, 23, 25);
            modernTextBox4.BorderStyle = BorderStyle.None;
            modernTextBox4.Font = new Font("Segoe UI", 10F);
            modernTextBox4.ForeColor = Color.White;
            modernTextBox4.Location = new Point(757, 448);
            modernTextBox4.MaxLength = 50;
            modernTextBox4.Name = "modernTextBox4";
            modernTextBox4.PlaceholderText = "Y";
            modernTextBox4.Size = new Size(63, 18);
            modernTextBox4.TabIndex = 106;
            // 
            // modernTextBox5
            // 
            modernTextBox5.BackColor = Color.FromArgb(23, 23, 25);
            modernTextBox5.BorderStyle = BorderStyle.None;
            modernTextBox5.Font = new Font("Segoe UI", 10F);
            modernTextBox5.ForeColor = Color.White;
            modernTextBox5.Location = new Point(757, 421);
            modernTextBox5.MaxLength = 50;
            modernTextBox5.Name = "modernTextBox5";
            modernTextBox5.PlaceholderText = "X";
            modernTextBox5.Size = new Size(63, 18);
            modernTextBox5.TabIndex = 105;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(23, 23, 25);
            label5.ForeColor = Color.White;
            label5.Location = new Point(704, 396);
            label5.Name = "label5";
            label5.Size = new Size(45, 15);
            label5.TabIndex = 104;
            label5.Text = "Map ID";
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(103, 106, 106);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Segoe UI", 15F);
            textBox4.ForeColor = Color.Black;
            textBox4.Location = new Point(877, 350);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "Lives";
            textBox4.Size = new Size(32, 27);
            textBox4.TabIndex = 103;
            // 
            // modernTextBox3
            // 
            modernTextBox3.BackColor = Color.Black;
            modernTextBox3.BorderStyle = BorderStyle.None;
            modernTextBox3.Font = new Font("Segoe UI", 20F);
            modernTextBox3.ForeColor = Color.White;
            modernTextBox3.Location = new Point(375, 9);
            modernTextBox3.Multiline = true;
            modernTextBox3.Name = "modernTextBox3";
            modernTextBox3.PlaceholderText = "TS NAME";
            modernTextBox3.Size = new Size(577, 37);
            modernTextBox3.TabIndex = 48;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(3, 2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(356, 480);
            pictureBox2.TabIndex = 36;
            pictureBox2.TabStop = false;
            // 
            // textBox15
            // 
            textBox15.BackColor = Color.FromArgb(28, 28, 28);
            textBox15.BorderStyle = BorderStyle.None;
            textBox15.Font = new Font("Segoe UI", 10F);
            textBox15.ForeColor = Color.White;
            textBox15.Location = new Point(375, 52);
            textBox15.Multiline = true;
            textBox15.Name = "textBox15";
            textBox15.PlaceholderText = "TS DESCRIPTION";
            textBox15.Size = new Size(700, 281);
            textBox15.TabIndex = 45;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(103, 106, 106);
            label4.Font = new Font("Segoe UI", 15F);
            label4.ForeColor = Color.Black;
            label4.Location = new Point(649, 349);
            label4.Name = "label4";
            label4.Size = new Size(26, 28);
            label4.TabIndex = 44;
            label4.Text = "~";
            // 
            // textBox9
            // 
            textBox9.BackColor = Color.FromArgb(103, 106, 106);
            textBox9.BorderStyle = BorderStyle.None;
            textBox9.Font = new Font("Segoe UI", 15F);
            textBox9.ForeColor = Color.Black;
            textBox9.Location = new Point(676, 349);
            textBox9.Name = "textBox9";
            textBox9.PlaceholderText = "max";
            textBox9.Size = new Size(31, 27);
            textBox9.TabIndex = 43;
            // 
            // textBox14
            // 
            textBox14.BackColor = Color.FromArgb(103, 106, 106);
            textBox14.BorderStyle = BorderStyle.None;
            textBox14.Font = new Font("Segoe UI", 15F);
            textBox14.ForeColor = Color.Black;
            textBox14.Location = new Point(622, 349);
            textBox14.Name = "textBox14";
            textBox14.PlaceholderText = "min";
            textBox14.Size = new Size(31, 27);
            textBox14.TabIndex = 42;
            // 
            // textBox10
            // 
            textBox10.BackColor = Color.FromArgb(103, 106, 106);
            textBox10.BorderStyle = BorderStyle.None;
            textBox10.Font = new Font("Segoe UI", 15F);
            textBox10.ForeColor = Color.Black;
            textBox10.Location = new Point(1000, 349);
            textBox10.Name = "textBox10";
            textBox10.PlaceholderText = "Seeds";
            textBox10.Size = new Size(32, 27);
            textBox10.TabIndex = 41;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(375, 339);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(700, 54);
            pictureBox5.TabIndex = 39;
            pictureBox5.TabStop = false;
            // 
            // textBox8
            // 
            textBox8.BackColor = Color.FromArgb(23, 23, 25);
            textBox8.BorderStyle = BorderStyle.None;
            textBox8.Font = new Font("Segoe UI", 10F);
            textBox8.ForeColor = Color.White;
            textBox8.Location = new Point(550, 448);
            textBox8.MaxLength = 50;
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(72, 18);
            textBox8.TabIndex = 17;
            textBox8.Text = "5000";
            // 
            // numericUpDown2
            // 
            numericUpDown2.BackColor = Color.FromArgb(23, 23, 25);
            numericUpDown2.ForeColor = Color.White;
            numericUpDown2.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown2.Location = new Point(550, 421);
            numericUpDown2.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(72, 25);
            numericUpDown2.TabIndex = 18;
            numericUpDown2.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // textBox5
            // 
            textBox5.BackColor = Color.FromArgb(23, 23, 25);
            textBox5.BorderStyle = BorderStyle.None;
            textBox5.Font = new Font("Segoe UI", 10F);
            textBox5.ForeColor = Color.White;
            textBox5.Location = new Point(635, 394);
            textBox5.MaxLength = 50;
            textBox5.Name = "textBox5";
            textBox5.PlaceholderText = "Y";
            textBox5.Size = new Size(66, 18);
            textBox5.TabIndex = 5;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(23, 23, 25);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Segoe UI", 10F);
            textBox3.ForeColor = Color.White;
            textBox3.Location = new Point(552, 394);
            textBox3.MaxLength = 50;
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "X";
            textBox3.Size = new Size(59, 18);
            textBox3.TabIndex = 3;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.FromArgb(23, 23, 25);
            label8.ForeColor = Color.White;
            label8.Location = new Point(451, 450);
            label8.Name = "label8";
            label8.Size = new Size(93, 15);
            label8.TabIndex = 16;
            label8.Text = "BP Drop Chance";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.FromArgb(23, 23, 25);
            label7.ForeColor = Color.White;
            label7.Location = new Point(451, 423);
            label7.Name = "label7";
            label7.Size = new Size(53, 15);
            label7.TabIndex = 14;
            label7.Text = "Duration";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(6, 110);
            label3.Name = "label3";
            label3.Size = new Size(0, 15);
            label3.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(23, 23, 25);
            label2.ForeColor = Color.White;
            label2.Location = new Point(451, 397);
            label2.Name = "label2";
            label2.Size = new Size(73, 15);
            label2.TabIndex = 2;
            label2.Text = "Spawn Point";
            // 
            // isHeroCheckBox
            // 
            isHeroCheckBox.BackColor = Color.FromArgb(23, 23, 25);
            isHeroCheckBox.ForeColor = Color.White;
            isHeroCheckBox.Location = new Point(375, 396);
            isHeroCheckBox.Name = "isHeroCheckBox";
            isHeroCheckBox.Size = new Size(79, 20);
            isHeroCheckBox.TabIndex = 102;
            isHeroCheckBox.Text = "Is Heroic";
            isHeroCheckBox.UseVisualStyleBackColor = false;
            // 
            // isSpecialCheckBox
            // 
            isSpecialCheckBox.BackColor = Color.FromArgb(23, 23, 25);
            isSpecialCheckBox.ForeColor = Color.White;
            isSpecialCheckBox.Location = new Point(375, 421);
            isSpecialCheckBox.Name = "isSpecialCheckBox";
            isSpecialCheckBox.Size = new Size(79, 20);
            isSpecialCheckBox.TabIndex = 101;
            isSpecialCheckBox.Text = "Is Special";
            isSpecialCheckBox.UseVisualStyleBackColor = false;
            // 
            // isHiddenCheckBox
            // 
            isHiddenCheckBox.BackColor = Color.FromArgb(23, 23, 25);
            isHiddenCheckBox.ForeColor = Color.White;
            isHiddenCheckBox.Location = new Point(375, 448);
            isHiddenCheckBox.Name = "isHiddenCheckBox";
            isHiddenCheckBox.Size = new Size(79, 20);
            isHiddenCheckBox.TabIndex = 100;
            isHiddenCheckBox.Text = "Is Hidden";
            isHiddenCheckBox.UseVisualStyleBackColor = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(365, 2);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(720, 480);
            pictureBox3.TabIndex = 37;
            pictureBox3.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(3, 484);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1086, 120);
            pictureBox1.TabIndex = 46;
            pictureBox1.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(3, 607);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(1086, 120);
            pictureBox4.TabIndex = 47;
            pictureBox4.TabStop = false;
            // 
            // minPlayersLabel
            // 
            minPlayersLabel.ForeColor = Color.White;
            minPlayersLabel.Location = new Point(8, 402);
            minPlayersLabel.Name = "minPlayersLabel";
            minPlayersLabel.Size = new Size(100, 20);
            minPlayersLabel.TabIndex = 0;
            minPlayersLabel.Text = "Min Players";
            // 
            // maxPlayersLabel
            // 
            maxPlayersLabel.ForeColor = Color.White;
            maxPlayersLabel.Location = new Point(8, 432);
            maxPlayersLabel.Name = "maxPlayersLabel";
            maxPlayersLabel.Size = new Size(100, 20);
            maxPlayersLabel.TabIndex = 0;
            maxPlayersLabel.Text = "Max Players";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Black;
            label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(958, 11);
            label1.Name = "label1";
            label1.Size = new Size(67, 30);
            label1.TabIndex = 111;
            label1.Text = "TS ID:";
            // 
            // TimeSpaceTool
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1831, 858);
            Controls.Add(tabControl1);
            Name = "TimeSpaceTool";
            Text = "TimeSpaceTool";
            Load += Form1_Load;
            tabPage3.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            objectivesPanel.ResumeLayout(false);
            objectivesPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
        }

        #endregion
        public OpenFileDialog openFileDialog1;
        public TabPage tabPage3;
        public ModernTabControl tabControl2;
        public MaterialButton button6;
        public MaterialButton button5;
        public MaterialButton button4;
        public ModernTextBox textBox7;
        public TabPage tabPage1;
        public Label minPlayersLabel;
        public Label maxPlayersLabel;
        public Label IsHiddenLabel;
        public Label IsSpecialLabel;
        public Label IsHeroLabel;
        public MaterialButton button8;
        public MaterialButton button7;
        public ModernTextBox textBox12;
        public ModernTextBox textBox11;
        public ModernTextBox textBox1;
        public MaterialButton button1;
        public ModernTabControl tabControl1;
        public MaterialButton button9;
        public ModernTextBox textBox13;
        public Panel panelButtons;
        public Dictionary<string, (ModernTextBox, ModernTextBox)> objectiveValueControls;
        public MaterialButton materialButton1;
        public ModernTextBox modernTextBox1;
        public ModernTextBox modernTextBox2;
        public MaterialButton materialButton2;
        public ItemSlot pictureBox15;
        public ItemSlot pictureBox14;
        public ItemSlot pictureBox13;
        public ItemSlot pictureBox12;
        public TabPage tabPage2;
        public TextBox textBox4;
        public TextBox modernTextBox3;
        public PictureBox pictureBox2;
        public PictureBox pictureBox4;
        public PictureBox pictureBox1;
        public ModernTextBox textBox15;
        public Label label4;
        public TextBox textBox9;
        public TextBox textBox14;
        public TextBox textBox10;
        public PictureBox pictureBox5;
        public ModernTextBox textBox8;
        public ModernNumericUpDown numericUpDown2;
        public ModernTextBox textBox5;
        public ModernTextBox textBox3;
        public Label label8;
        public Label label7;
        public Label label3;
        public Label label2;
        public CollapsibleObjectivesPanel objectivesPanel;
        public Label headerLabel;
        public CheckBox isHeroCheckBox;
        public CheckBox isSpecialCheckBox;
        public CheckBox isHiddenCheckBox;
        public PictureBox pictureBox3;
        public ModernTextBox modernTextBox6;
        public ModernTextBox modernTextBox4;
        public ModernTextBox modernTextBox5;
        public Label label5;
        public TextBox textBox2;
        public Label label9;
        public Label label6;
        public ItemSlot DrawItemSlot4;
        public ItemSlot DrawItemSlot3;
        public ItemSlot DrawItemSlot2;
        public ItemSlot DrawItemSlot1;
        public ItemSlot SpecialItemSlot1;
        public ItemSlot DrawItemSlot5;
        public ItemSlot BonusItemSlot3;
        public ItemSlot BonusItemSlot2;
        public ItemSlot BonusItemSlot1;
        public ItemSlot SpecialItemSlot2;
        private Label label1;
    }
}