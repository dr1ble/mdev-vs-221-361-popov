using System;
using System.Collections.ObjectModel; 
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TimeTrainer.Infrastructure; // Убедитесь, что пространство имен RelayCommand правильное

namespace TimeTrainer.ViewModels
{
    public enum GameMode
    {
        Learning,
        Testing
    }

    public class MainViewModel : BaseViewModel 
    {
        private static readonly Random _random = new Random(); 
        private GameMode _currentMode;
        public GameMode CurrentMode
        {
            get => _currentMode;
            set { _currentMode = value; OnPropertyChanged(); UpdateModeProperties(); }
        }

        // --- Свойства для режима обучения ---
        private string _targetTimeDisplayString; 
        public string TargetTimeDisplayString
        {
            get => _targetTimeDisplayString;
            set { _targetTimeDisplayString = value; OnPropertyChanged(); }
        }
        
        private DateTime _internalTargetLearningTime; 

        private bool _learningTimePassed;
        public bool LearningTimePassed
        {
            get => _learningTimePassed;
            set { _learningTimePassed = value; OnPropertyChanged(); }
        }

        private bool _showNextButton;
        public bool ShowNextButton
        {
            get => _showNextButton;
            set { _showNextButton = value; OnPropertyChanged(); }
        }

        private DateTime _learningClockTime; 
        public DateTime LearningClockTime
        {
            get => _learningClockTime;
            set { _learningClockTime = value; OnPropertyChanged(); OnPropertyChanged(nameof(LearningDigitalTime)); }
        }
        public string LearningDigitalTime => LearningClockTime.ToString("HH:mm"); 

