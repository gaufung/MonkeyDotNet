using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast
{
    using Token;
    public class BlockStatement:Statement
    {
        public Token Token { get; set; }

        public IList<Statement> Statements { get; set; }

        protected override void StatementNode()
        {
            //no opertions
        }

        public override string TokenLiteral()
        {
            return this.Token.Literal;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stmt in this.Statements)
            {
                sb.Append(sb.ToString());
            }
            return sb.ToString();
        }
    }
}
