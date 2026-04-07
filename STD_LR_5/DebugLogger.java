import java.io.*;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.logging.*;

public class DebugLogger {
    
    private static final String LOG_FILE = "logs/app.log";
    private static final Logger LOGGER = Logger.getLogger("MediaDownloader");
    private static boolean initialized = false;
    
    public enum DebugLevel {
        DEBUG, INFO, WARNING, SEVERE
    }
    
    static {
        initializeLogger();
    }

    private static void initializeLogger() {
        try {
            LOGGER.setUseParentHandlers(false);
            LOGGER.setLevel(java.util.logging.Level.ALL);
            
            // Консольный обработчик
            ConsoleHandler consoleHandler = new ConsoleHandler();
            consoleHandler.setLevel(java.util.logging.Level.INFO);
            consoleHandler.setFormatter(new LogFormatter());
            LOGGER.addHandler(consoleHandler);
            
            // Файловый обработчик
            new File("logs").mkdirs();
            FileHandler fileHandler = new FileHandler(LOG_FILE, true);
            fileHandler.setLevel(java.util.logging.Level.ALL);
            fileHandler.setFormatter(new LogFormatter());
            LOGGER.addHandler(fileHandler);
            
            initialized = true;
            log(DebugLevel.INFO, "Логгер инициализирован. Логи записываются в: " + LOG_FILE);
        } catch (IOException e) {
            System.err.println("Ошибка инициализации логгера: " + e.getMessage());
        }
    }

    static class LogFormatter extends Formatter {
        private static final DateTimeFormatter DATE_FORMAT = 
                DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        
        @Override
        public String format(LogRecord record) {
            String timestamp = LocalDateTime.now().format(DATE_FORMAT);
            String level = record.getLevel().getName();
            String message = formatMessage(record);
            String sourceClass = record.getSourceClassName();
            String sourceMethod = record.getSourceMethodName();
            
            return String.format("[%s] %-7s [%s.%s] %s%n",
                    timestamp, level, sourceClass, sourceMethod, message);
        }
    }
    public static void log(DebugLevel level, String message) {
        if (!initialized) {
            System.err.println("Логгер не инициализирован");
            return;
        }
        
        switch (level) {
            case DEBUG:
                LOGGER.fine(message);
                break;
            case INFO:
                LOGGER.info(message);
                break;
            case WARNING:
                LOGGER.warning(message);
                break;
            case SEVERE:
                LOGGER.severe(message);
                break;
            default:
                LOGGER.info(message);
        }
    }

    public static void log(DebugLevel level, String message, Throwable thrown) {
        if (!initialized) {
            System.err.println("Логгер не инициализирован");
            return;
        }
        
        String fullMessage = message + " | Исключение: " + thrown.getMessage();
        switch (level) {
            case DEBUG:
                LOGGER.log(java.util.logging.Level.FINE, fullMessage, thrown);
                break;
            case INFO:
                LOGGER.log(java.util.logging.Level.INFO, fullMessage, thrown);
                break;
            case WARNING:
                LOGGER.log(java.util.logging.Level.WARNING, fullMessage, thrown);
                break;
            case SEVERE:
                LOGGER.log(java.util.logging.Level.SEVERE, fullMessage, thrown);
                break;
            default:
                LOGGER.log(java.util.logging.Level.INFO, fullMessage, thrown);
        }
    }

    public static void debug(String message) {
        log(DebugLevel.DEBUG, message);
    }
    public static void info(String message) {
        log(DebugLevel.INFO, message);
    }
    public static void warn(String message) {
        log(DebugLevel.WARNING, message);
    }
    public static void error(String message, Throwable thrown) {
        log(DebugLevel.SEVERE, message, thrown);
    }

}
