using ImageCombiner.Core.Interfaces;
using ImageCombiner.Core.Math;
using ImageCombiner.Core.Models;

namespace ImageCombiner.Core.Infrastructure;

public static class CombinationStrategyFactory
{
    public static ICombinationStrategy Build(CombinerContext ctx, ImagePoint? basePoint = null)
    {
        return ctx.Input.CombinationType switch
        {
            CombinationType.Vertical => new VerticalCombinationStrategy(ctx, basePoint),
            CombinationType.Horizontal => new HorizontalCombinationStrategy(ctx, basePoint),
            _ => throw new ArgumentOutOfRangeException(nameof(ctx.Input.CombinationType), ctx.Input.CombinationType,
                $"Unexpected {nameof(CombinationType)}")
        };
    }
}