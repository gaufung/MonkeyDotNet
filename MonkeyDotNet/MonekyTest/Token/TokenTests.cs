namespace MonkeyTest.Token
{
    using NUnit.Framework;
    using Monkey.Token;

    [TestFixture]
    public class TokenTests
    {
        [Test]
        public void TestLookUp()
        {
            var tests = new[]
            {
                 new {Input="fn", Expect=TokenType.FUNCTION},
                 new {Input="let", Expect=TokenType.LET},
                 new {Input="else", Expect=TokenType.ELSE},
                 new {Input="true", Expect=TokenType.TRUE},
                 new {Input="false", Expect=TokenType.FALSE},
                 new {Input="return", Expect=TokenType.RETURN},
                 new {Input="for", Expect=TokenType.FOR},
                 new {Input="moneky", Expect=TokenType.IDENT},
            };
            foreach (var test in tests)
            {
                Assert.AreEqual(Token.LookupIdentifier(test.Input), test.Expect,string.Format("{0} is not {1}", test.Input, test.Expect));
            }
        }
    }
}
