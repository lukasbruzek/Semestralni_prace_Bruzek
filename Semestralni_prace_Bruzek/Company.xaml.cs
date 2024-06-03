using System;
using System.Windows;
using System.Data.SQLite;
using System.Windows.Controls;
using Semestralka_Bruzek;

namespace Semestralka
{
    public partial class Company : Page
    {
        public Company()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadCompanyInfo();
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS CompanyInfo (
                CompanyName TEXT,
                ICO TEXT,
                DIC TEXT,
                Country TEXT,
                Email TEXT,
                IsVATPayer INTEGER
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
                MessageBox.Show("Chyba při inicializaci databáze: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCompanyInfo()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectQuery = "SELECT * FROM CompanyInfo";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCompanyName.Text = reader["CompanyName"].ToString();
                                txtICO.Text = reader["ICO"].ToString();
                                txtDIC.Text = reader["DIC"].ToString();
                                txtCountry.Text = reader["Country"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                chkIsVATPayer.IsChecked = Convert.ToBoolean(reader["IsVATPayer"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání informací o firmě: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveCompanyInfo_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";

            string checkQuery = "SELECT COUNT(*) FROM CompanyInfo";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(checkQuery, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 0)
                    {
                        UpdateCompanyInfo(connection);
                    }
                    else
                    {
                        InsertCompanyInfo(connection);
                    }
                }
            }
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            Window.GetWindow(this).Close();
        }

        private void UpdateCompanyInfo(SQLiteConnection connection)
        {
            string updateQuery = @"UPDATE CompanyInfo 
                                   SET CompanyName = @CompanyName, 
                                       ICO = @ICO, 
                                       DIC = @DIC, 
                                       Country = @Country, 
                                       Email = @Email, 
                                       IsVATPayer = @IsVATPayer";
            using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@CompanyName", txtCompanyName.Text);
                command.Parameters.AddWithValue("@ICO", txtICO.Text);
                command.Parameters.AddWithValue("@DIC", txtDIC.Text);
                command.Parameters.AddWithValue("@Country", txtCountry.Text);
                command.Parameters.AddWithValue("@Email", txtEmail.Text);
                command.Parameters.AddWithValue("@IsVATPayer", chkIsVATPayer.IsChecked);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Údaje o firmě byly aktualizovány.", "Informace", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Chyba při aktualizaci údajů o firmě: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void InsertCompanyInfo(SQLiteConnection connection)
        {
            string insertQuery = @"INSERT INTO CompanyInfo (CompanyName, ICO, DIC, Country, Email, IsVATPayer) 
                                   VALUES (@CompanyName, @ICO, @DIC, @Country, @Email, @IsVATPayer)";
            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@CompanyName", txtCompanyName.Text);
                command.Parameters.AddWithValue("@ICO", txtICO.Text);
                command.Parameters.AddWithValue("@DIC", txtDIC.Text);
                command.Parameters.AddWithValue("@Country", txtCountry.Text);
                command.Parameters.AddWithValue("@Email", txtEmail.Text);
                command.Parameters.AddWithValue("@IsVATPayer", chkIsVATPayer.IsChecked);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Údaje o firmě byly uloženy do databáze.", "Informace", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Chyba při ukládání údajů o firmě: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
