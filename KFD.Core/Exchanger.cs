namespace KFD.Core;

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

    public ExchangeResult Buy(string pairString, decimal amount) {
        foreach (var pair in Rates.Pairs) {
            if (pair.Key == pairString) {
                string[] currencies = pairString.Split('/');
                string baseCurrency = currencies[0];
                string quoteCurrency = currencies[1];

                decimal userExpense = amount * pair.Value;

                if (userExpense > UserBalance[quoteCurrency])
                    return new ExchangeResult { Status = ExchangeStatus.InsufficientFundsUser };

                if (amount > ExchangerBalance[baseCurrency])
                    return new ExchangeResult { Status = ExchangeStatus.InsufficientFundsExchanger };

                UserBalance[quoteCurrency] -= userExpense;
                UserBalance[baseCurrency] += amount;
                ExchangerBalance[baseCurrency] -= amount;
                ExchangerBalance[quoteCurrency] += userExpense;

                ExchangeResult result = new ExchangeResult() {
                    Status = ExchangeStatus.OK,
                    PlusValue = amount,
                    PlusCurrency = baseCurrency,
                    MinusValue = userExpense,
                    MinusCurrency = quoteCurrency,
                };

                var rateCopy = Rates.Pairs.ToDictionary(entry => entry.Key,
                                                        entry => entry.Value);

                Rates.UpdateRates();
                foreach (var entry in rateCopy) {
                    result.RatesDiff[entry.Key] = Rates.Pairs[entry.Key] - entry.Value;
                }
                return result;
            }
        }

        return new ExchangeResult { Status = ExchangeStatus.InvalidPair };
    }

    public ExchangeResult Sell(string pairString, decimal amount) {
        foreach (var pair in Rates.Pairs) {
            if (pair.Key == pairString) {
                string[] currencies = pairString.Split('/');
                string baseCurrency = currencies[0];
                string quoteCurrency = currencies[1];

                decimal exchangerExpense = amount * pair.Value;

                if (amount > UserBalance[baseCurrency])
                    return new ExchangeResult { Status = ExchangeStatus.InsufficientFundsUser };

                if (exchangerExpense > ExchangerBalance[quoteCurrency])
                    return new ExchangeResult { Status = ExchangeStatus.InsufficientFundsExchanger };

                UserBalance[baseCurrency] -= amount;
                UserBalance[quoteCurrency] += exchangerExpense;
                ExchangerBalance[baseCurrency] += amount;
                ExchangerBalance[quoteCurrency] -= exchangerExpense;

                ExchangeResult result = new ExchangeResult() {
                    Status = ExchangeStatus.OK,
                    PlusValue = exchangerExpense,
                    PlusCurrency = quoteCurrency,
                    MinusValue = amount,
                    MinusCurrency = baseCurrency
                };

                var rateCopy = Rates.Pairs.ToDictionary(entry => entry.Key,
                                                        entry => entry.Value);

                Rates.UpdateRates();
                foreach (var entry in rateCopy) {
                    result.RatesDiff[entry.Key] = Rates.Pairs[entry.Key] - entry.Value;
                }
                return result;
            }
        }

        return new ExchangeResult { Status = ExchangeStatus.InvalidPair };
    }
}
