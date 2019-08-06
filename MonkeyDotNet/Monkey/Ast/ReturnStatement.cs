namespace Monkey.Ast
{
    using System.Text;
    using Token;
    public class ReturnStatement : Statement
    {
        public Token Token { get; set; }

        public Expression ReturnValue { get; set; }


        public ReturnStatement(Token t)
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
            var sb = new StringBuilder();
            sb.Append(this.TokenLiteral() + " ");
            if (this.ReturnValue != null)
            {
                sb.Append(this.ReturnValue.ToString());
            }
            sb.Append(";");
            return sb.ToString();
        }
    }
}
