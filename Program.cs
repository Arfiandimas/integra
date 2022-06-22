using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Net.Sockets;
using System.Net;

class Program
{
    static MySqlConnection con = new MySqlConnection("Database=integra;Server=localhost;user=barjono;password=password");

    // Connection UDP Send
    static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
    static IPAddress serverAddr = IPAddress.Parse("192.168.1.23");
    static IPEndPoint endPoint = new IPEndPoint(serverAddr, 8888);

    // Connection UDP Receive
    static UdpClient listener = new UdpClient(9999);
    static IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("192.168.1.23"), 9999);

    static void Main(string[] args)
    {
        SendData();
    }

    static void SendData()
    {
        con.Open();

        using (MySqlCommand command = new MySqlCommand("SELECT * FROM print_scan WHERE is_print = 0 ORDER BY created_at ASC LIMIT 1", con))
        {
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string data = reader.GetString(1);
                bool is_print = reader.GetBoolean(2);
                bool is_scan = reader.GetBoolean(3);
                string created_at = reader.GetString(4);
                string updated_at = reader.GetString(5);

                Console.WriteLine(data);
            }
        }
        Console.WriteLine(printScans);
        // con.Close();
        // UdpSend(printScans[0].ToString());
    }

    static void UdpSend(string message = "")
    {
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
        IPAddress serverAddr = IPAddress.Parse("192.168.1.23");
        IPEndPoint endPoint = new IPEndPoint(serverAddr, 8888);
        byte[] send_buffer = Encoding.ASCII.GetBytes(message);
        sock.SendTo(send_buffer, endPoint);

        try
        {
            while (true)
            {
                Console.WriteLine("Waiting for response");
                byte[] bytes = listener.Receive(ref groupEP);
                string result = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                if(result != message){
                    Console.WriteLine("gagal");
                } else {
                    Console.WriteLine(result);
                    SendData();
                }
            }
        } catch (SocketException e) {
            Console.WriteLine(e);
        } finally {
            listener.Close();
        }
    }
}
