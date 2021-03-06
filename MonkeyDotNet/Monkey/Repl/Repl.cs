﻿using System.IO;
using System;
namespace Monkey.Repl
{
    using Lexer;
    using Parser;
    using Evaluator;
    public class Repl
    {
        private readonly static string _PROMPT = ">> ";
        public static void Start(TextReader reader, TextWriter writer)
        {
            var env = new Object.Environment();
            while (true)
            {
                writer.Write(_PROMPT);
                var input = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(input)){
                    return;
                }
                try
                {
                    var lexer = Lexer.Create(input);
                    var parser = new Parser(lexer);
                    var program = parser.ParseProgram();
                    Object.Object evaluated = Evaluator.Eval(program, env);
                    if(evaluated !=null)
                    {
                        writer.Write(evaluated.Inspect());
                        writer.Write(Environment.NewLine);
                    }
                }
                catch (ParserException e)
                {
                    writer.Write(e.Message);
                    writer.Write(Environment.NewLine);
                }
            }
        }
    }
}
