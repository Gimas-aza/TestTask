public interface IItem
{
    public string Name { get; }
    public int Price { get; }
    public int Number { get; }
    public void AcceptTrade();
    public void Cancel();
}
