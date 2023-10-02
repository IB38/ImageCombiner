using CodeJam;
using CodeJam.Threading;
using ImageCombiner.Core.Extensions;
using ImageCombiner.Core.Models;
using SixLabors.ImageSharp.Drawing.Processing;

namespace ImageCombiner.Core;

public class ImageCombiner
{
    public async Task CombineAsync(CombinerInput input, CancellationToken ct = default)
    {
        Code.NotNull(input, nameof(input));

        var fileProvider = input.FileProvider;
        var getInputStreams = fileProvider.GetInputStreamsAsync(ct);
        var getOutputStream = fileProvider.GetOutputStreamAsync(ct);
        
        using var inputStreams = await getInputStreams;
        Code.NotNullNorEmpty(inputStreams, nameof(inputStreams));
        await using var outputStream = await getOutputStream;
        Code.NotNull(outputStream, nameof(outputStream));
        
        var imagesMetadata = await inputStreams.Select(p => Image.IdentifyAsync(p, ct)).WhenAll();

        var maxWidth = imagesMetadata.Max(meta => meta.Width);
        var maxHeight = imagesMetadata.Max(meta => meta.Height);
        var combinedHeight = imagesMetadata.Sum(meta => meta.Height);
        var framingOptions = input.FramingOptions;
        var frameOffset = framingOptions?.Thickness ?? 0;

        using var outputImage = new Image<Rgba32>(maxWidth, combinedHeight);

        var heightOffset = 0;
        foreach (var imgStream in inputStreams)
        {
            if (imgStream.CanSeek)
                imgStream.Seek(0, SeekOrigin.Begin);
            
            using var img = await Image.LoadAsync(imgStream, ct);
            var (width, height) = (img.Width, img.Height);
            
            outputImage.Mutate(
                o =>
                    // TODO: Implement CombinationType
                    // TODO: Implement FramingOptions
                {
                    if (framingOptions != null && frameOffset > 0)
                    {
                        var rect = new RectangleF(0, heightOffset, width + 2 * frameOffset, height + 2 * frameOffset);
                        o.Fill(framingOptions.Color.ToImageSharpColor(), rect);
                        heightOffset += frameOffset;
                    }
                    o.DrawImage(img, new Point(frameOffset, heightOffset), 1f);
                });
            heightOffset += height + frameOffset;
        }
        
        await outputImage.SaveAsJpegAsync(outputStream, ct);
    }
}