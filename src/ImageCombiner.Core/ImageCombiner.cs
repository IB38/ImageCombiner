using CodeJam;
using CodeJam.Threading;
using ImageCombiner.Core.Extensions;
using ImageCombiner.Core.Infrastructure;
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
        
        var ctx = new CombinerContext(input, imagesMetadata);
        var combinationStrategy = CombinationStrategyFactory.Build(ctx);
        var outputImgSize = combinationStrategy.ResultImageDimensions;
        
        using var outputImage = new Image<Rgba32>(outputImgSize.Width, outputImgSize.Height);
        
        foreach (var imgStream in inputStreams)
        {
            if (imgStream.CanSeek)
                imgStream.Seek(0, SeekOrigin.Begin);
            
            using var img = await Image.LoadAsync(imgStream, ct);
            var (width, height) = (img.Width, img.Height);
            
            outputImage.Mutate(
                o =>
                {
                    var framingOptions = input.FramingOptions;
                    var basePoint = combinationStrategy.BasePoint;

                    if (framingOptions != null)
                    {
                        var rect = new RectangleF(basePoint.X, basePoint.Y, 
                            width + 2 * framingOptions.Thickness, height + 2 * framingOptions.Thickness);
                        o.Fill(framingOptions.Color.ToImageSharpColor(), rect);
                        combinationStrategy.ApplyFrameOffset();
                    }

                    o.DrawImage(img, new Point(basePoint.X, basePoint.Y), 1f);
                    combinationStrategy.ApplyImageOffset(img);
                });
        }
        
        await outputImage.SaveAsJpegAsync(outputStream, ct);
    }
}