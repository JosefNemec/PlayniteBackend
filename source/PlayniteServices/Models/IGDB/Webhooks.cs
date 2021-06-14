using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayniteServices.Models.IGDB
{
    public class Webhook
    {
        public int id { get; set; }
        public string url { get; set; }
        public int category { get; set; }
        public int sub_category { get; set; }
        public bool active { get; set; }
        public int number_of_retries { get; set; }
        public string api_key { get; set; }
        public string secret { get; set; }
        public int created_at { get; set; }
        public int updated_at { get; set; }
    }
}
