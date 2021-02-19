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
    }
}
