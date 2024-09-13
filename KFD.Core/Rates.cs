namespace KFD.Core;

public class Rates {

    public Dictionary<string, decimal> Pairs = new Dictionary<string, decimal>() {
        {"RUB/USD", 0.011M},
        {"RUB/EUR", 0.010M},
        {"USD/EUR", 0.9M},
        {"USD/USDT", 1.00M},
        {"USD/BTC", 0.000018M},
    };

    public const decimal MaxChangeratePercent = 0.05M;
    public const decimal MinChangeratePercent = -0.05M;

    public void UpdateRate() {
        foreach(var pair in Pairs) {
            decimal minValue = pair.Value * (1.00M + MinChangeratePercent);
            decimal maxValue = pair.Value * (1.00M + MaxChangeratePercent);
            Pairs[pair.Key] = (decimal)Random.Shared.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
