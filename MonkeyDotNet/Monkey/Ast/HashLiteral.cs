using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Monkey.Ast
{
    using Token;
    public class HashLiteral : Expression
    {

        public Token Token { get; set; }

        public IDictionary<Expression,Expression> Pairs { get; set; }

        public override string TokenLiteral()
        {
            return this.Token.Literal;
        }

        protected override void ExpressionNode()
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Join(",", this.Pairs.Select(pair => $"{pair.Key.ToString()}:{pair.Value.ToString()}")));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
