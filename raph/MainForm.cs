using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace raph
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void ToolStripMenuItem_New_Click(object sender, EventArgs e)
        {
            DocumentForm tForm = new DocumentForm();
            tForm.Show(dockPanel_main, DockState.Document);
        }

        private void ToolStripMenuItem_Open_Click(object sender, EventArgs e)
        {
            if (openFileDialog_main.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DocumentForm tForm = new DocumentForm(openFileDialog_main.FileName);
                tForm.Show(dockPanel_main, DockState.Document);
            }
        }

        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ToolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            using (AboutForm tForm = new AboutForm())
            {
                tForm.ShowDialog();
            }
        }
    }
}
