namespace Dice
{
    public interface IResultEntry<T>
    {
        double Propability { get; }
        T Result { get; }
    }
}