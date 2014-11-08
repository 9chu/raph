using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;

namespace raph
{
    public partial class DocumentForm : DockContent
    {
        private PaintForm _PaintForm = new PaintForm();

        private string _FilePath = String.Empty;
        private string _Title = "无标题";
        private bool _Edited = false;
        private bool _StopPaint = false;

        private void refreshTitle()
        {
            this.Text = String.Format("{0}{1}", _Title, _Edited ? "*" : String.Empty);
            _PaintForm.DocumentTitle = _Title;
        }

        // 着色处理函数
        private void highlightCallback(RichTextBox TextBox, int From, int To, Language.CodeHighlight.HighlightState State)
        {
            switch (State)
            {
                case Language.CodeHighlight.HighlightState.Comment:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = TextBox.Font;
                    TextBox.SelectionColor = Color.Green;
                    break;
                case Language.CodeHighlight.HighlightState.Symbol:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = new Font(TextBox.Font, FontStyle.Bold);
                    TextBox.SelectionColor = Color.Navy;
                    break;
                case Language.CodeHighlight.HighlightState.Digit:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = TextBox.Font;
                    TextBox.SelectionColor = Color.Maroon;
                    break;
                case Language.CodeHighlight.HighlightState.Identifier:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = TextBox.Font;
                    TextBox.SelectionColor = Color.DimGray;
                    break;
                case Language.CodeHighlight.HighlightState.Keyword:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = new Font(TextBox.Font, FontStyle.Bold | FontStyle.Italic);
                    TextBox.SelectionColor = Color.CornflowerBlue;
                    break;
                case Language.CodeHighlight.HighlightState.Const:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = new Font(TextBox.Font, FontStyle.Bold);
                    TextBox.SelectionColor = Color.MediumOrchid;
                    break;
                case Language.CodeHighlight.HighlightState.InternalFunction:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = TextBox.Font;
                    TextBox.SelectionColor = Color.LightSalmon;
                    break;
                case Language.CodeHighlight.HighlightState.String:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = TextBox.Font;
                    TextBox.SelectionColor = Color.DarkRed;
                    break;
                default:
                    TextBox.Select(From, To - From + 1);
                    TextBox.SelectionFont = TextBox.Font;
                    TextBox.SelectionColor = Color.Black;
                    break;
            }
        }
        
        // 装载文档
        private void loadDocument(string FilePath)
        {
            _FilePath = FilePath;
            _Title = Path.GetFileNameWithoutExtension(FilePath);

            try
            {
                richTextBox_main.Text = File.ReadAllText(_FilePath, Encoding.UTF8);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("打开文件\"{0}\"失败。\n\n{1}", _FilePath, e.ToString()), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 进行着色
            _StopPaint = true;
            Language.CodeHighlight.PaintDocument(richTextBox_main, 0, richTextBox_main.Text.Length - 1, highlightCallback);
            _StopPaint = false;
            _Edited = false;

            refreshTitle();
        }

        // 保存文档
        private void saveDocument(string FilePath)
        {
            _FilePath = FilePath;
            _Title = Path.GetFileNameWithoutExtension(FilePath);

            try
            {
                File.WriteAllText(_FilePath, richTextBox_main.Text, Encoding.UTF8);
                _Edited = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("保存文件\"{0}\"失败。\n\n{1}", _FilePath, e.ToString()), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _FilePath = String.Empty;
            }

            refreshTitle();
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
        }

        /// <summary>
        /// 是否已修改
        /// </summary>
        public bool Edited
        {
            get { return _Edited; }
        }

        /// <summary>
        /// 创建一个空的文档
        /// </summary>
        public DocumentForm()
        {
            InitializeComponent();
            _PaintForm.Owner = this;

            refreshTitle();
        }

        /// <summary>
        /// 从文件加载文档
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        public DocumentForm(string FilePath)
        {
            InitializeComponent();
            _PaintForm.Owner = this;

            loadDocument(FilePath);
        }

        private void DocumentForm_Load(object sender, EventArgs e)
        {
        }

        private void richTextBox_main_TextChanged(object sender, EventArgs e)
        {
            if (_StopPaint == false)
            {
                // 重新着色相邻两行
                int tLine = richTextBox_main.GetLineFromCharIndex(richTextBox_main.SelectionStart);

                int tIndex;
                int tLength;
                if (tLine < richTextBox_main.Lines.Length)
                {
                    tIndex = richTextBox_main.GetFirstCharIndexFromLine(tLine);
                    tLength = richTextBox_main.Lines[tLine].Length;
                    Language.CodeHighlight.PaintDocument(richTextBox_main, tIndex, tIndex + tLength, highlightCallback);
                }

                if (tLine > 0)
                {
                    tIndex = richTextBox_main.GetFirstCharIndexFromLine(tLine - 1);
                    tLength = richTextBox_main.Lines[tLine - 1].Length;
                    Language.CodeHighlight.PaintDocument(richTextBox_main, tIndex, tIndex + tLength, highlightCallback);
                }
            }

            if (!_Edited)
            {
                _Edited = true;
                refreshTitle();
            }
        }

        private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_Edited)
            {
                if (DialogResult.No == MessageBox.Show("当前文档还没有被保存，确定要关闭吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    e.Cancel = true;
            }
        }

        private void ToolStripMenuItem_Save_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_FilePath))
            {
                ToolStripMenuItem_SaveAs_Click(sender, e);
            }
            else
            {
                saveDocument(_FilePath);
            }
        }

        private void ToolStripMenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog_main.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveDocument(saveFileDialog_main.FileName);
            }
        }

        private void toolStripButton_run_Click(object sender, EventArgs e)
        {
            _PaintForm.DockPanel = this.DockPanel;
            _PaintForm.Show();
            _PaintForm.Activate();
            _PaintForm.SubmitSourceCode(richTextBox_main.Text);
        }
    }
}
