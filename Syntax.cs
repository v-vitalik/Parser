using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    class Syntax
    {
        public void Run(Lexer lex, Queue<lexem> q, ref TreeView tree)
        {
            tree.Nodes.Add(Prog(lex, q));
        }

        private TreeNode Prog(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<program>");
            if (queue.First().code != 401)
            {
                if (queue.Dequeue().code != 402)
                    throw new Exception("'program' or 'procedure' wasn't found");
                node.Nodes.Add("procedure");

                node.Nodes.Add(procedureIdentifier(lex, queue));
                node.Nodes.Add(parameters(lex, queue));
                if (queue.Dequeue().code != 59)
                    throw new Exception("Expected ';'");
                node.Nodes.Add(";");

                node.Nodes.Add(block(lex, queue));

                if (queue.Dequeue().code != ';')
                    throw new Exception("Expected ';'");
                node.Nodes.Add(";");
                //node.Nodes.Add(procedure(lex, queue));
                return node;
            }
            
            node.Nodes.Add(queue.Dequeue().value);
            node.Nodes.Add(procedureIdentifier(lex, queue));

            if (queue.Dequeue().code != ';')
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");

            node.Nodes.Add(block(lex, queue));

            if (queue.Dequeue().code != '.')
                throw new Exception("Expected '.'");
            node.Nodes.Add(".");
            return node;
        }

        private TreeNode procedure(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode(/*"procedure"*/);
            /*if (queue.Dequeue().code != 402)
                throw new Exception("'program' or 'procedure' wasn't found");
            //node.Nodes.Add("procedure");*/
            node.Nodes.Add(procedureIdentifier(lex, queue));
            node.Nodes.Add(parameters(lex, queue));
            if (queue.Dequeue().code != 59)
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");

            node.Nodes.Add(block(lex, queue));

            if (queue.Dequeue().code != ';')
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");
            return node; 
        }

        private TreeNode block(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<block>");
            if (queue.Dequeue().code != 403)
                throw new Exception("Expected 'begin'");
            node.Nodes.Add("Begin");
            
            node.Nodes.Add(statementsList(lex, queue));
            
            if (queue.Dequeue().code != 404)
                throw new Exception("Expected 'end'");

            node.Nodes.Add("End");
            return node;
        }

        private TreeNode statementsList(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<statements-list>");
            if (queue.First().code > 1000)
                node.Nodes.Add(prysv(lex, queue));
            if (queue.First().code == 411)
                node.Nodes.Add(Link(lex, queue));

            if (queue.First().code != 404)
                node.Nodes.Add(statementsList(lex, queue));
            node.Nodes.Add("<empty>");
            /*
            if (queue.Dequeue().code != ';')
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");
            */
            return node;
        }

        private TreeNode Link(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode(queue.Dequeue().value);

            

            node.Nodes.Add("<variable-identifier>");
            node.Nodes.Add(identifier(lex, queue));

            if (queue.Dequeue().code != ',')
                throw new Exception("Expected ','");
            node.Nodes.Add(",");

            if (queue.First().code / 100 != 5)
                throw new Exception("Expecte constant");
            node.Nodes.Add("<expretion>");
            node.Nodes.Add(queue.Dequeue().value);
            if (queue.Dequeue().code != ';')
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");

            return node;
        }

        private TreeNode prysv(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<variable>");
            node.Nodes.Add(identifier(lex, queue));
            if (queue.First().code != '=')
                throw new Exception("Expected '='");
            node.Nodes.Add(queue.Dequeue().value);
            if ((queue.First().code / 100 != 5) && (queue.First().code / 100 !=10 ))
                throw new Exception("Expecte constant");
            node.Nodes.Add("<expretion>");
            node.Nodes.Add(queue.Dequeue().value);
            if (queue.Dequeue().code != ';')
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");
            return node;
        }

        private TreeNode procedureIdentifier(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<procedure-identifier>");
            node.Nodes.Add(identifier(lex, queue));
            return node;
        }

        private TreeNode parameters(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<parameters-list>");
            if (queue.First().code == ';')
                return node.Nodes.Add("<empty>");
            if (queue.Dequeue().code != '(')
                throw new Exception("Expexted declarations or ';'");
            node.Nodes.Add("(");
            node.Nodes.Add(declarationList(lex, queue));

            if (queue.Dequeue().code != ')')
                throw new Exception("Expexted ')'");
            node.Nodes.Add(")");
            return node;
        }

        private TreeNode declarationList(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<declaration-list>");
            if(queue.First().code == ')')
            {
                node.Nodes.Add("<empty>");
                return node;
            }

            node.Nodes.Add(declaration(lex, queue));
            
            node.Nodes.Add(declarationList(lex, queue));
            return node;
        }

        private TreeNode declaration(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<declaration>");
            if (queue.First().code < 1000)
                throw new Exception("Expected identifier");
            node.Nodes.Add(variableIdent(lex, queue));
            node.Nodes.Add(identifierList(lex, queue));
            if (queue.First().code != ':')
                throw new Exception("Expected ':'");
            node.Nodes.Add(queue.Dequeue().value);
            node.Nodes.Add(attribute(lex, queue));
            node.Nodes.Add(attributeList(lex, queue));
            if (queue.Dequeue().code != ';')
                throw new Exception("Expected ';'");
            node.Nodes.Add(";");

            return node;
        }

        private TreeNode attributeList(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<attribute-list>");
            if (queue.First().code == ';')
            {
                node.Nodes.Add("<empty>");
                return node;
            }
            if ((queue.First().code > 404) && (queue.First().code < 411))
                node.Nodes.Add(attribute(lex, queue));
            return node;
        }

        private TreeNode attribute(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<attribute>");
            string[] attrib = new string[] { "integer", "float", "blockfloat", "signal", "complex", "ext" };
            if (!((queue.First().code > 404)&&(queue.First().code < 411)))
                throw new Exception("Attribute was expected");
            node.Nodes.Add(queue.Dequeue().value);
            return node;
        }

        private TreeNode variableIdent(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<variable-identifier>");
            node.Nodes.Add(identifier(lex, queue));
            /*if (queue.First().code == ',')
            {
                node.Nodes.Add(identifierList(lex, queue));
                //node.Nodes.Add(queue.Dequeue().value);
                //node.Nodes.Add("<identifier list>");
                //node.Nodes.Add(variableIdent(lex, queue));
            }*/
            //node.Nodes.Add("<empty>");
            return node;
        }

        private TreeNode identifierList(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<identifier-list>");
            if (queue.First().code == ',')
            {
                node.Nodes.Add(queue.Dequeue().value);
                if (queue.First().code < 1000)
                    throw new Exception("Expected identifier");
                node.Nodes.Add(identifier(lex, queue));
                node.Nodes.Add(identifierList(lex, queue));

                //node.Nodes.Add(queue.Dequeue().value);
                //node.Nodes.Add("<identifier list>");
                //node.Nodes.Add(variableIdent(lex, queue));
            }
            else
                node.Nodes.Add("<empty>");
            return node;
        }


        private bool ListContains(Lexer lex, string l)
        {
            for(int i = 0; i<lex.id_list.Count(); i++)
            {
                if (lex.id_list[i].value == l)
                {
                    //lex.id_list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private TreeNode identifier(Lexer lex, Queue<lexem> queue)
        {
            TreeNode node = new TreeNode("<identifier>");
            if (ListContains(lex, queue.First().value))
                node.Nodes.Add(queue.Dequeue().value);
            else
                throw new Exception("Expected identifier");
            return node;
        }
    }
}
