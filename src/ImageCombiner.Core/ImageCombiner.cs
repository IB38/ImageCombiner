using CodeJam;
using CodeJam.Threading;
using ImageCombiner.Core.Models;

namespace ImageCombiner.Core;

public class ImageCombiner
{
    public async Task CombineAsync(CombinerInput input, CancellationToken ct = default)
    {
        Code.NotNull(input, nameof(input));

        var imagePaths = input.InputImagePaths;
        var imagesMetadata = await imagePaths.Select(p => Image.IdentifyAsync(p, ct)).WhenAll();

        var maxWidth = imagesMetadata.Max(meta => meta.Width);
        var maxHeight = imagesMetadata.Max(meta => meta.Height);
        var combinedHeight = imagesMetadata.Sum(meta => meta.Height);

        using var outputImage = new Image<Rgba32>(maxWidth, combinedHeight);

        var heightOffset = 0;
        foreach (var imgPath in imagePaths)
        {
            using var img = await Image.LoadAsync(imgPath, ct);
            var (width, height) = (img.Width, img.Height);
            
            outputImage.Mutate(o => o.DrawImage(img, new Point(0, heightOffset), 1f));
            heightOffset += height;
        }

        await outputImage.SaveAsJpegAsync(input.OutputImagePath, ct);
    }
}