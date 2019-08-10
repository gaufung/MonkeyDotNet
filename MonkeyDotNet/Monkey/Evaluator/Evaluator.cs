using System.Collections.Generic;
namespace Monkey.Evaluator
{
    public class Evaluator
    {
        private readonly static Object.Boolean TRUE = new Object.Boolean { Value = true };

        private readonly static Object.Boolean FALSE = new Object.Boolean { Value = false };

        private readonly static Object.Null NULL = new Object.Null();


        public static Object.Object Eval(Ast.Node node)
        {
            if(node is Ast.Program)
            {
                return EvalStatements(((Ast.Program)node).Statements);
            }
            if(node is Ast.ExpressionStatement)
            {
                return Eval(((Ast.ExpressionStatement)node).Expression);
            }
            if(node is Ast.IntegerLiteral)
            {
                return new Object.Integer { Value = ((Ast.IntegerLiteral)node).Value };
            }
            if (node is Ast.Boolean)
            {
                return NativeBoolToBooleanObject(((Ast.Boolean)node).Value);
            }
            if(node is Ast.PrefixExpression)
            {
                Object.Object right = Eval(((Ast.PrefixExpression)node).Right);
                return EvalPrefixExpression(((Ast.PrefixExpression)node).Operator, right);
            }
            if(node is Ast.InfixExpression)
            {
                Object.Object left = Eval(((Ast.InfixExpression)node).Left);
                Object.Object right = Eval(((Ast.InfixExpression)node).Right);
                return EvalInfixExpression(((Ast.InfixExpression)node).Operator, left,right);
            }
            return null;
        }

        private static Object.Object EvalStatements(IEnumerable<Ast.Statement> stmts)
        {
            Object.Object result=null;
            foreach (var stmt in stmts)
            {
                result = Eval(stmt);
            }
            return result;
        }

        private static Object.Boolean NativeBoolToBooleanObject(bool input)
        {
            return input ? TRUE : FALSE;
        }

        private static Object.Object EvalPrefixExpression(string op, Object.Object right)
        {
            switch (op)
            {
                case "!":
                    return EvalBangOperatorExpression(right);
                case "-":
                    return EvalMinusPrefixOpeatorExpression(right);
                default:
                    return NULL;
            }
        }
        private static Object.Object EvalBangOperatorExpression(Object.Object right)
        {
            if (right == TRUE)
            {
                return FALSE;
            }else if (right == FALSE)
            {
                return TRUE;
            }else if (right == NULL)
            {
                return TRUE;
            }
            return FALSE;
        }

        private static Object.Object EvalMinusPrefixOpeatorExpression(Object.Object right)
        {
            if(right.Type() != Object.ObjectType.INTEGER_OBJ)
            {
                return NULL;
            }
            var value = ((Object.Integer)right).Value;
            return new Object.Integer { Value = -value };
        }

        private static Object.Object EvalInfixExpression(string op, Object.Object left, Object.Object right)
        {
            if(left.Type()==Object.ObjectType.INTEGER_OBJ && right.Type()==Object.ObjectType.INTEGER_OBJ)
            {
                return EvaIntegerInfixExpression(op, left, right);
            }
            switch(op)
            {
                case "==":
                    return NativeBoolToBooleanObject(left==right);
                case "!=":
                    return NativeBoolToBooleanObject(left!=right);
                default:
                    return NULL;
            }
        }

        private static Object.Object EvaIntegerInfixExpression(string op, Object.Object left, Object.Object right)
        {
            long leftValue = ((Object.Integer)left).Value;
            long rightValue = ((Object.Integer)right).Value;
            switch (op)
            {
                case "+":
                    return new Object.Integer{Value=leftValue+rightValue};
                case "-":
                    return new Object.Integer{Value=leftValue - rightValue};
                case "*":
                    return new Object.Integer{Value=leftValue * rightValue};
                case "/":
                    return new Object.Integer{Value=leftValue / rightValue};
                case "<":
                    return NativeBoolToBooleanObject(leftValue < rightValue);
                case ">":
                    return NativeBoolToBooleanObject(leftValue > rightValue);
                case "==":
                    return NativeBoolToBooleanObject(leftValue == rightValue);
                case "!=":
                    return NativeBoolToBooleanObject(leftValue != rightValue);
                default:
                    return NULL;
            }
        }
    }
}
