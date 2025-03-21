namespace wfaEvent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Click += Button2_Click;
            // способ 3 - через анонимный метод
            button3.Click += delegate
            {
                MessageBox.Show("Cпособ 3");
            };
            // button3.Click += Button2_Click;
            // способ 4
            // Лучше использовать этот способ
            button4.Click += (s, e) => MessageBox.Show("Способ 4");
        }

        // 2ой способ он лучше потому что конструктор не поломается если удалит и сразу видно к чему подключен обработчик
        // Лучше использовать этот способ

        private void Button2_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Способ 2");
        }
        // 1-ый способ просто через форму двойным нажатиему. Хуже он, потому что при удалении данного кода ломается форма.

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Способ 1");
        }
    }
}
