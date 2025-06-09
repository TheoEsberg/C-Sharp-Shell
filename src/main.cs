using System.Net;
using System.Net.Sockets;

Console.Write("$ ");

// Wait for user input
while (true)
{
    var command = Console.ReadLine();
    Console.WriteLine($"{command}: command not found");
}

