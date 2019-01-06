using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System.Collections.ObjectModel;
using System;

namespace Dice.Parser.Syntax
{
    internal abstract class Syntax
    {
    }

    class StatementSyntax : Syntax
    {

    }

    class ExpresionSyntax : Syntax
    {

    }

    class IdentifierSyntax : ExpresionSyntax
    {
        public IdentifierSyntax(string literal)
        {
            this.literal = literal ?? throw new ArgumentNullException(nameof(literal));
        }

        public string literal { get; }

    }

    class SwitchSyntax : StatementSyntax
    {

        public SwitchSyntax(IdentifierSyntax target, ExpresionSyntax input, ExpresionSyntax defaultResult, CaseSyntax[] cases)
        {
            this.Target = target;
            this.Input = input;
            this.DefaultResult = defaultResult;
            this.Cases = cases;
        }

        public IdentifierSyntax Target { get; }
        public ExpresionSyntax Input { get; }
        public ExpresionSyntax DefaultResult { get; }
        public CaseSyntax[] Cases { get; }
    }
    class CaseSyntax
    {


        public CaseSyntax(ExpresionSyntax input, BinaryOperator op, ExpresionSyntax result)
        {
            this.Input = input;
            this.Op = op;
            this.Result = result;
        }

        public ExpresionSyntax Input { get; }
        public BinaryOperator Op { get; }
        public ExpresionSyntax Result { get; }
    }

    abstract class ConstSyntax : ExpresionSyntax
    {
        protected ConstSyntax(object value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public abstract Type Type { get; }
        public object Value { get; }
    }
    class ConstSyntax<T> : ConstSyntax
    {
        public ConstSyntax(T value) : base(value)
        {
            this.Value = value;
        }

        public new T Value { get; }

        public override Type Type => typeof(T);
    }

    class DiceSyntax : ExpresionSyntax
    {
        public DiceSyntax(int faces, int count)
        {
            this.Faces = faces;
            this.Count = count;
        }

        public int Faces { get; }
        public int Count { get; }
    }

    class BinaryOpereratorSyntax : ExpresionSyntax
    {
        public BinaryOpereratorSyntax(BinaryOperator @operator, ExpresionSyntax argument1, ExpresionSyntax argument2)
        {
            this.Operator = @operator;
            this.Argument1 = argument1 ?? throw new ArgumentNullException(nameof(argument1));
            this.Argument2 = argument2 ?? throw new ArgumentNullException(nameof(argument2));
        }

        public BinaryOperator Operator { get; }
        public ExpresionSyntax Argument1 { get; }
        public ExpresionSyntax Argument2 { get; }
    }

    class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(Type type, IdentifierSyntax identifier)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        public Type Type { get; }
        public IdentifierSyntax Identifier { get; }
    }

    class VariableAssignmentSyntax : StatementSyntax
    {

        public VariableAssignmentSyntax(IdentifierSyntax identifier, ExpresionSyntax expresion)
        {
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            this.Expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        public VariableAssignmentSyntax(VariableDeclarationSyntax variableDeclarationSyntax, ExpresionSyntax expresion)
        {
            this.VariableDeclarationSyntax = variableDeclarationSyntax ?? throw new ArgumentNullException(nameof(variableDeclarationSyntax));
            this.Expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
            this.Identifier = this.VariableDeclarationSyntax.Identifier;
        }

        public IdentifierSyntax Identifier { get; }
        public ExpresionSyntax Expresion { get; }
        public VariableDeclarationSyntax VariableDeclarationSyntax { get; }
    }

    class BlockSyntax : StatementSyntax
    {
        public BlockSyntax(StatementSyntax[] statements)
        {
            this.Statements = statements ?? throw new ArgumentNullException(nameof(statements));
        }

        public StatementSyntax[] Statements { get; }
    }

    class DoWhileSyntax : StatementSyntax
    {
        public DoWhileSyntax(StatementSyntax statement, ExpresionSyntax condition)
        {
            this.Statement = statement ?? throw new ArgumentNullException(nameof(statement));
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public StatementSyntax Statement { get; }
        public ExpresionSyntax Condition { get; }
    }

    class IfSyntax : StatementSyntax
    {
        public IfSyntax(ExpresionSyntax condition, StatementSyntax then, ElseSyntax @else)
        {
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.Then = then ?? throw new ArgumentNullException(nameof(then));
            this.Else = @else;
        }

        public ExpresionSyntax Condition { get; }
        public StatementSyntax Then { get; }
        public ElseSyntax Else { get; }
    }


    class ElseSyntax
    {
        public ElseSyntax(StatementSyntax then)
        {
            this.Then = then ?? throw new ArgumentNullException(nameof(then));
        }

        public StatementSyntax Then { get; }

    }

    class ProgramSyntax
    {
        public ProgramSyntax(StatementSyntax[] statements, ExpresionSyntax @return)
        {
            this.Statements = statements ?? throw new ArgumentNullException(nameof(statements));
            this.Return = @return ?? throw new ArgumentNullException(nameof(@return));
        }

        public StatementSyntax[] Statements { get; }
        public ExpresionSyntax Return { get; }
    }

    enum BinaryOperator
    {
        Addition,
        Substraction,
        Multiplication,
        Division,
        Modulo,

        BitAnd,
        BitOr,
        BitXor,

        ShiftLeft,
        ShiftRight,

        LessThen,
        LessOrEquals,
        GreaterThen,
        GreaterOrEquals,
        Equals,
        NotEquals


    }


}
