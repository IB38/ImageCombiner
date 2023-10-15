namespace ImageCombiner.Core.Models;

/// <summary>
/// Combiner behavior when input images have different resolution.
/// </summary>
public enum SizeMatchingType
{
    /// <summary>
    /// Combine as is
    /// </summary>
    Disabled = 0,
    
    /// <summary>
    /// Stretch smaller images to align with the biggest one
    /// </summary>
    Stretch = 1,
    
    /// <summary>
    /// Stretch bigger images to align with the smallest one
    /// </summary>
    Shrink = 2
}