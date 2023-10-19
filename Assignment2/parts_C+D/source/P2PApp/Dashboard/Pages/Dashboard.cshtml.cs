using Dashboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard.Pages {
    public class DashboardModel : PageModel {
        public List<dynamic> Clients { get; set; }

        public void OnGet() {
            var clientService = new ClientService();
            Clients = clientService.GetClients();
        }
    }
}
