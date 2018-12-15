namespace Dice.States
{
    internal interface IWhileManager
    {
        (int count, WhileState state) this[int index] { get; }
    }
}