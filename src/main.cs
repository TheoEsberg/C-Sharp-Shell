using System.Net;
using System.Net.Sockets;

while (true)
{
    Console.Write("$ ");
    var command = Console.ReadLine();

    if (command!.Split(" ")[0] == "exit")
    {
        Environment.Exit(int.Parse(command.Split(" ")[1]));
    }

    Console.WriteLine($"{command}: command not found");
}

