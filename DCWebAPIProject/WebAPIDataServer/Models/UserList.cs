using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.IO;
using Shared;

// This class is used to generate a list of users of size NUMBER_OF_ENTRIES
namespace WebAPIDataServer.Models {
    public class UserList
    {
        private const int NUMBER_OF_ENTRIES = 10_000;

        public static List<User> users = new List<User>();

        public static List<User> AllUsers()
        {
            return users;
        }
        public static void AddUser(User user)
        {
            users.Add(user);
        }
        public static User GetUserByAcct(uint acct)
        {
            foreach (User user in users)
            {
                if (user.acctNo == acct)
                {
                    return user;
                }
            }
            return null;
        }

        public static User GetUserByIndex(int index) {
            if (index < 0 || index >= users.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid index provided.");
            }
            return users[index];
        }

        public static User GetUserByName(string name) {
            foreach (User user in users) {
                if (string.Equals(user.firstName, name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(user.lastName, name, StringComparison.OrdinalIgnoreCase)) {
                    return user;
                }
            }
            return null;
        }

        public static void Generate()
        {
            UserGenerator generator = new UserGenerator();
            for (int i = 0; i < NUMBER_OF_ENTRIES; i++)
            {
                uint pin, acctNo;
                string firstName, lastName;
                int balance;
                byte[]? imageBytes;
                generator.GetNextAccount(out acctNo, out pin, out firstName, out lastName, out balance, out imageBytes);
                User user = new User();
                user.acctNo = acctNo;
                user.pin = pin;
                user.balance = balance;
                user.firstName = firstName;
                user.lastName = lastName;
                user.imageBytes = imageBytes;
                users.Add(user);
            }
            Console.WriteLine("Generated " + NUMBER_OF_ENTRIES + " users");
        }

        public static int GetNumberOfEntries() {
            return NUMBER_OF_ENTRIES;
        }
    }
}
