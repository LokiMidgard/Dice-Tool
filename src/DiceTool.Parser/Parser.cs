﻿using Dice.Parser.Syntax;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dice.Parser
{
    public class SimpleParser
    {
        public static string Format(string program)
        {
            var p = Program.Parse(program);
            return p.ToString();
        }
        public static IExecutor<TReturn, int> ParseExpression<TReturn>(string program)
            where TReturn : notnull
        {
            var p = Program.Parse(program);
            var variables = Validator.Validate(p);

            var c = new Compiler(variables);
            return c.Compile<TReturn>(p);

        }

        public static Type? GetReturnType(string program)
        {
            var p = Program.Parse(program);
            var variables = Validator.Validate(p);
            var c = new Compiler(variables);
            return c.GetReturnType(p);

        }


        static Parser<BinaryOperator> Operator(string op, BinaryOperator opType) => Parse.String(op).Token().Return(opType).Named(op);

        static readonly Parser<BinaryOperator> Add = Operator("+", BinaryOperator.Addition);
        static readonly Parser<BinaryOperator> Subtract = Operator("-", BinaryOperator.Substraction);
        static readonly Parser<BinaryOperator> Multiply = Operator("*", BinaryOperator.Multiplication);
        static readonly Parser<BinaryOperator> Divide = Operator("/", BinaryOperator.Division);
        static readonly Parser<BinaryOperator> Modulo = Operator("%", BinaryOperator.Modulo);

        static readonly Parser<BinaryOperator> BitAnd = Operator("&", BinaryOperator.BitAnd);
        static readonly Parser<BinaryOperator> BitOr = Operator("|", BinaryOperator.BitOr);
        static readonly Parser<BinaryOperator> BitXor = Operator("^", BinaryOperator.BitXor);

        static readonly Parser<BinaryOperator> ShiftLeft = Operator("<<", BinaryOperator.ShiftLeft);
        static readonly Parser<BinaryOperator> ShiftRight = Operator(">>", BinaryOperator.ShiftRight);

        static readonly Parser<BinaryOperator> LessOrEqual = Operator("<=", BinaryOperator.LessOrEquals);
        static readonly Parser<BinaryOperator> GreaterOrEqual = Operator(">=", BinaryOperator.GreaterOrEquals);
        static readonly Parser<BinaryOperator> LessThen = Operator("<", BinaryOperator.LessThen);
        static readonly Parser<BinaryOperator> GreaterThen = Operator(">", BinaryOperator.GreaterThen);
        static readonly Parser<BinaryOperator> AreEqual = Operator("==", BinaryOperator.Equals);
        static readonly Parser<BinaryOperator> NotEquals = Operator("!=", BinaryOperator.NotEquals);

        static readonly Parser<BinaryOperator> CompareOperators = LessOrEqual
                                                                    .Or(GreaterOrEqual)
                                                                    .Or(LessThen)
                                                                    .Or(GreaterThen)
                                                                    .Or(AreEqual)
                                                                    .Or(NotEquals);



        //static readonly Parser<Expression> Function =
        //    from name in Parse.Letter.AtLeastOnce().Text()
        //    from lparen in Parse.Char('(')
        //    from expr in Parse.Ref(() => Expr).DelimitedBy(Parse.Char(',').Token())
        //    from rparen in Parse.Char(')')
        //    select CallFunction(name, expr.ToArray());

        //static Expression CallFunction(string name, Expression[] parameters)
        //{
        //    var methodInfo = typeof(Math).GetTypeInfo().GetMethod(name, parameters.Select(e => e.Type).ToArray());
        //    if (methodInfo == null)
        //        throw new ParseException(string.Format("Function '{0}({1})' does not exist.", name,
        //                                               string.Join(",", parameters.Select(e => e.Type.Name))));

        //    return Expression.Call(methodInfo, parameters);
        //}

        static readonly Parser<string> DoKeyword = Parse.String("do").Text().Token().Named("do");
        static readonly Parser<string> WhileKeyword = Parse.String("while").Text().Token().Named("while");
        static readonly Parser<string> VarKeyword = Parse.String("var").Text().Token().Named("var");
        static readonly Parser<string> IfKeyword = Parse.String("if").Text().Token().Named("if");
        static readonly Parser<string> ElseKeyword = Parse.String("else").Text().Token().Named("else");
        static readonly Parser<string> ReturnKeyword = Parse.String("return").Text().Token().Named("return");
        static readonly Parser<string> IntKeyword = Parse.String("int").Text().Token().Named("int");
        static readonly Parser<string> BoolKeyword = Parse.String("bool").Text().Token().Named("bool");
        static readonly Parser<string> StringKeyword = Parse.String("string").Text().Token().Named("string");
        static readonly Parser<string> TrueKeyword = Parse.String("true").Text().Token().Named("true");
        static readonly Parser<string> FalseKeyword = Parse.String("false").Text().Token().Named("false");
        static readonly Parser<string> SwitchKeyword = Parse.String("switch").Text().Token().Named("switch");
        static readonly Parser<string> DefaultKeyword = Parse.String("default").Text().Token().Named("default");

        static readonly Parser<string> Keyword = DoKeyword
                                                .Or(WhileKeyword)
                                                .Or(VarKeyword)
                                                .Or(IfKeyword)
                                                .Or(ReturnKeyword)
                                                .Or(StringKeyword)
                                                .Or(IntKeyword)
                                                .Or(BoolKeyword)
                                                .Or(TrueKeyword)
                                                .Or(FalseKeyword)
                                                .Or(SwitchKeyword)
                                                .Or(DefaultKeyword)
                                                .Or(ElseKeyword).Named("Keyword");


        static readonly Parser<int> Number =
         Parse.Number
            .Select(x => int.Parse(x))
            .Named("number");



        static readonly Parser<DiceSyntax> Dice = (from count in Number.Optional()
                                                   from d in Parse.Chars("DW")
                                                   from number in Number
                                                   select new DiceSyntax(number, count.GetOrElse(1))).Named("Dice");

        static readonly Parser<IdentifierSyntax> Identifier = (from firstChar in Parse.Lower.Once()
                                                               from rest in Parse.LetterOrDigit.Many()
                                                               select new IdentifierSyntax(new string(firstChar.Concat(rest).ToArray())))
                                                                .Except(Keyword).Named("Identifier");


        static class StringParser
        {
            private static Parser<IEnumerable<char>> Escape = Parse.Char('\\').Once();
            private static Parser<IEnumerable<char>> Quote = Parse.Char('"').Once();
            private static Parser<IEnumerable<char>> EscapedCharacter = from escape in Escape
                                                                        from c in Parse.Chars('\\', '"').Once()
                                                                        select c;


            private static Parser<IEnumerable<char>> EscapelessLiteral = Parse.AnyChar.Except(Quote).Except(Escape).Many();

            public static Parser<string> StringLiteral = from start in Quote
                                                         from content in EscapelessLiteral.Or(EscapedCharacter).Many()
                                                         from end in Quote
                                                         select new string(content.SelectMany(x => x).ToArray());

        }


        private static Parser<ConstSyntax<string>> StringConstant = from literal in StringParser.StringLiteral
                                                                    select new ConstSyntax<string>(literal);

        static readonly Parser<ConstSyntax<int>> ConstantNumber =
             Number
             .Select(x => new ConstSyntax<int>(x)).Named("Const");


        static readonly Parser<ConstSyntax<bool>> ConstantTrue = FalseKeyword.Return(new ConstSyntax<bool>(false));
        static readonly Parser<ConstSyntax<bool>> ConstantFalse = TrueKeyword.Return(new ConstSyntax<bool>(true));
        static readonly Parser<ConstSyntax<bool>> ConstantBool = ConstantTrue.XOr(ConstantFalse);

        static readonly Parser<CaseSyntax> Case = from op in CompareOperators
                                                  from input in Parse.Ref(() => Expr)
                                                  from collon in Parse.Char(':').Token()
                                                  from result in Parse.Ref(() => Expr)
                                                  from seperator in Parse.Char(';').Token()
                                                  select new CaseSyntax(input, op, result);

        static readonly Parser<CaseSyntax[]> CaseList = from c in Case.XMany()
                                                        select c.ToArray();


        static readonly Parser<SwitchSyntax> Switch = from target in Identifier
                                                      from keyword in SwitchKeyword
                                                      from input in Parse.Ref(() => Expr)
                                                      from collon in Parse.Char(':').Token()
                                                      from cases in CaseList
                                                      from defaultKeyword in DefaultKeyword
                                                      from collon2 in Parse.Char(':').Token()
                                                      from defaultResult in Parse.Ref(() => Expr)
                                                      from seperator in Parse.Char(';').Token()

                                                      select new SwitchSyntax(target, input, defaultResult, cases);



        static readonly Parser<ExpresionSyntax> Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Expr)
             from rparen in Parse.Char(')')
             select new ParentisedSyntax(expr) as ExpresionSyntax).Named("expression")
             .Or(Dice)
             .Or(ConstantNumber)
             .Or(ConstantBool)
             .Or(Identifier)
             .Or(StringConstant)
            //.XOr(Function)
            ;

        static readonly Parser<ExpresionSyntax> Operand = Factor.Token();
        //((from sign in Parse.Char('-')
        //  from factor in Factor
        //  select Expression.Negate(factor)
        // ).XOr(Factor)).Token();

        private static ExpresionSyntax MakeOperation(BinaryOperator op, ExpresionSyntax first, ExpresionSyntax seccond) => new BinaryOpereratorSyntax(op, first, seccond);

        static readonly Parser<ExpresionSyntax> Term1 = Operand;
        static readonly Parser<ExpresionSyntax> Term2 = Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), Term1, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term3 = Parse.ChainOperator(Add.Or(Subtract), Term2, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term4 = Parse.ChainOperator(ShiftLeft.Or(ShiftRight), Term3, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term5 = Parse.ChainOperator(LessOrEqual.Or(GreaterOrEqual).Or(LessThen).Or(GreaterThen), Term4, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term6 = Parse.ChainOperator(AreEqual.Or(NotEquals), Term5, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term7 = Parse.ChainOperator(BitAnd, Term6, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term8 = Parse.ChainOperator(BitXor, Term7, MakeOperation);
        static readonly Parser<ExpresionSyntax> Term9 = Parse.ChainOperator(BitOr, Term8, MakeOperation);


        static readonly Parser<ExpresionSyntax> Expr = Parse.ChainOperator(Add.Or(Subtract), Term9, MakeOperation);




        static readonly Parser<Type> IntTypes = IntKeyword.Return(typeof(int));
        static readonly Parser<Type> StringTypes = StringKeyword.Return(typeof(string));
        static readonly Parser<Type> BoolTypes = BoolKeyword.Return(typeof(bool));
        static readonly Parser<Type> Types = IntTypes.Or(StringTypes).Or(BoolTypes);

        static readonly Parser<StatementSyntax> VariableDeclaration = from keyword in VarKeyword
                                                                      from identeifer in Identifier.Token()
                                                                      from colon in Parse.Char(':').Token()
                                                                      from type in Types.Token()
                                                                      select new VariableDeclarationSyntax(type, identeifer);

        static readonly Parser<StatementSyntax> VariableAssignment = from assignedTo in VariableDeclaration.Or<object>(Identifier)
                                                                     from assigneOperator in Parse.Char('=').Token()
                                                                     from exp in Expr
                                                                     select assignedTo is IdentifierSyntax
                                                                        ? new VariableAssignmentSyntax((IdentifierSyntax)assignedTo, exp)
                                                                        : new VariableAssignmentSyntax((VariableDeclarationSyntax)assignedTo, exp);


        static readonly Parser<BlockSyntax> Block = from opening in Parse.Char('{').Token()
                                                    from list in Parse.Ref(() => StatementList)
                                                    from closing in Parse.Char('}').Token()
                                                    select new BlockSyntax(list);

        static readonly Parser<DoWhileSyntax> DoWhile = from @do in DoKeyword
                                                        from statement in Parse.Ref(() => Statement)
                                                        from @while in WhileKeyword
                                                        from condition in Expr
                                                        select new DoWhileSyntax(statement, condition);

        static readonly Parser<ElseSyntax> Else = from @else in ElseKeyword
                                                  from then in Parse.Ref(() => Statement)
                                                  select new ElseSyntax(then);

        static readonly Parser<IfSyntax> If = from @if in IfKeyword
                                              from condition in Expr
                                              from then in Parse.Ref(() => Statement)
                                              from @else in Else.Optional()
                                              select new IfSyntax(condition, then, @else.GetOrDefault());



        static readonly Parser<string> CommentLine = Parse.Char('#').Then(x => Parse.AnyChar.Until(Parse.LineEnd)).Token().Text();
        static readonly Parser<StatementSyntax> Comment = from c in CommentLine.XMany()
                                                          select new CommentSyntax(c);



        static readonly Parser<StatementSyntax> Statement =
                                                                If
                                                                .Or(Comment)
                                                                .Or(VariableAssignment)
                                                                .Or(VariableDeclaration)
                                                                .Or(Block)
                                                                .Or(DoWhile)
                                                                .Or(Switch)
                                                                ;


        static readonly Parser<StatementSyntax[]> StatementList = from s in Statement.Many()
                                                                  select s.ToArray();

        static readonly Parser<ProgramSyntax> Program = from s in StatementList
                                                        from keyword in ReturnKeyword
                                                        from @return in Expr.End()
                                                        select new ProgramSyntax(s, @return);




        //Expr.End().Select(body => Expression.Lambda<Func<double>>(body));
    }
}
