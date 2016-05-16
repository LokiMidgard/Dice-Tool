using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Text.RegularExpressions;

namespace DiceConsoleTest
{
    class YieldRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel SemanticModel;


        public YieldRewriter(SemanticModel model)
        {
            this.SemanticModel = model;
        }

        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            return YieldStatement(SyntaxKind.YieldReturnStatement, node.Expression);
        }



    }

    static class WRewriter
    {



        public static SyntaxNode Rewrite(SemanticModel model, SyntaxTree sourceTree)
        {
            SyntaxNode node = sourceTree.GetRoot();
            //SemanticModel model = test.GetSemanticModel(sourceTree);
            var toChanges = node.DescendantNodes(x => !IsWDeclaration(x, model)).Where(x => IsWDeclaration(x, model)).Cast<LocalDeclarationStatementSyntax>();
            node = node.TrackNodes(toChanges);
            foreach (var toChange in toChanges)
            {
                var current = node.GetCurrentNode(toChange);

                var silblings = current.Parent.ChildNodes().ToList();
                var ownPosition = silblings.IndexOf(current);
                var silblingsAfter = silblings.Skip(1 + ownPosition).Cast<StatementSyntax>();


                var symbols = model.LookupSymbols(silblingsAfter.First().FullSpan.Start).OfType<ILocalSymbol>().Where(x => x.Type.Name != "W" && x.Locations.Single().SourceSpan.Start < silblingsAfter.First().FullSpan.Start);






                var dice = toChange.Declaration.Variables[0].Initializer.Value.GetText().ToString();
                var name = toChange.Declaration.Variables[0].Identifier.ValueText;
                var reg = new Regex(@"(?<count>\d+)?[DdWw](?<size>\d+)");
                var match = reg.Match(dice);

                var size = int.Parse(match.Groups["size"].Value);
                var count = int.Parse(match.Groups["count"].Success ? match.Groups["count"].Value : "1");

                var identifires = silblingsAfter.SelectMany(y => y.DescendantTokens().Where(x => x.IsKind(SyntaxKind.IdentifierToken)));

                var newSyntaxtree = SyntaxTree(node);
                model = model.Compilation.ReplaceSyntaxTree(sourceTree, newSyntaxtree).GetSemanticModel(newSyntaxtree);
                sourceTree = newSyntaxtree;
                foreach (var item in identifires)
                {
                    var sinfo = model.GetSymbolInfo(item.Parent);
                }

                var blockSyntax = Block(silblingsAfter);

                var each = ForEachStatement(IdentifierName("var"), name, ParseExpression($"D({count},{size})"), blockSyntax);


                node = node.RemoveNodes(silblingsAfter, SyntaxRemoveOptions.KeepNoTrivia);

                current = node.GetCurrentNode(toChange);

                node = node.ReplaceNode(current, each);

            }
            return node;
        }

        private static bool IsWDeclaration(SyntaxNode node, SemanticModel model)
        {
            var decNode = node as LocalDeclarationStatementSyntax;
            if (decNode == null)
                return false;
            if (decNode.Declaration.Variables.Count > 1)
            {
                return false; // Maybe not suport?
            }
            VariableDeclaratorSyntax declarator = decNode.Declaration.Variables.First();
            TypeSyntax variableTypeName = decNode.Declaration.Type;
            //ITypeSymbol variableType =
            //               (ITypeSymbol)SemanticModel.GetSymbolInfo(variableTypeName)
            //                                        .Symbol;
            TypeInfo initializerInfo =
                         model.GetTypeInfo(declarator
                                                   .Initializer
                                                   .Value);

            if (initializerInfo.Type.Name == "W")
            {
                // Dann mach spezielle sachen :)
                return true;
            }

            return false;
        }
    }
}
