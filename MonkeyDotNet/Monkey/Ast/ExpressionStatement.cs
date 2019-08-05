using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast
{
    using Token;
    public class ExpressionStatement : Statement
    {
        public Token Token { get; set; }

        public Expression Expression { get; set; }

        public ExpressionStatement(Token t)
        {
            Token = t;
        }
        public override string TokenLiteral()
        {
            return this.Token.Literal;
        }

        protected override void StatementNode()
        {
        }

        public override string ToString()
        {
            if (this.Expression != null)
            {
                return this.Expression.ToString();
            }
            return "";
        }
    }
}
