namespace Monkey.Ast
{
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
    }
}
