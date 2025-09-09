using GreenDonut.Data;

namespace Demo.Reviews.Data;

internal static class ReviewOrdering
{
    public static SortDefinition<Review> DefaultOrder(
        SortDefinition<Review> sortDefinition)
        => sortDefinition
            .IfEmpty(p => p.AddDescending(t => t.CreateAt))
            .AddAscending(t => t.Id);
}