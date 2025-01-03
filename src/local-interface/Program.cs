using model;
using System.Drawing;

Console.CursorVisible = false;
Console.WriteLine("Starting sim generation...");

const string outDir = "results";
const int width = 1920;
const int height = 1080;
const int fps = 30;
const int length = 20;
const int agents = 1000;

const int steps = fps * length;

if (Directory.Exists(outDir))
{
    Directory.Delete(outDir, true);
}
Directory.CreateDirectory(outDir);

SlimeMould slime = new SlimeMould(width, height, agents);

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
    bitmap.Save(outDir + "/" + i.ToString() + ".bmp");
    bitmap.Dispose();
}

Console.WriteLine("\nComplete.");