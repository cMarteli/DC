namespace WebApi.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string Action { get; set; }
        public string OriginalValues { get; set; }
        public string CurrentValues { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
