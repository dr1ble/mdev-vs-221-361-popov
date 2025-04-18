using myCore;

namespace cnsTrainerAccount
{
    internal class Program
    {
        private static Game game;

        static void Main(string[] args)
        {
            Console.WriteLine("Игра 'Устный счёт'");
        
            game = new Game();
            game.ChangeQuestion += () => Console.WriteLine($"Вопрос: {game.QuestionLine}");
            game.ChangeStat += () => Console.WriteLine(
                $"Статистика: Верно = {game.CountCorrect}, Неверно = {game.CountWrong}");
            game.Restart();

            while (true)
            {
                Console.WriteLine("Ответ Y/N?");
                var line = Console.ReadLine()?.ToUpper();
                if (line == "Y")
                    game.DoAnswer(true);
                else if (line == "N")
                    game.DoAnswer(false);
                else
                    break;
            }
            Console.WriteLine("Пока!");
        }
    }
}
