using ImageCombiner.Core.Interfaces;
using ImageCombiner.Core.Models;

namespace ImageCombiner.Core.Math;

public class VerticalCombinationStrategy : ICombinationStrategy
{
    public CombinerContext Context { get; }
    public ImagePoint BasePoint { get; }
    public (int Width, int Height) ResultImageDimensions => _resultDimensions.Value;
    private int FrameOffset => Context.Input.FramingOptions?.Thickness ?? 0;
    
    private readonly Lazy<(int Width, int Height)> _resultDimensions;
    
    public VerticalCombinationStrategy(CombinerContext ctx, ImagePoint? basePoint = null)
    {
        Context = ctx;
        BasePoint = basePoint ?? ImagePoint.Zero;
        _resultDimensions = new Lazy<(int Width, int Height)>(CalculateResultDimensions);
    }
    
    public void ApplyFrameOffset()
    {
        // Add frame offset
        BasePoint.X += FrameOffset;
        BasePoint.Y += FrameOffset;
    }

    public void ApplyImageOffset(Image image)
    {
        // Roll back X frame offset so we are back at zero
        BasePoint.X -= FrameOffset;
        
        // Add image height (don't add frame offset again because of single divider implementation)
        BasePoint.Y += image.Height;
    }

    private (int Width, int Height) CalculateResultDimensions()
    {
        var imagesMetadata = Context.ImagesMetadata;
        var numberOfImages = imagesMetadata.Length;
        var maxWidth = imagesMetadata.Max(meta => meta.Width);
        var combinedHeight = imagesMetadata.Sum(meta => meta.Height);
        
        // Only 2 border frames
        var totalHorizontalFrameOffset = 2 * FrameOffset;
        
        // Single dividers implied, x 2 if changed to double divider frames
        var verticalDividerCount = numberOfImages - 1;
        // 2 border frames (top and bottom) + divider frames
        var totalVerticalFrameOffset = (2 + verticalDividerCount) * FrameOffset;

        return (maxWidth + totalHorizontalFrameOffset, combinedHeight + totalVerticalFrameOffset);
    }
}