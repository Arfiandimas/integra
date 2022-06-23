using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Net.Sockets;
using System.Net;

class Program
{
    static int id;
    static string data;
    static bool is_print;
    static bool is_scan;
    static string created_at;
    static string updated_at;

    // static bool check_reader;

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
        bool check_reader = false;
        con.Open();
        using (MySqlCommand command = new MySqlCommand("SELECT * FROM print_scan WHERE is_print = 0 ORDER BY created_at ASC LIMIT 1", con))
        {
            MySqlDataReader reader = command.ExecuteReader();
            check_reader = reader.Read();
            if (check_reader){
                id = reader.GetInt32(0);
                data = reader.GetString(1);
                is_print = reader.GetBoolean(2);
                is_scan = reader.GetBoolean(3);
                created_at = reader.GetString(4);
                updated_at = reader.GetString(5);
                con.Close();
                UdpSend(id, is_print, data);
            } else {
                con.Close();
            }
        }
        Console.WriteLine("Selesai");
        Environment.Exit(0);
    }

    static void UdpSend(int id_data, bool is_print_result, string message = "")
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
                if(result != "success"){
                    Console.WriteLine("gagal");
                } else {
                    Console.WriteLine(result);
                    UpdateData(id_data, 1);
                    SendData();
                }
            }
        } catch (SocketException e) {
            Console.WriteLine(e);
        } finally {
            listener.Close();
        }
    }

    static void UpdateData(int id, int is_print)
    {
        con.Open();
        using (MySqlCommand cmd = new MySqlCommand("UPDATE print_scan SET is_print = @is_print WHERE id = @id", con))
        {
            cmd.Parameters.Add(new MySqlParameter("id", id));
            cmd.Parameters.Add(new MySqlParameter("is_print", is_print));
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
