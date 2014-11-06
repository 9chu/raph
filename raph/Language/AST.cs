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
        Power
    }

    /// <summary>
    /// 一元运算符
    /// </summary>
    public enum UnaryOp
    {
        Negative
    }

    public class ASTNode
    {
        /// <summary>
        /// AST语法树类型
        /// </summary>
        public enum ASTType
        {
            NotImpl,
            StatementList,
            ArgList,

            Assignment,
            Call,
            ForStatement,

            BinaryExpression,
            UnaryExpression,
            DigitLiteral,
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
    }

    public class ASTNode_StatementList : ASTNode
    {
        private List<ASTNode_Statement> _Statements = new List<ASTNode_Statement>();

        public IList<ASTNode_Statement> Statements
        {
            get
            {
                return _Statements;
            }
        }

        public ASTNode_StatementList()
            : base(ASTNode.ASTType.StatementList) { }
    }

    public class ASTNode_Statement : ASTNode
    {
        public ASTNode_Statement(ASTNode.ASTType AType, int LineNum)
            : base(AType, LineNum) { }
    }

    public class ASTNode_Assignment : ASTNode_Statement
    {
        private string _Identifier = String.Empty;
        private string _IdentifierLower = String.Empty;
        private ASTNode_Expression _Expression = null;

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

        public ASTNode_Expression Expression
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

        public ASTNode_Assignment(int LineNum, string Id, ASTNode_Expression Expr)
            : base(ASTNode.ASTType.Assignment, LineNum)
        {
            Identifier = Id;
            Expression = Expr;
        }
    }
    
    public class ASTNode_Call : ASTNode_Statement
    {
        private string _Identifier = String.Empty;
        private string _IdentifierLower = String.Empty;
        private ASTNode_ArgList _ArgList = null;

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

        public ASTNode_ArgList ArgList
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

        public ASTNode_Call(int LineNum, string Id, ASTNode_ArgList AL)
            : base(ASTNode.ASTType.Call, LineNum)
        {
            Identifier = Id;
            ArgList = AL;
        }
    }

    public class ASTNode_ForStatement : ASTNode_Statement
    {
        private string _Identifier = String.Empty;
        private string _IdentifierLower = String.Empty;
        private ASTNode_Expression _FromExpression = null;
        private ASTNode_Expression _ToExpression = null;
        private ASTNode_Expression _StepExpression = null;
        private ASTNode_StatementList _ExecBlock = null;

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

        public ASTNode_Expression FromExpression
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

        public ASTNode_Expression ToExpression
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

        public ASTNode_Expression StepExpression
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

        public ASTNode_StatementList ExecBlock
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

        public ASTNode_ForStatement(int LineNum)
            : base(ASTNode.ASTType.ForStatement, LineNum) { }
    }

    public class ASTNode_ArgList : ASTNode
    {
        private List<ASTNode_Expression> _Args = new List<ASTNode_Expression>();
        
        public IList<ASTNode_Expression> Args
        {
            get
            {
                return _Args;
            }
        }

        public ASTNode_ArgList()
            : base(ASTNode.ASTType.ArgList) { }
    }

    public class ASTNode_Expression : ASTNode
    {
        public ASTNode_Expression(ASTNode.ASTType AType, int LineNum)
            : base(AType, LineNum) { }
    }

    public class ASTNode_BinaryExpression : ASTNode_Expression
    {
        private BinaryOp _BinaryOperator;
        private ASTNode_Expression _Left;
        private ASTNode_Expression _Right;

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

        public ASTNode_Expression Left
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

        public ASTNode_Expression Right
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

        public ASTNode_BinaryExpression(int LineNum, BinaryOp Opt, ASTNode_Expression LeftNode, ASTNode_Expression RightNode)
            : base(ASTNode.ASTType.BinaryExpression, LineNum)
        {
            BinaryOperator = Opt;
            Left = LeftNode;
            Right = RightNode;
        }
    }

    public class ASTNode_UnaryExpression : ASTNode_Expression
    {
        private UnaryOp _UnaryOperator;
        private ASTNode_Expression _Right;

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
        
        public ASTNode_Expression Right
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

        public ASTNode_UnaryExpression(int LineNum, UnaryOp Opt, ASTNode_Expression RightNode)
            : base(ASTNode.ASTType.UnaryExpression, LineNum)
        {
            UnaryOperator = Opt;
            Right = RightNode;
        }
    }

    public class ASTNode_DigitLiteral : ASTNode_Expression
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

        public ASTNode_DigitLiteral(int LineNum, double Val)
            : base(ASTNode.ASTType.DigitLiteral, LineNum)
        {
            Value = Val;
        }
    }

    public class ASTNode_CallExpression : ASTNode_Expression
    {
        private string _Identifier = String.Empty;
        private string _IdentifierLower = String.Empty;
        private ASTNode_ArgList _ArgList = null;

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

        public ASTNode_ArgList ArgList
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

        public ASTNode_CallExpression(int LineNum, string Id, ASTNode_ArgList AL)
            : base(ASTNode.ASTType.CallExpression, LineNum)
        {
            Identifier = Id;
            ArgList = AL;
        }
    }

    public class ASTNode_SymbolExpression : ASTNode_Expression
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

        public ASTNode_SymbolExpression(int LineNum, string Id)
            : base(ASTNode.ASTType.SymbolExpression, LineNum)
        {
            Identifier = Id;
        }
    }

    public class ASTNode_TupleExpression : ASTNode_Expression
    {
        private List<ASTNode_Expression> _Args = new List<ASTNode_Expression>();

        public IList<ASTNode_Expression> Args
        {
            get
            {
                return _Args;
            }
        }

        public ASTNode_TupleExpression(int LineNum)
            : base(ASTNode.ASTType.TupleExpression, LineNum) { }
    }
}
