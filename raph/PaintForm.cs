using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;

namespace raph
{
    public partial class PaintForm : DockContent
    {
        private enum LogType
        {
            Infomation,
            Error
        }

        private string _DocumentTitle = String.Empty;
        private string _SourceCode = String.Empty;

        // 编译执行线程
        private Thread _WorkThread = null;

        // AST树
        private Language.ASTNode_StatementList _AST = null;

        // 初始化控件状态
        private void initState()
        {
            listView_info.Items.Clear();
            textBox_output.Text = String.Empty;
            pictureBox_result.BackgroundImage = Properties.Resources.Icon_Busy;
            pictureBox_result.Width = pictureBox_result.BackgroundImage.Width;
            pictureBox_result.Height = pictureBox_result.BackgroundImage.Height;
        }

        // 写日志函数
        private void writeLog(string Info, LogType LogType = LogType.Infomation, int Line = -1, int Row = -1, int Pos = -1)
        {
            this.Invoke((Action)delegate() {
                ListViewItem tItem = new ListViewItem();
                switch (LogType)
                {
                    case PaintForm.LogType.Infomation:
                        tItem.Text = "信息";
                        tItem.ImageIndex = imageList_main.Images.IndexOfKey("Infomation");
                        break;
                    case PaintForm.LogType.Error:
                        tItem.Text = "错误";
                        tItem.ImageIndex = imageList_main.Images.IndexOfKey("Error");
                        break;
                }
                
                // 时间
                tItem.SubItems.Add(new ListViewItem.ListViewSubItem(tItem, DateTime.Now.ToShortTimeString()));
                // 说明
                tItem.SubItems.Add(new ListViewItem.ListViewSubItem(tItem, Info));
                // 行号
                tItem.SubItems.Add(new ListViewItem.ListViewSubItem(tItem, Line == -1 ? String.Empty : Line.ToString()));
                // 列号
                tItem.SubItems.Add(new ListViewItem.ListViewSubItem(tItem, Row == -1 ? String.Empty : Row.ToString()));
                // 位置
                tItem.SubItems.Add(new ListViewItem.ListViewSubItem(tItem, Pos == -1 ? String.Empty : Pos.ToString()));

                listView_info.Items.Add(tItem);
            });
        }

        // 写输出文本
        private void writeOutputText(string Content)
        {
            this.Invoke((Action)delegate() {
                textBox_output.Text += Content;
            });
        }

        // 工作线程
        private void workThreadJob()
        {
            Stopwatch tWatch = new Stopwatch();

            // 初始化状态
            this.Invoke((Action)initState);

            // 检查AST树是否已生成
            if (_AST != null)
                writeLog("忽略解析过程");
            else
            {
                writeLog("正在执行解析过程...");

                // 解析代码
                bool bCompileSucceed = false;
                using (StringReader tReader = new StringReader(_SourceCode))
                {
                    tWatch.Start();
                    try
                    {
                        Language.Lexer tLexer = new Language.Lexer(tReader);  // 初始化Lexer
                        _AST = Language.Syntax.Parse(tLexer);  // 解析
                        bCompileSucceed = true;
                    }
                    catch (Language.LexcialException e)
                    {
                        writeLog(String.Format("词法错误：{0}", e.Description), LogType.Error, e.Line, e.Row, e.Position);
                    }
                    catch (Language.SyntaxException e)
                    {
                        writeLog(String.Format("语法错误：{0}", e.Description), LogType.Error, e.Line, e.Row, e.Position);
                    }
                    catch (Exception e)
                    {
                        writeLog(String.Format("一般性错误：{0}", e.Message), LogType.Error);
                    }
                    tWatch.Stop();
                    if (bCompileSucceed)
                        writeLog(String.Format("解析成功，耗时：{0} 秒", tWatch.ElapsedMilliseconds / 1000.0));
                    else
                        writeLog(String.Format("解析失败，耗时：{0} 秒", tWatch.ElapsedMilliseconds / 1000.0), LogType.Error);
                }
            }

            if (_AST != null)
            {
                // 执行语法树
                PaintRuntime tRT = new PaintRuntime();
                tRT.OnOutputText += delegate(PaintRuntime sender, string Content)
                {
                    writeOutputText(Content + "\r\n");
                };
                tRT.OnRuntimeException += delegate(PaintRuntime sender, Language.RuntimeException e)
                {
                    writeLog(String.Format("运行时错误：{0}", e.Description), LogType.Error, e.Line);
                };

                // 执行
                writeLog("正在执行...");
                tWatch.Start();
                tRT.RunAST(_AST);
                tWatch.Stop();
                writeLog(String.Format("执行完毕，耗时：{0} 秒", tWatch.ElapsedMilliseconds / 1000.0));

                // 设置图片
                this.Invoke((Action)delegate() {
                    pictureBox_result.BackgroundImage = tRT.TargetBuffer;
                    pictureBox_result.Width = pictureBox_result.BackgroundImage.Width;
                    pictureBox_result.Height = pictureBox_result.BackgroundImage.Height;
                });
            }
        }

        /// <summary>
        /// 获取或设置关联的文档标题
        /// </summary>
        public string DocumentTitle
        {
            get
            {
                return _DocumentTitle;
            }
            set
            {
                _DocumentTitle = value;

                if (String.IsNullOrEmpty(_DocumentTitle))
                    this.Text = "执行";
                else
                    this.Text = "执行 - " + _DocumentTitle;
            }
        }

        /// <summary>
        /// 提交源代码
        /// </summary>
        /// <param name="SourceCode">源代码</param>
        public void SubmitSourceCode(string SourceCode)
        {
            if (_WorkThread != null && _WorkThread.IsAlive)
            {
                MessageBox.Show(this, "请先结束当前任务然后继续。", "正忙", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (SourceCode != _SourceCode)
            {
                _SourceCode = SourceCode;
                _AST = null;
            }

            _WorkThread = new Thread(new ThreadStart(workThreadJob));
            _WorkThread.IsBackground = true;
            _WorkThread.Start();
        }

        public PaintForm()
        {
            InitializeComponent();
        }

        private void PaintForm_Load(object sender, EventArgs e)
        {

        }

        private void PaintForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Visible = false;
                e.Cancel = true;
            }
        }
    }
}