        // --- Свойства для режима тестирования (сколько минут прошло) ---
        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(); }
        }

        private string _elapsedMinutesInput;
        public string ElapsedMinutesInput
        {
            get => _elapsedMinutesInput;
            set { _elapsedMinutesInput = value; OnPropertyChanged(); }
        }
        
        private int _actualElapsedMinutes;

        // --- Общие свойства ---
        private string _feedbackMessage;
        public string FeedbackMessage
        {
            get => _feedbackMessage;
            set { _feedbackMessage = value; OnPropertyChanged(); }
        }

        public bool IsCheckButtonVisible => CurrentMode == GameMode.Learning ? !ShowNextButton : true; 


        // --- Команды ---
        public ICommand CheckAnswerCommand { get; } 
        public ICommand GenerateNewProblemCommand { get; } 
        public ICommand AddInputCommand { get; } 
        public ICommand CheckLearningTimeCommand { get; } 
        public ICommand NextLearningTaskCommand { get; } 

        public MainViewModel()
        {
            LearningClockTime = DateTime.Now; 
            
            CheckAnswerCommand = new RelayCommand(PerformCheckElapsedTime, () => CurrentMode == GameMode.Testing && !string.IsNullOrEmpty(ElapsedMinutesInput));
            GenerateNewProblemCommand = new RelayCommand(GenerateNewElapsedTimeProblem);
            AddInputCommand = new RelayCommand<string>(AddInputToElapsed);
            CheckLearningTimeCommand = new RelayCommand(PerformCheckLearningTime, () => CurrentMode == GameMode.Learning && !ShowNextButton);
            NextLearningTaskCommand = new RelayCommand(GenerateNewLearningTask, () => CurrentMode == GameMode.Learning && ShowNextButton);

            CurrentMode = GameMode.Learning; 
        }

        private void UpdateModeProperties()
        {
            OnPropertyChanged(nameof(IsLearningModeActive));
            OnPropertyChanged(nameof(IsTestingModeActive));

            FeedbackMessage = "";
            ElapsedMinutesInput = ""; 
            LearningTimePassed = false; 
            ShowNextButton = false;
            OnPropertyChanged(nameof(IsCheckButtonVisible));

            if (CurrentMode == GameMode.Testing)
            {
                GenerateNewElapsedTimeProblem();
            }
            else 
            {
                GenerateNewLearningTask();
            }
        }

        public bool IsLearningModeActive => CurrentMode == GameMode.Learning;
        public bool IsTestingModeActive => CurrentMode == GameMode.Testing;

        #region Logic for Testing Mode (Elapsed Time)

        private void GenerateNewElapsedTimeProblem()
        {
            int startHour = _random.Next(0, 23); 
            int startMinute = _random.Next(0, 12) * 5; 
            StartTime = new DateTime(2023, 1, 1, startHour, startMinute, 0);

            _actualElapsedMinutes = _random.Next(1, 37) * 5; 
            EndTime = StartTime.AddMinutes(_actualElapsedMinutes);

            ElapsedMinutesInput = "";
            FeedbackMessage = "";
        }

        private void PerformCheckElapsedTime()
        {
            if (int.TryParse(ElapsedMinutesInput, out int userAnswer))
            {
                if (userAnswer == _actualElapsedMinutes)
                {
                    FeedbackMessage = "Правильно! Отличная работа!";
                    System.Threading.Tasks.Task.Delay(1500).ContinueWith(t => {
                        System.Windows.Application.Current.Dispatcher.Invoke(GenerateNewElapsedTimeProblem);
                    });
                }
                else
                {
                    FeedbackMessage = $"Неверно. Правильный ответ: {_actualElapsedMinutes} минут.";
                }
            }
            else
            {
                FeedbackMessage = "Пожалуйста, введите корректное число.";
            }
        }

        private void AddInputToElapsed(string digit)
        {
            if (digit == "DEL")
            {
                if (!string.IsNullOrEmpty(ElapsedMinutesInput))
                    ElapsedMinutesInput = ElapsedMinutesInput.Substring(0, ElapsedMinutesInput.Length - 1);
            }
            else if (ElapsedMinutesInput == null || ElapsedMinutesInput.Length < 3) 
            {
                ElapsedMinutesInput += digit;
            }
        }
        #endregion

        #region Logic for Learning Mode (Set Time on Clock)

        private void GenerateNewLearningTask()
        {
            int hourTarget_0_23 = _random.Next(0, 23); 
            int minuteTarget_0_55 = _random.Next(0, 12) * 5; 

            _internalTargetLearningTime = new DateTime(2023, 1, 1, hourTarget_0_23, minuteTarget_0_55, 0);

            string taskString = "";
            int formatType = _random.Next(0, 4); 

            switch (formatType)
            {
                case 0: // Цифровой формат
                    taskString = $"{hourTarget_0_23:D2}:{minuteTarget_0_55:D2}";
                    break;
                case 1: // Четверти и половина
                    if (minuteTarget_0_55 == 15)
                        // "Четверть третьего" (для _internalTargetLearningTime = 02:15)
                        taskString = $"Четверть {GetHourNameGenitive((hourTarget_0_23 + 1) % 24)}"; 
                    else if (minuteTarget_0_55 == 30)
                        // "Половина третьего" (для _internalTargetLearningTime = 02:30)
                        taskString = $"Половина {GetHourNameGenitive((hourTarget_0_23 + 1) % 24)}"; 
                    else if (minuteTarget_0_55 == 45)
                        // "Без четверти три" (для _internalTargetLearningTime = 02:45)
                        taskString = $"Без четверти {GetHourNameNominative((hourTarget_0_23 + 1) % 24)}"; 
                    else 
                        taskString = $"{hourTarget_0_23:D2}:{minuteTarget_0_55:D2}"; 
                    break;
                case 2: // Ровно
                     if (minuteTarget_0_55 == 0)
                        taskString = $"{GetHourNameNominative(hourTarget_0_23)} ровно";
                     else
                        taskString = $"{hourTarget_0_23:D2}:{minuteTarget_0_55:D2}"; 
                    break;
                case 3: // Словесный X минут Y-го / до Y-го
                    if (minuteTarget_0_55 == 0) {
                         taskString = $"{GetHourNameNominative(hourTarget_0_23)} ровно";
                    } else if (minuteTarget_0_55 <= 30) {
                        // "10 минут первого" (для _internalTargetLearningTime = 00:10)
                        taskString = $"{minuteTarget_0_55} {GetMinuteWord(minuteTarget_0_55)} {GetHourNameGenitive(hourTarget_0_23)}"; 
                    } else { 
                        // "Без 20 час" (для _internalTargetLearningTime = 00:40 -> 20 минут до часа (01:00))
                        taskString = $"Без {60-minuteTarget_0_55} {GetMinuteWord(60-minuteTarget_0_55)} {GetHourNameNominative((hourTarget_0_23 + 1) % 24)}";
                    }
                    break;
                default: // На всякий случай, если random выдаст что-то не то
                     taskString = $"{hourTarget_0_23:D2}:{minuteTarget_0_55:D2}";
                    break;
            }
            
            TargetTimeDisplayString = $"{taskString}";
            LearningClockTime = new DateTime(2023, 1, 1, _random.Next(0, 23), _random.Next(0, 12) * 5, 0);

            LearningTimePassed = false;
            ShowNextButton = false;
            FeedbackMessage = "";
            OnPropertyChanged(nameof(IsCheckButtonVisible));
        }
        
        private void PerformCheckLearningTime()
        {
            int clockHour_0_23 = LearningClockTime.Hour; 
            int clockMinute_0_59 = LearningClockTime.Minute; 

            // Округляем до ближайших 5 минут
            int roundedClockMinute_0_55 = (int)(Math.Round(clockMinute_0_59 / 5.0) * 5);

            // Корректируем час, если минуты округлились до 60
            int clockHourForCompare_0_23 = clockHour_0_23;
            if (roundedClockMinute_0_55 == 60)
            {
                roundedClockMinute_0_55 = 0;
                clockHourForCompare_0_23 = (clockHourForCompare_0_23 + 1) % 24; 
            }

            // Получаем часы и минуты из целевого времени
            int targetHourToCompare_0_23 = _internalTargetLearningTime.Hour;
            int targetMinuteToCompare_0_55 = _internalTargetLearningTime.Minute;

            // Сравниваем часы и минуты
            // Для часов сравниваем в 12-часовом формате (для аналоговых часов)
            bool hoursMatch = (clockHourForCompare_0_23 % 12) == (targetHourToCompare_0_23 % 12);
            bool minutesMatch = roundedClockMinute_0_55 == targetMinuteToCompare_0_55;
            
            System.Diagnostics.Debug.WriteLine($"ЗАДАНИЕ: \"{TargetTimeDisplayString}\" (Внутренне: {_internalTargetLearningTime:HH:mm})");
            System.Diagnostics.Debug.WriteLine($"ПОЛЬЗОВАТЕЛЬ: {LearningClockTime:HH:mm:ss} (Для сравнения: H={clockHourForCompare_0_23}, M={roundedClockMinute_0_55})");
            System.Diagnostics.Debug.WriteLine($"РЕЗУЛЬТАТ: Часы совпали={hoursMatch}, Минуты совпали={minutesMatch}");

            if (hoursMatch && minutesMatch)
            {
                FeedbackMessage = "Отлично! Вы правильно установили время!";
                LearningTimePassed = true;
                ShowNextButton = true;
            }
            else
            {
                FeedbackMessage = "Попробуйте еще раз. Время установлено неверно.";
                LearningTimePassed = false;
                ShowNextButton = false;
            }
            OnPropertyChanged(nameof(IsCheckButtonVisible));
        }

        // Вспомогательные методы для русского языка - ТРЕБУЮТ ЗНАЧИТЕЛЬНОЙ ДОРАБОТКИ
        private string GetMinuteWord(int minutes)
        {
            minutes %= 60;
            if (minutes == 0) return "минут"; // Для "Без 60 минут..." -> "ровно"
            if (minutes == 1 || (minutes % 10 == 1 && minutes != 11)) return "минута";
            if ((minutes >=2 && minutes <=4) || (minutes % 10 >=2 && minutes % 10 <=4 && (minutes <10 || minutes > 20))) return "минуты";
            return "минут";
        }

        private string GetHourNameNominative(int hour24) // Именительный падеж: час, два, ..., двенадцать
        {
            hour24 %= 24;
            int hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12; // 00:xx и 12:xx -> для имени это 12-й час

            switch (hour12)
            {
                case 1: return "час"; // "Без четверти ЧАС"
                case 2: return "два"; 
                case 3: return "три"; 
                case 4: return "четыре"; 
                case 12: return "двенадцать"; 
                default: return hour12.ToString(); 
            }
        }

        private string GetHourNameGenitive(int hour24) // Родительный падеж: первого, второго, ... двенадцатого
        {
            // Принимает час 0-23.
            // Для "10 минут ПЕРВОГО" (00:10), сюда передается 0.
            // Для "Четверть ПЕРВОГО" (00:15), сюда передается 0.
            // Для "Половина ПЕРВОГО" (00:30), сюда передается 0 (или 1, если логика такая, что передаем следующий час).
            // В текущей логике GenerateNewLearningTask:
            // - для "Четверть/Половина X" передается (hourTarget_0_23 + 1) % 24
            // - для "X минут Y-го" (где Y - текущий час) передается hourTarget_0_23

            hour24 %= 24; 
            int hour12_for_name = hour24 % 12;
            if (hour12_for_name == 0) hour12_for_name = 12; // 00:xx или 12:xx это 12-й час для названия

            switch (hour12_for_name)
            {
                case 1: return "первого";
                case 2: return "второго";
                case 3: return "третьего";
                case 4: return "четвертого";
                case 5: return "пятого";
                case 6: return "шестого";
                case 7: return "седьмого";
                case 8: return "восьмого";
                case 9: return "девятого";
                case 10: return "десятого";
                case 11: return "одиннадцатого";
                case 12: return "двенадцатого";
                default: return hour12_for_name.ToString(); // Не должно произойти
            }
        }
        #endregion
    }
}