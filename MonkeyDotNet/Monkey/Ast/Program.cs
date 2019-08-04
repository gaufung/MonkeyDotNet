using System.Collections.Generic;
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

        public override string TokenLiteral()
        {
            return _statements.Count > 0 ? _statements[0].TokenLiteral() : "";
        }
    }
}
