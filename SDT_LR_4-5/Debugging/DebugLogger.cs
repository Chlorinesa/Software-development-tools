namespace ConsoleApp1.Debugging;

/// <summary>
/// Отладочный класс для логирования процесса расчёта закупки.
/// </summary>
public class DebugLogger
{
    private readonly List<string> _logs = new();
    private readonly bool _enabled;
    private readonly bool _writeToConsole;

    public DebugLogger(bool enabled = true, bool writeToConsole = false)
    {
        _enabled = enabled;
        _writeToConsole = writeToConsole;
    }

    public void Log(string message)
    {
        if (!_enabled)
            return;

        var logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
        _logs.Add(logEntry);
        
        if (_writeToConsole)
            Console.WriteLine(logEntry);
    }

    public void LogBatchInfo(string type, int price, int quantity, int index)
    {
        Log($"Партия #{index}: Тип={type}, Цена={price} руб./шт., Кол-во={quantity} шт., Общая стоимость={price * quantity} руб.");
    }

    public IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();

    public void Clear() => _logs.Clear();

    public string GetFullLog() => string.Join(Environment.NewLine, _logs);

    public void SaveToFile(string filePath)
    {
        File.WriteAllLines(filePath, _logs);
        Log($"Лог сохранён в файл: {filePath}");
    }
}
