namespace Semestralka_Bruzek
{
    public class InvoiceItem
    {
        public int InvoiceID { get; set; }  
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
