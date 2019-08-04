using System;

namespace MonkeyCLI
{
    using Monkey.Repl;
    class Program
    {
        static void Main(string[] args)
        {
            var user = System.Environment.UserName;
            Console.WriteLine($"Hello {user}, welcome to Monkey program language!");
            Console.WriteLine("Feel free to type any command.");
            Repl.Start(System.Console.In, System.Console.Out);
        }
    }
}
