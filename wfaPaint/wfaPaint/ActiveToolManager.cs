using System.Drawing.Drawing2D; 
using System.Windows.Forms;     
using System;                  
namespace wfaPaint
{
    // Класс ActiveToolManager отвечает за управление текущим активным инструментом (режимом) рисования.
    // Он инкапсулирует логику смены инструмента и связанные с этим побочные эффекты,
    // такие как изменение режима наложения (CompositingMode) для Graphics
    // или сброс активного выделения при переключении на инструмент рисования.
    public class ActiveToolManager
    {
        // Перечисление MyDrawMode определяет все доступные инструменты/режимы рисования в приложении.
        public enum MyDrawMode
        {
            Pencil,    // Карандаш
            Line,      // Линия
            Ellipse,   // Эллипс
            Rectangle, // Прямоугольник
            Triangle,  // Треугольник
            Arrow,     // Стрелка
            Star,      // Звезда
            Hexagon,   // Шестиугольник
            Select,    // Режим выделения области
            Eraser     // Ластик
        }

        // --- Приватные поля только для чтения (readonly) ---
        // Ссылки на другие менеджеры, необходимые для корректной работы ActiveToolManager.

        // Ссылка на CanvasController для доступа к объекту Graphics (например, для изменения CompositingMode).
        private readonly CanvasController _canvasController; // Изменено с _drawingManager
        // Ссылка на SelectionManager для управления состоянием выделения (например, для его сброса).
        private readonly SelectionManager _selectionManager;
        // Ссылка на PictureBox, который необходимо перерисовать (Invalidate) после некоторых операций.
        private readonly PictureBox _pictureBoxToInvalidate;

        // --- Открытые свойства ---

        // CurrentMode хранит текущий активный инструмент/режим рисования.
        // Имеет публичный getter и приватный setter, т.е. изменяется только внутри этого класса методом SetActiveTool.
        public MyDrawMode CurrentMode { get; private set; }

        // --- События ---

        // Событие ToolChanged генерируется после смены активного инструмента.
        // Другие части приложения могут подписаться на него, чтобы отреагировать на изменение режима.
        // Знак '?' указывает, что событие может не иметь подписчиков (nullable reference type).
        public event EventHandler<MyDrawMode>? ToolChanged;

        // --- Конструктор ---

        // Конструктор инициализирует ActiveToolManager, получая необходимые зависимости.
        // Устанавливает начальный режим рисования (карандаш по умолчанию).
        public ActiveToolManager(CanvasController canvasController, SelectionManager selectionManager, PictureBox pictureBoxToInvalidate)
        {
            _canvasController = canvasController;
            _selectionManager = selectionManager;
            _pictureBoxToInvalidate = pictureBoxToInvalidate;
            CurrentMode = MyDrawMode.Pencil; // Карандаш устанавливается как инструмент по умолчанию при запуске.
        }

        // --- Публичные методы ---

        // SetActiveTool - основной метод для смены текущего инструмента/режима рисования.
        // Принимает новый режим в качестве параметра.
        public void SetActiveTool(MyDrawMode newMode)
        {
            MyDrawMode previousMode = CurrentMode; // Сохраняем предыдущий режим для сравнения.
            CurrentMode = newMode;                // Устанавливаем новый активный режим.

            // Логика для управления CompositingMode объекта Graphics.
            // Это важно для корректной работы ластика, который требует CompositingMode.SourceCopy,
            // в то время как обычное рисование использует CompositingMode.SourceOver.
            if (_canvasController.Graphics != null) // Проверка на null перед доступом к Graphics
            {
                // Если предыдущий инструмент был ластиком, а новый - нет,
                // или если новый инструмент не ластик, а режим наложения по какой-то причине не SourceOver,
                // то принудительно устанавливаем SourceOver (стандартный режим наложения с учетом прозрачности).
                if ((previousMode == MyDrawMode.Eraser && newMode != MyDrawMode.Eraser) ||
                    (newMode != MyDrawMode.Eraser && _canvasController.Graphics.CompositingMode != CompositingMode.SourceOver))
                {
                    _canvasController.Graphics.CompositingMode = CompositingMode.SourceOver;
                }
                // Установка CompositingMode.SourceCopy для ластика происходит непосредственно
                // в PxImage_MouseDown перед началом операции стирания.
            }


            // Логика для сброса активного выделения.
            // Если существует активное выделение (HasSelection) и новый выбранный инструмент
            // не является инструментом "Выделение" (Select), то выделение сбрасывается.
            // Это предотвращает ситуацию, когда рамка выделения остается видимой при рисовании другими инструментами.
            if (_selectionManager.HasSelection && newMode != MyDrawMode.Select)
            {
                _selectionManager.ClearSelection();         // Сброс данных выделения.
                _pictureBoxToInvalidate.Invalidate();     // Запрос на перерисовку PictureBox для скрытия рамки.
            }

            // Генерация события о смене инструмента.
            OnToolChanged(newMode);
        }

        // --- Защищенные виртуальные методы ---

        // OnToolChanged - защищенный виртуальный метод для генерации события ToolChanged.
        protected virtual void OnToolChanged(MyDrawMode newMode)
        {
            // Безопасный вызов делегата события с использованием оператора ?.
            // Событие будет вызвано только если на него есть подписчики.
            // В качестве аргумента события передается новый выбранный режим (newMode).
            ToolChanged?.Invoke(this, newMode);
        }
    }
}