namespace ImageCombiner.Core.Models;

public class FramingOptions
{
    public int Thickness { get; }
    public FrameColor Color { get; }

    public FramingOptions(int thickness, FrameColor color)
    {
        Thickness = thickness;
        Color = color;
    }
}