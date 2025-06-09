using System.Net;
using System.Net.Sockets;

while (true)
{
    Console.Write("$ ");
    String? input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input) || input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
    {
        Console.WriteLine(0);
        break;
    }

    Console.WriteLine($"{input}: command not found");
}

