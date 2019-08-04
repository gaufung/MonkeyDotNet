namespace Monkey.Parser
{
    using Lexer;
    using Token;
    using Ast;
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _curToken;
        private Token _peekToken;

        public Parser(Lexer lexer)
        {
            this._lexer = lexer;
            this.NextToken();
            this.NextToken();
        }

        private void NextToken()
        {
            this._curToken = this._peekToken;
            this._peekToken = this._lexer.NextToken();
        }

        public Program ParseProgram()
        {
            return null;
        }
    }
}
