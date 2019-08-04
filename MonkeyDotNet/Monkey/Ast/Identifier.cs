namespace Monkey.Ast
{
    using Token;
    public class Identifier:Expression
    {
        public Token Token { get; set; }
        public string Value { get; set; }

        protected override void ExpressionNode()
        {
            
        }

        public override string TokenLiteral()
        {
            return Token.Literal;
        }
    }
}
