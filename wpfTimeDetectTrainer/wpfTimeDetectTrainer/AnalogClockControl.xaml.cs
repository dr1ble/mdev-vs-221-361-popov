using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TimeTrainer.Controls 
{
    public class TickMarkViewModel 
    {
        public double Angle { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double Thickness { get; set; }
        public Brush Stroke { get; set; }
    }

    public partial class AnalogClockControl : UserControl, INotifyPropertyChanged
    {
        private bool _isDraggingHourHand = false;
        private bool _isDraggingMinuteHand = false;
        private Point _centerPoint = new Point(100, 100); // Инициализируем сразу, т.к. Canvas 200x200
        private bool _isMouseOverHandArea = false; // Флаг для отслеживания состояния курсора

        #region DependencyProperties

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(DateTime), typeof(AnalogClockControl),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, OnTimeChanged));

        public DateTime Time
        {
            get { return (DateTime)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(AnalogClockControl), new PropertyMetadata(false, OnIsReadOnlyChanged));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.Register("ToolTipContent", typeof(string), typeof(AnalogClockControl),
            new PropertyMetadata("В режиме обучения: перетаскивайте стрелки или используйте колесико мыши."));

        public string ToolTipContent
        {
            get { return (string)GetValue(ToolTipContentProperty); }
            set { SetValue(ToolTipContentProperty, value); }
        }

        #endregion

        #region Calculated Properties for Hand Angles

        public double HourAngle => (Time.Hour % 12 + Time.Minute / 60.0) * 30;
        public double MinuteAngle => Time.Minute * 6; 

        #endregion

        public ObservableCollection<TickMarkViewModel> TickMarks { get; } = new ObservableCollection<TickMarkViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AnalogClockControl()
        {
            InitializeComponent();
            PopulateTickMarks();
            UpdateToolTip(); 
            // Подписываемся на MouseLeave для ClockCanvas, чтобы сбросить курсор
            // Этот код должен быть здесь, после InitializeComponent, чтобы ClockCanvas был доступен.
            if (ClockCanvas != null) 
            {
                 ClockCanvas.MouseLeave += ClockCanvas_MouseLeave;
            }
        }

        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AnalogClockControl)d;
            control.OnPropertyChanged(nameof(HourAngle));   
            control.OnPropertyChanged(nameof(MinuteAngle)); 
            control.UpdateNumberHighlights();                
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AnalogClockControl)d;
            control.UpdateToolTip();
            if (control.IsReadOnly && control._isMouseOverHandArea)
            {
                Mouse.OverrideCursor = null;
                control._isMouseOverHandArea = false;
            }
        }

        private void UpdateToolTip()
        {
            if (IsReadOnly)
            {
                ToolTipContent = "Время зафиксировано.";
            }
            else
            {
                ToolTipContent = "В режиме обучения: перетаскивайте стрелки или используйте колесико мыши.";
            }
        }

        private void PopulateTickMarks()
        {
            TickMarks.Clear(); 
            double centerX = 100; 
            double centerY = 100; 
            double outerRadius = 92; 

            double hourTickLength = 17;  
            double minuteTickLength = 7; 

            for (int i = 0; i < 60; i++)
            {
                var tick = new TickMarkViewModel();
                tick.Angle = i * 6; 
                tick.Stroke = Brushes.Black;

                if (i % 5 == 0) 
                {
                    tick.Y1 = centerY - outerRadius;
                    tick.Y2 = centerY - (outerRadius - hourTickLength);
                    tick.Thickness = 2.0;
                    if (i % 15 == 0) 
                    {
                        tick.Thickness = 2.5;
                    }
                }
                else 
                {
                    tick.Y1 = centerY - outerRadius;
                    tick.Y2 = centerY - (outerRadius - minuteTickLength);
                    tick.Thickness = 0.8;
                }
                TickMarks.Add(tick);
            }
        }

        #region Mouse Interaction Handlers

        private bool IsMouseOverHand(Point mousePosition, out bool isMinuteHand)
        {
            isMinuteHand = false; // Инициализация out параметра
            if (IsReadOnly || ClockCanvas == null) return false; // Добавил проверку ClockCanvas на null

            double hourHandVisualLength = 50; 
            double minuteHandVisualLength = 75;
            double grabRadius = 40; // Ваш рабочий радиус для захвата

            Point hourHandTip = GetHandTipPosition(HourAngle, hourHandVisualLength);
            Point minuteHandTip = GetHandTipPosition(MinuteAngle, minuteHandVisualLength);

            double distToMinuteTip = GetDistance(mousePosition, minuteHandTip);
            if (distToMinuteTip < grabRadius)
            {
                isMinuteHand = true;
                return true;
            }

            double distToHourTip = GetDistance(mousePosition, hourHandTip);
            if (distToHourTip < grabRadius)
            {
                isMinuteHand = false; // Убедимся, что это не минутная, если попали в часовую
                return true;
            }
            return false;
        }


        private void ClockCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly) return;
            // _centerPoint уже инициализирован
            Point clickPoint = e.GetPosition(ClockCanvas);

            if (IsMouseOverHand(clickPoint, out bool isOverMinuteHand))
            {
                if (isOverMinuteHand)
                {
                    _isDraggingMinuteHand = true;
                    _isDraggingHourHand = false;
                }
                else
                {
                    _isDraggingHourHand = true;
                    _isDraggingMinuteHand = false;
                }
                Mouse.Capture(ClockCanvas);
                e.Handled = true; 
            }
        }

        private Point GetHandTipPosition(double angleDegrees, double length)
        {
            double angleRadians = (angleDegrees - 90) * Math.PI / 180.0; 
            return new Point(
                _centerPoint.X + length * Math.Cos(angleRadians),
                _centerPoint.Y + length * Math.Sin(angleRadians)
            );
        }

        private double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void ClockCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (ClockCanvas == null) return; // Защита от null
            Point currentMousePosition = e.GetPosition(ClockCanvas);

            // --- Логика изменения курсора ---
            if (!IsReadOnly && !_isDraggingHourHand && !_isDraggingMinuteHand) 
            {
                if (IsMouseOverHand(currentMousePosition, out _))
                {
                    if (!_isMouseOverHandArea)
                    {
                        Mouse.OverrideCursor = Cursors.Hand;
                        _isMouseOverHandArea = true;
                    }
                }
                else
                {
                    if (_isMouseOverHandArea)
                    {
                        Mouse.OverrideCursor = null;
                        _isMouseOverHandArea = false;
                    }
                }
            }

            // --- Логика перетаскивания стрелок ---
            if (IsReadOnly || e.LeftButton != MouseButtonState.Pressed || !(_isDraggingHourHand || _isDraggingMinuteHand))
            {
                return;
            }
            
            double dx = currentMousePosition.X - _centerPoint.X;
            double dy = currentMousePosition.Y - _centerPoint.Y;
            double angleRad = Math.Atan2(dy, dx); 
            double angleDeg = angleRad * 180 / Math.PI + 90; 
            if (angleDeg < 0) angleDeg += 360; 

            DateTime currentTime = Time; 

            if (_isDraggingMinuteHand)
            {
                int newMinute = (int)Math.Round(angleDeg / 6.0) % 60; 
                int currentHour24 = currentTime.Hour;
                
                if (currentTime.Minute >= 50 && newMinute <= 10) 
                {
                    currentHour24 = (currentHour24 + 1) % 24;
                }
                else if (currentTime.Minute <= 10 && newMinute >= 50) 
                {
                    currentHour24 = (currentHour24 - 1 + 24) % 24;
                }
                Time = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentHour24, newMinute, 0, currentTime.Kind);
            }
            else if (_isDraggingHourHand)
            {
                int targetHour_1_12 = (int)Math.Round(angleDeg / 30.0);
                if (targetHour_1_12 == 0) targetHour_1_12 = 12; 

                int finalHour_0_23;
                int currentHour_0_23 = currentTime.Hour;
                int currentMinutes = currentTime.Minute; 
                bool currentWasAM = currentHour_0_23 < 12;

                if (currentWasAM)
                {
                    finalHour_0_23 = (targetHour_1_12 == 12) ? 0 : targetHour_1_12; 
                }
                else 
                {
                    finalHour_0_23 = (targetHour_1_12 == 12) ? 12 : targetHour_1_12 + 12; 
                }
                
                int prevHour_1_12_display = currentHour_0_23 % 12;
                if (prevHour_1_12_display == 0) prevHour_1_12_display = 12;

                // Условие для смены AM/PM при перетаскивании часовой стрелки
                // Если разница между старым и новым положением часовой стрелки на циферблате 
                // составляет примерно от 5 до 7 часов (т.е. перешли через "полциферблата")
                int hourDiff12 = Math.Abs(targetHour_1_12 - prevHour_1_12_display);
                if (hourDiff12 > 12) hourDiff12 = 12 - (hourDiff12 % 12); // корректируем разницу для круга

                if (hourDiff12 >= 5 && hourDiff12 <= 7) 
                {
                    // Просто инвертируем AM/PM
                    if (finalHour_0_23 < 12) finalHour_0_23 += 12;
                    else finalHour_0_23 -=12;
                }
                finalHour_0_23 %=24; // Убедимся, что в диапазоне 0-23
                
                Time = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, finalHour_0_23, currentMinutes, 0, currentTime.Kind);
            }
        }
        
        private void ClockCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isMouseOverHandArea && !_isDraggingHourHand && !_isDraggingMinuteHand) 
            {
                Mouse.OverrideCursor = null;
                _isMouseOverHandArea = false;
            }
        }

        private void ClockCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool wasDragging = _isDraggingHourHand || _isDraggingMinuteHand;
            if (_isDraggingHourHand || _isDraggingMinuteHand)
            {
                _isDraggingHourHand = false;
                _isDraggingMinuteHand = false;
                Mouse.Capture(null);
            }

            if (_isMouseOverHandArea) // Если курсор был "рукой"
            {
                 Point currentMousePosition = e.GetPosition(ClockCanvas);
                 if (!IsMouseOverHand(currentMousePosition, out _)) // И сейчас он не над стрелкой
                 {
                    Mouse.OverrideCursor = null; // Сбрасываем курсор
                    _isMouseOverHandArea = false;
                 }
            }


            if (wasDragging && !IsReadOnly) 
            {
                DateTime currentTime = Time; 
                int hour = currentTime.Hour;
                int minute = currentTime.Minute;
                int roundedMinute = (int)(Math.Round((double)minute / 5.0) * 5);

                if (roundedMinute == 60)
                {
                    roundedMinute = 0;
                    hour = (hour + 1) % 24;
                }
                
                if (Time.Hour != hour || Time.Minute != roundedMinute || Time.Second != 0) 
                {
                    Time = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, roundedMinute, 0, currentTime.Kind);
                }
            }
        }

        private void ClockCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsReadOnly) return;

            int minuteChange = (e.Delta > 0) ? 5 : -5; 

            if (minuteChange != 0)
            {
                DateTime baseTime = Time; 
                DateTime timeAfterMinuteChange = baseTime.AddMinutes(minuteChange);

                int hour = timeAfterMinuteChange.Hour;
                int minute = timeAfterMinuteChange.Minute;
                int roundedMinute = (int)(Math.Round((double)minute / 5.0) * 5);

                if (roundedMinute == 60)
                {
                    roundedMinute = 0;
                    hour = (hour + 1) % 24; 
                }
                
                if (Time.Hour != hour || Time.Minute != roundedMinute || Time.Second != 0)
                {
                     Time = new DateTime(timeAfterMinuteChange.Year, timeAfterMinuteChange.Month, timeAfterMinuteChange.Day, hour, roundedMinute, 0, baseTime.Kind);
                }
            }
        }

        #endregion

        #region Number Highlighting

        private void UpdateNumberHighlights()
        {
            if (this.ClockCanvas == null) return; 

            ResetNumberStyles();

            int hourForHighlight = Time.Hour % 12;
            if (hourForHighlight == 0) hourForHighlight = 12;

            TextBlock hourTextToHighlight = FindNameInCanvas($"Hour{hourForHighlight}Text") as TextBlock;
            if (hourTextToHighlight != null)
            {
                hourTextToHighlight.FontWeight = FontWeights.ExtraBold;
                hourTextToHighlight.Foreground = Brushes.Red;
            }

            int roundedMinutes = (int)Math.Round(Time.Minute / 5.0) * 5;
            if (roundedMinutes == 60) roundedMinutes = 0; 

            int minuteAsHourNumberEquivalent = (roundedMinutes == 0) ? 12 : roundedMinutes / 5; 

            TextBlock minuteNumberTextToHighlight = FindNameInCanvas($"Hour{minuteAsHourNumberEquivalent}Text") as TextBlock;
            if (minuteNumberTextToHighlight != null)
            {
                if (minuteNumberTextToHighlight == hourTextToHighlight)
                {
                    minuteNumberTextToHighlight.Foreground = Brushes.Purple; 
                }
                else
                {
                    minuteNumberTextToHighlight.FontWeight = FontWeights.ExtraBold;
                    minuteNumberTextToHighlight.Foreground = Brushes.Blue; 
                }
            }
        }

        private void ResetNumberStyles()
        {
            if (this.ClockCanvas == null) return;

            for (int i = 1; i <= 12; i++)
            {
                TextBlock numberText = FindNameInCanvas($"Hour{i}Text") as TextBlock;
                if (numberText != null)
                {
                    numberText.FontWeight = FontWeights.Bold; 
                    numberText.Foreground = Brushes.Black;    
                }
            }
        }
        
        private FrameworkElement FindNameInCanvas(string name)
        {
            // ClockCanvas должен иметь x:Name="ClockCanvas" в XAML
            return ClockCanvas?.FindName(name) as FrameworkElement;
        }

        #endregion
    }
}