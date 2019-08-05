using System;
using System.Collections.Generic;
using System.Text;

namespace MonkeyTest.Ast
{
    using NUnit.Framework;
    using Monkey.Token;
    using Monkey.Ast;

    [TestFixture]
    public class AstTests
    {
        [Test]
        public void TestAstString()
        {
            var letStmt = new LetStatement();
            letStmt.Token = Token.Create(TokenType.LET, "let");
            letStmt.Name = new Identifier(Token.Create(TokenType.IDENT, "myVar"), "myVar");
            letStmt.Value = new Identifier(Token.Create(TokenType.IDENT, "anotherVar"), "anotherVar");
            Assert.AreEqual("let myVar = anotherVar;", letStmt.ToString());
        }
    }
}
