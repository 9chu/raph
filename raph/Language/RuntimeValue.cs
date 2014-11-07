using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 运行时值类型
    /// </summary>
    public enum RuntimeValueType
    {
        None,              // 空类型
        ExternalFunction,  // 外部函数
        Boolean,           // 逻辑
        Digit,             // 数字
        Tuple,             // 元组
        String             // 字符串
    }

    /// <summary>
    /// 外部函数句柄
    /// </summary>
    /// <param name="Context">运行时</param>
    /// <param name="Args">参数</param>
    /// <returns>执行结果，若为None类型则表示无返回值</returns>
    public delegate RuntimeValue ExternalFunctionHandler(RuntimeContext Context, RuntimeValue[] Args);

    /// <summary>
    /// 运行时值对象
    /// </summary>
    public abstract class RuntimeValue
    {
        private RuntimeValueType _ValueType;

        /// <summary>
        /// 获取对象的类型
        /// </summary>
        public RuntimeValueType ValueType
        {
            get
            {
                return _ValueType;
            }
        }

        /// <summary>
        /// 一元运算符操作
        /// </summary>
        /// <param name="OperatorType">运算符</param>
        /// <returns>返回新值</returns>
        public virtual RuntimeValue ApplyUnaryOperator(UnaryOp OperatorType)
        {
            throw new OperationNotSupport();
        }

        /// <summary>
        /// 二元运算符操作
        /// </summary>
        /// <param name="OperatorType">运算符</param>
        /// <param name="RightValueRef">右侧运算结果引用</param>
        /// <returns>返回新值</returns>
        public virtual RuntimeValue ApplyBinaryOperator(BinaryOp OperatorType, RuntimeValue RightValueRef)
        {
            if (RightValueRef.ValueType == RuntimeValueType.String && OperatorType == BinaryOp.Plus)
            {
                String r = (String)RightValueRef;
                return new String(DataToString() + r.Value);
            }
            else if (RightValueRef.ValueType == RuntimeValueType.Tuple)
            {
                Tuple r = (Tuple)RightValueRef;
                RuntimeValue[] t = new RuntimeValue[r.Value.Length];
                for (int i = 0; i < t.Length; ++i)
                {
                    t[i] = ApplyBinaryOperator(OperatorType, r.Value[i]);
                }
                return new Tuple(t);
            }

            throw new OperationNotSupport();
        }

        /// <summary>
        /// 执行调用操作
        /// </summary>
        /// <param name="Context">上下文</param>
        /// <param name="Args">参数</param>
        /// <returns>返回值</returns>
        public virtual RuntimeValue ApplyCallOperator(RuntimeContext Context, RuntimeValue[] Args)
        {
            throw new OperationNotSupport();
        }

        /// <summary>
        /// 将类型表示为字符串
        /// </summary>
        /// <returns>类型</returns>
        public virtual string TypeToString()
        {
            switch (_ValueType)
            {
                case RuntimeValueType.None:
                    return "none";
                case RuntimeValueType.ExternalFunction:
                    return "extfunction";
                case RuntimeValueType.Boolean:
                    return "boolean";
                case RuntimeValueType.Digit:
                    return "digit";
                case RuntimeValueType.Tuple:
                    return "tuple";
                case RuntimeValueType.String:
                    return "string";
                default:
                    return "unknown";
            }
        }

        /// <summary>
        /// 将数据表示为字符串
        /// </summary>
        /// <returns>数据</returns>
        public virtual string DataToString()
        {
            return System.String.Format("<{0}>", TypeToString());
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>转换结果</returns>
        public T CastTo<T>()
        {
            if (typeof(T) == typeof(bool))
            {
                if (ValueType != RuntimeValueType.Boolean)
                    throw new InvalidCastException(
                        System.String.Format("can't cast type {0} to boolean.", TypeToString()));
                Boolean tDigit = (Boolean)this;
                return (T)(object)tDigit.Value;
            }
            else if (typeof(T) == typeof(double))
            {
                if (ValueType != RuntimeValueType.Digit)
                    throw new InvalidCastException(
                        System.String.Format("can't cast type {0} to digit.", TypeToString()));
                Digit tDigit = (Digit)this;
                return (T)(object)tDigit.Value;
            }
            else if (typeof(T) == typeof(string))
            {
                if (ValueType != RuntimeValueType.String)
                    throw new InvalidCastException(
                        System.String.Format("can't cast type {0} to string.", TypeToString()));
                String tString = (String)this;
                return (T)(object)tString.Value;
            }
            else
                throw new InvalidCastException("bad cast operation.");
        }

        public RuntimeValue(RuntimeValueType ValueType)
        {
            _ValueType = ValueType;
        }

        #region 具体类型实现
        /// <summary>
        /// 空类型
        /// </summary>
        public class None : RuntimeValue
        {
            public None()
                : base(RuntimeValueType.None) { }
        }

        /// <summary>
        /// 外部函数
        /// </summary>
        public class ExternalFunction : RuntimeValue
        {
            private ExternalFunctionHandler _Value;
            private int _ArgCount;

            public ExternalFunctionHandler Value
            {
                get
                {
                    return _Value;
                }
            }

            public int ArgCount
            {
                get
                {
                    return _ArgCount;
                }
            }

            public override RuntimeValue ApplyCallOperator(RuntimeContext Context, RuntimeValue[] Args)
            {
                if (_ArgCount >= 0 && _ArgCount != Args.Length)
                    throw new ArgumentCountMismatch(_ArgCount, Args.Length);

                return _Value(Context, Args);
            }

            public ExternalFunction(ExternalFunctionHandler Value, int ArgCount = -1)
                : base(RuntimeValueType.ExternalFunction)
            {
                _Value = Value;
                _ArgCount = ArgCount;
            }
        }

        /// <summary>
        /// 逻辑型
        /// </summary>
        public class Boolean : RuntimeValue
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

            public override RuntimeValue ApplyUnaryOperator(UnaryOp OperatorType)
            {
                if (OperatorType == UnaryOp.Not)
                    return new Boolean(!_Value);

                return base.ApplyUnaryOperator(OperatorType);
            }

            public override RuntimeValue ApplyBinaryOperator(BinaryOp OperatorType, RuntimeValue RightValueRef)
            {
                if (RightValueRef.ValueType == RuntimeValueType.Boolean)
                {
                    Boolean r = (Boolean)RightValueRef;
                    if (OperatorType == BinaryOp.Equal)
                        return new Boolean(_Value == r.Value);
                    else if (OperatorType == BinaryOp.NotEqual)
                        return new Boolean(_Value != r.Value);
                }

                return base.ApplyBinaryOperator(OperatorType, RightValueRef);
            }

            public override string DataToString()
            {
                return _Value ? "true" : "false";
            }

            public Boolean(bool Value)
                : base(RuntimeValueType.Boolean)
            {
                _Value = Value;
            }
        }

        /// <summary>
        /// 数字
        /// </summary>
        public class Digit : RuntimeValue
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

            public override RuntimeValue ApplyUnaryOperator(UnaryOp OperatorType)
            {
                if (OperatorType == UnaryOp.Negative)
                    return new Digit(-_Value);

                return base.ApplyUnaryOperator(OperatorType);
            }

            public override RuntimeValue ApplyBinaryOperator(BinaryOp OperatorType, RuntimeValue RightValueRef)
            {
                if (RightValueRef.ValueType == RuntimeValueType.Digit)
                {
                    Digit r = (Digit)RightValueRef;
                    switch (OperatorType)
                    {
                        case BinaryOp.Plus:
                            return new Digit(_Value + r.Value);
                        case BinaryOp.Minus:
                            return new Digit(_Value - r.Value);
                        case BinaryOp.Mul:
                            return new Digit(_Value * r.Value);
                        case BinaryOp.Div:
                            return new Digit(_Value / r.Value);
                        case BinaryOp.Power:
                            return new Digit(Math.Pow(_Value, r.Value));
                        case BinaryOp.Greater:
                            return new Boolean(_Value > r.Value);
                        case BinaryOp.Less:
                            return new Boolean(_Value < r.Value);
                        case BinaryOp.GreaterEqual:
                            return new Boolean(_Value >= r.Value);
                        case BinaryOp.LessEqual:
                            return new Boolean(_Value <= r.Value);
                        case BinaryOp.Equal:
                            return new Boolean(_Value == r.Value);
                        case BinaryOp.NotEqual:
                            return new Boolean(_Value != r.Value);
                    }
                }

                return base.ApplyBinaryOperator(OperatorType, RightValueRef);
            }

            public override string DataToString()
            {
                return _Value.ToString();
            }

            public Digit(double Value)
                : base(RuntimeValueType.Digit)
            {
                _Value = Value;
            }
        }

        /// <summary>
        /// 元组
        /// </summary>
        public class Tuple : RuntimeValue
        {
            private RuntimeValue[] _Value;

            public RuntimeValue[] Value
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

            public override RuntimeValue ApplyBinaryOperator(BinaryOp OperatorType, RuntimeValue RightValueRef)
            {
                if (RightValueRef.ValueType == RuntimeValueType.Tuple)
                {
                    Tuple r = (Tuple)RightValueRef;
                    if (Value.Length != r.Value.Length)
                        throw new OperationNotSupport();
                    RuntimeValue[] t = new RuntimeValue[Value.Length];
                    for (int i = 0; i < t.Length; ++i)
                    {
                        t[i] = Value[i].ApplyBinaryOperator(OperatorType, r.Value[i]);
                    }
                    return new Tuple(t);
                }
                else
                {
                    RuntimeValue[] t = new RuntimeValue[Value.Length];
                    for (int i = 0; i < t.Length; ++i)
                    {
                        t[i] = Value[i].ApplyBinaryOperator(OperatorType, RightValueRef);
                    }
                    return new Tuple(t);
                }
            }

            public override string TypeToString()
            {
                return System.String.Format("tuple({0})", Value.Length);
            }

            public override string DataToString()
            {
                StringBuilder tBuilder = new StringBuilder();
                tBuilder.Append('(');
                for (int i = 0; i < Value.Length; ++i)
                {
                    tBuilder.Append(Value[i].ToString());
                    if (i != Value.Length - 1)
                        tBuilder.Append(", ");
                }
                tBuilder.Append(')');
                return tBuilder.ToString();
            }

            public Tuple(RuntimeValue[] Value)
                : base(RuntimeValueType.Tuple)
            {
                _Value = Value;
            }
        }

        /// <summary>
        /// 字符串
        /// </summary>
        // this is not System.String!
        public class String : RuntimeValue
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
            
            public override RuntimeValue ApplyBinaryOperator(BinaryOp OperatorType, RuntimeValue RightValueRef)
            {
                return new String(_Value + RightValueRef.DataToString());
            }

            public override string DataToString()
            {
                return _Value;
            }

            public String(string Value)
                : base(RuntimeValueType.String)
            {
                _Value = Value;
            }
        }
        #endregion
    }
}
