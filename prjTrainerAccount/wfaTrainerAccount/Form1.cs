
using myCore;

namespace wfaTrainerAccount
{
    public partial class Form1 : Form
    {
        private readonly Game game;

        public Form1()
        {
            InitializeComponent();

            game = new Game();
            game.ChangeQuestion += () => laQuestion.Text = game.QuestionLine;
            game.ChangeStat += Game_ChangeStat;
            game.Restart();

            buYes.Click += (s, e) => game.DoAnswer(true);
            buNo.Click += (s, e) => game.DoAnswer(false);

        }

        private void Game_ChangeStat()
        {
            label1.Text = $"Верно = {game.CountCorrect}";
            label2.Text = $"Неверно = {game.CountWrong}";
        }
    }
}
