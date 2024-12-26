using System.Windows.Forms;
using System.Drawing;
using YamlDotNet.Core.Tokens;
using MaterialSkin.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            openFileDialog1 = new OpenFileDialog();
            tabPage3 = new TabPage();
            tabControl2 = new ModernTabControl();
            panelButtons = new Panel();
            button4 = new MaterialButton();
            button5 = new MaterialButton();
            button6 = new MaterialButton();
            tabPage2 = new TabPage();
            textBox10 = new MaterialTextBox();
            textBox9 = new MaterialTextBox();
            textBox8 = new MaterialTextBox();
            textBox7 = new MaterialTextBox();
            textBox6 = new MaterialTextBox();
            textBox4 = new MaterialTextBox();
            textBox5 = new MaterialTextBox();
            textBox3 = new MaterialTextBox();
            textBox2 = new MaterialTextBox();
            label10 = new Label();
            label9 = new Label();
            checkBox1 = new ModernCheckBox();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            comboBox1 = new CustomMaterialStyleComboBox();
            label4 = new Label();
            numericUpDown1 = new ModernNumericUpDown();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            tabPage1 = new TabPage();
            button9 = new MaterialButton();
            textBox13 = new MaterialTextBox();
            button8 = new MaterialButton();
            button7 = new MaterialButton();
            textBox12 = new MaterialTextBox();
            textBox11 = new MaterialTextBox();
            textBox1 = new MaterialTextBox();
            button1 = new MaterialButton();
            tabControl1 = new ModernTabControl();
            tabPage3.SuspendLayout();
            panelButtons.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
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
            tabPage3.Size = new Size(1817, 758);
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
            tabControl2.Size = new Size(1811, 692);
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
            panelButtons.Location = new Point(3, 695);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(1811, 60);
            panelButtons.TabIndex = 5;

            ConfigureButton(button4, "Remove", new Point(10, 10), button4_Click);
            ConfigureButton(button5, "Add", new Point(120, 10), button5_Click);
            ConfigureButton(button6, "Generate", new Point(230, 10), button6_Click);
            // 
            // tabPage2
            // 
            tabPage2.AutoScroll = true;
            tabPage2.BackColor = Color.FromArgb(45, 45, 48);
            tabPage2.Controls.Add(textBox10);
            tabPage2.Controls.Add(textBox9);
            tabPage2.Controls.Add(textBox8);
            tabPage2.Controls.Add(textBox7);
            tabPage2.Controls.Add(textBox6);
            tabPage2.Controls.Add(textBox4);
            tabPage2.Controls.Add(textBox5);
            tabPage2.Controls.Add(textBox3);
            tabPage2.Controls.Add(textBox2);
            tabPage2.Controls.Add(label10);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(checkBox1);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(comboBox1);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(numericUpDown1);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(label1);
            tabPage2.ForeColor = Color.White;
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1817, 758);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "TimeSpace Configuration";
            // 
            // textBox10
            // 
            textBox10.AnimateReadOnly = false;
            textBox10.BackColor = Color.FromArgb(28, 28, 28);
            textBox10.BorderStyle = BorderStyle.None;
            textBox10.Depth = 0;
            textBox10.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox10.ForeColor = Color.White;
            textBox10.LeadingIcon = null;
            textBox10.Location = new Point(251, 222);
            textBox10.MaxLength = 50;
            textBox10.MouseState = MaterialSkin.MouseState.OUT;
            textBox10.Multiline = false;
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(82, 36);
            textBox10.TabIndex = 24;
            textBox10.Text = "";
            textBox10.TrailingIcon = null;
            textBox10.UseTallSize = false;
            textBox10.Visible = false;
            // 
            // textBox9
            // 
            textBox9.AnimateReadOnly = false;
            textBox9.BackColor = Color.FromArgb(28, 28, 28);
            textBox9.BorderStyle = BorderStyle.None;
            textBox9.Depth = 0;
            textBox9.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox9.ForeColor = Color.White;
            textBox9.LeadingIcon = null;
            textBox9.Location = new Point(98, 222);
            textBox9.MaxLength = 50;
            textBox9.MouseState = MaterialSkin.MouseState.OUT;
            textBox9.Multiline = false;
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(82, 36);
            textBox9.TabIndex = 22;
            textBox9.Text = "";
            textBox9.TrailingIcon = null;
            textBox9.UseTallSize = false;
            textBox9.Visible = false;
            // 
            // textBox8
            // 
            textBox8.AnimateReadOnly = false;
            textBox8.BackColor = Color.FromArgb(28, 28, 28);
            textBox8.BorderStyle = BorderStyle.None;
            textBox8.Depth = 0;
            textBox8.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox8.ForeColor = Color.White;
            textBox8.LeadingIcon = null;
            textBox8.Location = new Point(123, 479);
            textBox8.MaxLength = 50;
            textBox8.MouseState = MaterialSkin.MouseState.OUT;
            textBox8.Multiline = false;
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(152, 36);
            textBox8.TabIndex = 17;
            textBox8.Text = "";
            textBox8.TrailingIcon = null;
            textBox8.UseTallSize = false;
            textBox8.TextChanged += textBox8_TextChanged;
            // 
            // textBox7
            // 
            textBox7.AnimateReadOnly = false;
            textBox7.BackColor = Color.FromArgb(28, 28, 28);
            textBox7.BorderStyle = BorderStyle.None;
            textBox7.Depth = 0;
            textBox7.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox7.ForeColor = Color.White;
            textBox7.LeadingIcon = null;
            textBox7.Location = new Point(123, 433);
            textBox7.MaxLength = 50;
            textBox7.MouseState = MaterialSkin.MouseState.OUT;
            textBox7.Multiline = false;
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(152, 36);
            textBox7.TabIndex = 15;
            textBox7.Text = "";
            textBox7.TrailingIcon = null;
            textBox7.UseTallSize = false;
            // 
            // textBox6
            // 
            textBox6.AnimateReadOnly = false;
            textBox6.BackColor = Color.FromArgb(28, 28, 28);
            textBox6.BorderStyle = BorderStyle.None;
            textBox6.Depth = 0;
            textBox6.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox6.ForeColor = Color.White;
            textBox6.LeadingIcon = null;
            textBox6.Location = new Point(123, 385);
            textBox6.MaxLength = 50;
            textBox6.MouseState = MaterialSkin.MouseState.OUT;
            textBox6.Multiline = false;
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(152, 36);
            textBox6.TabIndex = 13;
            textBox6.Text = "";
            textBox6.TrailingIcon = null;
            textBox6.UseTallSize = false;
            // 
            // textBox4
            // 
            textBox4.AnimateReadOnly = false;
            textBox4.BackColor = Color.FromArgb(28, 28, 28);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Depth = 0;
            textBox4.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox4.ForeColor = Color.White;
            textBox4.LeadingIcon = null;
            textBox4.Location = new Point(123, 344);
            textBox4.MaxLength = 50;
            textBox4.MouseState = MaterialSkin.MouseState.OUT;
            textBox4.Multiline = false;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(152, 36);
            textBox4.TabIndex = 11;
            textBox4.Text = "";
            textBox4.TrailingIcon = null;
            textBox4.UseTallSize = false;
            // 
            // textBox5
            // 
            textBox5.AnimateReadOnly = false;
            textBox5.BackColor = Color.FromArgb(28, 28, 28);
            textBox5.BorderStyle = BorderStyle.None;
            textBox5.Depth = 0;
            textBox5.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox5.ForeColor = Color.White;
            textBox5.LeadingIcon = null;
            textBox5.Location = new Point(196, 62);
            textBox5.MaxLength = 50;
            textBox5.MouseState = MaterialSkin.MouseState.OUT;
            textBox5.Multiline = false;
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(63, 36);
            textBox5.TabIndex = 5;
            textBox5.Text = "";
            textBox5.TrailingIcon = null;
            textBox5.UseTallSize = false;
            // 
            // textBox3
            // 
            textBox3.AnimateReadOnly = false;
            textBox3.BackColor = Color.FromArgb(28, 28, 28);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Depth = 0;
            textBox3.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox3.ForeColor = Color.White;
            textBox3.LeadingIcon = null;
            textBox3.Location = new Point(107, 62);
            textBox3.MaxLength = 50;
            textBox3.MouseState = MaterialSkin.MouseState.OUT;
            textBox3.Multiline = false;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(63, 36);
            textBox3.TabIndex = 3;
            textBox3.Text = "";
            textBox3.TrailingIcon = null;
            textBox3.UseTallSize = false;
            // 
            // textBox2
            // 
            textBox2.AnimateReadOnly = false;
            textBox2.BackColor = Color.FromArgb(28, 28, 28);
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Depth = 0;
            textBox2.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox2.ForeColor = Color.White;
            textBox2.LeadingIcon = null;
            textBox2.Location = new Point(107, 13);
            textBox2.MaxLength = 50;
            textBox2.MouseState = MaterialSkin.MouseState.OUT;
            textBox2.Multiline = false;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(152, 36);
            textBox2.TabIndex = 1;
            textBox2.Text = "";
            textBox2.TrailingIcon = null;
            textBox2.UseTallSize = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = Color.White;
            label10.Location = new Point(201, 225);
            label10.Name = "label10";
            label10.Size = new Size(10, 15);
            label10.TabIndex = 23;
            label10.Text = " ";
            label10.Visible = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = Color.White;
            label9.Location = new Point(6, 225);
            label9.MinimumSize = new Size(80, 0);
            label9.Name = "label9";
            label9.Size = new Size(80, 15);
            label9.TabIndex = 21;
            label9.Text = " ";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            label9.Visible = false;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.FromArgb(45, 45, 48);
            checkBox1.FlatStyle = FlatStyle.Flat;
            checkBox1.Font = new Font("Segoe UI", 9F);
            checkBox1.ForeColor = Color.White;
            checkBox1.Location = new Point(107, 185);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(88, 19);
            checkBox1.TabIndex = 20;
            checkBox1.Text = "Protect NPC";
            checkBox1.UseVisualStyleBackColor = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.White;
            label8.Location = new Point(11, 482);
            label8.Name = "label8";
            label8.Size = new Size(93, 15);
            label8.TabIndex = 16;
            label8.Text = "BP Drop Chance";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.White;
            label7.Location = new Point(11, 436);
            label7.Name = "label7";
            label7.Size = new Size(53, 15);
            label7.TabIndex = 14;
            label7.Text = "Duration";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.White;
            label6.Location = new Point(11, 393);
            label6.Name = "label6";
            label6.Size = new Size(82, 15);
            label6.TabIndex = 12;
            label6.Text = "TS Description";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.White;
            label5.Location = new Point(9, 347);
            label5.Name = "label5";
            label5.Size = new Size(54, 15);
            label5.TabIndex = 10;
            label5.Text = "TS Name";
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.FromArgb(30, 30, 30);
            comboBox1.ComboboxHeight = 30;
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.ForeColor = Color.White;
            comboBox1.FormattingEnabled = true;
            comboBox1.ItemHeight = 20;
            comboBox1.Items.AddRange(new object[] { "KillAllMonster", "KillMonsterVnum", "CollectItemVnum", "InteractObjectsVnum", "Conversation", "GoToExit" });
            comboBox1.Location = new Point(107, 140);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(152, 26);
            comboBox1.TabIndex = 9;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.White;
            label4.Location = new Point(8, 151);
            label4.Name = "label4";
            label4.Size = new Size(57, 15);
            label4.TabIndex = 8;
            label4.Text = "Objective";
            // 
            // numericUpDown1
            // 
            numericUpDown1.BackColor = Color.FromArgb(30, 30, 30);
            numericUpDown1.ForeColor = Color.White;
            numericUpDown1.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Location = new Point(107, 102);
            numericUpDown1.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(152, 23);
            numericUpDown1.TabIndex = 25;
            numericUpDown1.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(6, 110);
            label3.Name = "label3";
            label3.Size = new Size(33, 15);
            label3.TabIndex = 6;
            label3.Text = "Lives";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(6, 65);
            label2.Name = "label2";
            label2.Size = new Size(73, 15);
            label2.TabIndex = 2;
            label2.Text = "Spawn Point";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.White;
            label1.Location = new Point(6, 16);
            label1.Name = "label1";
            label1.Size = new Size(77, 15);
            label1.TabIndex = 0;
            label1.Text = "Timespace ID";
            // 
            // tabPage1
            // 
            tabPage1.AutoScroll = true;
            tabPage1.BackColor = Color.FromArgb(45, 45, 45);
            tabPage1.Controls.Add(button9);
            tabPage1.Controls.Add(textBox13);
            tabPage1.Controls.Add(button8);
            tabPage1.Controls.Add(button7);
            tabPage1.Controls.Add(textBox12);
            tabPage1.Controls.Add(textBox11);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(button1);
            tabPage1.ForeColor = Color.White;
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1817, 758);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Settings";
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
            button9.Location = new Point(662, 174);
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
            textBox13.AnimateReadOnly = false;
            textBox13.BackColor = Color.FromArgb(28, 28, 28);
            textBox13.BorderStyle = BorderStyle.None;
            textBox13.Depth = 0;
            textBox13.Font = new Font("Microsoft Sans Serif", 12F);
            textBox13.ForeColor = Color.White;
            textBox13.LeadingIcon = null;
            textBox13.Location = new Point(8, 168);
            textBox13.MaxLength = 50;
            textBox13.MouseState = MaterialSkin.MouseState.OUT;
            textBox13.Multiline = false;
            textBox13.Name = "textBox13";
            textBox13.ReadOnly = true;
            textBox13.Size = new Size(647, 36);
            textBox13.TabIndex = 8;
            textBox13.Text = "Choose TimeSpace to Load";
            textBox13.TrailingIcon = null;
            textBox13.UseTallSize = false;
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
            button8.Location = new Point(662, 117);
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
            button7.Location = new Point(662, 63);
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
            textBox12.AnimateReadOnly = false;
            textBox12.BackColor = Color.FromArgb(28, 28, 28);
            textBox12.BorderStyle = BorderStyle.None;
            textBox12.Depth = 0;
            textBox12.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox12.ForeColor = Color.White;
            textBox12.LeadingIcon = null;
            textBox12.Location = new Point(8, 110);
            textBox12.MaxLength = 50;
            textBox12.MouseState = MaterialSkin.MouseState.OUT;
            textBox12.Multiline = false;
            textBox12.Name = "textBox12";
            textBox12.ReadOnly = true;
            textBox12.Size = new Size(647, 36);
            textBox12.TabIndex = 5;
            textBox12.Text = "";
            textBox12.TrailingIcon = null;
            textBox12.UseTallSize = false;
            // 
            // textBox11
            // 
            textBox11.AnimateReadOnly = false;
            textBox11.BackColor = Color.FromArgb(28, 28, 28);
            textBox11.BorderStyle = BorderStyle.None;
            textBox11.Depth = 0;
            textBox11.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox11.ForeColor = Color.White;
            textBox11.LeadingIcon = null;
            textBox11.Location = new Point(8, 56);
            textBox11.MaxLength = 50;
            textBox11.MouseState = MaterialSkin.MouseState.OUT;
            textBox11.Multiline = false;
            textBox11.Name = "textBox11";
            textBox11.ReadOnly = true;
            textBox11.Size = new Size(647, 36);
            textBox11.TabIndex = 4;
            textBox11.Text = "";
            textBox11.TrailingIcon = null;
            textBox11.UseTallSize = false;
            // 
            // textBox1
            // 
            textBox1.AnimateReadOnly = false;
            textBox1.BackColor = Color.FromArgb(28, 28, 28);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Depth = 0;
            textBox1.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            textBox1.ForeColor = Color.White;
            textBox1.LeadingIcon = null;
            textBox1.Location = new Point(8, 6);
            textBox1.MaxLength = 50;
            textBox1.MouseState = MaterialSkin.MouseState.OUT;
            textBox1.Multiline = false;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(647, 36);
            textBox1.TabIndex = 2;
            textBox1.Text = "";
            textBox1.TrailingIcon = null;
            textBox1.UseTallSize = false;
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
            button1.Location = new Point(662, 10);
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
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.ForeColor = Color.White;
            tabControl1.ItemSize = new Size(75, 25);
            tabControl1.Location = new Point(3, 64);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(0, 0);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1825, 791);
            tabControl1.SizeMode = TabSizeMode.FillToRight;
            tabControl1.TabIndex = 0;
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
            panelButtons.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void ConfigureButton(MaterialButton button, string text, Point location, EventHandler onClick)
        {
            button.AutoSize = false;
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button.BackColor = Color.FromArgb(28, 28, 28);
            button.Density = MaterialButton.MaterialButtonDensity.Default;
            button.Depth = 0;
            button.Font = new Font("Segoe UI", 9F);
            button.ForeColor = Color.White;
            button.HighEmphasis = true;
            button.Icon = null;
            button.Location = location;
            button.Margin = new Padding(4, 6, 4, 6);
            button.MouseState = MaterialSkin.MouseState.HOVER;
            button.Name = text.Replace(" ", "").ToLower() + "Button";
            button.NoAccentTextColor = Color.Empty;
            button.Size = new Size(100, 50); // Adjust size as needed  
            button.TabIndex = 1;
            button.Text = text;
            button.Type = MaterialButton.MaterialButtonType.Contained;
            button.UseAccentColor = false;
            button.UseVisualStyleBackColor = false;
            button.Click += onClick;
        }
        #endregion
        private OpenFileDialog openFileDialog1;
        private TabPage tabPage3;
        public ModernTabControl tabControl2;
        private MaterialButton button6;
        private MaterialButton button5;
        private MaterialButton button4;
        private TabPage tabPage2;
        private MaterialTextBox textBox10;
        private MaterialTextBox textBox9;
        private MaterialTextBox textBox8;
        private MaterialTextBox textBox7;
        private MaterialTextBox textBox6;
        private MaterialTextBox textBox4;
        private MaterialTextBox textBox5;
        private MaterialTextBox textBox3;
        private MaterialTextBox textBox2;
        private Label label10;
        private Label label9;
        private ModernCheckBox checkBox1;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private CustomMaterialStyleComboBox comboBox1;
        private Label label4;
        private ModernNumericUpDown numericUpDown1;
        private Label label3;
        private Label label2;
        private Label label1;
        private TabPage tabPage1;
        private MaterialButton button8;
        private MaterialButton button7;
        private MaterialTextBox textBox12;
        private MaterialTextBox textBox11;
        private MaterialTextBox textBox1;
        private MaterialButton button1;
        private ModernTabControl tabControl1;
        private MaterialButton button9;
        private MaterialTextBox textBox13;
        private Panel panelButtons;
    }
}
