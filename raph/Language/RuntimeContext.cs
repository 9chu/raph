using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 运行时上下文环境
    /// 
    /// 提供变量存储、查表支持
    /// </summary>
    /// <remarks>接受的标识符默认均为小写形式</remarks>
    public class RuntimeContext
    {
        private RuntimeContext _Parent = null;
        private Dictionary<string, RuntimeValue> _Environment = new Dictionary<string, RuntimeValue>();
        
        /// <summary>
        /// 获取绑定的父环境
        /// </summary>
        public RuntimeContext Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// 在上下文中寻找指定的标识符所对应的变量
        /// </summary>
        /// <param name="IdentifierLower">标识符小写形式</param>
        /// <exception>KeyNotFoundException</exception>
        /// <returns>值</returns>
        public RuntimeValue this[string IdentifierLower]
        {
            get
            {
                RuntimeValue tRet;
                if (!_Environment.TryGetValue(IdentifierLower, out tRet))
                {
                    if (_Parent != null)
                        return _Parent[IdentifierLower];
                    else
                        throw new KeyNotFoundException();
                }
                return tRet;
            }
            set
            {
                _Parent[IdentifierLower] = value;
            }
        }

        /// <summary>
        /// 从当前环境中移除一个标识符
        /// </summary>
        /// <param name="IdentifierLower">标识符小写形式</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveValue(string IdentifierLower)
        {
            return _Environment.Remove(IdentifierLower);
        }

        /// <summary>
        /// 是否存在标识符
        /// </summary>
        /// <param name="IdentifierLower">标识符小写形式</param>
        /// <returns>判断结果</returns>
        public bool Contains(string IdentifierLower)
        {
            return _Environment.ContainsKey(IdentifierLower);
        }

        /// <summary>
        /// 在上下文链中检查是否存在标识符
        /// </summary>
        /// <param name="IdentifierLower">标识符小写形式</param>
        /// <param name="Owner">所有者</param>
        /// <returns>判断结果</returns>
        public bool ContainsRecursive(string IdentifierLower, out RuntimeContext Owner)
        {
            if (_Environment.ContainsKey(IdentifierLower))
            {
                Owner = this;
                return true;
            }
            else if (_Parent != null)
                return _Parent.ContainsRecursive(IdentifierLower, out Owner);
            else
            {
                Owner = null;
                return false;
            }
        }

        public RuntimeContext(RuntimeContext Parent)
        {
            _Parent = Parent;
        }
    }
}
