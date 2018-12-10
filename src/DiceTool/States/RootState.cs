namespace Dice.States
{
    internal class RootState : State
    {
        private readonly IComposer composer;

        public RootState(IComposer composer) : base(null!)
        {
            this.composer = composer;
        }
        public override IComposer Composer => this.composer;

        public override int Depth => 0;

    }

}
