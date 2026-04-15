using ComputerScience.DataStructures;
namespace ComputerScience.Core
{
    internal class Program
    {
        static void Main ()
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine();
            Dictionary<string, string> list = new();
            for ( int i = 0; i < 5000000; i++ )
            {
                list.Add("1234567890" + i.ToString(), "1234567890" + i.ToString());
            }
            string c;
            DateTime start = DateTime.Now;
            c = list["1234567890" + 2500000.ToString()];
            Console.WriteLine(DateTime.Now - start);
            List<string> arr = new(5000000);
            for ( int i = 0; i < arr.Capacity; i++ )
            {
                arr.Add("1234567890" + i.ToString());
            }
            start = DateTime.Now;
            c = arr[2500000];
            Console.WriteLine(DateTime.Now - start);
        }
    }
}
