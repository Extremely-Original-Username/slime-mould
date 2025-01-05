﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;

namespace model.rendering
{
    public class SlimeMouldRenderer : ISlimeMouldRenderer
    {
        private int steps;
        private int fps;
        private SlimeMould slime;
        private PixelFormat format;
        private List<string> files;
        private string outDir;

        public SlimeMouldRenderer(int steps, int fps, SlimeMould slimeMould, string outDir)
        {
            this.steps = steps;
            this.fps = fps;
            this.slime = slimeMould;
            this.outDir = outDir;

            this.files = new List<string>();
            this.format = PixelFormat.Undefined;
        }

        public void generateFrames(Action<int> onStep, Action onComplete)
        {
            for (int i = 0; i < steps; i++)
            {
                onStep.Invoke(i);
                slime.step();

                Bitmap bitmap = new Bitmap(slime.Width, slime.Height);
                MonoGrid state = slime.getState();

                for (int y = 0; y < slime.Height; y++)
                {
                    for (int x = 0; x < slime.Width; x++)
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
            onComplete.Invoke();
        }

        public void saveVideo(Action onSave)
        {
            FFmpegLoader.FFmpegPath =
                Environment.CurrentDirectory + @"\ffmpeg\ffmpeg-n6.0-34-g3d5edb89e7-win64-gpl-shared-6.0\bin";

            var settings = new VideoEncoderSettings(width: slime.Width, height: slime.Height, framerate: fps, codec: VideoCodec.H264);
            settings.EncoderPreset = EncoderPreset.Fast;
            settings.CRF = 20;

            onSave.Invoke();
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
        }
    }
}
