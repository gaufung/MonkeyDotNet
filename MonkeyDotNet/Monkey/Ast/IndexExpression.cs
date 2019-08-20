using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast
{
    using Token;
    public class IndexExpression : Expression
    {
        public Token Token { get; set; }

        public Expression Left { get; set; }

        public Expression Index { get; set; }

        public override string TokenLiteral()
        {
            return Token.Literal;
        }

        protected override void ExpressionNode()
        {
          
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            sb.Append(Left.ToString());
            sb.Append("[");
            sb.Append(Index.ToString());
            sb.Append("])");
            return sb.ToString();
        }
    }
}
