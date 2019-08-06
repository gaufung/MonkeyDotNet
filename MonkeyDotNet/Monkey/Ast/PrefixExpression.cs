using System.Text;
namespace Monkey.Ast
{
    using Token;
    public class PrefixExpression : Expression
    {
        public Token Token { get; set; }

        public string Operator { get; set; }

        public Expression Right { get; set; }


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
            sb.Append("(");
            sb.Append(Operator);
            sb.Append(Right.ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }
}
