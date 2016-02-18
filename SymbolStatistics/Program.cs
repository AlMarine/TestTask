namespace SymbolStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length != 0)
            {
                path = args[0];
            }
            else
            {
                System.Console.WriteLine("Введите путь к папке.");
                path = System.Console.ReadLine();
            }
            SymbolStatisticsLib.FolderWatcher folderWatcher = new SymbolStatisticsLib.FolderWatcher(path);
            System.Console.WriteLine("Нажмите любую клавишу для выхода.");
            System.Console.ReadKey();
        }
    }
}
