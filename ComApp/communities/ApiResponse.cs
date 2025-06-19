using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comApp.communities
{
    public class ApiResponse
    {
        public bool IsSuccessStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Content { get; set; }
    }
}

