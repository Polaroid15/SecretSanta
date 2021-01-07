using System;

namespace SecretSanta
{
    public static class StringConstants
    {
        private static readonly string[] ListOfHello =
        {
            "Hello", "Aloha!", "Hola, amigos!", "Chao!",
            "Hello Comrade!", "Bonjour!", "Welcome back! Me..", "Guten tag!"
        };

        public static string GetHelloMessage()
        {
            return ListOfHello[new Random().Next(ListOfHello.Length)];
        }

        public static string GetAnswerMessage()
        {
            return ListOfAnswers[new Random().Next(ListOfAnswers.Length)];
        }

        public static string GetAlienMessage()
        {
            return ListOfAlien[new Random().Next(ListOfAlien.Length)];
        }

        private static readonly string[] ListOfAnswers =
        {
            "Bye.", "Мне нечего тебе сказать.", "У тебя уже есть жертва.", "Опять ты?!",
            "AGGGRHHH...", "Baby don't hurt me.. don't hurt me... no more.",
            "Мы уже выбрали у кого ты будешь тайным сантой, промотай чат."
        };

        private static readonly string[] ListOfAlien =
        {
            "Чужак!", "Ты не заявлен в список участников SS(secret_santa).", "Тебе что, больше написать некому?",
            "Перестань пожалуйста сюда писать какие либо символы.",
            "Baby don't hurt me.. don't hurt me... no more.",
            "Если хочешь участвовать в SS(secret_santa), напиши в группу."
        };

        public static string AdminActionMsg = "Введите username(например, @SS_OlivieBot), если это пара,\n" +
                                              "то укажите через',' (например, @SS_OlivieBot, @SS_OlivieBot2).\nКогда закончите список, напишите GO";

        public static string StartAppMsg = "Найдите в телеграмме @SS_OlivieBot. Активируйте его, введя команду 'sss'.\n" +
                                           "Подождите, пока каждому достанется его Санта, а затем можете нажать \n" +
                                           "любую клавишу, чтобы закрыть приложение.\n" +
                                           "ВНИМАНИЕ! Человек активировавший бота, получает жертву последним.";
        
        public static string ResetMsg = "Santa's list clear";
        public static string FinishMsg = "Event is finished";
    }
}