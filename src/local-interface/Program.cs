using model;
using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using model.rendering;
using model.slimeMould;

Console.CursorVisible = false;
Console.WriteLine("Starting sim generation...");

const string outDir = "results";
if (Directory.Exists(outDir))
{
    Directory.Delete(outDir, true);
}
Directory.CreateDirectory(outDir);

ISlimeMould slime = new GpuSlimeMould(SlimeMouldParams.GetDefault());
SlimeMouldRenderer renderer = new SlimeMouldRenderer(SlimeMouldrendererParams.GetDefault(), slime);

renderer.generateFrames((i) =>
{
    Console.Write("\nStepping (" + (i + 1).ToString() + "/" + renderer.Parameters.length * renderer.Parameters.fps + ")\n");
    Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 2);
},
() => Console.SetCursorPosition(0, Console.GetCursorPosition().Top + 3));
renderer.saveVideo(() =>
{
    Console.WriteLine("Saving video...");
});

Console.WriteLine("\nComplete.");