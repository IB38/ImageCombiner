using CodeJam;

namespace ImageCombiner.Core.Models;

public class CombinerInput
{
    public string[] InputImagePaths { get; }
    public string OutputImagePath { get; }

    public CombinationType CombinationType { get; init; } = CombinationType.Vertical;
    public FramingOptions? FramingOptions { get; init; } = null;

    public CombinerInput(string[] inputImagePaths, string outputImagePath)
    {
        Code.NotNullNorEmptyAndItemNotNull(inputImagePaths, nameof(inputImagePaths));
        Code.NotNullNorEmpty(outputImagePath, nameof(outputImagePath));
        
        InputImagePaths = inputImagePaths;
        OutputImagePath = outputImagePath;
    }
}