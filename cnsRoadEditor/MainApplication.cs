using System;
using System.IO;
using System.Text; 

namespace cnsRoadEditor // Пространство имен для проекта
{
    // Главный класс приложения "Редактор Дорожной Карты".
    // Демонстрирует возможности класса RoadMapBase.
    class MainApplication
    {
        // Основной метод выполнения приложения.
        // args: Аргументы командной строки (в данном приложении не используются).
        static void Main(string[] args)
        {
            // Установка кодировки вывода консоли на UTF-8
            // для корректного отображения кириллических символов и псевдографики в сообщениях.
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Редактор Дорожной Карты");
            Console.WriteLine("------------------------------------------------------");

            // 1. Создание экземпляра основной дорожной карты.
            // Карта будет иметь размеры 35 ячеек в ширину и 20 в высоту.
            RoadMapBase mainRoadMap = new RoadMapBase(width: 35, height: 20);

            // 2. Отрисовка различных дорожных элементов на основной карте.
            // Используются методы DrawCross, DrawTShape, DrawLine, DrawRectangleOutline.

            // Отрисовка крестообразных перекрестков:
            // - Первый крест в точке (5,5) с длиной "рук" 3 ячейки.
            mainRoadMap.DrawCross(centerRow: 5, centerCol: 5, armLength: 3);
            // - Т-образный перекресток в (5,15) с ножкой вниз, длиной ножки 4, длиной перекладины 7.
            mainRoadMap.DrawTShape(junctionRow: 5, junctionCol: 15, legLength: 4, crossbarLength: 7, orientation: TShapeOrientation.LegDown);
            // - Т-образный перекресток в (15,5) с ножкой вправо, длиной ножки 3, длиной перекладины 5.
            mainRoadMap.DrawTShape(junctionRow: 15, junctionCol: 5, legLength: 3, crossbarLength: 5, orientation: TShapeOrientation.LegRight);
            // - Второй крест в точке (15,15) с длиной "рук" 2 ячейки.
            mainRoadMap.DrawCross(centerRow: 15, centerCol: 15, armLength: 2);

            // Отрисовка прямых дорожных линий, соединяющих части ранее нарисованных перекрестков:
            mainRoadMap.DrawLine(5, 8, 5, 11);  // Горизонтальная линия
            mainRoadMap.DrawLine(8, 5, 11, 5);  // Вертикальная линия
            mainRoadMap.DrawLine(9, 15, 12, 15); // Горизонтальная линия
            mainRoadMap.DrawLine(15, 8, 15, 12); // Горизонтальная линия

            // Отрисовка дополнительных структур в правой части карты:
            // - Контур прямоугольника (1,20) высотой 7, шириной 10.
            mainRoadMap.DrawRectangleOutline(1, 20, 7, 10);
            // - Горизонтальная линия.
            mainRoadMap.DrawLine(8, 25, 12, 25);
            // - Вертикальная линия, пересекающая предыдущие структуры.
            mainRoadMap.DrawLine(12, 20, 12, 30);
            // - Еще один крест.
            mainRoadMap.DrawCross(16, 28, 3);

            // 3. Отображение различных представлений основной карты в консоли.

            Console.WriteLine("\n--- Базовая карта---");
            // Вывод карты с использованием простых символов ('#' для дороги, '_' для пустого места) и рамки.
            mainRoadMap.PrintBasicMapWithBorder('#', '_');

            Console.WriteLine("\n--- Символьная карта (стандартная) ---");
            // Преобразование карты в представление с использованием символов псевдографики (─, │, ┌, и т.д.).
            char[,] symbolRepresentationMap = mainRoadMap.ConvertToSymbolMap();
            // Вывод символьной карты с заголовком и рамкой.
            RoadMapBase.PrintSymbolMapWithHeaderAndBorder(symbolRepresentationMap, "Символьная карта");

            Console.WriteLine("\n--- Детализированная символьная карта (3x3 блоки) ---");
            // Преобразование карты в "утолщенное" представление, где каждая ячейка отображается блоком символов 3x3.
            char[,] detailedSymbolMap = mainRoadMap.ConvertToDetailedSymbolMap();
            // Вывод детализированной символьной карты.
            RoadMapBase.PrintSymbolMapWithHeaderAndBorder(detailedSymbolMap, "Детализированная карта (3x3)");

            // 4. Сохранение основной карты в JSON-файл.
            // Формирование полного пути к файлу сохранения в директории запуска приложения.
            string mapDataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "my_road_map_data.json");
            Console.WriteLine($"\n--- Сохранение карты в: {mapDataFilePath} ---");
            mainRoadMap.SaveToFile(mapDataFilePath);

