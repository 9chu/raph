using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace raph.Language
{
    /// <summary>
    /// 实现代码高亮
    /// </summary>
    public abstract class CodeHighlight
    {
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        
        private static readonly string[] ConstList = new string[] {
            "pi", "e"
        };

        private static readonly string[] FunctionList = new string[] {
            "dot", "cross", "sin", "cos", "tan", "sinh", "cosh", "tanh", "asin", 
            "acos", "atan", "atan2", "sqrt", "exp", "ln", "log2", "log10", "abs",
            "max", "min", "ceil", "floor", "round", "trunc", "sgn"
        };

        /// <summary>
        /// 代码高亮词法状态机
        /// </summary>
        public enum HighlightState
        {
            Default,         // 默认文本
            Comment,         // 注释
            Symbol,          // 符号
            Digit,           // 数字
            Identifier,      // 标识符
            String,          // 字符串
            Keyword,         // 关键词
            Const,           // 常量
            InternalFunction // 内置函数
        }

        /// <summary>
        /// 着色回调函数
        /// </summary>
        /// <param name="TextBox">控件</param>
        /// <param name="From">从</param>
        /// <param name="To">到</param>
        /// <param name="State">着色状态</param>
        public delegate void HighlightHandler(RichTextBox TextBox, int From, int To, HighlightState State);

        /// <summary>
        /// 在一个范围内执行代码高亮
        /// </summary>
        /// <param name="TextBox">控件</param>
        /// <param name="From">从</param>
        /// <param name="To">到</param>
        /// <param name="Handler">回调</param>
        public static void PaintDocument(RichTextBox TextBox, int From, int To, HighlightHandler Handler)
        {
            if (From < 0)
                From = 0;
            if (To > TextBox.TextLength)
                To = TextBox.TextLength;

            // 保存现场
            int tCurPos = TextBox.SelectionStart;
            int tCurLen = TextBox.SelectionLength;

            SendMessage(TextBox.Handle, WM_SETREDRAW, 0, IntPtr.Zero);

            // 进行着色
            bool bStringEscape = false;
            int tStateStart = From;
            HighlightState tState = HighlightState.Default;
            for (int i = From; i <= To; ++i)
            {
                char c;
                if (i == To)
                    c = '\0';
                else
                    c = TextBox.Text[i];

                switch (tState)
                {
                    case HighlightState.Default:
                        switch (c)
                        {
                            case '+':
                            case '-':
                            case '*':
                            case '/':
                            case ';':
                            case '>':
                            case '<':
                            case '=':
                            case '!':
                            case '&':
                            case '|':
                            case '(':
                            case ')':
                            case '{':
                            case '}':
                            case ',':
                            case '.':
                                Handler(TextBox, tStateStart, i - 1, HighlightState.Default);
                                tStateStart = i;
                                tState = HighlightState.Symbol;
                                break;
                            case '"':
                                Handler(TextBox, tStateStart, i - 1, HighlightState.Default);
                                tStateStart = i;
                                tState = HighlightState.String;
                                bStringEscape = false;
                                break;
                            default:
                                if (c >= '0' && c <= '9')
                                {
                                    Handler(TextBox, tStateStart, i - 1, HighlightState.Default);
                                    tStateStart = i;
                                    tState = HighlightState.Digit;
                                }
                                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= 128))
                                {
                                    Handler(TextBox, tStateStart, i - 1, HighlightState.Default);
                                    tStateStart = i;
                                    tState = HighlightState.Identifier;
                                }
                                break;
                        }
                        break;
                    case HighlightState.Comment:
                        if (c == '\n' || c == '\0')
                        {
                            Handler(TextBox, tStateStart, i, HighlightState.Comment);
                            tStateStart = i + 1;
                            tState = HighlightState.Default;
                        }
                        break;
                    case HighlightState.Symbol:
                        switch (c)
                        {
                            case '+':
                            case '-':
                            case '*':
                            case '/':
                            case ';':
                            case '>':
                            case '<':
                            case '=':
                            case '!':
                            case '&':
                            case '|':
                            case '(':
                            case ')':
                            case '{':
                            case '}':
                            case ',':
                            case '.':
                                if (i == tStateStart + 1)
                                {
                                    if ((TextBox.Text[tStateStart] == '/' && c == '/') ||
                                        (TextBox.Text[tStateStart] == '-' && c == '-'))
                                    {
                                        // 应该被解释为注释
                                        i = tStateStart - 1;
                                        tState = HighlightState.Comment;
                                        break;
                                    }
                                }
                                break;
                            default:
                                Handler(TextBox, tStateStart, i - 1, HighlightState.Symbol);
                                tStateStart = i;
                                tState = HighlightState.Default;
                                i--;
                                break;
                        }
                        break;
                    case HighlightState.Digit:
                        if (!((c >= '0' && c <= '9') || c == '.'))
                        {
                            Handler(TextBox, tStateStart, i - 1, HighlightState.Digit);
                            tStateStart = i;
                            tState = HighlightState.Default;
                            i--;
                        }
                        break;
                    case HighlightState.Identifier:
                        if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= 128) || (c >= '0' && c <= '9')))
                        {
                            // 截取字符串
                            string tIdentifier = TextBox.Text.Substring(tStateStart, i - tStateStart);
                            string tIdentifierLower = tIdentifier.ToLower();
                            if (Lexer.KeywordList.Contains(tIdentifierLower))
                                tState = HighlightState.Keyword;
                            else if (ConstList.Contains(tIdentifierLower))
                                tState = HighlightState.Const;
                            else if (FunctionList.Contains(tIdentifierLower))
                                tState = HighlightState.InternalFunction;

                            Handler(TextBox, tStateStart, i - 1, tState);
                            tStateStart = i;
                            tState = HighlightState.Default;
                            i--;
                        }
                        break;
                    case HighlightState.String:
                        if (c == '\0' || c == '\n' || c == '\r')  // 错误的终止字符
                        {
                            Handler(TextBox, tStateStart, i, tState);
                            tStateStart = i + 1;
                            tState = HighlightState.Default;
                        }
                        else
                        {
                            if (!bStringEscape)
                            {
                                if (c == '"')
                                {
                                    Handler(TextBox, tStateStart, i, tState);
                                    tStateStart = i + 1;
                                    tState = HighlightState.Default;
                                }
                                else if (c == '\\')
                                {
                                    bStringEscape = true;
                                }
                            }
                            else
                                bStringEscape = false;
                        }
                        break;
                }
            }

            // 恢复现场
            TextBox.Select(tCurPos, tCurLen);
            TextBox.SelectionFont = TextBox.Font;
            TextBox.SelectionColor = TextBox.ForeColor;

            SendMessage(TextBox.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            TextBox.Refresh();
        }
    }
}
