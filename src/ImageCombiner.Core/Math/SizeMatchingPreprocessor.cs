using CodeJam.Threading;
using ImageCombiner.Core.Models;

namespace ImageCombiner.Core.Math;

public static class SizeMatchingPreprocessor
{
    public static async Task<ImageInfo[]> LoadMetadataAsync(IEnumerable<Stream> inputStreams, 
        CombinationType combinationType,
        SizeMatchingType sizeMatchingType, 
        CancellationToken ct = default)
    {
        var rawImageMetadata = await inputStreams.Select(s => Image.IdentifyAsync(s, ct)).WhenAll();
        var target = CalculateTargetDimensionValue(rawImageMetadata);

        return rawImageMetadata.Select(m => Resize(m, target)).ToArray();
        
        ImageInfo Resize(ImageInfo info, int targetDimensionValue)
        {
            if (targetDimensionValue == 0)
                return info;
            
            var normalizationRatio = GetNormalizationRatio(info, targetDimensionValue);
            var newWidth = (int)(info.Width * normalizationRatio);
            var newHeight = (int)(info.Height * normalizationRatio);

            return new ImageInfo(info.PixelType, 
                new Size(newWidth, newHeight), 
                info.Metadata,
                info.FrameMetadataCollection);
        }
        
        // Calculate target dimension according to SizeMatchingType
        int CalculateTargetDimensionValue(ImageInfo[] infos)
        {
            switch (sizeMatchingType)
            {
                case SizeMatchingType.Disabled:
                    return 0;
                case SizeMatchingType.Stretch:
                    return infos.Select(GetNormalizationDimension).Max();
                case SizeMatchingType.Shrink:
                    return infos.Select(GetNormalizationDimension).Min();;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sizeMatchingType), sizeMatchingType, $"Unexpected {nameof(SizeMatchingType)}");
            }
        }

        // Calculate normalization ratio (ratio for dimension multiplication for resizing)
        double GetNormalizationRatio(ImageInfo info, int targetDimensionValue)
        {
            return (double)targetDimensionValue / GetNormalizationDimension(info);
        }
        
        // Get normalization dimension value according to CombinationType
        int GetNormalizationDimension(ImageInfo info)
        {
            return combinationType switch
            {
                CombinationType.Vertical => info.Width,
                CombinationType.Horizontal => info.Height,
                _ => throw new ArgumentOutOfRangeException(nameof(combinationType), combinationType,
                    $"Unexpected {nameof(CombinationType)}")
            };
        }
    }
}