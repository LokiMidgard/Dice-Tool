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


        internal IExecutor<T, int> Compile<T>(ProgramSyntax p)
        {
            var returnType = Validator.GetType(p.Return, this.variables);
            if (returnType == typeof(T))
                return this.Configure<T>(p);
            throw new NotSupportedException();
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

                    Action @else = null;
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
                default:
                    throw new NotSupportedException();
            }

        }

        private P<T> GenerateExpression<T>(ExpresionSyntax expresion, Composer<int> x)
        {

            switch (expresion)
            {
                case ConstSyntax<T> constSyntax:
                    return x.Const(constSyntax.Value);

                case IdentifierSyntax identifierSyntax:
                    return x.GetNamed<T>(identifierSyntax.literal);
                case DiceSyntax diceSyntax when typeof(T) == typeof(int):
                    return (P<T>)(object)Enumerable.Range(1, diceSyntax.Count).Select(i => x.Dice(diceSyntax.Faces)).Aggregate((d1, d2) => d1.Add(d2));
                case BinaryOpereratorSyntax binaryOpereratorSyntax:

                    var type = Validator.GetType(binaryOpereratorSyntax.Argument1, this.variables);
                    if (type == typeof(int))
                        return this.GenerateBinarayExpresion<int, T>(binaryOpereratorSyntax.Operator, binaryOpereratorSyntax.Argument1, binaryOpereratorSyntax.Argument2, x);
                    throw new NotSupportedException();


                default:
                    throw new NotSupportedException();
            }

        }

        private P<TOut> GenerateBinarayExpresion<TIn, TOut>(BinaryOperator @operator, ExpresionSyntax argument1, ExpresionSyntax argument2, Composer<int> x)
        {
            switch (@operator)
            {
                case BinaryOperator.Addition:
                    if (typeof(TIn) == typeof(string))
                        return (P<TOut>)(object)(this.GenerateExpression<string>(argument1, x).Add(this.GenerateExpression<string>(argument2, x)));
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Add(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Substraction:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Substract(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Multiplication:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Multiply(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Division:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Divide(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Modulo:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).Modulo(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                //case BinaryOperator.LogicAnd:
                case BinaryOperator.BitAnd:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).BitAnd(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn) == typeof(bool))
                        return (P<TOut>)(object)(this.GenerateExpression<bool>(argument1, x).And(this.GenerateExpression<bool>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.BitOr:
                //case BinaryOperator.LogicOr:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).BitOr(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn) == typeof(bool))
                        return (P<TOut>)(object)(this.GenerateExpression<bool>(argument1, x).Or(this.GenerateExpression<bool>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.BitXor:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).BitXOr(this.GenerateExpression<int>(argument2, x)));
                    if (typeof(TIn) == typeof(bool))
                        return (P<TOut>)(object)((this.GenerateExpression<bool>(argument1, x).Not().And(this.GenerateExpression<bool>(argument2, x))).Or(this.GenerateExpression<bool>(argument1, x).And(this.GenerateExpression<bool>(argument2, x).Not())));
                    throw new NotSupportedException();

                case BinaryOperator.ShiftLeft:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).LeftShift(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();
                case BinaryOperator.ShiftRight:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).RightShift(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();


                case BinaryOperator.LessThen:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).LessThen(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.LessOrEquals:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).LessOrEqual(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();
                case BinaryOperator.GreaterThen:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).GreaterThen(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.GreaterOrEquals:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).GreaterOrEqual(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.Equals:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).AreEqual(this.GenerateExpression<int>(argument2, x)));
                    throw new NotSupportedException();

                case BinaryOperator.NotEquals:
                    if (typeof(TIn) == typeof(int))
                        return (P<TOut>)(object)(this.GenerateExpression<int>(argument1, x).AreEqual(this.GenerateExpression<int>(argument2, x)).Not());
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}