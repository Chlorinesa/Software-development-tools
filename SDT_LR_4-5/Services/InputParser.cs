




using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

/// <summary>
/// Парсер входных данных.
/// </summary>
public class InputParser
{
    /// <summary>
    /// Исключение для ошибок формата входных данных.
    /// </summary>
    public class InputFormatException : Exception
    {
        public InputFormatException(string message) : base(message) { }
        public InputFormatException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Исключение для ошибок в данных партий.
    /// </summary>
    public class BatchDataException : Exception
    {
        public int LineNumber { get; }

        public BatchDataException(string message, int lineNumber) : base(message)
        {
            LineNumber = lineNumber;
        }
    }

    /// <summary>
    /// Парсит строку входных данных и возвращает информацию о партиях.
    /// </summary>
    public (int batchCount, int budget, List<Batch> batches) ParseInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new InputFormatException("Входные данные не могут быть пустыми.");

        var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length < 1)
            throw new InputFormatException("Входные данные должны содержать как минимум одну строку с N и M.");

        var firstLineParts = lines[0].Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (firstLineParts.Length != 2)
            throw new InputFormatException($"Первая строка должна содержать два числа (N и M), получено: {lines[0]}");

        if (!int.TryParse(firstLineParts[0], out int batchCount) || batchCount <= 0)
            throw new InputFormatException($"Неверное количество партий: {firstLineParts[0]}");

        if (!int.TryParse(firstLineParts[1], out int budget) || budget <= 0)
            throw new InputFormatException($"Неверный бюджет: {firstLineParts[1]}");

        var batches = new List<Batch>();

        for (int i = 1; i <= batchCount && i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length != 3)
                throw new BatchDataException( $"Строка партии должна содержать: цену, количество и тип (A/B). Получено: {line}", i + 1);

            if (!int.TryParse(parts[0], out int price) || price <= 0)
                throw new BatchDataException($"Неверная цена в партии: {parts[0]}", i + 1);

            if (!int.TryParse(parts[1], out int quantity) || quantity <= 0)
                throw new BatchDataException($"Неверное количество в партии: {parts[1]}", i + 1);

            if (parts[2].Length != 1 || (parts[2][0] is not 'A' and not 'B'))
                throw new BatchDataException($"Неверный тип изделия: {parts[2]}. Должен быть 'A' или 'B'.", i + 1);

            batches.Add(new Batch(price, quantity, parts[2][0]));
        }

        if (batches.Count != batchCount)
            throw new InputFormatException($"Ожидалось {batchCount} партий, получено: {batches.Count}");

        return (batchCount, budget, batches);
    }

    /// <summary>
    /// Читает и парсит входные данные из файла
    /// </summary>
    public (int batchCount, int budget, List<Batch> batches) ParseInputFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        var content = File.ReadAllText(filePath);
        return ParseInput(content);
    }
}
