using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace Semestralka_Bruzek
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeBankDatabase();
            CreateInvoicesTable();
            CreateInvoiceItemsTable();
            //DeleteInvoiceInfoTable();
            //DeleteBankInfoTable();
            //DeleteAllInvoiceItems();
        }
        private void CreateInvoiceItemsTable()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS InvoiceItems (
        InvoiceItemID INTEGER PRIMARY KEY AUTOINCREMENT,
        InvoiceID INTEGER,
        ItemName TEXT,
        Quantity INTEGER,
        Price REAL,
        Tax REAL,
        Total REAL,
        FOREIGN KEY (InvoiceID) REFERENCES Invoices(InvoiceID)
    );";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error creating InvoiceItems table: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteAllInvoiceItems()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string deleteAllItemsQuery = "DELETE FROM InvoiceItems";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteAllItemsQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("All invoice items deleted successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error deleting all invoice items: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateInvoicesTable()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS Invoices (
                                    InvoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    BankID INTEGER,
                                    CustomerICO TEXT,
                                    CustomerDIC TEXT,
                                    CustomerAddress TEXT,
                                    CustomerCountry TEXT,
                                    CustomerEmail TEXT,
                                    CustomerPhone TEXT,
                                    TotalPrice REAL,
                                    CompanyName TEXT,
                                    FOREIGN KEY (BankID) REFERENCES BankInfo(ID)
                                );";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error creating Invoices table: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteBankInfoTable()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string dropTableQuery = "DROP TABLE IF EXISTS BankInfo";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(dropTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("BankInfo table deleted successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error deleting BankInfo table: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteInvoiceInfoTable()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string dropTableQuery = "DROP TABLE IF EXISTS Invoices";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(dropTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Invoice table deleted successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error deleting Invoice table: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeBankDatabase()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS BankInfo (
        ID INTEGER PRIMARY KEY,
        BankName TEXT,
        AccountNumber TEXT,
        BankCode TEXT,
        IBAN TEXT,
        SWIFT TEXT
    )";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při inicializaci databáze pro bankovní informace: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


