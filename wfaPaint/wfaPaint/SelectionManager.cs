using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace wfaPaint
{
    // Класс SelectionManager управляет всеми аспектами выделения областей на холсте:
    // созданием, обновлением геометрии, хранением содержимого выделенной области,
    // операциями перетаскивания, удаления и программной установки выделения.
    public class SelectionManager
    {
        // --- Открытые свойства ---

        // SelectedArea определяет текущий прямоугольник выделения на холсте.
        public Rectangle SelectedArea { get; private set; }

        // SelectedBitmap хранит пиксели, находящиеся внутри SelectedArea.
        // Это изображение "вырезается" из основного холста при выделении.
        // Может быть null, если выделение отсутствует или имеет нулевые размеры.
        public Bitmap? SelectedBitmap { get; private set; }

        // IsDragging указывает, активен ли в данный момент процесс перетаскивания выделенной области.
        public bool IsDragging { get; private set; }

        // HasSelection возвращает true, если существует активное выделение (SelectedBitmap не null).
        // Для большей точности можно добавить проверку на ненулевые размеры SelectedArea.
        public bool HasSelection => SelectedBitmap != null;

        // --- Приватные поля ---

        // dragOffset хранит смещение курсора относительно угла выделения при начале перетаскивания.
        private Point dragOffset;
        // currentSelectionStartPoint - это точка, где пользователь нажал ЛКМ для начала текущей операции создания выделения.
        private Point currentSelectionStartPoint;

        // --- Конструктор ---
        public SelectionManager()
        {
            // При создании менеджера активного выделения нет.
            SelectedArea = Rectangle.Empty;
        }

        // --- Публичные методы управления выделением ---

        // StartSelection вызывается при начале новой операции выделения (обычно из PxImage_MouseDown).
        // Сохраняет начальную точку, инициализирует SelectedArea и сбрасывает предыдущее выделение.
        public void StartSelection(Point startPoint)
        {
            currentSelectionStartPoint = startPoint;
            SelectedArea = new Rectangle(startPoint, Size.Empty);
            IsDragging = false;
            SelectedBitmap?.Dispose();
            SelectedBitmap = null;
        }

        // UpdateSelection обновляет выделенную область во время движения мыши (при создании рамки).
        // Восстанавливает фон из бэкапа, копирует новую выделенную область в SelectedBitmap
        // и "вырезает" ее с основного холста для визуального эффекта "поднятой" области.
        public void UpdateSelection(CanvasController canvasController, Point currentMousePosition)
        {
            var newRect = GetRect(currentSelectionStartPoint, currentMousePosition);

            canvasController.RestoreBackup(); // Восстановление "чистого" фона перед операциями.

            SelectedArea = newRect;

            if (newRect.Width > 0 && newRect.Height > 0)
            {
                SelectedBitmap?.Dispose();
                SelectedBitmap = new Bitmap(newRect.Width, newRect.Height);
                using (Graphics gSelected = Graphics.FromImage(SelectedBitmap))
                {
                    // Копирование данных из основного холста в SelectedBitmap.
                    gSelected.DrawImage(canvasController.Bitmap,
                                      new Rectangle(0, 0, newRect.Width, newRect.Height),
                                      newRect,
                                      GraphicsUnit.Pixel);
                }

                // "Вырезание" области на основном холсте (делаем ее прозрачной).
                if (canvasController.Graphics != null && canvasController.Bitmap != null)
                {
                    using (Graphics gMain = Graphics.FromImage(canvasController.Bitmap))
                    {
                        using SolidBrush transparentBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));
                        gMain.CompositingMode = CompositingMode.SourceCopy; // Прямая замена пикселей.
                        gMain.FillRectangle(transparentBrush, newRect);
                        gMain.CompositingMode = CompositingMode.SourceOver; // Возврат стандартного режима.
                    }
                }
            }
            else
            {
                SelectedBitmap?.Dispose();
                SelectedBitmap = null;
            }
        }

        // StartDragging вызывается при нажатии ЛКМ внутри существующего выделения.
        // Рассчитывает смещение для корректного перетаскивания и устанавливает флаг IsDragging.
        public void StartDragging(Point mouseLocation)
        {
            if (!HasSelection) return;
            dragOffset = new Point(mouseLocation.X - SelectedArea.X, mouseLocation.Y - SelectedArea.Y);
            IsDragging = true;
        }

        // DragTo обновляет позицию SelectedArea во время перетаскивания.
        public void DragTo(Point mouseLocation)
        {
            if (IsDragging)
            {
                var newTopLeft = new Point(mouseLocation.X - dragOffset.X, mouseLocation.Y - dragOffset.Y);
                SelectedArea = new Rectangle(newTopLeft, SelectedArea.Size);
            }
        }

        // FinishDragging вызывается при отпускании ЛКМ после перетаскивания.
        // "Впечатывает" содержимое SelectedBitmap на основной холст в новой позиции.
        // Затем сбрасывает выделение.
        public void FinishDragging(CanvasController canvasController)
        {
            if (IsDragging && SelectedBitmap != null && canvasController.Bitmap != null && canvasController.Graphics != null)
            {
                using (Graphics g = Graphics.FromImage(canvasController.Bitmap))
                {
                    g.CompositingMode = CompositingMode.SourceOver; // Для правильного наложения с прозрачностью.
                    g.DrawImage(SelectedBitmap, SelectedArea);
                }
            }
            ClearSelection();
        }

        // ClearSelection сбрасывает все данные о текущем выделении.
        public void ClearSelection()
        {
            SelectedArea = Rectangle.Empty;
            SelectedBitmap?.Dispose();
            SelectedBitmap = null;
            IsDragging = false;
        }

        // DeleteSelection удаляет содержимое выделенной области с основного холста,
        // делая эту область прозрачной. Затем сбрасывает выделение.
        public void DeleteSelection(CanvasController canvasController)
        {
            if (SelectedArea != Rectangle.Empty && canvasController.Graphics != null && canvasController.Bitmap != null)
            {
                var originalMode = canvasController.Graphics.CompositingMode;
                canvasController.Graphics.CompositingMode = CompositingMode.SourceCopy;
                using (SolidBrush transparentBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
                {
                    canvasController.Graphics.FillRectangle(transparentBrush, SelectedArea);
                }
                canvasController.Graphics.CompositingMode = originalMode;

                ClearSelection();
            }
        }

        // SetSelection позволяет программно установить выделенную область и ее содержимое.
        // Используется, например, при вставке из буфера обмена.
        public void SetSelection(Rectangle area, Bitmap bitmap)
        {
            ClearSelection();
            currentSelectionStartPoint = area.Location;
            SelectedArea = area;
            SelectedBitmap?.Dispose();
            SelectedBitmap = (Bitmap)bitmap.Clone(); // Клонирование для управления временем жизни копии.
        }

        // GetRect - вспомогательный приватный метод для расчета прямоугольника
        // по двум диагональным точкам. Гарантирует корректные X, Y (верхний левый угол)
        // и положительные Width, Height.
        private Rectangle GetRect(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y)
            );
        }
    }
}