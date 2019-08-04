using System.IO;
namespace Monkey.Repl
{
    using Lexer;
    using Token;
    public class Repl
    {
        private readonly static string _PROMPT = ">> ";
        public static void Start(TextReader reader, TextWriter writer)
        {
            while (true)
            {
                writer.Write(_PROMPT);
                var input = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(input)){
                    return;
                }
                var lexer = Lexer.Create(input);
                for(Token token = lexer.NextToken(); token.Type != TokenType.EOF; token = lexer.NextToken())
                {
                    writer.WriteLine($"Token Type:{token.Type}, Token Literal:{token.Literal}");
                }
            }
        }
    }
}
