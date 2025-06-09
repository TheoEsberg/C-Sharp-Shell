using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

internal static class Program
{
    private static string[] _paths = [];
    static List<DirectoryInfo> path = null!;
    private static bool _isInteractive = false;

    public static void Main(string[] args)
    {
        path = [..Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries).Select(p => new DirectoryInfo(p))];
        _isInteractive = args.Length == 0;

        while (true)
        {
            Console.Write("$ ");
            string? commandString = Console.ReadLine();
            Command command = new(commandString);
            command.CommandAction.Invoke();
        }

        //if (!_isInteractive)
        //{
        //    // If arguments are provided, run the command directly
        //    RunCommand(string.Join(' ', args));
        //} 
        //else
        //{
        //    // start REPL (Read-Eval-Print Loop)
        //    while (true)
        //    {
        //        Repl();
        //    }
        //}
    }

    class Command
    {
        public string Name { get; }
        public string[] Args { get; }
        public string CombinedArgs { get => string.Join(' ', Args); }
        public string Type { get; } = "a shell builtin";

        public Action CommandAction { get; }

        public Command(string? commandString)
        {
            var splitString = commandString!.Split();
            Name = splitString[0];
            Args = splitString.Length > 1 ? splitString[1..] : [];

            switch (Name)
            {
                case "exit":
                    CommandAction = () =>
                    {
                        if (Args.Length > 0 && int.TryParse(Args[0], out var n) && n is >= 0 and <= 255)
                            Environment.Exit(n);
                        Environment.Exit(0); // Default exit code if not specified or invalid
                    };
                    return;
                case "echo":
                    CommandAction = () => Console.WriteLine(CombinedArgs);
                    return;
                case "type":
                    CommandAction = () =>
                    {
                        Command typedCommand = new(Args[0]);
                        if (typedCommand.Type == "invalid")
                            Console.WriteLine($"{typedCommand.Name}: not found");
                        else
                            Console.WriteLine($"{typedCommand.Name} is {typedCommand.Type}");
                    };
                    return;
                case "pwd":
                    CommandAction = () => Console.WriteLine(Environment.CurrentDirectory);
                    return;
                default:
                    foreach (var dir in path)
                    {
                        string path = Path.Combine(dir.FullName, Name);
                        if (File.Exists(path))
                        {
                            // Executable found in PATH
                            Type = path;
                            CommandAction = () =>
                            {
                                var process = Process.Start(new ProcessStartInfo(Name, Args));
                                process?.WaitForExit();
                            };
                            return;
                        }
                    }

                    CommandAction = () => Console.WriteLine($"{Name}: command not found");
                    Type = "invalid";
                    return;
            }
        }
    }
}

    //private static void Repl()
    //{
    //    if (_isInteractive)
    //        Console.Write("$ ");
            
    //    var userInput = Console.ReadLine();
    //    RunCommand(userInput);
    //}

    //private static void RunCommand(string? userInput)
    //{
    //    if (string.IsNullOrEmpty(userInput))
    //    {
    //        return;
    //    }

    //    var command = userInput.Split(' ');
    //    var builtin = command[0];

    //    if (builtin == "exit")
    //        Exit(command);
    //    else if (builtin == "echo")
    //        Echo(command);
    //    else if (builtin == "type")
    //        Type(command);
    //    else if (ExecutableInPath(builtin, out var location))
    //    {
    //        var startInfo = new ProcessStartInfo
    //        {
    //            FileName = command[0],
    //            Arguments = string.Join(' ', command[1..]),
    //            RedirectStandardOutput = true,
    //            RedirectStandardError = true,
    //            UseShellExecute = false,
    //            CreateNoWindow = true,
    //        };

    //        var process = new Process { StartInfo = startInfo };
    //        process.Start();

    //        // Print stdout
    //        Console.Write(process.StandardOutput.ReadToEnd());
    //        Console.Error.Write(process.StandardError.ReadToEnd());

    //        process.WaitForExit();
    //        //Process.Start(location, string.Join(' ', command[1..]));
    //    }
    //    else
    //        Console.WriteLine($"{userInput}: command not found");
    //}

    //private static void Type(string[] command)
    //{
    //    if (command.Length <= 1)
    //        return;

    //    var arguments = string.Join(' ', command[1..]);
    //    if (arguments == "echo")
    //        Console.WriteLine("echo is a shell builtin");
    //    else if (arguments == "exit")
    //        Console.WriteLine("exit is a shell builtin");
    //    else if (arguments == "type")
    //        Console.WriteLine("type is a shell builtin");
    //    else if (ExecutableInPath(arguments, out var location))
    //        Console.WriteLine($"{arguments} is {location}");
    //    else
    //        Console.WriteLine($"{arguments}: not found");
    //}

    //private static bool ExecutableInPath(string arguments, out string location)
    //{
    //    location = "";
    //    foreach (var path in _paths)
    //    {
    //        if (File.Exists($"{path}/{arguments}"))
    //        {
    //            location = path + "/" + arguments;
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //private static void Echo(string[] command)
    //{
    //    if (command.Length > 1)
    //        Console.WriteLine(string.Join(' ', command[1..]));
    //}

    //private static void Exit(string[] command)
    //{
    //    if (command.Length > 1 && int.TryParse(command[1], out var n) && n is >= 0 and <= 255)
    //        Environment.Exit(n);
    //    Environment.Exit(0); // Default exit code if not specified or invalid
    //}