using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 语法分析器
    /// </summary>
    public abstract class Syntax
    {
        /// <summary>
        /// 二元运算符优先级表
        /// </summary>
        /// <remarks>与Lexer.Token具有相同顺序</remarks>
        private static readonly int[] BinaryOperatorPriorityTable = new int[]
        {
            3, // Plus
            3, // Minus
            2, // Mul
            2, // Div
            1, // Power
            5, // Greater
            5, // Less
            5, // GreaterEqual
            5, // LessEqual
            6, // Equal
            6, // NotEqual
            7, // LogicalAnd
            8, // LogicalOr
            4  // Assign
        };

        /// <summary>
        /// 二元算符的最小优先级
        /// </summary>
        private static readonly int MaxBinaryOperatorPriority = 8;

        /// <summary>
        /// 匹配一个Token
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="TokenToMatch">需要匹配的Token</param>
        private static void MatchToken(Lexer Lex, Lexer.Token TokenToMatch)
        {
            if (Lex.CurrentToken != TokenToMatch)
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("expect {0}, but found {1}.", Lexer.FormatToken(TokenToMatch), Lex.FormatCurrentToken()));
            
            Lex.Next();
        }

        /// <summary>
        /// 匹配一个Identifier
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>ID（大小写）</returns>
        private static string MatchIdentifier(Lexer Lex)
        {
            if (Lex.CurrentToken != Lexer.Token.Identifier)
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("expect {0}, but found {1}.", Lexer.FormatToken(Lexer.Token.Identifier), Lex.FormatCurrentToken()));

            string tRet = Lex.Identify;
            Lex.Next();
            return tRet;
        }

        /// <summary>
        /// 尝试匹配一个Token，若成功则向后推进
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="TokenToMatch">需要匹配的Token</param>
        /// <returns>是否成功匹配</returns>
        private static bool TryMatchToken(Lexer Lex, Lexer.Token TokenToMatch)
        {
            if (Lex.CurrentToken != TokenToMatch)
                return false;

            Lex.Next();
            return true;
        }

        /// <summary>
        /// 尝试匹配一个Identifier，若成功则向后推进
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="Id">匹配到的Id</param>
        /// <returns>是否成功匹配</returns>
        private static bool TryMatchIdentifier(Lexer Lex, out string Id)
        {
            Id = String.Empty;

            if (Lex.CurrentToken != Lexer.Token.Identifier)
                return false;

            Id = Lex.Identify;
            Lex.Next();
            return true;
        }

        /// <summary>
        /// 解析语法树
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>语法树</returns>
        public static ASTNode.StatementList Parse(Lexer Lex)
        {
            Lex.Next();  // 预读取第一个词法单元

            ASTNode.StatementList tRet = ParseStatementList(Lex);
            if (Lex.CurrentToken != Lexer.Token.EOF)
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));

            return tRet;
        }

        /// <summary>
        /// 解析Statement
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="Result">解析到的语句</param>
        /// <returns>是否解析成功</returns>
        private static bool ParseStatement(Lexer Lex, out ASTNode.Statement Result)
        {
            if (Lex.CurrentToken == Lexer.Token.For)  // for_statement
                Result = ParseForStatement(Lex);
            else if (Lex.CurrentToken == Lexer.Token.While)  // while_statement
                Result = ParseWhileStatement(Lex);
            else if (Lex.CurrentToken == Lexer.Token.If)  // if_statement
                Result = ParseIfStatement(Lex);
            else if (Lex.CurrentToken == Lexer.Token.Identifier)  // assignment or call
                Result = ParseAssignmentOrCall(Lex);
            else
            {
                Result = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析一个块中的StatementList
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.StatementList ParseStatementList(Lexer Lex)
        {
            ASTNode.StatementList tRet = new ASTNode.StatementList();
            ASTNode.Statement tStatement;
            while (ParseStatement(Lex, out tStatement))
            {
                tRet.Statements.Add(tStatement);
            }
            return tRet;
        }

        /// <summary>
        /// 解析一个Block
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.StatementList ParseBlock(Lexer Lex)
        {
            ASTNode.StatementList tRet;

            if (TryMatchToken(Lex, Lexer.Token.Begin))  // begin
            {
                tRet = ParseStatementList(Lex);

                MatchToken(Lex, Lexer.Token.End);  // end

                return tRet;
            }
            else  // 单条语句
            {
                ASTNode.Statement tStatement;
                if (ParseStatement(Lex, out tStatement))
                {
                    tRet = new ASTNode.StatementList();
                    tRet.Statements.Add(tStatement);
                    return tRet;
                }
                else
                    throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                        String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
            }
        }

        /// <summary>
        /// 解析一个ArgList
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.ArgList ParseArgList(Lexer Lex)
        {
            ASTNode.ArgList tRet = new ASTNode.ArgList();
            if (Lex.CurrentToken != Lexer.Token.RightBracket) // 非空arglist
            {
                while (true)
                {
                    tRet.Args.Add(ParseExpression(Lex));
                    if (TryMatchToken(Lex, Lexer.Token.Comma))  // ','
                        continue;
                    else if (Lex.CurrentToken == Lexer.Token.RightBracket)  // peek ')'
                        break;
                    else
                        throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                            String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
                }
            }
            return tRet;
        }

        /// <summary>
        /// 解析For语句
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.ForStatement ParseForStatement(Lexer Lex)
        {
            ASTNode.ForStatement tRet = new ASTNode.ForStatement(Lex.Line);

            // for <identifier> from <expression> to <expression> {step <expression>} <block>
            MatchToken(Lex, Lexer.Token.For);
            tRet.Identifier = MatchIdentifier(Lex);
            MatchToken(Lex, Lexer.Token.From);
            tRet.FromExpression = ParseExpression(Lex);
            MatchToken(Lex, Lexer.Token.To);
            tRet.ToExpression = ParseExpression(Lex);
            if (TryMatchToken(Lex, Lexer.Token.Step))
                tRet.StepExpression = ParseExpression(Lex);
            tRet.ExecBlock = ParseBlock(Lex);

            return tRet;
        }

        /// <summary>
        /// 解析While语句
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.WhileStatement ParseWhileStatement(Lexer Lex)
        {
            int tLine = Lex.Line;

            // while <expression> <block>
            MatchToken(Lex, Lexer.Token.While);
            ASTNode.Expression tConditionExpression = ParseExpression(Lex);
            ASTNode.StatementList tExecBlock = ParseBlock(Lex);

            return new ASTNode.WhileStatement(tConditionExpression, tExecBlock, tLine);
        }
        
        /// <summary>
        /// 解析If语句
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.IfStatement ParseIfStatement(Lexer Lex)
        {
            // if <expr> <block> {else <block>}
            int tLine = Lex.Line;
            MatchToken(Lex, Lexer.Token.If);
            ASTNode.Expression tConditionExpression = ParseExpression(Lex);
            ASTNode.StatementList tThenBlock = ParseBlock(Lex);
            ASTNode.StatementList tElseBlock = null;
            if (TryMatchToken(Lex, Lexer.Token.Else))
                tElseBlock = ParseBlock(Lex);
            return new ASTNode.IfStatement(tConditionExpression, tThenBlock, tElseBlock, tLine);
        }

        /// <summary>
        /// 解析一条赋值或者调用指令
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.Statement ParseAssignmentOrCall(Lexer Lex)
        {
            int tLine = Lex.Line;
            string tIdentifier = MatchIdentifier(Lex);
            if (TryMatchToken(Lex, Lexer.Token.LeftBracket))  // call
            {
                ASTNode.Call tCall = new ASTNode.Call(tLine, tIdentifier, ParseArgList(Lex));

                MatchToken(Lex, Lexer.Token.RightBracket);  // ')'
                MatchToken(Lex, Lexer.Token.Semico);  // ';'
                return tCall;
            }
            else if (TryMatchToken(Lex, Lexer.Token.Assign))  // assignment
            {
                ASTNode.Assignment tAssign = new ASTNode.Assignment(tLine, tIdentifier, ParseExpression(Lex));

                MatchToken(Lex, Lexer.Token.Semico);  // ';'
                return tAssign;
            }
            else if (TryMatchToken(Lex, Lexer.Token.Is))  // initialization
            {
                ASTNode.Initialization tInit = new ASTNode.Initialization(tLine, tIdentifier, ParseExpression(Lex));

                MatchToken(Lex, Lexer.Token.Semico);  // ';'
                return tInit;
            }
            else
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
        }

        /// <summary>
        /// 解析表达式
        /// 
        /// 保证后续Token能产生一个表达式，否则抛出异常
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.Expression ParseExpression(Lexer Lex)
        {
            return ParseBinaryExpression(Lex, MaxBinaryOperatorPriority);
        }

        /// <summary>
        /// 解析二元表达式
        /// 
        /// 当优先级为0时退化到一元表达式
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="Priority">优先级</param>
        /// <returns>解析结果</returns>
        private static ASTNode.Expression ParseBinaryExpression(Lexer Lex, int Priority)
        {
            // 退化
            if (Priority == 0)
                return ParseUnaryExpression(Lex);

            // 递归解析左侧的表达式
            ASTNode.Expression tRet = ParseBinaryExpression(Lex, Priority - 1);

            // 检查是否为二元运算符
            if (Lex.CurrentToken >= Lexer.Token.Plus && (int)Lex.CurrentToken < (int)Lexer.Token.Plus + BinaryOperatorPriorityTable.Length)
            {
                int tPriority = BinaryOperatorPriorityTable[Lex.CurrentToken - Lexer.Token.Plus];
                if (tPriority > Priority) // 优先级不符，返回
                    return tRet;
                else
                {
                    // 低于左侧表达式二元算符的表达式已全部被解析，故可以安全的提升优先级。
                    // 此时能保证 Priority >= tPriority > 左侧表达式的二元算符优先级
                    Priority = tPriority;
                }   
            }
            else
                return tRet;

            // 循环解析右侧的表达式
            while (true)
            {
                // 检查下一个算符的优先级
                Lexer.Token tOpt = Lex.CurrentToken;
                if (!(tOpt >= Lexer.Token.Plus && (int)tOpt < (int)Lexer.Token.Plus + BinaryOperatorPriorityTable.Length &&
                    BinaryOperatorPriorityTable[Lex.CurrentToken - Lexer.Token.Plus] == Priority))
                {
                    break;
                }

                // 吃掉运算符
                Lex.Next();
                
                // 获取算符右侧
                ASTNode.Expression tRight = ParseBinaryExpression(Lex, Priority - 1);
                
                // 组合成二元AST树
                tRet = new ASTNode.BinaryExpression(Lex.Line, BinaryOp.Plus + (tOpt - Lexer.Token.Plus), tRet, tRight);
            }

            return tRet;
        }

        /// <summary>
        /// 解析一元表达式
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.Expression ParseUnaryExpression(Lexer Lex)
        {
            // 判断是否为一元表达式
            if (TryMatchToken(Lex, Lexer.Token.Minus))  // 一元负号
                return new ASTNode.UnaryExpression(Lex.Line, UnaryOp.Negative, ParseUnaryExpression(Lex));
            if (TryMatchToken(Lex, Lexer.Token.Not))  // 一元非号
                return new ASTNode.UnaryExpression(Lex.Line, UnaryOp.Not, ParseUnaryExpression(Lex));
            else
                return ParseAtomExpression(Lex);
        }

        /// <summary>
        /// 解析原子表达式
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.Expression ParseAtomExpression(Lexer Lex)
        {
            ASTNode.Expression tRet;
            if (Lex.CurrentToken == Lexer.Token.DigitLiteral)  // digit_literal
            {
                tRet = new ASTNode.DigitLiteral(Lex.Line, Lex.DigitLiteral);
                Lex.Next();
                return tRet;
            }
            else if (Lex.CurrentToken == Lexer.Token.StringLiteral)  // string_literal
            {
                tRet = new ASTNode.StringLiteral(Lex.Line, Lex.StringLiteral);
                Lex.Next();
                return tRet;
            }
            else if (Lex.CurrentToken == Lexer.Token.True)  // boolean_literal
            {
                tRet = new ASTNode.BooleanLiteral(Lex.Line, true);
                Lex.Next();
                return tRet;
            }
            else if (Lex.CurrentToken == Lexer.Token.False)  // boolean_literal
            {
                tRet = new ASTNode.BooleanLiteral(Lex.Line, false);
                Lex.Next();
                return tRet;
            }
            else if (Lex.CurrentToken == Lexer.Token.Identifier)  // symbol or call_expression
            {
                string tIdentifier = MatchIdentifier(Lex);

                // 检查下一个符号
                if (TryMatchToken(Lex, Lexer.Token.LeftBracket))  // '(' -- call_expression
                {
                    ASTNode.ArgList tArgList = ParseArgList(Lex);
                    MatchToken(Lex, Lexer.Token.RightBracket);  // ')'
                    return new ASTNode.CallExpression(Lex.Line, tIdentifier, tArgList);
                }
                else  // symbol
                    return new ASTNode.SymbolExpression(Lex.Line, tIdentifier);
            }
            else if (TryMatchToken(Lex, Lexer.Token.LeftBracket))  // '(' -- bracket_expression
            {
                tRet = ParseBracketExpression(Lex);

                MatchToken(Lex, Lexer.Token.RightBracket);  // ')'
                return tRet;
            }
            else
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
        }

        /// <summary>
        /// 解析括号表达式
        /// 
        /// 括号表达式不可为空，当仅有一个元素时表达为单值，当有多个元素时表达为向量
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode.Expression ParseBracketExpression(Lexer Lex)
        {
            // 先读取第一个表达式
            ASTNode.Expression tFirstExpr = ParseExpression(Lex);
            
            // 检查后续元素
            if (Lex.CurrentToken != Lexer.Token.RightBracket)
            {
                ASTNode.TupleExpression tList = new ASTNode.TupleExpression(Lex.Line);
                tList.Args.Add(tFirstExpr);

                while (true)
                {
                    MatchToken(Lex, Lexer.Token.Comma);  // ','
                    tList.Args.Add(ParseExpression(Lex));

                    if (Lex.CurrentToken == Lexer.Token.Comma)  // peek ','
                        continue;
                    if (Lex.CurrentToken == Lexer.Token.RightBracket)  // peek ')'
                        break;
                    else
                        throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                            String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
                }
                return tList;
            }
            else
                return tFirstExpr;
        }
    }
}
