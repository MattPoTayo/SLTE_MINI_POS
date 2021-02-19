using Dapper;
using System;
using System.Data.SQLite;
using System.IO;
using System.Transactions;
using System.Data;
using System.Windows.Forms;

namespace SLTE_MINI_POS.Helpers
{
    public class DataBaseHelper
    {
        private static string _dbFile = Helper.DbFile;
        public static void CreateDatabase()
        {
            var dbFile = _dbFile;
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var conn = DbConnection())
                    {
                        conn.Open();
                        //create Queue table
                        conn.Execute("CREATE TABLE IF NOT EXISTS Product (" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            "barcode VARCHAR(100) NOT NULL, " +
                            "productname VARCHAR(100) NOT NULL, " +
                            "stockno VARCHAR(100) NOT NULL DEFAULT '', " +
                            "price DECIMAL(19,6) NOT NULL DEFAULT 0, " +
                            "discontinued INT(32) NOT NULL DEFAULT 1, " +
                            "inventory INTEGER DEFAULT 0)");
                        conn.Execute("CREATE TABLE IF NOT EXISTS TransactionHead (" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            "transactionnumber VARCHAR(100) NOT NULL, " +
                            "transdate DATETIME NOT NULL)");
                        conn.Execute("CREATE TABLE IF NOT EXISTS TransactionDetail (" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            "headid INTEGER NOT NULL, " +
                            "productid INTEGER NOT NULL, " +
                            "quantity INTEGER DEFAULT 0, " +
                            "price DECIMAL(19,6) NOT NULL)");
                        conn.Execute("CREATE TABLE IF NOT EXISTS InventoryAdjust (" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            "productid INTEGER NOT NULL, " +
                            "quantity INTEGER DEFAULT 0, " +
                            "transdate DATETIME NOT NULL)");
                    }

                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                if (File.Exists(_dbFile))
                    File.Delete(_dbFile);

                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static bool SetDB(string query)
        {
            var id = null as int?;
            if (query == null)
                return false;
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var conn = DbConnection())
                    {
                        conn.Open();
                        id = conn.Execute(query);

                        if (id == null)
                            throw new Exception("Unable to execute query.");
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ReInitializeDataBase();
            }

            return true;
        }

        public static DataTable GetDB(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DbConnection())
                {
                    conn.Open();
                    SQLiteDataAdapter sQLiteDataAdapter = new SQLiteDataAdapter(query, conn);
                    sQLiteDataAdapter.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                ReInitializeDataBase();
            }


            return dt;
        }

        public static SQLiteConnection DbConnection()
        {
            return new SQLiteConnection($"Data Source={_dbFile}; foreign keys=true;datetimeformat=CurrentCulture");
        }

        public static void ReInitializeDataBase()
        {
            CreateDatabase();
        }

        private static bool DatabaseIsValid()
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={_dbFile};FailIfMissing=True;"))
            {
                try
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
