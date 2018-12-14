//using Sprache;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Text;

//namespace Dice
//{
//    class Gramer
//    {
//        public Gramer(IComposer composer)
//        {
//            this.composer = composer;
//        }


//        static Parser<IP> Identifier => from firstChar in Parse.Lower.Once()
//                                        from rest in Parse.LetterOrDigit.Many()
//                                        select P.FromIdentifier(new string(firstChar.Concat(rest).ToArray()));


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


//        private Parser<P<string>> StringConstant => from literal in StringParser.StringLiteral
//                                                    select this.composer.CreateConstState(literal);

//        private Parser<P<int>> IntConstant => from literal in Number
//                                              select this.composer.CreateConstState(literal);


//        private Parser<int> Number => from literal in Parse.Digit.Many().Text()
//                                      select int.Parse(literal);

//        private Parser<P<int>> SingelDice => from d in Parse.Chars("DW")
//                                             from number in Number
//                                             select this.composer.CreateVariableState(Enumerable.Range(1, number).Select(i => (i, 1.0 / number)).ToArray());

//        public Parser<CompareOperator> Operator => from op in Parse.String("<=").Or(Parse.String("<")).Or(Parse.String(">=")).Or(Parse.String(">")).Or(Parse.String("==")).Or(Parse.String("!=")).Text()
//                                                   select OperatorSwitch(op);

//        private CompareOperator OperatorSwitch(string op)
//        {
//            switch (op)
//            {
//                case "<=": return CompareOperator.LessOrEquals;
//                case ">=": return CompareOperator.GreaterOrEquals;
//                case "<": return CompareOperator.LessThen;
//                case ">": return CompareOperator.GreaterThen;
//                case "==": return CompareOperator.Equals;
//                case "!=": return CompareOperator.NotEquals;
//                default:
//                    throw new NotImplementedException($"The Operator {op} failed.");
//            }
//        }

//        private Parser<P<bool>> StringCompare => from i1 in StringConstant.Or(Identifier.Select(x => (P<string>)x))
//                                                 from op in Operator
//                                                 from i2 in StringConstant.Or(Identifier.Select(x => (P<string>)x))
//                                                 select composer.CreateCombineState(i1, i2, (e1, e2) =>
//                                                 {
//                                                     bool result;
//                                                     switch (op)
//                                                     {
//                                                         case CompareOperator.LessThen:
//                                                             result = e1.CompareTo(e2) < 0;
//                                                             break;
//                                                         case CompareOperator.LessOrEquals:
//                                                             result = e1.CompareTo(e2) <= 0;
//                                                             break;
//                                                         case CompareOperator.GreaterThen:
//                                                             result = e1.CompareTo(e2) > 0;
//                                                             break;
//                                                         case CompareOperator.GreaterOrEquals:
//                                                             result = e1.CompareTo(e2) >= 0;
//                                                             break;
//                                                         case CompareOperator.Equals:
//                                                             result = e1 == e2;
//                                                             break;
//                                                         case CompareOperator.NotEquals:
//                                                             result = e1 != e2;
//                                                             break;
//                                                         default:
//                                                             throw new NotSupportedException($"The operator {op} is not supported.");
//                                                     }
//                                                     return result;
//                                                 });

//        private Parser<P<bool>> IntCompare => from i1 in IntConstant.Or(Identifier.Select(x => (P<int>)x))
//                                              from op in Operator
//                                              from i2 in IntConstant.Or(Identifier.Select(x => (P<int>)x))
//                                              select composer.CreateCombineState(i1, i2, (e1, e2) =>
//                                              {
//                                                  bool result;
//                                                  switch (op)
//                                                  {
//                                                      case CompareOperator.LessThen:
//                                                          result = e1.CompareTo(e2) < 0;
//                                                          break;
//                                                      case CompareOperator.LessOrEquals:
//                                                          result = e1.CompareTo(e2) <= 0;
//                                                          break;
//                                                      case CompareOperator.GreaterThen:
//                                                          result = e1.CompareTo(e2) > 0;
//                                                          break;
//                                                      case CompareOperator.GreaterOrEquals:
//                                                          result = e1.CompareTo(e2) >= 0;
//                                                          break;
//                                                      case CompareOperator.Equals:
//                                                          result = e1 == e2;
//                                                          break;
//                                                      case CompareOperator.NotEquals:
//                                                          result = e1 != e2;
//                                                          break;
//                                                      default:
//                                                          throw new NotSupportedException($"The operator {op} is not supported.");
//                                                  }
//                                                  return result;
//                                              });






//        static readonly Parser<P<int>> IntConst = Parse.String(op).Token().Return(opType);

//        static readonly Parser<char> Addition = Parse.String(op).Token().Return(opType);
//        private readonly IComposer composer;
//    }

//    enum CompareOperator
//    {
//        LessThen,
//        LessOrEquals,
//        GreaterThen,
//        GreaterOrEquals,
//        Equals,
//        NotEquals
//    }
//}
