namespace raph
{
    partial class PaintForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaintForm));
            this.toolStrip_main = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip_main = new System.Windows.Forms.StatusStrip();
            this.splitContainer_main = new System.Windows.Forms.SplitContainer();
            this.tabControl_main = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView_info = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList_main = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox_output = new System.Windows.Forms.TextBox();
            this.pictureBox_result = new System.Windows.Forms.PictureBox();
            this.toolStripButton_stop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_restart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_save = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog_image = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).BeginInit();
            this.splitContainer_main.Panel1.SuspendLayout();
            this.splitContainer_main.Panel2.SuspendLayout();
            this.splitContainer_main.SuspendLayout();
            this.tabControl_main.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_result)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip_main
            // 
            this.toolStrip_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_stop,
            this.toolStripButton_restart,
            this.toolStripSeparator1,
            this.toolStripButton_save});
            this.toolStrip_main.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_main.Name = "toolStrip_main";
            this.toolStrip_main.Size = new System.Drawing.Size(626, 25);
            this.toolStrip_main.TabIndex = 0;
            this.toolStrip_main.Text = "工具栏";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // statusStrip_main
            // 
            this.statusStrip_main.Location = new System.Drawing.Point(0, 424);
            this.statusStrip_main.Name = "statusStrip_main";
            this.statusStrip_main.Size = new System.Drawing.Size(626, 22);
            this.statusStrip_main.TabIndex = 1;
            this.statusStrip_main.Text = "statusStrip1";
            // 
            // splitContainer_main
            // 
            this.splitContainer_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_main.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer_main.Location = new System.Drawing.Point(0, 25);
            this.splitContainer_main.Name = "splitContainer_main";
            this.splitContainer_main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_main.Panel1
            // 
            this.splitContainer_main.Panel1.AutoScroll = true;
            this.splitContainer_main.Panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer_main.Panel1.Controls.Add(this.pictureBox_result);
            // 
            // splitContainer_main.Panel2
            // 
            this.splitContainer_main.Panel2.Controls.Add(this.tabControl_main);
            this.splitContainer_main.Size = new System.Drawing.Size(626, 399);
            this.splitContainer_main.SplitterDistance = 200;
            this.splitContainer_main.TabIndex = 2;
            // 
            // tabControl_main
            // 
            this.tabControl_main.Controls.Add(this.tabPage1);
            this.tabControl_main.Controls.Add(this.tabPage2);
            this.tabControl_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_main.Name = "tabControl_main";
            this.tabControl_main.SelectedIndex = 0;
            this.tabControl_main.Size = new System.Drawing.Size(626, 195);
            this.tabControl_main.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView_info);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(618, 169);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "错误列表";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView_info
            // 
            this.listView_info.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader6,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_info.FullRowSelect = true;
            this.listView_info.GridLines = true;
            this.listView_info.Location = new System.Drawing.Point(3, 3);
            this.listView_info.Name = "listView_info";
            this.listView_info.Size = new System.Drawing.Size(612, 163);
            this.listView_info.SmallImageList = this.imageList_main;
            this.listView_info.TabIndex = 0;
            this.listView_info.UseCompatibleStateImageBehavior = false;
            this.listView_info.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "类型";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "时间";
            this.columnHeader2.Width = 45;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "说明";
            this.columnHeader6.Width = 380;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "行";
            this.columnHeader3.Width = 30;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "列";
            this.columnHeader4.Width = 30;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "位置";
            this.columnHeader5.Width = 40;
            // 
            // imageList_main
            // 
            this.imageList_main.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_main.ImageStream")));
            this.imageList_main.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_main.Images.SetKeyName(0, "Error");
            this.imageList_main.Images.SetKeyName(1, "Infomation");
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox_output);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(618, 169);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "输出";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox_output
            // 
            this.textBox_output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_output.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_output.Location = new System.Drawing.Point(3, 3);
            this.textBox_output.Multiline = true;
            this.textBox_output.Name = "textBox_output";
            this.textBox_output.ReadOnly = true;
            this.textBox_output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_output.Size = new System.Drawing.Size(612, 163);
            this.textBox_output.TabIndex = 0;
            // 
            // pictureBox_result
            // 
            this.pictureBox_result.BackColor = System.Drawing.Color.White;
            this.pictureBox_result.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox_result.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_result.Name = "pictureBox_result";
            this.pictureBox_result.Size = new System.Drawing.Size(100, 50);
            this.pictureBox_result.TabIndex = 0;
            this.pictureBox_result.TabStop = false;
            // 
            // toolStripButton_stop
            // 
            this.toolStripButton_stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_stop.Enabled = false;
            this.toolStripButton_stop.Image = global::raph.Properties.Resources.Icon_Stop;
            this.toolStripButton_stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_stop.Name = "toolStripButton_stop";
            this.toolStripButton_stop.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_stop.Text = "终止";
            this.toolStripButton_stop.Click += new System.EventHandler(this.toolStripButton_stop_Click);
            // 
            // toolStripButton_restart
            // 
            this.toolStripButton_restart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_restart.Enabled = false;
            this.toolStripButton_restart.Image = global::raph.Properties.Resources.Icon_Restart;
            this.toolStripButton_restart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_restart.Name = "toolStripButton_restart";
            this.toolStripButton_restart.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_restart.Text = "重新运行";
            this.toolStripButton_restart.Click += new System.EventHandler(this.toolStripButton_restart_Click);
            // 
            // toolStripButton_save
            // 
            this.toolStripButton_save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_save.Enabled = false;
            this.toolStripButton_save.Image = global::raph.Properties.Resources.Icon_Save;
            this.toolStripButton_save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_save.Name = "toolStripButton_save";
            this.toolStripButton_save.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_save.Text = "保存当前绘制结果";
            this.toolStripButton_save.Click += new System.EventHandler(this.toolStripButton_save_Click);
            // 
            // saveFileDialog_image
            // 
            this.saveFileDialog_image.DefaultExt = "png";
            this.saveFileDialog_image.Filter = "PNG图像|*.png";
            this.saveFileDialog_image.Title = "保存图片";
            // 
            // PaintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 446);
            this.Controls.Add(this.splitContainer_main);
            this.Controls.Add(this.statusStrip_main);
            this.Controls.Add(this.toolStrip_main);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PaintForm";
            this.Text = "执行";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PaintForm_FormClosing);
            this.Load += new System.EventHandler(this.PaintForm_Load);
            this.toolStrip_main.ResumeLayout(false);
            this.toolStrip_main.PerformLayout();
            this.splitContainer_main.Panel1.ResumeLayout(false);
            this.splitContainer_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).EndInit();
            this.splitContainer_main.ResumeLayout(false);
            this.tabControl_main.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_result)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip_main;
        private System.Windows.Forms.StatusStrip statusStrip_main;
        private System.Windows.Forms.SplitContainer splitContainer_main;
        private System.Windows.Forms.TabControl tabControl_main;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView_info;
        private System.Windows.Forms.ImageList imageList_main;
        private System.Windows.Forms.ToolStripButton toolStripButton_stop;
        private System.Windows.Forms.ToolStripButton toolStripButton_restart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_save;
        private System.Windows.Forms.TextBox textBox_output;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.PictureBox pictureBox_result;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_image;
    }
}