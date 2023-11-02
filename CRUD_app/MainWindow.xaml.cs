using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRUD_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            switch (radioButton.Name)
            {
                case "RadioButtonAdd":
                    TextBoxID.IsEnabled = false; 
                    TextBoxName.IsEnabled = true;
                    TextBoxEmail.IsEnabled = true;
                    TextBoxPhone.IsEnabled = true;
                    break;
                case "RadioButtonUpdate":
                    TextBoxID.IsEnabled = true;
                    TextBoxName.IsEnabled = true;
                    TextBoxEmail.IsEnabled = true;
                    TextBoxPhone.IsEnabled = true;
                    break;
                case "RadioButtonDelete":
                    TextBoxID.IsEnabled = true;
                    TextBoxName.IsEnabled = false;
                    TextBoxEmail.IsEnabled = false;
                    TextBoxPhone.IsEnabled = false;
                    break;
                case "RadioButtonSearchAll":
                    TextBoxID.IsEnabled = false;
                    TextBoxName.IsEnabled = false;
                    TextBoxEmail.IsEnabled = false;
                    TextBoxPhone.IsEnabled = false;
                    break;
                case "RadioButtonSearchById":
                    TextBoxID.IsEnabled = true;
                    TextBoxName.IsEnabled = false;
                    TextBoxEmail.IsEnabled = false;
                    TextBoxPhone.IsEnabled = false;
                    break;
                default:
                    TextBoxID.IsEnabled = false;
                    TextBoxName.IsEnabled = false;
                    TextBoxEmail.IsEnabled = false;
                    TextBoxPhone.IsEnabled = false;
                    break;
            }
        }

        private void ButtonSave(object sender, RoutedEventArgs e)
        {
            string phone = TextBoxPhone.Text;
            string email = TextBoxEmail.Text;

            if (!ValidatePhoneNumber(phone))
            {
                MessageBox.Show("Invalid phone number format. Please use the format +xx xxx xxx xxx.");
                return;
            }

            if (!ValidateEmailAddress(email))
            {
                MessageBox.Show("Invalid email address format. Please use the format name@example.com.");
                return;
            }

            SqlConnection con = new SqlConnection("Data Source=LAPTOP-IDD43QN2;Initial Catalog=CRUD_app;Integrated Security=True");

            try
            {
                con.Open();

                if (string.IsNullOrWhiteSpace(TextBoxName.Text) || string.IsNullOrWhiteSpace(TextBoxPhone.Text) || string.IsNullOrWhiteSpace(TextBoxEmail.Text))
                {
                    MessageBox.Show("Please fill in all required fields.");
                    return;
                }

                using (SqlCommand command = new SqlCommand("insert into [user] values(@name,@phone,@email); SELECT SCOPE_IDENTITY();", con))
                {
                    command.Parameters.AddWithValue("@name", TextBoxName.Text);
                    command.Parameters.AddWithValue("@phone", TextBoxPhone.Text);
                    command.Parameters.AddWithValue("@email", TextBoxEmail.Text);
                    int lastInsertedID = Convert.ToInt32(command.ExecuteScalar());

                    MessageBox.Show("The record with ID " + lastInsertedID + " has been successfully saved.");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while saving the record: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                con.Close(); 
            }
        }


        private void ButtonUpdate(object sender, RoutedEventArgs e)
        {
            string phone = TextBoxPhone.Text;
            string email = TextBoxEmail.Text;

            if (!ValidatePhoneNumber(phone))
            {
                MessageBox.Show("Invalid phone number format. Please use the format +xx xxx xxx xxx.");
                return;
            }

            if (!ValidateEmailAddress(email))
            {
                MessageBox.Show("Invalid email address format. Please use the format name@example.com.");
                return;
            }

            SqlConnection con = new SqlConnection("Data Source=LAPTOP-IDD43QN2;Initial Catalog=CRUD_app;Integrated Security=True");

            try
            {
                con.Open();

                if (string.IsNullOrWhiteSpace(TextBoxID.Text))
                {
                    MessageBox.Show("Please enter an ID to update the record.");
                    return;
                }

                using (SqlCommand command = new SqlCommand("update [user] set name=@name, phone=@phone, email=@email where id=@id", con))
                {
                    command.Parameters.AddWithValue("@id", TextBoxID.Text);
                    command.Parameters.AddWithValue("@name", TextBoxName.Text);
                    command.Parameters.AddWithValue("@phone", TextBoxPhone.Text);
                    command.Parameters.AddWithValue("@email", TextBoxEmail.Text);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("The record has been successfully updated.");
                    }
                    else
                    {
                        MessageBox.Show("No record was updated. The provided ID does not exist.");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while updating the record: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); 
                }
            }
        }

        private void ButtonDelete(object sender, RoutedEventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection("Data Source=LAPTOP-IDD43QN2;Initial Catalog=CRUD_app;Integrated Security=True");
                con.Open();

                using (SqlCommand command = new SqlCommand("delete [user] where id=@id", con))
                {
                    command.Parameters.AddWithValue("@id", TextBoxID.Text);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("The record has been successfully deleted.");
                    }
                    else
                    {
                        MessageBox.Show("No record was deleted. The provided ID does not exist.");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while deleting the record: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                if (con != null)
                {
                    con.Close(); 
                }
            }
        }

        private void ButtonSelectAll(object sender, RoutedEventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection("Data Source=LAPTOP-IDD43QN2;Initial Catalog=CRUD_app;Integrated Security=True");
                con.Open();

                using (SqlCommand command = new SqlCommand("select * from [user]", con))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    DataGrid1.ItemsSource = dataTable.DefaultView;
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while selecting the record: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        private void ButtonSelectById(object sender, RoutedEventArgs e)
        {
            SqlConnection con = null; 

            try
            {
                con = new SqlConnection("Data Source=LAPTOP-IDD43QN2;Initial Catalog=CRUD_app;Integrated Security=True");
                con.Open();

                using (SqlCommand command = new SqlCommand("select * from [user] where id=@id", con))
                {
                    command.Parameters.AddWithValue("@id", TextBoxID.Text);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    DataGrid1.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while retrieving records: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
            finally
            {
                if (con != null)
                {
                    con.Close(); 
                }
            }

        }
        private bool ValidatePhoneNumber(string phoneNumber)
        {
            phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());
            return Regex.IsMatch(phoneNumber, @"^\d{2}(\d{3}){3}$");
        }

        private bool ValidateEmailAddress(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
    }
}
