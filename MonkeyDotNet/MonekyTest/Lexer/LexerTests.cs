namespace MonkeyTest.Lexer
{
    using NUnit.Framework;
    using Monkey.Lexer;
    using Monkey.Token;

    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void TestNextToken1()
        {
            var input = @"=+(){},;";
            var tests = new[]
            {
                new {ExpectType=TokenType.ASSIGN, ExpectLiteral="="},
                new {ExpectType=TokenType.PLUS, ExpectLiteral="+"},
                new {ExpectType=TokenType.LPAREN, ExpectLiteral="("},
                new {ExpectType=TokenType.RPAEN, ExpectLiteral=")"},
                new {ExpectType=TokenType.LBRACE, ExpectLiteral="{"},
                new {ExpectType=TokenType.RBRACE, ExpectLiteral="}"},
                new {ExpectType=TokenType.COMMA, ExpectLiteral=","},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
            };
            var lexer = Lexer.Create(input);
            for(int i=0; i<tests.Length; i++)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(token.Type, tests[i].ExpectType,
                    string.Format("test:{0} - TokenType wrong, expect={1}, got={2}", i, tests[i].ExpectType, token.Type));
                Assert.AreEqual(token.Literal, tests[i].ExpectLiteral,
                    string.Format("test:{0} - TokenLiteral wrong, expect={1}, got={2}", i, tests[i].ExpectLiteral, token.Literal));
            }
        }

        [Test]
        public void TestNextToken2()
        {
            string input = @"let five=5;
let ten =10;
let add = fn(x, y){
  x+y;
};
let result = add(five, ten);
!-/*5;
5<10>5;
if(5<10){
	return true;
}else{
	return false;
}
10 == 10;
10 != 9;
""foobar""
""foo bar""
[1, 2];
            { ""foo"":""bar""}
for

            ";

            var tests = new[]
            {
                new {ExpectType=TokenType.LET, ExpectLiteral="let"},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="five"},
                new {ExpectType=TokenType.ASSIGN, ExpectLiteral="="},
                new {ExpectType=TokenType.INT, ExpectLiteral="5"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.LET, ExpectLiteral="let"},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="ten"},
                new {ExpectType=TokenType.ASSIGN, ExpectLiteral="="},
                new {ExpectType=TokenType.INT, ExpectLiteral="10"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.LET, ExpectLiteral="let"},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="add"},
                new {ExpectType=TokenType.ASSIGN, ExpectLiteral="="},
                new {ExpectType=TokenType.FUNCTION, ExpectLiteral="fn"},
                new {ExpectType=TokenType.LPAREN, ExpectLiteral="("},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="x"},
                new {ExpectType=TokenType.COMMA, ExpectLiteral=","},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="y"},
                new {ExpectType=TokenType.RPAEN, ExpectLiteral=")"},
                new {ExpectType=TokenType.LBRACE, ExpectLiteral="{"},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="x"},
                new {ExpectType=TokenType.PLUS, ExpectLiteral="+"},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="y"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.RBRACE, ExpectLiteral="}"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.LET, ExpectLiteral="let"},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="result"},
                new {ExpectType=TokenType.ASSIGN, ExpectLiteral="="},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="add"},
                new {ExpectType=TokenType.LPAREN, ExpectLiteral="("},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="five"},
                new {ExpectType=TokenType.COMMA, ExpectLiteral=","},
                new {ExpectType=TokenType.IDENT, ExpectLiteral="ten"},
                new {ExpectType=TokenType.RPAEN, ExpectLiteral=")"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.BANG, ExpectLiteral="!"},
                new {ExpectType=TokenType.MINUS, ExpectLiteral="-"},
                new {ExpectType=TokenType.SLASH, ExpectLiteral="/"},
                new {ExpectType=TokenType.ATERISK, ExpectLiteral="*"},
                new {ExpectType=TokenType.INT, ExpectLiteral="5"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.INT, ExpectLiteral="5"},
                new {ExpectType=TokenType.LT, ExpectLiteral="<"},
                new {ExpectType=TokenType.INT, ExpectLiteral="10"},
                new {ExpectType=TokenType.GT, ExpectLiteral=">"},
                new {ExpectType=TokenType.INT, ExpectLiteral="5"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.IF, ExpectLiteral="if"},
                new {ExpectType=TokenType.LPAREN, ExpectLiteral="("},
                new {ExpectType=TokenType.INT, ExpectLiteral="5"},
                new {ExpectType=TokenType.LT, ExpectLiteral="<"},
                new {ExpectType=TokenType.INT, ExpectLiteral="10"},
                new {ExpectType=TokenType.RPAEN, ExpectLiteral=")"},
                new {ExpectType=TokenType.LBRACE, ExpectLiteral="{"},
                new {ExpectType=TokenType.RETURN, ExpectLiteral="return"},
                new {ExpectType=TokenType.TRUE, ExpectLiteral="true"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.RBRACE, ExpectLiteral="}"},
                new {ExpectType=TokenType.ELSE, ExpectLiteral="else"},
                new {ExpectType=TokenType.LBRACE, ExpectLiteral="{"},
                new {ExpectType=TokenType.RETURN, ExpectLiteral="return"},
                new {ExpectType=TokenType.FALSE, ExpectLiteral="false"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.RBRACE, ExpectLiteral="}"},
                new {ExpectType=TokenType.INT, ExpectLiteral="10"},
                new {ExpectType=TokenType.EQ, ExpectLiteral="=="},
                new {ExpectType=TokenType.INT, ExpectLiteral="10"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.INT, ExpectLiteral="10"},
                new {ExpectType=TokenType.NOT_EQ, ExpectLiteral="!="},
                new {ExpectType=TokenType.INT, ExpectLiteral="9"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.STRING, ExpectLiteral="foobar"},
                new {ExpectType=TokenType.STRING, ExpectLiteral="foo bar"},
                new {ExpectType=TokenType.LBRACKET, ExpectLiteral="["},
                new {ExpectType=TokenType.INT, ExpectLiteral="1"},
                new {ExpectType=TokenType.COMMA, ExpectLiteral=","},
                new {ExpectType=TokenType.INT, ExpectLiteral="2"},
                new {ExpectType=TokenType.RBRACKET, ExpectLiteral="]"},
                new {ExpectType=TokenType.SEMICOLON, ExpectLiteral=";"},
                new {ExpectType=TokenType.LBRACE, ExpectLiteral="{"},
                new {ExpectType=TokenType.STRING, ExpectLiteral="foo"},
                new {ExpectType=TokenType.COLON, ExpectLiteral=":"},
                new {ExpectType=TokenType.STRING, ExpectLiteral="bar"},
                new {ExpectType=TokenType.RBRACE, ExpectLiteral="}"},
                new {ExpectType=TokenType.FOR, ExpectLiteral="for"},
            };
            var lexer = Lexer.Create(input);
            for(int i=0; i<tests.Length; i++)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(token.Type, tests[i].ExpectType,
                    string.Format("test:{0} - TokenType wrong, expect={1}, got={2}", i, tests[i].ExpectType, token.Type));
                Assert.AreEqual(token.Literal, tests[i].ExpectLiteral,
                    string.Format("test:{0} - TokenLiteral wrong, expect={1}, got={2}", i, tests[i].ExpectLiteral, token.Literal));
            }
        }
    }
}
