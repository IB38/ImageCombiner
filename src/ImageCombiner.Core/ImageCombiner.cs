using CodeJam;
using CodeJam.Threading;
using ImageCombiner.Core.Models;

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

        using var outputImage = new Image<Rgba32>(maxWidth, combinedHeight);

        var heightOffset = 0;
        foreach (var imgStream in inputStreams)
        {
            if (imgStream.CanSeek)
                imgStream.Seek(0, SeekOrigin.Begin);
            
            using var img = await Image.LoadAsync(imgStream, ct);
            var (width, height) = (img.Width, img.Height);
            
            outputImage.Mutate(o => o.DrawImage(img, new Point(0, heightOffset), 1f));
            heightOffset += height;
        }
        
        await outputImage.SaveAsJpegAsync(outputStream, ct);
    }
}