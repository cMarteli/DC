using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Shared;

// Contains methods to generate random users
namespace WebAPIDataServer.Models {
    public class UserGenerator
    {

        private static Random random = new Random();
        private HashSet<uint> generatedAccountNumbers = new HashSet<uint>();

        private const int NAME_SIZE = 10;
        private const int MAX_ACCT_DIGITS = 6;
        private const int MAX_PIN_DIGITS = 4;
        private const int MIN_BALANCE = 100;
        private const int MAX_BALANCE = 100_000;
        private const int BITMAP_SIZE = 4;

        public void GetNextAccount(out uint acctNo, out uint pin, out string firstName, out string lastName, out int balance, out byte[] imageBytes)
        {
            acctNo = GetUniqueAcctNo();
            pin = GetPIN();
            firstName = GetFirstName();
            lastName = GetLastName();
            balance = GetBalance();
            imageBytes = GetImageBytes();
        }

        private string GetFirstName()
        {
            return GenerateRandomName(NAME_SIZE);
        }

        private string GetLastName()
        {
            return GenerateRandomName(NAME_SIZE);
        }

        private uint GetPIN()
        {
            return GenerateRandomNDigitInteger(MAX_PIN_DIGITS);
        }

        private uint GetUniqueAcctNo()
        {
            uint acctNo;
            do
            {
                acctNo = GenerateRandomNDigitInteger(MAX_ACCT_DIGITS);
            } while (generatedAccountNumbers.Contains(acctNo));

            generatedAccountNumbers.Add(acctNo);
            return acctNo;
        }

        private int GetBalance()
        {
            return random.Next(MIN_BALANCE, MAX_BALANCE);
        }

        //Generates a bitmap image and serializes it to a byte array
        private byte[] GetImageBytes()
        {
            GenerateBitmap(BITMAP_SIZE); //first generate a bitmap by the default dimensions
            return BitmapToByteArray(GenerateBitmap(BITMAP_SIZE)); //return the serialized bitmap
        }

        private string GenerateRandomName(int maxSize)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < maxSize; i++)
            {
                char randomChar = (char)('a' + random.Next(26));
                stringBuilder.Append(randomChar);
            }
            return stringBuilder.ToString();
        }

        private uint GenerateRandomNDigitInteger(int numDigits)
        {
            if (numDigits <= 0)
            {
                throw new ArgumentException("Number of digits must be greater than 0.");
            }

            int minValue = (int)Math.Pow(10, numDigits - 1);
            int maxValue = (int)Math.Pow(10, numDigits);

            return (uint)random.Next(minValue, maxValue);
        }

        /** Helper method to serialize bitmap image **/
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private Bitmap GenerateBitmap(int bSize)
        {
            Bitmap image = new(bSize, bSize);
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color newColor = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    image.SetPixel(x, y, newColor);
                }
            }
            return image;
        }
    }
}
