using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseDLL {
    public class ChatDatabase {
        /* Singleton using the .NET "lazy" convention to avoid threading issues */
        private static readonly Lazy<ChatDatabase> lazy = new Lazy<ChatDatabase>(() => new ChatDatabase());
        public static ChatDatabase Instance => lazy.Value;

        /* Using SortedSet for unique usernames and chatroom names */
        public SortedSet<User> Users { get; set; }
        public SortedSet<Chatroom> Chatrooms { get; set; }
        public List<PrivateChatroom> PrivateChatrooms { get; set; }

        /// Private constructor to prevent multiple instances.
        /// Initializes users, chatrooms, and private chatrooms collections.

        private ChatDatabase() {
            /* Using default string comparison for SortedSet */
            Users = new SortedSet<User>(Comparer<User>.Create((a, b) => a.Username.CompareTo(b.Username)));
            Chatrooms = new SortedSet<Chatroom>(Comparer<Chatroom>.Create((a, b) => a.Name.CompareTo(b.Name)));
            PrivateChatrooms = new List<PrivateChatroom>();
        }

        /* Methods for adding, removing, and checking existence of users */
        public bool AddUser(string username) => Users.Add(new User(username));
        public bool RemoveUser(string username) => Users.Remove(new User(username));
        public bool UserExists(string username) => Users.Contains(new User(username));

        /* Methods for chatrooms */
        public bool AddChatroom(string roomName) => Chatrooms.Add(new Chatroom(roomName));
        public bool RemoveChatroom(string roomName) => Chatrooms.Remove(new Chatroom(roomName));
        public bool ChatroomExists(string roomName) => Chatrooms.Contains(new Chatroom(roomName));

        /* Methods for private chatrooms */
        public bool AddPrivateChatroom(string roomName, User userOne, User userTwo) {
            if (!PrivateChatrooms.Any(room => room.AllowedUsers.Contains(userOne) && room.AllowedUsers.Contains(userTwo))) {
                PrivateChatrooms.Add(new PrivateChatroom(roomName, userOne, userTwo));
                return true;
            }
            return false;
        }

        public PrivateChatroom GetPrivateChatroom(User userOne, User userTwo) {
            var room = PrivateChatrooms.FirstOrDefault(r => r.AllowedUsers.Contains(userOne) && r.AllowedUsers.Contains(userTwo));
            if (room == null) throw new KeyNotFoundException("Private chatroom not found.");
            return room;
        }

        public bool PrivateChatroomExists(string roomName) {
            /* Find the private chatroom by name */
            var room = PrivateChatrooms.FirstOrDefault(r => r.Name == roomName);

            /* If found, return true; otherwise, return false */
            return room != null;
        }


        public bool RemovePrivateChatroom(string roomName) {
            /* Find the private chatroom by name */
            var roomToRemove = PrivateChatrooms.FirstOrDefault(room => room.Name == roomName);

            /* If found, remove it and return true; otherwise, return false */
            if (roomToRemove != null) {
                PrivateChatrooms.Remove(roomToRemove);
                return true;
            }
            return false;
        }


        public PrivateChatroom SearchPrivateChatroomByName(string name) {
            var room = PrivateChatrooms.FirstOrDefault(r => r.Name == name);
            if (room != null) {
                return room;
            }
            throw new KeyNotFoundException("Private chatroom not found.");
        }

        public User SearchUserByName(string name) => Users.FirstOrDefault(u => u.Username == name) ?? throw new KeyNotFoundException("User not found.");
        public Chatroom SearchChatroomByName(string name) => Chatrooms.FirstOrDefault(r => r.Name == name) ?? throw new KeyNotFoundException("Chatroom not found.");


        public string[] GetUserNames() => Users.Select(u => u.Username).ToArray();
        public string[] GetChatroomNames() => Chatrooms.Select(r => r.Name).ToArray();
        public string[] GetAllowedPrivateChatroomNames(User user) => PrivateChatrooms.Where(r => r.AllowedUsers.Contains(user)).Select(r => r.Name).OrderBy(n => n).ToArray();

        public List<Chatroom> ListChatRooms() => Chatrooms.ToList();
        

        public bool AddSharedFileToChatroom(string roomName, SharedFile file) {
            var chatroom = SearchChatroomByName(roomName);
            return chatroom.AddSharedFile(file);
        }

        public bool RemoveSharedFileFromChatroom(string roomName, string fileName) {
            var chatroom = SearchChatroomByName(roomName);
            return chatroom.RemoveSharedFile(fileName);
        }

        public SharedFile GetSharedFileFromChatroom(string roomName, string fileName) {
            var chatroom = SearchChatroomByName(roomName);
            return chatroom.GetSharedFile(fileName);
        }

        public List<SharedFile> GetAllSharedFilesFromChatroom(string roomName) {
            var chatroom = SearchChatroomByName(roomName);
            return chatroom.GetAllSharedFiles();
        }
        /* Methods for messages in chatrooms */
        public void AddMessageToChatroom(string roomName, Message message)
        {
            var chatroom = SearchChatroomByName(roomName);
            chatroom.AddMessage(message);
        }
        /* Methods for users in chatrooms */
        public bool AddUserToChatroom(User user, Chatroom chatroom)
        {
            if (!chatroom.Users.Contains(user))
            {
                chatroom.Users.Add(user);
                return true;
            }
            return false;
        }



        public bool RemoveUserFromChatroom(string username, string roomName)
        {
            Chatroom chatroom = SearchChatroomByName(roomName);
            return chatroom.RemoveUser(username);
  
        }

        /* Methods for messages in private chatrooms */
        public void AddMessageToPrivateChatroom(string usernameOne, string usernameTwo, string message)
        {
            User userOne = SearchUserByName(usernameOne);
            User userTwo = SearchUserByName(usernameTwo);
            Message msg = new Message(userOne, message);
            PrivateChatroom chatroom = GetPrivateChatroom(userOne, userTwo);
            chatroom.AddMessage(msg);
        }

        //public List<Message> ListMessagesInPrivateChatroom(string usernameOne, string usernameTwo)
        //{
        //    User userOne = SearchUserByName(usernameOne);
        //    User userTwo = SearchUserByName(usernameTwo);
        //    PrivateChatroom chatroom = GetPrivateChatroom(userOne, userTwo);
        //    return chatroom.Messages.ToList();
        //}

    }

}
