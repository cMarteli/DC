namespace WebAPIDataServer.Models {
    public class ErrorViewModel {
        public string? RequestId { get; set; }
        public string ErrorMessage { get; set; } = "An error occurred.";

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}