using model;

Console.CursorVisible = false;
Console.WriteLine("Starting sim generation...");

SlimeMould slime = new SlimeMould(1000, 1000, 1000);

const int steps = 1000;
for (int i = 0; i < steps; i++)
{
    Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
    Console.Write("Stepping (" + i.ToString() + "/" + steps + ")");
    slime.step();
}

Console.WriteLine("\nComplete.");