using System;
using System.Collections.Generic;
using System.Linq;
using Dice.Parser.Syntax;

namespace Dice.Parser
{
    internal class Compiler
    {
        private Dictionary<string, Type> variables;

        public Compiler(Dictionary<string, Type> variables)
        {
            this.variables = variables;
        }

        internal Type? GetReturnType(ProgramSyntax p)
        {
            var returnType = Validator.GetType(p.Return, this.variables);
            return returnType;
        }


        internal IExecutor<T, int> Compile<T>(ProgramSyntax p)
        {
            var returnType = Validator.GetType(p.Return, this.variables);
            if (returnType == typeof(T))
                return this.Configure<T>(p);
            throw new NotSupportedException($"Returntype must be {typeof(T)} but was {returnType}.");
        }

        private IExecutor<T, int> Configure<T>(ProgramSyntax p)
        {
            return Dice.Calculator<int>.Configure(x =>
            {
                foreach (var statement in p.Statements)
                    this.GenerateStatement(statement, x);

                return this.GenerateExpression<T>(p.Return, x);
            });
        }


        private void GenerateStatement(StatementSyntax statement, Composer<int> x)
        {

            switch (statement)
            {
                case VariableAssignmentSyntax assignmentSyntax:
                    var name = assignmentSyntax.Identifier.literal;
                    if (this.variables[name] == typeof(string))
                        x.AssignName(name, this.GenerateExpression<string>(assignmentSyntax.Expresion, x));
                    if (this.variables[name] == typeof(bool))
                        x.AssignName(name, this.GenerateExpression<bool>(assignmentSyntax.Expresion, x));
                    if (this.variables[name] == typeof(int))
                        x.AssignName(name, this.GenerateExpression<int>(assignmentSyntax.Expresion, x));
                    break;
                case VariableDeclarationSyntax declarationSyntax: // Nothing to do
                    break;
                case DoWhileSyntax doWhileSyntax:

                    x.DoWhile(() =>
                    {
                        this.GenerateStatement(doWhileSyntax.Statement, x);
                        return this.GenerateExpression<bool>(doWhileSyntax.Condition, x);
                    });


                    break;
                case IfSyntax ifSyntax:

                    Action? @else = null;
                    if (ifSyntax.Else != null)
                        @else = () => this.GenerateStatement(ifSyntax.Else.Then, x);

                    x.If(this.GenerateExpression<bool>(ifSyntax.Condition, x),
                        then: () => this.GenerateStatement(ifSyntax.Then, x),
                        @else: @else);
                    break;
                case BlockSyntax blockSyntax:
                    foreach (var item in blockSyntax.Statements)
                        this.GenerateStatement(item, x);
                    break;

                case SwitchSyntax switchSyntax:

                    var resultType = Validator.GetType(switchSyntax.Target, this.variables);
                    var targetType = Validator.GetType(switchSyntax.Input, this.variables);


                    if (targetType == typeof(int) && resultType == typeof(int))
                        GenerateSwitch<int, int>();
                    if (targetType == typeof(bool) && resultType == typeof(int))
                        GenerateSwitch<bool, int>();
                    if (targetType == typeof(string) && resultType == typeof(int))
                        GenerateSwitch<string, int>();

                    if (targetType == typeof(int) && resultType == typeof(bool))
                        GenerateSwitch<int, bool>();
                    if (targetType == typeof(bool) && resultType == typeof(bool))
                        GenerateSwitch<bool, bool>();
                    if (targetType == typeof(string) && resultType == typeof(bool))
                        GenerateSwitch<string, bool>();

                    if (targetType == typeof(int) && resultType == typeof(string))
                        GenerateSwitch<int, string>();
                    if (targetType == typeof(bool) && resultType == typeof(string))
                        GenerateSwitch<bool, string>();
                    if (targetType == typeof(string) && resultType == typeof(string))
                        GenerateSwitch<string, string>();


                    void GenerateSwitch<TIn, TOut>()
                    {
                        var conditions = switchSyntax.Cases.Select(c => this.GenerateBinarayExpresion<TIn, TIn, bool>(c.Op, switchSyntax.Input, c.Input, x)).ToArray();
                        var combinedConditions = x.Combine(conditions);

                        var index = combinedConditions.Select(array =>
                        {
                            for (int i = 0; i < array.Length; i++)
                                if (array[i])
                                    return i;
                            return -1;
                        });

                        var variables = new[] { this.GenerateExpression<TOut>(switchSyntax.DefaultResult, x) }.Concat(switchSyntax.Cases.Select(c => this.GenerateExpression<TOut>(c.Result, x))).ToArray();
                        var combinedVariables = x.Combine(variables);

                        var result = x.Combine(index, combinedVariables, (i, v) => v[i + 1 /*Added default expression as first result in array*/], Tables.OptimisationStrategy.Optimize);
                        x.AssignName(switchSyntax.Target.literal, result);
                    }


                    break;
                case CommentSyntax commentSyntax:
                    break;

                default:
                    throw new NotSupportedException();
            }

        }


