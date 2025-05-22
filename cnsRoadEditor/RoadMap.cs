using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;

namespace cnsRoadEditor
{
    public class RoadMap
    {
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        private bool[,] _roadGridData;

        public bool[][] RoadGridDataSerializable
        {
            get
            {
                bool[][] jaggedArray = new bool[MapHeight][];
                for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
                {
                    jaggedArray[rowIndex] = new bool[MapWidth];
                    for (int colIndex = 0; colIndex < MapWidth; colIndex++)
                    {
                        jaggedArray[rowIndex][colIndex] = _roadGridData[rowIndex, colIndex];
                    }
                }
                return jaggedArray;
            }
            set
            {
                if (value == null) // Добавлена проверка на null для всего 'value'
                {
                    MapHeight = 0;
                    MapWidth = 0;
                    _roadGridData = new bool[0, 0];
                    Console.WriteLine("Предупреждение: Попытка установить RoadGridDataSerializable значением null. Карта очищена.");
                    return;
                }

                MapHeight = value.Length;
                if (MapHeight > 0)
                {
                    // Безопасное получение длины первой строки, если она существует
                    MapWidth = value[0]?.Length ?? 0;
                    _roadGridData = new bool[MapHeight, MapWidth];

                    for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
                    {
                        if (value[rowIndex] == null)
                        {
                            Console.WriteLine($"Предупреждение: Строка {rowIndex} в данных карты равна null при установке RoadGridDataSerializable. Инициализируется значениями false.");
                            // Инициализируем строку в _roadGridData значениями false, если MapWidth > 0
                            if (MapWidth > 0)
                            {
                                for (int c = 0; c < MapWidth; c++)
                                {
                                    _roadGridData[rowIndex, c] = false;
                                }
                            }
                        }
                        else
                        {
                            // Если MapWidth был определен как 0 (например, value[0] был null, но value не был),
                            // а другие строки имеют длину, это может быть проблемой.
                            // Здесь мы доверяем MapWidth, определенному по первой строке.
                            // Если строки имеют разную длину, это будет обработано ниже.
                            if (value[rowIndex].Length != MapWidth && MapWidth > 0)
                            {
                                Console.WriteLine($"Предупреждение: Строка {rowIndex} имеет длину {value[rowIndex].Length}, ожидалось {MapWidth}. Данные могут быть усечены или дополнены.");
                            }

                            for (int colIndex = 0; colIndex < MapWidth; colIndex++)
                            {
                                if (colIndex < value[rowIndex].Length)
                                {
                                    _roadGridData[rowIndex, colIndex] = value[rowIndex][colIndex];
                                }
                                else
                                {
                                    // Если строка value[rowIndex] короче, чем MapWidth,
                                    // заполняем оставшуюся часть _roadGridData[rowIndex] значениями false.
                                    _roadGridData[rowIndex, colIndex] = false;
                                }
                            }
                        }
                    }
                }
                else // MapHeight == 0
                {
                    MapWidth = 0;
                    _roadGridData = new bool[0, 0];
                }
            }
        }

        public RoadMap(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Ширина и Высота должны быть положительными.");
            MapWidth = width;
            MapHeight = height;
            _roadGridData = new bool[MapHeight, MapWidth];
        }

        public bool GetCellState(int rowIndex, int columnIndex)
        {
            if (rowIndex >= 0 && rowIndex < MapHeight && columnIndex >= 0 && columnIndex < MapWidth)
                return _roadGridData[rowIndex, columnIndex];
            return false;
        }

        public void SetCellState(int rowIndex, int columnIndex, bool isRoad)
        {
            if (rowIndex >= 0 && rowIndex < MapHeight && columnIndex >= 0 && columnIndex < MapWidth)
            {
                _roadGridData[rowIndex, columnIndex] = isRoad;
            }
        }

        public void ClearMap()
        {
            _roadGridData = new bool[MapHeight, MapWidth];
        }

