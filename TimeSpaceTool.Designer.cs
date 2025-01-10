using MaterialSkin.Controls;
using System.Windows.Forms;

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
        //private void InitializeItemSearch()
        //{
        //    itemSearchManager = new ItemSearchManager();
        //    itemSearchManager.LoadItems(
        //        config.GameDataPath,
        //        config.GameTranslationPath,
        //        Path.Combine(Application.StartupPath, "icons")
        //    );

        //    // Modify the itemVnumNumeric to be read-only and add a search button
        //    var searchButton = new MaterialButton
        //    {
        //        Text = "Search",
        //        Location = new Point(itemVnumNumeric.Right + 5, itemVnumNumeric.Top),
        //        Size = new Size(60, itemVnumNumeric.Height),
        //        UseAccentColor = false
        //    };

        //    searchButton.Click += (s, e) =>
        //    {
        //        var dialog = new ItemSearchDialog(itemSearchManager, (vnum, amount) =>
        //        {
        //            itemVnumNumeric.Value = vnum;
        //            itemAmountNumeric.Value = amount;
        //        });
        //        dialog.ShowDialog(this);
        //    };

        //    rewardsPanel.Controls.Add(searchButton);
        //}
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
            tabPage2 = new TabPage();
            pictureBox2 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox1 = new PictureBox();
            textBox15 = new ModernTextBox();
            label4 = new Label();
            textBox9 = new ModernTextBox();
            textBox14 = new ModernTextBox();
            textBox10 = new ModernTextBox();
            pictureBox5 = new PictureBox();
            pictureBox3 = new PictureBox();
            textBox8 = new ModernTextBox();
            numericUpDown2 = new ModernNumericUpDown();
            textBox6 = new ModernTextBox();
            textBox4 = new ModernTextBox();
            textBox5 = new ModernTextBox();
            textBox3 = new ModernTextBox();
            textBox2 = new ModernTextBox();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            numericUpDown1 = new ModernNumericUpDown();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            objectivesPanel = new Panel();
            objectiveTypeComboBox = new MaterialComboBox();
            objectiveValuesPanel = new Panel();
            selectedObjectivesListBox = new ListBox();
            addObjectiveButton = new MaterialButton();
            removeObjectiveButton = new MaterialButton();
            headerLabel = new Label();
            minLevelNumeric = new ModernNumericUpDown();
            maxLevelNumeric = new ModernNumericUpDown();
            seedsRequiredNumeric = new ModernNumericUpDown();
            minPlayersNumeric = new ModernNumericUpDown();
            maxPlayersNumeric = new ModernNumericUpDown();
            isHeroCheckBox = new CheckBox();
            isSpecialCheckBox = new CheckBox();
            isHiddenCheckBox = new CheckBox();
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
            minPlayersLabel = new Label();
            maxPlayersLabel = new Label();
            pictureBox6 = new ItemSlot(config);
            tabPage3.SuspendLayout();
            panelButtons.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            objectivesPanel.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
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
            // tabPage2
            // 
            tabPage2.AutoScroll = true;
            tabPage2.BackColor = Color.FromArgb(45, 45, 48);
            tabPage2.Controls.Add(pictureBox6);
            tabPage2.Controls.Add(pictureBox2);
            tabPage2.Controls.Add(pictureBox4);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Controls.Add(textBox15);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(textBox9);
            tabPage2.Controls.Add(textBox14);
            tabPage2.Controls.Add(textBox10);
            tabPage2.Controls.Add(pictureBox5);
            tabPage2.Controls.Add(pictureBox3);
            tabPage2.Controls.Add(textBox8);
            tabPage2.Controls.Add(numericUpDown2);
            tabPage2.Controls.Add(textBox6);
            tabPage2.Controls.Add(textBox4);
            tabPage2.Controls.Add(textBox5);
            tabPage2.Controls.Add(textBox3);
            tabPage2.Controls.Add(textBox2);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(numericUpDown1);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(objectivesPanel);
            tabPage2.Controls.Add(minLevelNumeric);
            tabPage2.Controls.Add(maxLevelNumeric);
            tabPage2.Controls.Add(seedsRequiredNumeric);
            tabPage2.Controls.Add(minPlayersNumeric);
            tabPage2.Controls.Add(maxPlayersNumeric);
            tabPage2.Controls.Add(isHeroCheckBox);
            tabPage2.Controls.Add(isSpecialCheckBox);
            tabPage2.Controls.Add(isHiddenCheckBox);
            tabPage2.ForeColor = Color.White;
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1823, 825);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "TimeSpace Configuration";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(648, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(356, 480);
            pictureBox2.TabIndex = 36;
            pictureBox2.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(648, 608);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(1086, 120);
            pictureBox4.TabIndex = 47;
            pictureBox4.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(648, 485);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1086, 120);
            pictureBox1.TabIndex = 46;
            pictureBox1.TabStop = false;
            // 
            // textBox15
            // 
            textBox15.BackColor = Color.FromArgb(28, 28, 28);
            textBox15.BorderStyle = BorderStyle.None;
            textBox15.Font = new Font("Segoe UI", 10F);
            textBox15.ForeColor = Color.White;
            textBox15.Location = new Point(1020, 53);
            textBox15.Multiline = true;
            textBox15.Name = "textBox15";
            textBox15.Size = new Size(700, 281);
            textBox15.TabIndex = 45;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(103, 106, 106);
            label4.ForeColor = Color.Black;
            label4.Location = new Point(1304, 358);
            label4.Name = "label4";
            label4.Size = new Size(15, 15);
            label4.TabIndex = 44;
            label4.Text = "~";
            // 
            // textBox9
            // 
            textBox9.BackColor = Color.FromArgb(28, 28, 28);
            textBox9.BorderStyle = BorderStyle.FixedSingle;
            textBox9.Font = new Font("Segoe UI", 10F);
            textBox9.ForeColor = Color.White;
            textBox9.Location = new Point(1321, 354);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(31, 25);
            textBox9.TabIndex = 43;
            // 
            // textBox14
            // 
            textBox14.BackColor = Color.FromArgb(28, 28, 28);
            textBox14.BorderStyle = BorderStyle.FixedSingle;
            textBox14.Font = new Font("Segoe UI", 10F);
            textBox14.ForeColor = Color.White;
            textBox14.Location = new Point(1267, 354);
            textBox14.Name = "textBox14";
            textBox14.Size = new Size(31, 25);
            textBox14.TabIndex = 42;
            // 
            // textBox10
            // 
            textBox10.BackColor = Color.FromArgb(28, 28, 28);
            textBox10.BorderStyle = BorderStyle.FixedSingle;
            textBox10.Font = new Font("Segoe UI", 10F);
            textBox10.ForeColor = Color.White;
            textBox10.Location = new Point(1652, 354);
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(32, 25);
            textBox10.TabIndex = 41;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(1020, 340);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(700, 54);
            pictureBox5.TabIndex = 39;
            pictureBox5.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(1010, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(720, 480);
            pictureBox3.TabIndex = 37;
            pictureBox3.TabStop = false;
            // 
            // textBox8
            // 
            textBox8.BackColor = Color.FromArgb(28, 28, 28);
            textBox8.BorderStyle = BorderStyle.None;
            textBox8.Font = new Font("Segoe UI", 10F);
            textBox8.ForeColor = Color.White;
            textBox8.Location = new Point(128, 269);
            textBox8.MaxLength = 50;
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(152, 18);
            textBox8.TabIndex = 17;
            // 
            // numericUpDown2
            // 
            numericUpDown2.BackColor = Color.FromArgb(28, 28, 28);
            numericUpDown2.ForeColor = Color.White;
            numericUpDown2.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDown2.Location = new Point(128, 227);
            numericUpDown2.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(152, 25);
            numericUpDown2.TabIndex = 18;
            numericUpDown2.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // textBox6
            // 
            textBox6.BackColor = Color.FromArgb(28, 28, 28);
            textBox6.BorderStyle = BorderStyle.None;
            textBox6.Font = new Font("Segoe UI", 10F);
            textBox6.ForeColor = Color.White;
            textBox6.Location = new Point(128, 189);
            textBox6.MaxLength = 50;
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(152, 18);
            textBox6.TabIndex = 13;
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(28, 28, 28);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Segoe UI", 10F);
            textBox4.ForeColor = Color.White;
            textBox4.Location = new Point(128, 148);
            textBox4.MaxLength = 50;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(152, 18);
            textBox4.TabIndex = 11;
            // 
            // textBox5
            // 
            textBox5.BackColor = Color.FromArgb(28, 28, 28);
            textBox5.BorderStyle = BorderStyle.None;
            textBox5.Font = new Font("Segoe UI", 10F);
            textBox5.ForeColor = Color.White;
            textBox5.Location = new Point(217, 60);
            textBox5.MaxLength = 50;
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(63, 18);
            textBox5.TabIndex = 5;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(28, 28, 28);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Segoe UI", 10F);
            textBox3.ForeColor = Color.White;
            textBox3.Location = new Point(128, 60);
            textBox3.MaxLength = 50;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(63, 18);
            textBox3.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(28, 28, 28);
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Segoe UI", 10F);
            textBox2.ForeColor = Color.White;
            textBox2.Location = new Point(128, 11);
            textBox2.MaxLength = 50;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(152, 18);
            textBox2.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.White;
            label8.Location = new Point(8, 271);
            label8.Name = "label8";
            label8.Size = new Size(93, 15);
            label8.TabIndex = 16;
            label8.Text = "BP Drop Chance";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.White;
            label7.Location = new Point(7, 229);
            label7.Name = "label7";
            label7.Size = new Size(53, 15);
            label7.TabIndex = 14;
            label7.Text = "Duration";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.White;
            label6.Location = new Point(6, 191);
            label6.Name = "label6";
            label6.Size = new Size(82, 15);
            label6.TabIndex = 12;
            label6.Text = "TS Description";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.White;
            label5.Location = new Point(6, 150);
            label5.Name = "label5";
            label5.Size = new Size(54, 15);
            label5.TabIndex = 10;
            label5.Text = "TS Name";
            // 
            // numericUpDown1
            // 
            numericUpDown1.BackColor = Color.FromArgb(30, 30, 30);
            numericUpDown1.ForeColor = Color.White;
            numericUpDown1.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Location = new Point(128, 100);
            numericUpDown1.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(152, 25);
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
            // objectivesPanel
            // 
            objectivesPanel.BackColor = Color.FromArgb(45, 45, 48);
            objectivesPanel.Controls.Add(objectiveTypeComboBox);
            objectivesPanel.Controls.Add(objectiveValuesPanel);
            objectivesPanel.Controls.Add(selectedObjectivesListBox);
            objectivesPanel.Controls.Add(addObjectiveButton);
            objectivesPanel.Controls.Add(removeObjectiveButton);
            objectivesPanel.Controls.Add(headerLabel);
            objectivesPanel.Location = new Point(300, 13);
            objectivesPanel.Name = "objectivesPanel";
            objectivesPanel.Size = new Size(300, 400);
            objectivesPanel.TabIndex = 26;
            // 
            // objectiveTypeComboBox
            // 
            objectiveTypeComboBox.AutoResize = false;
            objectiveTypeComboBox.BackColor = Color.FromArgb(28, 28, 28);
            objectiveTypeComboBox.Depth = 0;
            objectiveTypeComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            objectiveTypeComboBox.DropDownHeight = 118;
            objectiveTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            objectiveTypeComboBox.DropDownWidth = 121;
            objectiveTypeComboBox.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            objectiveTypeComboBox.ForeColor = Color.White;
            objectiveTypeComboBox.IntegralHeight = false;
            objectiveTypeComboBox.ItemHeight = 29;
            objectiveTypeComboBox.Items.AddRange(new object[] { "KillAllMonsters", "GoToExit", "KillMonsterVnum", "CollectItem", "Conversation", "InteractObjects", "ProtectNPC" });
            objectiveTypeComboBox.Location = new Point(0, 32);
            objectiveTypeComboBox.MaxDropDownItems = 4;
            objectiveTypeComboBox.MouseState = MaterialSkin.MouseState.OUT;
            objectiveTypeComboBox.Name = "objectiveTypeComboBox";
            objectiveTypeComboBox.Size = new Size(180, 35);
            objectiveTypeComboBox.StartIndex = 0;
            objectiveTypeComboBox.TabIndex = 0;
            objectiveTypeComboBox.UseTallSize = false;
            objectiveTypeComboBox.SelectedIndexChanged += ObjectiveTypeComboBox_SelectedIndexChanged;
            // 
            // objectiveValuesPanel
            // 
            objectiveValuesPanel.BackColor = Color.FromArgb(45, 45, 48);
            objectiveValuesPanel.Location = new Point(0, 65);
            objectiveValuesPanel.Name = "objectiveValuesPanel";
            objectiveValuesPanel.Size = new Size(300, 80);
            objectiveValuesPanel.TabIndex = 1;
            // 
            // selectedObjectivesListBox
            // 
            selectedObjectivesListBox.BackColor = Color.FromArgb(28, 28, 28);
            selectedObjectivesListBox.ItemHeight = 15;
            selectedObjectivesListBox.Location = new Point(0, 155);
            selectedObjectivesListBox.Name = "selectedObjectivesListBox";
            selectedObjectivesListBox.Size = new Size(300, 184);
            selectedObjectivesListBox.TabIndex = 2;
            // 
            // addObjectiveButton
            // 
            addObjectiveButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            addObjectiveButton.BackColor = Color.FromArgb(28, 28, 28);
            addObjectiveButton.Density = MaterialButton.MaterialButtonDensity.Default;
            addObjectiveButton.Depth = 0;
            addObjectiveButton.ForeColor = Color.White;
            addObjectiveButton.HighEmphasis = true;
            addObjectiveButton.Icon = null;
            addObjectiveButton.Location = new Point(0, 355);
            addObjectiveButton.Margin = new Padding(4, 6, 4, 6);
            addObjectiveButton.MouseState = MaterialSkin.MouseState.HOVER;
            addObjectiveButton.Name = "addObjectiveButton";
            addObjectiveButton.NoAccentTextColor = Color.Empty;
            addObjectiveButton.Size = new Size(131, 36);
            addObjectiveButton.TabIndex = 3;
            addObjectiveButton.Text = "Add Objective";
            addObjectiveButton.Type = MaterialButton.MaterialButtonType.Contained;
            addObjectiveButton.UseAccentColor = false;
            addObjectiveButton.UseVisualStyleBackColor = false;
            addObjectiveButton.Click += AddObjectiveButton_Click;
            // 
            // removeObjectiveButton
            // 
            removeObjectiveButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            removeObjectiveButton.BackColor = Color.FromArgb(28, 28, 28);
            removeObjectiveButton.Density = MaterialButton.MaterialButtonDensity.Default;
            removeObjectiveButton.Depth = 0;
            removeObjectiveButton.ForeColor = Color.White;
            removeObjectiveButton.HighEmphasis = true;
            removeObjectiveButton.Icon = null;
            removeObjectiveButton.Location = new Point(145, 355);
            removeObjectiveButton.Margin = new Padding(4, 6, 4, 6);
            removeObjectiveButton.MouseState = MaterialSkin.MouseState.HOVER;
            removeObjectiveButton.Name = "removeObjectiveButton";
            removeObjectiveButton.NoAccentTextColor = Color.Empty;
            removeObjectiveButton.Size = new Size(160, 36);
            removeObjectiveButton.TabIndex = 4;
            removeObjectiveButton.Text = "Remove Objective";
            removeObjectiveButton.Type = MaterialButton.MaterialButtonType.Contained;
            removeObjectiveButton.UseAccentColor = false;
            removeObjectiveButton.UseVisualStyleBackColor = false;
            removeObjectiveButton.Click += RemoveObjectiveButton_Click;
            // 
            // headerLabel
            // 
            headerLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            headerLabel.ForeColor = Color.White;
            headerLabel.Location = new Point(0, 0);
            headerLabel.Name = "headerLabel";
            headerLabel.Size = new Size(300, 25);
            headerLabel.TabIndex = 5;
            headerLabel.Text = "TimeSpace Objectives";
            headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // minLevelNumeric
            // 
            minLevelNumeric.BackColor = Color.FromArgb(28, 28, 28);
            minLevelNumeric.ForeColor = Color.White;
            minLevelNumeric.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            minLevelNumeric.Location = new Point(128, 310);
            minLevelNumeric.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            minLevelNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            minLevelNumeric.Name = "minLevelNumeric";
            minLevelNumeric.Size = new Size(152, 25);
            minLevelNumeric.TabIndex = 27;
            minLevelNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // maxLevelNumeric
            // 
            maxLevelNumeric.BackColor = Color.FromArgb(28, 28, 28);
            maxLevelNumeric.ForeColor = Color.White;
            maxLevelNumeric.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            maxLevelNumeric.Location = new Point(128, 340);
            maxLevelNumeric.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            maxLevelNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxLevelNumeric.Name = "maxLevelNumeric";
            maxLevelNumeric.Size = new Size(152, 25);
            maxLevelNumeric.TabIndex = 28;
            maxLevelNumeric.Value = new decimal(new int[] { 99, 0, 0, 0 });
            // 
            // seedsRequiredNumeric
            // 
            seedsRequiredNumeric.BackColor = Color.FromArgb(28, 28, 28);
            seedsRequiredNumeric.ForeColor = Color.White;
            seedsRequiredNumeric.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            seedsRequiredNumeric.Location = new Point(128, 370);
            seedsRequiredNumeric.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            seedsRequiredNumeric.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            seedsRequiredNumeric.Name = "seedsRequiredNumeric";
            seedsRequiredNumeric.Size = new Size(152, 25);
            seedsRequiredNumeric.TabIndex = 29;
            seedsRequiredNumeric.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // minPlayersNumeric
            // 
            minPlayersNumeric.BackColor = Color.FromArgb(28, 28, 28);
            minPlayersNumeric.ForeColor = Color.White;
            minPlayersNumeric.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            minPlayersNumeric.Location = new Point(128, 400);
            minPlayersNumeric.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            minPlayersNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            minPlayersNumeric.Name = "minPlayersNumeric";
            minPlayersNumeric.Size = new Size(152, 25);
            minPlayersNumeric.TabIndex = 30;
            minPlayersNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // maxPlayersNumeric
            // 
            maxPlayersNumeric.BackColor = Color.FromArgb(28, 28, 28);
            maxPlayersNumeric.ForeColor = Color.White;
            maxPlayersNumeric.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            maxPlayersNumeric.Location = new Point(128, 430);
            maxPlayersNumeric.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            maxPlayersNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxPlayersNumeric.Name = "maxPlayersNumeric";
            maxPlayersNumeric.Size = new Size(152, 25);
            maxPlayersNumeric.TabIndex = 31;
            maxPlayersNumeric.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // isHeroCheckBox
            // 
            isHeroCheckBox.BackColor = Color.FromArgb(28, 28, 28);
            isHeroCheckBox.ForeColor = Color.White;
            isHeroCheckBox.Location = new Point(128, 459);
            isHeroCheckBox.Name = "isHeroCheckBox";
            isHeroCheckBox.Size = new Size(152, 20);
            isHeroCheckBox.TabIndex = 32;
            isHeroCheckBox.UseVisualStyleBackColor = false;
            // 
            // isSpecialCheckBox
            // 
            isSpecialCheckBox.BackColor = Color.FromArgb(45, 45, 48);
            isSpecialCheckBox.ForeColor = Color.White;
            isSpecialCheckBox.Location = new Point(128, 485);
            isSpecialCheckBox.Name = "isSpecialCheckBox";
            isSpecialCheckBox.Size = new Size(152, 20);
            isSpecialCheckBox.TabIndex = 33;
            isSpecialCheckBox.Text = "Is Special";
            isSpecialCheckBox.UseVisualStyleBackColor = false;
            // 
            // isHiddenCheckBox
            // 
            isHiddenCheckBox.BackColor = Color.FromArgb(45, 45, 48);
            isHiddenCheckBox.ForeColor = Color.White;
            isHiddenCheckBox.Location = new Point(128, 510);
            isHiddenCheckBox.Name = "isHiddenCheckBox";
            isHiddenCheckBox.Size = new Size(152, 20);
            isHiddenCheckBox.TabIndex = 34;
            isHiddenCheckBox.Text = "Is Hidden";
            isHiddenCheckBox.UseVisualStyleBackColor = false;
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
            // pictureBox6
            // 
            pictureBox6.Location = new Point(944, 499);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(106, 94);
            pictureBox6.TabIndex = 48;
            pictureBox6.TabStop = false;
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
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            objectivesPanel.ResumeLayout(false);
            objectivesPanel.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
        }

        private void RemoveRewardButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddRewardButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
        private OpenFileDialog openFileDialog1;
        private TabPage tabPage3;
        public ModernTabControl tabControl2;
        private MaterialButton button6;
        private MaterialButton button5;
        private MaterialButton button4;
        private TabPage tabPage2;
        private ModernTextBox textBox8;
        private ModernTextBox textBox7;
        private ModernTextBox textBox6;
        private ModernTextBox textBox4;
        private ModernTextBox textBox5;
        private ModernTextBox textBox3;
        private ModernTextBox textBox2;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private ModernNumericUpDown numericUpDown1;
        private ModernNumericUpDown numericUpDown2;
        private ModernNumericUpDown minLevelNumeric;
        private ModernNumericUpDown maxLevelNumeric;
        private ModernNumericUpDown seedsRequiredNumeric;
        private ModernNumericUpDown minPlayersNumeric;
        private ModernNumericUpDown maxPlayersNumeric;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label headerLabel;
        private TabPage tabPage1;
        private Label minPlayersLabel;
        private Label maxPlayersLabel;
        private MaterialButton button8;
        private MaterialButton button7;
        private CheckBox isHeroCheckBox;
        private CheckBox isSpecialCheckBox;
        private CheckBox isHiddenCheckBox;
        private ModernTextBox textBox12;
        private ModernTextBox textBox11;
        private ModernTextBox textBox1;
        private MaterialButton button1;
        private ModernTabControl tabControl1;
        private MaterialButton button9;
        private ModernTextBox textBox13;
        private Panel panelButtons;
        private Panel objectivesPanel;
        private MaterialComboBox objectiveTypeComboBox;
        private Panel objectiveValuesPanel;
        private ListBox selectedObjectivesListBox;
        private MaterialButton addObjectiveButton;
        private MaterialButton removeObjectiveButton;
        private Dictionary<string, (ModernTextBox, ModernTextBox)> objectiveValueControls;
        private MaterialButton materialButton1;
        private ModernTextBox modernTextBox1;
        private ListBox drawRewardsListBox;
        private ListBox specialRewardsListBox;
        private ListBox bonusRewardsListBox;
        private ModernTextBox modernTextBox2;
        private MaterialButton materialButton2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox5;
        private Label label4;
        private ModernTextBox textBox9;
        private ModernTextBox textBox14;
        private ModernTextBox textBox10;
        private ModernTextBox textBox15;
        private PictureBox pictureBox1;
        private PictureBox pictureBox4;
        private ItemSlot pictureBox6;
    }
}