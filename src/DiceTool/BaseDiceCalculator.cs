using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseDiceCalculator<T> : BaseBaseDiceCalculator<T>
    {
        internal override WAutomataBase<T> Automata { get; } = new WAutomata<T>();
        internal BaseDiceCalculator()
        {

        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseDiceCalculator<T, P1> : BaseBaseDiceCalculator<T>
    {
        internal override WAutomataBase<T> Automata { get; } = new WAutomata<T, P1>();
        internal BaseDiceCalculator()
        {

        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseDiceCalculator<T, P1, P2> : BaseBaseDiceCalculator<T>
    {
        internal override WAutomataBase<T> Automata { get; } = new WAutomata<T, P1, P2>();
        internal BaseDiceCalculator()
        {

        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseDiceCalculator<T, P1, P2, P3> : BaseBaseDiceCalculator<T>
    {
        internal override WAutomataBase<T> Automata { get; } = new WAutomata<T, P1, P2, P3>();
        internal BaseDiceCalculator()
        {

        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseDiceCalculator<T, P1, P2, P3, P4> : BaseBaseDiceCalculator<T>
    {
        internal override WAutomataBase<T> Automata { get; } = new WAutomata<T, P1, P2, P3, P4>();
        internal BaseDiceCalculator()
        {

        }
    }


    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseDiceCalculator<T, P1, P2, P3, P4, P5> : BaseBaseDiceCalculator<T>
    {
        internal override WAutomataBase<T> Automata { get; } = new WAutomata<T, P1, P2, P3, P4, P5>();

        internal BaseDiceCalculator()
        {

        }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BaseBaseDiceCalculator<T>
    {
        internal abstract WAutomataBase<T> Automata { get; }

        protected D<T, TParam> CreateDice<TParam>(params (TParam value, double propability)[] faces) => this.CreateDice(faces as IEnumerable<(TParam value, double propability)>);
        protected D<T, TParam> CreateDice<TParam>(IEnumerable<(TParam value, double propability)> faces) => new D<T, TParam>(this.Automata, faces.Select(x => x.value).ToList(), faces.Select(x => x.propability).ToList());

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


        internal BaseBaseDiceCalculator()
        {
            W2 = new D<T>(2, Automata);
            W3 = new D<T>(3, Automata);
            W4 = new D<T>(4, Automata);
            W5 = new D<T>(5, Automata);
            W6 = new D<T>(6, Automata);
            W7 = new D<T>(7, Automata);
            W8 = new D<T>(8, Automata);
            W9 = new D<T>(9, Automata);
            W10 = new D<T>(10, Automata);
            W11 = new D<T>(11, Automata);
            W12 = new D<T>(12, Automata);
            W13 = new D<T>(13, Automata);
            W14 = new D<T>(14, Automata);
            W15 = new D<T>(15, Automata);
            W16 = new D<T>(16, Automata);
            W17 = new D<T>(17, Automata);
            W18 = new D<T>(18, Automata);
            W19 = new D<T>(19, Automata);
            W20 = new D<T>(20, Automata);
            W21 = new D<T>(21, Automata);
            W22 = new D<T>(22, Automata);
            W23 = new D<T>(23, Automata);
            W24 = new D<T>(24, Automata);
            W25 = new D<T>(25, Automata);
            W26 = new D<T>(26, Automata);
            W27 = new D<T>(27, Automata);
            W28 = new D<T>(28, Automata);
            W29 = new D<T>(29, Automata);
            W30 = new D<T>(30, Automata);
            W31 = new D<T>(31, Automata);
            W32 = new D<T>(32, Automata);
            W33 = new D<T>(33, Automata);
            W34 = new D<T>(34, Automata);
            W35 = new D<T>(35, Automata);
            W36 = new D<T>(36, Automata);
            W37 = new D<T>(37, Automata);
            W38 = new D<T>(38, Automata);
            W39 = new D<T>(39, Automata);
            W40 = new D<T>(40, Automata);
            W41 = new D<T>(41, Automata);
            W42 = new D<T>(42, Automata);
            W43 = new D<T>(43, Automata);
            W44 = new D<T>(44, Automata);
            W45 = new D<T>(45, Automata);
            W46 = new D<T>(46, Automata);
            W47 = new D<T>(47, Automata);
            W48 = new D<T>(48, Automata);
            W49 = new D<T>(49, Automata);
            W50 = new D<T>(50, Automata);
            W51 = new D<T>(51, Automata);
            W52 = new D<T>(52, Automata);
            W53 = new D<T>(53, Automata);
            W54 = new D<T>(54, Automata);
            W55 = new D<T>(55, Automata);
            W56 = new D<T>(56, Automata);
            W57 = new D<T>(57, Automata);
            W58 = new D<T>(58, Automata);
            W59 = new D<T>(59, Automata);
            W60 = new D<T>(60, Automata);
            W61 = new D<T>(61, Automata);
            W62 = new D<T>(62, Automata);
            W63 = new D<T>(63, Automata);
            W64 = new D<T>(64, Automata);
            W65 = new D<T>(65, Automata);
            W66 = new D<T>(66, Automata);
            W67 = new D<T>(67, Automata);
            W68 = new D<T>(68, Automata);
            W69 = new D<T>(69, Automata);
            W70 = new D<T>(70, Automata);
            W71 = new D<T>(71, Automata);
            W72 = new D<T>(72, Automata);
            W73 = new D<T>(73, Automata);
            W74 = new D<T>(74, Automata);
            W75 = new D<T>(75, Automata);
            W76 = new D<T>(76, Automata);
            W77 = new D<T>(77, Automata);
            W78 = new D<T>(78, Automata);
            W79 = new D<T>(79, Automata);
            W80 = new D<T>(80, Automata);
            W81 = new D<T>(81, Automata);
            W82 = new D<T>(82, Automata);
            W83 = new D<T>(83, Automata);
            W84 = new D<T>(84, Automata);
            W85 = new D<T>(85, Automata);
            W86 = new D<T>(86, Automata);
            W87 = new D<T>(87, Automata);
            W88 = new D<T>(88, Automata);
            W89 = new D<T>(89, Automata);
            W90 = new D<T>(90, Automata);
            W91 = new D<T>(91, Automata);
            W92 = new D<T>(92, Automata);
            W93 = new D<T>(93, Automata);
            W94 = new D<T>(94, Automata);
            W95 = new D<T>(95, Automata);
            W96 = new D<T>(96, Automata);
            W97 = new D<T>(97, Automata);
            W98 = new D<T>(98, Automata);
            W99 = new D<T>(99, Automata);
            W100 = new D<T>(100, Automata);

            D2 = new D<T>(2, Automata);
            D3 = new D<T>(3, Automata);
            D4 = new D<T>(4, Automata);
            D5 = new D<T>(5, Automata);
            D6 = new D<T>(6, Automata);
            D7 = new D<T>(7, Automata);
            D8 = new D<T>(8, Automata);
            D9 = new D<T>(9, Automata);
            D10 = new D<T>(10, Automata);
            D11 = new D<T>(11, Automata);
            D12 = new D<T>(12, Automata);
            D13 = new D<T>(13, Automata);
            D14 = new D<T>(14, Automata);
            D15 = new D<T>(15, Automata);
            D16 = new D<T>(16, Automata);
            D17 = new D<T>(17, Automata);
            D18 = new D<T>(18, Automata);
            D19 = new D<T>(19, Automata);
            D20 = new D<T>(20, Automata);
            D21 = new D<T>(21, Automata);
            D22 = new D<T>(22, Automata);
            D23 = new D<T>(23, Automata);
            D24 = new D<T>(24, Automata);
            D25 = new D<T>(25, Automata);
            D26 = new D<T>(26, Automata);
            D27 = new D<T>(27, Automata);
            D28 = new D<T>(28, Automata);
            D29 = new D<T>(29, Automata);
            D30 = new D<T>(30, Automata);
            D31 = new D<T>(31, Automata);
            D32 = new D<T>(32, Automata);
            D33 = new D<T>(33, Automata);
            D34 = new D<T>(34, Automata);
            D35 = new D<T>(35, Automata);
            D36 = new D<T>(36, Automata);
            D37 = new D<T>(37, Automata);
            D38 = new D<T>(38, Automata);
            D39 = new D<T>(39, Automata);
            D40 = new D<T>(40, Automata);
            D41 = new D<T>(41, Automata);
            D42 = new D<T>(42, Automata);
            D43 = new D<T>(43, Automata);
            D44 = new D<T>(44, Automata);
            D45 = new D<T>(45, Automata);
            D46 = new D<T>(46, Automata);
            D47 = new D<T>(47, Automata);
            D48 = new D<T>(48, Automata);
            D49 = new D<T>(49, Automata);
            D50 = new D<T>(50, Automata);
            D51 = new D<T>(51, Automata);
            D52 = new D<T>(52, Automata);
            D53 = new D<T>(53, Automata);
            D54 = new D<T>(54, Automata);
            D55 = new D<T>(55, Automata);
            D56 = new D<T>(56, Automata);
            D57 = new D<T>(57, Automata);
            D58 = new D<T>(58, Automata);
            D59 = new D<T>(59, Automata);
            D60 = new D<T>(60, Automata);
            D61 = new D<T>(61, Automata);
            D62 = new D<T>(62, Automata);
            D63 = new D<T>(63, Automata);
            D64 = new D<T>(64, Automata);
            D65 = new D<T>(65, Automata);
            D66 = new D<T>(66, Automata);
            D67 = new D<T>(67, Automata);
            D68 = new D<T>(68, Automata);
            D69 = new D<T>(69, Automata);
            D70 = new D<T>(70, Automata);
            D71 = new D<T>(71, Automata);
            D72 = new D<T>(72, Automata);
            D73 = new D<T>(73, Automata);
            D74 = new D<T>(74, Automata);
            D75 = new D<T>(75, Automata);
            D76 = new D<T>(76, Automata);
            D77 = new D<T>(77, Automata);
            D78 = new D<T>(78, Automata);
            D79 = new D<T>(79, Automata);
            D80 = new D<T>(80, Automata);
            D81 = new D<T>(81, Automata);
            D82 = new D<T>(82, Automata);
            D83 = new D<T>(83, Automata);
            D84 = new D<T>(84, Automata);
            D85 = new D<T>(85, Automata);
            D86 = new D<T>(86, Automata);
            D87 = new D<T>(87, Automata);
            D88 = new D<T>(88, Automata);
            D89 = new D<T>(89, Automata);
            D90 = new D<T>(90, Automata);
            D91 = new D<T>(91, Automata);
            D92 = new D<T>(92, Automata);
            D93 = new D<T>(93, Automata);
            D94 = new D<T>(94, Automata);
            D95 = new D<T>(95, Automata);
            D96 = new D<T>(96, Automata);
            D97 = new D<T>(97, Automata);
            D98 = new D<T>(98, Automata);
            D99 = new D<T>(99, Automata);
            D100 = new D<T>(100, Automata);

        }
    }
}
