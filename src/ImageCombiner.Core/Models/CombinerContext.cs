namespace ImageCombiner.Core.Models;

public class CombinerContext
{
    public CombinerInput Input { get; }
    public ImageInfo[] ImagesMetadata { get; }

    public CombinerContext(CombinerInput input, ImageInfo[] imagesMetadata)
    {
        Input = input;
        ImagesMetadata = imagesMetadata;
    }
}