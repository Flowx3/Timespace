﻿using System.Windows.Forms;
using System.Drawing;
using YamlDotNet.Core.Tokens;

namespace TimeSpace
{
    partial class Form1
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
            button6 = new ModernButton();
            button5 = new ModernButton();
            button4 = new ModernButton();
            tabPage2 = new TabPage();
            textBox10 = new ModernTextBox();
            textBox9 = new ModernTextBox();
            textBox8 = new ModernTextBox();
            textBox7 = new ModernTextBox();
            textBox6 = new ModernTextBox();
            textBox4 = new ModernTextBox();
            textBox5 = new ModernTextBox();
            textBox3 = new ModernTextBox();
            textBox2 = new ModernTextBox();
            label10 = new Label();
            label9 = new Label();
            checkBox1 = new ModernCheckBox();
            button3 = new ModernButton();
            button2 = new ModernButton();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            comboBox1 = new SearchableComboBox();
            label4 = new Label();
            numericUpDown1 = new NumericUpDown();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            tabPage1 = new TabPage();
            button9 = new ModernButton();
            textBox13 = new ModernTextBox();
            button8 = new ModernButton();
            button7 = new ModernButton();
            textBox12 = new ModernTextBox();
            textBox11 = new ModernTextBox();
            textBox1 = new ModernTextBox();
            button1 = new ModernButton();
            tabControl1 = new ModernTabControl();
            tabPage3.SuspendLayout();
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
            tabPage3.BackColor = Color.FromArgb(45, 45, 48);
            tabPage3.Controls.Add(tabControl2);
            tabPage3.Controls.Add(button6);
            tabPage3.Controls.Add(button5);
            tabPage3.Controls.Add(button4);
            tabPage3.ForeColor = Color.White;
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1823, 830);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "TimeSpace Editor";
            // 
            // tabControl2
            // 
            tabControl2.ForeColor = Color.White;
            tabControl2.Location = new Point(6, 3);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(1811, 1000);
            tabControl2.TabIndex = 4;
            tabControl2.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button6.BackColor = Color.FromArgb(28, 28, 28);
            button6.ForeColor = Color.White;
            button6.Location = new Point(139, 755);
            button6.Name = "button6";
            button6.Size = new Size(66, 67);
            button6.TabIndex = 3;
            button6.Text = "Generate";
            button6.Click += button6_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button5.BackColor = Color.FromArgb(28, 28, 28);
            button5.ForeColor = Color.White;
            button5.Location = new Point(71, 755);
            button5.Name = "button5";
            button5.Size = new Size(62, 67);
            button5.TabIndex = 2;
            button5.Text = "Add Map";
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.BackColor = Color.FromArgb(28, 28, 28);
            button4.ForeColor = Color.White;
            button4.Location = new Point(3, 755);
            button4.Name = "button4";
            button4.Size = new Size(62, 67);
            button4.TabIndex = 1;
            button4.Text = "Remove Map";
            button4.Click += button4_Click;
            // 
            // tabPage2
            // 
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
            tabPage2.Controls.Add(button3);
            tabPage2.Controls.Add(button2);
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
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1823, 830);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "TimeSpace Configuration";
            // 
            // textBox10
            // 
            textBox10.BackColor = Color.FromArgb(30, 30, 30);
            textBox10.ForeColor = Color.White;
            textBox10.Location = new Point(251, 222);
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(82, 23);
            textBox10.TabIndex = 24;
            textBox10.Visible = false;
            // 
            // textBox9
            // 
            textBox9.BackColor = Color.FromArgb(30, 30, 30);
            textBox9.ForeColor = Color.White;
            textBox9.Location = new Point(98, 222);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(82, 23);
            textBox9.TabIndex = 22;
            textBox9.Visible = false;
            // 
            // textBox8
            // 
            textBox8.BackColor = Color.FromArgb(30, 30, 30);
            textBox8.ForeColor = Color.White;
            textBox8.Location = new Point(113, 479);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(152, 23);
            textBox8.TabIndex = 17;
            // 
            // textBox7
            // 
            textBox7.BackColor = Color.FromArgb(30, 30, 30);
            textBox7.ForeColor = Color.White;
            textBox7.Location = new Point(113, 433);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(152, 23);
            textBox7.TabIndex = 15;
            // 
            // textBox6
            // 
            textBox6.BackColor = Color.FromArgb(30, 30, 30);
            textBox6.ForeColor = Color.White;
            textBox6.Location = new Point(113, 385);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(152, 23);
            textBox6.TabIndex = 13;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(30, 30, 30);
            textBox4.ForeColor = Color.White;
            textBox4.Location = new Point(113, 344);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(152, 23);
            textBox4.TabIndex = 11;
            // 
            // textBox5
            // 
            textBox5.BackColor = Color.FromArgb(30, 30, 30);
            textBox5.ForeColor = Color.White;
            textBox5.Location = new Point(178, 62);
            textBox5.Name = "textBox5";
            textBox5.PlaceholderText = "Y";
            textBox5.Size = new Size(63, 23);
            textBox5.TabIndex = 5;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(30, 30, 30);
            textBox3.ForeColor = Color.White;
            textBox3.Location = new Point(89, 62);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "X";
            textBox3.Size = new Size(63, 23);
            textBox3.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(30, 30, 30);
            textBox2.ForeColor = Color.White;
            textBox2.Location = new Point(89, 13);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(152, 23);
            textBox2.TabIndex = 1;
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
            checkBox1.ForeColor = Color.White;
            checkBox1.Location = new Point(88, 185);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(91, 19);
            checkBox1.TabIndex = 20;
            checkBox1.Text = "Protect NPC";
            checkBox1.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(28, 28, 28);
            button3.ForeColor = Color.White;
            button3.Location = new Point(181, 537);
            button3.Name = "button3";
            button3.Size = new Size(75, 24);
            button3.TabIndex = 19;
            button3.Text = "Generate";
            button3.Click += button6_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(28, 28, 28);
            button2.ForeColor = Color.White;
            button2.Location = new Point(14, 537);
            button2.Name = "button2";
            button2.Size = new Size(75, 24);
            button2.TabIndex = 18;
            button2.Text = "Save TS";
            button2.Click += button2_Click;
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
            comboBox1.ForeColor = Color.White;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "KillAllMonster", "KillMonsterVnum", "CollectItemVnum", "InteractObjectsVnum", "Conversation", "GoToExit" });
            comboBox1.Location = new Point(89, 143);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(152, 23);
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
            numericUpDown1.Location = new Point(89, 102);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(152, 23);
            numericUpDown1.TabIndex = 7;
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
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1823, 830);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Settings";
            // 
            // button9
            // 
            button9.BackColor = Color.FromArgb(28, 28, 28);
            button9.ForeColor = Color.White;
            button9.Location = new Point(532, 102);
            button9.Name = "button9";
            button9.Size = new Size(75, 23);
            button9.TabIndex = 9;
            button9.Text = "Browse";
            button9.Click += button9_Click;
            // 
            // textBox13
            // 
            textBox13.BackColor = Color.FromArgb(30, 30, 30);
            textBox13.ForeColor = Color.White;
            textBox13.Location = new Point(8, 102);
            textBox13.Name = "textBox13";
            textBox13.Size = new Size(512, 23);
            textBox13.TabIndex = 8;
            textBox13.Text = "Choose TimeSpace to Load";
            // 
            // button8
            // 
            button8.BackColor = Color.FromArgb(28, 28, 28);
            button8.ForeColor = Color.White;
            button8.Location = new Point(532, 68);
            button8.Name = "button8";
            button8.Size = new Size(75, 23);
            button8.TabIndex = 7;
            button8.Text = "Browse";
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.BackColor = Color.FromArgb(28, 28, 28);
            button7.ForeColor = Color.White;
            button7.Location = new Point(532, 39);
            button7.Name = "button7";
            button7.Size = new Size(75, 23);
            button7.TabIndex = 6;
            button7.Text = "Browse";
            button7.Click += button7_Click;
            // 
            // textBox12
            // 
            textBox12.BackColor = Color.FromArgb(30, 30, 30);
            textBox12.ForeColor = Color.White;
            textBox12.Location = new Point(8, 64);
            textBox12.Name = "textBox12";
            textBox12.Size = new Size(512, 23);
            textBox12.TabIndex = 5;
            // 
            // textBox11
            // 
            textBox11.BackColor = Color.FromArgb(30, 30, 30);
            textBox11.ForeColor = Color.White;
            textBox11.Location = new Point(8, 35);
            textBox11.Name = "textBox11";
            textBox11.Size = new Size(512, 23);
            textBox11.TabIndex = 4;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(30, 30, 30);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(8, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(512, 23);
            textBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(28, 28, 28);
            button1.ForeColor = Color.White;
            button1.Location = new Point(532, 10);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Browse";
            button1.Click += button1_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.ForeColor = Color.White;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1831, 858);
            tabControl1.SizeMode = TabSizeMode.FillToRight;
            tabControl1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1831, 858);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Resize += Form1_Resize;
            tabPage3.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private OpenFileDialog openFileDialog1;
        private TabPage tabPage3;
        public ModernTabControl tabControl2;
        private ModernButton button6;
        private ModernButton button5;
        private ModernButton button4;
        private TabPage tabPage2;
        private ModernTextBox textBox10;
        private ModernTextBox textBox9;
        private ModernTextBox textBox8;
        private ModernTextBox textBox7;
        private ModernTextBox textBox6;
        private ModernTextBox textBox4;
        private ModernTextBox textBox5;
        private ModernTextBox textBox3;
        private ModernTextBox textBox2;
        private Label label10;
        private Label label9;
        private ModernCheckBox checkBox1;
        private ModernButton button3;
        private ModernButton button2;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private SearchableComboBox comboBox1;
        private Label label4;
        private NumericUpDown numericUpDown1;
        private Label label3;
        private Label label2;
        private Label label1;
        private TabPage tabPage1;
        private ModernButton button8;
        private ModernButton button7;
        private ModernTextBox textBox12;
        private ModernTextBox textBox11;
        private ModernTextBox textBox1;
        private ModernButton button1;
        private ModernTabControl tabControl1;
        private ModernButton button9;
        private ModernTextBox textBox13;
    }
}
