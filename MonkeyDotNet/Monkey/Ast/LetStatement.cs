namespace Monkey.Ast
{
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using Token;
    public class LetStatement:Statement
    {

        public Token Token { get; set; }

        public Identifier Name{ get; set; }

        public Expression Value { get; set; }

        protected override void StatementNode()
        {
            
        }

        public override string TokenLiteral()
        {
            return Token.Literal;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.TokenLiteral() + " ");
            sb.Append(this.Name);
            sb.Append(" = ");

            if (this.Value != null)
            {
                sb.Append(this.Value.ToString());
            }
            sb.Append(";");
            return sb.ToString();
        }
    }
}
