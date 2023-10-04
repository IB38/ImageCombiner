using CodeJam;

namespace ImageCombiner.Core.Models;

public class CombinerInput
{
    public IFileProvider FileProvider { get; }

    public CombinationType CombinationType { get; init; } = CombinationType.Vertical;

    public SizeMatchingStrategy SizeMatchingStrategy { get; init; } = SizeMatchingStrategy.Disabled;
    
    public FramingOptions? FramingOptions { get; init; } = null;

    public CombinerInput(IFileProvider fileProvider)
    {
        Code.NotNull(fileProvider, nameof(fileProvider));
        
        FileProvider = fileProvider;
    }
}