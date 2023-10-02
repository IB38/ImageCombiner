using ImageCombiner.Core.Models;

namespace ImageCombiner.Core.Extensions;

public static class FrameColorExtensions
{
    public static Color ToImageSharpColor(this FrameColor color)
    {
        return color switch
        {
            FrameColor.Black => Color.Black,
            FrameColor.White => Color.White,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, $"Unexpected FrameColor {color}")
        };
    }
}