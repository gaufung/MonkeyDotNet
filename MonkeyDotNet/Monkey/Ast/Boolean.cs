namespace Monkey.Ast
{
    using Token;
    public class Boolean : Expression
    {
        public Token Token { get; set; }

        public bool Value { get; set; }

        public override string TokenLiteral()
        {
            return this.Token.Literal;
        }

        public override string ToString()
        {
            return this.Token.Literal;
        }

        protected override void ExpressionNode()
        {
            throw new System.NotImplementedException();
        }
    }
}