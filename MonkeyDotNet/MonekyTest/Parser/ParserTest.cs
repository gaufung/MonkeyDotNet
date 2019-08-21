using System;
using System.Collections.Generic;
namespace MonkeyTest.Parser
{
    using NUnit.Framework;
    using Monkey.Parser;
    using Monkey.Ast;
    using Monkey.Lexer;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Extensions.DependencyModel;

    [TestFixture]
    public class ParserTest
    {

        class LetTestCase 
        {
            public string Input { get; set; }
            public string ExpectedIdentifer { get; set; }

            public object ExpectedValue { get; set; }
        }

        [Test]
        public void TestLetStatements()
        {
            var tests = new []
            {
                new LetTestCase{Input="let x = 5;", ExpectedIdentifer="x", ExpectedValue=5},
                new LetTestCase{Input="let y = true;", ExpectedIdentifer="y", ExpectedValue = true},
                new LetTestCase{Input="let foobar=y", ExpectedIdentifer="foobar", ExpectedValue="y"},
            };
            foreach (var tt in tests)
            {
                var lexer = Lexer.Create(tt.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                Assert.AreEqual(1, program.Statements.Count, $"program.Statements.Count is not 1, but got {program.Statements.Count}");

                var stmt = program.Statements[0];
                TestLetStatement(stmt, tt.ExpectedIdentifer);
                var val = (stmt as LetStatement).Value;
                TestLiteralExpression(val, tt.ExpectedValue);
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
            if(expected.GetType()==typeof(string))
            {
                TestIdentifier(exp, (expected as string));
            }else if (expected.GetType()==typeof(bool)) 
            {
                TestBooleanLiteral(exp, (bool)expected);
            }else if (expected.GetType()==typeof(long))
            {
                TestIntegerLiteral(exp, (long)expected);
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
                new {Input="1+(2+3)+4", Expect="((1 + (2 + 3)) + 4)"},
                new {Input="(5+5)*2", Expect="((5 + 5) * 2)"},
                new {Input="2 / (5 + 5)",Expect="(2 / (5 + 5))"},
                new {Input="-(5+5)", Expect="(-(5 + 5))"},
                new {Input="!(true==true)", Expect="(!(true == true))"},
                new {Input="a + add(b*c) + d", Expect="((a + add((b * c))) + d)"},
                new {Input="add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", Expect="add(a,b,1,(2 * 3),(4 + 5),add(6,(7 * 8)))"},
                new {Input="add(a + b + c * d / f + g)", Expect="add((((a + b) + ((c * d) / f)) + g))"},
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

        [Test]
        public void TestIfExpression()
        {
            var input = "if (x<y) {x}";
            var lexer = Lexer.Create(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            Assert.AreEqual(1, program.Statements.Count);

            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");

            var exp = stmt.Expression as IfExpression;
            Assert.IsNotNull(exp, "stmt.Expression is not IfExpression.");

            TestInfixExpression(exp.Condition, "x", "<", "y");

            Assert.AreEqual(1, exp.Consequence.Statements.Count, 
                $"consequence is not 1 statement, but got {exp.Consequence.Statements.Count}");

            var consequecne = exp.Consequence.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(consequecne, "Statement[0] is not ExpressionStatement");

            TestIdentifier(consequecne.Expression, "x");

            Assert.IsNull(exp.Alternative, "exp.Alternative is not null");
        }

        [Test]
        public void TestFunctionLiteralParsing()
        {
            var input = "fn(x, y) { x + y; }";
            var lexer = Lexer.Create(input);
            var parse = new Parser(lexer);
            var program = parse.ParseProgram();
            Assert.AreEqual(1, program.Statements.Count);
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, $"program.Statement[0] is not ExpressionStatement");
            var function = stmt.Expression as FunctionLiteral;
            Assert.IsNotNull(function, "stmt.Expression is not FunctionLiteral");
            Assert.AreEqual(2, function.Parameters.Count, $"function.Parameters count is not 2, but got {function.Parameters.Count}");

            TestLiteralExpression(function.Parameters[0], "x");
            TestLiteralExpression(function.Parameters[1], "y");
            Assert.AreEqual(1, function.Body.Statements.Count, $"function.Body.Statements.Count is not 2, but got {function.Body.Statements.Count}");
            var bodyStmt = function.Body.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(bodyStmt, "function.Body.Statement[0] is not ExpressionStatement");

            TestInfixExpression(bodyStmt.Expression, "x", "+", "y");
        }

        [Test]
        public void TestFunctionParameterParsing()
        {
            var tests = new []
            {
                new {Input = "fn(){}", ExpectParamer= new List<string>()},
                new {Input = "fn(x){}", ExpectParamer=new List<string>{"x"}},
                new {Input = "fn(x, y, z){}", ExpectParamer=new List<string>{"x", "y", "z"}},
            };
            foreach (var test in tests)
            {
                var lexer = Lexer.Create(test.Input);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                var stmt = program.Statements[0] as ExpressionStatement;
                var function = stmt.Expression as FunctionLiteral;

                Assert.AreEqual(test.ExpectParamer.Count, function.Parameters.Count,
                $"length parameter wrong. want {test.ExpectParamer.Count}, but got {function.Parameters.Count}");
                for (int i = 0; i < test.ExpectParamer.Count; i++)
                {
                    TestLiteralExpression(function.Parameters[i], test.ExpectParamer[i]);
                }
            }
        }

        [Test]
        public void TestCallExpression() 
        {
            var input = "add(1, 2 * 3, 4 + 5);";
            var lexer = Lexer.Create(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();

            Assert.AreEqual(1, program.Statements.Count, $"program.Statements.Count is not 1, but got {program.Statements.Count}");

            var stmt = program.Statements[0] as ExpressionStatement;

            Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatment");

            var exp = stmt.Expression as CallExpression;

            Assert.IsNotNull(exp, "stmt.Expression is not CallExpression");

            TestIdentifier(exp.Function, "add");

            Assert.AreEqual(3, exp.Arguments.Count, $"exp.Arguments.Count is 3, but got {exp.Arguments.Count}");

            TestLiteralExpression(exp.Arguments[0], 1);
            TestInfixExpression(exp.Arguments[1], 2, "*", 3);
            TestInfixExpression(exp.Arguments[2], 4, "+", 5);
        }


        [Test]
        public void TestStringLiteralExpression()
        {
            var input = "\"hello world\"";
            var lexer = Lexer.Create(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();

            var stmt = program.Statements[0] as ExpressionStatement;
            var literal = stmt.Expression as StringLiteral;
            Assert.IsNotNull(literal, "stmt.Expression is not StringLiteral");

            Assert.AreEqual("hello world", literal.Value);
        }

        [Test]
        public void TestParsingArrayLiteral()
        {
            var input = "[1, 2*2, 3 + 3]";
            var program = new Parser(Lexer.Create(input)).ParseProgram();
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");
            var array = stmt.Expression as ArrayLiteral;
            Assert.IsNotNull(array, "stmt.Expression is not ArrayLiteral");
            Assert.AreEqual(3, array.Elements.Count, $"wrong array elements, want 3, got {array.Elements.Count}");
            TestIntegerLiteral(array.Elements[0], 1);
            TestInfixExpression(array.Elements[1], 2, "*", 2);
            TestInfixExpression(array.Elements[2], 3, "+", 3);
        }

        [Test]
        public void TestParsingIndexExpression()
        {
            var input = "myArray[1+1]";
            var program = new Parser(Lexer.Create(input)).ParseProgram();
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");
            var indexExp = stmt.Expression as IndexExpression;
            Assert.IsNotNull(indexExp, "stmt.Expression is not IndexExpression");
            TestIdentifier(indexExp.Left, "myArray");
            TestInfixExpression(indexExp.Index, 1, "+", 1);
        }

        [Test]
        public void TestParsingHashLiteralStringKeys()
        {
            var input = "{\"one\" : 1, \"two\": 2, \"three\":3}";
            var program = new Parser(Lexer.Create(input)).ParseProgram();
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, "stmt.Expression is not HashLiteral");
            Assert.AreEqual(3, hash.Pairs.Count, $"hash.pair has wrong legnth, want 3, but got {hash.Pairs.Count}");
            var expected = new Dictionary<string, long>()
            {
                {"one", 1 },
                {"two", 2 },
                {"three",3 },
            };
            foreach (KeyValuePair<Expression, Expression> pair in hash.Pairs)
            {
                var literal = pair.Key as StringLiteral;
                Assert.IsNotNull(literal, "pair.Key is not StringLiteral");
                var expectedValue = expected[literal.ToString()];
                TestIntegerLiteral(pair.Value, expectedValue);
            }
        }

        [Test]
        public void TestParsingEmptyHashLiteral()
        {
            var input = "{}";
            var program = new Parser(Lexer.Create(input)).ParseProgram();
            var stmt = program.Statements[0] as ExpressionStatement;
            Assert.IsNotNull(stmt, "program.Statements[0] is not ExpressionStatement");
            var hash = stmt.Expression as HashLiteral;
            Assert.IsNotNull(hash, "stmt.Expression is not HashLiteral");
            Assert.AreEqual(0, hash.Pairs.Count, $"hash.pair has wrong legnth, want 0, but got {hash.Pairs.Count}");
        }
    }
}
