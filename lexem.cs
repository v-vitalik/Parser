using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class lexem
    {
        public string value;
        public int code;

        public lexem(string value, int code)
        {
            this.value = value;
            this.code = code;
        }

        static public int index(List<lexem> table, string buf)
        {
            for (int i = 0; i < table.Count; i++)
                if (table[i].value == buf)
                    return i;
            return -1;
        }
    }
}
