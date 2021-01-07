namespace SecretSanta.TelegramBase
{
    public class User
    {
        public string Username { get; set; }
        public string[] ForbiddenUsernames { get; set; }
        public string SantaName { get; set; }
    }
}