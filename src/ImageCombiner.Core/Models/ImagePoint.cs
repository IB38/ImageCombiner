namespace ImageCombiner.Core.Models;

public class ImagePoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public static ImagePoint Zero => new ImagePoint();
}