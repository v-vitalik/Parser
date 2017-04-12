using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    class Lexer
    {
        public List<lexem> lex_list;
        public List<lexem> id_list;
        public List<lexem> const_list;

        public Lexer(string file_name)
        {
            this.id_list = new List<lexem>();
            this.lex_list = new List<lexem>();
            this.const_list = new List<lexem>();
            StreamReader sr = File.OpenText(file_name);
            Scan(sr, ref this.lex_list, ref this.id_list, ref this.const_list);
        }

        private static void gets(StreamReader sr, TSymbol s, out bool eof)
        {
            eof = false;
            int a = sr.Read();
            if (a > 0)
                s.value = (char)a;
            else
                eof = true;
            s.attr = TSymbol.get_attr(s.value);
        }

        private static int ArraySearch(string buf, string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].ToLower() == buf.ToLower())
                {
                    return i;
                }
            }
            return -1;
        }
        private static void Scan(StreamReader sr, ref List<lexem> lex_list, ref List<lexem> id_list, ref List<lexem> const_list)
        {
            StreamWriter wr1 = new StreamWriter("Result.txt");


            TSymbol symbol = new TSymbol(' ', 1);
            string[] KeyWords = new string[] { "program", "procedure", "begin", "end", "integer", "float", "blockfloat", "signal", "complex", "ext", "link" };
            string buf;
            int lexCode;
            bool commentOutput, errorOutput, SuppressOutput = false;
            bool eof = false;
            gets(sr, symbol, out eof);

            do
            {
                lexCode = 0;
                buf = "";
                SuppressOutput = false;
                errorOutput = false;
                commentOutput = false;
                switch (symbol.attr)
                {
                    case 0:
                        while ((!eof) && (symbol.attr == 0))
                        {
                            gets(sr, symbol, out eof);
                        }
                        SuppressOutput = true;
                        break;
                    case 1:
                        while ((!eof) && (symbol.attr == 1))
                        {
                            buf = buf + symbol.value;
                            gets(sr, symbol, out eof);
                        }
                        lexCode = const_list.Count() + 501;
                        const_list.Add(new lexem(buf, lexCode));
                        break;
                    case 2:
                        while ((!eof) && ((symbol.attr == 1)) || (symbol.attr == 2))
                        {
                            buf = buf + symbol.value;
                            gets(sr, symbol, out eof);
                        }
                        if (ArraySearch(buf, KeyWords) >= 0)
                            lexCode = ArraySearch(buf, KeyWords) + 401;
                        else
                        {
                            if (lexem.index(id_list, buf) > 0)
                                lexCode = id_list[lexem.index(id_list, buf)].code;
                            else
                            {
                                lexCode = id_list.Count() + 1001;
                                id_list.Add(new lexem(buf, lexCode));
                            }
                        }
                        break;
                    case 3:
                        if (eof)
                            lexCode = '(';
                        else
                        {
                            gets(sr, symbol, out eof);
                            if (symbol.value == '*')
                            {
                                if (eof)
                                {
                                    wr1.WriteLine("'*' expected, but end of file found");
                                    commentOutput = true;
                                }
                                else
                                {
                                    gets(sr, symbol, out eof);
                                    do
                                    {
                                        while ((!eof) && (symbol.value != '*'))
                                            gets(sr, symbol, out eof);
                                        if (eof)
                                        {
                                            wr1.WriteLine("'*' expected, but end of file found");
                                            commentOutput = true;
                                            break;
                                        }
                                        else
                                            gets(sr, symbol, out eof);
                                    } while (symbol.value != ')');
                                    if (!eof)
                                    {
                                        gets(sr, symbol, out eof);
                                        commentOutput = true;
                                    }
                                }
                            }
                            else
                            {
                                buf = "(";
                                lexCode = '(';
                            }
                        }
                        break;
                    case 4:
                        buf = buf + symbol.value;
                        lexCode = symbol.value;
                        gets(sr, symbol, out eof);
                        break;
                    case 5:
                        buf += symbol.value;
                        gets(sr, symbol, out eof);
                        lex_list.Add(new lexem(buf.ToLower(), -1));
                        wr1.WriteLine("{0} Error !!!", buf);
                        errorOutput = true;
                        break;
                }
                if (!(SuppressOutput) && !(errorOutput) && !(commentOutput))
                {
                    lex_list.Add(new lexem(buf.ToLower(), lexCode));
                    wr1.WriteLine("{0}\t{1}", buf, lexCode);
                }
            } while (!eof);
            wr1.Close();
            StreamWriter wr2 = new StreamWriter("identTable.txt");
            for (int i = 0; i < id_list.Count(); i++)
            {
                wr2.WriteLine("{0}\t{1}", id_list[i].value, id_list[i].code);
            }
            wr2.Close();
        }
    }
}
