using System;
using System.Collections.Generic;
using System.Text;

namespace Monkey.Ast
{
    using Token;
    public class StringLiteral : Expression
    {

        public Token Token { get; set; }

        public string Value { get; set; }

        public override string TokenLiteral()
        {
            return Token.Literal;
        }

        protected override void ExpressionNode()
        {
            
        }

        public override string ToString()
        {
            return Token.Literal;
        }
    }
}
