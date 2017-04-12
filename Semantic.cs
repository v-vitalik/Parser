using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Parser
{
    class Semantic
    {
        private int getSizeAttr(string attribute)
        {
            switch (attribute){
                case "integer":
                    return 2;
                case "float":
                    return 4;
                case "complex":
                    return 20;
                case "signal":
                    return 10;
            }
            return 0;
        }

        private string getTypeAttr(TreeNode node)
        {
            if ((node.Nodes[3].Nodes[0].Text != "ext") && (node.Nodes[3].Nodes[0].Text != "blockfloat"))
                return node.Nodes[3].Nodes[0].Text;
            if(node.Nodes[4].Text != "<empty>") {
                if ((node.Nodes[4].Nodes[0].Nodes[0].Text != "ext") && (node.Nodes[4].Nodes[0].Nodes[0].Text != "blockfloat"))
                    return node.Nodes[4].Nodes[0].Nodes[0].Text;
            }
            return "not good";
        }

        private bool getType(string s)
        {
            bool b = true;
            for (int i = 0; i<s.Length; i++)
            {
                if (!(Char.IsDigit(s[i])))
                    b = false;
            }
            return b;
        }
        public Queue<string> variable_ident_list = new Queue<string>();
        public void SemanticAnalysis(StreamWriter sw, TreeNode node)
        {
            
            switch (node.Text)
            {
                case "<program>":
                    if (node.FirstNode.Text == "procedure")
                    {
                        sw.WriteLine("code segment");
                        sw.WriteLine();
                        SemanticAnalysis(sw, node.Nodes[1]);
                        SemanticAnalysis(sw, node.Nodes[2]);
                        sw.WriteLine();
                        sw.WriteLine("push\tebp");
                        sw.WriteLine("mov\tebp, esp");
                        sw.WriteLine();
                        SemanticAnalysis(sw, node.Nodes[4]);
                        sw.WriteLine();
                        sw.WriteLine("pop\tebp");
                        sw.WriteLine();
                        sw.WriteLine("ret");
                        sw.WriteLine();
                        sw.WriteLine("code ends");
                    }
                    else
                    {
                        sw.WriteLine("code segment");
                        sw.WriteLine();
                        
                        SemanticAnalysis(sw, node.Nodes[1]);
                        SemanticAnalysis(sw, node.Nodes[2]);
                        sw.WriteLine("code ends");
                    }
                    break;
                case "<procedure-identifier>":
                    SemanticAnalysis(sw, node.Nodes[0]);
                    sw.WriteLine(":");
                    break;
                case "<identifier>":
                    if (node.Parent.Text == "<procedure-identifier>")
                        sw.Write("{0}", node.Nodes[0].Text);
                    else
                        variable_ident_list.Enqueue(node.Nodes[0].Text);
                    break;
                case "<parameters-list>":
                    SemanticAnalysis(sw, node.Nodes[1]);
                    break;
                case "<declaration-list>":
                    if (node.Nodes[0].Text != "<empty>")
                    {
                        SemanticAnalysis(sw, node.Nodes[0]);
                        SemanticAnalysis(sw, node.Nodes[1]);
                    }
                    break;
                case "<declaration>":
                    SemanticAnalysis(sw, node.Nodes[0]);
                    SemanticAnalysis(sw, node.Nodes[1]);
                    while (variable_ident_list.Count() > 0)
                    {

                        sw.WriteLine("{0}\tequ\t[ebp + {1}]",variable_ident_list.Dequeue() , getSizeAttr(getTypeAttr(node)));
                    }
                    break;
                case "<variable-identifier>":
                    SemanticAnalysis(sw, node.Nodes[0]);
                    //sw.Write("\tequ");
                    break;
                case "<identifier-list>":
                    if (node.Nodes[0].Text != "<empty>")
                    {
                        SemanticAnalysis(sw, node.Nodes[1]);
                        //sw.Write("\tequ");
                        //SemanticAnalysis(sw, node.Nodes[2]);
                    }
                    break;
                case "<block>":
                    SemanticAnalysis(sw, node.Nodes[1]);
                    break;
                case "<statements-list>":
                    SemanticAnalysis(sw, node.Nodes[0]);
                    SemanticAnalysis(sw, node.Nodes[1]);
                    break;
                case "<variable>":
                    SemanticAnalysis(sw, node.Nodes[0]);
                    if (getType(node.Nodes[3].Text))
                    {
                        sw.WriteLine("mov\t{0}, {1}", variable_ident_list.Dequeue(), node.Nodes[3].Text);
                    }
                    else
                    {
                        sw.WriteLine("mov\teax, {0}", node.Nodes[3].Text);
                    }
                    

                    break;
            }
        }
    }
}
