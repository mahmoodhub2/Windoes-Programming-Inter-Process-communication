/*
  FILE : MainWindow.cs
* PROJECT :  Assignment 5#
* PROGRAMMER : Mahmood Al-Zubaidi
* FIRST VERSION : November 11/2021
* DESCRIPTION : The purpose of this class is to prompt the user for an input and send it to
* the serverSideApp.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

namespace guessing_game
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
        private string userID = "";
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Won.Text = null;
            yes.Visibility = Visibility.Hidden;
            no.Visibility = Visibility.Hidden;

            string name = firstName.Text;
            string server = IPAddress.Text;
            string userPort = Port.Text;
            ConnectClient(server, userPort, name+"name");
           
        }


        // ------------------------------------------------------------
        // Function: ConnectClient
        // Description: it sends the messages of the user's input to the sever that's provided to it.
        // Parameters: String server: the server, String port1: the port, string message: the message to send.
        // Returns: Nothing.
        // ------------------------------------------------------------
        private void ConnectClient(String server, String port1, string message)
        {
            try
            {
                Int32 port = Int32.Parse(port1);
                TcpClient client = new TcpClient(server, port);
                
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                data = new Byte[256];

                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                if (responseData.Contains("choose"))
                {
                    string range = "";
                    string id = "";
                    for(int i = 0; i <= 37; i++)
                    {
                        range += responseData[i];
                    }
                    Range.Text = range;
                    for (int i = 0; i < responseData.Length; i++)
                    {
                        if(responseData[i] == ':')
                        {
                            for (int x = i+1; x < responseData.Length; x++){
                                id += responseData[x];
                            }
                            
                        }
                    }
                    userID = id;

                }
                else if (responseData.Contains("put"))
                {
                    Range.Text = responseData;
                }
                else if (responseData.Contains("Won"))
                {
                    Won.Text = responseData;
                    yes.Visibility = Visibility.Visible;
                    no.Visibility = Visibility.Visible;
                }
                else if (responseData.Contains("sure"))
                {
                    sure.Text = responseData;
                    yes2.Visibility = Visibility.Visible;
                }
                else if (responseData.Contains("confirmed"))
                {
                    stream.Close();
                    client.Close();
                    System.Windows.Application.Current.Shutdown();

                }

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }


        // ------------------------------------------------------------
        // Function: Guess_Click
        // Description: it sends the user's guess to the sever by calling the ConnectClient and passing the guess to it.
        // Parameters: object sender, RoutedEventArgs e.
        // Returns: Nothing
        // ------------------------------------------------------------
        private void Guess_Click(object sender, RoutedEventArgs e)
        {
            string userGuess = Guess.Text;
            string server = IPAddress.Text;
            string userPort = Port.Text;
            Int32 userGuessInt=0;
            long number1 = 0;
            bool valid = false;
            bool canConvert = long.TryParse(userGuess, out number1);

            if (canConvert == false)
            {
                OutOfRange.Text = "Invalid, please put a number";
            }
            else if(canConvert == true)
            {
                userGuessInt = Int32.Parse(userGuess);
                if (userGuessInt <= 0)
                {
                    OutOfRange.Text = "Invalid, please put a postive integer";
                }
                else
                {
                    valid = true;
                }
            }
            if(valid == true)
            {
                ConnectClient(server, userPort, userGuess + " "+ userID);
            }

        }


        // ------------------------------------------------------------
        // Function: No_click
        // Description: it gets exicuted when the user press no after promting them to make sure that they want to exit.
        // Parameters: object sender, RoutedEventArgs e.
        // Returns: Nothing
        // ------------------------------------------------------------
        private void No_click(object sender, RoutedEventArgs e)
        {
            string message = "disconnect";
            string server = IPAddress.Text;
            string userPort = Port.Text;
            ConnectClient(server, userPort, message);
        }


        // ------------------------------------------------------------
        // Function: ExitApp
        // Description: gets exicuted when the user confirms that they want to exit in the middle of the game.
        // Parameters: object sender, RoutedEventArgs e.
        // Returns: Nothing.
        // ------------------------------------------------------------
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            string message = "confrim";
            string server = IPAddress.Text;
            string userPort = Port.Text;
            ConnectClient(server, userPort, message);
        }
    }
}
