using System.Text;
namespace Monkey.Ast
{
    using Token;
    public class InfixExpression:Expression
    {
        public Token Token { get; set; }

        public Expression Left { get; set; }

        public string Operator { get; set; }

        public Expression Right { get; set; }

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
            sb.Append("(");
            sb.Append(this.Left.ToString());
            sb.Append(" " + this.Operator + " ");
            sb.Append(this.Right.ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }
}
