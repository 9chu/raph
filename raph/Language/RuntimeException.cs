using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 运行时错误
    /// </summary>
    public class RuntimeException : Exception
    {
        private int _Line;
        private string _Description;
        
        private static string combineException(int Line, string Description)
        {
            return String.Format("({0}): {1}", Line, Description);
        }

        public int Line
        {
            get
            {
                return _Line;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
        }

        public RuntimeException(int Line, string Description)
            : base(combineException(Line, Description))
        {
            _Line = Line;
            _Description = Description;
        }
    }

    /// <summary>
    /// 操作不支持
    /// </summary>
    public class OperationNotSupport : Exception { }

    /// <summary>
    /// 参数数量不匹配
    /// </summary>
    public class ArgumentCountMismatch : Exception
    {
        private int _ArgumentGiven = 0;
        private int _ArgumentRequired = 0;

        public int ArgumentGiven
        {
            get
            {
                return _ArgumentGiven;
            }
        }

        public int ArgumentRequired
        {
            get
            {
                return _ArgumentRequired;
            }
        }

        public ArgumentCountMismatch(int iArgumentRequired, int iArgumentGiven)
        {
            _ArgumentGiven = iArgumentGiven;
            _ArgumentRequired = iArgumentRequired;
        }
    }
}
