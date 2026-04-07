import java.io.*;
import java.net.URL;
import java.nio.channels.Channels;
import java.nio.channels.ReadableByteChannel;
import java.util.*;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class SP_lr8_play_song_and_download_image {
    private static final String SONG_PATH= "music/";
    private static final String IMAGE_PATH = "images/";
    private static ExecutorService executor = Executors.newFixedThreadPool(5);

    public static void main(String[] args) {
        DebugLogger.info("Приложение запущено");
        try {
            new File(SONG_PATH).mkdirs();
            new File(IMAGE_PATH).mkdirs();
            DebugLogger.debug("Директории созданы: " + SONG_PATH + ", " + IMAGE_PATH);
            
            Scanner scanner = new Scanner(System.in);
            while (true) {
                System.out.println("1 - Скачать и открыть картинку");
                System.out.println("2 - Скачать и включить музыку");
                System.out.println("3 - Музыка играет параллельно скачиванию картинки");
                String choice = scanner.nextLine();
                DebugLogger.debug("Пользователь выбрал пункт меню: " + choice);
                switch (choice) {
                    case "1": downloadAndOpenImage(scanner);break;
                    case "2": downloadAndPlaySong(scanner);break;
                    case "3": parallelSongAndImage(scanner);break;
                    default: 
                        DebugLogger.warn("Неверный выбор пользователя: " + choice);
                        System.out.println("Неверный выбор!");
                }
            }
        } catch (Exception e) {
            DebugLogger.error("Критическая ошибка приложения", e);
            System.err.println("Ошибка: " + e.getMessage());
        } finally {
            executor.shutdown();
            DebugLogger.info("Приложение остановлено");
        }
    }

    // Task 1: Download and open image
    private static void downloadAndOpenImage(Scanner scanner) {
        System.out.print("Введите URL картинки: ");
        String imageUrl = scanner.nextLine().trim();
        DebugLogger.info("Запрос на скачивание изображения: " + imageUrl);
        executor.submit(() -> {
            try {
                String fileName = IMAGE_PATH + "image_" + System.currentTimeMillis() + getFileExtension(imageUrl);
                DebugLogger.debug("Начало загрузки: " + imageUrl + " -> " + fileName);
                downloadFile(imageUrl, fileName);
                DebugLogger.info("Изображение скачано: " + new File(fileName).getName());
                System.out.println(" Картинка скачана: " + new File(fileName).getName());
                // Отображение скаченной картинки
                openImageFile(new File(fileName));
                DebugLogger.info("Изображение открыто: " + fileName);
                System.out.println(" Картинка открыта!");

            } catch (IOException e) {
                DebugLogger.error("Ошибка загрузки изображения: " + imageUrl, e);
                System.err.println(" Ошибка загрузки картинки: " + e.getMessage());
            }
        });
    }

    // Task 2: Download and play song
    private static void downloadAndPlaySong(Scanner scanner) {
        System.out.print("Введите URL музыки: ");
        String songUrl = scanner.nextLine().trim();
        DebugLogger.info("Запрос на скачивание музыки: " + songUrl);
        executor.submit(() -> {
            try {
                String fileName = SONG_PATH + "track_" + System.currentTimeMillis() + getFileExtension(songUrl);
                DebugLogger.debug("Начало загрузки: " + songUrl + " -> " + fileName);
                downloadFile(songUrl, fileName);
                DebugLogger.info("Музыка скачана: " + new File(fileName).getName());
                System.out.println(" Музыка скачана: " + new File(fileName).getName());
                // Воспроизведение музыки
                playWithSystemPlayer(new File(fileName));
                DebugLogger.info("Музыка запущена: " + fileName);
                System.out.println(" Музыка запущена!");
            } catch (IOException e) {
                DebugLogger.error("Ошибка загрузки музыки: " + songUrl, e);
                System.err.println(" Ошибка загрузки музыки: " + e.getMessage());
            }
        });
    }

    // Task 3: Parallel play song and download image
    private static void parallelSongAndImage(Scanner scanner) {
        System.out.print("Введите URL музыки: ");
        String songUrl = scanner.nextLine().trim();
        System.out.print("Введите URL картинки: ");
        String imageUrl = scanner.nextLine().trim();
        DebugLogger.info("Параллельная загрузка: музыка=" + songUrl + ", изображение=" + imageUrl);
        final String songFileName = SONG_PATH + "track_" + System.currentTimeMillis() + getFileExtension(songUrl);
        final String imageFileName = IMAGE_PATH + "image_" + System.currentTimeMillis() + getFileExtension(imageUrl);
        // Загрузка музыки и воспроизведение
        executor.submit(() -> {
            try {
                DebugLogger.debug("Начало загрузки музыки: " + songUrl + " -> " + songFileName);
                downloadFile(songUrl, songFileName);
                DebugLogger.info("Музыка скачана: " + new File(songFileName).getName());
                System.out.println("Музыка скачана: " + new File(songFileName).getName());
                // Воспроизведение музыки после загрузки
                playWithSystemPlayer(new File(songFileName));
                DebugLogger.info("Музыка запущена: " + songFileName);
                System.out.println("Музыка запущена!");
            } catch (IOException e) {
                DebugLogger.error("Ошибка загрузки музыки: " + songUrl, e);
                System.err.println("Ошибка загрузки музыки: " + e.getMessage());
            }
        });

        // Параллельная загрузка картинки
        executor.submit(() -> {
            try {
                DebugLogger.debug("Начало загрузки изображения: " + imageUrl + " -> " + imageFileName);
                downloadFile(imageUrl, imageFileName);
                DebugLogger.info("Картинка скачана: " + new File(imageFileName).getName());
                System.out.println("✓ Картинка скачана: " + new File(imageFileName).getName());
                openImageFile(new File(imageFileName));
                DebugLogger.info("Картинка открыта: " + imageFileName);
                System.out.println("Картинка открыта!");
            } catch (IOException e) {
                DebugLogger.error("Ошибка загрузки изображения: " + imageUrl, e);
                System.err.println(" Ошибка загрузки картинки: " + e.getMessage());
            }
        });

        DebugLogger.info("Параллельная загрузка запущена");
        System.out.println(" Параллельная загрузка запущена...");
    }

    private static String getFileExtension(String url) {
        DebugLogger.debug("Определение расширения файла для URL: " + url);
        if (url.contains(".mp3")) return ".mp3";
        if (url.contains(".wav")) return ".wav";
        if (url.contains(".png")) return ".png";
        if (url.contains(".jpeg")) return ".jpeg";
        if (url.contains(".gif")) return ".gif";
        return ".jpg";
    }

    private static void downloadFile(String fileUrl, String fileName) throws IOException {
        DebugLogger.debug("Скачивание файла: " + fileUrl);
        URL url = new URL(fileUrl);
        try (ReadableByteChannel byteChannel = Channels.newChannel(url.openStream());
             FileOutputStream stream = new FileOutputStream(fileName)) {
            stream.getChannel().transferFrom(byteChannel, 0, Long.MAX_VALUE);
            DebugLogger.debug("Файл успешно загружен: " + fileName);
        }
    }
    private static void playWithSystemPlayer(File musicFile) {
        DebugLogger.debug("Запуск системного плеера для: " + musicFile.getName());
        try {
            Runtime.getRuntime().exec(new String[]{"cmd", "/c", "start", "\"\"", "\"" + musicFile.getAbsolutePath() + "\""});
        } catch (Exception e) {
            DebugLogger.error("Ошибка запуска плеера: " + musicFile.getName(), e);
            System.err.println("Ошибка запуска плеера: " + e.getMessage());
        }
    }

    private static void openImageFile(File imageFile) {
        DebugLogger.debug("Открытие изображения: " + imageFile.getName());
        try {
            Runtime.getRuntime().exec(new String[]{"cmd", "/c", "start", "\"\"", "\"" + imageFile.getAbsolutePath() + "\""});
        } catch (Exception e) {
            DebugLogger.error("Ошибка открытия картинки: " + imageFile.getName(), e);
            System.err.println("Ошибка открытия картинки: " + e.getMessage());
        }
    }
}