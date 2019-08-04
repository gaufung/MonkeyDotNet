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


       
    }
}
