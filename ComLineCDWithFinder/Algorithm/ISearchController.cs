namespace ComLineCDWithFinder.Algorithm
{
    public interface ISearchController<T>
    {
        bool IsEnd { get; }
        void GetItem(T item);
        T[] FoundedItems { get; }
    }
}