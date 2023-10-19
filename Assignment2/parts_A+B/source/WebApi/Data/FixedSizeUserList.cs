////Generates a user list with a bank account
using LocalDBWebApiUsingEF.Models;

namespace WebApi.Data
{
    public class FixedSizeUserList
    {
        private static FixedSizeUserList _instance;
        private static object _lockObject = new object();

        private List<User> _users;
        private List<BankAccount> _bankAccounts;
        private int _size;
        private UserGenerator _userGenerator;

        //private const int NUMBER_OF_ENTRIES = 1_002;
        private const int NUMBER_OF_ENTRIES = 12;

        private FixedSizeUserList(int size)
        {
            _size = size;
            _users = new List<User>(size);
            _bankAccounts = new List<BankAccount>(size);
            _userGenerator = new UserGenerator();
            //Creating two additional users for testing purposes
            GenerateTestAccounts();
            PopulateUsers();
        }

        private void GenerateTestAccounts()
        {
            _users.Add(new User("userjoe")
            {
                Name = "Joe Biden",
                Email = "biden@potus.com",
                Address = "Washington DC",
                Phone = "777-777-7777",
                Password = "userpassword",
                Picture = UserGenerator.GetImageBytes(),
                SessionID = "null"
            });
            _bankAccounts.Add(new BankAccount(1, "userjoe")
            {
                AccountHolderName = "Joe Biden's Account",
                Balance = 100000
            });
            _users.Add(new User("userdonald")
            {
                Name = "Donny Trumpas",
                Email = "donald@realdonald.com",
                Address = "Florida",
                Phone = "666-666-6666",
                Password = "userpassword",
                Picture = UserGenerator.GetImageBytes(),
                SessionID = "null"
            });
            _bankAccounts.Add(new BankAccount(2, "userdonald")
            {
                AccountHolderName = "Donald Trump's Account",
                Balance = 100000
            });
        }

        public static FixedSizeUserList GetInstance()
        {
            if (_instance == null)
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new FixedSizeUserList(NUMBER_OF_ENTRIES);
                    }
                }
            }
            return _instance;
        }

        private void PopulateUsers()
        {
            for (int i = 0; i < _size; i++)
            {
                _userGenerator.GetNextAccount(
                    out string username,
                    out string name,
                    out string email,
                    out string address,
                    out string phone,
                    out byte[] picture,
                    out string password);

                User user = new User(username)
                {
                    Name = name,
                    Email = email,
                    Address = address,
                    Phone = phone,
                    Picture = picture,
                    Password = password
                };

                // Create a bank account for the user
                BankAccount bankAccount = GenerateBankAccountForUser(user);

                _users.Add(user);
                _bankAccounts.Add(bankAccount); // Add the bank account to the bank accounts list
            }
        }

        // Generates a bank account for a given user
        private BankAccount GenerateBankAccountForUser(User user)
        {
            int accountNumber = _bankAccounts.Count + 1; // Calculate next account number based on existing accounts
            BankAccount bankAccount = new BankAccount(accountNumber, user.Username)
            {
                AccountHolderName = $"{user.Name}'s Account",
                // random balance between 0 to 10000 with 2 decimal places
                Balance = Math.Round(new Random().NextDouble() * 10000, 2)
            };

            return bankAccount;
        }

        public List<User> GetUsers()
        {
            return _users;
        }

        public List<BankAccount> GetBankAccounts()
        {
            return _bankAccounts;
        }

        public User GetUser(int index)
        {
            return _users[index];
        }
    }
}
