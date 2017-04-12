using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class TSymbol
    {
        public char value;
        public int attr;

        public TSymbol(char value, int attr)
        {
            this.value = value;
            this.attr = attr;
        }

        public static int get_attr(char value)
        {
            if ((value == ' ') || (value == '\r') || (value == '\n') || (value == '\t'))
                return 0;
            if (Char.IsDigit(value))
                return 1;
            if (Char.IsLetter(value))
                return 2;
            if (value == '(')
                return 3;
            List<char> listOfPunct = new List<char> { ';', '.', ':', ',', ')', '=' };
            if (listOfPunct.Contains(value))
                return 4;
            return 5;
        }
    }
}
