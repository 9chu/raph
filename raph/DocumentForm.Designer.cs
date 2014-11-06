namespace raph
{
    partial class DocumentForm
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
            this.toolStrip_main = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.ToolStripMenuItem_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_run = new System.Windows.Forms.ToolStripButton();
            this.statusStrip_main = new System.Windows.Forms.StatusStrip();
            this.richTextBox_main = new System.Windows.Forms.RichTextBox();
            this.saveFileDialog_main = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip_main
            // 
            this.toolStrip_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripButton_run});
            this.toolStrip_main.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_main.Name = "toolStrip_main";
            this.toolStrip_main.Size = new System.Drawing.Size(286, 25);
            this.toolStrip_main.TabIndex = 0;
            this.toolStrip_main.Text = "工具栏";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Save,
            this.ToolStripMenuItem_SaveAs});
            this.toolStripDropDownButton1.Image = global::raph.Properties.Resources.Icon_Save;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButton1.Text = "保存";
            // 
            // ToolStripMenuItem_Save
            // 
            this.ToolStripMenuItem_Save.Name = "ToolStripMenuItem_Save";
            this.ToolStripMenuItem_Save.Size = new System.Drawing.Size(130, 22);
            this.ToolStripMenuItem_Save.Text = "保存(&S)";
            this.ToolStripMenuItem_Save.Click += new System.EventHandler(this.ToolStripMenuItem_Save_Click);
            // 
            // ToolStripMenuItem_SaveAs
            // 
            this.ToolStripMenuItem_SaveAs.Name = "ToolStripMenuItem_SaveAs";
            this.ToolStripMenuItem_SaveAs.Size = new System.Drawing.Size(130, 22);
            this.ToolStripMenuItem_SaveAs.Text = "另存为...";
            this.ToolStripMenuItem_SaveAs.Click += new System.EventHandler(this.ToolStripMenuItem_SaveAs_Click);
            // 
            // toolStripButton_run
            // 
            this.toolStripButton_run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_run.Image = global::raph.Properties.Resources.Icon_Play;
            this.toolStripButton_run.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_run.Name = "toolStripButton_run";
            this.toolStripButton_run.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton_run.Text = "运行脚本";
            this.toolStripButton_run.Click += new System.EventHandler(this.toolStripButton_run_Click);
            // 
            // statusStrip_main
            // 
            this.statusStrip_main.Location = new System.Drawing.Point(0, 244);
            this.statusStrip_main.Name = "statusStrip_main";
            this.statusStrip_main.Size = new System.Drawing.Size(286, 22);
            this.statusStrip_main.TabIndex = 1;
            this.statusStrip_main.Text = "statusStrip1";
            // 
            // richTextBox_main
            // 
            this.richTextBox_main.AcceptsTab = true;
            this.richTextBox_main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_main.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_main.Location = new System.Drawing.Point(0, 25);
            this.richTextBox_main.Name = "richTextBox_main";
            this.richTextBox_main.Size = new System.Drawing.Size(286, 219);
            this.richTextBox_main.TabIndex = 2;
            this.richTextBox_main.Text = "";
            this.richTextBox_main.WordWrap = false;
            this.richTextBox_main.TextChanged += new System.EventHandler(this.richTextBox_main_TextChanged);
            // 
            // saveFileDialog_main
            // 
            this.saveFileDialog_main.DefaultExt = "raph";
            this.saveFileDialog_main.Filter = "raph代码文件|*.raph";
            this.saveFileDialog_main.Title = "保存代码文件";
            // 
            // DocumentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 266);
            this.Controls.Add(this.richTextBox_main);
            this.Controls.Add(this.statusStrip_main);
            this.Controls.Add(this.toolStrip_main);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "DocumentForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentForm_FormClosing);
            this.Load += new System.EventHandler(this.DocumentForm_Load);
            this.toolStrip_main.ResumeLayout(false);
            this.toolStrip_main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip_main;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Save;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_SaveAs;
        private System.Windows.Forms.ToolStripButton toolStripButton_run;
        private System.Windows.Forms.StatusStrip statusStrip_main;
        private System.Windows.Forms.RichTextBox richTextBox_main;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_main;
    }
}