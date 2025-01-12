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

        public void generateFrames(Action<int> beforeStep, Action whileAwaitingCompletion, Action onComplete)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < steps; i++)
            {
                beforeStep.Invoke(i);
                slime.step();

                string filename = Parameters.outDir + "/" + i.ToString() + ".bmp";
                threads.Add(saveImage(slime.getState(), i, filename));
                files.Add(Environment.CurrentDirectory + @"\" + filename);
            }

            whileAwaitingCompletion.Invoke();
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            onComplete.Invoke();
        }

        private Thread saveImage(MonoGrid image, int index, string filename)
        {
            var result = new Thread(() =>
            {
                Bitmap bitmap = new Bitmap(slime.Parameters.width, slime.Parameters.height);

                for (int y = 0; y < slime.Parameters.height; y++)
                {
                    for (int x = 0; x < slime.Parameters.width; x++)
                    {
                        int value = image.getValue(x, y);
                        bitmap.SetPixel(x, y, Color.FromArgb(
                            interpolate(Parameters.background.A, Parameters.foreground.A, value), 
                            interpolate(Parameters.background.R, Parameters.foreground.R, value),
                            interpolate(Parameters.background.G, Parameters.foreground.G, value),
                            interpolate(Parameters.background.B, Parameters.foreground.B, value)
                            ));
                    }
                }
                format = bitmap.PixelFormat;
                bitmap.Save(filename);
                bitmap.Dispose();
            });

            result.Start();
            return result;
        }

        private int interpolate(int start, int end, int position)
        {
            return start + (end - start) / 100 * position;
        }

        public void saveVideo(Action onSave)
        {
            FFmpegLoader.FFmpegPath =
                Environment.CurrentDirectory + @"\ffmpeg\ffmpeg-n6.0.1-win64-gpl-shared-6.0\bin";

            var settings = new VideoEncoderSettings(width: slime.Parameters.width, height: slime.Parameters.height, framerate: Parameters.fps, codec: VideoCodec.H264);
            settings.EncoderPreset = EncoderPreset.Fast;
            settings.CRF = 20;

            onSave.Invoke();
            var file = MediaBuilder.CreateContainer(Environment.CurrentDirectory + "\\" + Parameters.outDir + @"\_out.mp4").WithVideo(settings).Create();
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
