using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

internal static class Program
{
    private static string[] _paths = [];

    public static void Main(string[] args)
    {
        //_paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();
        //_paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? Array.Empty<string>();
        _paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? [];
        var isInteractive = args.Length == 0;

        if (!isInteractive)
        {
            // If arguments are provided, run the command directly
            RunCommand(string.Join(' ', args));
        } else
        {
            // start REPL (Read-Eval-Print Loop)
            while (true)
            {
                Repl();
            }
        }
    }

    private static void Repl()
    {
        if (!Console.IsInputRedirected)
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
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = location,
                Arguments = string.Join(' ', command[1..]),
                RedirectStandardOutput = false,
                UseShellExecute = false,
                CreateNoWindow = false,
            };
            var process = new Process { StartInfo = startInfo };
            process.Start();
            //Process.Start(location, string.Join(' ', command[1..]));
        }
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