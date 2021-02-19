using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLTE_MINI_POS.Model
{
    public class Product
    {
        public long ID { get; set; } = 0;
        public string Name { get; set; } = "";
        public string StockNo { get; set; } = "";
        public string Barcode { get; set; } = "";
        public decimal Price { get; set; } = 0;
        public decimal Qty { get; set; } = 0;
        public int Show { get; set; } = 1;
        public Product()
        {
        }
    }
}
