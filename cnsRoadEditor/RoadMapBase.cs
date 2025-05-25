using System.Drawing; 
using System.Drawing.Imaging; 
using System.Text.Json; 

namespace cnsRoadEditor
{
    // Определяет ориентацию "ножки" Т-образного дорожного перекрестка.
    public enum TShapeOrientation
    {
        LegDown,    // Ножка Т-образного перекрестка направлена вниз.
        LegUp,      // Ножка Т-образного перекрестка направлена вверх.
        LegLeft,    // Ножка Т-образного перекрестка направлена влево.
        LegRight    // Ножка Т-образного перекрестка направлена вправо.
    }

    // Представляет базовую дорожную карту как 2D-сетку ячеек, где каждая ячейка может быть либо дорогой, либо пустой.
    // Предоставляет методы для отрисовки дорожных элементов, сохранения/загрузки и преобразования в различные представления.
    public class RoadMapBase
    {
        // Ширина карты в ячейках.
        public int MapWidth { get; private set; }

        // Высота карты в ячейках.
        public int MapHeight { get; private set; }

        // Внутреннее представление данных карты с использованием списка списков булевых значений.
        // True означает дорожную ячейку, false - пустую ячейку.
        private List<List<bool>> _roadCellData;

        // Свойство для получения или установки данных о дорожных ячейках с использованием невыровненного массива булевых значений.
        // Это свойство в основном предназначено для сериализации/десериализации JSON,
        // так как System.Text.Json обрабатывает массивы проще, чем List<List<bool>>.
        public bool[][] RoadCellDataForSerialization
        {
            get
            {
                if (_roadCellData == null) return new bool[0][];
                // Преобразование внутреннего List<List<bool>> в bool[][] для сериализации.
                return _roadCellData.Select(rowList => rowList.ToArray()).ToArray();
            }
            set
            {
                if (value == null)
                {
                    // Обработка null на входе созданием пустой карты.
                    MapHeight = 0; MapWidth = 0;
                    _roadCellData = new List<List<bool>>();
                    return;
                }

                MapHeight = value.Length;
                _roadCellData = new List<List<bool>>(MapHeight);

                if (MapHeight > 0)
                {
                    // Предполагаем, что все строки должны иметь ту же ширину, что и первая строка.
                    // Если value[0] равно null, ширина становится 0.
                    MapWidth = value[0]?.Length ?? 0;
                    for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
                    {
                        var newRowList = new List<bool>(MapWidth);
                        if (value[rowIndex] == null) // Обработка null-строк путем заполнения их false
                        {
                            for (int colIndex = 0; colIndex < MapWidth; colIndex++) newRowList.Add(false);
                        }
                        else
                        {
                            // Закомментированный блок для потенциальной проверки разной длины строк.
                            if (value[rowIndex].Length != MapWidth) { /* Потенциально логировать или выбрасывать ошибку */ }

                            // Заполнение строки, обеспечивая, что мы не выйдем за границы, если value[rowIndex] короче MapWidth.
                            for (int colIndex = 0; colIndex < MapWidth; colIndex++)
                            {
                                newRowList.Add(colIndex < value[rowIndex].Length && value[rowIndex][colIndex]);
                            }
                        }
                        _roadCellData.Add(newRowList);
                    }
                }
                else
                {
                    // Если высота равна 0, ширина также должна быть 0.
                    MapWidth = 0;
                }
            }
        }

        // Инициализирует новый экземпляр класса RoadMapBase с указанными размерами.
        // Все ячейки изначально установлены как пустые (false).
        // width: Ширина карты.
        // height: Высота карты.
        // ArgumentOutOfRangeException: Выбрасывается, если ширина или высота отрицательные.
        public RoadMapBase(int width, int height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentOutOfRangeException("Ширина и Высота карты не могут быть отрицательными.");

            MapWidth = width;
            MapHeight = height;
            _roadCellData = new List<List<bool>>(height);
            for (int i = 0; i < height; i++)
            {
                // Инициализация каждой строки 'width' количеством значений false (пустые ячейки).
                _roadCellData.Add(new List<bool>(new bool[width]));
            }
        }

        // Проверяет, находятся ли указанные координаты ячейки в пределах границ карты.
        // rowIndex: Индекс строки ячейки.
        // columnIndex: Индекс столбца ячейки.
        // Возвращает true, если ячейка действительна, иначе false.
        private bool IsValidCell(int rowIndex, int columnIndex)
        {
            return rowIndex >= 0 && rowIndex < MapHeight && columnIndex >= 0 && columnIndex < MapWidth;
        }

        // Получает состояние ячейки (дорога или пусто).
        // rowIndex: Индекс строки ячейки.
        // columnIndex: Индекс столбца ячейки.
        // Возвращает true, если ячейка является дорогой, false, если она пуста или находится за пределами карты.
        public bool GetCellState(int rowIndex, int columnIndex)
        {
            if (IsValidCell(rowIndex, columnIndex))
                return _roadCellData[rowIndex][columnIndex];
            return false; // Обработка выхода за границы как отсутствие дороги
        }

