using System;
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

        public void TestParsingInfixExpression()
        {
            var infixTests = new[]
            {

                new {Input="5+5", LeftValue=5, Operator="+", RightValue=5},
                new {Input="5-5", LeftValue=5, Operator="-", RightValue=5},
                new {Input="5*5", LeftValue=5, Operator="*", RightValue=5},
                new {Input="5/5", LeftValue=5, Operator="/", RightValue=5},
                new {Input="5>5", LeftValue=5, Operator=">", RightValue=5},
                new {Input="5<5", LeftValue=5, Operator="<", RightValue=5},
                new {Input="5==5", LeftValue=5, Operator="==", RightValue=5},
                new {Input="5!=5", LeftValue=5, Operator="!=", RightValue=5},
            };
            foreach (var test in infixTests)
            {
                var lexer = Lexer.Create(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                Assert.AreEqual(1, program.Statements.Count, $"prgoram.Statements does not contain 1 statement(s), but got {program.Statements.Count}");

                var stmt = program.Statements[0] as ExpressionStatement;
                Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");
                var exp = stmt.Expression as InfixExpression;
                Assert.IsNotNull(exp, "stmt.Expression is not InfixExpression");
                TestIntegerLiteral(exp.Left, test.LeftValue);
                Assert.AreEqual(test.Operator, exp.Operator);
                TestIntegerLiteral(exp.Right, test.RightValue);

            }
        }

        private void TestIdentifier(Expression exp, string value)
        {
            var ident = exp as Identifier;
            Assert.IsNotNull(ident);
            Assert.AreEqual(value, ident.Value, $"identifier.Value is not {value}, but got {ident.Value}");
            Assert.AreEqual(value, ident.TokenLiteral(), $"identifier.TokenLiteral not {value}, but got {ident.TokenLiteral()}");
        }

        private void TestLiteralExpression(Expression exp, object expected)
        {
            string str = expected as String;
            if(str!=null){
                TestIdentifier(exp, str);
                return;
            }
            bool outbo;
            if(bool.TryParse(expected.ToString(), out outbo))
            {
                TestBooleanLiteral(exp, outbo);
                return;
            }
            long outlong;
            if(long.TryParse(expected.ToString(), out outlong))
            {
                TestIntegerLiteral(exp, outlong);
            }

        }


        private void TestInfixExpression(Expression exp, object left, string op, object right)
        {
            var opExp = exp as InfixExpression;
            Assert.IsNotNull(opExp, "exp is not InfixExpression");
            
            TestLiteralExpression(opExp.Left, left);
            Assert.AreEqual(op, opExp.Operator, $"exp.Operator is not {opExp.Operator}, but got {op}");

            TestLiteralExpression(opExp.Right, right);
        }

        private void TestBooleanLiteral(Expression exp, bool value)
        {
            var bo = exp as Boolean;
            Assert.IsNotNull(bo, "exp is not Ast.Boolean");
            Assert.AreEqual(value, bo.Value, $"bo.Value is not {value}, but got {bo.Value}");
            Assert.AreEqual(value.ToString(), bo.TokenLiteral(), $"bo.TokenLiteral is not {value.ToString()}, but got {bo.TokenLiteral()}");

        }

        [Test]
        public void TestOperatorPrecedenceParsiong()
        {
            var tests = new[] 
            {
                new {Input="true", Expect="true"},
                new {Input="false", Expect="false"},
                new {Input="3>5 == false", Expect="((3 > 5) == false)"},
                new {Input="3<5==true", Expect="((3 < 5) == true)" },
            };

            foreach (var test in tests)
            {
                var lexer = Lexer.Create(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                Assert.AreEqual(1, program.Statements.Count, $"program.Statement.Count is not 1, but got {program.Statements.Count}");
                var exp = program.Statements[0];
                Assert.AreEqual(test.Expect, exp.ToString(), $"exp.ToString is not {test.Expect}, but got {exp.ToString()}");
            }

        }
    }
}
