using System.Text;

namespace Monkey.Ast
{
    using Token;
    public class IfExpression :Expression
    {
        public Token Token { get; set; }

        public Expression Condition { get; set; }

        public BlockStatement Consequence { get; set; }

        public BlockStatement Alternative { get; set; }

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
            sb.Append("if");
            sb.Append(this.Condition.ToString());
            sb.Append(" ");
            sb.Append(this.Consequence.ToString());
            if (this.Alternative != null)
            {
                sb.Append("else ");
                sb.Append(this.Alternative.ToString());
            }
            return sb.ToString();
        }
    }
}
