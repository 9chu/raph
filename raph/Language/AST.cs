using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 二元运算符
    /// </summary>
    /// <remarks>顺序和Lexer.Token中定义一致</remarks>
    public enum BinaryOp
    {
        Plus,
        Minus,
        Mul,
        Div,
        Power,
        Greater,
        Less,
        GreaterEqual,
        LessEqual,
        Equal,
        NotEqual,
        LogicalAnd,
        LogicalOr,
        Assign
    }

    /// <summary>
    /// 一元运算符
    /// </summary>
    public enum UnaryOp
    {
        Negative,
        Not
    }

    public abstract class ASTNode
    {
        /// <summary>
        /// AST语法树类型
        /// </summary>
        public enum ASTType
        {
            NotImpl,
            StatementList,
            ArgList,

            // 语句块组成成分
            Initialization,
            Assignment,
            Call,
            ForStatement,
            WhileStatement,
            IfStatement,

            // 表达式组成成分
            BinaryExpression,
            UnaryExpression,
            DigitLiteral,
            StringLiteral,
            BooleanLiteral,
            CallExpression,
            SymbolExpression,
            TupleExpression
        }

        private ASTType _Type = ASTType.NotImpl;
        private int _LineNumber = 0;

        public ASTType Type
        {
            get { return _Type; }
        }

        public int LineNumber
        {
            get { return _LineNumber; }
        }

        public ASTNode(ASTType AType)
        {
            _Type = AType;
        }

        public ASTNode(ASTType AType, int LineNum)
        {
            _Type = AType;
            _LineNumber = LineNum;
        }

        #region 具体AST节点实现
        public class StatementList : ASTNode
        {
            private List<Statement> _Statements = new List<Statement>();

            public IList<Statement> Statements
            {
                get
                {
                    return _Statements;
                }
            }

            public StatementList()
                : base(ASTType.StatementList) { }
        }

        public class Statement : ASTNode
        {
            public Statement(ASTType AType, int LineNum)
                : base(AType, LineNum) { }
        }

        public class Initialization : Statement
        {
            private string _Identifier = String.Empty;
            private string _IdentifierLower = String.Empty;
            private Expression _Expression = null;

            public string Identifier
            {
                get
                {
                    return _Identifier;
                }
                set
                {
                    _Identifier = value;
                    _IdentifierLower = _Identifier.ToLower();
                }
            }

            public string IdentifierLower
            {
                get
                {
                    return _IdentifierLower;
                }
            }

            public Expression AssignmentExpression
            {
                get
                {
                    return _Expression;
                }
                set
                {
                    _Expression = value;
                }
            }

            public Initialization(int LineNum, string Id, Expression Expr)
                : base(ASTType.Initialization, LineNum)
            {
                Identifier = Id;
                AssignmentExpression = Expr;
            }
        }

        public class Assignment : Statement
        {
            private string _Identifier = String.Empty;
            private string _IdentifierLower = String.Empty;
            private Expression _Expression = null;

            public string Identifier
            {
                get
                {
                    return _Identifier;
                }
                set
                {
                    _Identifier = value;
                    _IdentifierLower = _Identifier.ToLower();
                }
            }

            public string IdentifierLower
            {
                get
                {
                    return _IdentifierLower;
                }
            }

            public Expression AssignmentExpression
            {
                get
                {
                    return _Expression;
                }
                set
                {
                    _Expression = value;
                }
            }

            public Assignment(int LineNum, string Id, Expression Expr)
                : base(ASTType.Assignment, LineNum)
            {
                Identifier = Id;
                AssignmentExpression = Expr;
            }
        }

        public class Call : Statement
        {
            private string _Identifier = String.Empty;
            private string _IdentifierLower = String.Empty;
            private ArgList _ArgList = null;

            public string Identifier
            {
                get
                {
                    return _Identifier;
                }
                set
                {
                    _Identifier = value;
                    _IdentifierLower = _Identifier.ToLower();
                }
            }

            public string IdentifierLower
            {
                get
                {
                    return _IdentifierLower;
                }
            }

            public ArgList Args
            {
                get
                {
                    return _ArgList;
                }
                set
                {
                    _ArgList = value;
                }
            }

            public Call(int LineNum, string Id, ArgList AL)
                : base(ASTType.Call, LineNum)
            {
                Identifier = Id;
                Args = AL;
            }
        }

        public class ForStatement : Statement
        {
            private string _Identifier = String.Empty;
            private string _IdentifierLower = String.Empty;
            private Expression _FromExpression = null;
            private Expression _ToExpression = null;
            private Expression _StepExpression = null;
            private StatementList _ExecBlock = null;

            public string Identifier
            {
                get
                {
                    return _Identifier;
                }
                set
                {
                    _Identifier = value;
                    _IdentifierLower = _Identifier.ToLower();
                }
            }

            public string IdentifierLower
            {
                get
                {
                    return _IdentifierLower;
                }
            }

            public Expression FromExpression
            {
                get
                {
                    return _FromExpression;
                }
                set
                {
                    _FromExpression = value;
                }
            }

            public Expression ToExpression
            {
                get
                {
                    return _ToExpression;
                }
                set
                {
                    _ToExpression = value;
                }
            }

            public Expression StepExpression
            {
                get
                {
                    return _StepExpression;
                }
                set
                {
                    _StepExpression = value;
                }
            }

            public StatementList ExecBlock
            {
                get
                {
                    return _ExecBlock;
                }
                set
                {
                    _ExecBlock = value;
                }
            }

            public ForStatement(int LineNum)
                : base(ASTType.ForStatement, LineNum) { }
        }

        public class WhileStatement : Statement
        {
            private Expression _ConditionExpression = null;
            private StatementList _ExecBlock = null;

            public Expression ConditionExpression
            {
                get
                {
                    return _ConditionExpression;
                }
                set
                {
                    _ConditionExpression = value;
                }
            }
            
            public StatementList ExecBlock
            {
                get
                {
                    return _ExecBlock;
                }
                set
                {
                    _ExecBlock = value;
                }
            }

            public WhileStatement(Expression ConditionExpr, StatementList Block, int LineNum)
                : base(ASTType.WhileStatement, LineNum)
            {
                _ConditionExpression = ConditionExpr;
                _ExecBlock = Block;
            }
        }

        public class IfStatement : Statement
        {
            private Expression _ConditionExpression = null;
            private StatementList _ThenBlock = null;
            private StatementList _ElseBlock = null;

            public Expression ConditionExpression
            {
                get
                {
                    return _ConditionExpression;
                }
                set
                {
                    _ConditionExpression = value;
                }
            }

            public StatementList ThenBlock
            {
                get
                {
                    return _ThenBlock;
                }
                set
                {
                    _ThenBlock = value;
                }
            }

            public StatementList ElseBlock
            {
                get
                {
                    return _ElseBlock;
                }
                set
                {
                    _ElseBlock = value;
                }
            }

            public IfStatement(Expression Condition, StatementList Then, StatementList Else, int LineNum)
                : base(ASTType.IfStatement, LineNum)
            {
                _ConditionExpression = Condition;
                _ThenBlock = Then;
                _ElseBlock = Else;
            }
        }

        public class ArgList : ASTNode
        {
            private List<Expression> _Args = new List<Expression>();

            public IList<Expression> Args
            {
                get
                {
                    return _Args;
                }
            }

            public ArgList()
                : base(ASTType.ArgList) { }
        }

        public class Expression : ASTNode
        {
            public Expression(ASTType AType, int LineNum)
                : base(AType, LineNum) { }
        }

        public class BinaryExpression : Expression
        {
            private BinaryOp _BinaryOperator;
            private Expression _Left;
            private Expression _Right;

            public BinaryOp BinaryOperator
            {
                get
                {
                    return _BinaryOperator;
                }
                set
                {
                    _BinaryOperator = value;
                }
            }

            public Expression Left
            {
                get
                {
                    return _Left;
                }
                set
                {
                    _Left = value;
                }
            }

            public Expression Right
            {
                get
                {
                    return _Right;
                }
                set
                {
                    _Right = value;
                }
            }

            public BinaryExpression(int LineNum, BinaryOp Opt, Expression LeftNode, Expression RightNode)
                : base(ASTType.BinaryExpression, LineNum)
            {
                BinaryOperator = Opt;
                Left = LeftNode;
                Right = RightNode;
            }
        }
        public class UnaryExpression : Expression
        {
            private UnaryOp _UnaryOperator;
            private Expression _Right;

            public UnaryOp UnaryOperator
            {
                get
                {
                    return _UnaryOperator;
                }
                set
                {
                    _UnaryOperator = value;
                }
            }

            public Expression Right
            {
                get
                {
                    return _Right;
                }
                set
                {
                    _Right = value;
                }
            }

            public UnaryExpression(int LineNum, UnaryOp Opt, Expression RightNode)
                : base(ASTType.UnaryExpression, LineNum)
            {
                UnaryOperator = Opt;
                Right = RightNode;
            }
        }

        public class DigitLiteral : Expression
        {
            private double _Value;

            public double Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                }
            }

            public DigitLiteral(int LineNum, double Val)
                : base(ASTType.DigitLiteral, LineNum)
            {
                Value = Val;
            }
        }

        public class StringLiteral : Expression
        {
            private string _Value;

            public string Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                }
            }

            public StringLiteral(int LineNum, string Val)
                : base(ASTType.StringLiteral, LineNum)
            {
                Value = Val;
            }
        }

        public class BooleanLiteral : Expression
        {
            private bool _Value;

            public bool Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                }
            }

            public BooleanLiteral(int LineNum, bool Val)
                : base(ASTType.BooleanLiteral, LineNum)
            {
                Value = Val;
            }
        }

        public class CallExpression : Expression
        {
            private string _Identifier = String.Empty;
            private string _IdentifierLower = String.Empty;
            private ArgList _ArgList = null;

            public string Identifier
            {
                get
                {
                    return _Identifier;
                }
                set
                {
                    _Identifier = value;
                    _IdentifierLower = _Identifier.ToLower();
                }
            }

            public string IdentifierLower
            {
                get
                {
                    return _IdentifierLower;
                }
            }

            public ArgList Args
            {
                get
                {
                    return _ArgList;
                }
                set
                {
                    _ArgList = value;
                }
            }

            public CallExpression(int LineNum, string Id, ArgList AL)
                : base(ASTType.CallExpression, LineNum)
            {
                Identifier = Id;
                Args = AL;
            }
        }

        public class SymbolExpression : Expression
        {
            private string _Identifier = String.Empty;
            private string _IdentifierLower = String.Empty;

            public string Identifier
            {
                get
                {
                    return _Identifier;
                }
                set
                {
                    _Identifier = value;
                    _IdentifierLower = _Identifier.ToLower();
                }
            }

            public string IdentifierLower
            {
                get
                {
                    return _IdentifierLower;
                }
            }

            public SymbolExpression(int LineNum, string Id)
                : base(ASTType.SymbolExpression, LineNum)
            {
                Identifier = Id;
            }
        }

        public class TupleExpression : Expression
        {
            private List<Expression> _Args = new List<Expression>();

            public IList<Expression> Args
            {
                get
                {
                    return _Args;
                }
            }

            public TupleExpression(int LineNum)
                : base(ASTType.TupleExpression, LineNum) { }
        }
        #endregion
    }
}
