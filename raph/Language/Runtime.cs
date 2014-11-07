using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 运行时
    /// </summary>
    public class Runtime
    {
        private static readonly RuntimeValue[] EmptyArgs = new RuntimeValue[0];
        private static readonly RuntimeValue DefaultForStepSize = new RuntimeValue.Digit(1);
        private static readonly RuntimeValue DefaultForStepSizeReverse = new RuntimeValue.Digit(-1);

        private RuntimeContext _RootContext = new RuntimeContext(null);

        /// <summary>
        /// 根执行环境
        /// </summary>
        public RuntimeContext RootContext
        {
            get
            {
                return _RootContext;
            }
        }

        // 初始化根执行环境
        // 对内置常量、函数进行初始化
        private void initRootContext()
        {
            RootContext.Register("pi", Math.PI);
            RootContext.Register("e", Math.E);

            // === func(digit)->digit 形式 ===
            string[] MathFuncName = new string[] {
                "sin", "cos", "tan", 
                "sinh", "cosh", "tanh",
                "asin", "acos", "atan",
                "exp", "ln", "log10",
                "sqrt", "abs",
                "round", "trunc",
                "floor", "ceil"
            };
            Func<double, double>[] MathFuncList = new Func<double,double>[] {
                Math.Sin, Math.Cos, Math.Tan,
                Math.Sinh, Math.Cosh, Math.Tanh,
                Math.Asin, Math.Acos, Math.Atan,
                Math.Exp, Math.Log, Math.Log10,
                Math.Sqrt, Math.Abs,
                Math.Round, Math.Truncate,
                Math.Floor, Math.Ceiling
            };
            for (int i = 0; i < MathFuncName.Length; ++i)
            {
                Func<double, double> tFunc = MathFuncList[i];
                RootContext.Register(MathFuncName[i], (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
                {
                    return new RuntimeValue.Digit(tFunc(Args[0].CastTo<double>()));
                }, 1);
            }

            // === 其他类型func ===
            RootContext.Register("atan2", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                return new RuntimeValue.Digit(Math.Atan2(Args[0].CastTo<double>(), Args[1].CastTo<double>()));
            }, 2);
            RootContext.Register("log2", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                return new RuntimeValue.Digit(Math.Log(Args[0].CastTo<double>(), 2));
            }, 1);
            RootContext.Register("max", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                return new RuntimeValue.Digit(Math.Max(Args[0].CastTo<double>(), Args[1].CastTo<double>()));
            }, 2);
            RootContext.Register("min", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                return new RuntimeValue.Digit(Math.Min(Args[0].CastTo<double>(), Args[1].CastTo<double>()));
            }, 2);
            RootContext.Register("sgn", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                return new RuntimeValue.Digit(Math.Sign(Args[0].CastTo<double>()));
            }, 1);

            RootContext.Register("dot", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                if (Args[0].ValueType != RuntimeValueType.Tuple)
                    throw new ArgumentException("arg 1 require type tuple(2).");
                RuntimeValue.Tuple tTuple = (RuntimeValue.Tuple)Args[0];
                if (tTuple.Value.Length != 2)
                    throw new ArgumentException("arg 1 require type tuple(2).");
                double tX = tTuple.Value[0].CastTo<double>();
                double tY = tTuple.Value[1].CastTo<double>();
                return new RuntimeValue.Digit(tX * tX + tY * tY);
            }, 1);
            RootContext.Register("cross", (ExternalFunctionHandler)delegate(RuntimeContext Context, RuntimeValue[] Args)
            {
                if (Args[0].ValueType != RuntimeValueType.Tuple)
                    throw new ArgumentException("arg 1 require type tuple(2).");
                if (Args[1].ValueType != RuntimeValueType.Tuple)
                    throw new ArgumentException("arg 2 require type tuple(2).");
                RuntimeValue.Tuple tTuple1 = (RuntimeValue.Tuple)Args[0];
                RuntimeValue.Tuple tTuple2 = (RuntimeValue.Tuple)Args[1];
                if (tTuple1.Value.Length != 2)
                    throw new ArgumentException("arg 1 require type tuple(2).");
                if (tTuple2.Value.Length != 2)
                    throw new ArgumentException("arg 2 require type tuple(2).");
                double tX1 = tTuple1.Value[0].CastTo<double>();
                double tY1 = tTuple1.Value[1].CastTo<double>();
                double tX2 = tTuple2.Value[0].CastTo<double>();
                double tY2 = tTuple2.Value[1].CastTo<double>();
                return new RuntimeValue.Digit(tX1 * tY2 - tX2 * tY1);
            }, 2);
        }

        // 调用函数
        // 不处理异常转换
        private RuntimeValue doCallOperation(RuntimeContext Context, RuntimeValue Target, ASTNode.ArgList CallArgs)
        {
            // 准备参数
            RuntimeValue[] pArgs = EmptyArgs;
            if (CallArgs.Args.Count > 0)
            {
                pArgs = new RuntimeValue[CallArgs.Args.Count];
                for (int i = 0; i < CallArgs.Args.Count; ++i)
                {
                    // 计算参数
                    pArgs[i] = execExpressionAST(Context, CallArgs.Args[i]);
                }
            }

            // 调用函数
            return Target.ApplyCallOperator(Context, pArgs);
        }

        // 执行一个表达式语法树
        // 返回执行结果
        // 处理异常转换
        private RuntimeValue execExpressionAST(RuntimeContext Context, ASTNode.Expression AST)
        {
            switch (AST.Type)
            {
                case ASTNode.ASTType.BinaryExpression:
                    {
                        ASTNode.BinaryExpression tBinaryExpression = (ASTNode.BinaryExpression)AST;

                        // 特殊处理assign操作
                        if (tBinaryExpression.BinaryOperator == BinaryOp.Assign)
                        {
                            if (tBinaryExpression.Left.Type != ASTNode.ASTType.SymbolExpression)
                            {
                                throw new RuntimeException(tBinaryExpression.Left.LineNumber,
                                    String.Format("left hand expression of assignment expression must be a symbol."));
                            }

                            // 进行赋值操作
                            ASTNode.SymbolExpression tLeftHand = (ASTNode.SymbolExpression)tBinaryExpression.Left;
                            try
                            {
                                RuntimeValue tRightVar = execExpressionAST(Context, tBinaryExpression.Right);
                                Context[tLeftHand.IdentifierLower] = tRightVar;
                                return tRightVar;
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new RuntimeException(tBinaryExpression.LineNumber,
                                    String.Format("identifier \"{0}\" is not defined.", tLeftHand.Identifier));
                            }
                        }
                        else if (tBinaryExpression.BinaryOperator == BinaryOp.LogicalAnd)  // 特殊处理&&操作
                        {
                            RuntimeValue tLeftVar = execExpressionAST(Context, tBinaryExpression.Left);
                            if (tLeftVar.ValueType != RuntimeValueType.Boolean)
                                throw new RuntimeException(tBinaryExpression.Left.LineNumber,
                                    String.Format("left hand expression of LogicAnd operator must return boolean."));
                            RuntimeValue.Boolean tLeftBoolean = (RuntimeValue.Boolean)tLeftVar;
                            if (tLeftBoolean.Value == false)
                                return tLeftBoolean;  // 截断

                            RuntimeValue tRightVar = execExpressionAST(Context, tBinaryExpression.Right);
                            if (tRightVar.ValueType != RuntimeValueType.Boolean)
                                throw new RuntimeException(tBinaryExpression.Right.LineNumber,
                                    String.Format("right hand expression of LogicAnd operator must return boolean."));
                            return tRightVar;
                        }
                        else if (tBinaryExpression.BinaryOperator == BinaryOp.LogicalOr)
                        {
                            RuntimeValue tLeftVar = execExpressionAST(Context, tBinaryExpression.Left);
                            if (tLeftVar.ValueType != RuntimeValueType.Boolean)
                                throw new RuntimeException(tBinaryExpression.Left.LineNumber,
                                    String.Format("left hand expression of LogicOr operator must return boolean."));
                            RuntimeValue.Boolean tLeftBoolean = (RuntimeValue.Boolean)tLeftVar;
                            if (tLeftBoolean.Value == true)
                                return tLeftBoolean;  // 截断

                            RuntimeValue tRightVar = execExpressionAST(Context, tBinaryExpression.Right);
                            if (tRightVar.ValueType != RuntimeValueType.Boolean)
                                throw new RuntimeException(tBinaryExpression.Right.LineNumber,
                                    String.Format("right hand expression of LogicOr operator must return boolean."));
                            return tRightVar;
                        }
                        else
                        {
                            RuntimeValue tLeftVar = execExpressionAST(Context, tBinaryExpression.Left);
                            RuntimeValue tRightVar = execExpressionAST(Context, tBinaryExpression.Right);
                            try
                            {
                                return tLeftVar.ApplyBinaryOperator(tBinaryExpression.BinaryOperator, tRightVar);
                            }
                            catch (OperationNotSupport)
                            {
                                throw new RuntimeException(tBinaryExpression.LineNumber,
                                    String.Format("can't perform {0} operation on type {1} and type {2}.", tBinaryExpression.BinaryOperator, tLeftVar.TypeToString(), tRightVar.TypeToString()));
                            }
                        }
                    }
                case ASTNode.ASTType.UnaryExpression:
                    {
                        ASTNode.UnaryExpression tUnaryExpression = (ASTNode.UnaryExpression)AST;
                        RuntimeValue tRightVar = execExpressionAST(Context, tUnaryExpression.Right);
                        try
                        {
                            return tRightVar.ApplyUnaryOperator(tUnaryExpression.UnaryOperator);
                        }
                        catch (OperationNotSupport)
                        {
                            throw new RuntimeException(tUnaryExpression.LineNumber,
                                String.Format("can't perform {0} operation on type {1}.", tUnaryExpression.UnaryOperator, tRightVar.TypeToString()));
                        }
                    }
                case ASTNode.ASTType.DigitLiteral:
                    {
                        ASTNode.DigitLiteral tDigitLiteral = (ASTNode.DigitLiteral)AST;
                        return new RuntimeValue.Digit(tDigitLiteral.Value);
                    }
                case ASTNode.ASTType.StringLiteral:
                    {
                        ASTNode.StringLiteral tStringLiteral = (ASTNode.StringLiteral)AST;
                        return new RuntimeValue.String(tStringLiteral.Value);
                    }
                case ASTNode.ASTType.BooleanLiteral:
                    {
                        ASTNode.BooleanLiteral tBooleanLiteral = (ASTNode.BooleanLiteral)AST;
                        return new RuntimeValue.Boolean(tBooleanLiteral.Value);
                    }
                case ASTNode.ASTType.CallExpression:
                    {
                        ASTNode.CallExpression tCallExpression = (ASTNode.CallExpression)AST;
                        RuntimeValue tCallTarget;
                        try
                        {
                            // 获取被调用对象
                            tCallTarget = Context[tCallExpression.IdentifierLower];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new RuntimeException(tCallExpression.LineNumber,
                                String.Format("identifier \"{0}\" is not defined.", tCallExpression.Identifier));
                        }
                        try
                        {
                            // 执行调用操作
                            return doCallOperation(Context, tCallTarget, tCallExpression.Args);
                        }
                        catch (OperationNotSupport)  // 操作不支持
                        {
                            throw new RuntimeException(tCallExpression.LineNumber,
                                String.Format("identifier \"{0}\" is not callable.", tCallExpression.Identifier));
                        }
                        catch (ArgumentCountMismatch e)  // 参数数量不匹配
                        {
                            throw new RuntimeException(tCallExpression.LineNumber,
                                String.Format("function \"{0}\" requires {1} arg(s), but {2} arg(s) are given.", tCallExpression.Identifier, e.ArgumentRequired, e.ArgumentGiven));
                        }
                        catch (Exception e)  // 一般性错误
                        {
                            throw new RuntimeException(tCallExpression.LineNumber,
                                String.Format("function \"{0}\" throws exception: {1}", tCallExpression.Identifier, e.Message));
                        }
                    }
                case ASTNode.ASTType.SymbolExpression:
                    {
                        ASTNode.SymbolExpression tSymbolExpression = (ASTNode.SymbolExpression)AST;
                        try
                        {
                            // 获取值
                            return Context[tSymbolExpression.IdentifierLower];
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new RuntimeException(tSymbolExpression.LineNumber,
                                String.Format("identifier \"{0}\" is not defined.", tSymbolExpression.Identifier));
                        }
                    }
                case ASTNode.ASTType.TupleExpression:
                    {
                        ASTNode.TupleExpression tTupleExpression = (ASTNode.TupleExpression)AST;
                        RuntimeValue[] tTupleValues = new RuntimeValue[tTupleExpression.Args.Count];

                        for (int i = 0; i < tTupleExpression.Args.Count; ++i)
                        {
                            tTupleValues[i] = execExpressionAST(Context, tTupleExpression.Args[i]);
                        }
                        return new RuntimeValue.Tuple(tTupleValues);
                    }
                default:
                    throw new RuntimeException(AST.LineNumber, "internal error, current syntax not implemented.");
            }
        }

        // 执行一个语句块
        // 处理异常转换
        private void execBlockAST(RuntimeContext Context, ASTNode.StatementList AST)
        {
            foreach (ASTNode.Statement s in AST.Statements)
            {
                switch (s.Type)
                {
                    case ASTNode.ASTType.Initialization:
                        {
                            ASTNode.Initialization tInitialization = (ASTNode.Initialization)s;
                            RuntimeValue tResult = execExpressionAST(Context, tInitialization.AssignmentExpression);
                            Context.Set(tInitialization.IdentifierLower, tResult);  // 不抛出异常
                        }
                        break;
                    case ASTNode.ASTType.Assignment:
                        {
                            ASTNode.Assignment tAssignment = (ASTNode.Assignment)s;
                            RuntimeValue tResult = execExpressionAST(Context, tAssignment.AssignmentExpression);
                            try
                            {
                                Context[tAssignment.IdentifierLower] = tResult;
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new RuntimeException(s.LineNumber,
                                    String.Format("identifier \"{0}\" is not defined.", tAssignment.Identifier));
                            }
                        }
                        break;
                    case ASTNode.ASTType.Call:
                        {
                            ASTNode.Call tCall = (ASTNode.Call)s;
                            RuntimeValue tCallTarget;
                            try
                            {
                                // 获取被调用对象
                                tCallTarget = Context[tCall.IdentifierLower];
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new RuntimeException(s.LineNumber,
                                    String.Format("identifier \"{0}\" is not defined.", tCall.Identifier));
                            }
                            try
                            {
                                // 执行调用操作
                                doCallOperation(Context, tCallTarget, tCall.Args);
                            }
                            catch (OperationNotSupport)  // 操作不支持
                            {
                                throw new RuntimeException(s.LineNumber,
                                    String.Format("identifier \"{0}\" is not callable.", tCall.Identifier));
                            }
                            catch (ArgumentCountMismatch e)  // 参数数量不匹配
                            {
                                throw new RuntimeException(s.LineNumber,
                                    String.Format("function \"{0}\" requires {1} arg(s), but {2} arg(s) are given.", tCall.Identifier, e.ArgumentRequired, e.ArgumentGiven));
                            }
                            catch (Exception e)  // 一般性错误
                            {
                                throw new RuntimeException(s.LineNumber,
                                    String.Format("function \"{0}\" throws exception: {1}", tCall.Identifier, e.Message));
                            }
                        }
                        break;
                    case ASTNode.ASTType.ForStatement:
                        {
                            ASTNode.ForStatement tForStatement = (ASTNode.ForStatement)s;

                            // 计算from、to
                            RuntimeValue tFromResult = execExpressionAST(Context, tForStatement.FromExpression);
                            RuntimeValue tToResult = execExpressionAST(Context, tForStatement.ToExpression);

                            // 检查方向
                            bool bIncreasing;
                            try
                            {
                                // 执行比较过程
                                RuntimeValue tTest = tFromResult.ApplyBinaryOperator(BinaryOp.LessEqual, tToResult);

                                // 赋予初值：若from <= to，则Step = 1，否则Step = -1
                                if (tTest.ValueType == RuntimeValueType.Boolean)
                                {
                                    RuntimeValue.Boolean tBoolean = (RuntimeValue.Boolean)tTest;
                                    bIncreasing = tBoolean.Value;
                                }
                                else
                                    throw new RuntimeException(s.LineNumber, "the LessEqual operation on from and or expression should return a boolean type.");
                            }
                            catch (OperationNotSupport)
                            {
                                throw new RuntimeException(s.LineNumber, "can't apply LessEqual operator on result of from and or expression.");
                            }

                            // 计算step或者给予初值
                            RuntimeValue tStepResult = null;
                            if (tForStatement.StepExpression != null)
                                tStepResult = execExpressionAST(Context, tForStatement.StepExpression);
                            else
                                tStepResult = bIncreasing ? DefaultForStepSize : DefaultForStepSizeReverse;

                            // 在For循环的上下文中设置循环变量
                            Context.Set(tForStatement.IdentifierLower, tFromResult);

                            // 执行for循环
                            while (true)
                            {
                                // 获取循环变量
                                RuntimeValue tForVar;
                                try
                                {
                                    tForVar = Context[tForStatement.IdentifierLower];
                                }
                                catch (KeyNotFoundException)
                                {
                                    throw new RuntimeException(s.LineNumber,
                                        String.Format("loop variable \"{0}\" is not defined.", tForStatement.Identifier));
                                }

                                // 检查循环是否结束
                                if (bIncreasing)
                                {
                                    try
                                    {
                                        RuntimeValue tTest = tForVar.ApplyBinaryOperator(BinaryOp.Greater, tToResult);
                                        if (tTest.ValueType == RuntimeValueType.Boolean)
                                        {
                                            RuntimeValue.Boolean tBoolean = (RuntimeValue.Boolean)tTest;
                                            if (tBoolean.Value)
                                                break;
                                        }
                                        else
                                            throw new RuntimeException(s.LineNumber, "the Greater operation on loop variable and to expression should return a boolean type.");
                                    }
                                    catch (OperationNotSupport)
                                    {
                                        throw new RuntimeException(s.LineNumber, "can't apply Greater operator on loop variable and to expression.");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        RuntimeValue tTest = tForVar.ApplyBinaryOperator(BinaryOp.Less, tToResult);
                                        if (tTest.ValueType == RuntimeValueType.Boolean)
                                        {
                                            RuntimeValue.Boolean tBoolean = (RuntimeValue.Boolean)tTest;
                                            if (tBoolean.Value)
                                                break;
                                        }
                                        else
                                            throw new RuntimeException(s.LineNumber, "the Less operation on loop variable and to expression should return a boolean type.");
                                    }
                                    catch (OperationNotSupport)
                                    {
                                        throw new RuntimeException(s.LineNumber, "can't apply Less operator on loop variable and to expression.");
                                    }
                                }

                                // 执行函数体
                                execBlockAST(Context, tForStatement.ExecBlock);

                                // Step计数
                                try
                                {
                                    Context.Set(tForStatement.IdentifierLower, tForVar.ApplyBinaryOperator(BinaryOp.Plus, tStepResult));
                                }
                                catch (OperationNotSupport)
                                {
                                    throw new RuntimeException(s.LineNumber, "can't apply Plus operator on loop variable and step expression.");
                                }
                            }
                        }
                        break;
                    case ASTNode.ASTType.WhileStatement:
                        {
                            ASTNode.WhileStatement tWhileStatement = (ASTNode.WhileStatement)s;
                            while (true)
                            {
                                RuntimeValue tTest = execExpressionAST(Context, tWhileStatement.ConditionExpression);
                                if (tTest.ValueType != RuntimeValueType.Boolean)
                                    throw new RuntimeException(tWhileStatement.ConditionExpression.LineNumber,
                                        String.Format("the result of while condition expression must be a boolean."));
                                RuntimeValue.Boolean tBoolean = (RuntimeValue.Boolean)tTest;
                                if (tBoolean.Value)
                                    execBlockAST(Context, tWhileStatement.ExecBlock);
                                else
                                    break;
                            }
                        }
                        break;
                    case ASTNode.ASTType.IfStatement:
                        {
                            ASTNode.IfStatement tIfStatement = (ASTNode.IfStatement)s;
                            RuntimeValue tTest = execExpressionAST(Context, tIfStatement.ConditionExpression);
                            if (tTest.ValueType != RuntimeValueType.Boolean)
                                throw new RuntimeException(tIfStatement.ConditionExpression.LineNumber,
                                    String.Format("the result of if condition expression must be a boolean."));
                            RuntimeValue.Boolean tBoolean = (RuntimeValue.Boolean)tTest;
                            if (tBoolean.Value)  // then
                                execBlockAST(Context, tIfStatement.ThenBlock);
                            else
                            {
                                if (tIfStatement.ElseBlock != null)  // else
                                    execBlockAST(Context, tIfStatement.ElseBlock);
                            }
                        }
                        break;
                    default:
                        throw new RuntimeException(s.LineNumber, "internal error, current syntax not implemented.");
                }
            }
        }

        /// <summary>
        /// 执行语法树
        /// </summary>
        /// <param name="StatementList">语法树对象</param>
        public void ExecAST(ASTNode.StatementList AST)
        {
            execBlockAST(_RootContext, AST);
        }

        public Runtime()
        {
            initRootContext();
        }
    }
}
