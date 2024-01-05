using CodeJam;
using CodeJam.Collections;
using CodeJam.Threading;
using ImageCombiner.Core.Extensions;
using ImageCombiner.Core.Infrastructure;
using ImageCombiner.Core.Math;
using ImageCombiner.Core.Models;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

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
        
        // Preprocessor "virtually" resizes images by changing metadata used in calculations
        var imagesMetadata = await SizeMatchingPreprocessor.LoadMetadataAsync(inputStreams, 
            input.CombinationType, 
            input.SizeMatchingType, 
            ct);
        
        var ctx = new CombinerContext(input, imagesMetadata);
        var combinationStrategy = CombinationStrategyFactory.Build(ctx);
        var outputImgSize = combinationStrategy.ResultImageDimensions;
        
        using var outputImage = new Image<Rgba32>(outputImgSize.Width, outputImgSize.Height);
        
        foreach (var (i, imgStream) in inputStreams.WithIndex())
        {
            if (imgStream.CanSeek)
                imgStream.Seek(0, SeekOrigin.Begin);
            
            using var img = await Image.LoadAsync(imgStream, ct);
            // Get previously preprocessed metadata and do the real resizing
            var meta = imagesMetadata[i];
            img.Mutate(o => o.Resize(meta.Size));
            
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

        var maxRes = input.MaxResolution;
        if (maxRes > 0)
        {
            if(outputImage.Height > maxRes)
                outputImage.Mutate(o => o.Resize(0, maxRes));
            
            if(outputImage.Width > maxRes)
                outputImage.Mutate(o => o.Resize(maxRes, 0));
        }

        var encoder = new JpegEncoder() { Quality = input.OutputJpegQuality };
        await outputImage.SaveAsJpegAsync(outputStream, encoder, ct);
    }
}