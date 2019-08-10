using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Monkey.Ast
{
    using Token;
    public class FunctionLiteral:Expression
    {
       public Token Token { get; set; } 

       public IList<Identifier> Parameters { get; set; }

       public BlockStatement Body { get; set; }

       protected override void ExpressionNode()
       {

       }

        public override string TokenLiteral()
        {
            return Token.Literal;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(TokenLiteral());
            sb.Append("(");
            sb.Append(string.Join(",", Parameters.Select(p=>p.ToString())));
            sb.Append(")");
            sb.Append(Body.ToString());
            return sb.ToString();
        }
    }
}
