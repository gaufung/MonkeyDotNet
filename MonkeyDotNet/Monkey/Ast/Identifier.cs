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

        public Identifier(Token t, string v)
        {
            Token = t;
            Value = v;
        }

        public override string TokenLiteral()
        {
            return Token.Literal;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
