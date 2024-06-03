using System.Collections.Generic;

namespace Semestralka_Bruzek
{
    public class VAT
    {
        public string Name { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal VatPercentageForCalculation { get; set; }

        public VAT(string name, int vatPercentage, decimal vatPercentageForCalculation)
        {
            Name = name;
            VatPercentage = vatPercentage;
            VatPercentageForCalculation = vatPercentageForCalculation; 
        }
    }

    public static class VatRates
    {
        public static List<VAT> DefaultRates = new List<VAT>
        {
            new VAT("0 %", 0,0),
            new VAT("12 %", 12,0.12m),
            new VAT("21 %", 21,0.21m)
        };
    }
}
