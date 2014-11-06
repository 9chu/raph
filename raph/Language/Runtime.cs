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
        /// <summary>
        /// 空类型
        /// </summary>
        public struct None {}

        /// <summary>
        /// 二维向量类型
        /// </summary>
        public struct Vector2
        {
            public double x;
            public double y;

            public override string ToString()
            {
                return String.Format("({0}, {1})", x, y);
            }
        }

        /// <summary>
        /// 本地调用处理函数
        /// </summary>
        /// <param name="Context">运行时</param>
        /// <param name="Args">参数</param>
        /// <param name="LineNumber">执行操作所在的行数</param>
        /// <returns>执行结果，若为None类型则表示无返回值</returns>
        public delegate object NativeCallHandler(Runtime Context, object[] Args, int LineNumber);

        /// <summary>
        /// 参数辅助检查
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="FuncName">函数名</param>
        /// <param name="Args">参数列表</param>
        /// <param name="ArgIndex">下标</param>
        /// <param name="LineNumber">行号</param>
        /// <returns>目标类型值</returns>
        public static T ArgCheckHelper<T>(string FuncName, object[] Args, int ArgIndex, int LineNumber)
        {
            if (ArgIndex >= Args.Length || ArgIndex < 0)
                throw new RuntimeException(LineNumber, String.Format("{0}: insufficient args.", FuncName));
            else if (!(Args[ArgIndex] is T))
                throw new RuntimeException(LineNumber, String.Format("{0}: type of arg {1} dismatched.", FuncName, ArgIndex));
            else
                return (T)Args[ArgIndex];
        }

        /// <summary>
        /// 参数数量检测辅助函数
        /// </summary>
        /// <param name="FuncName">函数名</param>
        /// <param name="Args">参数列表</param>
        /// <param name="ArgCount">需要的参数数量</param>
        /// <param name="LineNumber">行号</param>
        public static void ArgCountCheckHelper(string FuncName, object[] Args, int ArgCount, int LineNumber)
        {
            if (ArgCount != Args.Length)
                throw new RuntimeException(LineNumber, String.Format("{0}: insufficient args, {1} arg(s) needed.", FuncName, ArgCount));
        }

        private Dictionary<string, object> _Environment = new Dictionary<string, object>();
        
        // 计算二元运算
        private object applyBinaryOperator(BinaryOp Operator, object Left, object Right, int LineNumber)
        {
            switch (Operator)
            {
                case BinaryOp.Plus:
                    if (Left is double)
                    {
                        if (Right is double)
                            return (double)Left + (double)Right;
                        else
                            throw new RuntimeException(LineNumber, 
                                String.Format("can't perform plus operator on type {0}.", Right.GetType().ToString()));
                    }
                    else if (Left is Vector2)
                    {
                        if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x += ((Vector2)Right).x;
                            tOrg.y += ((Vector2)Right).y;
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform plus operator on type {0}.", Right.GetType().ToString()));
                    }
                    else
                        throw new RuntimeException(LineNumber,
                            String.Format("can't perform plus operator on type {0}.", Left.GetType().ToString()));
                case BinaryOp.Minus:
                    if (Left is double)
                    {
                        if (Right is double)
                            return (double)Left - (double)Right;
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform minus operator on type {0}.", Right.GetType().ToString()));
                    }
                    else if (Left is Vector2)
                    {
                        if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x -= ((Vector2)Right).x;
                            tOrg.y -= ((Vector2)Right).y;
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform minus operator on type {0}.", Right.GetType().ToString()));
                    }
                    else
                        throw new RuntimeException(LineNumber,
                            String.Format("can't perform minus operator on type {0}.", Left.GetType().ToString()));
                case BinaryOp.Mul:
                    if (Left is double)
                    {
                        if (Right is double)
                            return (double)Left * (double)Right;
                        else if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Right;
                            tOrg.x *= (double)Left;
                            tOrg.y *= (double)Left;
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform mul operator on type {0}.", Right.GetType().ToString()));
                    }
                    else if (Left is Vector2)
                    {
                        if (Right is double)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x *= (double)Right;
                            tOrg.y *= (double)Right;
                            return tOrg;
                        }
                        else if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x *= ((Vector2)Right).x;
                            tOrg.y *= ((Vector2)Right).y;
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform mul operator on type {0}.", Right.GetType().ToString()));
                    }
                    else
                        throw new RuntimeException(LineNumber,
                            String.Format("can't perform mul operator on type {0}.", Left.GetType().ToString()));
                case BinaryOp.Div:
                    if (Left is double)
                    {
                        if (Right is double)
                            return (double)Left / (double)Right;
                        else if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Right;
                            tOrg.x = (double)Left / tOrg.x;
                            tOrg.y = (double)Left / tOrg.y;
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform div operator on type {0}.", Right.GetType().ToString()));
                    }
                    else if (Left is Vector2)
                    {
                        if (Right is double)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x /= (double)Right;
                            tOrg.y /= (double)Right;
                            return tOrg;
                        }
                        else if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x /= ((Vector2)Right).x;
                            tOrg.y /= ((Vector2)Right).y;
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform div operator on type {0}.", Right.GetType().ToString()));
                    }
                    else
                        throw new RuntimeException(LineNumber,
                            String.Format("can't perform div operator on type {0}.", Left.GetType().ToString()));
                case BinaryOp.Power:
                    if (Left is double)
                    {
                        if (Right is double)
                            return Math.Pow((double)Left, (double)Right);
                        else if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Right;
                            tOrg.x = Math.Pow((double)Left, tOrg.x);
                            tOrg.y = Math.Pow((double)Left, tOrg.y);
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform mul operator on type {0}.", Right.GetType().ToString()));
                    }
                    else if (Left is Vector2)
                    {
                        if (Right is double)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x = Math.Pow(tOrg.x, (double)Right);
                            tOrg.y = Math.Pow(tOrg.y, (double)Right);
                            return tOrg;
                        }
                        else if (Right is Vector2)
                        {
                            Vector2 tOrg = (Vector2)Left;
                            tOrg.x = Math.Pow(tOrg.x, ((Vector2)Right).x);
                            tOrg.y = Math.Pow(tOrg.y, ((Vector2)Right).y);
                            return tOrg;
                        }
                        else
                            throw new RuntimeException(LineNumber,
                                String.Format("can't perform mul operator on type {0}.", Right.GetType().ToString()));
                    }
                    else
                        throw new RuntimeException(LineNumber,
                            String.Format("can't perform mul operator on type {0}.", Left.GetType().ToString()));
                default:
                    throw new RuntimeException(LineNumber, "internal error.");
            }
        }

        // 计算一元运算
        private object applyUnaryOperator(UnaryOp Operator, object Right, int LineNumber)
        {
            switch (Operator)
            {
                case UnaryOp.Negative:
                    if (Right is double)
                        return -(double)Right;
                    else if (Right is Vector2)
                    {
                        Vector2 tOrg = (Vector2)Right;
                        tOrg.x = -tOrg.x;
                        tOrg.y = -tOrg.y;
                        return tOrg;
                    }
                    else
                        throw new RuntimeException(LineNumber, 
                            String.Format("can't perform negative operator on type {0}.", Right.GetType().ToString()));
                default:
                    throw new RuntimeException(LineNumber, "internal error.");
            }
        }

        // 执行call操作
        private object applyCallOperator(string Identifier, string IdentifierLower, ASTNode.ArgList Args, int LineNumber)
        {
            object pNativeFunc = fetchValueOfIdentifier(Identifier, IdentifierLower, LineNumber);
            if (pNativeFunc is NativeCallHandler == false)
                throw new RuntimeException(LineNumber, String.Format("\"{0}\" is not callable.", Identifier));
            
            NativeCallHandler pHandler = (NativeCallHandler)pNativeFunc;
            object[] pArgs = null;
            if (Args.Args.Count > 0)
            {
                pArgs = new object[Args.Args.Count];
                for (int i = 0; i < Args.Args.Count; ++i)
                {
                    // 计算参数
                    pArgs[i] = ExecExpression(Args.Args[i]);
                }
            }

            // 调用函数
            return pHandler(this, pArgs, LineNumber);
        }

        // 获取标识符的值
        private object fetchValueOfIdentifier(string Identifier, string IdentifierLower, int LineNumber)
        {
            object tRet;
            if (!_Environment.TryGetValue(IdentifierLower, out tRet))
                throw new RuntimeException(LineNumber, String.Format("\"{0}\" is not defined.", Identifier));
            return tRet;
        }

        // 计算元组的值
        private object calcuTupleValue(ASTNode.TupleExpression Expression, int LineNumber)
        {
            // 根据元组的成员数进行转换
            if (Expression.Args.Count < 2)
                throw new RuntimeException(LineNumber, "internal error.");
            else if (Expression.Args.Count == 2)
            {
                // 计算值
                object tX = ExecExpression(Expression.Args[0]);
                object tY = ExecExpression(Expression.Args[1]);
                if (!(tX is double) || !(tY is double))
                    throw new RuntimeException(LineNumber, "tuple element must be a digit.");
                return new Vector2 { x = (double)tX, y = (double)tY };
            }
            else
                throw new RuntimeException(LineNumber, "tuple element count must be two.");
        }
        
        // 执行for循环
        private void doForLoop(string Identifier, string IdentifierLower, object From, object To, object Step, ASTNode.StatementList Block, int LineNumber)
        {
            if (!(From is double))
                throw new RuntimeException(LineNumber, "from expression must return a digit.");
            if (!(To is double))
                throw new RuntimeException(LineNumber, "to expression must return a digit.");

            double tFrom = (double)From;
            double tTo = (double)To;

            // 设置默认步长
            if (Step == null)
            {
                if (tFrom <= tTo)
                    Step = 1.0;
                else if (tFrom >= tTo)
                    Step = -1.0;
            }

            if (!(Step is double))
                throw new RuntimeException(LineNumber, "step expression must return a digit.");

            double tStep = (double)Step;
            
            // 赋予初值
            if (!_Environment.ContainsKey(IdentifierLower))
                _Environment.Add(IdentifierLower, tFrom);
            else
                _Environment[IdentifierLower] = tFrom;

            // 执行for循环
            while (true)
            {
                // ！ 当前的值可能在循环体中被用户代码改变。
                object tCurrentObject = fetchValueOfIdentifier(Identifier, IdentifierLower, LineNumber);
                if (!(tCurrentObject is double))
                    throw new RuntimeException(LineNumber, "for iterator must be a digit.");

                double tCurrent = (double)tCurrentObject;
                
                // 检查循环是否结束
                if (tFrom <= tTo && tCurrent > tTo)
                    break;
                else if (tFrom > tTo && tCurrent < tTo)
                    break;

                // 执行函数体
                ExecBlock(Block);

                // Step计数
                // ！ 重新获取值
                tCurrentObject = fetchValueOfIdentifier(Identifier, IdentifierLower, LineNumber);
                if (!(tCurrentObject is double))
                    throw new RuntimeException(LineNumber, "for iterator must be a digit.");
                tCurrent = (double)tCurrentObject + tStep;

                // 设置值
                if (!_Environment.ContainsKey(IdentifierLower))
                    _Environment.Add(IdentifierLower, tCurrent);
                else
                    _Environment[IdentifierLower] = tCurrent;
            }
        }

        /// <summary>
        /// 全局环境表
        /// </summary>
        public IDictionary<string, object> Environment
        {
            get
            {
                return _Environment;
            }
        }

        /// <summary>
        /// 获取标识符的值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="Identifier">标识符</param>
        /// <returns>值</returns>
        public T FetchIdentifier<T>(string Identifier, int ContextLineNumber = 0)
        {
            object tValue = fetchValueOfIdentifier(Identifier, Identifier.ToLower(), ContextLineNumber);
            if (!(tValue is T))
                throw new RuntimeException(ContextLineNumber, String.Format("can't cast value to type {0}.", typeof(T).ToString()));
            return (T)tValue;
        }

        /// <summary>
        /// 注册空值
        /// </summary>
        /// <param name="Identifier">标识符</param>
        public void RegisterIdentifier(string Identifier)
        {
            _Environment.Add(Identifier.ToLower(), new None());
        }

        /// <summary>
        /// 注册一个数值
        /// </summary>
        /// <remarks>覆盖已有定义</remarks>
        /// <param name="Identifier">标识符</param>
        /// <param name="Value">数值</param>
        public void RegisterIdentifier(string Identifier, double Value)
        {
            string IdLower = Identifier.ToLower();
            if (_Environment.ContainsKey(IdLower))
                _Environment[IdLower] = Value;
            else
                _Environment.Add(IdLower, Value);
        }

        /// <summary>
        /// 注册一个二维向量
        /// </summary>
        /// <remarks>覆盖已有定义</remarks>
        /// <param name="Identifier">标识符</param>
        /// <param name="Value">数值</param>
        public void RegisterIdentifier(string Identifier, Vector2 Value)
        {
            string IdLower = Identifier.ToLower();
            if (_Environment.ContainsKey(IdLower))
                _Environment[IdLower] = Value;
            else
                _Environment.Add(IdLower, Value);
        }

        /// <summary>
        /// 注册原生函数
        /// </summary>
        /// <remarks>覆盖已有定义</remarks>
        /// <param name="Identifier">标识符</param>
        /// <param name="Handler">回调</param>
        public void RegisterIdentifier(string Identifier, NativeCallHandler Handler)
        {
            string IdLower = Identifier.ToLower();
            if (_Environment.ContainsKey(IdLower))
                _Environment[IdLower] = Handler;
            else
                _Environment.Add(IdLower, Handler);
        }

        /// <summary>
        /// 移除标识符
        /// </summary>
        /// <param name="Identifier">标识符</param>
        public bool RemoveIdentifier(string Identifier)
        {
            return _Environment.Remove(Identifier.ToLower());
        }

        /// <summary>
        /// 执行一个区块
        /// </summary>
        /// <param name="StatementList">语句列表语法树</param>
        public void ExecBlock(ASTNode.StatementList StatementList)
        {
            foreach (ASTNode.Statement s in StatementList.Statements)
            {
                switch (s.Type)
                {
                    case ASTNode.ASTType.Assignment:
                        {
                            ASTNode.Assignment tAssignment = (ASTNode.Assignment)s;
                            object tResult = ExecExpression(tAssignment.AssignmentExpression);
                            if (!_Environment.ContainsKey(tAssignment.IdentifierLower))
                                _Environment.Add(tAssignment.IdentifierLower, tResult);
                            else
                                _Environment[tAssignment.IdentifierLower] = tResult;
                        }
                        break;
                    case ASTNode.ASTType.Call:
                        {
                            ASTNode.Call tCall = (ASTNode.Call)s;
                            applyCallOperator(tCall.Identifier, tCall.IdentifierLower, tCall.Args, s.LineNumber);
                        }
                        break;
                    case ASTNode.ASTType.ForStatement:
                        {
                            ASTNode.ForStatement tForStatement = (ASTNode.ForStatement)s;
                            object tFromResult = ExecExpression(tForStatement.FromExpression);
                            object tToResult = ExecExpression(tForStatement.ToExpression);
                            object tStepResult = tForStatement.StepExpression == null ? null : ExecExpression(tForStatement.StepExpression);
                            doForLoop(
                                tForStatement.Identifier, 
                                tForStatement.IdentifierLower, 
                                tFromResult, 
                                tToResult, 
                                tStepResult, 
                                tForStatement.ExecBlock,
                                s.LineNumber
                                );
                        }
                        break;
                    default:
                        throw new RuntimeException(s.LineNumber, "internal error.");
                }
            }
        }

        /// <summary>
        /// 执行一个表达式
        /// </summary>
        /// <param name="Expression">表达式语法树</param>
        /// <returns>执行结果</returns>
        public object ExecExpression(ASTNode.Expression Expression)
        {
            switch (Expression.Type)
            {
                case ASTNode.ASTType.BinaryExpression:
                    {
                        ASTNode.BinaryExpression tBinaryExpression = (ASTNode.BinaryExpression)Expression;
                        object tLeftResult = ExecExpression(tBinaryExpression.Left);
                        object tRightResult = ExecExpression(tBinaryExpression.Right);
                        return applyBinaryOperator(tBinaryExpression.BinaryOperator, tLeftResult, tRightResult, Expression.LineNumber);
                    }
                case ASTNode.ASTType.UnaryExpression:
                    {
                        ASTNode.UnaryExpression tUnaryExpression = (ASTNode.UnaryExpression)Expression;
                        object tRightResult = ExecExpression(tUnaryExpression.Right);
                        return applyUnaryOperator(tUnaryExpression.UnaryOperator, tRightResult, Expression.LineNumber);
                    }
                case ASTNode.ASTType.DigitLiteral:
                    {
                        ASTNode.DigitLiteral tDigitLiteral = (ASTNode.DigitLiteral)Expression;
                        return tDigitLiteral.Value;
                    }
                case ASTNode.ASTType.CallExpression:
                    {
                        ASTNode.CallExpression tCallExpression = (ASTNode.CallExpression)Expression;
                        return applyCallOperator(
                            tCallExpression.Identifier, 
                            tCallExpression.IdentifierLower, 
                            tCallExpression.Args,
                            Expression.LineNumber
                            );
                    }
                case ASTNode.ASTType.SymbolExpression:
                    {
                        ASTNode.SymbolExpression tSymbolExpression = (ASTNode.SymbolExpression)Expression;
                        return fetchValueOfIdentifier(
                            tSymbolExpression.Identifier,
                            tSymbolExpression.IdentifierLower,
                            Expression.LineNumber
                            );
                    }
                case ASTNode.ASTType.TupleExpression:
                    {
                        ASTNode.TupleExpression tTupleExpression = (ASTNode.TupleExpression)Expression;
                        return calcuTupleValue(tTupleExpression, Expression.LineNumber);
                    }
                default:
                    throw new RuntimeException(Expression.LineNumber, "internal error.");
            }
        }

        public Runtime()
        {
            RegisterIdentifier("pi", Math.PI);
            RegisterIdentifier("e", Math.E);

            RegisterIdentifier("dot", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("dot", Args, 1, LineNumber);
                Vector2 tArg = ArgCheckHelper<Vector2>("dot", Args, 0, LineNumber);
                return tArg.x * tArg.x + tArg.y * tArg.y;
            });
            RegisterIdentifier("cross", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("cross", Args, 2, LineNumber);
                Vector2 tLeft = ArgCheckHelper<Vector2>("cross", Args, 0, LineNumber);
                Vector2 tRight = ArgCheckHelper<Vector2>("cross", Args, 1, LineNumber);
                return tLeft.x * tRight.y - tRight.x * tLeft.y;
            });
            RegisterIdentifier("sin", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("sin", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("sin", Args, 0, LineNumber);
                return Math.Sin(tArg);
            });
            RegisterIdentifier("cos", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("cos", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("cos", Args, 0, LineNumber);
                return Math.Cos(tArg);
            });
            RegisterIdentifier("tan", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("tan", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("tan", Args, 0, LineNumber);
                return Math.Tan(tArg);
            });
            RegisterIdentifier("sinh", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("sinh", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("sinh", Args, 0, LineNumber);
                return Math.Sinh(tArg);
            });
            RegisterIdentifier("cosh", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("cosh", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("cosh", Args, 0, LineNumber);
                return Math.Cosh(tArg);
            });
            RegisterIdentifier("tanh", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("tanh", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("tanh", Args, 0, LineNumber);
                return Math.Tanh(tArg);
            });
            RegisterIdentifier("asin", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("asin", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("asin", Args, 0, LineNumber);
                return Math.Asin(tArg);
            });
            RegisterIdentifier("acos", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("acos", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("acos", Args, 0, LineNumber);
                return Math.Acos(tArg);
            });
            RegisterIdentifier("atan", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("atan", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("atan", Args, 0, LineNumber);
                return Math.Atan(tArg);
            });
            RegisterIdentifier("atan2", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("atan2", Args, 2, LineNumber);
                double tY = ArgCheckHelper<double>("atan2", Args, 0, LineNumber);
                double tX = ArgCheckHelper<double>("atan2", Args, 1, LineNumber);
                return Math.Atan2(tY, tX);
            });
            RegisterIdentifier("sqrt", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("sqrt", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("sqrt", Args, 0, LineNumber);
                return Math.Sqrt(tArg);
            });
            RegisterIdentifier("exp", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("exp", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("exp", Args, 0, LineNumber);
                return Math.Exp(tArg);
            });
            RegisterIdentifier("ln", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("ln", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("ln", Args, 0, LineNumber);
                return Math.Log(tArg);
            });
            RegisterIdentifier("log2", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("log2", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("log2", Args, 0, LineNumber);
                return Math.Log(tArg, 2);
            });
            RegisterIdentifier("log10", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("log10", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("log10", Args, 0, LineNumber);
                return Math.Log10(tArg);
            });
            RegisterIdentifier("abs", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("abs", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("abs", Args, 0, LineNumber);
                return Math.Abs(tArg);
            });
            RegisterIdentifier("max", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("max", Args, 2, LineNumber);
                double tArg1 = ArgCheckHelper<double>("max", Args, 0, LineNumber);
                double tArg2 = ArgCheckHelper<double>("max", Args, 1, LineNumber);
                return Math.Max(tArg1, tArg2);
            });
            RegisterIdentifier("min", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("min", Args, 2, LineNumber);
                double tArg1 = ArgCheckHelper<double>("min", Args, 0, LineNumber);
                double tArg2 = ArgCheckHelper<double>("min", Args, 1, LineNumber);
                return Math.Min(tArg1, tArg2);
            });
            RegisterIdentifier("ceil", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("ceil", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("ceil", Args, 0, LineNumber);
                return Math.Ceiling(tArg);
            });
            RegisterIdentifier("floor", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("floor", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("floor", Args, 0, LineNumber);
                return Math.Floor(tArg);
            });
            RegisterIdentifier("round", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("round", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("round", Args, 0, LineNumber);
                return Math.Round(tArg);
            });
            RegisterIdentifier("trunc", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("trunc", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("trunc", Args, 0, LineNumber);
                return Math.Truncate(tArg);
            });
            RegisterIdentifier("sgn", (NativeCallHandler)delegate(Runtime Context, object[] Args, int LineNumber)
            {
                ArgCountCheckHelper("sgn", Args, 1, LineNumber);
                double tArg = ArgCheckHelper<double>("sgn", Args, 0, LineNumber);
                return Math.Sign(tArg);
            });
        }
    }
}
