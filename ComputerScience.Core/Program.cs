using ComputerScience.DataStructures;
namespace ComputerScience.Core
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine();
            MyLinkedList<int> list = new (10);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = i;
            }
            list.Add(10);
            list.Insert(100, 0);
            Console.WriteLine(list.Remove(0));
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);
            }
        }
    }
}
