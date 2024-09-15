namespace KFD.Core;

public enum ExchangeStatus {
    OK = 0,
    InsufficientFundsExchanger = 1,
    InsufficientFundsUser = 2,
    InvalidPair = 3
};

public class ExchangeResult {
    public ExchangeStatus Status { get; init; }
    public decimal PlusValue { get; init; } = 0;
    public string PlusCurrency { get; init; } = "";
    public decimal MinusValue { get; init; } = 0;
    public string MinusCurrency { get; init; } = "";
    public Dictionary<string, decimal> RatesDiff { get; init; } = new Dictionary<string, decimal>();
}
