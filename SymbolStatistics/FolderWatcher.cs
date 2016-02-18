using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SymbolStatisticsLib
{
    public class FolderWatcher
    {
        private List<TextFile> textFiles;
        private FileSystemWatcher watcher;
        private List<Symbol> symbols;

        /// <summary>
        /// Класс, ведущий статистику использования символов в текстовых документах в заданой папки
        /// </summary>
        /// <param name="folder">Папка, для которой будет вестись подсчет статистики использования символов в текстовых документах</param>
        public FolderWatcher(string folder)
        {
            try
            {
                this.textFiles = new List<TextFile>();
                this.symbols = new List<Symbol>();
                //Устанавливаем наблюдение за текстовыми документами в заданной папке
                this.watcher = new FileSystemWatcher(folder, "*.txt");
                //Заполняем список существующих в папке текстовых документов
                this.initializeTextFiles(folder);
                //Начинаем отслеживать события папки.
                this.startWatching();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private void startWatching()
        {
            //Отслеживаем запись в текстовый документ, создание, преименование и удаление.
            this.watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            this.watcher.Changed += new FileSystemEventHandler(OnChanged);
            this.watcher.Created += new FileSystemEventHandler(OnChanged);
            this.watcher.Deleted += new FileSystemEventHandler(OnChanged);
            this.watcher.Renamed += new RenamedEventHandler(OnRenamed);
            this.watcher.EnableRaisingEvents = true;
        }

        private void initializeTextFiles(string folder)
        {
            //Загружаем файлы из заданной папки
            foreach (string file in Directory.GetFiles(folder))
            {
                //Выбираем текстовые документы
                if (file.Split('.').Last() == "txt")
                {
                    TextFile textFile = new TextFile(file);
                    this.addTextFile(textFile);
                }
            }

            this.calculateTotalStatistics();
            this.display();
        }

        private void addTextFile(TextFile textFile)
        {
            //Добавляем только не повторяющиеся
            if (this.textFiles.Where(tf => tf.Hash.SequenceEqual(textFile.Hash)).Count() == 0)
            {
                this.textFiles.Add(textFile);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Если изменили текстовый документ
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                //Пересчитываем hash соответствующего экземпляра TextFile
                IEnumerable<TextFile> list = this.textFiles.Where(textFile => textFile.Path == e.FullPath).ToList();
                if (list.Count() > 0)
                    list.First().AplyChanges();
            }
            //Если создали текстовый документ
            else if (e.ChangeType == WatcherChangeTypes.Created)
            {
                //Добавляем текстовый документ
                TextFile textFile = new TextFile(e.FullPath);
                this.addTextFile(textFile);
                
            }
            //Если удалили текстовый документ
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                /*
                 * ДОРАБОТАТЬ УЧЕТ ПОВТОРЯЮЩИХСЯ ФАЙЛОВ ПРИ УДАЛЕНИИ
                */
                
                //Удаляем текстовый документ
                IEnumerable<TextFile> list = this.textFiles.Where(textFile => textFile.Path == e.FullPath);
                if (list.Count() > 0)
                    this.textFiles.Remove(list.First());
            }
            this.calculateTotalStatistics();

            this.display();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            //Изменяем путь соответствующего экземпляра TextFile
            IEnumerable<TextFile> list = this.textFiles.Where(textFile => textFile.Path == e.OldFullPath);
            if (list.Count() > 0)
                list.First().Path = e.FullPath;
            //this.textFiles.Where(textFile => textFile.Path == e.OldFullPath).FirstOrDefault().Path = e.FullPath;
            this.display();
        }

        //Подсчитываем общую статистику в текстовых документах папки
        private void calculateTotalStatistics()
        {
            this.symbols.Clear();
            List<Symbol> list = new List<Symbol>();
            //Соединяем статистики всех текстовых документов
            foreach (TextFile textFile in this.textFiles)
            {
                list.AddRange(textFile.Symbols);
            }
            //Обрабатываем элементы общей статистики
            foreach (Symbol symbol in list)
            {
                this.statAdd(symbol);
            }
            //Сортируем статистику папки по убыванию
            this.symbols = this.symbols.OrderByDescending(sybol => sybol.Frequency).ToList();
        }

        private void statAdd(Symbol symbol)
        {
            //Проверяем, есть ли символ в статистике папки
            IEnumerable<Symbol> list = this.symbols.Where(s => s.Charecter == symbol.Charecter);
            //Если уже есть, увеличиваем частоту появления символа
            if (list.Count() > 0)
            {
                list.First().Frequency += symbol.Frequency;
            }
            //Если символа нет, добавляем его
            else
            {
                this.symbols.Add(symbol);
            }
        }

        /// <summary>
        /// Список текстовых документов, содержащихся в заданой папке
        /// </summary>
        public List<TextFile> TextFiles
        {
            get
            {
                return this.textFiles;
            }
        }

        //Выводим статистику использования символов в каждом текстовом документе и в целом в папке
        private void display()
        {
            System.Console.WriteLine("\n__________________________________________________");
            foreach (TextFile textFile in this.textFiles)
            {
                System.Console.WriteLine("\n" + textFile.ToString());
            }
            System.Console.WriteLine("\n-----------------Общая статистика-----------------");
            int i = 0;
            foreach (Symbol symbol in this.symbols)
            {
                System.Console.WriteLine("['{0}'] - {1}", symbol.Charecter, symbol.Frequency);
                ++i;
                if (i == 5)
                    break;
            }
            System.Console.WriteLine("__________________________________________________");
        }
    }
}
