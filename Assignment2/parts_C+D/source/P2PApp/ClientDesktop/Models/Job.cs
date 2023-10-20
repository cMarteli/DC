using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace ClientDesktop.Models {
    [DataContract]
    public class Job {
        private readonly string encodedPythonCode;
        private readonly string hash;
        private readonly object lockObject = new object();

        public Guid Id { get; set; }
        public bool IsCompleted { get; private set; }
        public object Result { get; private set; }

        public Job(string pyCode) {
            Id = Guid.NewGuid(); // Generate new unique identifier
            encodedPythonCode = Encode64(pyCode);
            hash = GenerateHash(pyCode); // Generate hashedBytes
            IsCompleted = false;
            Result = null;
        }

        private string Encode64(string str) {
            byte[] encodedDataAsBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encodedDataAsBytes);
        }

        private string Decode64(string base64Str) {
            byte[] encodedDataAsBytes = Convert.FromBase64String(base64Str);
            return Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        // Generate hashedBytes using SHA256
        private string GenerateHash(string input) {
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++) {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Verify the hashedBytes
        public bool VerifyHash(string input) {
            return GenerateHash(input) == hash;
        }

        public string GetDecodedJobCode() {
            return Decode64(encodedPythonCode);
        }

        public void SetResult(object result) {
            lock (lockObject) {
                IsCompleted = true;
                Result = result ?? throw new ArgumentNullException(nameof(result)); // Throw exception if result is null
            }
        }
    }
}
