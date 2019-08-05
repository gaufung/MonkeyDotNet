namespace Monkey.Parser
{
    using Lexer;
    using Token;
    using Ast;

    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _curToken;
        private Token _peekToken;

        public Parser(Lexer lexer)
        {
            this._lexer = lexer;
            this.NextToken();
            this.NextToken();
        }

        private void NextToken()
        {
            this._curToken = this._peekToken;
            this._peekToken = this._lexer.NextToken();
        }

        public Program ParseProgram()
        {
            var program = new Program();
            while (this._curToken.Type != TokenType.EOF)
            {
                Statement stmt = this.ParserStatement();
                if(stmt!= null)
                {
                    program.Statements.Add(stmt);
                }
                this.NextToken();
            }
            return program;
        }

        private Statement ParserStatement()
        {
            switch (this._curToken.Type)
            {
                case TokenType.LET:
                    return this.ParseLetStatement();
                case TokenType.RETURN:
                    return this.ParseReturnStatement();
                default:
                    return null;

            }
        }

        #region token tools
        private bool CurTokenIs(TokenType t)
        {
            return this._curToken.Type == t;
        }
        private bool PeekTokenIs(TokenType t)
        {
            return this._peekToken.Type == t;
        }
        private bool ExpectPeek(TokenType t)
        {
            if (this.PeekTokenIs(t))
            {
                this.NextToken();
                return true;
            }
            else
            {
                throw new ParserException($"expect next token is {t}, but got {this._peekToken}");
            }
        }
        #endregion

        #region parseLetStatement
        private LetStatement ParseLetStatement()
        {
            var stmt = new LetStatement()
            {
                Token = this._curToken,
            };
            if (!this.ExpectPeek(TokenType.IDENT))
            {
                return null;
            }

            stmt.Name = new Identifier(this._curToken, this._curToken.Literal);

            if (!this.ExpectPeek(TokenType.ASSIGN))
            {
                return null;
            }

            //todo: skipping the expression util we encounter a semicolon
            while (!this.CurTokenIs(TokenType.SEMICOLON))
            {
                this.NextToken();
            }
            return stmt;

        }
        #endregion

        #region parserReturnStatement
        public ReturnStatement ParseReturnStatement()
        {
            var stmt = new ReturnStatement(this._curToken);
            this.NextToken();
            //todo: skip the expression until we encounter a semicolon
            while (this.CurTokenIs(TokenType.SEMICOLON))
            {
                this.NextToken();
            }
            return stmt;
        }
        #endregion

    }
}
