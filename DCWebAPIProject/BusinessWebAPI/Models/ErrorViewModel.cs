namespace BusinessWebAPI.Models {
    public class ErrorViewModel {
        public string? RequestId { get; set; }
        public string ErrorMessage { get; set; } = "An error occurred."; // Added this line

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}