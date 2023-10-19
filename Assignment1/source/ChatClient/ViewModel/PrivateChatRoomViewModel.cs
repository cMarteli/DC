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
    

    internal class PrivateChatRoomViewModel
    {

        public ChatDatabase _database;

        public ObservableCollection<PrivateChatroom> PrivateChatrooms { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<User> AllowedUsers { get; set; }

        public ObservableCollection<SharedFile> SharedFiles { get; set; } = new ObservableCollection<SharedFile>();
        public PrivateChatRoomViewModel()
        {
            _database = ChatDatabase.Instance;


            PrivateChatrooms = new ObservableCollection<PrivateChatroom>();
            Users = new ObservableCollection<User>();
            Messages = new ObservableCollection<Message>();
            AllowedUsers = new ObservableCollection<User>();

        }


    }
}

