using System;
using System.Collections.Generic;
using System.Linq;
using Dice.Parser.Syntax;

namespace Dice.Parser
{
    internal class Validator
    {
        internal static Dictionary<string, Type> Validate(ProgramSyntax p)
        {
            var flatedStatements = p.Statements.SelectMany(FlatenStatements);

            var variableAssignments = flatedStatements.Select(GetVariableAssignment).Where(x => x != default).ToDictionary(x => x.name, x => x.type);

            var expresions = flatedStatements.SelectMany(GetExpresions).Concat(Enumerable.Repeat(p.Return, 1));

            var missingVariables = expresions.SelectMany(FlatenExpression).OfType<IdentifierSyntax>().Where(x => !variableAssignments.ContainsKey(x.literal)).Select(x => $"Variable {x.literal} is not declared");

            var invalidCast = flatedStatements.SelectMany(x => CheckTypes(x, variableAssignments));


            var errors = missingVariables.Concat(invalidCast);

            var errorText = String.Join("\n", errors);

            if (!string.IsNullOrWhiteSpace(errorText))
                throw new Exception(errorText);

            return variableAssignments;
        }

        private static IEnumerable<string> CheckTypes(StatementSyntax input, Dictionary<string, Type> typeLookup)
        {
            switch (input)
            {
                case VariableAssignmentSyntax assignmentSyntax:
                    var targetType = typeLookup[assignmentSyntax.Identifier.literal];
                    var (actualType, error) = GetTypeOrError(assignmentSyntax.Expresion, typeLookup);
                    if (error != null)
                        yield return error;
                    else if (targetType != actualType)
                        yield return $"Value {actualType} cannot be assigned to {targetType}";

                    break;
                case DoWhileSyntax doWhileSyntax:
                    (actualType, error) = GetTypeOrError(doWhileSyntax.Condition, typeLookup);
                    if (error != null)
                        yield return error;
                    else if (actualType != typeof(bool))
                        yield return $"Condition must be bool ({actualType})";
                    break;
                case IfSyntax ifSyntax:
                    (actualType, error) = GetTypeOrError(ifSyntax.Condition, typeLookup);
                    if (error != null)
                        yield return error;
                    else if (actualType != typeof(bool))
                        yield return $"Condition must be bool ({actualType})";
                    break;
                case SwitchSyntax switchSyntax:
                    targetType = typeLookup[switchSyntax.Target.literal];

                    foreach (var c in switchSyntax.Cases)
                    {
                        (actualType, error) = GetTypeOrError(c.Result, typeLookup);
                        if (error != null)
                            yield return error;
                        else if (targetType != actualType)
                            yield return $"Value {actualType} cannot be assigned to {targetType}";
                    }

                    break;
                default:
                    yield break;
            }

        }

        public static Type GetType(ExpresionSyntax expresion, Dictionary<string, Type> typeLookup) => GetTypeOrError(expresion, typeLookup).t;

