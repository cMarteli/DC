using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.IO;

namespace WebAPI.Models.User {
    public class UserList {
        private const int NUMBER_OF_ENTRIES = 1_00;

        public static List<User> users = new List<User>();

        public static List<User> AllUsers() {
            return users;
        }
        public static void AddUser(User user) {
            users.Add(user);
        }
        public static User GetUserByAcct(uint acct) {
            foreach (User user in users) {
                if (user.acctNo == acct) {
                    return user;
                }
            }
            return null;
        }
        public static void Generate() {
            UserGenerator generator = new UserGenerator();
            for (int i = 0; i < NUMBER_OF_ENTRIES; i++) {
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
        }
    }
}
