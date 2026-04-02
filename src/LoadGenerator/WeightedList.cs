namespace LoadGenerator;

internal sealed class WeightedList<T>
{
    private readonly T[] _items;
    private readonly int[] _cumulativeWeights;
    private readonly int _totalWeight;

    public WeightedList(IEnumerable<(T Item, int Weight)> weightedItems)
    {
        var items = new List<T>();
        var cumulative = new List<int>();
        var total = 0;

        foreach (var (item, weight) in weightedItems)
        {
            if (weight <= 0)
            {
                continue;
            }

            total += weight;
            items.Add(item);
            cumulative.Add(total);
        }

        _items = items.ToArray();
        _cumulativeWeights = cumulative.ToArray();
        _totalWeight = total;
    }

    public int Count => _items.Length;

    public T SelectRandom()
    {
        var value = Random.Shared.Next(_totalWeight);
        var index = Array.BinarySearch(_cumulativeWeights, value + 1);

        if (index < 0)
        {
            index = ~index;
        }

        return _items[index];
    }
}
