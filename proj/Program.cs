namespace proj
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Data data = new Data();

            Console.WriteLine("Creating tables...");
            data.CreateTables();
            Console.WriteLine("Tables are ready.");
        }
    }
}
