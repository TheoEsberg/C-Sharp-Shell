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
        //_paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();
        //_paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? Array.Empty<string>();
        _paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? [];
        _isInteractive = args.Length == 0;

        if (!_isInteractive)
        {
            // If arguments are provided, run the command directly
            RunCommand(string.Join(' ', args));
        } 
        else
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
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = location,
                Arguments = string.Join(' ', command[1..]),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();

            // Print stdout
            Console.Write(process.StandardOutput.ReadToEnd());
            Console.Error.Write(process.StandardError.ReadToEnd());

            process.WaitForExit();
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
        location = "";
        foreach (var path in _paths)
        {
            if (File.Exists($"{path}/{arguments}"))
            {
                location = path + "/" + arguments;
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