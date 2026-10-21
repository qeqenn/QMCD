namespace QuickMinecraftDownloader
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
            menuStrip1 = new MenuStrip();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            comboBox1 = new ComboBox();
            ChooseOutDirbtn = new Button();
            label2 = new Label();
            GameVers = new ComboBox();
            正式版 = new RadioButton();
            GameVerPanel = new Panel();
            愚人节版 = new RadioButton();
            预览版 = new RadioButton();
            刷新GameVer = new Button();
            label3 = new Label();
            ModLoaderPanel = new Panel();
            Optifine = new RadioButton();
            Fabric = new RadioButton();
            NeoForge = new RadioButton();
            Forge = new RadioButton();
            无加载器 = new RadioButton();
            menuStrip1.SuspendLayout();
            GameVerPanel.SuspendLayout();
            ModLoaderPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.ControlLight;
            menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 21);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 31);
            label1.Name = "label1";
            label1.Size = new Size(80, 17);
            label1.TabIndex = 1;
            label1.Text = "输出文件夹：";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(98, 28);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(601, 25);
            comboBox1.TabIndex = 2;
            // 
            // ChooseOutDirbtn
            // 
            ChooseOutDirbtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ChooseOutDirbtn.Location = new Point(705, 30);
            ChooseOutDirbtn.Name = "ChooseOutDirbtn";
            ChooseOutDirbtn.Size = new Size(83, 23);
            ChooseOutDirbtn.TabIndex = 3;
            ChooseOutDirbtn.Text = "选择文件夹";
            ChooseOutDirbtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 66);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 4;
            label2.Text = "选择版本：";
            // 
            // GameVers
            // 
            GameVers.FormattingEnabled = true;
            GameVers.Location = new Point(98, 63);
            GameVers.Name = "GameVers";
            GameVers.Size = new Size(156, 25);
            GameVers.TabIndex = 5;
            // 
            // 正式版
            // 
            正式版.AutoSize = true;
            正式版.Location = new Point(3, 3);
            正式版.Name = "正式版";
            正式版.Size = new Size(62, 21);
            正式版.TabIndex = 7;
            正式版.TabStop = true;
            正式版.Text = "正式版";
            正式版.UseVisualStyleBackColor = true;
            // 
            // GameVerPanel
            // 
            GameVerPanel.Controls.Add(愚人节版);
            GameVerPanel.Controls.Add(预览版);
            GameVerPanel.Controls.Add(正式版);
            GameVerPanel.Location = new Point(312, 63);
            GameVerPanel.Name = "GameVerPanel";
            GameVerPanel.Size = new Size(222, 25);
            GameVerPanel.TabIndex = 8;
            // 
            // 愚人节版
            // 
            愚人节版.AutoSize = true;
            愚人节版.Location = new Point(139, 3);
            愚人节版.Name = "愚人节版";
            愚人节版.Size = new Size(74, 21);
            愚人节版.TabIndex = 9;
            愚人节版.TabStop = true;
            愚人节版.Text = "愚人节版";
            愚人节版.UseVisualStyleBackColor = true;
            // 
            // 预览版
            // 
            预览版.AutoSize = true;
            预览版.Location = new Point(71, 3);
            预览版.Name = "预览版";
            预览版.Size = new Size(62, 21);
            预览版.TabIndex = 8;
            预览版.TabStop = true;
            预览版.Text = "预览版";
            预览版.UseVisualStyleBackColor = true;
            // 
            // 刷新GameVer
            // 
            刷新GameVer.Location = new Point(260, 64);
            刷新GameVer.Name = "刷新GameVer";
            刷新GameVer.Size = new Size(46, 23);
            刷新GameVer.TabIndex = 9;
            刷新GameVer.Text = "刷新";
            刷新GameVer.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 104);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 10;
            label3.Text = "选择加载器：";
            // 
            // ModLoaderPanel
            // 
            ModLoaderPanel.Controls.Add(Optifine);
            ModLoaderPanel.Controls.Add(Fabric);
            ModLoaderPanel.Controls.Add(NeoForge);
            ModLoaderPanel.Controls.Add(Forge);
            ModLoaderPanel.Controls.Add(无加载器);
            ModLoaderPanel.Location = new Point(98, 100);
            ModLoaderPanel.Name = "ModLoaderPanel";
            ModLoaderPanel.Size = new Size(358, 25);
            ModLoaderPanel.TabIndex = 11;
            // 
            // Optifine
            // 
            Optifine.AutoSize = true;
            Optifine.Location = new Point(271, 3);
            Optifine.Name = "Optifine";
            Optifine.Size = new Size(72, 21);
            Optifine.TabIndex = 14;
            Optifine.TabStop = true;
            Optifine.Text = "Optifine";
            Optifine.UseVisualStyleBackColor = true;
            // 
            // Fabric
            // 
            Fabric.AutoSize = true;
            Fabric.Location = new Point(204, 3);
            Fabric.Name = "Fabric";
            Fabric.Size = new Size(61, 21);
            Fabric.TabIndex = 13;
            Fabric.TabStop = true;
            Fabric.Text = "Fabric";
            Fabric.UseVisualStyleBackColor = true;
            // 
            // NeoForge
            // 
            NeoForge.AutoSize = true;
            NeoForge.Location = new Point(113, 3);
            NeoForge.Name = "NeoForge";
            NeoForge.Size = new Size(85, 21);
            NeoForge.TabIndex = 12;
            NeoForge.TabStop = true;
            NeoForge.Text = "NeoForge";
            NeoForge.UseVisualStyleBackColor = true;
            // 
            // Forge
            // 
            Forge.AutoSize = true;
            Forge.Location = new Point(47, 3);
            Forge.Name = "Forge";
            Forge.Size = new Size(60, 21);
            Forge.TabIndex = 1;
            Forge.TabStop = true;
            Forge.Text = "Forge";
            Forge.UseVisualStyleBackColor = true;
            // 
            // 无加载器
            // 
            无加载器.AutoSize = true;
            无加载器.Location = new Point(3, 3);
            无加载器.Name = "无加载器";
            无加载器.Size = new Size(38, 21);
            无加载器.TabIndex = 0;
            无加载器.TabStop = true;
            无加载器.Text = "无";
            无加载器.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ModLoaderPanel);
            Controls.Add(label3);
            Controls.Add(刷新GameVer);
            Controls.Add(GameVerPanel);
            Controls.Add(GameVers);
            Controls.Add(label2);
            Controls.Add(ChooseOutDirbtn);
            Controls.Add(comboBox1);
            Controls.Add(label1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "QMCD";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            GameVerPanel.ResumeLayout(false);
            GameVerPanel.PerformLayout();
            ModLoaderPanel.ResumeLayout(false);
            ModLoaderPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem 文件ToolStripMenuItem;
        private Label label1;
        private ComboBox comboBox1;
        private Button ChooseOutDirbtn;
        private Label label2;
        private ComboBox GameVers;
        private RadioButton 正式版;
        private Panel GameVerPanel;
        private RadioButton 愚人节版;
        private RadioButton 预览版;
        private Button 刷新GameVer;
        private Label label3;
        private Panel ModLoaderPanel;
        private RadioButton Fabric;
        private RadioButton NeoForge;
        private RadioButton Forge;
        private RadioButton 无加载器;
        private RadioButton Optifine;
    }
}
