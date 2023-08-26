using System.Collections.Generic;
using System.Drawing;
using System;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace DCDatabase {
    /** Singleton using the .NET "lazy" convention to avoid threading issues **/
    public class DatabaseClass {
        private const int NUMBER_OF_ENTRIES = 100_000;
        private static readonly Lazy<DatabaseClass> lazy = new Lazy<DatabaseClass>(() => new DatabaseClass());
        public static DatabaseClass Instance => lazy.Value;

        private readonly List<DataStruct> cachedData;

        private DatabaseClass() {
            cachedData = GenerateUserData();
        }

        /** Helper method to serialize bitmap image **/
        public static byte[] BitmapToByteArray(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private List<DataStruct> GenerateUserData() {
            List<DataStruct> ds = new List<DataStruct>();
            DBGenerator generator = new DBGenerator();

            for (int i = 0; i < NUMBER_OF_ENTRIES; i++) {
                generator.GetNextAccount(out uint pin, out uint acctNo, out string firstName,
                    out string lastName, out int balance, out Bitmap image);

                DataStruct entry = new DataStruct {
                    acctNo = acctNo,
                    pin = pin,
                    balance = balance,
                    firstName = firstName,
                    lastName = lastName,
                    imageBytes = BitmapToByteArray(image) //serialization happens here
                };

                ds.Add(entry);
            }

            return ds;
        }

        /** Caches data so generated values are the same **/
        public List<DataStruct> UserData() {
            return cachedData;
        }
    }
}
