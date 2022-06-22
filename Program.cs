using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

class Program
{
    static MySqlConnection con = new MySqlConnection("Database=integra;Server=localhost;user=barjono;password=password");

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Input 'g' untuk GET, input 'c' untuk create : ");
            string[] check = Console.ReadLine().Split(',');
            try
            {
                char c = char.ToLower(check[0][0]);
                if (c == 'g'){
                    GetData();
                    continue;
                } else if (c == 'c') {
                    Console.WriteLine("Input data : ");
                    string[] input = Console.ReadLine().Split(',');
                    string data = input[0];
                    bool is_print = false;
                    bool is_scan = false;
                    string created_at = DateTime.Now.ToString("yyyy-MM-dd H:mm:s");
                    string updated_at = DateTime.Now.ToString("yyyy-MM-dd H:mm:s");
                    InsertData(data, is_print, is_scan, created_at, updated_at);
                } else {
                    Console.WriteLine("Input tidak didefinisikan");
                }
                
            } catch {
                Console.WriteLine("Input error");
            }
        }
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

    static void GetData()
    {
        List<PrintScan> printScans = new List<PrintScan>();
        con.Open();

        using (MySqlCommand command = new MySqlCommand("SELECT * FROM print_scan", con))
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

        foreach (PrintScan printScan in printScans)
        {
            Console.WriteLine(printScan);
        }
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
        return string.Format("id: {0}, data: {1}, is_print: {2}, is_scan: {3}, created_at: {4}, updated_at: {5}",
            id, data, is_print, is_scan, created_at, updated_at);
    }
}
