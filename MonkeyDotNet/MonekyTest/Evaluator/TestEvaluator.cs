using System;
namespace MonkeyTest.Evaluator
{
    using NUnit.Framework;
    using Monkey.Evaluator;
    using Monkey.Lexer;
    using Monkey.Object;
    using Monkey.Parser;
    using System.Globalization;
    using NUnit.Framework.Interfaces;
    using System.Collections.Generic;
    using NUnit.Framework.Internal;

    [TestFixture]
    public class TestEvaluator
    {
        #region Test Integer evaluation expression
        [Test]
        public void TestEvalIntegerExpression()
        {
            var tests = new[]
            {
                new {Input="5", Expected=5},
                new {Input="10", Expected=10},
                new {Input="-5", Expected=-5},
                new {Input="-10", Expected=-10},
                new {Input="5 + 5 + 5 + 5 - 10", Expected=10},
                new {Input="2*2*2*2*2", Expected=32},
                new {Input="-50 + 100 + -50", Expected=0},
                new {Input="5 * 2 + 10", Expected=20},
                new {Input="5 + 2 * 10", Expected=25},
                new {Input="20 + 2 * -10", Expected=0},
                new {Input="50 / 2 * 2 + 10", Expected=60},
                new {Input="3 * 3 * 3 + 10", Expected=37},
                new {Input="3 * (3*3) + 10", Expected=37},
                new {Input="(5 + 10 * 2 + 15 / 3) * 2 + -10", Expected=50},
            };
            foreach (var test in tests)
            {
                Object evaluted = TestEval(test.Input);
                TestIntegerObject(evaluted, test.Expected);
            }
        }

        private Object TestEval(string input)
        {
            var lexer = Lexer.Create(input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            var env = new Environment();
            return Evaluator.Eval(program, env);
        }

        private void TestIntegerObject(Object obj, long expected)
        {
            var result = obj as Integer;
            Assert.IsNotNull(result, "obj is not Integer.");
            Assert.AreEqual(expected, result.Value, $"obj's Value is not {expected}, but got {result.Value}");
        }
        #endregion

        #region Test Boolean evaluation expression
        [Test]
        public void TestEvalBooleanExpression()
        {
            var tests = new[]
            {
                new {Input="true", Expected=true },
                new {Input="false", Expected=false},
                new {Input="1 < 2", Expected=true},
                new {Input="1 > 2", Expected=false},
                new {Input="1 < 1", Expected=false},
                new {Input="1 > 1", Expected=false},
                new {Input="1 == 1", Expected=true},
                new {Input="1 != 1", Expected=false},
                new {Input="1==2", Expected=false},
                new {Input="1 != 2", Expected=true},
                new {Input="true == true", Expected=true},
                new {Input="false == false", Expected=true},
                new {Input="true == false", Expected=false},
                new {Input="true != false", Expected=true},
                new {Input="false != true", Expected=true},
                new {Input="(1<2) == true", Expected=true},
                new {Input="(1 < 2) == false", Expected=false},
                new {Input="(1 > 2) == true", Expected=false},
                new {Input="(1 > 2) == false", Expected=true},
            };
            foreach (var test in tests)
            {
                var evaluated = TestEval(test.Input);
                TestBooleanObject(evaluated, test.Expected);
            }

        }

        private void TestBooleanObject(Object obj, bool expect)
        {
            var result = obj as Boolean;
            Assert.IsNotNull(result, "obj is not Boolean");
            Assert.AreEqual(expect, result.Value, $"result.Value is not {expect}, but got {result.Value}");
        }
        #endregion

        #region Test Bang Operator
        public void TestBangOperator()
        {
            var tests = new[]
            {
                new {Input="!true", Expect=false },
                new {Input="!false", Expect=true},
                new {Input="!5", Expect=false},
                new {Input="!!true", Expect=true},
                new {Input="!!false", Expect=false},
                new {Input="!!5", Expect=true},
            };
            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Input);
                TestBooleanObject(evaluated, tt.Expect);
            }
        }
        #endregion

        #region TestIfElseExpression

        class IfElseTestCase 
        {
            public string Input { get; set; }

            public long? Expected { get; set; }

            public IfElseTestCase(string input, long? expected)
            {
                Input= input;
                Expected=expected;
            }
        }

