using System.Diagnostics.Tracing;
using System.Net;
using System.Net.Sockets;


List<string> Types = new List<string>()
{
    "exit", "echo", "type"
};

// REPL (Read-Eval-Print Loop) for a simple command line interface
while (true)
{
    Console.Write("$ ");
    string? command = Console.ReadLine();

    if (string.IsNullOrEmpty(command))
    {
        continue;
    }

    // If user input is exit, we exit the program with a status code
    else if (command.Split(" ")[0].ToLower() == "exit")
    {
        Environment.Exit(int.Parse(command.Split(" ")[1]));
    }

    // If user input is echo, we print the rest of the comman
    else if (command.ToLower().StartsWith("echo") == true)
    {
        Console.WriteLine(command[5..]);
    }

    else if(command.ToLower().StartsWith("type") == true)
    {
        string commandType = command.Split(" ")[1];
        if (Types.Contains(commandType.ToLower()))
        {
            Console.WriteLine($"{commandType} is a shell builtin");
        }
        else
        {
            Console.WriteLine($"{commandType}: not found");
        }
    }

    else {
        Console.WriteLine($"{command}: command not found");
    }
}

