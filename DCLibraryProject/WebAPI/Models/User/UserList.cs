using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.IO;

namespace WebAPI.Models.User {
    public class UserList {
        private const int NUMBER_OF_ENTRIES = 1_000;
        private static readonly Lazy<UserList> lazy =
            new Lazy<UserList>(() => new UserList(), true); // Thread-safe lazy initialization
        public static UserList Instance => lazy.Value;

        private readonly ConcurrentDictionary<uint, User> cachedData;

        private UserList() {
            cachedData = new ConcurrentDictionary<uint, User>(GenerateUserData());
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static void Generate() {
            var _ = UserList.Instance.UserData(); // Call to trigger initialization if needed
        }

        private IDictionary<uint, User> GenerateUserData() {
            var ds = new Dictionary<uint, User>();
            UserGenerator generator = new UserGenerator();

            for (int i = 0; i < NUMBER_OF_ENTRIES; i++) {
                generator.GetNextAccount(out uint pin, out uint acctNo, out string firstName,
                    out string lastName, out int balance, out Bitmap image);

                User entry = new User {
                    acctNo = acctNo,
                    pin = pin,
                    balance = balance,
                    firstName = firstName,
                    lastName = lastName,
                    imageBytes = BitmapToByteArray(image)
                };

                ds.Add(acctNo, entry);
            }

            return ds;
        }

        public IEnumerable<User> UserData() {
            return cachedData.Values;
        }

        public void AddUser(User newUser) {
            if (!cachedData.TryAdd(newUser.acctNo, newUser)) {
                throw new ArgumentException($"User with account number {newUser.acctNo} already exists.");
            }
        }

        public bool DeleteUser(uint acctNo) {
            return cachedData.TryRemove(acctNo, out _);
        }

        public User GetUserByAcct(uint acctNo) {
            if (cachedData.TryGetValue(acctNo, out User user)) {
                return user;
            }
            return null;
        }
    }
}
