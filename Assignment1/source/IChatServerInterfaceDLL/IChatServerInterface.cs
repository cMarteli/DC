using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DatabaseDLL;

namespace IChatServerInterfaceDLL
{
    /// <summary>
    /// ChatServer Interface
    /// </summary>
    [ServiceContract]
    public interface IChatServerInterface
    {
        [OperationContract]
        bool AddUser(string username);

        [OperationContract]
        bool RemoveUser(string username);

        [OperationContract]
        bool UserExists(string username);

        [OperationContract]
        bool AddChatroom(string roomName);

        [OperationContract]
        bool RemoveChatroom(string roomName);

        [OperationContract]
        bool ChatroomExists(string roomName);

        [OperationContract]
        bool AddPrivateChatroom(string roomName, string userOne, string userTwo);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        PrivateChatroom GetPrivateChatroom(string userOne, string userTwo);

        [OperationContract]
        bool RemovePrivateChatroom(string roomName);

        [OperationContract]
        bool PrivateChatroomExists(string roomName);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        User SearchUserByName(string name);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        Chatroom SearchChatroomByName(string name);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        PrivateChatroom SearchPrivateChatroomByName(string name);

        [OperationContract]
        string[] GetUserNames();

       /* [OperationContract]
        string[] GetChatroomNames();*/

        [OperationContract]
        List<Chatroom> ListChatRooms();


        [OperationContract]
        string[] GetAllowedPrivateChatroomNames(string username);

        /* FileManagement methods */
        [OperationContract]
        SharedFile CreateSharedFile(string selectedFilePath);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        bool AddSharedFileToChatroom(string roomName, SharedFile file);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        bool RemoveSharedFileFromChatroom(string roomName, string fileName);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        SharedFile GetSharedFileFromChatroom(string roomName, string fileName);

        [OperationContract]
        [FaultContract(typeof(KeyNotFoundException))]
        List<SharedFile> GetAllSharedFilesFromChatroom(string roomName);


        [OperationContract]
        bool AddUserToChatroom(string username, string roomName);

        [OperationContract]
        bool RemoveUserFromChatroom(string username, string roomName);

        [OperationContract]
        HashSet<User> ListUsersInChatroom(string roomName);

        [OperationContract]
        List<Message> ListMessagesInChatroom(string roomName);

        [OperationContract]
        void AddMessageToChatroom(string roomName, Message message);

        [OperationContract]
        void AddMessageToPrivateChatroom(string usernameOne, string usernameTwo, string message);

        [OperationContract]
        List<Message> ListMessagesInPrivateChatroom(string usernameOne, string usernameTwo);
    }
}
