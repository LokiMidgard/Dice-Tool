//using Sprache;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Text;
//using Dice.Parser.Syntax;

//namespace Dice.Parser
//{
//    public class Parsers
//    {
//        public static void Test()
//        {
//            var g = new Gramer();
//            var program = g.Program.Parse(@"
//return x<4
//");
//        }
//    }
//    class Gramer
//    {

//        static Parser<IdentifierSyntax> Identifier => from firstChar in Parse.Lower.Once()
//                                                      from rest in Parse.LetterOrDigit.Many()
//                                                      select new IdentifierSyntax(new string(firstChar.Concat(rest).ToArray()));


//        static class StringParser
//        {
//            private static Parser<IEnumerable<char>> Escape => Parse.Char('\\').Once();
//            private static Parser<IEnumerable<char>> Quote => Parse.Char('"').Once();
//            private static Parser<IEnumerable<char>> EscapedCharacter => from escape in Escape
//                                                                         from c in Parse.Chars('\\', '"').Once()
//                                                                         select c;


//            private static Parser<IEnumerable<char>> EscapelessLiteral => Parse.AnyChar.Except(Quote).Except(Escape).Many();

//            public static Parser<string> StringLiteral => from start in Quote
//                                                          from content in EscapelessLiteral.Or(EscapedCharacter).Many()
//                                                          from end in Quote
//                                                          select new string(content.SelectMany(x => x).ToArray());

//        }


//        private Parser<ConstSyntax<string>> StringConstant => from literal in StringParser.StringLiteral
//                                                              select new ConstSyntax<string>(literal);

//        private Parser<ConstSyntax<int>> IntConstant => from literal in this.Number
//                                                        select new ConstSyntax<int>(literal);


//        private Parser<int> Number => from literal in Parse.Digit.Many().Text()
//                                      select int.Parse(literal);

//        private Parser<DiceSyntax> SingelDice => from count in this.Number.Optional()
//                                                 from d in Parse.Chars("DW")
//                                                 from number in this.Number
//                                                 select new DiceSyntax(number, count.GetOrElse(1));

//        public Parser<BinaryOperator> Operator => from op in
//                                                          Parse.String("+")
//                                                      .Or(Parse.String("-"))
//                                                      .Or(Parse.String("*"))
//                                                      .Or(Parse.String("/"))
//                                                      .Or(Parse.String("%"))

//                                                      .Or(Parse.String("&&"))
//                                                      .Or(Parse.String("||"))

//                                                      .Or(Parse.String("&"))
//                                                      .Or(Parse.String("|"))
//                                                      .Or(Parse.String("^"))

//                                                      .Or(Parse.String(">>"))
//                                                      .Or(Parse.String("<<"))

//                                                      .Or(Parse.String("<="))
//                                                      .Or(Parse.String("<"))
//                                                      .Or(Parse.String(">="))
//                                                      .Or(Parse.String(">"))
//                                                      .Or(Parse.String("=="))
//                                                      .Or(Parse.String("!="))
//                                                      .Text()
//                                                  select this.OperatorSwitch(op);

//        private BinaryOperator OperatorSwitch(string op)
//        {
//            switch (op)
//            {
//                case "+": return BinaryOperator.Addition;
//                case "-": return BinaryOperator.Substraction;
//                case "*": return BinaryOperator.Multiplication;
//                case "/": return BinaryOperator.Division;
//                case "%": return BinaryOperator.Modulo;

//                case "&": return BinaryOperator.BitAnd;
//                case "|": return BinaryOperator.BitOr;
//                case "^": return BinaryOperator.BitXor;

//                case "&&": return BinaryOperator.LogicAnd;
//                case "||": return BinaryOperator.LogicOr;


//                case "<<": return BinaryOperator.ShiftRight;
//                case ">>": return BinaryOperator.ShiftLeft;


//                case "<=": return BinaryOperator.LessOrEquals;
//                case ">=": return BinaryOperator.GreaterOrEquals;
//                case "<": return BinaryOperator.LessThen;
//                case ">": return BinaryOperator.GreaterThen;
//                case "==": return BinaryOperator.Equals;
//                case "!=": return BinaryOperator.NotEquals;
//                default:
//                    throw new NotImplementedException($"The Operator {op} failed.");
//            }
//        }


//        public Parser<BinaryOpereratorSyntax> BinarayOperation => from first in this.Expresion
//                                                                  from op in this.Operator
//                                                                  from seccond in this.Expresion
//                                                                  select new BinaryOpereratorSyntax(op, first, seccond);

//        public Parser<ExpresionSyntax> Expresion => from e in Identifier
//                                                        .Or<ExpresionSyntax>(this.IntConstant)
//                                                        .Or<ExpresionSyntax>(this.StringConstant)
//                                                        .Or<ExpresionSyntax>(this.SingelDice)
//                                                        .Or<ExpresionSyntax>(this.BinarayOperation)
//                                                    select e;

//        public Parser<StatementSyntax> Statement => from s in (this.Block)
//                                                        .Or<StatementSyntax>(this.DoWhile)
//                                                        .Or<StatementSyntax>(this.If)
//                                                    select s;

//        public Parser<StatementSyntax[]> StatementList => from s in this.Statement
//                                                          from seperator in Parse.Char(';')
//                                                          from list in this.StatementList.Optional()
//                                                          select this.ConcatStatemnts(s, list);

//        public Parser<BlockSyntax> Block => from opening in Parse.Char('{')
//                                            from list in this.StatementList
//                                            from closing in Parse.Char('}')
//                                            select new BlockSyntax(list);

//        public Parser<DoWhileSyntax> DoWhile => from @do in Parse.String("do")
//                                                from statement in this.Statement
//                                                from @while in Parse.String("while")
//                                                from condition in this.Expresion
//                                                select new DoWhileSyntax(statement, condition);

//        public Parser<IfSyntax> If => from @if in Parse.String("if")
//                                      from condition in this.Expresion
//                                      from then in this.Statement
//                                      from @else in this.Else.Optional()
//                                      select new IfSyntax(condition, then, @else.GetOrDefault());

//        public Parser<ElseSyntax> Else => from @else in Parse.String("else")
//                                          from then in this.Statement
//                                          select new ElseSyntax(then);

//        public Parser<ProgramSyntax> Program => from s in this.StatementList.Optional()
//                                                from keyword in Parse.String("return").Token()
//                                                from @return in this.Expresion
//                                                select new ProgramSyntax(s.GetOrElse(new StatementSyntax[0]), @return);


//        private StatementSyntax[] ConcatStatemnts(StatementSyntax s, IOption<StatementSyntax[]> list)
//        {
//            var statements = list.GetOrDefault();
//            var array = new StatementSyntax[1 + statements?.Length ?? 0];
//            array[0] = s;
//            if (statements != null)
//                Array.Copy(statements, 0, array, 0, statements.Length);
//            return array;
//        }
//    }

//}
