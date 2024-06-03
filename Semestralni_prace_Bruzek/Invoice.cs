using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semestralka_Bruzek
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerICO { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
