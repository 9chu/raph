using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace raph.Language
{
    /// <summary>
    /// 词法分析器
    /// </summary>
    public class Lexer
    {
        // 词法元素
        public enum Token
        {
            EOF,             // EOF
            Semico,          // ;
            LeftBracket,     // (
            RightBracket,    // )
            LeftBrace,       // {
            RightBrace,      // }
            Comma,           // ,
            DigitLiteral,    // 数字字面量
            StringLiteral,   // 字符串字面量
            Identifier,      // 标识符

            // 运算符
            Plus = 50,       // +
            Minus,           // -
            Mul,             // *
            Div,             // /
            Power,           // **
            Greater,         // >
            Less,            // <
            GreaterEqual,    // >=
            LessEqual,       // <=
            Equal,           // ==
            NotEqual,        // !=
            LogicalAnd,      // &&
            LogicalOr,       // ||
            Assign,          // =

            // 关键词
            Is = 100,
            Begin,
            End,
            True,
            False,
            For,
            From,
            To,
            Step
        }

        // 关键词列表
        private static readonly string[] KeywordList = new string[]
        {
            "is", "begin", "end", "true", "false", "for", "from", "to", "step"
        };

        /// <summary>
        /// 格式化一个Token
        /// </summary>
        /// <param name="TokenToFormat">需要格式化显示的Token</param>
        /// <returns>格式化返回字串</returns>
        public static string FormatToken(Token TokenToFormat)
        {
            switch (TokenToFormat)
            {
                case Token.EOF:
                    return "<EOF>";
                case Token.Semico:
                    return "';'";
                case Token.LeftBracket:
                    return "'('";
                case Token.RightBracket:
                    return "')'";
                case Token.LeftBrace:
                    return "'{'";
                case Token.RightBrace:
                    return "'}'";
                case Token.Comma:
                    return "','";
                case Token.Plus:
                    return "'+'";
                case Token.Minus:
                    return "'-'";
                case Token.Mul:
                    return "'*'";
                case Token.Div:
                    return "'/'";
                case Token.Power:
                    return "'**'";
                case Token.Greater:
                    return "'>'";
                case Token.Less:
                    return "'<'";
                case Token.GreaterEqual:
                    return "'>='";
                case Token.LessEqual:
                    return "'<='";
                case Token.Equal:
                    return "'=='";
                case Token.NotEqual:
                    return "'!='";
                case Token.LogicalAnd:
                    return "'&&'";
                case Token.LogicalOr:
                    return "'||'";
                case Token.DigitLiteral:
                    return "<digit>";
                case Token.StringLiteral:
                    return "<string>";
                case Token.Identifier:
                    return "<identifier>";
                default:
                    if (TokenToFormat >= Token.Is && (int)TokenToFormat < (int)Token.Is + KeywordList.Length)
                        return String.Format("<{0}>", KeywordList[TokenToFormat - Token.Is]);
                    else
                        return "<unknown>";
            }
        }

        private TextReader _Reader = null;
        private int _BufNext = -1;

        private Token _CurToken = Token.EOF;
        private double _CurDigit = 0;
        private string _CurId = String.Empty;
        private string _CurIdLower = String.Empty;
        private string _CurString = String.Empty;
        private int _Position = 0;
        private int _Line = 1;
        private int _Row = 0;
        
        private static string formatCharacter(int c)
        {
            if (c == -1)
                return "\\0";
            else if (Char.IsControl((char)c))
                return "\\x" + ((short)c).ToString("X");
            else
                return ((char)c).ToString();
        }

        private static bool isIdentifyStart(int c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c >= 128;
        }

        private static bool isIdentifyCharacter(int c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || (c >= '0' && c <= '9') || c >= 128;
        }

        private int readNext()
        {
            int t = _BufNext;
            if (t != -1)
            {
                if (t == '\n')
                {
                    _Line++;
                    _Row = 0;
                }
                _Position++;
                _Row++;
            }
            _BufNext = _Reader.Read();
            return t;
        }

        private int peekNext()
        {
            return _BufNext;
        }

        private int peekNext2()
        {
            return _Reader.Peek();
        }

        /// <summary>
        /// 跳过空白或者注释
        /// </summary>
        /// <returns>是否发生跳过操作</returns>
        private bool skipBlankOrComment()
        {
            bool bRet = false;
            while (true)
            {
                int c = peekNext();
                if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
                {
                    bRet = true;
                    readNext();  // 吃掉空白符
                }
                else if ((c == '/' && peekNext2() == '/') || (c == '-' && peekNext2() == '-'))
                {
                    bRet = true;
                    while (!(c == '\n' || c == -1))
                        c = readNext();  // 读到行尾
                }
                else
                    break;
            }
            return bRet;
        }

        /// <summary>
        /// 读取数字
        /// </summary>
        /// <returns>读取的数字</returns>
        private double parseDigit()
        {
            int c;
            double tRet = 0;
            
            // 读取整数部分
            while (true)
            {
                c = readNext();
                tRet = tRet * 10;
                tRet += c - '0';
                c = peekNext();
                if (!(c >= '0' && c <= '9'))
                    break;
            }

            // 读取小数部分
            if (peekNext() == '.')
            {
                readNext();

                double tFrac = 0;
                double tPow = 0.1;
                while (true)
                {
                    c = peekNext();
                    if (!(c >= '0' && c <= '9'))
                        break;
                    c = readNext();
                    tFrac += tPow * (c - '0');
                    tPow *= 0.1;
                }
                tRet += tFrac;
            }

            // 检查后续字符
            c = peekNext();
            if (isIdentifyCharacter(c))
                throw new LexcialException(_Position, _Line, _Row,
                    String.Format("not a valid following charcater '{0}'.", formatCharacter(c)));
            return tRet;
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <returns>读取的字符串</returns>
        private string parseString()
        {
            if ('"' != readNext())
                throw new LexcialException(_Position, _Line, _Row,
                    String.Format("internal error."));

            StringBuilder tBuilder = new StringBuilder();
            while (true)
            {
                int c = peekNext();
                if (c == '\r' || c == '\n' || c == '\0')
                    throw new LexcialException(_Position, _Line, _Row,
                        String.Format("unexpected character '{0}' in string literal.", formatCharacter(c)));
                else if (c == '"')
                {
                    readNext();
                    break;
                }
                else if (c == '\\')  // 转义字符
                {
                    readNext();

                    switch (c = peekNext())
                    {
                        case '"':
                            readNext();
                            tBuilder.Append('"');
                            break;
                        case '\\':
                            readNext();
                            tBuilder.Append('\\');
                            break;
                        case '/':
                            readNext();
                            tBuilder.Append('/');
                            break;
                        case 'b':
                            readNext();
                            tBuilder.Append('\b');
                            break;
                        case 'f':
                            readNext();
                            tBuilder.Append('\f');
                            break;
                        case 'n':
                            readNext();
                            tBuilder.Append('\n');
                            break;
                        case 'r':
                            readNext();
                            tBuilder.Append('\r');
                            break;
                        case 't':
                            readNext();
                            tBuilder.Append('\t');
                            break;
                        case 'u':
                            {
                                readNext();

                                int tCurEscape = 0;
                                for (int i = 0; i < 4; ++i)
                                {
                                    c = peekNext();
                                    if (c >= '0' && c <= '9')
                                        tCurEscape = tCurEscape * 16 + (c - '0');
                                    else if (c >= 'a' && c <= 'z')
                                        tCurEscape = tCurEscape * 16 + (c - 'a');
                                    else if (c >= 'A' && c <= 'Z')
                                        tCurEscape = tCurEscape * 16 + (c - 'a');
                                    else
                                        throw new LexcialException(_Position, _Line, _Row,
                                            String.Format("expect digit character, but found '{0}'.", formatCharacter(c)));
                                    readNext();
                                }
                                tBuilder.Append((char)tCurEscape);
                            }
                            break;
                        default:
                            throw new LexcialException(_Position, _Line, _Row,
                                String.Format("unexpected escape character '{0}'.", formatCharacter(c)));
                    }
                }
                else
                {
                    readNext();
                    tBuilder.Append((char)c);
                }   
            }

            return tBuilder.ToString();
        }

        /// <summary>
        /// 当前的词法元素
        /// </summary>
        public Token CurrentToken
        {
            get
            {
                return _CurToken;
            }
        }

        /// <summary>
        /// 当前的数字字面量
        /// </summary>
        public double DigitLiteral
        {
            get
            {
                return _CurDigit;
            }
        }

        /// <summary>
        /// 当前的标识符
        /// </summary>
        public string Identify
        {
            get
            {
                return _CurId;
            }
        }

        /// <summary>
        /// 当前的标识符
        /// </summary>
        public string IdentifyLower
        {
            get
            {
                return _CurIdLower;
            }
        }

        /// <summary>
        /// 字符串字面量
        /// </summary>
        public string StringLiteral
        {
            get
            {
                return _CurString;
            }
        }

        /// <summary>
        /// 获取当前位置
        /// </summary>
        public int Position
        {
            get
            {
                return _Position;
            }
        }

        /// <summary>
        /// 获取行号
        /// </summary>
        public int Line
        {
            get
            {
                return _Line;
            }
        }

        /// <summary>
        /// 获取列号
        /// </summary>
        public int Row
        {
            get
            {
                return _Row;
            }
        }

        /// <summary>
        /// 推进词法分析器
        /// </summary>
        public void Next()
        {
            // 跳过所有的空白和注释
            while (skipBlankOrComment()) { }

            // 检查字符
            int c = peekNext();
            switch (c)
            {
                case -1:
                    _CurToken = Token.EOF;
                    return;
                case ';':
                    readNext();
                    _CurToken = Token.Semico;
                    return;
                case '(':
                    readNext();
                    _CurToken = Token.LeftBracket;
                    return;
                case ')':
                    readNext();
                    _CurToken = Token.RightBracket;
                    return;
                case '{':
                    readNext();
                    _CurToken = Token.LeftBrace;
                    return;
                case '}':
                    readNext();
                    _CurToken = Token.RightBrace;
                    return;
                case ',':
                    readNext();
                    _CurToken = Token.Comma;
                    return;
                case '+':
                    readNext();
                    _CurToken = Token.Plus;
                    return;
                case '-':
                    readNext();
                    _CurToken = Token.Minus;
                    return;
                case '*':
                    readNext();
                    if (peekNext() == '*')
                    {
                        readNext();
                        _CurToken = Token.Power;
                    }
                    else
                        _CurToken = Token.Mul;
                    return;
                case '/':
                    readNext();
                    _CurToken = Token.Div;
                    return;
                case '>':
                    readNext();
                    if (peekNext() == '=')  // >=
                    {
                        readNext();
                        _CurToken = Token.GreaterEqual;
                    }
                    else
                        _CurToken = Token.Greater;  // >
                    return;
                case '<':
                    readNext();
                    if (peekNext() == '=')  // <=
                    {
                        readNext();
                        _CurToken = Token.LessEqual;
                    }
                    else
                        _CurToken = Token.Less;  // <
                    return;
                case '=':
                    readNext();
                    if (peekNext() == '=')  // ==
                    {
                        readNext();
                        _CurToken = Token.Equal;
                    }
                    else
                        _CurToken = Token.Assign;  // =
                    return;
                case '!':
                    readNext();
                    if (peekNext() == '=')  // !=
                    {
                        readNext();
                        _CurToken = Token.NotEqual;
                    }
                    else
                        throw new LexcialException(_Position, _Line, _Row,
                            String.Format("expect '=', but found '{0}'.", formatCharacter(c)));
                    return;
                case '&':
                    readNext();
                    if (peekNext() == '&')  // &&
                    {
                        readNext();
                        _CurToken = Token.LogicalAnd;
                    }
                    else
                        throw new LexcialException(_Position, _Line, _Row,
                            String.Format("expect '&', but found '{0}'.", formatCharacter(c)));
                    return;
                case '|':
                    readNext();
                    if (peekNext() == '|')  // ||
                    {
                        readNext();
                        _CurToken = Token.LogicalOr;
                    }
                    else
                        throw new LexcialException(_Position, _Line, _Row,
                            String.Format("expect '|', but found '{0}'.", formatCharacter(c)));
                    return;
                case '"':
                    _CurToken = Token.StringLiteral;
                    _CurString = parseString();
                    return;
                default:
                    if (c >= '0' && c <= '9')
                    {
                        // 数字字面量
                        _CurToken = Token.DigitLiteral;
                        _CurDigit = parseDigit();
                        return;
                    }
                    else if (isIdentifyStart(c))
                    {
                        // 标识符
                        _CurToken = Token.Identifier;
                        
                        StringBuilder t = new StringBuilder();
                        while (isIdentifyCharacter(c))
                        {
                            readNext();
                            t.Append((char)c);
                            c = peekNext();
                        }
                        _CurId = t.ToString();
                        _CurIdLower = _CurId.ToLower();

                        // 转换为关键词
                        for (int i = 0; i < KeywordList.Length; ++i)
                        {
                            if (KeywordList[i] == _CurIdLower)
                            {
                                _CurToken = Token.Is + i;
                                break;
                            }   
                        }
                        return;
                    }
                    else
                    {
                        throw new LexcialException(_Position, _Line, _Row + 1,
                            String.Format("unexpected character '{0}'.", formatCharacter(c)));
                    }
            }
        }

        /// <summary>
        /// 格式化当前Token
        /// </summary>
        /// <returns>格式化结果</returns>
        public string FormatCurrentToken()
        {
            switch (_CurToken)
            {
                case Token.DigitLiteral:
                    return _CurDigit.ToString();
                case Token.Identifier:
                    return "\"" + _CurId.ToString() + "\"";
                default:
                    return FormatToken(_CurToken);
            }
        }

        public Lexer(TextReader Reader)
        {
            _Reader = Reader;
            readNext();  // 填充缓冲区
        }
    }
}
