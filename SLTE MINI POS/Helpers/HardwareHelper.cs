using SLTE_MINI_POS.Enums;
using SLTE_MINI_POS.Model;
using SLTE_MINI_POS.Model.Global;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SLTE_MINI_POS.Helpers
{
    class HardwareHelper
    {
        public static Font font_Title = new Font("Consolas", 14.25F, FontStyle.Bold);
        public static Font font_Price = new Font("Consolas", 9.25F, FontStyle.Bold);
        public static Font font_Content = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        public static Font font_Change = new Font("Consolas", 12, FontStyle.Bold);
        public static SolidBrush brush_Black = new SolidBrush(Color.Black);
        public static int getpbheight;
        public static void PrintPageReceipt(Transaction tran, bool isVoid, Bitmap bmp)
        {
            PrintReceipt(null, null, bmp, tran, isVoid, true, false); // tran.getsenior().get_fullname() != "");
        }
        public static void Print(Transaction tran, bool isReprint, bool isVoid, PrintingType ej = PrintingType.NormalPrinting)
        {
            
            if (ej == PrintingType.NormalPrinting)
                RawPrinterHelper.OpenCashDrawer(true);
            
            PrintReceipt(null, null, null, tran, isVoid, isReprint, false, ej);
        }
        public static Graphics PrintReceipt(object sender, PrintPageEventArgs e, Bitmap bmp, Transaction tran, bool isvoid, bool isreprint, bool customercopy, PrintingType ej = PrintingType.NormalPrinting)
        {
            // This actually doesnt use Graphics
            DataTable tempDataTable;
            int value = 64;
            if (globalvariables.PrintReceiptFormat == PrintFormat.Custom_76mm_journal)
                value = 40;
            else if (globalvariables.PrintReceiptFormat == PrintFormat.Custom_JNF_57mm)
                value = 42;
            ReceiptPrinterHelper printer = new ReceiptPrinterHelper(value);
            if (globalvariables.PrintReceiptBuffer != 0)
                printer.StringBufferWidth = globalvariables.PrintReceiptBuffer;
            if (globalvariables.PrintReceiptActual != 0)
                printer.StringFullWidth = globalvariables.PrintReceiptActual;
            if (globalvariables.PrintReceiptLimit != 0)
                printer.StringWidth = globalvariables.PrintReceiptLimit;
            if (globalvariables.PrintReceiptLinespacing != 0)
                printer.LineSpacing = globalvariables.PrintReceiptLinespacing;

            // Header
            printer.LargeFont();
            printer.CPI12();
            if (globalvariables.BusinessName != "")
                printer.WriteLines(globalvariables.BusinessName);
            printer.NormalFont();
            printer.CPI12();
            if (globalvariables.Owner != "")
                printer.WriteLines(globalvariables.Owner);
            printer.WriteLines(((globalvariables.IsVatable) ?
                "VAT REG. " : "NON VAT REG. ") + globalvariables.TIN);
            if (globalvariables.MIN != "")
                printer.WriteLines(globalvariables.MIN);
            if (globalvariables.Serial != "")
                printer.WriteLines(globalvariables.Serial);
            if (globalvariables.Address != "")
                printer.WriteLines(globalvariables.Address);
            if (globalvariables.TelNo != "")
                printer.WriteLines("Tel.#: " + globalvariables.TelNo);
            if (customercopy)
                printer.WriteLines("Customer's Copy");

            // Transaction Info
            tempDataTable = new DataTable();
            tempDataTable.Columns.Add();
            tempDataTable.Columns.Add();
            tempDataTable.Rows.Add("TERMINAL#: " + globalvariables.Terminal, "REF#:" + tran.ID.ToString());
            tempDataTable.Rows.Add("SI#: " + tran.InvoiceNumber, "TRAN#: " + tran.TransactionNo);
            tempDataTable.Rows.Add("DATE: " + tran.Date.ToShortDateString(), "TIME: " + tran.Date.ToLongTimeString());

            printer.WriteRepeatingCharacterLine('=');
            printer.WriteTable(
                tempDataTable,
                new StringAlignment[] { StringAlignment.Near, StringAlignment.Near },
                new int[] { printer.StringWidth / 2, printer.StringWidth / 2 }
            );
            //if (tran.getmemo() != "")
            //    printer.WriteRow(
            //      new string[] { "MEMO: ", tran.getmemo() },
            //      new StringAlignment[] { StringAlignment.Near, StringAlignment.Near },
            //      new int[] { 6, printer.StringWidth }
            //    );

            // Product Info
            tempDataTable = new DataTable();
            tempDataTable.Columns.Add();
            tempDataTable.Columns.Add();
            tempDataTable.Columns.Add();
            tempDataTable.Columns.Add();
            tempDataTable.Rows.Add("QTY", "", "DESCRIPTION", "AMOUNT");
            foreach (Product prod in tran.productlist)
            {
                string proddesc = prod.Name;
                tempDataTable.Rows.Add(prod.Qty.ToString("0.####"), "", proddesc, prod.Price.ToString("N2"));
                //if ((prod.getPrice() != prod.getOrigPrice()) && (prod.getOrigPrice() != 0)
                //    && cls_globalvariables.DiscountDetails == true)
                //    tempDataTable.Rows.Add("", "", "(P" + prod.getOrigPrice().ToString("N2") + " - " + ((1 - (prod.getPrice() / prod.getOrigPrice())) * 100).ToString("N2") + "%)", "");

                //if (cls_globalvariables.PrintProdMemo == ProductMemoType.ProductMemoNoSI || cls_globalvariables.PrintProdMemo == ProductMemoType.ProductMemoWithSI)
                //{
                //    string productmemo = prod.getProductMemo();
                //    string memo = prod.getMemo();
                //    if (isreprint)
                //        prod.get_productmemo_by_wid(tran.getWid(), prod.getWid());
                //    if (!string.IsNullOrEmpty(productmemo) && !string.IsNullOrWhiteSpace(productmemo) && productmemo.Length != 0)
                //        tempDataTable.Rows.Add("", "", prod.getProductMemo(), "");
                //    if (!string.IsNullOrEmpty(memo) && !string.IsNullOrWhiteSpace(memo) && memo.Length != 0)
                //        tempDataTable.Rows.Add("", "", cls_globalvariables.PrintProdMemo == ProductMemoType.ProductMemoWithSI ? memo : prod.getMemoNoOR(), "");
                //}

                //if (prod.getQty() < 0)
                //{
                //    string refundRef = prod.getRefundReference();
                //    tempDataTable.Rows.Add("", "", cls_globalvariables.PrintProdMemo == ProductMemoType.ReferenceNo ? "ITEM REFUND! REF#" + refundRef : "ITEM REFUND!", "");
                //    LOGS.LOG_PRINT(mt.logprint_itemrefunded);
                //}
            }
            printer.WriteRepeatingCharacterLine('=');
            printer.WriteTable(
                tempDataTable,
                new StringAlignment[] { StringAlignment.Far, StringAlignment.Far, StringAlignment.Near, StringAlignment.Far },
                new int[] { (int)(printer.StringWidth * 0.15), 1, (int)(printer.StringWidth * 0.6), (int)(printer.StringWidth * 0.25) }
            );

            printer.WriteRepeatingCharacterLine('=');
            // Discount Breakdown
            decimal total_sale = tran.GetTotalAmountDue();
            decimal total_gross = tran.GetTotalAmountDue();

            // Sub-Total Info
            tempDataTable = new DataTable();
            tempDataTable.Columns.Add();
            tempDataTable.Columns.Add();
            decimal totalQuantity = tran.GetTotalQty();
            //totalQuantity -= tran.get_gcexcessamt() > 0 ? 1 : 0;
            tempDataTable.Rows.Add("Total QTY:", totalQuantity.ToString("0.####"));
            printer.WriteTable(
                tempDataTable,
                new StringAlignment[] { StringAlignment.Near, StringAlignment.Far },
                new int[] { printer.StringWidth / 2, printer.StringWidth / 2 }
            );
            printer.LargeFont();
            printer.CPI12();
            printer.WriteRow(
               new string[] { "AMOUNT DUE:", total_sale.ToString("N2") },
               new StringAlignment[] { StringAlignment.Near, StringAlignment.Far },
               new int[] { printer.StringWidth / 2, printer.StringWidth / 2 }
           );
            printer.NormalFont();
            printer.CPI12();

            // Payment
            decimal tendered_cash = tran.TenderAmount;
            decimal tendered_change = tran.GetChange();
            tempDataTable = new DataTable();
            tempDataTable.Columns.Add(); tempDataTable.Columns.Add();
            int cnt_item_t = 0;

            //totalAmountDue -= gcexcessamt > 0 ? gcexcessamt : 0; 
            printer.NormalFont();
            printer.CPI12();

            if (tendered_cash != 0)
                tempDataTable.Rows.Add("Cash:", tendered_cash.ToString("N2")); cnt_item_t++;
            

            tempDataTable.Rows.Add("Change:", tendered_change.ToString("N2"));


            printer.WriteTable(
                tempDataTable,
                new StringAlignment[] { StringAlignment.Near, StringAlignment.Far },
                new int[] { printer.StringWidth / 2, printer.StringWidth / 2 }
            );

            // Sub-Total Info
            tempDataTable = new DataTable();
            tempDataTable.Columns.Add();
            tempDataTable.Columns.Add();
            //gcexcess value is 0 if transaction is zero rated
            //decimal tempExcess = tran.get_productlist().get_zeroratedsale() > 0 ? 0 : gcexcessamt;
            if (globalvariables.IsVatable)
            {
                decimal vat = (total_sale / 1.12M) * 0.12M;
                tempDataTable.Rows.Add("VATABLE SALE:", total_sale.ToString("N2"));
                //if (vatable_return != 0)
                //    tempDataTable.Rows.Add("VATABLE RETURN:", vatable_return.ToString("N2"));
                //if (vatable_sale != 0 && vatable_return != 0)
                //    tempDataTable.Rows.Add("VATABLE SUBTOTAL:", vatableSubtotal.ToString("N2"));
                printer.WriteRepeatingCharacterLine('=');
                // VAT
                tempDataTable.Rows.Add("VAT AMOUNT:", vat.ToString("N2"));
            }
            //gcexcess value is 0 if transaction is vatable for vat exempt breakdown
            printer.WriteTable(
                tempDataTable,
                new StringAlignment[] { StringAlignment.Near, StringAlignment.Far },
                new int[] { printer.StringWidth / 2, printer.StringWidth / 2 }
            );
            // POS Provider Info
            // Based on BIR Revenue Regulations No. 10-2015
            printer.WriteRepeatingCharacterLine('=');
            if (globalvariables.PosProviderName != "")
                printer.WriteLines(globalvariables.PosProviderName);
            if (globalvariables.PosProviderAddress != "")
                printer.WriteLines(globalvariables.PosProviderAddress);
            if (globalvariables.PosProviderTIN != "")
                printer.WriteLines(globalvariables.PosProviderTIN);
            if (globalvariables.Acc != "")
                printer.WriteLines(globalvariables.Acc);
            if (globalvariables.AccDate != "")
            {
                string validUntil = GetValidUntilDate(true);
                printer.WriteLines(globalvariables.AccDate);
                if (validUntil != "")
                    printer.WriteLines("Valid Until: " + validUntil);
            }
            if (globalvariables.PermitNo != "")
                printer.WriteLines(globalvariables.PermitNo);
            if (globalvariables.ApprovalDate != "")
            {
                string validUntil = GetValidUntilDate(false);
                printer.WriteLines("Date Issued: " + globalvariables.ApprovalDate);
                if (validUntil != "")
                    printer.WriteLines("Valid Until: " + validUntil);
            }
            printer.WriteLines("THIS INVOICE/RECEIPT SHALL BE VALID FOR FIVE(5) YEARS FROM THE DATE OF THE PERMIT TO USE.");

            // Footers
            printer.WriteRepeatingCharacterLine('=');
            if (globalvariables.OrFooter1 != "")
                printer.WriteLines(globalvariables.OrFooter1);
            if (globalvariables.OrFooter2 != "")
                printer.WriteLines(globalvariables.OrFooter2);
            if (globalvariables.OrFooter3 != "")
                printer.WriteLines(globalvariables.OrFooter3);
            if (globalvariables.OrFooter4 != "")
                printer.WriteLines(globalvariables.OrFooter4);

            // Based on BIR Revenue Regulations No. 10-2015
            printer.LargeFont();
            printer.CPI12();
            if (!globalvariables.IsVatable)
                printer.WriteLines("THIS DOCUMENT IS NOT VALID FOR CLAIM OF INPUT TAX.");
            printer.NormalFont();
            printer.CPI12();

            // Reprint/Voided Tag
            if (isvoid)
                printer.WriteLines("VOIDED RECEIPT!");
            if (isreprint)
            {
                printer.WriteLines("REPRINTED RECEIPT!");
            }
            else
            {
                DateTime now = DateTime.Now;
                printer.WriteLines("PRINTED: " + now.ToShortDateString() + " " + now.ToLongTimeString());
            }

            if (bmp == null)
            {
                printer.Print();
                if (ej == PrintingType.EJournalHardCopy || ej == PrintingType.NormalPrinting)
                    printer.ActivateCutter();
            }
            else
                printer.PreviewOR(e, bmp);

            return null;
        }

        public static string GetValidUntilDate(bool isAccDate)
        {
            //1 = acc_date; 0 = approvaldate
            DateTime validUntilDate;
            try
            {
                if (isAccDate)
                    validUntilDate = GetValidDate(globalvariables.AccDate);
                else
                    validUntilDate = GetValidDate(globalvariables.ApprovalDate);

                if (validUntilDate <= globalvariables.birUntilDate)
                    validUntilDate = globalvariables.birUntilDate.AddYears(5).AddDays(-1);
                else
                    validUntilDate = validUntilDate.AddYears(5).AddDays(-1);

                return validUntilDate.ToString("MMMM dd, yyyy");
            }
            catch { return ""; }
        }
        public static DateTime GetValidDate(string value)
        {
            DateTime validDate;
            Regex regex = new Regex(@"(\d+)[-.\/](\d+)[-.\/](\d+)");
            Match match = regex.Match(value);
            if (match.Success)
            {
                string dateString = match.Value;
                validDate = DateTime.Parse(dateString, System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                validDate = DateTime.Now.AddMonths(1);
            }
            return validDate;
        }
    }
}
