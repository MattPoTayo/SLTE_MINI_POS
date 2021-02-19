﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLTE_MINI_POS.Model
{
    public class Transaction
    {
        public long ID { get; set; } = 0;
        public List<Product> productlist { get; set; } = new List<Product>();
        public string TransactionNo { get; set; } = "";
        public string InvoiceNumber { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;

        public Transaction()
        {
            productlist = new List<Product>();
        }

        public decimal GetTotalAmountDue()
        {
            return productlist.Sum(x => x.Price * x.Qty);
        }
        public decimal GetTotalQty()
        {
            return productlist.Sum(x => x.Qty);        }
    }
}
