using DatabaseDLL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModel
{
    

    internal class ChatRoomViewModel
    {

        public ChatDatabase _database;

        public ObservableCollection<Chatroom> Chatrooms { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Message> Messages { get; set; }

        public ObservableCollection<SharedFile> SharedFiles { get; set; } = new ObservableCollection<SharedFile>();
        public ChatRoomViewModel()
        {
            _database = ChatDatabase.Instance;


            Chatrooms = new ObservableCollection<Chatroom>();
            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<Message>();


        }


    }
}

