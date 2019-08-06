namespace Monkey.Parser
{
    using Lexer;
    using Token;
    using Ast;
    using System;
    using System.Collections.Generic;

    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _curToken;
        private Token _peekToken;

        private IDictionary<TokenType, Func<Expression>> _prefixParseFn;
        private IDictionary<TokenType, Func<Expression, Expression>> _infixParseFn;


        private static readonly IDictionary<TokenType, Precedence> _PRECEDENCES=new Dictionary<TokenType, Precedence>(){
            {TokenType.EQ, Precedence.EQUALS},
            {TokenType.NOT_EQ, Precedence.EQUALS},
            {TokenType.LT, Precedence.LESSGRATER},
            {TokenType.GT, Precedence.LESSGRATER},
            {TokenType.PLUS, Precedence.SUM},
            {TokenType.MINUS, Precedence.SUM},
            {TokenType.SLASH, Precedence.PRODUCT},
            {TokenType.ATERISK, Precedence.PRODUCT},
        };


        public Parser(Lexer lexer)
        {
            this._lexer = lexer;
            this.NextToken();
            this.NextToken();
            this._prefixParseFn = new Dictionary<TokenType, Func<Expression>>();
            this._infixParseFn = new Dictionary<TokenType, Func<Expression, Expression>>();
            this.RegisterPrefix(TokenType.IDENT, this.ParseIdentifier);
            this.RegisterPrefix(TokenType.INT, this.ParseIntegerLiteral);
            this.RegisterPrefix(TokenType.MINUS, this.ParsePrefixExpression);
            this.RegisterPrefix(TokenType.BANG, this.ParsePrefixExpression);
            this.RegisterInfix(TokenType.PLUS, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.MINUS, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.SLASH, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.ATERISK, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.EQ, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.NOT_EQ, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.LT, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.GT, this.ParseInfixExpression);

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

        
        private Expression ParseExpression(Precedence precedence)
        {
            if (this._prefixParseFn.ContainsKey(this._curToken.Type))
            {
                Func<Expression> prefix = this._prefixParseFn[this._curToken.Type];
                Expression leftExp =  prefix();
                while(!this.PeekTokenIs(TokenType.SEMICOLON) && precedence < this.PeekPrecedence())
                {
                    if (this._infixParseFn.ContainsKey(this._peekToken.Type))
                    {
                        Func<Expression, Expression> infix = this._infixParseFn[this._peekToken.Type];
                        this.NextToken();
                        leftExp = infix(leftExp);
                    }else{
                        return leftExp;
                    }
                }
                return leftExp;
            }
            throw new ParserException($"Unsupport token: {this._curToken.Type}");
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

        #region precedences
        private Precedence PeekPrecedence()
        {
            if(_PRECEDENCES.ContainsKey(this._peekToken.Type)) 
            {
                return _PRECEDENCES[this._peekToken.Type];
            }
            return Precedence.LOWEST;
        }

        private Precedence CurPrecedence()
        {
            if(_PRECEDENCES.ContainsKey(this._curToken.Type))
            {
                return _PRECEDENCES[this._curToken.Type];
            }
            return Precedence.LOWEST;
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


        #endregion

        private Expression ParseIdentifier()
        {
            return new Identifier(this._curToken, this._curToken.Literal);
        }


        #region parseIntegerLiteral
        private IntegerLiteral ParseIntegerLiteral()
        {
            var smst = new IntegerLiteral();
            smst.Token = this._curToken;
            try
            {
                var value = long.Parse(this._curToken.Literal);
                smst.Value = value;
                return smst;
            }
            catch (FormatException)
            {
                throw new ParserException($"illegal integer: {this._curToken.Literal}");
            }

        }
        #endregion


        #region parsePrfixExpression
        private Expression ParsePrefixExpression()
        {
            var exp = new PrefixExpression();
            exp.Token = this._curToken;
            exp.Operator = this._curToken.Literal;
            this.NextToken();
            exp.Right = this.ParseExpression(Precedence.PREFIX);
            return exp;
        } 
        #endregion

        #region parseInfixExpression
        private Expression ParseInfixExpression(Expression left) 
        {
            InfixExpression exp = new InfixExpression{
                Token=this._curToken,
                Operator = this._curToken.Literal,
                Left = left,
            };
            Precedence prece = this.CurPrecedence();
            this.NextToken();
            exp.Right = this.ParseExpression(prece);
            return exp;
        }
        #endregion
    }
}
