using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SLTE_MINI_POS.Model;

namespace SLTE_MINI_POS.Helpers
{
    public static class DataHandler
    {

        public static bool UpdateProduct(Product prod)
        {
            try
            {
                DataBaseHelper.SetDB(string.Format(@"UPDATE Product
                                        SET productname = '{0}',
	                                        stockno ='{1}',
	                                        price = '{2}',
                                            discontinued = '{3}'
                                        WHERE id = '{4}'",  prod.Name, prod.StockNo, prod.Price, prod.Show, prod.ID));
                return true;
            }
            catch(Exception exe)
            {
                MessageBox.Show(exe.Message);
                return false;
            }
        }
        public static bool SaveProduct(Product prod)
        {
            try
            {
                string barcode = GetNextBarcode();

                DataBaseHelper.SetDB(string.Format(@"INSERT INTO Product(barcode, productname, stockno, price)
                    VALUES('{0}','{1}','{2}','{3}')", barcode, prod.Name, prod.StockNo, prod.Price));
                return true;
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.Message);
                return false;
            }

        }
        public static bool SaveTransaction(Transaction trans)
        {
            try
            {
                string transNo  = GetNextTransNumber();
                string idNo = GetNextID("TransactionHead");

                DataBaseHelper.SetDB(string.Format(@"INSERT INTO TransactionHead(transactionnumber, invoicenumber, transdate)
                    VALUES('{0}','{1}','{2}')", transNo, trans.InvoiceNumber, trans.Date));

                foreach(Product prod in trans.productlist)
                {
                    DataBaseHelper.SetDB(string.Format(@"INSERT INTO TransactionDetail(headid, productid, quantity, price)
                    VALUES('{0}','{1}','{2}', '{3}')", idNo, prod.ID, prod.Qty, prod.Price));
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public static string GetNextID(string table)
        {
            DataTable dt = DataBaseHelper.GetDB(string.Format(@"SELECT id FROM {0} ORDER BY id DESC LIMIT 1", table));
            if (dt.Rows.Count == 0)
                return "1";
            else
            {
                string test = dt.Rows[0]["id"].ToString();
                long idraw = Convert.ToInt64(test);
                return (idraw + 1).ToString();
            }
        }
        public static string GetNextBarcode()
        {
            DataTable dt = DataBaseHelper.GetDB("SELECT barcode FROM Product ORDER BY barcode DESC LIMIT 1");
            if (dt.Rows.Count == 0)
                return "100000000001";
            else
            {
                string test = dt.Rows[0]["barcode"].ToString();
                long barcoderaw = Convert.ToInt64(test);
                return (barcoderaw + 1).ToString();
            }
        }

        public static string GetNextORNumber()
        {
            DataTable dt = DataBaseHelper.GetDB("SELECT invoicenumber FROM TransactionHead ORDER BY invoicenumber DESC LIMIT 1");
            if (dt.Rows.Count == 0)
                return "1";
            else
            {
                string test = dt.Rows[0]["invoicenumber"].ToString();
                long ornumberraw = Convert.ToInt64(test);
                return (ornumberraw + 1).ToString();
            }
        }
        public static string GetNextTransNumber()
        {
            DataTable dt = DataBaseHelper.GetDB("SELECT transactionnumber FROM TransactionHead ORDER BY transactionnumber DESC LIMIT 1");
            if (dt.Rows.Count == 0)
                return "1";
            else
            {
                string test = dt.Rows[0]["transactionnumber"].ToString();
                long transnumbraw = Convert.ToInt64(test);
                return (transnumbraw + 1).ToString();
            }
        }
    }
}
