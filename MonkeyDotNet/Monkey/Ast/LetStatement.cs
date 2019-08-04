namespace Monkey.Ast
{

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
    }
}
