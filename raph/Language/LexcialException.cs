using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace raph.Language
{
    /// <summary>
    /// 词法错误
    /// </summary>
    public class LexcialException : Exception
    {
        private int _Position;
        private int _Line;
        private int _Row;
        private string _Description;
        
        private static string combineException(int Position, int Line, int Row, string Description)
        {
            return String.Format("({0},{1},{2}): {3}", Line, Row, Position, Description);
        }

        public int Position
        {
            get
            {
                return _Position;
            }
        }

        public int Line
        {
            get
            {
                return _Line;
            }
        }

        public int Row
        {
            get
            {
                return _Row;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
        }

        public LexcialException(int Position, int Line, int Row, string Description)
            : base(combineException(Position, Line, Row, Description))
        {
            _Position = Position;
            _Line = Line;
            _Row = Row;
            _Description = Description;
        }
    }
}