        // Устанавливает состояние ячейки.
        // rowIndex: Индекс строки ячейки.
        // columnIndex: Индекс столбца ячейки.
        // isRoad: True, чтобы установить ячейку как дорогу, false - как пустую.
        public void SetCellState(int rowIndex, int columnIndex, bool isRoad)
        {
            if (IsValidCell(rowIndex, columnIndex))
            {
                _roadCellData[rowIndex][columnIndex] = isRoad;
            }
        }

        // Очищает карту, устанавливая все ячейки как пустые.
        public void ClearMap()
        {
            for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < MapWidth; columnIndex++)
                {
                    _roadCellData[rowIndex][columnIndex] = false;
                }
            }
        }

        // Рисует линию из дорожных ячеек между двумя точками.
        // Поддерживает горизонтальные, вертикальные и диагональные линии (используя вариант алгоритма Брезенхэма).
        // startRow: Начальный индекс строки.
        // startColumn: Начальный индекс столбца.
        // endRow: Конечный индекс строки.
        // endColumn: Конечный индекс столбца.
        public void DrawLine(int startRow, int startColumn, int endRow, int endColumn)
        {
            // Обработка горизонтальных линий
            if (startRow == endRow)
            {
                for (int column = Math.Min(startColumn, endColumn); column <= Math.Max(startColumn, endColumn); column++)
                    SetCellState(startRow, column, true);
            }
            // Обработка вертикальных линий
            else if (startColumn == endColumn)
            {
                for (int row = Math.Min(startRow, endRow); row <= Math.Max(startRow, endRow); row++)
                    SetCellState(row, startColumn, true);
            }
            // Обработка диагональных линий (вариант алгоритма Брезенхэма)
            else
            {
                int deltaRow = Math.Abs(endRow - startRow);
                int deltaColumn = Math.Abs(endColumn - startColumn);
                int stepRow = (startRow < endRow) ? 1 : -1;
                int stepColumn = (startColumn < endColumn) ? 1 : -1;
                int error = deltaRow - deltaColumn;
                int currentRow = startRow;
                int currentColumn = startColumn;

                while (true)
                {
                    SetCellState(currentRow, currentColumn, true);
                    if (currentRow == endRow && currentColumn == endColumn) break; // Отрисовка линии завершена

                    int errorTimesTwo = 2 * error;
                    if (errorTimesTwo > -deltaColumn) { error -= deltaColumn; currentRow += stepRow; }
                    if (errorTimesTwo < deltaRow) { error += deltaRow; currentColumn += stepColumn; }
                }
            }
        }

        // Рисует контур прямоугольника из дорожных ячеек.
        // topLeftRow: Индекс строки верхнего левого угла.
        // topLeftColumn: Индекс столбца верхнего левого угла.
        // rectangleHeight: Высота прямоугольника.
        // rectangleWidth: Ширина прямоугольника.
        public void DrawRectangleOutline(int topLeftRow, int topLeftColumn, int rectangleHeight, int rectangleWidth)
        {
            if (rectangleHeight <= 0 || rectangleWidth <= 0) return; // Нечего рисовать для неположительных размеров

            int bottomRightRow = topLeftRow + rectangleHeight - 1;
            int bottomRightColumn = topLeftColumn + rectangleWidth - 1;

            // Отрисовка четырех сторон прямоугольника
            DrawLine(topLeftRow, topLeftColumn, topLeftRow, bottomRightColumn);       // Верхняя сторона
            DrawLine(bottomRightRow, topLeftColumn, bottomRightRow, bottomRightColumn); // Нижняя сторона
            DrawLine(topLeftRow, topLeftColumn, bottomRightRow, topLeftColumn);       // Левая сторона
            DrawLine(topLeftRow, bottomRightColumn, bottomRightRow, bottomRightColumn); // Правая сторона
        }

        // Заполняет прямоугольную область дорожными ячейками.
        // startRow: Индекс строки верхнего левого угла области для заполнения.
        // startColumn: Индекс столбца верхнего левого угла.
        // rectangleHeight: Высота области для заполнения.
        // rectangleWidth: Ширина области для заполнения.
        public void FillRectangleWithRoad(int startRow, int startColumn, int rectangleHeight, int rectangleWidth)
        {
            if (rectangleHeight <= 0 || rectangleWidth <= 0) return;

            for (int rowOffset = 0; rowOffset < rectangleHeight; rowOffset++)
            {
                for (int columnOffset = 0; columnOffset < rectangleWidth; columnOffset++)
                {
                    SetCellState(startRow + rowOffset, startColumn + columnOffset, true);
                }
            }
        }

        // Рисует сетку из дорожных линий на карте.
        // rowSpacing: Расстояние между горизонтальными линиями сетки. Должно быть положительным.
        // columnSpacing: Расстояние между вертикальными линиями сетки. Должно быть положительным.
        public void DrawGridLines(int rowSpacing, int columnSpacing)
        {
            if (rowSpacing <= 0 || columnSpacing <= 0) return;

            // Отрисовка горизонтальных линий сетки
            for (int rowIndex = 0; rowIndex < MapHeight; rowIndex += rowSpacing)
                DrawLine(rowIndex, 0, rowIndex, MapWidth - 1);
            // Гарантия отрисовки последней строки, если она не покрывается шагом
            if (MapHeight > 0 && (MapHeight - 1) % rowSpacing != 0)
                DrawLine(MapHeight - 1, 0, MapHeight - 1, MapWidth - 1);

            // Отрисовка вертикальных линий сетки
            for (int columnIndex = 0; columnIndex < MapWidth; columnIndex += columnSpacing)
                DrawLine(0, columnIndex, MapHeight - 1, columnIndex);
            // Гарантия отрисовки последнего столбца, если он не покрывается шагом
            if (MapWidth > 0 && (MapWidth - 1) % columnSpacing != 0)
                DrawLine(0, MapWidth - 1, MapHeight - 1, MapWidth - 1);
        }

        // Рисует крестообразный дорожный перекресток.
        // centerRow: Индекс строки центра креста.
        // centerCol: Индекс столбца центра креста.
        // armLength: Длина каждого луча от центра (например, armLength 1 создает крест 3x3).
        public void DrawCross(int centerRow, int centerCol, int armLength)
        {
            if (armLength <= 0) return; // Крест без длины лучей - это просто точка, обрабатывается SetCellState, если необходимо.

            // Отрисовка горизонтального луча, с проверкой на выход за границы карты
            DrawLine(centerRow, Math.Max(0, centerCol - armLength), centerRow, Math.Min(MapWidth - 1, centerCol + armLength));
            // Отрисовка вертикального луча, с проверкой на выход за границы карты
            DrawLine(Math.Max(0, centerRow - armLength), centerCol, Math.Min(MapHeight - 1, centerRow + armLength), centerCol);
        }

        // Рисует Т-образный дорожный перекресток.
        // junctionRow: Индекс строки точки Т-образного соединения.
        // junctionCol: Индекс столбца точки Т-образного соединения.
        // legLength: Длина ножки Т-образного перекрестка. Должна быть положительной.
        // crossbarLength: Длина перекладины Т-образного перекрестка. Должна быть положительной и, в идеале, нечетной для симметрии.
        // orientation: Ориентация ножки Т-образной формы.
        public void DrawTShape(int junctionRow, int junctionCol, int legLength, int crossbarLength, TShapeOrientation orientation)
        {
            if (legLength <= 0 || crossbarLength <= 0) return;

            int halfCrossbar = (crossbarLength - 1) / 2; // Для центрирования перекладины

            switch (orientation)
            {
                case TShapeOrientation.LegDown:
                    DrawLine(junctionRow, junctionCol - halfCrossbar, junctionRow, junctionCol + halfCrossbar); // Перекладина
                    DrawLine(junctionRow, junctionCol, junctionRow + legLength - 1, junctionCol);                // Ножка
                    break;
                case TShapeOrientation.LegUp:
                    DrawLine(junctionRow, junctionCol - halfCrossbar, junctionRow, junctionCol + halfCrossbar); // Перекладина
                    DrawLine(junctionRow, junctionCol, junctionRow - legLength + 1, junctionCol);                // Ножка
                    break;
                case TShapeOrientation.LegLeft:
                    DrawLine(junctionRow - halfCrossbar, junctionCol, junctionRow + halfCrossbar, junctionCol); // Перекладина (вертикальная)
                    DrawLine(junctionRow, junctionCol, junctionRow, junctionCol - legLength + 1);                // Ножка
                    break;
                case TShapeOrientation.LegRight:
                    DrawLine(junctionRow - halfCrossbar, junctionCol, junctionRow + halfCrossbar, junctionCol); // Перекладина (вертикальная)
                    DrawLine(junctionRow, junctionCol, junctionRow, junctionCol + legLength - 1);                // Ножка
                    break;
            }
            // Гарантия, что сама точка соединения отмечена как дорога, так как DrawLine может не включать начало для линий длиной 1
            SetCellState(junctionRow, junctionCol, true);
        }

        // Автоматически генерирует сетку дорог.
        // Очищает существующую карту перед генерацией.
        // numberOfHorizontalRoads: Количество горизонтальных дорог для генерации.
        // numberOfVerticalRoads: Количество вертикальных дорог для генерации.
        public void AutoGenerateGridRoads(int numberOfHorizontalRoads, int numberOfVerticalRoads)
        {
            ClearMap();
            if (numberOfHorizontalRoads > 0 && MapHeight > 0)
            {
                for (int i = 1; i <= numberOfHorizontalRoads; i++)
                {
                    // Расчет равномерно расположенного индекса строки
                    int rowIndex = (MapHeight * i) / (numberOfHorizontalRoads + 1);
                    rowIndex = Math.Max(0, Math.Min(MapHeight - 1, rowIndex)); // Ограничение по границам карты
                    DrawLine(rowIndex, 0, rowIndex, MapWidth - 1);
                }
            }

            if (numberOfVerticalRoads > 0 && MapWidth > 0)
            {
                for (int i = 1; i <= numberOfVerticalRoads; i++)
                {
                    // Расчет равномерно расположенного индекса столбца
                    int columnIndex = (MapWidth * i) / (numberOfVerticalRoads + 1);
                    columnIndex = Math.Max(0, Math.Min(MapWidth - 1, columnIndex)); // Ограничение по границам карты
                    DrawLine(0, columnIndex, MapHeight - 1, columnIndex);
                }
            }
        }

        // Автоматически генерирует набор случайных сегментов дорог.
        // Очищает существующую карту перед генерацией.
        // numberOfRoads: Количество случайных сегментов дорог для генерации.
        // maximumRoadLength: Максимальная длина каждого отдельного сегмента дороги.
        public void AutoGenerateRandomRoads(int numberOfRoads, int maximumRoadLength)
        {
            ClearMap();
            Random randomGenerator = new Random();

            if (MapWidth == 0 || MapHeight == 0) return; // Невозможно генерировать на карте с нулевыми размерами

            for (int i = 0; i < numberOfRoads; i++)
            {
                int startRow = randomGenerator.Next(MapHeight);
                int startColumn = randomGenerator.Next(MapWidth);
                int roadLength = randomGenerator.Next(1, Math.Max(2, maximumRoadLength + 1)); // Длина не менее 1
                bool isHorizontal = randomGenerator.Next(2) == 0; // 50/50 шанс для горизонтальной или вертикальной ориентации

                if (isHorizontal)
                {
                    // Случайный выбор направления (влево или вправо)
                    int direction = (randomGenerator.Next(2) == 0) ? 1 : -1;
                    int endColumn = startColumn + direction * (roadLength - 1);
                    // Ограничение endColumn по границам карты
                    endColumn = Math.Max(0, Math.Min(MapWidth - 1, endColumn));
                    // Корректировка startColumn, если endColumn был ограничен и направление вывело его за пределы
                    if (direction == 1 && endColumn < startColumn + (roadLength - 1)) startColumn = Math.Max(0, endColumn - (roadLength - 1));
                    else if (direction == -1 && endColumn > startColumn - (roadLength - 1)) startColumn = Math.Min(MapWidth - 1, endColumn + (roadLength - 1));

                    DrawLine(startRow, startColumn, startRow, endColumn);
                }
                else // Вертикальная дорога
                {
                    // Случайный выбор направления (вверх или вниз)
                    int direction = (randomGenerator.Next(2) == 0) ? 1 : -1;
                    int endRow = startRow + direction * (roadLength - 1);
                    // Ограничение endRow по границам карты
                    endRow = Math.Max(0, Math.Min(MapHeight - 1, endRow));
                    if (direction == 1 && endRow < startRow + (roadLength - 1)) startRow = Math.Max(0, endRow - (roadLength - 1));
                    else if (direction == -1 && endRow > startRow - (roadLength - 1)) startRow = Math.Min(MapHeight - 1, endRow + (roadLength - 1));

                    DrawLine(startRow, startColumn, endRow, startColumn);
                }
            }
        }

        // Сохраняет текущие данные дорожной карты в JSON-файл.
        // filePath: Путь к файлу, в который будет сохранена карта.
        public void SaveToFile(string filePath)
        {
            // Подготовка данных в сериализуемом формате.
            var mapDataToSave = new RoadMapSaveFormat
            {
                Width = this.MapWidth,
                Height = this.MapHeight,
                RoadCells = this.RoadCellDataForSerialization // Использование свойства, предназначенного для сериализации
            };
            string jsonString = JsonSerializer.Serialize(mapDataToSave, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        // Загружает дорожную карту из JSON-файла.
        // filePath: Путь к файлу, из которого будет загружена карта.
        // Возвращает экземпляр RoadMapBase, загруженный из файла, или null, если загрузка не удалась.
        public static RoadMapBase LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return null;
            }
            try
            {
                var loadedData = JsonSerializer.Deserialize<RoadMapSaveFormat>(File.ReadAllText(filePath));

                // Валидация загруженных данных
                if (loadedData == null || loadedData.RoadCells == null || loadedData.Height < 0 || loadedData.Width < 0)
                {
                    Console.WriteLine("Ошибка: Некорректные данные в файле карты.");
                    return null;
                }
                if (loadedData.RoadCells.Length != loadedData.Height)
                {
                    Console.WriteLine("Ошибка: Несоответствие высоты и данных карты.");
                    return null;
                }
                // Дополнительная валидация: проверка, имеют ли все внутренние массивы (строки) корректную ширину, если Height > 0 и Width > 0.
                if (loadedData.Height > 0 && loadedData.Width > 0)
                {
                    for (int i = 0; i < loadedData.RoadCells.Length; ++i)
                    {
                        if (loadedData.RoadCells[i] == null || loadedData.RoadCells[i].Length != loadedData.Width)
                        {
                            Console.WriteLine($"Ошибка: Некорректная ширина строки {i} в данных карты. Ожидалось {loadedData.Width}, получено {loadedData.RoadCells[i]?.Length ?? -1}");
                            return null;
                        }
                    }
                }


                RoadMapBase loadedMap = new RoadMapBase(loadedData.Width, loadedData.Height);
                // Использование сеттера RoadCellDataForSerialization для заполнения карты
                loadedMap.RoadCellDataForSerialization = loadedData.RoadCells;

                // Валидация после загрузки для гарантии, что размеры были корректно установлены сеттером
                if (loadedMap.MapWidth != loadedData.Width || loadedMap.MapHeight != loadedData.Height)
                {
                    Console.WriteLine("Критическая ошибка: Размеры карты после загрузки неверны. Это может указывать на проблему в сеттере RoadCellDataForSerialization.");
                    return null;
                }
                return loadedMap;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Ошибка десериализации JSON: {jsonEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки карты: {ex.Message}");
                return null;
            }
        }

        // Определяет структуру для сохранения и загрузки данных дорожной карты в формате JSON.
        private class RoadMapSaveFormat
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public bool[][] RoadCells { get; set; } // Использование невыровненного массива для упрощения JSON-сериализации
        }

        // Печатает базовое представление карты в консоль с рамкой.
        // Это метод экземпляра и использует данные текущей карты.
        // roadChar: Символ для представления дорожной ячейки.
        // emptyChar: Символ для представления пустой ячейки.
        public void PrintBasicMapWithBorder(char roadChar = '*', char emptyChar = '-')
        {
            Console.WriteLine($"Базовая карта ({this.MapWidth}x{this.MapHeight}) с рамкой:");
            Console.Write("╔"); for (int i = 0; i < this.MapWidth; i++) Console.Write("═"); Console.WriteLine("╗");
            for (int rowIndex = 0; rowIndex < this.MapHeight; rowIndex++)
            {
                Console.Write("║");
                for (int colIndex = 0; colIndex < this.MapWidth; colIndex++)
                {
                    Console.Write(this.GetCellState(rowIndex, colIndex) ? roadChar : emptyChar);
                }
                Console.WriteLine("║");
            }
            Console.Write("╚"); for (int i = 0; i < this.MapWidth; i++) Console.Write("═"); Console.WriteLine("╝");
        }

        // Печатает символьную карту (2D массив char) в консоль с заголовком и рамкой.
        // Это статический служебный метод.
        // symbolMap: 2D массив char, представляющий карту для печати.
        // title: Заголовок для отображения над картой.
        public static void PrintSymbolMapWithHeaderAndBorder(char[,] symbolMap, string title)
        {
            if (symbolMap == null) { Console.WriteLine($"Карта '{title}' не существует для вывода."); return; }
            int numRows = symbolMap.GetLength(0);
            int numCols = symbolMap.GetLength(1);
            Console.WriteLine($"{title} ({numCols}x{numRows}):");

            // Верхняя рамка
            Console.Write("╔");
            for (int i = 0; i < numCols; i++) Console.Write("═");
            Console.WriteLine("╗");

            // Содержимое карты с боковыми рамками
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                Console.Write("║");
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Console.Write(symbolMap[rowIndex, colIndex]);
                }
                Console.WriteLine("║");
            }

            // Нижняя рамка
            Console.Write("╚");
            for (int i = 0; i < numCols; i++) Console.Write("═");
            Console.WriteLine("╝");
        }

        // Константы битовых масок для идентификации соединений дорог с соседними ячейками.
        private const int CONNECTION_TO_NORTH = 1; // Двоичное 0001
        private const int CONNECTION_TO_EAST = 2;  // Двоичное 0010
        private const int CONNECTION_TO_SOUTH = 4; // Двоичное 0100
        private const int CONNECTION_TO_WEST = 8;  // Двоичное 1000

        // Словарь, отображающий битовые маски соединений на соответствующие символы псевдографики.
        private static readonly Dictionary<int, char> RoadSegmentSymbols = new Dictionary<int, char>
        {
            // Прямые линии
            { CONNECTION_TO_EAST | CONNECTION_TO_WEST,   '─'}, // Горизонтальная
            { CONNECTION_TO_NORTH | CONNECTION_TO_SOUTH, '│'}, // Вертикальная
            // Углы
            { CONNECTION_TO_SOUTH | CONNECTION_TO_EAST,  '┌'}, // Верхний левый угол (соединяет Юг и Восток)
            { CONNECTION_TO_SOUTH | CONNECTION_TO_WEST,  '┐'}, // Верхний правый угол (соединяет Юг и Запад)
            { CONNECTION_TO_NORTH | CONNECTION_TO_EAST,  '└'}, // Нижний левый угол (соединяет Север и Восток)
            { CONNECTION_TO_NORTH | CONNECTION_TO_WEST,  '┘'}, // Нижний правый угол (соединяет Север и Запад)
            // Т-образные перекрестки
            { CONNECTION_TO_SOUTH | CONNECTION_TO_EAST | CONNECTION_TO_WEST,  '┬'}, // Т-образный перекресток, ножка вниз
            { CONNECTION_TO_NORTH | CONNECTION_TO_EAST | CONNECTION_TO_WEST,  '┴'}, // Т-образный перекресток, ножка вверх
            { CONNECTION_TO_NORTH | CONNECTION_TO_SOUTH | CONNECTION_TO_EAST, '├'}, // Т-образный перекресток, ножка вправо
            { CONNECTION_TO_NORTH | CONNECTION_TO_SOUTH | CONNECTION_TO_WEST, '┤'}, // Т-образный перекресток, ножка влево
            // Крестообразный перекресток
            { CONNECTION_TO_NORTH | CONNECTION_TO_EAST | CONNECTION_TO_SOUTH | CONNECTION_TO_WEST, '┼'},
            // Тупики (трактуются как короткие линии, если другие соединения отсутствуют контекстуально)
            { CONNECTION_TO_NORTH, '│'}, // Соединяется только с Севером
            { CONNECTION_TO_EAST,  '─'}, // Соединяется только с Востоком
            { CONNECTION_TO_SOUTH, '│'}, // Соединяется только с Югом
            { CONNECTION_TO_WEST,  '─'}  // Соединяется только с Западом
            // Одиночная изолированная точка дороги (маска 0) будет обработана пропуском и получит ' ' или значение по умолчанию.
        };

        // Вычисляет битовую маску, представляющую соединения дорожной ячейки с ее соседями.
        // rowIndex: Индекс строки ячейки.
        // columnIndex: Индекс столбца ячейки.
        // Возвращает целочисленную битовую маску. Возвращает 0, если сама ячейка не является дорогой.
        private int GetCellConnectionBitmask(int rowIndex, int columnIndex)
        {
            if (!GetCellState(rowIndex, columnIndex)) return 0; // Не дорожная ячейка, нет соединений

            int connectionBitmask = 0;
            if (GetCellState(rowIndex - 1, columnIndex)) connectionBitmask |= CONNECTION_TO_NORTH; // Проверка Севера
            if (GetCellState(rowIndex, columnIndex + 1)) connectionBitmask |= CONNECTION_TO_EAST;  // Проверка Востока
            if (GetCellState(rowIndex + 1, columnIndex)) connectionBitmask |= CONNECTION_TO_SOUTH; // Проверка Юга
            if (GetCellState(rowIndex, columnIndex - 1)) connectionBitmask |= CONNECTION_TO_WEST;  // Проверка Запада
            return connectionBitmask;
        }

        // Преобразует текущую дорожную карту в 2D массив символов, используя символы псевдографики.
        // Возвращает char[,], где дорожные ячейки представлены соответствующими символами, а пустые ячейки - пробелами.
        public char[,] ConvertToSymbolMap()
        {
            char[,] symbolMapGrid = new char[MapHeight, MapWidth];
            for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < MapWidth; columnIndex++)
                {
                    if (!GetCellState(rowIndex, columnIndex))
                    {
                        symbolMapGrid[rowIndex, columnIndex] = ' '; // Пустая ячейка
                        continue;
                    }

                    int connectionBitmask = GetCellConnectionBitmask(rowIndex, columnIndex);
                    if (RoadSegmentSymbols.TryGetValue(connectionBitmask, out char roadSymbol))
                    {
                        symbolMapGrid[rowIndex, columnIndex] = roadSymbol;
                    }
                    else
                    {
                        // Значение по умолчанию для изолированных дорожных ячеек или не сопоставленных битовых масок (например, одиночная точка дороги).
                        // Это может быть другой символ, например, '#' или оставаться ' ', если изолированные точки не являются дорогами.
                        // Текущая логика: если битовая маска равна 0 (изолированная точка), по умолчанию используется ' '.
                        // Для этого конкретного набора символов одиночная точка дороги имеет битовую маску 0, поэтому она становится ' '.
                        // Если это должна быть точка, добавьте {0, '.'} в RoadSegmentSymbols.
                        // Или, если любая точка дороги является дорогой, используйте символ по умолчанию, например '■'.
                        symbolMapGrid[rowIndex, columnIndex] = ' '; // Или, возможно, символ дороги по умолчанию, такой как '+' или '*'
                    }
                }
            }
            return symbolMapGrid;
        }

        // Печатает символьную карту (2D массив char) непосредственно в консоль без рамок и заголовков.
        // Примечание: PrintSymbolMapWithHeaderAndBorder предлагает более форматированный вывод.
        // symbolMapToPrint: 2D массив char для печати.
        public static void PrintSymbolMapToConsole(char[,] symbolMapToPrint)
        {
            // Этот метод дублирует функциональность PrintSymbolMapWithHeaderAndBorder,
            // но без рамки и заголовка. Оставляю его, если он нужен для других целей,
            // иначе его можно удалить, если PrintSymbolMapWithHeaderAndBorder достаточно.
            if (symbolMapToPrint == null) return;
            int numberOfRows = symbolMapToPrint.GetLength(0);
            int numberOfColumns = symbolMapToPrint.GetLength(1);
            for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < numberOfColumns; columnIndex++)
                {
                    Console.Write(symbolMapToPrint[rowIndex, columnIndex]);
                }
                Console.WriteLine();
            }
        }

        // Хранит спрайты символов 3x3 для каждого основного символа дороги, используется в ConvertToDetailedSymbolMap.
        private static Dictionary<char, char[,]> _expandedRoadSymbolSprites;

        // Инициализирует словарь спрайтов символов 3x3 для дорожных символов, если он еще не был инициализирован.
        // Это форма отложенной инициализации.
        // roadBlockCharacter: Символ, используемый для отрисовки частей дороги внутри спрайта 3x3.
        // backgroundCharacter: Символ, используемый для пустого пространства внутри спрайта 3x3.
        private static void InitializeExpandedRoadSymbolSprites(char roadBlockCharacter = '█', char backgroundCharacter = ' ')
        {
            if (_expandedRoadSymbolSprites != null) return; // Уже инициализировано

            char R = roadBlockCharacter; // Псевдоним для блока дороги
            char B = backgroundCharacter; // Псевдоним для фона

            _expandedRoadSymbolSprites = new Dictionary<char, char[,]>
            {
                // Определение шаблонов символов 3x3 для каждого символа дороги
                { '─', new char[3,3]{{B,B,B},{R,R,R},{B,B,B}} }, // Горизонтальная линия
                { '│', new char[3,3]{{B,R,B},{B,R,B},{B,R,B}} }, // Вертикальная линия
                { '┌', new char[3,3]{{B,B,B},{B,R,R},{B,R,B}} }, // Верхний левый угол
                { '┐', new char[3,3]{{B,B,B},{R,R,B},{B,R,B}} }, // Верхний правый угол
                { '└', new char[3,3]{{B,R,B},{B,R,R},{B,B,B}} }, // Нижний левый угол
                { '┘', new char[3,3]{{B,R,B},{R,R,B},{B,B,B}} }, // Нижний правый угол
                { '┬', new char[3,3]{{B,B,B},{R,R,R},{B,R,B}} }, // Т-образный перекресток, ножка вниз
                { '┴', new char[3,3]{{B,R,B},{R,R,R},{B,B,B}} }, // Т-образный перекресток, ножка вверх
                { '├', new char[3,3]{{B,R,B},{B,R,R},{B,R,B}} }, // Т-образный перекресток, ножка вправо
                { '┤', new char[3,3]{{B,R,B},{R,R,B},{B,R,B}} }, // Т-образный перекресток, ножка влево
                { '┼', new char[3,3]{{B,R,B},{R,R,R},{B,R,B}} }, // Крестообразный перекресток
                { ' ', new char[3,3]{{B,B,B},{B,B,B},{B,B,B}} }  // Пустое пространство
            };
        }

        // Преобразует стандартную символьную карту в "детализированную" символьную карту, где каждая исходная ячейка
        // расширяется в блок символов 3x3, обеспечивая визуально более толстое представление.
        // Возвращает char[,], представляющий детализированную карту, или массив char[0,0], если исходная карта пуста.
        public char[,] ConvertToDetailedSymbolMap()
        {
            InitializeExpandedRoadSymbolSprites(); // Гарантия загрузки спрайтов

            if (MapWidth == 0 || MapHeight == 0) return new char[0, 0]; // Обработка пустой карты

            char[,] baseSymbolMap = ConvertToSymbolMap(); // Сначала получаем стандартную символьную карту
            int outputHeight = MapHeight * 3;
            int outputWidth = MapWidth * 3;
            char[,] detailedMapGrid = new char[outputHeight, outputWidth];

            for (int baseRowIndex = 0; baseRowIndex < MapHeight; baseRowIndex++)
            {
                for (int baseColumnIndex = 0; baseColumnIndex < MapWidth; baseColumnIndex++)
                {
                    char baseSymbolCharacter = baseSymbolMap[baseRowIndex, baseColumnIndex];

                    // Получение спрайта 3x3 для текущего символа, по умолчанию пустое пространство, если не найден
                    if (!_expandedRoadSymbolSprites.TryGetValue(baseSymbolCharacter, out var currentCharacterSprite))
                    {
                        currentCharacterSprite = _expandedRoadSymbolSprites[' ']; // По умолчанию пустой спрайт
                    }

                    // Копирование спрайта 3x3 в сетку детализированной карты
                    for (int spriteRow = 0; spriteRow < 3; spriteRow++)
                    {
                        for (int spriteColumn = 0; spriteColumn < 3; spriteColumn++)
                        {
                            // Проверка границ на всякий случай, хотя размеры должны совпадать
                            if ((baseRowIndex * 3 + spriteRow < outputHeight) && (baseColumnIndex * 3 + spriteColumn < outputWidth))
                            {
                                detailedMapGrid[baseRowIndex * 3 + spriteRow, baseColumnIndex * 3 + spriteColumn] = currentCharacterSprite[spriteRow, spriteColumn];
                            }
                        }
                    }
                }
            }
            return detailedMapGrid;
        }

        // Сопоставляет символы дороги с соответствующими координатами тайлов (Rectangle) на спрайт-листе.
        private static Dictionary<char, Rectangle> _symbolToTileSpriteMap;

        // Инициализирует сопоставление символов дороги с координатами тайлов спрайт-листа.
        // Это форма отложенной инициализации. Координаты специфичны для "preview_22.jpg".
        // Предполагается, что каждый тайл имеет размер 256x256 пикселей.
        private static void InitializeSymbolToTileSpriteMappings()
        {
            if (_symbolToTileSpriteMap != null) return; // Уже инициализировано

            _symbolToTileSpriteMap = new Dictionary<char, Rectangle> {
                // Определение исходного прямоугольника (x, y, ширина, высота) на спрайт-листе для каждого символа
                { ' ', new Rectangle(256, 256, 256, 256) }, // Пустой тайл (например, тайл травы) - пример координат
                { '─', new Rectangle(0, 0, 256, 256) },     // Горизонтальная дорога
                { '│', new Rectangle(768, 0, 256, 256) },   // Вертикальная дорога
                { '┌', new Rectangle(512, 256, 256, 256) }, // Верхний левый угол
                { '┐', new Rectangle(768, 256, 256, 256) }, // Верхний правый угол
                { '└', new Rectangle(256, 0, 256, 256) },   // Нижний левый угол
                { '┘', new Rectangle(0, 256, 256, 256) },   // Нижний правый угол
                { '├', new Rectangle(0, 512, 256, 256) },   // Т-образный перекресток, ножка вправо
                { '┤', new Rectangle(768, 512, 256, 256) }, // Т-образный перекресток, ножка влево
                { '┬', new Rectangle(512, 512, 256, 256) }, // Т-образный перекресток, ножка вниз
                { '┴', new Rectangle(256, 512, 256, 256) }, // Т-образный перекресток, ножка вверх
                { '┼', new Rectangle(512, 0, 256, 256) }    // Крестообразный перекресток
            };
        }

        // Преобразует дорожную карту в изображение путем размещения тайлов из указанного спрайт-листа.
        // Требует NuGet-пакет System.Drawing.Common для кроссплатформенной совместимости, если не на Windows.
        // outputImageFilePath: Путь к файлу для сохранения сгенерированного PNG-изображения.
        // roadSpriteSheetFilePath: Путь к файлу изображения спрайт-листа.
        public void ConvertToImageSpriteMap(string outputImageFilePath, string roadSpriteSheetFilePath)
        {
            InitializeSymbolToTileSpriteMappings(); // Гарантия загрузки сопоставлений символов и тайлов

            char[,] symbolMapGrid = ConvertToSymbolMap(); // Получение стандартного символьного представления
            int numberOfRows = symbolMapGrid.GetLength(0);
            int numberOfColumns = symbolMapGrid.GetLength(1);
            int tilePixelWidth = 256;  // Ширина одного тайла в пикселях (должна соответствовать спрайт-листу)
            int tilePixelHeight = 256; // Высота одного тайла в пикселях (должна соответствовать спрайт-листу)

            if (numberOfRows == 0 || numberOfColumns == 0)
            {
                Console.WriteLine("Карта пуста, изображение не будет сгенерировано.");
                return;
            }

            // Определение полного пути к спрайт-листу
            string fullSpriteSheetPath = roadSpriteSheetFilePath;
            if (!Path.IsPathRooted(roadSpriteSheetFilePath))
            {
                fullSpriteSheetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, roadSpriteSheetFilePath);
            }

            if (!File.Exists(fullSpriteSheetPath))
            {
                Console.WriteLine($"Файл спрайт-листа не найден: {fullSpriteSheetPath}");
                return;
            }

            try
            {
                // Загрузка спрайт-листа
                using (Bitmap sourceSpriteSheet = new Bitmap(fullSpriteSheetPath))
                // Создание выходного изображения bitmap
                using (Bitmap outputImage = new Bitmap(numberOfColumns * tilePixelWidth, numberOfRows * tilePixelHeight))
                // Получение графического объекта для рисования на выходном изображении bitmap
                using (Graphics graphicsRenderer = Graphics.FromImage(outputImage))
                {
                    // Необязательно: заливка фона, если необходимо, иначе тайлы будут нарисованы на черном (по умолчанию для нового Bitmap)
                    graphicsRenderer.Clear(Color.LightSkyBlue); // Пример цвета фона

                    for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
                    {
                        for (int columnIndex = 0; columnIndex < numberOfColumns; columnIndex++)
                        {
                            char roadSymbolCharacter = symbolMapGrid[rowIndex, columnIndex];

                            // Получение исходного прямоугольника из спрайт-листа для текущего символа
                            // По умолчанию используется 'пустой' тайл, если символ не найден на карте
                            Rectangle sourceSpriteRectangle = _symbolToTileSpriteMap.GetValueOrDefault(roadSymbolCharacter, _symbolToTileSpriteMap[' ']);

                            // Определение целевого прямоугольника на выходном изображении
                            Rectangle destinationRectangleOnOutput = new Rectangle(
                                columnIndex * tilePixelWidth,
                                rowIndex * tilePixelHeight,
                                tilePixelWidth,
                                tilePixelHeight);

                            // Отрисовка тайла из спрайт-листа на выходное изображение
                            graphicsRenderer.DrawImage(
                                sourceSpriteSheet,
                                destinationRectangleOnOutput,
                                sourceSpriteRectangle,
                                GraphicsUnit.Pixel);
                        }
                    }
                    // Сохранение составленного изображения в указанный файл в формате PNG
                    outputImage.Save(outputImageFilePath, ImageFormat.Png);
                    Console.WriteLine($"Карта спрайтов изображений сохранена в {outputImageFilePath}");
                }
            }
            catch (Exception ex)
            {
                // Перехват потенциальных ошибок во время обработки изображений (например, доступ к файлу, ошибки GDI+)
                Console.WriteLine($"Ошибка при генерации карты спрайтов изображений: {ex.Message}");
            }
        }
    }
}