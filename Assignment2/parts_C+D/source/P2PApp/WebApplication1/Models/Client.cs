namespace WebApplication1.Models {
    public class Client {

        //public Client(int Id) {
        //    this.Id = Id!;
        //}

        public int Id { get; set; }
        public string? IPAddress { get; set; }
        public int? Port { get; set; }
    }
}
