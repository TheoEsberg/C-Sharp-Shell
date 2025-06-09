using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

internal static class Program
{
    private static string[] _paths = [];
    private static bool _isInteractive = false;

    public static void Main(string[] args)
    {
        _paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();

        if (args.Length > 0)
        {
            // Non-interactive mode: treat args as command
            var commandLine = string.Join(' ', args);
            RunCommand(commandLine);
        }
        else
        {
            // Interactive mode: start REPL (Read-Eval-Print Loop)
            while (true)
            {
                Repl();
            }
        }
    }

    private static void Repl()
    {
        if (_isInteractive)
            Console.Write("$ ");

        var userInput = Console.ReadLine();
        RunCommand(userInput);
    }

    private static void RunCommand(string? userInput)
    {
        if (string.IsNullOrEmpty(userInput))
        {
            return;
        }

        var command = userInput.Split(' ');
        var builtin = command[0];

        if (builtin == "exit")
            Exit(command);
        else if (builtin == "echo")
            Echo(command);
        else if (builtin == "type")
            Type(command);
        else if (ExecutableInPath(builtin, out var location))
            Process.Start(location, string.Join(' ', command[1..]));
        else
            Console.WriteLine($"{userInput}: command not found");
    }

    private static void Type(string[] command)
    {
        if (command.Length <= 1)
            return;

        var arguments = string.Join(' ', command[1..]);
        if (arguments == "echo")
            Console.WriteLine("echo is a shell builtin");
        else if (arguments == "exit")
            Console.WriteLine("exit is a shell builtin");
        else if (arguments == "type")
            Console.WriteLine("type is a shell builtin");
        else if (ExecutableInPath(arguments, out var location))
            Console.WriteLine($"{arguments} is {location}");
        else
            Console.WriteLine($"{arguments}: not found");
    }

    private static bool ExecutableInPath(string arguments, out string location)
    {
        location = string.Empty;
        foreach (var path in _paths)
        {
            var fullPath = Path.Combine(path, arguments);
            if (File.Exists(fullPath))
            {
                location = fullPath;
                return true;
            }
        }
        return false;
    }

    private static void Echo(string[] command)
    {
        if (command.Length > 1)
            Console.WriteLine(string.Join(' ', command[1..]));
    }

    private static void Exit(string[] command)
    {
        if (command.Length > 1 && int.TryParse(command[1], out var n) && n is >= 0 and <= 255)
            Environment.Exit(n);
        Environment.Exit(0); // Default exit code if not specified or invalid
    }
}

//using System.Diagnostics;
//using System.Diagnostics.Tracing;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.CompilerServices;

//var BUILTINS = new HashSet<string> 
//{
//    "exit", "echo", "type"
//};

//var PATHS = Environment.GetEnvironmentVariable("PATH")?.Split(":") ?? [];

//// REPL (Read-Eval-Print Loop) for a simple command line interface
//while (true)
//{
//    Console.Write("$ ");
//    string? command = Console.ReadLine();

//    if (string.IsNullOrEmpty(command))
//    {
//        continue;
//    }

//    // If user input is exit, we exit the program with a status code
//    else if (command.Split(" ")[0].ToLower() == "exit")
//    {
//        Environment.Exit(int.Parse(command.Split(" ")[1]));
//    }

//    // If user input is echo, we print the rest of the comman
//    else if (command.ToLower().StartsWith("echo") == true)
//    {
//        Console.WriteLine(command[5..]);
//    }

//    else if(command.ToLower().StartsWith("type") == true)
//    {
//        string commandType = command.Split(" ")[1].ToLower();

//        if (BUILTINS.Contains(commandType))
//        {
//            Console.WriteLine($"{commandType} is a shell builtin");
//            continue;
//        }

//        // Check if the command exists in the PATH environment variable
//        bool isFound = false;
//        foreach (var path in PATHS)
//        {
//            var fullPath = Path.Join(path, commandType);

//            if (File.Exists(fullPath)) {
//                Console.WriteLine($"{commandType} is {fullPath}");
//                isFound = true;
//                break;
//            }
//        }

//        if (!isFound)
//        {
//            Console.WriteLine($"{commandType}: not found");
//        }
//    }

//    else if (ExecutableInPath(BUILTINS, out var location))
//    {
//        Process.Start(location, string.Join(' ', command[1..]));
//    }

//    else
//    {
//        Console.WriteLine($"{command}: command not found");
//    }


//    static bool ExecutableInPath(string arguments, out string location)
//    {
//        location = string.Empty;
//        foreach (var path in PATHS)
//        {
//            if (File.Exists($"{path}/{arguments}"))
//            {
//                location = path + "/" + arguments;
//                return true;
//            }
//        }
//        return false;
//    }
//}
