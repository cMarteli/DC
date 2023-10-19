using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DatabaseDLL {

    /// Represents a private chatroom that extends the Chatroom class.
    /// This chatroom is designed for direct messaging between two users.

    [DataContract]
    public class PrivateChatroom : Chatroom {

        /// Gets or sets the allowed users in the private chatroom.
        [DataMember]
        public HashSet<User> AllowedUsers { get; set; } = new HashSet<User>();


        /// Initializes a new instance of the PrivateChatroom class with a given name.

        public PrivateChatroom(string name) : base(name) {
        }

        /// Initializes a new instance of the PrivateChatroom class with a given name and two users.

        public PrivateChatroom(string roomname, User userOne, User userTwo) : base(roomname) {
            AddAllowedUser(userOne);
            AddAllowedUser(userTwo);
        }

        /// Adds a user to the allowed users list.

        public void AddAllowedUser(User user) {
            if (AllowedUsers.Count >= 2) {
                throw new ArgumentException("Private chatroom already at capacity.");
            }
            AllowedUsers.Add(user);
        }


        /// Determines whether the specified object is equal to the current object.

        public override bool Equals(object obj) {
            return obj is PrivateChatroom chatroom &&
                   base.Equals(obj) &&
                   AllowedUsers.SetEquals(chatroom.AllowedUsers);
        }

        /// Serves as the default hash function.

        public override int GetHashCode() {
            return base.GetHashCode() ^ AllowedUsers.GetHashCode();
        }

        /// Returns a string that represents the current object.
        public override string ToString() {
            return $"PrivateChatroom: {Name}, Allowed Users: {string.Join(", ", AllowedUsers.Select(u => u.Username))}";
        }
    }
}
