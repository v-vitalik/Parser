using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DrawTreeNodes();
        }

        private void DrawTreeNodes()
        {
            Syntax parser = new Syntax();
            Lexer lex = new Lexer("test.txt");
            Queue<lexem> queue = new Queue<lexem>(lex.lex_list);
            parser.Run(lex, queue, ref treeView1);
            Semantic sem = new Semantic();
            StreamWriter sw = new StreamWriter("FinalResult.txt");
            sem.SemanticAnalysis(sw, treeView1.Nodes[0]);
            sw.Close();
        }
    }
}
