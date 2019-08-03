namespace Monkey.Lexer
{
    using Token;
    /// <summary>
    /// Lexer for Token
    /// </summary>
    public class Lexer
    {
        private int position;
        private int readposition;
        private char character;
        private readonly string input;

        internal Lexer(string input)
        {
            this.input = input;
            ReadChar();
        }

        /// <summary>
        /// Create Lexer instance.
        /// </summary>
        /// <param name="input">input stream</param>
        /// <returns>A lexer instance</returns>
        public static Lexer Create(string input)
        {
            return new Lexer(input);
        }

        /// <summary>
        /// Next token 
        /// </summary>
        /// <returns></returns>
        public Token NextToken()
        {
            Token token;
            SkipWhitespace();
            switch (this.character)
            {
                case '=':
                    token = NewToken(TokenType.ASSIGN, this.character);
                    if (this.PeekChar() == '=')
                    {
                        this.ReadChar();
                        token = NewToken(TokenType.EQ, "==");
                    }
                    break;
                case ';':
                    token = NewToken(TokenType.SEMICOLON, this.character);
                    break;
                case '(':
                    token = NewToken(TokenType.LPAREN, this.character);
                    break;
                case ')':
                    token = NewToken(TokenType.RPAEN, this.character);
                    break;
                case ',':
                    token = NewToken(TokenType.COMMA, this.character);
                    break;
                case '+':
                    token = NewToken(TokenType.PLUS, this.character);
                    break;
                case '{':
                    token = NewToken(TokenType.LBRACE, this.character);
                    break;
                case '}':
                    token = NewToken(TokenType.RBRACE, this.character);
                    break;
                case '-':
                    token = NewToken(TokenType.MINUS, this.character);
                    break;
                case '/':
                    token = NewToken(TokenType.SLASH, this.character);
                    break;
                case '*':
                    token = NewToken(TokenType.ATERISK, this.character);
                    break;
                case '<':
                    token = NewToken(TokenType.LT, this.character);
                    break;
                case '>':
                    token = NewToken(TokenType.GT, this.character);
                    break;
                case '!':
                    if(this.PeekChar() == '=')
                    {
                        this.ReadChar();
                        token = NewToken(TokenType.NOT_EQ, "!=");
                    }
                    else
                    {
                        token = NewToken(TokenType.BANG, this.character);
                    }
                    break;
                case '"':
                    token = NewToken(TokenType.STRING, this.ReadString());
                    break;
                case '[':
                    token = NewToken(TokenType.LBRACKET, this.character);
                    break;
                case ']':
                    token = NewToken(TokenType.RBRACKET, this.character);
                    break;
                case ':':
                    token = NewToken(TokenType.COLON, this.character);
                    break;
                case '\0':
                    token = NewToken(TokenType.EOF, this.character);
                    break;
                default:
                    if (IsDigit(this.character))
                    {
                        token = NewToken(TokenType.INT, this.ReadNumber());
                    }
                    else
                    {
                        var identifer = this.ReadIdentifier();
                        token = NewToken(Token.LookupIdentifier(identifer), identifer);
                    }
                    break;
            }
            ReadChar();
            return token;
        }

        private void ReadChar()
        {
            if (readposition >= input.Length) {
                character = '\0';
            }
            else
            {
                character = input[readposition];
            }
            position = readposition;
            readposition += 1;
        }

        private string ReadString()
        {
            var pos = this.position + 1;
            while (true)
            {
                this.ReadChar();
                if (this.PeekChar()== '"'){
                    break;
                }
            }
            this.ReadChar();
            return this.input.Substring(pos, this.position - pos);
        }

        private string ReadNumber()
        {
            var pos = this.position;
            while (IsDigit(this.PeekChar()))
            {
                this.ReadChar();
            }
            return this.input.Substring(pos, this.readposition - pos);
        }

        private string ReadIdentifier()
        {
            var pos = this.position;
            while(IsIdentifier(this.PeekChar()))
            {
                this.ReadChar();
            }
            return this.input.Substring(pos, this.readposition - pos);
        }

        private char PeekChar()
        {
            if(this.readposition >= this.input.Length)
            {
                return '\0';
            }
            else
            {
                return this.input[this.readposition];
            }
        }

        private Token NewToken(TokenType type, char ch)
        {
            return Token.Create(type, ch.ToString());
        }

        private Token NewToken(TokenType type, string literal)
        {
            return Token.Create(type, literal);
        }

        private static bool IsIdentifier(char ch)
        {
            return !IsWhitespace(ch) && !IsDigit(ch) && !IsParen(ch) && !IsBracket(ch)
                && !IsBrace(ch) && !IsOperator(ch) && !IsComparsion(ch) && !IsEmpty(ch) && !IsCompound(ch);
        }

        private void SkipWhitespace()
        {
            while(IsWhitespace(this.character))
            {
                ReadChar();
            }
        }


        private static bool IsDigit(char ch)
        {
            return '0' <= ch && '9' >= ch;
        }

        private static bool IsWhitespace(char ch)
        {
            return ' ' == ch || '\t' == ch || '\n' == ch || '\r' == ch;
        }

        private static bool IsParen(char ch)
        {
            return '(' == ch || ')' == ch;
        }

        private static bool IsCompound(char ch)
        {
            return ',' == ch || ':' == ch || '"' == ch || ';' == ch;
        }

        private static bool IsBracket(char ch)
        {
            return '[' == ch || ']' == ch;
        }

        private static bool IsBrace(char ch)
        {
            return '{' == ch || '}' == ch;
        }

        private static bool IsOperator(char ch)
        {
            return '+' == ch || '-' == ch || '*' == ch || '/' == ch;
        }

        private static bool IsComparsion(char ch)
        {
            return '=' == ch || '!' == ch || '<' == ch || '>' == ch;
        }

        private static bool IsEmpty(char ch)
        {
            return '\0' == ch;
        }
    }
}
