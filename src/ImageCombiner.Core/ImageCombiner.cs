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
        
        // TODO: Copy images to temp file, reading input stream twice doesn't work on WebAssembly 
        var imagesMetadata = await inputStreams.Select(p => Image.IdentifyAsync(p, ct)).WhenAll();

        // TODO: Implement CombinationType
        // TODO: Implement SizeMatchingStrategy
        var numberOfImages = inputStreams.Count;
        var maxWidth = imagesMetadata.Max(meta => meta.Width);
        var maxHeight = imagesMetadata.Max(meta => meta.Height);
        var combinedHeight = imagesMetadata.Sum(meta => meta.Height);
        var framingOptions = input.FramingOptions;
        var frameOffset = framingOptions?.Thickness ?? 0;

        var totalHorizontalFrameOffeset = 2 * frameOffset;
        // Single dividers implied, x 2 if changed to double divider frames
        var verticalDividerCount = numberOfImages - 1;
        
        // 2 edge frames (top and bottom) + divider frames
        var totalVerticalFrameOffset = (2 + verticalDividerCount) * frameOffset;
        using var outputImage = new Image<Rgba32>(maxWidth + totalHorizontalFrameOffeset, combinedHeight + totalVerticalFrameOffset);

        var heightOffset = 0;
        foreach (var imgStream in inputStreams)
        {
            if (imgStream.CanSeek)
                imgStream.Seek(0, SeekOrigin.Begin);
            
            using var img = await Image.LoadAsync(imgStream, ct);
            var (width, height) = (img.Width, img.Height);
            
            outputImage.Mutate(
                o =>
                {
                    if (framingOptions != null && frameOffset > 0)
                    {
                        var rect = new RectangleF(0, heightOffset, width + 2 * frameOffset, height + 2 * frameOffset);
                        o.Fill(framingOptions.Color.ToImageSharpColor(), rect);
                        heightOffset += frameOffset;
                    }
                    o.DrawImage(img, new Point(frameOffset, heightOffset), 1f);
                });
            // In-between frame doubles
            //heightOffset += height + frameOffset;
            
            // Skip adding frameOffset to have single height divider frame
            heightOffset += height;
        }
        
        await outputImage.SaveAsJpegAsync(outputStream, ct);
    }
}