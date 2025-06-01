using System;

namespace cnsGameFindLock
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; 
            ConsoleRenderer renderer = new ConsoleRenderer();
            KeyLockPattern.GameFindLock game = new KeyLockPattern.GameFindLock();
            bool exitGame = false; // Флаг для полного выхода из игры

            renderer.ShowWelcomeScreen();

            while (!exitGame) // Главный цикл программы, пока не решено выйти
            {
                MenuAction mainMenuChoice = renderer.ShowMainMenu();
                Console.Clear(); 

                switch (mainMenuChoice)
                {
                    case MenuAction.Play:
                        bool playAnotherRound = true;
                        while (playAnotherRound) // Внутренний цикл для повторных раундов
                        {
                            game.RunGameRound(); // RunGameRound теперь содержит ConfigureGame()

                            if (renderer.AskPlayAgain())
                            {
                                // Если "да", то цикл playAnotherRound продолжится,
                                // и RunGameRound() снова вызовет ConfigureGame()
                                Console.Clear(); 
                            }
                            else
                            {
                                playAnotherRound = false; // Если "нет", выходим из цикла повторных раундов
                                Console.Clear();          // Очищаем экран перед возвратом в главное меню
                            }
                        }
                        break; // Возврат в главное меню после завершения сессии игр

                    case MenuAction.ViewStats:
                        game.DisplayStats();
                        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case MenuAction.GameRules:
                        renderer.ShowGameRules();
                        Console.Clear();
                        break;

                    case MenuAction.Exit:
                        exitGame = true; // Устанавливаем флаг для выхода из главного цикла
                        break;
                }
            }
            Console.WriteLine("\nСпасибо за игру! До свидания!");
        }
    }
}