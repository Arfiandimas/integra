using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Net.Sockets;
using System.Net;

class Program
{
    static MySqlConnection con = new MySqlConnection("Database=integra;Server=localhost;user=barjono;password=password");

    static void Main(string[] args)
    {
        SendData();
    }

    static void InsertData(string data, bool is_print, bool is_scan, string created_at, string updated_at)
    {
        con.Open();
        try
        {
            using (MySqlCommand command = new MySqlCommand("INSERT INTO print_scan VALUES(@id, @data, @is_print, @is_scan, @created_at, @updated_at)", con))
            {
                command.Parameters.Add(new MySqlParameter("id", command.LastInsertedId));
                command.Parameters.Add(new MySqlParameter("data", data));
                command.Parameters.Add(new MySqlParameter("is_print", is_print == true ? 1 : 0));
                command.Parameters.Add(new MySqlParameter("is_scan", is_scan == true ? 1 : 0));
                command.Parameters.Add(new MySqlParameter("created_at", created_at));
                command.Parameters.Add(new MySqlParameter("updated_at", updated_at));
                command.ExecuteNonQuery();
                Console.WriteLine("Berhasil !");
            }
        }
        catch
        {
            Console.WriteLine("gagal menginputkan data ke database.");
        }
    }

    static void SendData()
    {
        List<PrintScan> printScans = new List<PrintScan>();
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
                printScans.Add(new PrintScan() { id = id, data = data, is_print = is_print, is_scan = is_scan, created_at = created_at, updated_at = updated_at });
            }
        }
        
        UDP(printScans[0].ToString());
    }

    static void UDP(string message = "")
    {
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
        IPAddress serverAddr = IPAddress.Parse("192.168.1.23");
        IPEndPoint endPoint = new IPEndPoint(serverAddr, 8888);
        byte[] send_buffer = Encoding.ASCII.GetBytes(message);
        sock.SendTo(send_buffer, endPoint);
    }
}

public class PrintScan
{
    public int id { get; set; }
    public string data { get; set; }
    public bool is_print { get; set; }
    public bool is_scan { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
    public override string ToString()
    {
        // return json.Format("id: {0}, data: {1}, is_print: {2}, is_scan: {3}, created_at: {4}, updated_at: {5}", id, data, is_print, is_scan, created_at, updated_at);
        return data;
    }
}
