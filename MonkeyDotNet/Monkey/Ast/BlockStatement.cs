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
                var sub = stmt.ToString();
                var type = stmt.GetType();
                sb.Append(sub);
            }
            return sb.ToString();
        }
    }
}
