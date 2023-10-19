////model to accept transfer-related parameters
namespace WebApp.Models
{
    public class TransferRequest
    {
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
    }
}
