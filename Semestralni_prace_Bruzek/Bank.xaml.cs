using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Windows.Navigation;
using Semestralka_Bruzek;

namespace Semestralka
{
    public partial class Bank : Page
    {
        private int? bankId;

        public Bank()
        {
            InitializeComponent();
        }

        public Bank(BankInfo bankInfo) : this()
        {
            bankId = bankInfo.ID;
            txtBankName.Text = bankInfo.BankName;
            txtAccountNumber.Text = bankInfo.AccountNumber;
            txtBankCode.Text = bankInfo.BankCode;
            txtIBAN.Text = bankInfo.IBAN;
            txtSWIFT.Text = bankInfo.SWIFT;
        }

        private void SaveBankInfo_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string query;

            if (bankId.HasValue)
            {
                query = @"UPDATE BankInfo SET BankName = @BankName, AccountNumber = @AccountNumber, BankCode = @BankCode, 
                          IBAN = @IBAN, SWIFT = @SWIFT WHERE ID = @ID";
            }
            else
            {
                query = @"INSERT INTO BankInfo (BankName, AccountNumber, BankCode, IBAN, SWIFT) 
                          VALUES (@BankName, @AccountNumber, @BankCode, @IBAN, @SWIFT)";
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BankName", txtBankName.Text);
                    command.Parameters.AddWithValue("@AccountNumber", txtAccountNumber.Text);
                    command.Parameters.AddWithValue("@BankCode", txtBankCode.Text);
                    command.Parameters.AddWithValue("@IBAN", txtIBAN.Text);
                    command.Parameters.AddWithValue("@SWIFT", txtSWIFT.Text);

                    if (bankId.HasValue)
                    {
                        command.Parameters.AddWithValue("@ID", bankId.Value);
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Údaje o bance byly uloženy do databáze.", "Informace", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService?.Navigate(new BankListPage());
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Chyba při ukládání údajů o bance: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BankListPage());
        }
    }
}
