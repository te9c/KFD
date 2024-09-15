using KFD.Core;

namespace KFD.Shell;

class Program {
    static Exchanger Exchanger = new Exchanger();

    public static void Main() {
        Command[] commands = {
            new Command("help", PrintHelp),
            new Command("exit", (string[] args) => { Environment.Exit(0); }),
            new Command("buy", Buy),
            new Command("sell", Sell),
            new Command("rates", PrintRates),
            new Command("balance", PrintBalance),
            new Command("clear", (args) => { Console.Clear(); })
        };

        while (true) {
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (input == null) {
                Console.WriteLine();
                continue;
            }

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

    private static void PrintHelp(string[] args) {
        Console.WriteLine(@"Currency exchange (KFD test assignment)
Available commands:

balance - show your and exchanger's current balance
rates - show exchange rates and exchanger balance
buy <Pair> <Amount> - buy specified pair
sell <Pair> <Amount> - sell specified pair
clear - clear terminal screen
help - show this message
exit - exit this shell");
    }

    private static void Buy(string[] args) {
        if (args.Count() != 3) {
            Console.WriteLine("buy takes 2 arguments!");
            return;
        }
        string pair = args[1].ToUpper();
        string amountStr = args[2];
        decimal amount = 0;

        if (!decimal.TryParse(amountStr, out amount)) {
            Console.WriteLine($"Unable to parse {amountStr}!");
            return;
        }

        var result = Exchanger.Buy(pair, amount);

        if (result.Status == ExchangeStatus.InvalidPair) {
            Console.WriteLine($"Invalid pair {pair}!");
            return;
        }
        if (result.Status == ExchangeStatus.InsufficientFundsExchanger) {
            Console.WriteLine($"Exchanger doesnt have sufficient amount of funds for this transaction!");
            return;
        }
        if (result.Status == ExchangeStatus.InsufficientFundsUser) {
            Console.WriteLine($"You dosnt have sufficient amount of funds for this transaction!");
            return;
        }

        Console.WriteLine(
@$"Successful transaction!
+{result.PlusValue} {result.PlusCurrency} 
-{result.MinusValue} {result.MinusCurrency}

Updated Rates:");

        foreach (var pairDiff in result.RatesDiff) {
            Console.Write($"{pairDiff.Key}: ");
            if (pairDiff.Value >= 0) {
                Console.Write('+');
            }
            Console.WriteLine(pairDiff.Value);
        }
    }

    private static void Sell(string[] args) {
        if (args.Count() != 3) {
            Console.WriteLine("sell takes 2 arguments!");
            return;
        }
        string pair = args[1].ToUpper();
        string amountStr = args[2];
        decimal amount = 0;

        if (!decimal.TryParse(amountStr, out amount)) {
            Console.WriteLine($"Unable to parse {amountStr}!");
            return;
        }

        var result = Exchanger.Sell(pair, amount);

        if (result.Status == ExchangeStatus.InvalidPair) {
            Console.WriteLine($"Invalid pair {pair}!");
            return;
        }
        if (result.Status == ExchangeStatus.InsufficientFundsExchanger) {
            Console.WriteLine($"Exchanger doesnt have sufficient amount of funds for this transaction!");
            return;
        }
        if (result.Status == ExchangeStatus.InsufficientFundsUser) {
            Console.WriteLine($"You dosnt have sufficient amount of funds for this transaction!");
            return;
        }

        Console.WriteLine(
@$"Successful transaction!
+{result.PlusValue} {result.PlusCurrency}
-{result.MinusValue} {result.MinusCurrency}

Updated Rates:");

        foreach (var pairDiff in result.RatesDiff) {
            Console.Write($"{pairDiff.Key}: ");
            if (pairDiff.Value >= 0) {
                Console.Write('+');
            }
            Console.WriteLine(pairDiff.Value);
        }
    }

    private static void PrintBalance(string[] args) {
        Console.WriteLine(@$"Your balance:
RUB  : {Exchanger.UserBalance["RUB"]}
USD  : {Exchanger.UserBalance["USD"]}
EUR  : {Exchanger.UserBalance["EUR"]}
USDT : {Exchanger.UserBalance["USDT"]}
BTC  : {Exchanger.UserBalance["BTC"]}

ExchangerBalance:
RUB  : {Exchanger.ExchangerBalance["RUB"]}
USD  : {Exchanger.ExchangerBalance["USD"]}
EUR  : {Exchanger.ExchangerBalance["EUR"]}
USDT : {Exchanger.ExchangerBalance["USDT"]}
BTC  : {Exchanger.ExchangerBalance["BTC"]}");
}

    private static void PrintRates(string[] args) {
        Console.WriteLine(@$"Current rates:
RUB/USD  : {Exchanger.Rates.Pairs["RUB/USD"]}
RUB/EUR  : {Exchanger.Rates.Pairs["RUB/EUR"]}
USD/EUR  : {Exchanger.Rates.Pairs["USD/EUR"]}
USD/USDT : {Exchanger.Rates.Pairs["USD/USDT"]}
USD/BTC  : {Exchanger.Rates.Pairs["USD/BTC"]}");
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
