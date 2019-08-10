using System;
namespace MonkeyTest.Evaluator
{
    using NUnit.Framework;
    using Monkey.Evaluator;
    using Monkey.Lexer;
    using Monkey.Object;
    using Monkey.Parser;

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
            return Evaluator.Eval(program);
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
    }
}
