using model;
using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;

Console.CursorVisible = false;
Console.WriteLine("Starting sim generation...");

const string outDir = "results";
const int width = 1920;
const int height = 1080;
const int fps = 20;
const int length = 10;
const int agents = 1000;

const int steps = fps * length;

if (Directory.Exists(outDir))
{
    Directory.Delete(outDir, true);
}
Directory.CreateDirectory(outDir);

SlimeMould slime = new SlimeMould(width, height, agents);

List<string> files = new List<string>();
PixelFormat format = PixelFormat.Undefined;
for (int i = 0; i < steps; i++)
{
    Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
    Console.Write("Stepping (" + i.ToString() + "/" + steps + ")");
    slime.step();

    Bitmap bitmap = new Bitmap(width, height);
    MonoGrid state = slime.getState();

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            int value = (int)(2.55f * state.getValue(x, y));
            bitmap.SetPixel(x, y, Color.FromArgb(255, value, value, value));
        }
    }
    format = bitmap.PixelFormat;
    bitmap.Save(outDir + "/" + i.ToString() + ".bmp");
    files.Add(Environment.CurrentDirectory + @"\" + outDir + @"\" + i.ToString() + ".bmp");
    bitmap.Dispose();
}


FFmpegLoader.FFmpegPath =
    Environment.CurrentDirectory + @"\ffmpeg\ffmpeg-n6.0-34-g3d5edb89e7-win64-gpl-shared-6.0\bin";

var settings = new VideoEncoderSettings(width: width, height: height, framerate: fps, codec: VideoCodec.H264);
settings.EncoderPreset = EncoderPreset.Fast;
settings.CRF = 20;
var file = MediaBuilder.CreateContainer(Environment.CurrentDirectory + @"\out.mp4").WithVideo(settings).Create();
foreach (var inputFile in files)
{
    var binInputFile = File.ReadAllBytes(inputFile);
    var memInput = new MemoryStream(binInputFile);
    var bitmap = Image.FromStream(memInput) as Bitmap;
    var rect = new Rectangle(Point.Empty, bitmap.Size);
    var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, format);
    var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Rgba32, bitmap.Size);
    file.Video.AddFrame(bitmapData); // Encode the frame
    bitmap.UnlockBits(bitLock);
}

file.Dispose();

Console.WriteLine("\nComplete.");