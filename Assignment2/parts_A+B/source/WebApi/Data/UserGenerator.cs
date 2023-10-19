using System.Drawing;
using System.Text;

namespace WebApi.Data
{
    public class UserGenerator
    {
        private static Random random = new Random();
        private HashSet<string> generatedUsernames = new HashSet<string>();
        private HashSet<string> generatedEmails = new HashSet<string>();
        private HashSet<int> generatedAccountNumbers = new HashSet<int>();

        private const int MAX_ACCT_DIGITS = 6;
        private const int MAX_PIN_DIGITS = 4;
        private const int MIN_BALANCE = 100;
        private const int MAX_BALANCE = 100_000;
        private const int BITMAP_SIZE = 8; // nxn bitmap

        private const string EMAIL_DOMAIN = "examplemail.com";
        private static readonly string[] streetNames = { "Maple", "Elm", "Oak", "Pine", "Birch" };
        private static readonly string[] streetTypes = { "St", "Ave", "Rd", "Blvd", "Ln" };


        public void GetNextAccount(out string username, out string name, out string email, out string address, out string phone,
            out byte[] picture, out string password)
        {
            string alias = GenerateRandomName(5); //used to generate unique username and email

            username = GenerateUniqueUsername(alias);
            name = GetName(6, 8);
            email = GenerateUniqueEmail(alias);
            address = GenerateAddress();
            phone = GeneratePhoneNumber();
            password = GetPIN().ToString(); // PIN is used as password TODO: may need to hash this?
            picture = GetImageBytes();

        }

        private string GetName(int fNameSize, int lNameSize)
        {
            return GenerateRandomName(fNameSize) + " " + GenerateRandomName(lNameSize);
        }

        private uint GetPIN()
        {
            return GenerateRandomNDigitInteger(MAX_PIN_DIGITS);
        }

        private string GenerateUniqueEmail(string baseName)
        {
            string email = $"{baseName.ToLower()}@{EMAIL_DOMAIN}";
            int attempts = 0;

            while (generatedEmails.Contains(email) && attempts < 100)
            {
                email = $"{baseName.ToLower()}{random.Next(1, 10000)}@{EMAIL_DOMAIN}";
                attempts++;
            }

            if (attempts == 100)
            {
                throw new Exception("Unable to generate a unique email.");
            }

            generatedEmails.Add(email);
            return email;
        }

        private string GenerateUniqueUsername(string baseName)
        {
            string username = baseName.ToLower();
            int attempts = 0;

            while (generatedUsernames.Contains(username) && attempts < 100)
            {
                username = $"{baseName.ToLower()}{random.Next(1, 10000)}";
                attempts++;
            }

            if (attempts == 100)
            {
                throw new Exception("Unable to generate a unique username.");
            }

            generatedUsernames.Add(username);
            return username;
        }

        public double GetBalance()
        {
            double balance = random.Next(MIN_BALANCE, MAX_BALANCE) + random.NextDouble(); // Generate value between MIN_BALANCE to (MAX_BALANCE + 0.999999...)
            return Math.Round(balance, 2);  // Round the balance to 2 decimal places
        }

        public int GetUniqueAcctNo()
        {
            int acctNo;
            do
            {
                acctNo = (int)GenerateRandomNDigitInteger(MAX_ACCT_DIGITS);
            } while (generatedAccountNumbers.Contains(acctNo));

            generatedAccountNumbers.Add(acctNo);
            return acctNo;
        }

        public static byte[] GetImageBytes()
        {
            return BitmapToByteArray(GenerateBitmap(BITMAP_SIZE));
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

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private static Bitmap GenerateBitmap(int bSize)
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

        private string GeneratePhoneNumber()
        {
            string part1 = random.Next(100, 999).ToString();
            string part2 = random.Next(100, 999).ToString();
            string part3 = random.Next(1000, 9999).ToString();
            return $"{part1}-{part2}-{part3}";
        }

        private string GenerateAddress()
        {
            string streetNum = random.Next(1, 9999).ToString();
            string streetName = streetNames[random.Next(streetNames.Length)];
            string streetType = streetTypes[random.Next(streetTypes.Length)];
            return $"{streetNum} {streetName} {streetType}";
        }


    }
}