        private static (Type t, String error) GetTypeOrError(ExpresionSyntax expresion, Dictionary<string, Type> typeLookup)
        {
            switch (expresion)
            {
                case ConstSyntax constSyntax:
                    return (constSyntax.Type, default);
                case IdentifierSyntax identifierSyntax:
                    return (typeLookup[identifierSyntax.literal], default);
                case DiceSyntax diceSyntax:
                    return (typeof(int), default);
                case BinaryOpereratorSyntax binaryOpereratorSyntax:
                    var (t1, error1) = GetTypeOrError(binaryOpereratorSyntax.Argument1, typeLookup);
                    var (t2, error2) = GetTypeOrError(binaryOpereratorSyntax.Argument2, typeLookup);

                    if (error1 != null)
                        return (default, error1);
                    if (error2 != null)
                        return (default, error2);


                    switch (binaryOpereratorSyntax.Operator)
                    {
                        case BinaryOperator.Addition:
                            if (t1 != typeof(string) && t1 != typeof(int)
                                || t2 != typeof(string) && t2 != typeof(int))
                                return (default, $"Operator {binaryOpereratorSyntax.Operator} does not support {t1} and {t2}");
                            if (t1 == typeof(string) || t2 == typeof(string))
                                return (typeof(string), default);
                            return (t1, default);
                        case BinaryOperator.Substraction:
                        case BinaryOperator.Multiplication:
                        case BinaryOperator.Division:
                        case BinaryOperator.Modulo:
                            if (t1 != t2 || t1 != typeof(int))
                                return (default, $"Operator {binaryOpereratorSyntax.Operator} does not support {t1} and {t2}");
                            return (t1, default);

                        case BinaryOperator.BitAnd:
                        case BinaryOperator.BitOr:
                        case BinaryOperator.BitXor:
                            if (t1 != t2 || (t1 != typeof(bool) && t1 != typeof(int)))
                                return (default, $"Operator {binaryOpereratorSyntax.Operator} does not support {t1} and {t2}");
                            return (t1, default);

                        case BinaryOperator.ShiftLeft:
                        case BinaryOperator.ShiftRight:
                            if (t1 != t2 || t1 != typeof(int))
                                return (default, $"Operator {binaryOpereratorSyntax.Operator} does not support {t1} and {t2}");
                            return (t1, default);

                        //case BinaryOperator.LogicAnd:
                        //case BinaryOperator.LogicOr:
                        //    if (t1 != typeof(bool))
                        //        throw new Exception();
                        //    return (typeof(bool), default);

                        case BinaryOperator.LessThen:
                        case BinaryOperator.LessOrEquals:
                        case BinaryOperator.GreaterThen:
                        case BinaryOperator.GreaterOrEquals:
                            if (t1 != t2 || t1 != typeof(int))
                                return (default, $"Operator {binaryOpereratorSyntax.Operator} does not support {t1} and {t2}");
                            return (typeof(bool), default);

                        case BinaryOperator.Equals:
                        case BinaryOperator.NotEquals:
                            return (typeof(bool), default);

                        default:
                            throw new NotSupportedException();
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private static IEnumerable<ExpresionSyntax> FlatenExpression(ExpresionSyntax input)
        {
            switch (input)
            {
                case BinaryOpereratorSyntax binaryOpereratorSyntax:
                    yield return binaryOpereratorSyntax.Argument1;
                    yield return binaryOpereratorSyntax.Argument2;
                    break;
                default:
                    break;
            }
            yield return input;
        }

        private static IEnumerable<ExpresionSyntax> GetExpresions(StatementSyntax input)
        {
            switch (input)
            {
                case VariableAssignmentSyntax assignmentSyntax:
                    yield return assignmentSyntax.Expresion;
                    break;
                case DoWhileSyntax doWhileSyntax:
                    yield return doWhileSyntax.Condition;
                    break;
                case IfSyntax ifSyntax:
                    yield return ifSyntax.Condition;
                    break;
                default:
                    yield break;
            }

        }

        private static IEnumerable<StatementSyntax> FlatenStatements(StatementSyntax input)
        {
            switch (input)
            {
                case BlockSyntax blockSyntax:
                    foreach (var item in blockSyntax.Statements.SelectMany(FlatenStatements))
                        yield return item;
                    break;
                case DoWhileSyntax doWhileSyntax:
                    foreach (var item in FlatenStatements(doWhileSyntax.Statement))
                        yield return item;
                    break;
                case IfSyntax ifSyntax:
                    foreach (var item in FlatenStatements(ifSyntax.Then))
                        yield return item;
                    if (ifSyntax.Else != null)
                        foreach (var item in FlatenStatements(ifSyntax.Else.Then))
                            yield return item;
                    break;

                default:
                    break;
            }
            // also return self.
            yield return input;
        }

        private static (string name, Type type) GetVariableAssignment(StatementSyntax input)
        {
            switch (input)
            {
                case VariableAssignmentSyntax assignmentSyntax when assignmentSyntax.VariableDeclarationSyntax != null:
                    return (assignmentSyntax.Identifier.literal, assignmentSyntax.VariableDeclarationSyntax.Type);
                case VariableDeclarationSyntax declarationSyntax:
                    return (declarationSyntax.Identifier.literal, declarationSyntax.Type);
                case DoWhileSyntax doWhileSyntax:
                case IfSyntax ifSyntax:
                case BlockSyntax blockSyntax:
                default:
                    return default;
            }
        }
    }
}