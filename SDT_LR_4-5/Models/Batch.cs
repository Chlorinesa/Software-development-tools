namespace ConsoleApp1.Models;

/// <summary>
/// Представляет партию изделий у поставщика.
/// </summary>
public class Batch
{
    public int PricePerItem { get; }
    public int Quantity { get; }
    public char ItemType { get; }

    public Batch(int pricePerItem, int quantity, char itemType)
    {
        if (pricePerItem <= 0)
            throw new ArgumentException("Цена изделия должна быть положительной.", nameof(pricePerItem));
        
        if (quantity <= 0)
            throw new ArgumentException("Количество изделий должно быть положительным.", nameof(quantity));
        
        if (itemType is not 'A' and not 'B')
            throw new ArgumentException("Тип изделия должен быть 'A' или 'B'.", nameof(itemType));

        PricePerItem = pricePerItem;
        Quantity = quantity;
        ItemType = itemType;
    }

    public int TotalCost => PricePerItem * Quantity;
}
