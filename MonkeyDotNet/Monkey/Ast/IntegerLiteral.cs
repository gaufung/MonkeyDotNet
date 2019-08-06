using System;
namespace Monkey.Ast
{
    using Monkey.Token;
    
    public class IntegerLiteral:Expression
    {
        public Token Token { get; set; }

        public long Value { get; set; }


        public override string TokenLiteral()
        {
            return Token.Literal;
        }

        public override string ToString()
        {
            return Token.Literal;
        }

        protected override void ExpressionNode()
        {

        }

    }
}
