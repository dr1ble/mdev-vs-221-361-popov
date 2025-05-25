using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace wfaPaint
{
    // Класс ClipboardManager отвечает за операции копирования и вставки изображений
    // с использованием системного буфера обмена. Он взаимодействует с CanvasController
    // для доступа к холсту и с SelectionManager для работы с выделенными областями.
    internal class ClipboardManager
    {
        // Ссылка на CanvasController для доступа к основному изображению и его резервному копированию.
        private readonly CanvasController canvasController;
        // Ссылка на SelectionManager для получения данных о выделении при копировании
        // и для установки выделения при вставке.
        private readonly SelectionManager selectionManager;

        // Конструктор инициализирует класс необходимыми зависимостями.
        public ClipboardManager(CanvasController canvasController, SelectionManager selectionManager)
        {
            this.canvasController = canvasController;
            this.selectionManager = selectionManager;
        }

        // Копирует текущее содержимое selectionManager.SelectedBitmap в буфер обмена.
        // Выполняется, если есть активное и непустое выделение.
        public void CopySelectionToClipboard()
        {
            if (selectionManager.HasSelection && selectionManager.SelectedBitmap != null &&
                selectionManager.SelectedBitmap.Width > 0 && selectionManager.SelectedBitmap.Height > 0)
            {
                // Клонирование Bitmap необходимо для безопасной работы с буфером обмена,
                // так как оригинальный SelectedBitmap может быть изменен или удален.
                using (Bitmap bitmapToCopy = (Bitmap)selectionManager.SelectedBitmap.Clone())
                {
                    Clipboard.SetImage(bitmapToCopy); // Помещение изображения в буфер.
                }
            }
        }

        // Вставляет изображение из буфера обмена на холст, центрируя его.
        // После вставки вокруг изображения создается выделение.
        // PictureBox передается для вызова Invalidate() и обновления отображения.
        public void PasteFromClipboard(PictureBox pictureBox)
        {
            try // Обработка возможных ошибок при доступе к буферу.
            {
                IDataObject dataObject = Clipboard.GetDataObject(); // Получение данных из буфера.
                // Проверка наличия данных и их соответствия формату Bitmap.
                if (dataObject != null && dataObject.GetDataPresent(DataFormats.Bitmap))
                {
                    Image clipboardImage = (Image)dataObject.GetData(DataFormats.Bitmap); // Извлечение изображения.

                    if (clipboardImage != null && canvasController.Bitmap != null)
                    {
                        canvasController.BackupImage(); // Сохранение текущего состояния холста.

                        // Расчет координат для центрирования вставляемого изображения.
                        // Math.Max предотвращает отрицательные координаты, если изображение больше холста.
                        int insertX = (canvasController.Bitmap.Width - clipboardImage.Width) / 2;
                        int insertY = (canvasController.Bitmap.Height - clipboardImage.Height) / 2;
                        Point insertPoint = new Point(Math.Max(0, insertX), Math.Max(0, insertY));

                        // Отрисовка изображения на основном холсте.
                        using (Graphics g = Graphics.FromImage(canvasController.Bitmap))
                        {
                            g.DrawImage(clipboardImage, insertPoint);
                        }

                        // Установка выделения вокруг вставленного изображения.
                        // Создается новый Bitmap из clipboardImage для передачи в SelectionManager.
                        selectionManager.SetSelection(new Rectangle(insertPoint, clipboardImage.Size), new Bitmap(clipboardImage));

                        pictureBox.Invalidate(); // Обновление отображения PictureBox.
                    }
                }
            }
            catch (ExternalException ex) // Ошибка, часто связанная с доступом к буферу, занятому другим процессом.
            {
                MessageBox.Show("Ошибка доступа к буферу обмена: " + ex.Message, "Ошибка буфера", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Другие непредвиденные ошибки.
            {
                MessageBox.Show("Неизвестная ошибка при работе с буфером обмена: " + ex.Message, "Ошибка буфера", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}