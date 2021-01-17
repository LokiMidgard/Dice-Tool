using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Dice.Parser.Syntax
{
    internal abstract class Syntax
    {
    }

    internal abstract class StatementSyntax : Syntax
    {
        internal abstract void ToString(IndentionWriter writer);

    }

    internal class CommentSyntax : StatementSyntax
    {
        public CommentSyntax(IEnumerable<string> comment)
        {
            this.Comment = comment.Select(x => x.Trim()).ToArray();
        }

        public string[] Comment { get; }


        internal override void ToString(IndentionWriter writer)
        {
            writer.WriteLine();

            foreach (var line in this.Comment)
            {
                writer.WriteLine($"# {line}");
            }
        }

    }

    internal abstract class ExpresionSyntax : Syntax
    {
        internal abstract string DisplayString();
        public override string ToString()
        {
            return this.DisplayString();
        }
    }

    internal class ParentisedSyntax : ExpresionSyntax
    {
        public ParentisedSyntax(ExpresionSyntax expression)
        {
            this.Expression = expression;
        }

        public ExpresionSyntax Expression { get; }

        internal override string DisplayString()
        {
            return $"({this.Expression})";
        }
    }

    internal class IdentifierSyntax : ExpresionSyntax
    {
        public IdentifierSyntax(string literal)
        {
            this.literal = literal ?? throw new ArgumentNullException(nameof(literal));
        }

        public string literal { get; }

        internal override string DisplayString() => this.literal;
    }

    internal class SwitchSyntax : StatementSyntax
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

        internal override void ToString(IndentionWriter writer)
        {
            writer.WriteLine($"{this.Target.literal} switch {this.Input}:");
            using (writer.Indent)
            {
                foreach (var @case in this.Cases)
                    writer.WriteLine(@case.ToString());
                writer.WriteLine($"default: {this.DefaultResult};");
            }
        }
    }

    internal class CaseSyntax
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
        public override string ToString()
        {
            return $"{this.Op.GetToken()} {this.Input}: {this.Result};";

        }
    }

    internal abstract class ConstSyntax : ExpresionSyntax
    {
        protected ConstSyntax(object value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public abstract Type Type { get; }
        public object Value { get; }

    }

    internal class ConstSyntax<T> : ConstSyntax
    {
        public ConstSyntax([DisallowNull] T value) : base(value)
        {
            this.Value = value;
        }

        [NotNull]
        public new T Value { get; }

        public override Type Type => typeof(T);

        internal override string DisplayString()
        {
            if (typeof(T) == typeof(string))
                return $"\"{this.Value}\"";
            if (typeof(T) == typeof(bool) && this.Value is not null)
                return ((bool)base.Value) ? "true" : "false";
            return this.Value.ToString()!;
        }
    }

    internal class DiceSyntax : ExpresionSyntax
    {
        public DiceSyntax(int faces, int count)
        {
            this.Faces = faces;
            this.Count = count;
        }

        public int Faces { get; }
        public int Count { get; }

        internal override string DisplayString()
        {
            if (this.Count > 1)
                return $"{this.Count}W{this.Faces}";
            return $"W{this.Faces}";
        }
    }

    internal class BinaryOpereratorSyntax : ExpresionSyntax
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

        internal override string DisplayString()
        {

            return $"{this.Argument1} {this.Operator.GetToken()} {this.Argument2}";
        }
    }

    internal class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(Type type, IdentifierSyntax identifier)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        public Type Type { get; }
        public IdentifierSyntax Identifier { get; }

        internal override void ToString(IndentionWriter writer)
        {
            string type;

            if (this.Type == typeof(int))
                type = "int";
            else if (this.Type == typeof(bool))
                type = "bool";
            else if (this.Type == typeof(string))
                type = "string";
            else
                throw new NotSupportedException();

            writer.WriteLine($"var {this.Identifier}: {type}");
        }
    }

    internal class VariableAssignmentSyntax : StatementSyntax
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
        public VariableDeclarationSyntax? VariableDeclarationSyntax { get; }

        internal override void ToString(IndentionWriter writer)
        {
            if (this.VariableDeclarationSyntax != null)
            {
                string type;

                if (this.VariableDeclarationSyntax.Type == typeof(int))
                    type = "int";
                else if (this.VariableDeclarationSyntax.Type == typeof(bool))
                    type = "bool";
                else if (this.VariableDeclarationSyntax.Type == typeof(string))
                    type = "string";
                else
                    throw new NotSupportedException();

                writer.WriteLine($"var {this.Identifier}: {type} = {this.Expresion}");

            }
            else
            {
                writer.WriteLine($"{this.Identifier} = {this.Expresion}");
            }
        }
    }

    internal class BlockSyntax : StatementSyntax
    {
        public BlockSyntax(StatementSyntax[] statements)
        {
            this.Statements = statements ?? throw new ArgumentNullException(nameof(statements));
        }

        public StatementSyntax[] Statements { get; }


        internal override void ToString(IndentionWriter writer)
        {
            writer.WriteLine("{");
            using (writer.Indent)
                foreach (var statement in this.Statements)
                    statement.ToString(writer);
            writer.WriteLine("}");
        }
    }

    internal class DoWhileSyntax : StatementSyntax
    {
        public DoWhileSyntax(StatementSyntax statement, ExpresionSyntax condition)
        {
            this.Statement = statement ?? throw new ArgumentNullException(nameof(statement));
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public StatementSyntax Statement { get; }
        public ExpresionSyntax Condition { get; }

        internal override void ToString(IndentionWriter writer)
        {
            writer.WriteLine("do");
            if (this.Statement is not BlockSyntax)
            {
                using (writer.Indent)
                    this.Statement.ToString(writer);
            }
            else
            {
                this.Statement.ToString(writer);
            }
            writer.WriteLine($"while {this.Condition}");
        }

    }

    internal class IfSyntax : StatementSyntax
    {
        public IfSyntax(ExpresionSyntax condition, StatementSyntax then, ElseSyntax? @else)
        {
            this.Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.Then = then ?? throw new ArgumentNullException(nameof(then));
            this.Else = @else;
        }

        public ExpresionSyntax Condition { get; }
        public StatementSyntax Then { get; }
        public ElseSyntax? Else { get; }

        internal override void ToString(IndentionWriter writer)
        {
            writer.WriteLine($"if {this.Condition}");
            if (this.Then is not BlockSyntax)
            {
                using (writer.Indent)
                    this.Then.ToString(writer);
            }
            else
            {
                this.Then.ToString(writer);
            }
            this.Else?.ToString(writer);
        }

    }

    internal class ElseSyntax
    {
        public ElseSyntax(StatementSyntax then)
        {
            this.Then = then ?? throw new ArgumentNullException(nameof(then));
        }

        public StatementSyntax Then { get; }

        internal void ToString(IndentionWriter writer)
        {
            writer.WriteLine($"else");
            if (this.Then is not BlockSyntax)
            {
                using (writer.Indent)
                    this.Then.ToString(writer);
            }
            else
            {
                this.Then.ToString(writer);
            }
        }
    }

    internal class ProgramSyntax
    {
        public ProgramSyntax(StatementSyntax[] statements, ExpresionSyntax @return)
        {
            this.Statements = statements ?? throw new ArgumentNullException(nameof(statements));
            this.Return = @return ?? throw new ArgumentNullException(nameof(@return));
        }

        public StatementSyntax[] Statements { get; }
        public ExpresionSyntax Return { get; }


        public override string ToString()
        {
            var writer = new IndentionWriter() { IndentionDepth = 2 };
            foreach (var statement in this.Statements)
            {
                statement.ToString(writer);
            }
            writer.WriteLine();
            writer.WriteLine($"return {this.Return}");

            return writer.ToString();
        }
    }

    internal static class BinaryOperatorExtension
    {
        public static string GetToken(this BinaryOperator @operator)
        {
            return @operator switch
            {
                BinaryOperator.Addition => "+",
                BinaryOperator.BitAnd => "&",
                BinaryOperator.BitOr => "|",
                BinaryOperator.BitXor => "^",
                BinaryOperator.Division => "/",
                BinaryOperator.Equals => "==",
                BinaryOperator.GreaterOrEquals => ">=",
                BinaryOperator.GreaterThen => ">",
                BinaryOperator.LessThen => "<",
                BinaryOperator.LessOrEquals => "<=",
                BinaryOperator.Modulo => "%",
                BinaryOperator.Multiplication => "*",
                BinaryOperator.NotEquals => "!=",
                BinaryOperator.ShiftLeft => "<<",
                BinaryOperator.ShiftRight => ">>",
                BinaryOperator.Substraction => "-",
                _ => throw new NotSupportedException()
            };
        }
    }

    internal enum BinaryOperator
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

