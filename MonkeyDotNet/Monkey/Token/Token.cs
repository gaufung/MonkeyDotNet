using System.Collections.Generic;
namespace Monkey.Token
{
    /// <summary>
    /// Token for Monkey
    /// </summary>
    public class Token
    {
        public TokenType Type { get; private set; }

        public string Literal { get; private set; }


        /// <summary>
        /// Create Token from tokentype and literal
        /// </summary>
        /// <param name="type">tokenType</param>
        /// <param name="literal">literal</param>
        /// <returns>The Token instance</returns>
        public static Token Create(TokenType type, string literal)
        {
            return new Token { Type = type, Literal = literal };
        }

        private readonly static IDictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            {"fn", TokenType.FUNCTION},
            {"let",TokenType.LET },
            {"true", TokenType.TRUE },
            {"false", TokenType.FALSE },
            {"if", TokenType.IF },
            {"else", TokenType.ELSE },
            {"return", TokenType.RETURN },
            {"for", TokenType.FOR },
        };

        /// <summary>
        /// Lookup identifier to determinate it whether it is keword
        /// </summary>
        /// <param name="identifier">identifier for lookup</param>
        /// <returns>the actual type.</returns>
        public static TokenType LookupIdentifier(string identifier)
        {
            return keywords.ContainsKey(identifier) ? keywords[identifier] : TokenType.IDENT;
        }
    }
}
