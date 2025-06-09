using System.Diagnostics.Tracing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;


var BUILTINS = new HashSet<string> 
{
    "exit", "echo", "type"
};

var PATHS = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(":");

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
        string commandType = command.Split(" ")[1].ToLower();

        if (BUILTINS.Contains(commandType))
        {
            Console.WriteLine($"{commandType} is a shell builtin");
            continue;
        }

        // Check if the command exists in the PATH environment variable
        bool isFound = false;
        foreach (var path in PATHS)
        {
            var fullPath = Path.Join(path, commandType);

            if (File.Exists(fullPath)) {
                Console.WriteLine($"{commandType} is {fullPath}");
                isFound = true;
                break;
            }
        }

        if (!isFound)
        {
            Console.WriteLine($"{commandType}: not found");
        }
    }

    else
    {
        Console.WriteLine($"{command}: command not found");
    }
}

