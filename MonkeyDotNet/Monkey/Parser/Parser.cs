namespace Monkey.Parser
{
    using Lexer;
    using Token;
    using Ast;
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Globalization;

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
            {TokenType.LPAREN, Precedence.CALL},
            {TokenType.LBRACKET, Precedence.INDEX },
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
            this.RegisterPrefix(TokenType.TRUE, this.ParseBoolean);
            this.RegisterPrefix(TokenType.FALSE, this.ParseBoolean);
            this.RegisterPrefix(TokenType.LPAREN, this.ParseGroupedExpression);
            this.RegisterPrefix(TokenType.IF, this.ParseIfExpression);
            this.RegisterPrefix(TokenType.FUNCTION, this.ParseFunctionLiteral);
            this.RegisterPrefix(TokenType.STRING, this.ParseStringLiteral);
            this.RegisterPrefix(TokenType.LBRACKET, this.ParseArrayLiteral);
            this.RegisterPrefix(TokenType.LBRACE, this.ParseHashLiteral);
            this.RegisterInfix(TokenType.PLUS, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.MINUS, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.SLASH, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.ATERISK, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.EQ, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.NOT_EQ, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.LT, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.GT, this.ParseInfixExpression);
            this.RegisterInfix(TokenType.LPAREN, this.ParseCallExpression);
            this.RegisterInfix(TokenType.LBRACKET, this.ParseIndexExpression);

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
                Statement stmt = this.ParseStatement();
                if(stmt!= null)
                {
                    program.Statements.Add(stmt);
                }
                this.NextToken();
            }
            return program;
        }

        private Statement ParseStatement()
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
            else
            {
                throw new ParserException($"Unsupport token: {this._curToken.Type}");
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
                throw new ParserException($"Expect IDENT Token but got {this._peekToken.Literal}");
            }

            stmt.Name = new Identifier(this._curToken, this._curToken.Literal);

            if (!this.ExpectPeek(TokenType.ASSIGN))
            {
                throw new ParserException($"Expect ASSIGN Token but got {this._peekToken.Literal}");
            }

            this.NextToken();

            stmt.Value = this.ParseExpression(Precedence.LOWEST);

            if(this.PeekTokenIs(TokenType.SEMICOLON)) 
            {
                this.NextToken();
            }
            return stmt;

        }
        #endregion

        #region parserReturnStatement
        private ReturnStatement ParseReturnStatement()
        {
            var stmt = new ReturnStatement(this._curToken);
            this.NextToken();

            stmt.ReturnValue = this.ParseExpression(Precedence.LOWEST);
            //todo: skip the expression until we encounter a semicolon
            if (this.PeekTokenIs(TokenType.SEMICOLON))
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

        #region parseBoolean
        private Expression ParseBoolean()
        {
            return new Ast.Boolean{Token=this._curToken, Value=this.CurTokenIs(TokenType.TRUE)};
        }
        #endregion

        #region parseGroupedExpression
        private Expression ParseGroupedExpression()
        {
            this.NextToken();
            var exp = this.ParseExpression(Precedence.LOWEST);
            if (!this.ExpectPeek(TokenType.RPAREN))
            {
                throw new ParserException($"Expect peek {TokenType.RPAREN}, but got {this._peekToken}");
            }
            return exp;
        }
        #endregion


        #region parseIfExpression
        private Expression ParseIfExpression()
        {
            var expression = new IfExpression();
            expression.Token = this._curToken;
            if (!this.ExpectPeek(TokenType.LPAREN))
            {
                throw new ParserException($"Expect peek a {TokenType.LPAREN} but got {this._peekToken}");
            }
            this.NextToken();
            expression.Condition = this.ParseExpression(Precedence.LOWEST);

            if (!this.ExpectPeek(TokenType.RPAREN))
            {
                throw new ParserException($"Expect peek a {TokenType.RPAREN}, but got {this._peekToken}");
            }
            if (!this.ExpectPeek(TokenType.LBRACE))
            {
                throw new ParserException($"Expect peek a {TokenType.LBRACE}, but got {this._peekToken}");
            }
            expression.Consequence = this.ParseBlockStatement();

            if (this.PeekTokenIs(TokenType.ELSE))
            {
                this.NextToken();
                if (!this.ExpectPeek(TokenType.LBRACE))
                {
                    throw new ParserException($"Expect peek a {TokenType.LBRACE}, but got {this._peekToken}");
                }
                expression.Alternative = this.ParseBlockStatement();
            }
            return expression;
        }


        #endregion

        #region parse block statemnt
        private BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement();
            block.Statements = new List<Statement>();
            this.NextToken();
            while (!this.CurTokenIs(TokenType.RBRACE))
            {
                var stmt = this.ParseStatement();
                block.Statements.Add(stmt);
                this.NextToken();
            }
            return block;
        }
        #endregion

        #region parse funticon literal
        private Expression ParseFunctionLiteral() 
        {
            var lit = new FunctionLiteral();
            lit.Token = this._curToken;
            if(!this.ExpectPeek(TokenType.LPAREN))
            {
                throw new ParserException($"expect peek {TokenType.LPAREN} but got {this._peekToken}");
            }
            lit.Parameters = this.ParseFunctionParameters();

            if(!this.ExpectPeek(TokenType.LBRACE))
            {
                throw new ParserException($"expect peek {TokenType.LBRACE} but got {this._peekToken}");
            }
            lit.Body = this.ParseBlockStatement();
            return lit;
        }

        private IList<Identifier> ParseFunctionParameters()
        {
            var identifiers = new List<Identifier>();
            if(this.PeekTokenIs(TokenType.RPAREN))
            {
                this.NextToken();
                return identifiers;
            }
            this.NextToken();
            var ident = new Identifier(this._curToken, this._curToken.Literal);

            identifiers.Add(ident);
            while (this.PeekTokenIs(TokenType.COMMA))
            {
                this.NextToken();
                this.NextToken();
                ident = new Identifier(this._curToken, this._curToken.Literal);
                identifiers.Add(ident);
            }
            if(!this.ExpectPeek(TokenType.RPAREN))
            {
                throw new ParserException($"expect peek token is {TokenType.RPAREN}, but got {this._peekToken}");
            }
            return identifiers;
        }
        #endregion

        #region parse call expression
        private Expression ParseCallExpression(Expression function) 
        {
            var exp = new CallExpression();
            exp.Token = this._curToken;
            exp.Function = function;
            exp.Arguments = this.ParseExpressionList(TokenType.RPAREN);
            return exp;
        }  
        private IList<Expression> ParseCallArguments() 
        {
            var args = new List<Expression>();
            if(this.PeekTokenIs(TokenType.RPAREN)) 
            {
                this.NextToken();
                return args;
            }

            this.NextToken();
            args.Add(this.ParseExpression(Precedence.LOWEST));

            while(this.PeekTokenIs(TokenType.COMMA)) 
            {
                this.NextToken();
                this.NextToken();
                args.Add(this.ParseExpression(Precedence.LOWEST));
            }
            if(!this.ExpectPeek(TokenType.RPAREN))
            {
                throw new ParserException($"expect peek {TokenType.RPAREN} but got {this._peekToken}");
            }
            return args;
        }
        #endregion

        #region parse string
        private Expression ParseStringLiteral()
        {
            return new StringLiteral { Token = this._curToken, Value = this._curToken.Literal };
        }
        #endregion

        #region parse array

        private Expression ParseArrayLiteral()
        {
            var array = new ArrayLiteral();
            array.Token = this._curToken;
            array.Elements = this.ParseExpressionList(TokenType.RBRACKET);
            return array;
        }

        private IList<Expression> ParseExpressionList(TokenType end)
        {
            var list = new List<Expression>();
            if(this.PeekTokenIs(end))
            {
                this.NextToken();
                return list;
            }

            this.NextToken();
            list.Add(this.ParseExpression(Precedence.LOWEST));
            while(this.PeekTokenIs(TokenType.COMMA))
            {
                this.NextToken();
                this.NextToken();
                list.Add(this.ParseExpression(Precedence.LOWEST));
            }
            if(!this.ExpectPeek(end))
            {
                throw new ParserException($"expect peek {end} but got {this._peekToken}");
            }
            return list;

        }
        #endregion

        #region parse index expression
        private Expression ParseIndexExpression(Expression left)
        {
            var exp = new IndexExpression();
            exp.Token = this._curToken;
            exp.Left = left;
            this.NextToken();
            exp.Index = this.ParseExpression(Precedence.LOWEST);
            if(!this.ExpectPeek(TokenType.RBRACKET))
            {
                throw new ParserException($"want to {TokenType.RBRACKET} got {this._peekToken}");
            }
            return exp;
        }
        #endregion


        #region parse hash literal
        private Ast.Expression ParseHashLiteral()
        {
            var hash = new HashLiteral { Token = this._curToken };
            hash.Pairs = new Dictionary<Expression, Expression>();
            while (!this.PeekTokenIs(TokenType.RBRACE))
            {
                this.NextToken();
                var key = this.ParseExpression(Precedence.LOWEST);
                if (!this.ExpectPeek(TokenType.COLON))
                {
                    throw new ParserException($"want {TokenType.COLON} but got {this._peekToken}");
                }
                this.NextToken();
                var value = this.ParseExpression(Precedence.LOWEST);
                hash.Pairs[key] = value;
                if(!this.PeekTokenIs(TokenType.RBRACE) && !this.ExpectPeek(TokenType.COMMA))
                {
                    throw new ParserException($"want {TokenType.RBRACE} or {TokenType.COMMA}  but got {this._peekToken}");
                }
            }
            if(!this.ExpectPeek(TokenType.RBRACE))
            {
                throw new ParserException($"want {TokenType.RBRACE} but got {this._peekToken}");
            }
            return hash;
        }
        #endregion
    }
}
