using System;
using System.Drawing.Imaging;   
using System.Drawing;            
using System.Windows.Forms;      
using System.Drawing.Drawing2D;

namespace wfaPaint
{
    // Класс CanvasController является центральным компонентом для управления холстом рисования.
    // Он отвечает за хранение основного изображения (Bitmap), предоставляет объект Graphics для рисования,
    // управляет текущим пером (Pen) и реализует механизм создания и восстановления резервной копии холста.
    public class CanvasController
    {
        // --- Открытые свойства ---

        // Bitmap представляет собой растровое изображение, которое служит холстом для рисования.
        // Свойство 'get' публичное, позволяя другим классам читать Bitmap (например, для отображения).
        // Свойство 'private set' означает, что ссылка на объект Bitmap может быть изменена только внутри этого класса.
        public Bitmap Bitmap { get; private set; }

        // Graphics - это объект, предоставляющий методы для рисования на поверхности, связанной с Bitmap.
        // Через него выполняются все операции рисования (линии, фигуры, текст, изображения).
        public Graphics Graphics { get; private set; }

        // Pen определяет атрибуты, используемые для рисования линий и контуров фигур,
        // такие как цвет, толщина, стиль конца линии и т.д.
        public Pen Pen { get; private set; }

        // Backup хранит резервную копию основного Bitmap. Это позволяет откатывать изменения
        // или использовать его для предпросмотра операций без изменения основного холста до фиксации.
        // Тип Bitmap? указывает, что это поле может содержать null (если бэкап еще не создан).
        public Bitmap? Backup { get; private set; }

        // --- События ---

        // Событие BitmapChanged генерируется, когда основной холст (Bitmap) изменяется,
        // например, при загрузке нового изображения или создании нового холста.
        // Другие компоненты приложения могут подписаться на это событие, чтобы обновить свое состояние.
        public event EventHandler? BitmapChanged;

        // --- Конструктор ---

        // Конструктор CanvasController. Вызывается при создании нового экземпляра.
        // Инициализирует холст с заданными размерами, начальным цветом и толщиной пера.
        public CanvasController(int width, int height, Color color, int penWidth)
        {
            // Создание нового объекта Bitmap указанных размеров.
            // PixelFormat.Format32bppArgb используется для поддержки альфа-канала (прозрачности).
            Bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            // Получение объекта Graphics, связанного с созданным Bitmap.
            Graphics = Graphics.FromImage(Bitmap);
            // Начальная очистка холста. Color.Transparent делает фон полностью прозрачным.
            Graphics.Clear(Color.Transparent);
            // Установка режима сглаживания (Anti-aliasing) для улучшения качества отрисовки
            // линий и краев фигур, делая их менее "зубчатыми".
            Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Создание объекта Pen с начальным цветом и толщиной.
            Pen = new Pen(color, penWidth);
            // Установка стиля концов линий, рисуемых этим пером, на скругленные.
            Pen.StartCap = Pen.EndCap = LineCap.Round;
        }

        // --- Публичные методы управления холстом ---

        // Создает резервную копию (клон) текущего состояния основного Bitmap.
        // Если уже существует предыдущий бэкап, он освобождается (Dispose).
        public void BackupImage()
        {
            Backup?.Dispose(); // Освобождение ресурсов предыдущего бэкапа, если он существовал.
            if (Bitmap != null) // Проверка, что основной холст не null.
            {
                // Создание полной, независимой копии текущего Bitmap.
                Backup = (Bitmap)Bitmap.Clone();
            }
            else
            {
                Backup = null; // Если основного холста нет, то и бэкапа быть не может.
            }
        }

