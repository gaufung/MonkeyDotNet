using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Monkey.Ast
{
    using System.Collections;
    using Token;
    public class ArrayLiteral : Expression
    {
        public Token Token { get; set; }

        public IList<Expression> Elements { get; set; }


        public override string TokenLiteral()
        {
            return this.Token.Literal;
        }

        protected override void ExpressionNode()
        {
           
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            sb.Append(string.Join(",", this.Elements.Select(e => e.ToString())));
            sb.Append("]");
            return sb.ToString();
        }
    }
}
