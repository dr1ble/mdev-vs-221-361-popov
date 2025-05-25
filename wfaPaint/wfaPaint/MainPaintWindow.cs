using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace wfaPaint
{
    // Основной класс формы приложения, наследуется от Form.
    public partial class MainPaintWindow : Form
    {
        // Менеджеры, инкапсулирующие различную логику приложения.
        private CanvasController canvasController;    // Отвечает за холст и базовые операции рисования.
        private ActiveToolManager activeToolManager;  // Управляет текущим выбранным инструментом и режимом работы.
        private ClipboardManager clipboardManager;  // Реализует взаимодействие с буфером обмена.
        private SelectionManager selectionManager;  // Обрабатывает создание, изменение и применение выделенных областей.
        private InfoBarUpdater infoBarUpdater;    // Обновляет информацию о состоянии (размер изображения, позиция мыши).

        // Переменные состояния для текущей операции рисования или выделения.
        private Point startDrawLocation;        // Координаты начальной точки текущей операции (нажатия мыши).
        private bool isCurrentlyDrawing = false; // Флаг: true, если идет процесс рисования или создания выделения.

        // Поля для оптимизации (throttling) отрисовки сложных фигур.
        private DateTime lastShapeDrawTime = DateTime.MinValue; // Время последней отрисовки предпросмотра фигуры.
        private const int ShapeDrawIntervalMs = 30; // Минимальный интервал для обновления предпросмотра фигур (мс).
        private Point lastKnownMousePosition; // Последняя зафиксированная позиция курсора мыши.

        public MainPaintWindow()
        {
            InitializeComponent(); // Инициализация компонентов формы, созданных в дизайнере.

            // Создание и настройка экземпляров менеджеров.
            canvasController = new CanvasController(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, paSelectColorRed.BackColor, 10);
            selectionManager = new SelectionManager();
            activeToolManager = new ActiveToolManager(canvasController, selectionManager, pxImage);
            clipboardManager = new ClipboardManager(canvasController, selectionManager);
            infoBarUpdater = new InfoBarUpdater(this, canvasController, pxImage, toolStripLabelImageInfo);

            InitializeUI(); // Первичная настройка элементов интерфейса и их обработчиков.
            HookEvents();   // Подписка на события элементов управления, в основном для pxImage.
        }

        // Настройка пользовательского интерфейса: привязка обработчиков к кнопкам и другим элементам.
        private void InitializeUI()
        {
            // Обработчики для панелей выбора цвета.
            paSelectColorRed.Click += (s, e) => canvasController.ChangeColor(paSelectColorRed.BackColor);
            paSelectColorGreen.Click += (s, e) => canvasController.ChangeColor(paSelectColorGreen.BackColor);
            paSelectColorYellow.Click += (s, e) => canvasController.ChangeColor(paSelectColorYellow.BackColor);
            paSelectColorBlack.Click += (s, e) => canvasController.ChangeColor(paSelectColorBlack.BackColor);
            paSelectColorOrange.Click += (s, e) => canvasController.ChangeColor(paSelectColorOrange.BackColor);
            paSelectColorCyan.Click += (s, e) => canvasController.ChangeColor(paSelectColorCyan.BackColor);
            paSelectColorPink.Click += (s, e) => canvasController.ChangeColor(paSelectColorPink.BackColor);
            paSelectColorBlue.Click += (s, e) => canvasController.ChangeColor(paSelectColorBlue.BackColor);

            // Обработчики для кнопок выбора инструментов.
            buSelectPen.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Pencil);
            buSelectLine.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Line);
            buSelectEllipse.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Ellipse);
            buSelectRectangle.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Rectangle);
            buSelectTriangle.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Triangle);
            buSelectArrow.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Arrow);
            buSelectStar.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Star);
            buSelectHexagon.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Hexagon);
            buModeSelect.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Select);
            buEraser.Click += (s, e) => activeToolManager.SetActiveTool(ActiveToolManager.MyDrawMode.Eraser);

            // Обработчики для кнопок файловых операций.
            buSaveAsToFile.Click += BuSaveAsToFile_Click;
            buLoadFromFile.Click += BuLoadFromFile_Click;
            buNewImage.Click += BuNewImage_Click;

            // Настройка элемента для выбора толщины пера.
            trPenSize.Minimum = 1;
            trPenSize.Maximum = 12;
            trPenSize.Value = (int)canvasController.Pen.Width;
            trPenSize.ValueChanged += (s, e) => canvasController.ChangePenWidth(trPenSize.Value);
        }

        // Подписка на события PictureBox, необходимые для рисования.
        private void HookEvents()
        {
            pxImage.MouseDown += PxImage_MouseDown;
            pxImage.MouseMove += PxImage_MouseMove;
            pxImage.MouseUp += PxImage_MouseUp;
            pxImage.Paint += PxImage_Paint; // Основной метод для отрисовки.
        }

        // Метод отрисовки содержимого PictureBox.
        private void PxImage_Paint(object? sender, PaintEventArgs e)
        {
            // Отрисовка основного холста.
            if (canvasController.Bitmap != null)
            {
                e.Graphics.DrawImage(canvasController.Bitmap, 0, 0);
            }

            // Отрисовка выделенной области (содержимого и рамки), если она активна.
            if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Select && selectionManager.HasSelection)
            {
                if (selectionManager.SelectedBitmap != null)
                {
                    e.Graphics.DrawImage(selectionManager.SelectedBitmap, selectionManager.SelectedArea);
                }
                // Рисование пунктирной рамки.
                using Pen dashed = new(Color.Black) { DashStyle = DashStyle.Dash };
                e.Graphics.DrawRectangle(dashed, selectionManager.SelectedArea);
            }
        }

        // Создание нового изображения.
        private void BuNewImage_Click(object? sender, EventArgs e)
        {
            Color penColor = canvasController.Pen.Color;
            int penWidth = (int)canvasController.Pen.Width;
            canvasController = new CanvasController(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, penColor, penWidth);

            trPenSize.Value = penWidth;
            infoBarUpdater.AttachToCanvasController(canvasController);
            selectionManager.ClearSelection();
            pxImage.Invalidate();
        }

        // Сохранение изображения в файл.
        private void BuSaveAsToFile_Click(object? sender, EventArgs e)
        {
            using SaveFileDialog dialog = new() { Filter = "PNG Files(*.PNG)|*.PNG" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (canvasController.Bitmap != null)
                {
                    canvasController.Bitmap.Save(dialog.FileName);
                }
            }
        }

        // Загрузка изображения из файла.
        private void BuLoadFromFile_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new() { Filter = "PNG Files(*.PNG)|*.PNG" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                canvasController.LoadImage(dialog.FileName);
                infoBarUpdater.AttachToCanvasController(canvasController);
                selectionManager.ClearSelection();
                pxImage.Invalidate();
            }
        }

        // Обработка нажатия кнопки мыши на холсте.
        private void PxImage_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startDrawLocation = e.Location;
                lastKnownMousePosition = e.Location;

                if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Select) // Режим выделения.
                {
                    if (selectionManager.HasSelection)
                    {
                        if (selectionManager.SelectedArea.Contains(e.Location)) // Клик внутри существующего выделения.
                        {
                            selectionManager.StartDragging(e.Location); // Начало перетаскивания.
                        }
                        else // Клик мимо существующего выделения.
                        {
                            // "Впечатывание" старого выделения и начало нового.
                            if (selectionManager.SelectedBitmap != null && canvasController.Bitmap != null)
                            {
                                using (Graphics g = Graphics.FromImage(canvasController.Bitmap))
                                {
                                    g.CompositingMode = CompositingMode.SourceOver;
                                    g.DrawImage(selectionManager.SelectedBitmap, selectionManager.SelectedArea);
                                }
                            }
                            selectionManager.ClearSelection();
                            pxImage.Invalidate(); // Обновление для отображения "впечатанного" и скрытия старой рамки.

                            isCurrentlyDrawing = true; // Начало создания новой рамки.
                            canvasController.BackupImage();
                            selectionManager.StartSelection(startDrawLocation);
                        }
                    }
                    else // Выделения не было, начало нового.
                    {
                        isCurrentlyDrawing = true;
                        canvasController.BackupImage();
                        selectionManager.StartSelection(startDrawLocation);
                    }
                }
                else // Режимы рисования.
                {
                    isCurrentlyDrawing = true;
                    canvasController.BackupImage(); // Бэкап для предпросмотра или отмены.

                    if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Eraser) // Режим ластика.
                    {
                        if (canvasController.Graphics != null)
                        {
                            // Установка режима прямой замены пикселей для стирания.
                            canvasController.Graphics.CompositingMode = CompositingMode.SourceCopy;
                        }
                    }
                }
            }
        }

        // Обработка движения мыши по холсту с зажатой кнопкой.
        private void PxImage_MouseMove(object? sender, MouseEventArgs e)
        {
            infoBarUpdater.UpdateMousePosition(e.Location);
            lastKnownMousePosition = e.Location;

            if (e.Button != MouseButtons.Left) return;

            if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Select) // Режим выделения.
            {
                if (selectionManager.IsDragging) // Идет перетаскивание.
                {
                    selectionManager.DragTo(e.Location);
                    pxImage.Invalidate();
                }
                else if (isCurrentlyDrawing) // Идет создание новой рамки.
                {
                    canvasController.RestoreBackup(); // Восстановление холста для корректного UpdateSelection.
                    selectionManager.UpdateSelection(canvasController, e.Location);
                    pxImage.Invalidate();
                }
            }
            else // Режимы рисования.
            {
                if (!isCurrentlyDrawing) return;

                if (activeToolManager.CurrentMode != ActiveToolManager.MyDrawMode.Pencil &&
                    activeToolManager.CurrentMode != ActiveToolManager.MyDrawMode.Eraser) // Рисование фигур.
                {
                    // Ограничение частоты обновления для предпросмотра фигур.
                    if ((DateTime.Now - lastShapeDrawTime).TotalMilliseconds >= ShapeDrawIntervalMs)
                    {
                        canvasController.RestoreBackup();
                        ShapeDrawer.DrawFigure(canvasController.Graphics, canvasController.Pen, activeToolManager.CurrentMode, startDrawLocation, e.Location);
                        pxImage.Invalidate();
                        lastShapeDrawTime = DateTime.Now;
                    }
                }
                else if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Eraser) // Ластик.
                {
                    if (canvasController.Graphics != null)
                    {
                        using (Pen eraserPen = new Pen(Color.Transparent, canvasController.Pen.Width))
                        {
                            eraserPen.StartCap = eraserPen.EndCap = LineCap.Round;
                            ShapeDrawer.DrawLine(canvasController.Graphics, eraserPen, startDrawLocation, e.Location);
                        }
                        startDrawLocation = e.Location; // Обновление начальной точки для следующего сегмента.
                        pxImage.Invalidate();
                    }
                }
                else if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Pencil) // Карандаш.
                {
                    if (canvasController.Graphics != null)
                    {
                        ShapeDrawer.DrawLine(canvasController.Graphics, canvasController.Pen, startDrawLocation, e.Location);
                        startDrawLocation = e.Location; // Обновление начальной точки для следующего сегмента.
                        pxImage.Invalidate();
                    }
                }
            }
        }

        // Обработка отпускания кнопки мыши.
        private void PxImage_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Select) // Режим выделения.
                {
                    if (selectionManager.IsDragging) // Завершение перетаскивания.
                    {
                        selectionManager.FinishDragging(canvasController);
                    }
                    // Если isCurrentlyDrawing было true (создание рамки), то UpdateSelection в MouseMove
                    // уже подготовил SelectedBitmap и "вырезал" область. Рамка остается активной.
                }
                else // Режимы рисования.
                {
                    if (isCurrentlyDrawing) // Если операция рисования была активна.
                    {
                        if (activeToolManager.CurrentMode != ActiveToolManager.MyDrawMode.Pencil &&
                            activeToolManager.CurrentMode != ActiveToolManager.MyDrawMode.Eraser) // Фигуры.
                        {
                            // Финальная отрисовка фигуры с использованием последней известной позиции мыши.
                            canvasController.RestoreBackup();
                            ShapeDrawer.DrawFigure(canvasController.Graphics, canvasController.Pen, activeToolManager.CurrentMode, startDrawLocation, lastKnownMousePosition);
                        }

                        if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Eraser) // Ластик.
                        {
                            // Восстановление стандартного режима наложения.
                            if (canvasController.Graphics != null)
                            {
                                canvasController.Graphics.CompositingMode = CompositingMode.SourceOver;
                            }
                        }
                    }
                }

                isCurrentlyDrawing = false; // Сброс флага активности операции.
                pxImage.Invalidate(); // Финальная перерисовка.
            }

            // Дополнительный сброс CompositingMode для ластика, если операция была прервана.
            if (activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Eraser &&
                canvasController.Graphics != null &&
                canvasController.Graphics.CompositingMode == CompositingMode.SourceCopy &&
                !isCurrentlyDrawing)
            {
                canvasController.Graphics.CompositingMode = CompositingMode.SourceOver;
            }
        }

        // Обработка горячих клавиш.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V)) // Вставка.
            {
                clipboardManager.PasteFromClipboard(pxImage);
                return true;
            }

            if (keyData == (Keys.Control | Keys.C)) // Копирование.
            {
                if (selectionManager.HasSelection && activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Select)
                {
                    clipboardManager.CopySelectionToClipboard(); // Копирование выделенной области.
                }
                else
                {
                    // Копирование видимой части всего изображения.
                    if (canvasController.Bitmap != null && pxImage.Width > 0 && pxImage.Height > 0)
                    {
                        Bitmap visiblePartCopy = new Bitmap(pxImage.Width, pxImage.Height, canvasController.Bitmap.PixelFormat);
                        using (Graphics g = Graphics.FromImage(visiblePartCopy))
                        {
                            Rectangle sourceRect = new Rectangle(
                                0, 0,
                                Math.Min(pxImage.Width, canvasController.Bitmap.Width),
                                Math.Min(pxImage.Height, canvasController.Bitmap.Height)
                            );
                            Rectangle destRect = new Rectangle(0, 0, sourceRect.Width, sourceRect.Height);
                            g.DrawImage(canvasController.Bitmap, destRect, sourceRect, GraphicsUnit.Pixel);
                        }
                        Clipboard.SetImage(visiblePartCopy);
                    }
                }
                return true;
            }

            // Вырезание и удаление (требуют активного выделения и режима "Select").
            if (selectionManager.HasSelection && activeToolManager.CurrentMode == ActiveToolManager.MyDrawMode.Select)
            {
                if (keyData == (Keys.Control | Keys.X)) // Вырезать.
                {
                    clipboardManager.CopySelectionToClipboard();
                    selectionManager.DeleteSelection(canvasController);
                    pxImage.Invalidate();
                    return true;
                }

                if (keyData == Keys.Delete) // Удалить.
                {
                    selectionManager.DeleteSelection(canvasController);
                    pxImage.Invalidate();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData); // Передача необработанных клавиш дальше.
        }
    }
}