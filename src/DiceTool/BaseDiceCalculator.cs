using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    public abstract class BaseDiceCalculato<T>
    {
        internal WAutomata<T> automata = new WAutomata<T>();
        #region Dices
        protected D<T> W2 { get; }
        protected D<T> W3 { get; }
        protected D<T> W4 { get; }
        protected D<T> W5 { get; }
        protected D<T> W6 { get; }
        protected D<T> W7 { get; }
        protected D<T> W8 { get; }
        protected D<T> W9 { get; }
        protected D<T> W10 { get; }
        protected D<T> W11 { get; }
        protected D<T> W12 { get; }
        protected D<T> W13 { get; }
        protected D<T> W14 { get; }
        protected D<T> W15 { get; }
        protected D<T> W16 { get; }
        protected D<T> W17 { get; }
        protected D<T> W18 { get; }
        protected D<T> W19 { get; }
        protected D<T> W20 { get; }
        protected D<T> W21 { get; }
        protected D<T> W22 { get; }
        protected D<T> W23 { get; }
        protected D<T> W24 { get; }
        protected D<T> W25 { get; }
        protected D<T> W26 { get; }
        protected D<T> W27 { get; }
        protected D<T> W28 { get; }
        protected D<T> W29 { get; }
        protected D<T> W30 { get; }
        protected D<T> W31 { get; }
        protected D<T> W32 { get; }
        protected D<T> W33 { get; }
        protected D<T> W34 { get; }
        protected D<T> W35 { get; }
        protected D<T> W36 { get; }
        protected D<T> W37 { get; }
        protected D<T> W38 { get; }
        protected D<T> W39 { get; }
        protected D<T> W40 { get; }
        protected D<T> W41 { get; }
        protected D<T> W42 { get; }
        protected D<T> W43 { get; }
        protected D<T> W44 { get; }
        protected D<T> W45 { get; }
        protected D<T> W46 { get; }
        protected D<T> W47 { get; }
        protected D<T> W48 { get; }
        protected D<T> W49 { get; }
        protected D<T> W50 { get; }
        protected D<T> W51 { get; }
        protected D<T> W52 { get; }
        protected D<T> W53 { get; }
        protected D<T> W54 { get; }
        protected D<T> W55 { get; }
        protected D<T> W56 { get; }
        protected D<T> W57 { get; }
        protected D<T> W58 { get; }
        protected D<T> W59 { get; }
        protected D<T> W60 { get; }
        protected D<T> W61 { get; }
        protected D<T> W62 { get; }
        protected D<T> W63 { get; }
        protected D<T> W64 { get; }
        protected D<T> W65 { get; }
        protected D<T> W66 { get; }
        protected D<T> W67 { get; }
        protected D<T> W68 { get; }
        protected D<T> W69 { get; }
        protected D<T> W70 { get; }
        protected D<T> W71 { get; }
        protected D<T> W72 { get; }
        protected D<T> W73 { get; }
        protected D<T> W74 { get; }
        protected D<T> W75 { get; }
        protected D<T> W76 { get; }
        protected D<T> W77 { get; }
        protected D<T> W78 { get; }
        protected D<T> W79 { get; }
        protected D<T> W80 { get; }
        protected D<T> W81 { get; }
        protected D<T> W82 { get; }
        protected D<T> W83 { get; }
        protected D<T> W84 { get; }
        protected D<T> W85 { get; }
        protected D<T> W86 { get; }
        protected D<T> W87 { get; }
        protected D<T> W88 { get; }
        protected D<T> W89 { get; }
        protected D<T> W90 { get; }
        protected D<T> W91 { get; }
        protected D<T> W92 { get; }
        protected D<T> W93 { get; }
        protected D<T> W94 { get; }
        protected D<T> W95 { get; }
        protected D<T> W96 { get; }
        protected D<T> W97 { get; }
        protected D<T> W98 { get; }
        protected D<T> W99 { get; }
        protected D<T> W100 { get; }

        protected D<T> D2 { get; }
        protected D<T> D3 { get; }
        protected D<T> D4 { get; }
        protected D<T> D5 { get; }
        protected D<T> D6 { get; }
        protected D<T> D7 { get; }
        protected D<T> D8 { get; }
        protected D<T> D9 { get; }
        protected D<T> D10 { get; }
        protected D<T> D11 { get; }
        protected D<T> D12 { get; }
        protected D<T> D13 { get; }
        protected D<T> D14 { get; }
        protected D<T> D15 { get; }
        protected D<T> D16 { get; }
        protected D<T> D17 { get; }
        protected D<T> D18 { get; }
        protected D<T> D19 { get; }
        protected D<T> D20 { get; }
        protected D<T> D21 { get; }
        protected D<T> D22 { get; }
        protected D<T> D23 { get; }
        protected D<T> D24 { get; }
        protected D<T> D25 { get; }
        protected D<T> D26 { get; }
        protected D<T> D27 { get; }
        protected D<T> D28 { get; }
        protected D<T> D29 { get; }
        protected D<T> D30 { get; }
        protected D<T> D31 { get; }
        protected D<T> D32 { get; }
        protected D<T> D33 { get; }
        protected D<T> D34 { get; }
        protected D<T> D35 { get; }
        protected D<T> D36 { get; }
        protected D<T> D37 { get; }
        protected D<T> D38 { get; }
        protected D<T> D39 { get; }
        protected D<T> D40 { get; }
        protected D<T> D41 { get; }
        protected D<T> D42 { get; }
        protected D<T> D43 { get; }
        protected D<T> D44 { get; }
        protected D<T> D45 { get; }
        protected D<T> D46 { get; }
        protected D<T> D47 { get; }
        protected D<T> D48 { get; }
        protected D<T> D49 { get; }
        protected D<T> D50 { get; }
        protected D<T> D51 { get; }
        protected D<T> D52 { get; }
        protected D<T> D53 { get; }
        protected D<T> D54 { get; }
        protected D<T> D55 { get; }
        protected D<T> D56 { get; }
        protected D<T> D57 { get; }
        protected D<T> D58 { get; }
        protected D<T> D59 { get; }
        protected D<T> D60 { get; }
        protected D<T> D61 { get; }
        protected D<T> D62 { get; }
        protected D<T> D63 { get; }
        protected D<T> D64 { get; }
        protected D<T> D65 { get; }
        protected D<T> D66 { get; }
        protected D<T> D67 { get; }
        protected D<T> D68 { get; }
        protected D<T> D69 { get; }
        protected D<T> D70 { get; }
        protected D<T> D71 { get; }
        protected D<T> D72 { get; }
        protected D<T> D73 { get; }
        protected D<T> D74 { get; }
        protected D<T> D75 { get; }
        protected D<T> D76 { get; }
        protected D<T> D77 { get; }
        protected D<T> D78 { get; }
        protected D<T> D79 { get; }
        protected D<T> D80 { get; }
        protected D<T> D81 { get; }
        protected D<T> D82 { get; }
        protected D<T> D83 { get; }
        protected D<T> D84 { get; }
        protected D<T> D85 { get; }
        protected D<T> D86 { get; }
        protected D<T> D87 { get; }
        protected D<T> D88 { get; }
        protected D<T> D89 { get; }
        protected D<T> D90 { get; }
        protected D<T> D91 { get; }
        protected D<T> D92 { get; }
        protected D<T> D93 { get; }
        protected D<T> D94 { get; }
        protected D<T> D95 { get; }
        protected D<T> D96 { get; }
        protected D<T> D97 { get; }
        protected D<T> D98 { get; }
        protected D<T> D99 { get; }
        protected D<T> D100 { get; }

        #endregion


        protected BaseDiceCalculato()
        {
            W2 = new D<T>(2, automata);
            W3 = new D<T>(3, automata);
            W4 = new D<T>(4, automata);
            W5 = new D<T>(5, automata);
            W6 = new D<T>(6, automata);
            W7 = new D<T>(7, automata);
            W8 = new D<T>(8, automata);
            W9 = new D<T>(9, automata);
            W10 = new D<T>(10, automata);
            W11 = new D<T>(11, automata);
            W12 = new D<T>(12, automata);
            W13 = new D<T>(13, automata);
            W14 = new D<T>(14, automata);
            W15 = new D<T>(15, automata);
            W16 = new D<T>(16, automata);
            W17 = new D<T>(17, automata);
            W18 = new D<T>(18, automata);
            W19 = new D<T>(19, automata);
            W20 = new D<T>(20, automata);
            W21 = new D<T>(21, automata);
            W22 = new D<T>(22, automata);
            W23 = new D<T>(23, automata);
            W24 = new D<T>(24, automata);
            W25 = new D<T>(25, automata);
            W26 = new D<T>(26, automata);
            W27 = new D<T>(27, automata);
            W28 = new D<T>(28, automata);
            W29 = new D<T>(29, automata);
            W30 = new D<T>(30, automata);
            W31 = new D<T>(31, automata);
            W32 = new D<T>(32, automata);
            W33 = new D<T>(33, automata);
            W34 = new D<T>(34, automata);
            W35 = new D<T>(35, automata);
            W36 = new D<T>(36, automata);
            W37 = new D<T>(37, automata);
            W38 = new D<T>(38, automata);
            W39 = new D<T>(39, automata);
            W40 = new D<T>(40, automata);
            W41 = new D<T>(41, automata);
            W42 = new D<T>(42, automata);
            W43 = new D<T>(43, automata);
            W44 = new D<T>(44, automata);
            W45 = new D<T>(45, automata);
            W46 = new D<T>(46, automata);
            W47 = new D<T>(47, automata);
            W48 = new D<T>(48, automata);
            W49 = new D<T>(49, automata);
            W50 = new D<T>(50, automata);
            W51 = new D<T>(51, automata);
            W52 = new D<T>(52, automata);
            W53 = new D<T>(53, automata);
            W54 = new D<T>(54, automata);
            W55 = new D<T>(55, automata);
            W56 = new D<T>(56, automata);
            W57 = new D<T>(57, automata);
            W58 = new D<T>(58, automata);
            W59 = new D<T>(59, automata);
            W60 = new D<T>(60, automata);
            W61 = new D<T>(61, automata);
            W62 = new D<T>(62, automata);
            W63 = new D<T>(63, automata);
            W64 = new D<T>(64, automata);
            W65 = new D<T>(65, automata);
            W66 = new D<T>(66, automata);
            W67 = new D<T>(67, automata);
            W68 = new D<T>(68, automata);
            W69 = new D<T>(69, automata);
            W70 = new D<T>(70, automata);
            W71 = new D<T>(71, automata);
            W72 = new D<T>(72, automata);
            W73 = new D<T>(73, automata);
            W74 = new D<T>(74, automata);
            W75 = new D<T>(75, automata);
            W76 = new D<T>(76, automata);
            W77 = new D<T>(77, automata);
            W78 = new D<T>(78, automata);
            W79 = new D<T>(79, automata);
            W80 = new D<T>(80, automata);
            W81 = new D<T>(81, automata);
            W82 = new D<T>(82, automata);
            W83 = new D<T>(83, automata);
            W84 = new D<T>(84, automata);
            W85 = new D<T>(85, automata);
            W86 = new D<T>(86, automata);
            W87 = new D<T>(87, automata);
            W88 = new D<T>(88, automata);
            W89 = new D<T>(89, automata);
            W90 = new D<T>(90, automata);
            W91 = new D<T>(91, automata);
            W92 = new D<T>(92, automata);
            W93 = new D<T>(93, automata);
            W94 = new D<T>(94, automata);
            W95 = new D<T>(95, automata);
            W96 = new D<T>(96, automata);
            W97 = new D<T>(97, automata);
            W98 = new D<T>(98, automata);
            W99 = new D<T>(99, automata);
            W100 = new D<T>(100, automata);

            D2 = new D<T>(2, automata);
            D3 = new D<T>(3, automata);
            D4 = new D<T>(4, automata);
            D5 = new D<T>(5, automata);
            D6 = new D<T>(6, automata);
            D7 = new D<T>(7, automata);
            D8 = new D<T>(8, automata);
            D9 = new D<T>(9, automata);
            D10 = new D<T>(10, automata);
            D11 = new D<T>(11, automata);
            D12 = new D<T>(12, automata);
            D13 = new D<T>(13, automata);
            D14 = new D<T>(14, automata);
            D15 = new D<T>(15, automata);
            D16 = new D<T>(16, automata);
            D17 = new D<T>(17, automata);
            D18 = new D<T>(18, automata);
            D19 = new D<T>(19, automata);
            D20 = new D<T>(20, automata);
            D21 = new D<T>(21, automata);
            D22 = new D<T>(22, automata);
            D23 = new D<T>(23, automata);
            D24 = new D<T>(24, automata);
            D25 = new D<T>(25, automata);
            D26 = new D<T>(26, automata);
            D27 = new D<T>(27, automata);
            D28 = new D<T>(28, automata);
            D29 = new D<T>(29, automata);
            D30 = new D<T>(30, automata);
            D31 = new D<T>(31, automata);
            D32 = new D<T>(32, automata);
            D33 = new D<T>(33, automata);
            D34 = new D<T>(34, automata);
            D35 = new D<T>(35, automata);
            D36 = new D<T>(36, automata);
            D37 = new D<T>(37, automata);
            D38 = new D<T>(38, automata);
            D39 = new D<T>(39, automata);
            D40 = new D<T>(40, automata);
            D41 = new D<T>(41, automata);
            D42 = new D<T>(42, automata);
            D43 = new D<T>(43, automata);
            D44 = new D<T>(44, automata);
            D45 = new D<T>(45, automata);
            D46 = new D<T>(46, automata);
            D47 = new D<T>(47, automata);
            D48 = new D<T>(48, automata);
            D49 = new D<T>(49, automata);
            D50 = new D<T>(50, automata);
            D51 = new D<T>(51, automata);
            D52 = new D<T>(52, automata);
            D53 = new D<T>(53, automata);
            D54 = new D<T>(54, automata);
            D55 = new D<T>(55, automata);
            D56 = new D<T>(56, automata);
            D57 = new D<T>(57, automata);
            D58 = new D<T>(58, automata);
            D59 = new D<T>(59, automata);
            D60 = new D<T>(60, automata);
            D61 = new D<T>(61, automata);
            D62 = new D<T>(62, automata);
            D63 = new D<T>(63, automata);
            D64 = new D<T>(64, automata);
            D65 = new D<T>(65, automata);
            D66 = new D<T>(66, automata);
            D67 = new D<T>(67, automata);
            D68 = new D<T>(68, automata);
            D69 = new D<T>(69, automata);
            D70 = new D<T>(70, automata);
            D71 = new D<T>(71, automata);
            D72 = new D<T>(72, automata);
            D73 = new D<T>(73, automata);
            D74 = new D<T>(74, automata);
            D75 = new D<T>(75, automata);
            D76 = new D<T>(76, automata);
            D77 = new D<T>(77, automata);
            D78 = new D<T>(78, automata);
            D79 = new D<T>(79, automata);
            D80 = new D<T>(80, automata);
            D81 = new D<T>(81, automata);
            D82 = new D<T>(82, automata);
            D83 = new D<T>(83, automata);
            D84 = new D<T>(84, automata);
            D85 = new D<T>(85, automata);
            D86 = new D<T>(86, automata);
            D87 = new D<T>(87, automata);
            D88 = new D<T>(88, automata);
            D89 = new D<T>(89, automata);
            D90 = new D<T>(90, automata);
            D91 = new D<T>(91, automata);
            D92 = new D<T>(92, automata);
            D93 = new D<T>(93, automata);
            D94 = new D<T>(94, automata);
            D95 = new D<T>(95, automata);
            D96 = new D<T>(96, automata);
            D97 = new D<T>(97, automata);
            D98 = new D<T>(98, automata);
            D99 = new D<T>(99, automata);
            D100 = new D<T>(100, automata);

        }
    }
}
