using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Monkey.Ast
{
    using Token;

    public class CallExpression:Expression
    {
        public Token Token { get; set; }

        public Expression Function { get; set; }

        public IList<Expression> Arguments { get; set; }

        protected override void ExpressionNode() 
        {

        }

        public override string TokenLiteral()
        {
            return this.Token.Literal;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Function.ToString());
            sb.Append("(");
            sb.Append(string.Join(",", this.Arguments.Select(arg => arg.ToString())));
            sb.Append(")");
            return sb.ToString();
        }
    }
    
}