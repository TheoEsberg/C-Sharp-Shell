using System.Net;
using System.Net.Sockets;

while (true)
{
    Console.Write("$ ");
    String? input = Console.ReadLine()?.Trim();

    //if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
    //{
    //    break;
    //}

    Console.WriteLine($"{input}: command not found");
}

