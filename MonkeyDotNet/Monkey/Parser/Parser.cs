namespace Monkey.Parser
{
    using Lexer;
    using Token;
    using Ast;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Versioning;

    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _curToken;
        private Token _peekToken;

        private IDictionary<TokenType, Func<Expression>> _prefixParseFn;
        private IDictionary<TokenType, Func<Expression, Expression>> _infixParseFn;


        public Parser(Lexer lexer)
        {
            this._lexer = lexer;
            this.NextToken();
            this.NextToken();
            this._prefixParseFn = new Dictionary<TokenType, Func<Expression>>();
            this._infixParseFn = new Dictionary<TokenType, Func<Expression, Expression>>();
            this.RegisterPrefix(TokenType.IDENT, this.ParseIdentifier);
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
                    return this.ParseExpressionStatement();

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

        private void RegisterPrefix(TokenType t, Func<Expression> fn)
        {
            this._prefixParseFn.Add(t, fn);
        }

        private void RegisterInfix(TokenType t, Func<Expression, Expression> fn)
        {
            this._infixParseFn.Add(t, fn);
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
            while (!this.CurTokenIs(TokenType.SEMICOLON))
            {
                this.NextToken();
            }
            return stmt;
        }
        #endregion

        #region parseExpressionStatement

        private ExpressionStatement ParseExpressionStatement()
        {
            var smst = new ExpressionStatement(this._curToken);
            smst.Expression = this.ParseExpression(Precedence.LOWEST);
            if (this.PeekTokenIs(TokenType.SEMICOLON))
            {
                this.NextToken();
            }
            return smst;
        }

        private Expression ParseExpression(Precedence precedence)
        {
            if (this._prefixParseFn.ContainsKey(this._curToken.Type))
            {
                Func<Expression> prefix = this._prefixParseFn[this._curToken.Type];
                return prefix();
            }
            throw new ParserException($"Unsupport token: {this._curToken.Type}");
        }

        private Expression ParseIdentifier()
        {
            return new Identifier(this._curToken, this._curToken.Literal);
        }
        #endregion

    }
}
