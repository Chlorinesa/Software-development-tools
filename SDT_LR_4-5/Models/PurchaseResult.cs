namespace ConsoleApp1.Models;

/// <summary>
/// Результат закупки изделий.
/// </summary>
public class PurchaseResult
{
    public int ItemsBPurchased { get; init; }
    public int RemainingMoney { get; init; }

    public override string ToString() => 
        $"Изделий B: {ItemsBPurchased}, Остаток денег: {RemainingMoney} руб.";
}
