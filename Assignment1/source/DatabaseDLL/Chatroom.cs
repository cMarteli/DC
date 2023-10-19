/* Chatroom.cs
 * Holds information for a chatroom.
 * Including shared files
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DatabaseDLL {

    /// Represents a chatroom with users, messages, and shared files.

    [DataContract]
    public class Chatroom {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public HashSet<User> Users { get; set; } = new HashSet<User>();

        [DataMember]
        public List<Message> Messages { get; set; } = new List<Message>();

        [DataMember]
        public List<SharedFile> SharedFiles { get; set; } = new List<SharedFile>();

        public Chatroom(string name) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// Adds a shared file to the chatroom.
        public bool AddSharedFile(SharedFile file) {
            if (file == null) return false;
            SharedFiles.Add(file);
            return true;
        }

        /// Removes a shared file from the chatroom by its name.
        public bool RemoveSharedFile(string fileName) {
            var file = SharedFiles.FirstOrDefault(f => f.FileName == fileName);
            return file != null && SharedFiles.Remove(file);
        }


        /// Retrieves a shared file from the chatroom by its name.
        public SharedFile GetSharedFile(string fileName) => SharedFiles.FirstOrDefault(f => f.FileName == fileName);

        /// Retrieves all shared files in the chatroom.

        public List<SharedFile> GetAllSharedFiles() => SharedFiles;


        /// Adds a user to the chatroom.
        public bool AddUser(string username) => Users.Add(new User(username));

        /// Removes a user from the chatroom.
        public bool RemoveUser(string username) => Users.Remove(new User(username));

   
        /// Adds a message to the chatroom.
        public void AddMessage(Message message) {
            if (message != null) {
                Messages.Add(message);
            }
        }


        /// Determines whether the specified object is equal to the current object.

        public override bool Equals(object obj) {
            return obj is Chatroom chatroom && Name == chatroom.Name;
        }


        /// Serves as the default hash function.

        public override int GetHashCode() {
            return Name != null ? Name.GetHashCode() : 0;
        }

        /// Returns a string that represents the current object.

        public override string ToString() {
            return $"Chatroom: {Name}, Users: {Users.Count}, Messages: {Messages.Count}, SharedFiles: {SharedFiles.Count}";
        }



    }
}
