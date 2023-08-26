using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DCLibraryProject {
    internal class DBGenerator {

        private static Random random = new Random();

        private const int NAME_SIZE = 10;
        private const int MAX_PIN_DIGITS = 4;
        private const int MAX_ACCT_DIGITS = 6;
        private const int MIN_BALANCE = 100;
        private const int MAX_BALANCE = 100_000;
        private const int BITMAP_SIZE = 4;

        //randomly generates a first name string
        private string GetFirstName() {
            return GenerateRandomName(NAME_SIZE);
        }

        //reuses the first name method
        private string GetLastName() {
            return GenerateRandomName(NAME_SIZE);
        }

        private uint GetPIN() {
            return GenerateRandomNDigitInteger(MAX_PIN_DIGITS);
        }
        private uint GetAcctNo() {
            return GenerateRandomNDigitInteger(MAX_ACCT_DIGITS);
        }

        private int GetBalance() {
            return random.Next(MIN_BALANCE, MAX_BALANCE);
        }

        private Bitmap GetBitmap() {
            return GenerateBitmap(BITMAP_SIZE);
        }

        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out Bitmap image) {
            pin = GetPIN();
            acctNo = GetAcctNo();
            firstName = GetFirstName();
            lastName = GetLastName();
            balance = GetBalance();
            image = GetBitmap();

        }

        //helper method to generate names
        //INPUT: number of letters in generated name
        private string GenerateRandomName(int maxSize) {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < maxSize; i++) {
                char randomChar = (char)('a' + random.Next(26));
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
        }

        //helper method to generate numerical values
        //INPUT: number of digits in generated number
        private uint GenerateRandomNDigitInteger(int numDigits) {
            if (numDigits <= 0) {
                throw new ArgumentException("Number of digits must be greater than 0.");
            }

            int minValue = (int)Math.Pow(10, numDigits - 1);
            int maxValue = (int)Math.Pow(10, numDigits);

            return (uint)random.Next(minValue, maxValue);
        }
        /** Generates a bitmap image with random pixel rgb values **/
        private Bitmap GenerateBitmap(int bSize) {
            Bitmap image = new Bitmap(bSize, bSize);
            int x, y;
            // Loop through the images pixels to randomize color.
            for (x = 0; x < image.Width; x++) {
                for (y = 0; y < image.Height; y++) {
                    Color pixelColor = image.GetPixel(x, y);
                    Color newColor = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    image.SetPixel(x, y, newColor);
                }
            }

            return image;

        }



    }
}