        public void DrawLine(int startRow, int startColumn, int endRow, int endColumn)
        {
            if (startRow == endRow)
            {
                for (int column = Math.Min(startColumn, endColumn); column <= Math.Max(startColumn, endColumn); column++)
                    SetCellState(startRow, column, true);
            }
            else if (startColumn == endColumn)
            {
                for (int row = Math.Min(startRow, endRow); row <= Math.Max(startRow, endRow); row++)
                    SetCellState(row, startColumn, true);
            }
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
                    if (currentRow == endRow && currentColumn == endColumn) break;
                    int error2 = 2 * error;
                    if (error2 > -deltaColumn) { error -= deltaColumn; currentRow += stepRow; }
                    if (error2 < deltaRow) { error += deltaRow; currentColumn += stepColumn; }
                }
            }
        }

        public void DrawRectangleOutline(int startRow, int startColumn, int rectangleHeight, int rectangleWidth)
        {
            if (rectangleHeight <= 0 || rectangleWidth <= 0) return;
            int endCol = startColumn + rectangleWidth - 1;
            int endRow = startRow + rectangleHeight - 1;

            DrawLine(startRow, startColumn, startRow, endCol);
            DrawLine(endRow, startColumn, endRow, endCol);
            DrawLine(startRow, startColumn, endRow, startColumn);
            DrawLine(startRow, endCol, endRow, endCol);
        }

        public void FillRectangleWithRoad(int startRow, int startColumn, int rectangleHeight, int rectangleWidth)
        {
            if (rectangleHeight <= 0 || rectangleWidth <= 0) return;
            for (int rowIndex = 0; rowIndex < rectangleHeight; rowIndex++)
                for (int colIndex = 0; colIndex < rectangleWidth; colIndex++)
                    SetCellState(startRow + rowIndex, startColumn + colIndex, true);
        }

        public void DrawGridLines(int rowSpacing, int columnSpacing)
        {
            if (rowSpacing <= 0 || columnSpacing <= 0) return;

            for (int rowIndex = 0; rowIndex < MapHeight; rowIndex += rowSpacing)
                DrawLine(rowIndex, 0, rowIndex, MapWidth - 1);
            if (MapHeight > 0 && (MapHeight - 1) % rowSpacing != 0)
                DrawLine(MapHeight - 1, 0, MapHeight - 1, MapWidth - 1);

            for (int colIndex = 0; colIndex < MapWidth; colIndex += columnSpacing)
                DrawLine(0, colIndex, MapHeight - 1, colIndex);
            if (MapWidth > 0 && (MapWidth - 1) % columnSpacing != 0)
                DrawLine(0, MapWidth - 1, MapHeight - 1, MapWidth - 1);
        }


        public void AutoGenerateGridRoads(int numberOfHorizontalRoads, int numberOfVerticalRoads)
        {
            ClearMap();
            if (numberOfHorizontalRoads > 0 && MapHeight > 0)
            {
                for (int i = 1; i <= numberOfHorizontalRoads; i++)
                {
                    int rowIndex = (MapHeight * i) / (numberOfHorizontalRoads + 1);
                    rowIndex = Math.Max(0, Math.Min(MapHeight - 1, rowIndex));
                    DrawLine(rowIndex, 0, rowIndex, MapWidth - 1);
                }
            }
            if (numberOfVerticalRoads > 0 && MapWidth > 0)
            {
                for (int i = 1; i <= numberOfVerticalRoads; i++)
                {
                    int colIndex = (MapWidth * i) / (numberOfVerticalRoads + 1);
                    colIndex = Math.Max(0, Math.Min(MapWidth - 1, colIndex));
                    DrawLine(0, colIndex, MapHeight - 1, colIndex);
                }
            }
        }

        public void AutoGenerateRandomRoads(int numberOfRoads, int maximumRoadLength)
        {
            ClearMap();
            Random randomGenerator = new Random();
            if (MapWidth == 0 || MapHeight == 0) return;
            for (int i = 0; i < numberOfRoads; i++)
            {
                int startRow = randomGenerator.Next(MapHeight);
                int startColumn = randomGenerator.Next(MapWidth);
                int roadLength = randomGenerator.Next(1, Math.Max(2, maximumRoadLength + 1));
                bool isHorizontal = randomGenerator.Next(2) == 0;
                if (isHorizontal)
                {
                    int endColumn = Math.Max(0, Math.Min(MapWidth - 1, startColumn + ((randomGenerator.Next(2) == 0) ? 1 : -1) * (roadLength - 1)));
                    DrawLine(startRow, startColumn, startRow, endColumn);
                }
                else
                {
                    int endRow = Math.Max(0, Math.Min(MapHeight - 1, startRow + ((randomGenerator.Next(2) == 0) ? 1 : -1) * (roadLength - 1)));
                    DrawLine(startRow, startColumn, endRow, startColumn);
                }
            }
        }

        public void SaveToFile(string filePath)
        {
            var serializableMap = new RoadMapSerializableData { Width = this.MapWidth, Height = this.MapHeight, MapData = this.RoadGridDataSerializable };
            string jsonString = JsonSerializer.Serialize(serializableMap, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        public static RoadMap LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return null;
            }
            try
            {
                var serializableMapData = JsonSerializer.Deserialize<RoadMapSerializableData>(File.ReadAllText(filePath));
                if (serializableMapData == null)
                {
                    Console.WriteLine("Ошибка: Не удалось десериализовать данные карты из файла (результат null).");
                    return null;
                }
                if (serializableMapData.MapData == null)
                {
                    Console.WriteLine("Ошибка: Поле MapData в файле равно null.");
                    return null;
                }


                if (serializableMapData.Height <= 0 || serializableMapData.Width < 0)
                { // Width может быть 0, если Height > 0 но первая строка пустая
                    Console.WriteLine($"Ошибка: Размеры карты в файле некорректны (Height: {serializableMapData.Height}, Width: {serializableMapData.Width}).");
                    return null;
                }

                if (serializableMapData.MapData.Length != serializableMapData.Height)
                {
                    Console.WriteLine($"Ошибка: Высота карты ({serializableMapData.Height}) не соответствует количеству строк в MapData ({serializableMapData.MapData.Length}).");
                    return null;
                }

                RoadMap loadedMap = new RoadMap(serializableMapData.Width, serializableMapData.Height); // Используем размеры из файла

                // Присваиваем данные через свойство, которое теперь корректно обрабатывает разные длины
                loadedMap.RoadGridDataSerializable = serializableMapData.MapData;

                // Дополнительная проверка, что после присвоения размеры карты соответствуют заявленным в файле
                // (свойство RoadGridDataSerializable само должно было это обеспечить)
                if (loadedMap.MapWidth != serializableMapData.Width || loadedMap.MapHeight != serializableMapData.Height)
                {
                    Console.WriteLine("Критическая ошибка: Размеры карты после присвоения данных не совпадают с исходными из файла.");
                    return null;
                }

                return loadedMap;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка десериализации JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка при загрузке карты: {ex.Message}");
                return null;
            }
        }


        private class RoadMapSerializableData
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public bool[][] MapData { get; set; }
        }

        public void PrintToConsole(char roadCharacter = '#', char emptyCharacter = '.')
        {
            Console.WriteLine($"Карта дорог ({MapWidth}x{MapHeight}):");
            for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
            {
                for (int colIndex = 0; colIndex < MapWidth; colIndex++)
                {
                    Console.Write(_roadGridData[rowIndex, colIndex] ? roadCharacter : emptyCharacter);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        public char[,] ConvertToSymbolMap()
        {
            char[,] symbolMapGrid = new char[MapHeight, MapWidth];

            const int NORTH_BIT = 0b0001;
            const int EAST_BIT = 0b0010;
            const int SOUTH_BIT = 0b0100;
            const int WEST_BIT = 0b1000;

            Dictionary<int, char> roadSymbols = new Dictionary<int, char>
            {
                { EAST_BIT | WEST_BIT,   '─'},
                { NORTH_BIT | SOUTH_BIT, '│'},
                { SOUTH_BIT | EAST_BIT,  '┌'},
                { SOUTH_BIT | WEST_BIT,  '┐'},
                { NORTH_BIT | EAST_BIT,  '└'},
                { NORTH_BIT | WEST_BIT,  '┘'},
                { SOUTH_BIT | EAST_BIT | WEST_BIT,  '┬'},
                { NORTH_BIT | EAST_BIT | WEST_BIT,  '┴'},
                { NORTH_BIT | SOUTH_BIT | EAST_BIT, '├'},
                { NORTH_BIT | SOUTH_BIT | WEST_BIT, '┤'},
                { NORTH_BIT | EAST_BIT | SOUTH_BIT | WEST_BIT, '┼'}
            };

            char unrepresentableConnectionChar = '?';

            for (int rowIndex = 0; rowIndex < MapHeight; rowIndex++)
            {
                for (int colIndex = 0; colIndex < MapWidth; colIndex++)
                {
                    if (!_roadGridData[rowIndex, colIndex])
                    {
                        symbolMapGrid[rowIndex, colIndex] = ' ';
                        continue;
                    }

                    int connectionBitmask = 0;
                    if (rowIndex > 0 && _roadGridData[rowIndex - 1, colIndex]) connectionBitmask |= NORTH_BIT;
                    if (colIndex < MapWidth - 1 && _roadGridData[rowIndex, colIndex + 1]) connectionBitmask |= EAST_BIT;
                    if (rowIndex < MapHeight - 1 && _roadGridData[rowIndex + 1, colIndex]) connectionBitmask |= SOUTH_BIT;
                    if (colIndex > 0 && _roadGridData[rowIndex, colIndex - 1]) connectionBitmask |= WEST_BIT;

                    if (connectionBitmask == 0)
                    {
                        symbolMapGrid[rowIndex, colIndex] = ' ';
                    }
                    else if (roadSymbols.TryGetValue(connectionBitmask, out char roadSymbolCharacter))
                    {
                        symbolMapGrid[rowIndex, colIndex] = roadSymbolCharacter;
                    }
                    else
                    {
                        if (connectionBitmask == NORTH_BIT) symbolMapGrid[rowIndex, colIndex] = '│';
                        else if (connectionBitmask == EAST_BIT) symbolMapGrid[rowIndex, colIndex] = '─';
                        else if (connectionBitmask == SOUTH_BIT) symbolMapGrid[rowIndex, colIndex] = '│';
                        else if (connectionBitmask == WEST_BIT) symbolMapGrid[rowIndex, colIndex] = '─';
                        else symbolMapGrid[rowIndex, colIndex] = unrepresentableConnectionChar;
                    }
                }
            }
            return symbolMapGrid;
        }

        public static void PrintSymbolMapToConsole(char[,] symbolMapGrid)
        {
            if (symbolMapGrid == null) return;
            int numRows = symbolMapGrid.GetLength(0);
            int numCols = symbolMapGrid.GetLength(1);
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Console.Write(symbolMapGrid[rowIndex, colIndex]);
                }
                Console.WriteLine();
            }
        }

        private static Dictionary<char, char[,]> _blockRoadSprites;
        private static char _blockRoadChar = '█';
        private static char _blockBackgroundChar = ' ';

        private static void InitializeBlockRoadSprites()
        {
            if (_blockRoadSprites != null) return;

            char r = _blockRoadChar;
            char b = _blockBackgroundChar;

            _blockRoadSprites = new Dictionary<char, char[,]>
            {
                { '─', new char[3, 3] { {b,b,b}, {r,r,r}, {b,b,b} } },
                { '│', new char[3, 3] { {b,r,b}, {b,r,b}, {b,r,b} } },
                { '┌', new char[3, 3] { {b,b,b}, {b,r,r}, {b,r,b} } },
                { '┐', new char[3, 3] { {b,b,b}, {r,r,b}, {b,r,b} } },
                { '└', new char[3, 3] { {b,r,b}, {b,r,r}, {b,b,b} } },
                { '┘', new char[3, 3] { {b,r,b}, {r,r,b}, {b,b,b} } },
                { '┬', new char[3, 3] { {b,b,b}, {r,r,r}, {b,r,b} } },
                { '┴', new char[3, 3] { {b,r,b}, {r,r,r}, {b,b,b} } },
                { '├', new char[3, 3] { {b,r,b}, {b,r,r}, {b,r,b} } },
                { '┤', new char[3, 3] { {b,r,b}, {r,r,b}, {b,r,b} } },
                { '┼', new char[3, 3] { {b,r,b}, {r,r,r}, {b,r,b} } },
                { ' ', new char[3, 3] { {b,b,b}, {b,b,b}, {b,b,b} } },
                { '?', new char[3, 3] { {b,b,b}, {b,'?',b}, {b,b,b} } }
            };
        }

        public char[,] ConvertToDetailedSymbolMap()
        {
            InitializeBlockRoadSprites();

            if (MapWidth == 0 || MapHeight == 0)
            {
                return new char[0, 0];
            }

            char[,] baseSymbolMap = ConvertToSymbolMap();

            int outputHeight = MapHeight * 3;
            int outputWidth = MapWidth * 3;
            char[,] detailedMap = new char[outputHeight, outputWidth];

            for (int rIndex = 0; rIndex < MapHeight; rIndex++)
            {
                for (int cIndex = 0; cIndex < MapWidth; cIndex++)
                {
                    char baseSymbol = baseSymbolMap[rIndex, cIndex];
                    char[,] currentSprite;

                    if (!_blockRoadSprites.TryGetValue(baseSymbol, out currentSprite))
                    {
                        currentSprite = _blockRoadSprites[' '];
                    }

                    for (int sr = 0; sr < 3; sr++)
                    {
                        for (int sc = 0; sc < 3; sc++)
                        {
                            if ((rIndex * 3 + sr < outputHeight) && (cIndex * 3 + sc < outputWidth))
                            {
                                detailedMap[rIndex * 3 + sr, cIndex * 3 + sc] = currentSprite[sr, sc];
                            }
                        }
                    }
                }
            }
            return detailedMap;
        }

        private static Dictionary<char, Rectangle> _tileSpriteMappings;

        private static void InitializeTileSpriteMappings()
        {
            if (_tileSpriteMappings == null)
            {
                _tileSpriteMappings = new Dictionary<char, Rectangle> {
                    { ' ', new Rectangle(256, 256, 256, 256) },
                    { '─', new Rectangle(0, 0, 256, 256) },
                    { '│', new Rectangle(768, 0, 256, 256) },
                    { '┌', new Rectangle(512, 256, 256, 256) },
                    { '┐', new Rectangle(768, 256, 256, 256) },
                    { '└', new Rectangle(256, 0, 256, 256) },
                    { '┘', new Rectangle(0, 256, 256, 256) },
                    { '├', new Rectangle(0, 512, 256, 256) },
                    { '┤', new Rectangle(768, 512, 256, 256) },
                    { '┬', new Rectangle(512, 512, 256, 256) },
                    { '┴', new Rectangle(256, 512, 256, 256) },
                    { '┼', new Rectangle(512, 0, 256, 256) },
                    { '?', new Rectangle(256, 256, 256, 256) }
                };
            }
        }
        public void ConvertToImageSpriteMap(string outputImageFilePath, string spriteSheetFilePath)
        {
            InitializeTileSpriteMappings();
            char[,] symbolMapGrid = ConvertToSymbolMap();
            int numRows = symbolMapGrid.GetLength(0);
            int numCols = symbolMapGrid.GetLength(1);
            int tilePixelWidth = 256;
            int tilePixelHeight = 256;

            if (numRows == 0 || numCols == 0)
            {
                Console.WriteLine("Карта пуста, изображение не будет сгенерировано.");
                return;
            }

            string fullSpriteSheetPath = spriteSheetFilePath;
            if (!Path.IsPathRooted(spriteSheetFilePath))
            {
                fullSpriteSheetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, spriteSheetFilePath);
            }

            if (!File.Exists(fullSpriteSheetPath))
            {
                Console.WriteLine($"Файл спрайт-листа не найден: {fullSpriteSheetPath}");
                return;
            }

            try
            {
                using (Bitmap roadSpriteSheet = new Bitmap(fullSpriteSheetPath))
                using (Bitmap outputImage = new Bitmap(numCols * tilePixelWidth, numRows * tilePixelHeight))
                using (Graphics graphicsContext = Graphics.FromImage(outputImage))
                {
                    graphicsContext.Clear(Color.LightGreen);

                    for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
                    {
                        for (int colIndex = 0; colIndex < numCols; colIndex++)
                        {
                            char roadSymbol = symbolMapGrid[rowIndex, colIndex];
                            Rectangle sourceSpriteRectangle = _tileSpriteMappings.GetValueOrDefault(roadSymbol, _tileSpriteMappings[' ']);
                            Rectangle destinationRectangle = new Rectangle(colIndex * tilePixelWidth, rowIndex * tilePixelHeight, tilePixelWidth, tilePixelHeight);
                            graphicsContext.DrawImage(roadSpriteSheet, destinationRectangle, sourceSpriteRectangle, GraphicsUnit.Pixel);
                        }
                    }
                    outputImage.Save(outputImageFilePath, ImageFormat.Png);
                    Console.WriteLine($"Карта спрайтов изображений сохранена в {outputImageFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации карты спрайтов изображений: {ex.Message}");
                Console.WriteLine($"Проверьте путь к спрайт-листу: {fullSpriteSheetPath}");
                Console.WriteLine("Убедитесь, что файл спрайт-листа существует и System.Drawing.Common (NuGet) подключен и настроен для вашей ОС (для Linux/macOS может потребоваться libgdiplus).");
            }
        }
    }
}