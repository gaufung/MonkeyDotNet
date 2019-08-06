namespace MonkeyTest.Parser
{
    using NUnit.Framework;
    using Monkey.Parser;
    using Monkey.Ast;
    using Monkey.Lexer;

    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void TestLetStatements()
        {
            var input = @"
let x = 5;
let y = 10;
let foobar = 838383;
";
            Lexer lexer = Lexer.Create(input);
            var parser = new Parser(lexer);
            Program program = parser.ParseProgram();
            Assert.IsNotNull(program, "program is null");
            Assert.AreEqual(3, program.Statements.Count, $"program statement count is not 3, but got {program.Statements.Count}");

            var test = new[] { "x", "y", "foobar" };
            for(int i =0; i <test.Length; i++)
            {
                var smst = program.Statements[i];
                TestLetStatement(smst, test[i]);

            }
        }

        private void TestLetStatement(Statement smst, string name)
        {
            Assert.AreEqual("let", smst.TokenLiteral(), $"statment.TokenLiteral is not let , but got {smst.TokenLiteral()}");
            LetStatement lsmst = smst as LetStatement;
            Assert.IsNotNull(lsmst, "smt is not LetStatement");
            Assert.AreEqual(name, lsmst.Name.Value, $"lsmst' name value is not {name}, but got {lsmst.Name.Value}");
            Assert.AreEqual(name, lsmst.Name.TokenLiteral(), $"lsmst' name tokenliteral is not {name}, but got {lsmst.Name.Value}");
        }


        [Test]
        public void TestReturnStatements()
        {
            var input = @"
return 5;
return 10;
return 993322;
";
            Lexer lexer = Lexer.Create(input);
            var parser = new Parser(lexer);
            Program program = parser.ParseProgram();
            Assert.AreEqual(3, program.Statements.Count, $"program.Statements does not contain 3 statement, but got={program.Statements.Count}");
            foreach (var stmt in program.Statements)
            {
                var returnStatement = stmt as ReturnStatement;
                Assert.IsNotNull(returnStatement, "stmt should not be null");
                Assert.AreEqual("return", returnStatement.TokenLiteral(), $"returnStmt.TokenLiteral not 'return', but got={returnStatement.TokenLiteral()}");
            }
        }

        [Test]
        public void TestIdentifierExpression()
        {
            var input = @"foobar;";

            Lexer lexer = Lexer.Create(input);

            Parser p = new Parser(lexer);
            Program program = p.ParseProgram();

            Assert.AreEqual(1, program.Statements.Count, $"program has not enough statements, but got={program.Statements.Count}");
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "stmt is not ExpressionStatement");
            var ident = stmt.Expression as Identifier;
            Assert.IsNotNull(ident, "exp is not Identifier.");
            Assert.AreEqual("foobar", ident.Value, $"ident.Value is not foobar, but got {ident.Value}");
            Assert.AreEqual("foobar", ident.TokenLiteral(), $"ident.TokenLiteral is not foobar, but got {ident.TokenLiteral()}");
        }

        [Test]
        public void TestIntegerLiteralExpression()
        {
            var input = @"5;";
            Lexer lexer = Lexer.Create(input);
            Parser p = new Parser(lexer);
            Program program = p.ParseProgram();
            Assert.AreEqual(1, program.Statements.Count, $"program has not enough statements. but got={program.Statements.Count}");
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "stmt is not ExpressionStatement");
            var integer = stmt.Expression as IntegerLiteral;
            Assert.IsNotNull(integer, $"stmt.Expression is not Integerliteral");
            Assert.AreEqual(5, integer.Value, $"integer value is not 5, but got {integer.Value}");

        }

        [Test]
        public void TestParsingPrefixExpression()
        {
            var tests = new[]
            {
                new {Input="!5", Operator="!", IntegeValue=5},
                new {Input="-15", Operator="-", IntegeValue=15},
            };
            foreach (var test in tests)
            {
                var lexer = Lexer.Create(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                Assert.AreEqual(1, program.Statements.Count, $"program has not enough statement. but got={program.Statements.Count}");
                var stmt = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");
                var prefixExpression = stmt.Expression as PrefixExpression;
                Assert.IsNotNull(prefixExpression, "stmt.Expression is not PrefixExpression");
                Assert.AreEqual(test.Operator, prefixExpression.Operator, $"prefixExpression.Operator is not {test.Operator} but got {prefixExpression.Operator}");
                TestIntegerLiteral(prefixExpression.Right, test.IntegeValue);
            }
        }

        private void TestIntegerLiteral(Expression exp, long val)
        {
            var inte = exp as IntegerLiteral;
            Assert.IsNotNull(inte, "exp is not IntegerLiteral");
            Assert.AreEqual(val, inte.Value, $"integer value is not {val} but got {inte.Value}");
        }
       
    }
}
