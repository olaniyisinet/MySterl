using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doubble.DTOs
{
    public class WorkFlowDto
    {
        public string AccountNumber { get; set; }
        public string Channel { get; set; }
        public string CustomerID { get; set; }
        public DateTime Date { get; set; }
    }
}