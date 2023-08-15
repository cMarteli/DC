using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLibraryProject
{
    internal class DBGenerator
    {

        private static Random random = new Random();

        private const int NAME_SIZE = 10;
        private const int MAX_PIN_DIGITS = 6;
        private const int MAX_ACCT_DIGITS = 12;
        private const int MIN_BALANCE = 100;
        private const int MAX_BALANCE = 100_000;

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
        private string GetAcctNo() {
            return GenerateRandomNDigitInteger(MAX_ACCT_DIGITS).ToString();
        }

        private int GetBalance() {
            return random.Next(MIN_BALANCE, MAX_BALANCE);
        }

        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance) {
            pin = GetPIN();
            acctNo = uint.Parse(GetAcctNo());
            firstName = GetFirstName();
            lastName = GetLastName();
            balance = GetBalance();
        }

        //helper method to generate names
        //INPUT: number of letters in generated name
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

        //helper method to generate numerical values
        //INPUT: number of digits in generated number
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



    }
}
