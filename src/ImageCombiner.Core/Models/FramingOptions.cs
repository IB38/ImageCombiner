namespace ImageCombiner.Core.Models;

public class FramingOptions
{
    public float Thickness { get; }
    public FrameColor Color { get; }

    public FramingOptions(float thickness, FrameColor color)
    {
        Thickness = thickness;
        Color = color;
    }
}