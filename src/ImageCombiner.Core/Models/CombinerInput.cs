﻿using CodeJam;

namespace ImageCombiner.Core.Models;

public class CombinerInput
{
    public IFileProvider FileProvider { get; }

    public CombinationType CombinationType { get; init; } = CombinationType.Vertical;

    public SizeMatchingType SizeMatchingType { get; init; } = SizeMatchingType.Disabled;
    
    public FramingOptions? FramingOptions { get; init; } = null;
    
    public int MaxResolution { get; init; }

    public int OutputJpegQuality { get; init; } = 95;

    public CombinerInput(IFileProvider fileProvider)
    {
        Code.NotNull(fileProvider, nameof(fileProvider));
        
        FileProvider = fileProvider;
    }
}