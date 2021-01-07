using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace SecretSanta.TelegramBase
{
    public class SantaClient
    {
        private const string API_KEY = "1417826606:AAFojCrDnpD9Sh1sbcJp2NCTqEtNkdFpt9E";
        private const string START_COMMAND = "SSS"; //StartSecretSanta
        private const string RESET_COMMAND = "SSS_RESET";
        private const string STATS_COMMAND = "SSS_STATS";

        private readonly TelegramBotClient _botClient;
        private long _adminId;
        private bool _isAdminListDone;
        private readonly IList<User> _users;
        private readonly Dictionary<long, string> _activeUsers;

        public SantaClient()
        {
            _users = new List<User>();
            _activeUsers = new Dictionary<long, string>();
            _botClient = new TelegramBotClient(API_KEY);
        }

        public void Start()
        {
            _botClient.Timeout = TimeSpan.FromSeconds(10);
            _botClient.OnMessage += BotOnMessage;
            _botClient.StartReceiving(Array.Empty<UpdateType>());
        }

        private async void BotOnMessage(object sender, MessageEventArgs e)
        {
            try {
                var messageText = e.Message?.Text;
                if (messageText == null)
                    return;
                var fromUsername = e.Message.From.Username;

                long id = e.Message.Chat.Id;
                if (id == _adminId && await ActionCommand(messageText)) {
                    return;
                }

                if (_isAdminListDone) {
                    var pairMsg = ChoosePair(fromUsername);
                    _ = await _botClient.SendTextMessageAsync(id, pairMsg);
                    NotifyAdmin();
                    return;
                }

                if (await SetAdminSettings(id, messageText)) {
                    return;
                }

                await AddNewUserToHelloList(id, fromUsername);
                await InitAdmin(id, messageText);
                Console.WriteLine($"id: {id}, username: {fromUsername}, message: {messageText}");
            }
            catch {
                _ = await _botClient.GetUpdatesAsync(-1);
            }
        }

        private async Task<bool> ActionCommand(string messageText)
        {
            bool isActionDone = false;
            if (messageText.Equals(RESET_COMMAND, StringComparison.InvariantCultureIgnoreCase)) {
                _ = await _botClient.SendTextMessageAsync(_adminId, StringConstants.ResetMsg);
                _isAdminListDone = false;
                _users.Clear();
                isActionDone = true;
            }

            if (messageText.Equals(STATS_COMMAND, StringComparison.InvariantCultureIgnoreCase)) {
                var count = _users.Count(u => u.SantaName == null);
                var statMsg = $"users without santa: {count}";
                _ = await _botClient.SendTextMessageAsync(_adminId, statMsg);
                isActionDone = true;
            }

            return isActionDone;
        }

        private async void NotifyAdmin()
        {
            if (_users.Count(u => u.SantaName == null) == 1) {
                var lastUser = _users.First(u => u.SantaName == null);
                lastUser.SantaName = "admin";
                var msg = $"Вы санта у: {lastUser.Username}";
                _ = await _botClient.SendTextMessageAsync(_adminId, msg);
            }
        }

        private async Task<bool> SetAdminSettings(long id, string messageText)
        {
            if (_adminId == id) {
                if (_users.Count > 0 && messageText.ToUpper().Equals("GO")) {
                    var msg = "Можете приглашать остальных - пусть пишут мне, а я дам им жертву для санты.";
                    _isAdminListDone = true;
                    _ = await _botClient.SendTextMessageAsync(id, msg);
                    return true;
                }

                if (messageText.StartsWith("@")) {
                    var users = messageText.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var user in users) {
                        _users.Add(new User()
                        {
                            Username = user.Trim(),
                            ForbiddenUsernames = users.Where(u => u != user).ToArray()
                        });
                    }
                } else {
                    var errorMsg = "username должен начинаться с @";
                    _ = await _botClient.SendTextMessageAsync(id, errorMsg);
                }

                return true;
            }

            return false;
        }

        private string ChoosePair(string chatUsername)
        {
            var userName = "@" + chatUsername;

            var isExistUser = _users.Any(s =>
                s.Username != null && s.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
            if (!isExistUser) {
                return StringConstants.GetAlienMessage();
            }

            var isHaveSanta = _users.Any(s =>
                s.SantaName != null && s.SantaName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
            if (isHaveSanta) {
                return StringConstants.GetAnswerMessage();
            }

            var filteredUsers = _users.Where(u =>
            {
                var isFreeUser = !u.Username.Equals(userName) && u.SantaName == null;
                var forbidden =
                    u.ForbiddenUsernames.Any(f => f.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
                return isFreeUser && !forbidden;
            }).ToArray();

            if (!filteredUsers.Any()) return StringConstants.FinishMsg;

            var randomIndex = new Random().Next(filteredUsers.Length);
            var freeUser = filteredUsers[randomIndex];
            freeUser.SantaName = userName;
            return $"Ты будешь тайным сантой у {freeUser.Username}";
        }

        private async Task InitAdmin(long id, string messageText)
        {
            if (messageText.ToUpper() == START_COMMAND) {
                _adminId = id;
                _ = await _botClient.SendTextMessageAsync(_adminId, StringConstants.AdminActionMsg);
            }
        }

        private async Task AddNewUserToHelloList(long id, string username)
        {
            if (!_activeUsers.ContainsKey(id)) {
                _activeUsers.Add(id, username);
                var helloMessage = StringConstants.GetHelloMessage();
                _ = await _botClient.SendTextMessageAsync(id, helloMessage);
            }
        }
    }
}