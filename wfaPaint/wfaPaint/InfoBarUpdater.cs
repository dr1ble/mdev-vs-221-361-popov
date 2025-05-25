using System;
using System.Drawing;
using System.Windows.Forms;

namespace wfaPaint
{
    // Класс InfoBarUpdater отвечает за обновление информационной строки (ToolStripLabel)
    // актуальными данными: размером изображения, позицией курсора мыши и цветом пикселя под курсором.
    public class InfoBarUpdater
    {
        // --- Приватные поля ---

        // Ссылка на родительскую форму (не используется напрямую в текущей логике, но может быть полезна для расширения).
        private readonly Form parentForm;
        // PictureBox, размеры которого используются для отображения размера изображения.
        private PictureBox pictureBox;
        // Менеджер холста, из которого берется информация о Bitmap (для размера и цвета пикселя).
        private CanvasController canvasController; // Изменено с DrawingManager на CanvasController
        // ToolStripLabel, текст которого будет обновляться.
        private ToolStripLabel infoLabel;
        // Последняя зафиксированная позиция курсора мыши.
        private Point currentMousePosition;

        // --- Конструктор ---

        // Конструктор инициализирует все необходимые зависимости и подписывается на события.
        public InfoBarUpdater(Form form, CanvasController manager, PictureBox pictureBox, ToolStripLabel label)
        {
            parentForm = form;
            this.pictureBox = pictureBox;
            this.infoLabel = label;
            AttachToCanvasController(manager); // Привязка к менеджеру холста.

            // Подписка на событие изменения размера PictureBox для обновления информации о размере.
            this.pictureBox.Resize += (s, e) => Update();
        }

        // --- Публичные методы ---

        // AttachToDrawingManager позволяет переключить InfoBarUpdater на новый экземпляр CanvasController.
        // Это полезно, например, при создании нового изображения (когда создается новый CanvasController).
        // Отписывается от событий старого менеджера и подписывается на события нового.
        public void AttachToCanvasController(CanvasController newManager)
        {
            // Отписка от события BitmapChanged предыдущего менеджера, если он был.
            if (canvasController != null)
            {
                canvasController.BitmapChanged -= OnBitmapChanged;
            }

            canvasController = newManager; // Сохранение ссылки на новый менеджер.

            // Подписка на событие BitmapChanged нового менеджера.
            if (canvasController != null)
            {
                canvasController.BitmapChanged += OnBitmapChanged;
            }

            Update(); // Немедленное обновление информации при смене менеджера.
        }

        // OnBitmapChanged - обработчик события изменения Bitmap в CanvasController.
        // Вызывает обновление информационной строки.
        private void OnBitmapChanged(object? sender, EventArgs e)
        {
            Update();
        }

        // UpdateMousePosition вызывается извне (например, из PxImage_MouseMove)
        // для передачи актуальных координат курсора мыши.
        public void UpdateMousePosition(Point location)
        {
            currentMousePosition = location;
            Update(); // Обновление информации после изменения позиции мыши.
        }

        // --- Приватные методы ---

        // Update - основной метод, который формирует и отображает текст в infoLabel.
        // Собирает информацию о размере изображения, позиции мыши и цвете пикселя.
        private void Update()
        {
            // Проверка на null для предотвращения ошибок, если компоненты еще не инициализированы.
            if (infoLabel == null || pictureBox == null)
                return;

            // Формирование текста о размере изображения (берется из размеров PictureBox).
            var sizeText = $"Размер изображение: {pictureBox.Width} x {pictureBox.Height}";
            // Формирование текста о текущей позиции мыши.
            var mouseText = $"Позиция мыши: ({currentMousePosition.X}, {currentMousePosition.Y})";

            // Получение и форматирование информации о цвете пикселя под курсором.
            string colorText = "Цвет: N/A"; // Значение по умолчанию, если цвет определить не удалось.
            // Проверяем наличие холста и нахождение курсора в его пределах.
            if (canvasController != null && canvasController.Bitmap != null &&
                currentMousePosition.X >= 0 && currentMousePosition.X < canvasController.Bitmap.Width &&
                currentMousePosition.Y >= 0 && currentMousePosition.Y < canvasController.Bitmap.Height)
            {
                // Получение цвета пикселя с холста.
                Color pixelColor = canvasController.Bitmap.GetPixel(currentMousePosition.X, currentMousePosition.Y);
                // Форматирование цвета в HEX-виде (например, #RRGGBB).
                colorText = $"Цвет: #{pixelColor.R:X2}{pixelColor.G:X2}{pixelColor.B:X2}";
            }

            // Обновление текста в infoLabel.
            infoLabel.Text = $"{sizeText} | {mouseText} | {colorText}";
        }
    }
}