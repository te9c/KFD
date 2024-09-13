namespace KFD.Core;

public enum ExchangeStatus {
    OK = 0,
    InsufficientFundsExchanger = 1,
    InsufficientFundsUser = 2,
    InvalidPair = 3
};

public class Exchanger {
    public Dictionary<string, decimal> ExchangerBalance = new Dictionary<string, decimal> {
        {"RUB", 10000M},
        {"USD", 1000M},
        {"EUR", 1000M},
        {"USDT", 1000M},
        {"BTC", 1.5M},
    };
    public Dictionary<string, decimal> UserBalance = new Dictionary<string, decimal>(){
        {"RUB", 1000000M},
        {"USD", 0M},
        {"EUR", 0M},
        {"USDT", 0M},
        {"BTC", 0M},
    };

    public Rates Rates = new Rates();

    public ExchangeStatus Buy(string pairString, decimal amount) {
        foreach (var pair in Rates.Pairs) {
            if (pair.Key == pairString) {
                string[] currencies = pairString.Split('/');
                string baseCurrency = currencies[0];
                string quoteCurrency = currencies[1];

                decimal userExpense = amount * pair.Value;

                if (userExpense > UserBalance[quoteCurrency])
                    return ExchangeStatus.InsufficientFundsUser;

                if (amount > ExchangerBalance[baseCurrency])
                    return ExchangeStatus.InsufficientFundsExchanger;

                UserBalance[quoteCurrency] -= userExpense;
                UserBalance[baseCurrency] += amount;
                ExchangerBalance[baseCurrency] -= amount;
                ExchangerBalance[quoteCurrency] += userExpense;

                Rates.UpdateRate();
                return ExchangeStatus.OK;
            }
        }

        return ExchangeStatus.InvalidPair;
    }

    public ExchangeStatus Sell(string pairString, decimal amount) {
        foreach (var pair in Rates.Pairs) {
            if (pair.Key == pairString) {
                string[] currencies = pairString.Split('/');
                string baseCurrency = currencies[0];
                string quoteCurrency = currencies[1];

                decimal exchangerExpense = amount * pair.Value;

                if (amount > UserBalance[baseCurrency])
                    return ExchangeStatus.InsufficientFundsUser;

                if (exchangerExpense > ExchangerBalance[quoteCurrency])
                    return ExchangeStatus.InsufficientFundsExchanger;

                UserBalance[baseCurrency] -= amount;
                UserBalance[quoteCurrency] += exchangerExpense;
                ExchangerBalance[baseCurrency] += amount;
                ExchangerBalance[quoteCurrency] -= exchangerExpense;

                Rates.UpdateRate();
                return ExchangeStatus.OK;
            }
        }

        return ExchangeStatus.InvalidPair;
    }
}