            // 5. Загрузка карты из ранее сохраненного JSON-файла.
            Console.WriteLine("\n--- Загрузка карты из файла ---");
            RoadMapBase loadedRoadMap = RoadMapBase.LoadFromFile(mapDataFilePath);
            // Проверка успешности загрузки.
            if (loadedRoadMap != null)
            {
                Console.WriteLine("Карта успешно загружена. Символьное представление загруженной карты:");
                // Вывод символьного представления загруженной карты для проверки.
                RoadMapBase.PrintSymbolMapWithHeaderAndBorder(loadedRoadMap.ConvertToSymbolMap(), "Загруженная символьная карта");
            }
            else
            {
                Console.WriteLine("Не удалось загрузить карту.");
            }

            // 6. Демонстрация функций автогенерации дорог.

            Console.WriteLine($"\n--- Авто-генерация: Сеточная карта ---");
            // Создание новой пустой карты для генерации сетки.
            RoadMapBase autoGeneratedGridMap = new RoadMapBase(18, 6);
            // Генерация сетки из 2 горизонтальных и 4 вертикальных дорог.
            autoGeneratedGridMap.AutoGenerateGridRoads(2, 4);
            // Вывод сгенерированной сеточной карты.
            RoadMapBase.PrintSymbolMapWithHeaderAndBorder(autoGeneratedGridMap.ConvertToSymbolMap(), "Авто-сеточная карта");

            Console.WriteLine($"\n--- Авто-генерация: Случайная карта ---");
            // Создание новой пустой карты для случайной генерации.
            RoadMapBase autoGeneratedRandomMap = new RoadMapBase(22, 8);
            // Генерация 12 случайных сегментов дорог, каждый длиной до 5 ячеек.
            autoGeneratedRandomMap.AutoGenerateRandomRoads(12, 5);
            // Вывод стандартного символьного представления случайной карты.
            RoadMapBase.PrintSymbolMapWithHeaderAndBorder(autoGeneratedRandomMap.ConvertToSymbolMap(), "Случайная карта");
            // Вывод детализированного (3x3) символьного представления случайной карты.
            char[,] detailedGeneratedRandomMap = autoGeneratedRandomMap.ConvertToDetailedSymbolMap();
            RoadMapBase.PrintSymbolMapWithHeaderAndBorder(detailedGeneratedRandomMap, "Детализированная случайная карта (3x3)");

            // 7. Демонстрация генерации изображений из карт (если доступен спрайт-лист).

            // Имя файла спрайт-листа, который должен находиться в директории запуска.
            string roadSpriteSheetFileName = "preview_22.jpg";
            // Формирование полного пути к файлу спрайт-листа для проверки его наличия.
            string fullSpriteSheetPathCheck = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, roadSpriteSheetFileName);

            // Проверка, существует ли файл спрайт-листа.
            if (File.Exists(fullSpriteSheetPathCheck))
            {
                Console.WriteLine($"\n--- Генерация изображения для ОСНОВНОЙ карты (спрайт-лист: {roadSpriteSheetFileName}) ---");
                // Путь для сохранения изображения основной карты.
                string mainMapImageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "main_road_map_image.png");
                // Генерация и сохранение изображения. Оператор '?' используется для безопасного вызова, если mainRoadMap вдруг null.
                mainRoadMap?.ConvertToImageSpriteMap(mainMapImageFilePath, roadSpriteSheetFileName);

                Console.WriteLine($"\n--- Генерация изображения для СЛУЧАЙНОЙ карты (спрайт-лист: {roadSpriteSheetFileName}) ---");
                // Путь для сохранения изображения случайно сгенерированной карты.
                string randomMapImageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "random_road_map_image.png");
                // Генерация и сохранение изображения.
                autoGeneratedRandomMap?.ConvertToImageSpriteMap(randomMapImageFilePath, roadSpriteSheetFileName);
            }
            else
            {
                // Информирование пользователя, если спрайт-лист отсутствует и генерация изображений пропускается.
                Console.WriteLine($"\nВНИМАНИЕ: Файл спрайт-листа '{roadSpriteSheetFileName}' не найден в директории: {AppDomain.CurrentDomain.BaseDirectory}");
                Console.WriteLine("Генерация изображений пропущена. Поместите файл спрайт-листа в указанную директорию для работы этой функции.");
            }

            // Завершение работы программы.
            Console.WriteLine("\nРабота программы завершена. Нажмите любую клавишу для выхода.");
            // Ожидание нажатия любой клавиши пользователем перед закрытием консольного окна.
            Console.ReadKey();
        }
    }
}