        // Восстанавливает состояние основного Bitmap из ранее созданной резервной копии (Backup).
        // Текущие объекты Bitmap и Graphics основного холста освобождаются, и на их место
        // устанавливается клон резервной копии с новым объектом Graphics.
        public void RestoreBackup()
        {
            if (Backup == null) return; // Если резервной копии нет, операция невозможна.

            // Освобождение текущих графических ресурсов основного холста перед их заменой.
            Graphics?.Dispose(); // Оператор ?. безопасен, если Graphics равен null.
            Bitmap?.Dispose();   // Оператор ?. безопасен, если Bitmap равен null.

            // Важно восстанавливать из КЛОНА бэкапа. Это сохраняет сам объект Backup
            // неизменным, позволяя многократно вызывать RestoreBackup (если BackupImage не вызывался для обновления бэкапа).
            Bitmap = (Bitmap)Backup.Clone();
            Graphics = Graphics.FromImage(Bitmap); // Создание нового объекта Graphics для восстановленного Bitmap.
            Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Применение настроек сглаживания.
        }

        // Изменяет цвет текущего объекта Pen.
        public void ChangeColor(Color color) => Pen.Color = color;

        // Изменяет толщину линии текущего объекта Pen.
        public void ChangePenWidth(int width) => Pen.Width = width;

        // Загружает изображение из файла, полностью заменяя им текущий холст.
        // Метод включает обработку исключений и корректное управление ресурсами Bitmap.
        public void LoadImage(string filePath)
        {
            // Временные переменные для нового Bitmap и Graphics, чтобы не изменять основные поля до успешной загрузки.
            Bitmap? loadedBitmapFromFile = null;
            Bitmap? newBitmap = null;
            Graphics? newGraphics = null;

            try
            {
                // Загрузка изображения из файла в loadedBitmapFromFile.
                loadedBitmapFromFile = (Bitmap)Image.FromFile(filePath);

                // Создание нового основного холста (newBitmap) с размерами загруженного изображения.
                newBitmap = new Bitmap(loadedBitmapFromFile.Width, loadedBitmapFromFile.Height, PixelFormat.Format32bppArgb);
                // Получение объекта Graphics для нового холста.
                newGraphics = Graphics.FromImage(newBitmap);

                newGraphics.Clear(Color.Transparent); // Очистка нового холста перед отрисовкой.
                // Отрисовка загруженного изображения на новом холсте.
                newGraphics.DrawImage(loadedBitmapFromFile, 0, 0, loadedBitmapFromFile.Width, loadedBitmapFromFile.Height);

                // Если все прошло успешно до этого момента, освобождаем старые ресурсы.
                Graphics?.Dispose();
                Bitmap?.Dispose();

                // Присваиваем новые, подготовленные Bitmap и Graphics основным полям класса.
                Bitmap = newBitmap;
                Graphics = newGraphics;
                Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Настройка сглаживания для нового Graphics.

                // Обнуляем ссылки на временные переменные, так как они теперь указывают на основные ресурсы.
                // Это предотвратит их ошибочное освобождение в блоке finally.
                newBitmap = null;
                newGraphics = null;

                OnBitmapChanged(); // Вызов события, сигнализирующего об изменении холста.
            }
            catch (Exception ex) // Обработка любых ошибок, возникших при загрузке или обработке изображения.
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally // Блок finally выполняется всегда, независимо от того, было ли исключение.
            {
                // Гарантированное освобождение Bitmap, загруженного непосредственно из файла.
                loadedBitmapFromFile?.Dispose();
                // Освобождение newBitmap и newGraphics, если они не были успешно присвоены
                // основным полям (т.е. если в блоке try произошла ошибка до их обнуления).
                newBitmap?.Dispose();
                newGraphics?.Dispose();
            }
        }

        // --- Защищенные виртуальные методы ---

        // Метод для генерации события BitmapChanged.
        // Он объявлен как protected virtual, что позволяет классам-наследникам (если таковые появятся)
        // переопределить или расширить логику вызова этого события.
        protected virtual void OnBitmapChanged()
        {
            // Безопасный вызов делегата события с использованием оператора ?. (
            // Событие будет вызвано только если на него есть хотя бы один подписчик (т.е. BitmapChanged не равен null).
            BitmapChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}