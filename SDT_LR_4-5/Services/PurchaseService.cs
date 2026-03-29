using ConsoleApp1.Models;
using ConsoleApp1.Debugging;

namespace ConsoleApp1.Services;

/// <summary>
/// Сервис для расчёта оптимальной закупки изделий
/// </summary>
public class PurchaseService
{
    private readonly DebugLogger _debugLogger;

    public PurchaseService(DebugLogger debugLogger)
    {
        _debugLogger = debugLogger;
    }

    /// <summary>
    /// Рассчитывает количество закупленных изделий B и оставшуюся сумму денег
    /// </summary>
    public PurchaseResult CalculatePurchase(IEnumerable<Batch> batches, int budget)
    {
        if (batches == null || !batches.Any())
            throw new ArgumentException("Список партий не может быть пустым.");

        if (budget <= 0)
            throw new ArgumentException("Бюджет должен быть положительным.", nameof(budget));

        _debugLogger.Log($"Начало расчёта. Бюджет: {budget} руб.");

        var batchesList = batches.ToList();
        _debugLogger.Log($"Всего партий: {batchesList.Count}");

        var batchesA = batchesList.Where(b => b.ItemType == 'A').OrderBy(b => b.PricePerItem).ToList();
        var batchesB = batchesList.Where(b => b.ItemType == 'B').OrderByDescending(b => b.PricePerItem).ToList();

        _debugLogger.Log($"Партии A: {batchesA.Count}, Партии B: {batchesB.Count}");

        int remainingMoney = budget;
        int itemsBPurchased = 0;

        _debugLogger.Log("Закупка изделий A");
        foreach (var batch in batchesA)
        {
            int totalPrice = batch.PricePerItem * batch.Quantity;
            _debugLogger.Log($"Партия A: цена={batch.PricePerItem} руб/шт, кол-во={batch.Quantity}, всего за партию={totalPrice} руб.");
            if (remainingMoney >= totalPrice)
            {
                remainingMoney -= totalPrice;
                _debugLogger.Log($"  Куплена вся партия A за {totalPrice} руб. Остаток: {remainingMoney} руб.");
            }
            else
            {
                _debugLogger.Log($"  Недостаточно средств для покупки всей партии A (нужно {totalPrice} руб., осталось {remainingMoney} руб.)");
            }
        }

        _debugLogger.Log($"После закупки A осталось: {remainingMoney} руб.");

        _debugLogger.Log("Закупка изделий B");
        foreach (var batch in batchesB.OrderBy(b => b.PricePerItem))
        {
            _debugLogger.Log($"Партия B: цена={batch.PricePerItem} руб/шт, кол-во={batch.Quantity}");

            int canBuy = Math.Min(batch.Quantity, remainingMoney / batch.PricePerItem);
            int cost = canBuy * batch.PricePerItem;

            if (canBuy > 0)
            {
                itemsBPurchased += canBuy;
                remainingMoney -= cost;
                _debugLogger.Log($"  Куплено: {canBuy} шт. на сумму {cost} руб. Остаток: {remainingMoney} руб.");
            }
            else
            {
                _debugLogger.Log($"  Недостаточно средств для покупки из этой партии.");
            }
        }

        _debugLogger.Log($"ИТОГО: Изделий B = {itemsBPurchased}, Остаток = {remainingMoney} руб. ");

        return new PurchaseResult
        {
            ItemsBPurchased = itemsBPurchased,
            RemainingMoney = remainingMoney
        };
    }
}