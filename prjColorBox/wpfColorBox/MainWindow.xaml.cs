// MainWindow.xaml.cs
using prjColorBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace wpfColorBox
{
    // partial class MainWindow: Означает, что определение этого класса может быть разделено на несколько файлов.
    // В данном случае, вторая часть находится в MainWindow.xaml (сгенерированном из XAML разметки).
    // MainWindow наследуется от Window, что делает его окном WPF.
    public partial class MainWindow : Window
    {
        // --- Константы и поля класса ---

        // Константы, определяющие параметры игровой сетки.
        private const int GRID_ROWS = 7;    // Количество строк в игровой сетке.
        private const int GRID_COLS = 12;   // Количество столбцов в игровой сетке.
        private const int NUM_COLORS = 5;   // Количество различных цветов, генерируемых на уровне.
                                            // Должно соответствовать количеству доступных определений в _wpfColorMapping.

        // Объекты, отвечающие за логику игры и управление данными.
        private ColorMapGenerator _mapGenerator;    // Экземпляр класса для генерации новых игровых карт.
        private MapData _currentMapData;            // Хранит все данные текущего игрового уровня (сетка, количество цветов, правильный порядок угадывания).
        private List<int> _expectedColorOrder;      // Список ID цветов в том порядке, в котором их должен угадать игрок (от самого частого к самому редкому).

        // ObservableCollection<T>: Специализированная коллекция, которая автоматически уведомляет UI (через привязку данных)
        // об изменениях (добавлении, удалении, замене элементов).
        public ObservableCollection<CellViewModel> GameCells { get; set; } // Коллекция ViewModel'ей для каждой ячейки игровой сетки.
        public ObservableCollection<ColorButtonViewModel> ColorChoiceButtons { get; set; } // Коллекция ViewModel'ей для кнопок выбора цвета.

        // Словарь для сопоставления числовых ID цветов с их визуальным представлением в WPF:
        // - UiBrush: Кисть (Brush) для заливки фона.
        // - Name: Русское название цвета для отображения на кнопке.
        // - PreferredForegroundBrush: Предпочтительный цвет текста для этой фоновой кисти (для контрастности).
        //   (В текущей версии цвет текста кнопок всегда черный, так что это поле не используется для установки цвета текста кнопок).
        private Dictionary<int, (Brush UiBrush, string Name, Brush PreferredForegroundBrush)> _wpfColorMapping =
            new Dictionary<int, (Brush, string, Brush)>
        {
            { 1, (Brushes.Red,     "Красный", Brushes.White) },
            { 2, (Brushes.Green,   "Зеленый", Brushes.White) },
            { 3, (Brushes.Blue,    "Синий", Brushes.White) },
            { 4, (Brushes.Yellow,  "Желтый", Brushes.Black) }, // Для желтого фона черный текст обычно лучше.
            { 5, (Brushes.Orange,  "Оранжевый", Brushes.Black) },
            { 6, (Brushes.Purple,  "Фиолетовый", Brushes.White) }
        };
        // Кисть по умолчанию для ячеек, которые не содержат активного игрового цвета или для фона.
        private Brush _defaultCellBrush = Brushes.LightGray;

        // Конструктор главного окна. Вызывается при создании экземпляра окна.
        public MainWindow()
        {
            InitializeComponent(); // Обязательный вызов. Инициализирует компоненты, определенные в XAML (MainWindow.xaml).

            _mapGenerator = new ColorMapGenerator(); // Создаем экземпляр генератора карт.
            GameCells = new ObservableCollection<CellViewModel>(); // Инициализируем коллекцию для ячеек.
            ColorChoiceButtons = new ObservableCollection<ColorButtonViewModel>(); // Инициализируем коллекцию для кнопок.

            // Устанавливаем источники данных для ItemsControl'ов в XAML.
            // GameGridItemsControl будет отображать элементы из коллекции GameCells.
            GameGridItemsControl.ItemsSource = GameCells;
            // ColorButtonsItemsControl будет отображать элементы из коллекции ColorChoiceButtons.
            ColorButtonsItemsControl.ItemsSource = ColorChoiceButtons;

            // Подписываемся на событие Loaded окна. Это событие возникает, когда окно полностью загружено и готово к отображению.
            this.Loaded += MainWindow_Loaded;
        }

        // Обработчик события Loaded для окна. Вызывается один раз при первой загрузке окна.
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartNewLevel(); // Запускаем первый игровой уровень автоматически.
        }

        // Обработчик события Click для кнопки "Начать уровень" / "Перезапустить уровень".
        private void BtnStartLevel_Click(object sender, RoutedEventArgs e)
        {
            // Предотвращаем повторный запуск, если идет процесс автоматического перезапуска (кнопка неактивна).
            if (!btnStartLevel.IsEnabled) return;
            StartNewLevel(); // Запускаем/перезапускаем уровень.
        }

        // Основной метод для начала или перезапуска игрового уровня.
        private void StartNewLevel()
        {
            // Перед началом нового уровня убеждаемся, что кнопка "Старт/Перезапуск" активна.
            btnStartLevel.IsEnabled = true;

            try
            {
                // Генерируем новую игровую карту с заданными параметрами.
                _currentMapData = _mapGenerator.GenerateMap(GRID_ROWS, GRID_COLS, NUM_COLORS);
            }
            catch (ArgumentException ex) // Ловим исключение, если параметры генерации некорректны.
            {
                MessageBox.Show($"Ошибка генерации карты: {ex.Message}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                lblStatus.Text = "Ошибка! Проверьте настройки.";
                btnStartLevel.Content = "Начать уровень"; // Возвращаем исходный текст кнопки.
                return; // Прерываем выполнение, так как уровень не может быть начат.
            }

            // Получаем из сгенерированных данных отсортированный список ID цветов (по убыванию частоты).
            _expectedColorOrder = new List<int>(_currentMapData.SortedColorIDsByFrequency);

            // Находим панель UniformGrid внутри GameGridItemsControl, чтобы установить ей количество строк и столбцов.
            var uniformGridPanel = FindVisualChild<UniformGrid>(GameGridItemsControl);
            if (uniformGridPanel != null)
            {
                uniformGridPanel.Rows = GRID_ROWS;
                uniformGridPanel.Columns = GRID_COLS;
            }

            PopulateGameCells();    // Заполняем коллекцию GameCells данными с новой карты.
            SetupColorButtonsWPF(); // Создаем и настраиваем кнопки выбора цвета для нового уровня.

            // Обновляем текстовые элементы UI.
            lblStatus.Text = "Уровень начат! Найдите самый частый цвет.";
            btnStartLevel.Content = "Перезапустить уровень";
        }

        // Метод для заполнения коллекции GameCells ViewModel'ями ячеек на основе данных текущей карты.
        private void PopulateGameCells()
        {
            GameCells.Clear(); // Очищаем коллекцию от ячеек предыдущего уровня.
            if (_currentMapData == null) return; // Если данных карты нет, ничего не делаем.

            // Проходим по каждой ячейке сгенерированной карты.
            for (int r = 0; r < GRID_ROWS; r++)
            {
                for (int c = 0; c < GRID_COLS; c++)
                {
                    int colorId = _currentMapData.Grid[r][c]; // Получаем ID цвета для текущей ячейки.
                    // Создаем новую ViewModel для ячейки.
                    var cellVM = new CellViewModel
                    {
                        ColorId = colorId, // Сохраняем ID цвета.
                        IsVisible = true   // По умолчанию делаем ячейку видимой (логически).
                    };
                    // Если ID цвета не 0 (не пустая ячейка) и для этого ID есть определение в словаре цветов:
                    if (colorId != 0 && _wpfColorMapping.ContainsKey(colorId))
                    {
                        cellVM.BackgroundColor = _wpfColorMapping[colorId].UiBrush; // Устанавливаем цвет фона.
                    }
                    else // Если ячейка пустая или для ID нет цвета:
                    {
                        cellVM.BackgroundColor = _defaultCellBrush; // Устанавливаем цвет фона по умолчанию.
                        cellVM.IsVisible = false; // Делаем такие ячейки невидимыми в UI.
                    }
                    GameCells.Add(cellVM); // Добавляем ViewModel ячейки в коллекцию (UI обновится).
                }
            }
        }

        // Вложенный класс ViewModel для кнопок выбора цвета.
        // Реализует INotifyPropertyChanged для обновления свойства IsEnabled кнопки в UI.
        public class ColorButtonViewModel : INotifyPropertyChanged
        {
            public int ColorId { get; set; }        // ID цвета, который представляет кнопка.
            public string Name { get; set; }        // Текст на кнопке (название цвета).
            public Brush UiBrush { get; set; }       // Цвет фона кнопки.
            public Brush ForegroundBrush { get; set; } // Цвет текста кнопки (в текущей версии всегда черный).

            private bool _isEnabled = true; // Поле для хранения состояния активности кнопки.
            public bool IsEnabled           // Свойство для привязки к IsEnabled кнопки в XAML.
            {
                get => _isEnabled;
                set
                {
                    if (_isEnabled != value)
                    {
                        _isEnabled = value;
                        OnPropertyChanged(nameof(IsEnabled));
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged; // Событие для уведомления UI.
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Метод для создания и настройки кнопок выбора цвета.
        private void SetupColorButtonsWPF()
        {
            ColorChoiceButtons.Clear(); // Очищаем коллекцию от кнопок предыдущего уровня.
            // Если нет данных о цветах на карте, скрываем панель с кнопками.
            if (_currentMapData == null || _currentMapData.ColorCounts == null)
            {
                ColorButtonsItemsControl.Visibility = Visibility.Collapsed;
                return;
            }
            ColorButtonsItemsControl.Visibility = Visibility.Visible; // Делаем панель видимой.

            // Получаем список ID всех уникальных цветов, присутствующих на текущей карте,
            // и сортируем их по ID для единообразного порядка кнопок.
            var colorsInThisLevel = _currentMapData.ColorCounts.Keys.OrderBy(k => k);

            // Для каждого ID цвета из списка создаем ViewModel кнопки.
            foreach (int colorId in colorsInThisLevel)
            {
                if (_wpfColorMapping.ContainsKey(colorId)) // Убеждаемся, что для ID есть определение цвета.
                {
                    var mapping = _wpfColorMapping[colorId]; // Получаем информацию о цвете.
                    ColorChoiceButtons.Add(new ColorButtonViewModel
                    {
                        ColorId = colorId,
                        Name = mapping.Name,
                        UiBrush = mapping.UiBrush,
                        ForegroundBrush = Brushes.Black, // Устанавливаем цвет текста кнопки всегда черным.
                        IsEnabled = true                 // Новые кнопки всегда активны.
                    });
                }
            }
        }

        // Обработчик события Click для одной из кнопок выбора цвета.
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, можно ли сейчас обрабатывать клики.
            if (_currentMapData == null || _expectedColorOrder == null || !_expectedColorOrder.Any()) return;
            if (!btnStartLevel.IsEnabled) return; // Если идет авто-перезапуск, игнорируем клики.

            Button clickedButton = sender as Button; // Получаем ссылку на нажатую кнопку.
            if (clickedButton == null || clickedButton.Tag == null) return; // Проверка на корректность.

            int chosenColorId = (int)clickedButton.Tag; // Извлекаем ID выбранного цвета из Tag кнопки.
            // Находим соответствующую ViewModel для этой кнопки, чтобы изменить ее свойство IsEnabled.
            var buttonViewModel = ColorChoiceButtons.FirstOrDefault(b => b.ColorId == chosenColorId);

            // Сравниваем выбранный ID с первым ID в списке ожидаемых цветов (самый частый из оставшихся).
            if (chosenColorId == _expectedColorOrder.First())
            {
                // Правильный выбор!
                lblStatus.Text = $"Верно! Цвет '{_wpfColorMapping[chosenColorId].Name}' удален.";
                _expectedColorOrder.RemoveAt(0); // Удаляем угаданный цвет из списка ожидаемых.

                if (buttonViewModel != null) buttonViewModel.IsEnabled = false; // Делаем ViewModel кнопки (и саму кнопку) неактивной.

                // Скрываем все ячейки этого цвета на игровом поле, обновляя их ViewModel'и.
                foreach (var cellVM in GameCells.Where(cell => cell.ColorId == chosenColorId))
                {
                    cellVM.IsVisible = false;
                }

                // Проверяем, остались ли еще цвета для угадывания.
                if (!_expectedColorOrder.Any()) // Если список пуст, все цвета угаданы.
                {
                    // Уровень пройден!
                    lblStatus.Text = "Уровень пройден! Загрузка следующего уровня...";

                    // Временно делаем все кнопки выбора цвета и кнопку "Перезапуск" неактивными.
                    foreach (var btnVM in ColorChoiceButtons) btnVM.IsEnabled = false;
                    btnStartLevel.IsEnabled = false;

                    // Запускаем новый уровень асинхронно с небольшой задержкой, чтобы игрок успел прочитать сообщение.
                    Dispatcher.InvokeAsync(async () =>
                    {
                        await Task.Delay(1500); // Пауза 1.5 секунды.
                        StartNewLevel();       // Запускаем новый уровень.
                    });
                }
                else // Если еще есть цвета для угадывания.
                {
                    lblStatus.Text += $" Теперь найдите следующий самый частый цвет.";
                }
            }
            else
            {
                // Неправильный выбор.
                lblStatus.Text = $"Неверно. Попробуйте еще раз. Ищем самый частый из оставшихся.";
            }
        }

        // Вспомогательный рекурсивный метод для поиска дочернего элемента определенного типа (T)
        // внутри визуального дерева родительского элемента (parent).
        // Используется, например, для нахождения UniformGrid внутри ItemsControl.
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null; // Если родитель null, ничего не найдено.
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) // Проходим по всем прямым потомкам.
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i); // Получаем очередного потомка.
                if (child != null && child is T) // Если потомок не null и имеет нужный тип T.
                    return (T)child; // Возвращаем найденного потомка.
                else
                {
                    // Если прямой потомок не того типа, рекурсивно ищем в его потомках.
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null) // Если в глубине найден элемент нужного типа.
                        return childOfChild;  // Возвращаем его.
                }
            }
            return null; // Если ничего не найдено после обхода всех потомков.
        }
    }

    // Класс ViewModel для одной ячейки игровой сетки.
    // Реализует INotifyPropertyChanged для уведомления UI об изменениях свойств.
    public class CellViewModel : INotifyPropertyChanged
    {
        private Brush _backgroundColor; // Цвет фона ячейки.
        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set { if (_backgroundColor != value) { _backgroundColor = value; OnPropertyChanged(nameof(BackgroundColor)); } }
        }

        private bool _isVisible = true; // Видимость ячейки.
        public bool IsVisible
        {
            get => _isVisible;
            set { if (_isVisible != value) { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); } }
        }

        public int ColorId { get; set; } // ID цвета в ячейке.

        public event PropertyChangedEventHandler PropertyChanged; // Событие изменения свойства.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}