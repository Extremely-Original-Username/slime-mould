using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using model.slimeMould;

namespace model.rendering
{
    public class SlimeMouldRenderer : ISlimeMouldRenderer
    {
        private int steps;
        private ISlimeMould slime;
        private PixelFormat format;
        private List<string> files;
        public SlimeMouldrendererParams Parameters { get; }

        public SlimeMouldRenderer(SlimeMouldrendererParams parameters, ISlimeMould slimeMould)
        {
            this.Parameters = parameters;
            this.slime = slimeMould;
            this.steps = parameters.fps * parameters.length;

            this.files = new List<string>();
            this.format = PixelFormat.Undefined;
        }

        public void generateFrames(Action<int> onStep, Action onComplete)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < steps; i++)
            {
                onStep.Invoke(i);
                slime.step();

                threads.Add(saveImage(slime.getState(), i));
            }

            while (threads.Where((x) => x.IsAlive).Count() > 0)
            {
                Thread.Sleep(100);
            }
            onComplete.Invoke();
        }

        private Thread saveImage(MonoGrid image, int index)
        {
            var result = new Thread(() =>
            {
                Bitmap bitmap = new Bitmap(slime.Parameters.width, slime.Parameters.height);

                for (int y = 0; y < slime.Parameters.height; y++)
                {
                    for (int x = 0; x < slime.Parameters.width; x++)
                    {
                        int value = (int)(2.55f * image.getValue(x, y));
                        bitmap.SetPixel(x, y, Color.FromArgb(255, value, value, value));
                    }
                }
                format = bitmap.PixelFormat;
                bitmap.Save(Parameters.outDir + "/" + index.ToString() + ".bmp");
                files.Add(Environment.CurrentDirectory + @"\" + Parameters.outDir + @"\" + index.ToString() + ".bmp");
                bitmap.Dispose();
            });

            result.Start();
            return result;
        }

        public void saveVideo(Action onSave)
        {
            FFmpegLoader.FFmpegPath =
                Environment.CurrentDirectory + @"\ffmpeg\ffmpeg-n6.0.1-win64-gpl-shared-6.0\bin";

            var settings = new VideoEncoderSettings(width: slime.Parameters.width, height: slime.Parameters.height, framerate: Parameters.fps, codec: VideoCodec.H264);
            settings.EncoderPreset = EncoderPreset.Fast;
            settings.CRF = 20;

            onSave.Invoke();
            var file = MediaBuilder.CreateContainer(Environment.CurrentDirectory + "\\" + Parameters.outDir + @"\out.mp4").WithVideo(settings).Create();
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
        }
    }
}
