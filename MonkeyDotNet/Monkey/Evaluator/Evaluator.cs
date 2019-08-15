using Monkey.Ast;
using Monkey.Object;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Monkey.Evaluator
{
    public class Evaluator
    {
        public readonly static Object.Boolean TRUE = new Object.Boolean { Value = true };

        public readonly static Object.Boolean FALSE = new Object.Boolean { Value = false };

        public readonly static Object.Null NULL = new Object.Null();


        public static Object.Object Eval(Ast.Node node, Object.Environment env)
        {
            if(node is Ast.Program)
            {
                return EvalProgram((Ast.Program)node, env);
            }
            if(node is Ast.ExpressionStatement)
            {
                return Eval(((Ast.ExpressionStatement)node).Expression, env);
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
                Object.Object right = Eval(((Ast.PrefixExpression)node).Right, env);
                if(IsError(right))
                {
                    return right;
                }
                return EvalPrefixExpression(((Ast.PrefixExpression)node).Operator, right);
            }
            if(node is Ast.InfixExpression)
            {
                Object.Object left = Eval(((Ast.InfixExpression)node).Left, env);
                if(IsError(left))
                {
                    return left;
                }
                Object.Object right = Eval(((Ast.InfixExpression)node).Right, env);
                if(IsError(right))
                {
                    return right;
                }
                return EvalInfixExpression(((Ast.InfixExpression)node).Operator, left,right);
            }
            if(node is Ast.BlockStatement)
            {
                return EvalBlockStatement((Ast.BlockStatement)node, env);
            }
            if(node is Ast.IfExpression)
            {
                return EvalIfExpression((Ast.IfExpression)node, env);
            }
            if(node is Ast.ReturnStatement)
            {
                Object.Object val = Eval(((Ast.ReturnStatement)node).ReturnValue, env);
                if(IsError(val)) 
                {
                    return val;
                }
                return new Object.ReturnValue{Value=val};
            }
            if(node is Ast.LetStatement)
            {
                Object.Object val = Eval(((Ast.LetStatement)node).Value, env);
                if(IsError(val))
                {
                    return val;
                }
                env.Set(((Ast.LetStatement)node).Name.Value, val);
            }
            if(node is Ast.Identifier)
            {
                return EvalIdentifier((Ast.Identifier)node, env);
            }
            if(node is Ast.FunctionLiteral)
            {
                Ast.FunctionLiteral func = node as Ast.FunctionLiteral;
                return new Object.Function{Parameters=func.Parameters, Env=env, Body=func.Body};
            }
            if(node is Ast.CallExpression)
            {
                Object.Object function = Eval(((Ast.CallExpression)node).Function, env);
                if(IsError(function))
                {
                    return function;
                }
                var args = EvalExpressions(((Ast.CallExpression)node).Arguments, env);
                if(args.Count()==1 && IsError(args.First()))
                {
                    return args.First();
                }
                return ApplyFunction(function, args);
            }
            if (node is StringLiteral)
            {
                return new Object.Strings { Value = ((StringLiteral)node).Value };
            }
            return null;
        }

        private static Object.Object EvalProgram(Ast.Program program, Object.Environment env)
        {
            Object.Object result = null;
            foreach(var statement in program.Statements)
            {
                result = Eval(statement, env);
                if(result is Object.ReturnValue)
                {
                    return (result as Object.ReturnValue).Value;
                }
                if(result is Object.Error)
                {
                    return result as Object.Error;
                }
                
            }
            return result;
        }

        private static Object.Object EvalBlockStatement(Ast.BlockStatement block, Object.Environment env)
        {
            Object.Object result=null;
            foreach (var statement in block.Statements)
            {
                result = Eval(statement, env);
                if(result!=null)
                {
                    var rt = result.Type();
                    if(rt==Object.ObjectType.RETURN_VALUE_OBJ || rt==Object.ObjectType.ERROR_OBJ)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private static Object.Object EvalStatements(IEnumerable<Ast.Statement> stmts, Object.Environment env)
        {
            Object.Object result=null;
            foreach (var stmt in stmts)
            {
                result = Eval(stmt, env);
                Object.ReturnValue returnValue = result as Object.ReturnValue;
                if(returnValue != null)
                {
                    return returnValue.Value;
                }
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
                    return new Object.Error{Message=$"unknown operator: {op}{right.Type()}"};
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
                return new Object.Error{Message=$"unknown operator: -{right.Type()}"};;
            }
            var value = ((Object.Integer)right).Value;
            return new Object.Integer { Value = -value };
        }

        private static Object.Object EvalInfixExpression(string op, Object.Object left, Object.Object right)
        {
            if(left.Type() != right.Type())
            {
                return new Object.Error{Message=$"type mismatch: {left.Type()} {op} {right.Type()}"};
            }
            if(left.Type()==Object.ObjectType.INTEGER_OBJ && right.Type()==Object.ObjectType.INTEGER_OBJ)
            {
                return EvaIntegerInfixExpression(op, left, right);
            }
            if(left.Type()==Object.ObjectType.STRING_OBJ && right.Type()==Object.ObjectType.STRING_OBJ)
            {
                return EvalStringInfixExpression(op, left, right);
            }
            switch(op)
            {
                case "==":
                    return NativeBoolToBooleanObject(left==right);
                case "!=":
                    return NativeBoolToBooleanObject(left!=right);
                default:
                    return new Object.Error{Message=$"unknown operator: {left.Type()} {op} {right.Type()}"};
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
                    return new Object.Error{Message=$"unknown operator: {left.Type()} {op} {right.Type()}"};;
            }
        }


        private static Object.Object EvalStringInfixExpression(string op, Object.Object left, Object.Object right)
        {
            if(op!="+")
            {
                return new Error { Message = $"unknown operator: {left.Type()} {op} {right.Type()}" };
            }
            var leftVal = ((Object.Strings)left).Value;
            var rightVal = ((Object.Strings)right).Value;
            return new Object.Strings { Value = leftVal + rightVal };
        }

        private static Object.Object EvalIfExpression(Ast.IfExpression exp, Object.Environment env) 
        {
            Object.Object condition = Eval(exp.Condition, env);
            if(IsError(condition))
            {
                return condition;
            }
            if(IsTruthy(condition)) 
            {
                return Eval(exp.Consequence, env);
            }
            else if (exp.Alternative!=null)
            {
                return Eval(exp.Alternative, env);
            }
            else
            {
                return NULL;
            }
        }

        private static bool IsTruthy(Object.Object obj)
        {
            if(obj==NULL)
            {
                return false;
            }
            else if (obj==TRUE)
            {
                return true;
            }
            else if (obj==FALSE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsError(Object.Object obj)
        {
            return obj.Type() == Object.ObjectType.ERROR_OBJ;
        }

        private static Object.Object EvalIdentifier(Ast.Identifier node, Object.Environment env)
        {
            if(env.Exist(node.Value))
            {
                return env.Get(node.Value);
            }
            return new Object.Error{Message=$"identifier not found: {node.Value}"};
        }

        private static IEnumerable<Object.Object> EvalExpressions(IEnumerable<Ast.Expression> exps, Object.Environment env)
        {
            var result = new List<Object.Object>();
            foreach (var e in exps)
            {
                var evaluated = Eval(e, env);
                if(IsError(evaluated))
                {
                    return new List<Object.Object>(){evaluated};
                }
                result.Add(evaluated);
            }
            return result;
        }

        private static Object.Object ApplyFunction(Object.Object fn, IEnumerable<Object.Object> args) 
        {
            Object.Function function = fn as Object.Function;
            if(function==null)
            {
                return new Object.Error(){Message=$"not a funciton. {fn.Type()}"};
            }
            var extendedEnv = ExtendFuncitonEnv(function, args);
            var evaluated = Eval(function.Body, extendedEnv);
            return unwrapReturnValue(evaluated);

        }

        private static Object.Environment ExtendFuncitonEnv(Object.Function function, IEnumerable<Object.Object> args)
        {
            var env = new Object.Environment(function.Env);
            var idx = 0;
            foreach (var arg in args)
            {
                env.Set(function.Parameters[idx].Value, arg);
                idx++;
            }
            return env;
        }

        private static Object.Object unwrapReturnValue(Object.Object obj)
        {
            if(obj is Object.ReturnValue) 
            {
                return ((Object.ReturnValue)obj).Value;
            }
            return obj;
        }
    }
}