        [Test]
        public void TestIfElseExpression()
        {
            var tests = new []
            {
                new IfElseTestCase("if(true) {10}", 10),
                new IfElseTestCase("if(false) {10}", null),
                new IfElseTestCase("if(1) { 10 }", 10),
                new IfElseTestCase("if (1 < 2) {10}", 10),
                new IfElseTestCase("if(1>2){10} else {20}", 20),
                new IfElseTestCase("if (1 < 2) {10} else { 20}", 10),
            };
            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Input);
                if(tt.Expected.HasValue) 
                {
                    TestIntegerObject(evaluated, tt.Expected.Value);
                }
                else
                {
                    Assert.AreEqual(evaluated.Type(), ObjectType.NULL_OBJ);
                }   
            }
        }
        #endregion

        #region TestReturnStatment


        class ReturnTestCase 
        {
            public string Input { get; set; }

            public long Expected { get; set; }

            public ReturnTestCase(string input, long expected)
            {
                Input = input;
                Expected = expected;
            }
        }

        [Test]
        public void TestReturnStatements()
        {
            var tests = new []
            {
                new ReturnTestCase("return 10;", 10),
                new ReturnTestCase("return 10; 9;", 10),
                new ReturnTestCase("return 2 * 5; 9;", 10),
                new ReturnTestCase("9; return 2 * 5; 9;", 10),
                new ReturnTestCase("if(10>1){ if(10>1) { return 10;} return 1;}", 10),
            };
            foreach (var tt in tests)
            {
                Object evaluated = TestEval(tt.Input);
                TestIntegerObject(evaluated, tt.Expected);
            }
        }
        #endregion

        #region TestErrorHandling

        class ErrorTestCase 
        {
            public string Input { get; set; }

            public string Expected { get; set; }

            public ErrorTestCase(string input, string expected)
            {
                Input= input;
                Expected = expected;
            }
        }
        
        [Test]
        public void TestErrorHandling()
        {
            var tests = new []
            {
                new ErrorTestCase("5 + true", "type mismatch: INTEGER_OBJ + BOOLEAN_OBJ"),
                new ErrorTestCase("5 + true; 5;", "type mismatch: INTEGER_OBJ + BOOLEAN_OBJ"),
                new ErrorTestCase("-true", "unknown operator: -BOOLEAN_OBJ"),
                new ErrorTestCase("true + false", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
                new ErrorTestCase("5; true + false; 5", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
                new ErrorTestCase("if (10 > 1) { true + false }", "unknown operator: BOOLEAN_OBJ + BOOLEAN_OBJ"),
                new ErrorTestCase("foobar", "identifier not found: foobar"),
                new ErrorTestCase("\"Hello\" - \"World\"", "unknown operator: STRING_OBJ - STRING_OBJ" ),
            };
            foreach (var tt in tests)
            {
                Object evaluated = TestEval(tt.Input);
                Error errObj = evaluated as Error;
                Assert.IsNotNull(errObj, "no error object returned.");
                Assert.AreEqual( tt.Expected, errObj.Message,
                $"errObj.Message is not {tt.Expected} but got {errObj.Message}");
            }
        }
        #endregion

        #region Test Let Statement

        class LetTestCase
        {
            public string Input { get; set; }

            public long Expected { get; set; }
            public LetTestCase(string input, long expected)
            {
                Input= input;
                Expected = expected;
            }
        }

        [Test]
        public void TestLetStatement()
        {
            var tests = new []
            {
                new LetTestCase("let a = 5; a;", 5),
                new LetTestCase("let a=5 * 5; a;", 25),
                new LetTestCase("let a =5; let b=a; b;", 5),
                new LetTestCase("let a=5; let b =a; let c = a +b + 5; c", 15),
            };
            foreach (var test in tests)
            {
                TestIntegerObject(TestEval(test.Input), test.Expected);
            }
        }
        #endregion

        #region Test function object
        [Test]
        public void TestFunctionObject()
        {
            var input = "fn(x) { x + 2;};";
            Object evaluated = TestEval(input);
            Function fn = evaluated as Function;
            Assert.IsNotNull(fn, "object is not Object.Function.");
            Assert.AreEqual(1, fn.Parameters.Count, $"fn.Parameter.Count is not 1, but got {fn.Parameters.Count}");

            Assert.AreEqual("x", fn.Parameters[0].ToString(), $"fn.Parameter[0] is not 'x' but got {fn.Parameters[0].ToString()}");
            var bodyString = fn.Body.ToString();
            Assert.AreEqual("(x + 2)", bodyString, $"fn.body is not '(x+2)' but got {fn.Body.ToString()}");
        }


        #endregion

        #region TestFunctionApplication

        class FunctionApplicationTestCase 
        {
            public string Input { get; set; }

            public long Expected { get; set; }

            public FunctionApplicationTestCase(string input, long expected)
            {
                Input=input;
                Expected = expected;
            }
        }

        [Test]
        public void TestFunctionApplication()
        {
            var tests = new []
            {
                new FunctionApplicationTestCase("let identify = fn(x) { x;}; identify(5);", 5),
                new FunctionApplicationTestCase("let identify = fn(x) {return x;}; identify(5);", 5),
                new FunctionApplicationTestCase("let double = fn(x) { x * 2 ;}; double(5);", 10),
                new FunctionApplicationTestCase("let add = fn(x, y) {return x + y;}; add(5, 5);", 10),
                new FunctionApplicationTestCase("let add = fn(x, y) { x + y;}; add(5+5, add(5, 5));", 20),
                new FunctionApplicationTestCase("fn(x){x;}(5)", 5),
            };

            foreach (var tt in tests)
            {
                TestIntegerObject(TestEval(tt.Input), tt.Expected);
            }
        }
        #endregion

        #region TestClosures
        [Test]
        [Ignore("have not support closure.")]
        public void TestClosure()
        {
            string input = @"
            let newAdder = fn(x){
                fn(y) {x+y};
            };
            let addTwo = newAdder(2);
            addTwo(2);
            ";
            TestIntegerObject(TestEval(input), 4);
        }
        #endregion


        #region test string literal

        [Test]
        public void TestStringLiteral()
        {
            var input = "\"Hello World!\"";
            var evaluted = TestEval(input);
            var str = evaluted as Strings;
            Assert.IsNotNull(str, "evaluted is not Strings type");
            Assert.AreEqual("Hello World!", str.Value, $"str.Value is not {"Hello World!"}, but got {str.Value}");
        }

        #endregion

        #region Test string concatenation
        [Test]
        public void TestStringConcatenation()
        {
            var input = "\"Hello\" + \" \" + \"World!\"";
            var evaluated = TestEval(input);
            var str = evaluated as Strings;
            Assert.IsNotNull(str, "evaluted is not string type");
            Assert.AreEqual("Hello World!", str.Value, $"str.Value is not {"Hello World"}, but got {str.Value}");
        }
        #endregion

        #region test built function

        class BuiltinFunctionTestCase
        {
            public string Input { get; set; }
            public object Expected { get; set; }
        }
        [Test]
        public void TestBuiltinFunction()
        {
            var tests = new[]
            {
                new BuiltinFunctionTestCase{Input="len(\"\")", Expected=0L},
               new BuiltinFunctionTestCase{Input=$"len(\"four\")", Expected=4L},
                new BuiltinFunctionTestCase{Input=$"len(\"hello world\")", Expected=11L},
                new BuiltinFunctionTestCase{Input=$"len(1)", Expected="argument to len not supported got Monkey.Object.Integer"},
                new BuiltinFunctionTestCase{Input=$"len(\"one\", \"two\")", Expected="wrong number of the arguments. want 1, got 2"},

            };
            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Input);
                if(tt.Expected.GetType() == typeof(long))
                {
                    TestIntegerObject(evaluated, (long)tt.Expected);
                }
                if(tt.Expected.GetType()==typeof(string))
                {
                    var error = evaluated as Error;
                    Assert.IsNotNull(error, "evaluated is not Error");
                    Assert.AreEqual((string)tt.Expected, error.Message, $"Error message error, want {tt.Expected} got {error.Message}");
                }
            }
        }
        #endregion

        #region Test Array
        [Test]
        public void TestEvaluateArray()
        {
            var input = "[1, 2*2, 3+3]";
            var evaluted = TestEval(input);
            var result = evaluted as Array;
            Assert.IsNotNull(result, "evaluted is not Array");
            Assert.AreEqual(3, result.Elements.Count, $"result.Elements.Count is not 3, but got {result.Elements.Count}");
            TestIntegerObject(result.Elements[0], 1);
            TestIntegerObject(result.Elements[1], 4);
            TestIntegerObject(result.Elements[2], 6);
        }
        #endregion

        #region Test Index operator

        class ArrayIndexTestCase
        {
            public string Input { get; set; }

            public object Expected { get; set; }

            public ArrayIndexTestCase(string input, object expected)
            {
                Input = input;
                Expected = expected;
            }
        }

        private void TestNullObject(Object obj)
        {
            Assert.AreSame(Evaluator.NULL, obj);
        }

        [Test]
        public void TestArrayIndexExpression()
        {
            var tests = new ArrayIndexTestCase[]
            {
                new ArrayIndexTestCase("[1, 2, 3][0]", 1L),
                new ArrayIndexTestCase("[1, 2, 3][1]", 2L),
                new ArrayIndexTestCase("[1, 2, 3][2]", 3L),
                new ArrayIndexTestCase("let i=0; [1][i]", 1L),
                new ArrayIndexTestCase("[1, 2, 3][1+1]", 3L),
                new ArrayIndexTestCase("let myArray =[1, 2, 3]; myArray[2];", 3L),
                new ArrayIndexTestCase("let myArray = [1, 2, 3]; myArray[0] + myArray[1] + myArray[2]", 6L),
                new ArrayIndexTestCase("let myArray = [1, 2, 3]; let i = myArray[0]; myArray[i]", 2L),
                new ArrayIndexTestCase("[1, 2, 3][3]", null),
                new ArrayIndexTestCase("[1, 2, 3][-1]", null),
            };

            foreach (var tt in tests)
            {
                var evaluated = TestEval(tt.Input);
                if(tt.Expected!=null)
                {
                    TestIntegerObject(evaluated, (long)tt.Expected);
                }
                else
                {
                    TestNullObject(evaluated);
                }
            }
        }
        #endregion

        #region Test Hash Literal
        [Test]
        public void TEstHashLiterals()
        {
            var input = "let two = \"two\"; { \"one\": 10 -9, two: 1+1, \"thr\" + \"ee\": 6 / 2, 4 : 4, true: 5, false:6 }";
            var evaluted = TestEval(input);
            var result = evaluted as Hash;
            Assert.IsNotNull(result, "evaluted is not Hash");
            var expected = new Dictionary<HashKey, long>()
            {
                {new Strings{Value="one"}.HashKey(), 1},
                {new Strings{Value="two"}.HashKey(), 2},
                {new Strings{Value="three"}.HashKey(), 3},
                {new Integer{Value=4}.HashKey(), 4},
                {Evaluator.TRUE.HashKey(), 5 },
                {Evaluator.FALSE.HashKey(), 6 },
            };
            Assert.AreEqual(expected.Count, result.Pairs.Count, $"Hash has wrong number of pairs, want {expected.Count}, got {result.Pairs.Count}");

            foreach (KeyValuePair<HashKey, long> pair in expected)
            {
                Assert.IsTrue(result.Pairs.ContainsKey(pair.Key));
                TestIntegerObject(result.Pairs[pair.Key].Value, pair.Value);
            }
        }
        #endregion


        #region Test Hash index

        class HashIndexTestCase
        {
            public string Input { get; set; }

            public object Expected { get; set; }

            public HashIndexTestCase(string input, object expected)
            {
                Input = input;
                Expected = expected;
            }
        }


        [Test]
        public void TestHashIndexExpression()
        {
            var tests = new[]
            {
                new HashIndexTestCase("{\"foo\" : 5}[\"foo\"]", 5L),
                new HashIndexTestCase("{\"foo\" : 5}[\"bar\"]", null),
                new HashIndexTestCase("let key = \"foo\"; {\"foo\" : 5}[key]", 5L),
                new HashIndexTestCase("{}[\"foo\"]", null),
                new HashIndexTestCase("{5 : 5}[5]", 5L),
                new HashIndexTestCase("{true : 5}[true]", 5L),
                new HashIndexTestCase("{false : 5}[false]", 5L),
            };

            foreach (var tt in tests)
            {
                var evaluted = TestEval(tt.Input);
                if(tt.Expected!=null)
                {
                    TestIntegerObject(evaluted, (long)tt.Expected);
                }
                else
                {
                    TestNullObject(evaluted);
                }
            }

        }
        #endregion
    }
}
