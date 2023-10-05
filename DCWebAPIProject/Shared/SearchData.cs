using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shared {
    public class SearchData {
        [JsonProperty("searchStr")]
        public string SearchStr { get; set; }
    }
}
