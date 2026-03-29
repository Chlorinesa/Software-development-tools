using ConsoleApp1.Debugging;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
namespace ConsoleApp1;

class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var debugLogger = new DebugLogger(enabled: true, writeToConsole: true);
        var parser = new InputParser();
        var purchaseService = new PurchaseService(debugLogger);

        try
        {
            string input;

            if (args.Length > 0 && File.Exists(args[0]))
            {
                Console.WriteLine($"Чтение данных из файла: {args[0]}");
                input = File.ReadAllText(args[0]);
            }
            else if (File.Exists("input.txt"))
            {
                Console.WriteLine("Чтение данных из файла: input.txt");
                input = File.ReadAllText("input.txt");
            }
            else
            {
                Console.WriteLine("Введите данные (N M, затем N строк с партиями):");
                input = Console.In.ReadToEnd();
            }

            Console.WriteLine();

            var (batchCount, budget, batches) = parser.ParseInput(input);

            debugLogger.Log($"Парсинг завершён");
            debugLogger.Log($"Партий: {batchCount}, Бюджет: {budget} руб.");

            foreach (var (batch, index) in batches.Select((b, i) => (b, i)))
            {
                debugLogger.LogBatchInfo(
                    batch.ItemType.ToString(),
                    batch.PricePerItem,
                    batch.Quantity,
                    index + 1);
            }

            Console.WriteLine();

            var result = purchaseService.CalculatePurchase(batches, budget);

            Console.WriteLine($"Изделий типа B закуплено: {result.ItemsBPurchased}");
            Console.WriteLine($"Остаток денег: {result.RemainingMoney} руб.");
            Console.WriteLine("Ответ: " + result.ItemsBPurchased + " " + result.RemainingMoney);
        }
        catch (InputParser.InputFormatException ex)
        {
            Console.WriteLine($"Ошибка формата входных данных: {ex.Message}");
            Environment.Exit(1);
        }
        catch (InputParser.BatchDataException ex)
        {
            Console.WriteLine($"Ошибка в данных партии (строка {ex.LineNumber}): {ex.Message}");
            Environment.Exit(1);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка аргумента: {ex.Message}");
            Environment.Exit(1);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Файл не найден: {ex.Message}");
            Environment.Exit(1);
        }
    }
}