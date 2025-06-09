using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

internal static class Program
{
    static List<DirectoryInfo> path = null!;

    public static void Main(string[] args)
    {
        path = [..Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries).Select(p => new DirectoryInfo(p))];

        while (true)
        {
            Console.Write("$ ");
            string? commandString = Console.ReadLine();
            Command command = new(commandString);
            command.CommandAction.Invoke();
        }
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
                case "cd":
                    CommandAction = () =>
                    {
                        if (Args.Length == 0)
                        {
                            Console.WriteLine(Environment.CurrentDirectory);
                            return;
                        }
                        string newPath = Args[0];
                        try
                        {
                            Directory.SetCurrentDirectory(newPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"cd: {ex.Message}");
                        }
                    };
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