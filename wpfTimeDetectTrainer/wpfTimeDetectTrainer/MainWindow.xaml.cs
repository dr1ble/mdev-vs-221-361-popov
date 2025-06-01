using System.Windows;

namespace TimeTrainer // Убедитесь, что пространство имен совпадает с вашим проектом
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // DataContext устанавливается в MainWindow.xaml:
            // <Window.DataContext>
            //     <vm:MainViewModel/>
            // </Window.DataContext>
            // Поэтому здесь дополнительный код для установки DataContext не требуется.
        }
    }
}