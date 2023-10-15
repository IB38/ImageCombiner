using ImageCombiner.Core.Models;

namespace ImageCombiner.Core.Interfaces;

public interface ICombinationStrategy
{
    CombinerContext Context { get; }
    
    ImagePoint BasePoint { get; }
    (int Width, int Height) ResultImageDimensions { get; }

    void ApplyFrameOffset();
    void ApplyImageOffset(Image image);
}