using DatabaseDLL;
using IChatServerInterfaceDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    internal class ChatServer : IChatServerInterface
    {
        ChatDatabase db;

        /// ChatServer Constructor. 
        public ChatServer()
        {
            db = ChatDatabase.Instance;
        }
        /* Methods for managing users */
        public bool AddUser(string username)
        {
            Console.WriteLine($"Attempting to add user: {username}");
            bool result = db.AddUser(username);
            Console.WriteLine($"Add user result: {result}");
            return result;
        }

        public bool RemoveUser(string username)
        {
            Console.WriteLine($"Attempting to remove user: {username}");
            bool result = db.RemoveUser(username);
            Console.WriteLine($"Remove user result: {result}");
            return result;
        }

        public bool UserExists(string username)
        {
            Console.WriteLine($"Checking if user exists: {username}");
            bool result = db.UserExists(username);
            Console.WriteLine($"User exists result: {result}");
            return result;
        }

        public bool AddChatroom(string roomName)
        {
            Console.WriteLine($"Attempting to add chatroom: {roomName}");
            bool result = db.AddChatroom(roomName);
            Console.WriteLine($"Add chatroom result: {result}");
            return result;
        }

        public bool RemoveChatroom(string roomName)
        {
            Console.WriteLine($"Attempting to remove chatroom: {roomName}");
            bool result = db.RemoveChatroom(roomName);
            Console.WriteLine($"Remove chatroom result: {result}");
            return result;
        }

        public bool ChatroomExists(string roomName)
        {
            Console.WriteLine($"Checking if chatroom exists: {roomName}");
            bool result = db.ChatroomExists(roomName);
            Console.WriteLine($"Chatroom exists result: {result}");
            return result;
        }


        /* Methods for managing private chatrooms */
        public bool AddPrivateChatroom(string roomName, string usernameOne, string usernameTwo)
        {
            Console.WriteLine($"Attempting to add private chatroom: {roomName} between {usernameOne} and {usernameTwo}");
            User userOne = null;
            User userTwo = null;

            try
            {
                userOne = db.SearchUserByName(usernameOne);
            }
            catch (KeyNotFoundException e)
            {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("First user not found."));
            }
            try
            {
                userTwo = db.SearchUserByName(usernameTwo);
            }
            catch (KeyNotFoundException e)
            {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("Second user not found."));
            }

            if (userOne != null && userTwo != null)
            {
                bool result = db.AddPrivateChatroom(roomName, userOne, userTwo);
                Console.WriteLine($"Add private chatroom result: {result}");
                return result;
            }
            Console.WriteLine("Add private chatroom failed.");
            return false;
        }

   
        /// Get private chatroom based on two users. If no private chatroom
        /// exists between two users, throw KeyNotFoundException.
        public PrivateChatroom GetPrivateChatroom(string usernameOne, string usernameTwo)
        {
            Console.WriteLine($"Attempting to get private chatroom between {usernameOne} and {usernameTwo}");
            User userOne = null;
            User userTwo = null;

            try
            {
                userOne = db.SearchUserByName(usernameOne);
            }
            catch (KeyNotFoundException e)
            {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("First user not found."));
            }
            try
            {
                userTwo = db.SearchUserByName(usernameTwo);
            }
            catch (KeyNotFoundException e)
            {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("Second user not found."));
            }

            try
            {
                PrivateChatroom chatroom = db.GetPrivateChatroom(userOne, userTwo);
                Console.WriteLine("Successfully retrieved private chatroom.");
                return chatroom;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Failed to get private chatroom. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }
        }

        public bool RemovePrivateChatroom(string roomName)
        {
            Console.WriteLine($"Attempting to remove private chatroom: {roomName}");
            bool result = db.RemovePrivateChatroom(roomName);
            Console.WriteLine($"Remove private chatroom result: {result}");
            return result;
        }

        public bool PrivateChatroomExists(string roomName)
        {
            Console.WriteLine($"Checking if private chatroom exists: {roomName}");
            bool result = db.PrivateChatroomExists(roomName);
            Console.WriteLine($"Private chatroom exists result: {result}");
            return result;
        }


        /// Search for User by string. Either return User or throw
        public User SearchUserByName(string username)
        {
            try
            {
                User user = db.SearchUserByName(username);
                Console.WriteLine("User found.");
                return user;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"User not found. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }
        }


        /// Search for Chatroom by string. Either return Chatroom or throw
        public Chatroom SearchChatroomByName(string name)
        {
            Console.WriteLine($"Searching for chatroom by name: {name}");
            try
            {
                Chatroom chatroom = db.SearchChatroomByName(name);
                Console.WriteLine("Chatroom found.");
                return chatroom;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Chatroom not found. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }
        }


        /// Search for PrivateChatroom by string. Either return PrivateChatroom or throw
        public PrivateChatroom SearchPrivateChatroomByName(string name)
        {
            try
            {
                PrivateChatroom chatroom = db.SearchPrivateChatroomByName(name);
                Console.WriteLine("Private chatroom found.");
                return chatroom;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Private chatroom not found. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }
        }

        public string[] GetUserNames()
        {
            return db.GetUserNames();
        }


        public List<Chatroom> ListChatRooms() { return db.ListChatRooms(); }


        /// Search for User by string. If successful, get allowed private chatroom
        public string[] GetAllowedPrivateChatroomNames(string username)
        {
            User user;
            try
            {
                user = db.SearchUserByName(username);
                return db.GetAllowedPrivateChatroomNames(user);
            }
            catch (KeyNotFoundException e)
            {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }
        }

        /* FileManagement methods */

        /// <summary>
        /// Creates a sharedfile
        /// </summary>
        /// <param name="selectedFilePath"></param>
        /// <returns></returns>
        public SharedFile CreateSharedFile(string selectedFilePath) {
            string fileExtension = System.IO.Path.GetExtension(selectedFilePath);

            byte[] fileData = null;
            string fileType = "";

            if (fileExtension == ".txt") {
                fileType = "text";
                string fileContent = File.ReadAllText(selectedFilePath);
                fileData = Encoding.UTF8.GetBytes(fileContent);
            }
            else if (fileExtension == ".bmp") {
                fileType = "bitmap";
                // Use the helper method to convert the bitmap to a byte array
                using (Bitmap bitmap = new Bitmap(selectedFilePath)) {
                    fileData = BitmapToByteArray(bitmap);
                }
            }

            // Create a new shared file
            SharedFile newFile = new SharedFile {
                FileName = Path.GetFileName(selectedFilePath),
                FileType = fileType,
                FileData = fileData
            };
            return newFile;
        }

        /** Helper method to serialize bitmap image **/
        private byte[] BitmapToByteArray(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }


        /// Add File to Chatroom

        public bool AddSharedFileToChatroom(string roomName, SharedFile file) {
            try {
                return db.AddSharedFileToChatroom(roomName, file);
            } catch (KeyNotFoundException e) {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("Chatroom not found."));
            }
        }

        /// Remove file from chatroom

        public bool RemoveSharedFileFromChatroom(string roomName, string fileName) {
            try {
                return db.RemoveSharedFileFromChatroom(roomName, fileName);
            } catch (KeyNotFoundException e) {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("Chatroom not found."));
            }
        }


        /// Get shared file from chatroom
 
        public SharedFile GetSharedFileFromChatroom(string roomName, string fileName) {
            try {
                return db.GetSharedFileFromChatroom(roomName, fileName);
            } catch (KeyNotFoundException e) {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("Chatroom or File not found."));
            }
        }

        /// Get all shared files from chatroom

        public List<SharedFile> GetAllSharedFilesFromChatroom(string roomName) {
            Console.WriteLine($"Attempting to add shared file to chatroom: {roomName}");
            try {
                return db.GetAllSharedFilesFromChatroom(roomName);
            } catch (KeyNotFoundException e) {
                throw new FaultException<KeyNotFoundException>(e, new FaultReason("Chatroom not found."));
            }
        }
        /* Methods for managing users in chatrooms */
        public bool AddUserToChatroom(string username, string roomName)
        {
            var user = SearchUserByName(username);
            var chatroom = SearchChatroomByName(roomName);
            return chatroom.Users.Add(user);
        }

        public bool RemoveUserFromChatroom(string username, string roomName)
        {
            Console.WriteLine($"Attempting to remove user: {username}");
            bool result = db.RemoveUserFromChatroom(username, roomName);
            Console.WriteLine($"Remove user result: {result}");
            return result;
        }

        public HashSet<User> ListUsersInChatroom(string roomName)
        {
            // Search for the chatroom by its name
            Chatroom chatroom;
            try
            {
                chatroom = db.SearchChatroomByName(roomName);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Chatroom {roomName} not found. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }

            // Return the list of users in that chatroom
            return chatroom.Users;
        }
        /* Methods for managing messages in chatrooms */
        public void AddMessageToChatroom(string roomName, Message message)
        {
            var chatroom = SearchChatroomByName(roomName);
            chatroom.AddMessage(message);
        }


        /* Methods for managing messages in chatrooms */
        public void AddMessageToPrivateChatroom(string usernameOne, string usernameTwo, string message)
        {
            db.AddMessageToPrivateChatroom(usernameOne, usernameTwo, message);
        }

        public List<Message> ListMessagesInChatroom(string roomName)
        {
            // Search for the chatroom by its name
            Chatroom chatroom;
            try
            {
                chatroom = db.SearchChatroomByName(roomName);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Chatroom {roomName} not found. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }

            // Return the list of users in that chatroom
            return chatroom.Messages;
        }

        public List<Message> ListMessagesInPrivateChatroom(string usernameOne, string usernameTwo)
        {
            // Return the list of users in that chatroom
            PrivateChatroom privateChatroom;
            try
            {
                User userOne = db.SearchUserByName(usernameOne);
                User userTwo = db.SearchUserByName(usernameTwo);
                privateChatroom = db.GetPrivateChatroom(userOne, userTwo);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"Private Chatroom not found. Error: {e.Message}");
                throw new FaultException<KeyNotFoundException>(e, new FaultReason(e.Message));
            }

            // Return the list of users in that chatroom
            return privateChatroom.Messages;
            //return db.ListMessagesInPrivateChatroom(usernameOne, usernameTwo);
        }


    }
}
