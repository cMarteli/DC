using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models {
    public class ErrorViewModel {
            public string? RequestId { get; set; }
            public string ErrorMessage { get; set; } = "An error occurred.";

            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
