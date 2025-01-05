using model;
using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using model.rendering;

Console.CursorVisible = false;
Console.WriteLine("Starting sim generation...");

const string outDir = "results";
const int width = 1920;
const int height = 1080;
const int fps = 20;
const int length = 1;
const int agents = 1000;

const int steps = fps * length;

if (Directory.Exists(outDir))
{
    Directory.Delete(outDir, true);
}
Directory.CreateDirectory(outDir);

SlimeMould slime = new SlimeMould(width, height, agents);
SlimeMouldRenderer renderer = new SlimeMouldRenderer(steps, fps, slime, outDir);

renderer.generateFrames((i) =>
{
    Console.Write("\nStepping (" + (i + 1).ToString() + "/" + steps + ")\n");
    Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 2);
},
() => Console.SetCursorPosition(0, Console.GetCursorPosition().Top + 3));
renderer.saveVideo(() =>
{
    Console.WriteLine("Saving video...");
});

Console.WriteLine("\nComplete.");