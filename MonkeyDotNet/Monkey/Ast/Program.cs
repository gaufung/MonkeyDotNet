using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast
{
    public class Program : Node
    {
        private readonly IList<Statement> _statements;

        public IList<Statement> Statements
        {
            get
            { 
                return _statements;
            }
        }

        public Program()
        {
            _statements = new List<Statement>();
        }

        public override string TokenLiteral()
        {
            return _statements.Count > 0 ? _statements[0].TokenLiteral() : "";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stmt in this.Statements)
            {
                sb.Append(stmt.ToString());
            }
            return sb.ToString();
        }
    }
}
