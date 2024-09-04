namespace KFD.Shell;

class Program {

    public static void Main() {
        Command[] commands = {
            new Command("help", PrintHelp),
            new Command("exit", (string[] args) => { Environment.Exit(0); })
        };

        while (true) {
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (input == null) continue;

            string[] args = input.Split(' ',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!args.Any()) continue;

            var command = commands.Where(c => c.Name == args.First()).FirstOrDefault();
            if (command == null) {
                Console.WriteLine($"Unkown command {args.First()}!");
                continue;
            }

            command.Call(args);
        }
    }

    public static void PrintHelp(string[] args) {
        Console.WriteLine(@"Currency exchange (KFD test assignment)
Available commands:

balance - show your current balance
rates - show exchange rates and exchanger balance
exchange <Pair> <Ammount> - exchange first currency to second
help - show this message
exit - exit this shell");
    }

    public class Command {
        public string Name { get; init; }
        public delegate void CommandHandler(string[] args);
        public event CommandHandler? OnCall;

        public void Call(string[] args) {
            OnCall?.Invoke(args);
        }

        public Command(string name, CommandHandler commandHandler) {
            Name = name;
            OnCall += commandHandler;
        }
    }
}