        private P<T> GenerateExpression<T>(ExpresionSyntax expresion, Composer<int> x)
        {

            switch (expresion)
            {
                case ParentisedSyntax parentised:
                    return GenerateExpression<T>(parentised.Expression, x);
                case ConstSyntax<T> constSyntax:
                    return x.Const(constSyntax.Value);

                case IdentifierSyntax identifierSyntax:
                    return x.GetNamed<T>(identifierSyntax.literal);
                case DiceSyntax diceSyntax when typeof(T) == typeof(int):
                    return (P<T>)(object)Enumerable.Range(1, diceSyntax.Count).Select(i => x.Dice(diceSyntax.Faces)).Aggregate((d1, d2) => d1.Add(d2));
                case BinaryOpereratorSyntax binaryOpereratorSyntax:

                    var type1 = Validator.GetType(binaryOpereratorSyntax.Argument1, this.variables);
                    var type2 = Validator.GetType(binaryOpereratorSyntax.Argument2, this.variables);

                    if (type1 == typeof(int) && type2 == typeof(int))
                        return this.GenerateBinarayExpresion<int, int, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    if (type1 == typeof(bool) && type2 == typeof(int))
                        return this.GenerateBinarayExpresion<bool, int, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    if (type1 == typeof(string) && type2 == typeof(int))
                        return this.GenerateBinarayExpresion<string, int, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);

                    if (type1 == typeof(int) && type2 == typeof(bool))
                        return this.GenerateBinarayExpresion<int, bool, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    if (type1 == typeof(bool) && type2 == typeof(bool))
                        return this.GenerateBinarayExpresion<bool, bool, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    if (type1 == typeof(string) && type2 == typeof(bool))
                        return this.GenerateBinarayExpresion<string, bool, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);

                    if (type1 == typeof(int) && type2 == typeof(string))
                        return this.GenerateBinarayExpresion<int, string, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    if (type1 == typeof(bool) && type2 == typeof(string))
                        return this.GenerateBinarayExpresion<bool, string, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    if (type1 == typeof(string) && type2 == typeof(string))
                        return this.GenerateBinarayExpresion<string, string, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);


                    throw new NotSupportedException($"type {type1} is not supported for Binary Syntax.");


                default:
                    throw new NotSupportedException($"{expresion}");
            }

        }

        private P<TOut> GenerateBinarayExpresion<TIn1, TIn2, TOut>(BinaryOperator @operator, ExpresionSyntax argument1, ExpresionSyntax argument2, Composer<int> x)
        {
            switch (@operator)
            {
                case BinaryOperator.Addition:
                    if (typeof(TIn1) == typeof(string) && typeof(TIn2) == typeof(string))
                        return (P<TOut>)(object)(this.GenerateExpression<string>(argument1, x).Add(this.GenerateExpression<string>(argument2, x)));
                    if (typeof(TIn1) == typeof(int) && typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Add(this.GenerateExpression<int>(argument2, x)));

                    if (typeof(TIn1) == typeof(int) && typeof(TIn2) == typeof(string))
                    {
                        var p1 = this.GenerateExpression<int>(argument1, x);
                        var p2 = this.GenerateExpression<string>(argument2, x);
                        var result = x.Combine(p1, p2, (v1, v2) => v1 + v2, Tables.OptimisationStrategy.NoOptimisation);
                        return (P<TOut>)(object)result;
                    }
                    if (typeof(TIn1) == typeof(string) && typeof(TIn2) == typeof(int))
                    {
                        var p1 = this.GenerateExpression<string>(argument1, x);
                        var p2 = this.GenerateExpression<int>(argument2, x);
                        var result = x.Combine(p1, p2, (v1, v2) => v1 + v2, Tables.OptimisationStrategy.NoOptimisation);
                        return (P<TOut>)(object)result;
                    }


                    throw new NotSupportedException();

                case BinaryOperator.Substraction:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Substract(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Multiplication:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Multiply(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Division:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Divide(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Modulo:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Modulo(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                //case BinaryOperator.LogicAnd:
                case BinaryOperator.BitAnd:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).BitAnd(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn1) == typeof(bool))
                        return (P<TOut>)(object)(this.GenerateExpression<bool>(argument1, x).And(this.GenerateExpression<bool>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.BitOr:
                    //case BinaryOperator.LogicOr:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).BitOr(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn1) == typeof(bool))
                        return (P<TOut>)(object)(this.GenerateExpression<bool>(argument1, x).Or(this.GenerateExpression<bool>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.BitXor:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).BitXOr(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn1) == typeof(bool))
                        return (P<TOut>)(object)((this.GenerateExpression<bool>(argument1, x).Not().And(this.GenerateExpression<bool>(argument2, x))).Or(this.GenerateExpression<bool>(argument1, x).And(this.GenerateExpression<bool>(argument2, x).Not())));
                    throw new NotSupportedException();

                case BinaryOperator.ShiftLeft:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).LeftShift(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();
                case BinaryOperator.ShiftRight:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).RightShift(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();


                case BinaryOperator.LessThen:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).LessThen(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.LessOrEquals:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).LessOrEqual(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();
                case BinaryOperator.GreaterThen:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).GreaterThen(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.GreaterOrEquals:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).GreaterOrEqual(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Equals:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).AreEqual(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn1) == typeof(bool))
                        return (P<TOut>)(object)(this.GenerateExpression<bool>(argument1, x).AreEqual(this.GenerateExpression<bool>(argument2, x)));
                    if (typeof(TIn1) == typeof(string))
                        return (P<TOut>)(object)(this.GenerateExpression<string>(argument1, x).AreEqual(this.GenerateExpression<string>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.NotEquals:
                    if (typeof(TIn1) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).AreEqual(this.GenerateExpression<int>(argument2, x))).Not();
                    if (typeof(TIn1) == typeof(bool))
                        return (P<TOut>)(object)(this.GenerateExpression<bool>(argument1, x).AreEqual(this.GenerateExpression<bool>(argument2, x))).Not();
                    if (typeof(TIn1) == typeof(string))
                        return (P<TOut>)(object)(this.GenerateExpression<string>(argument1, x).AreEqual(this.GenerateExpression<string>(argument2, x))).Not();
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}