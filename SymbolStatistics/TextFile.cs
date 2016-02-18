using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SymbolStatisticsLib
{
    public class TextFile
    {
        private byte[] hash;
        private string path;
        private List<Symbol> symbols;
        private int totalSymbols;
        private HashAlgorithm hashAlgorithm;
        private Encoding encoding;
        private IEnumerable<char> chars;

        /// <summary>
        /// Класс, считающий количество использования символов в конкретном текстовом документе
        /// </summary>
        /// <param name="path">Путь текстового документа</param>
        public TextFile(string path)
        {
            this.path = path;
            this.symbols = new List<Symbol>();
            this.hashAlgorithm = HashAlgorithm.Create();
            this.calculateHash();
            this.getEncoding();
            this.getSymbols();
        }

        public void AplyChanges()
        {
            this.calculateHash();
            this.getSymbols();
        }

        //Расчет hash'а
        private void calculateHash()
        {
            using(FileStream fileStream = new FileStream(this.path, FileMode.Open, FileAccess.Read))
            {
                this.hash = this.hashAlgorithm.ComputeHash(fileStream);
            }
        }

        private void getSymbols()
        {
            //Получаем все символы из текстового документа с учетом кодировки
            this.chars = this.encoding.GetChars(File.ReadAllBytes(this.path));
            //Не учитываем переносы строк
            this.chars = this.chars.Where(c => c != '\n' && c != '\r');
            //Получаем общее число символов в текстовом документе
            this.totalSymbols = this.chars.Count();
            //Заново рассчитываем статистику символов
            this.symbols.Clear();
            //Проходим по всем символам текстового документа
            foreach (char c in this.chars)
            {
                //Ищем каждый символ в статистике текстового файла
                IEnumerable<Symbol> list = this.symbols.Where(symbol => symbol.Charecter == c);
                //Если символа нет, добавляем его
                if (list.Count() == 0)
                {
                    Symbol symbol = new Symbol(c);
                    symbol.Frequency = 1;
                    this.symbols.Add(symbol);
                }
                //Если символ есть, увеличиваем частоту его появления
                else
                {
                    list.First().Frequency++;
                }
            }
            //Сортируем статистику по убыванию
            this.symbols = this.symbols.OrderByDescending(sybol => sybol.Frequency).ToList();
        }

        //Определение кодировки
        private void getEncoding()
        {
            byte[] bom = new byte[4];
            using (FileStream fileStream = new FileStream(this.path, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(bom, 0, 4);
            }
            
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                this.encoding = Encoding.UTF7;
            else if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                this.encoding = Encoding.UTF8;
            else if (bom[0] == 0xff && bom[1] == 0xfe)
                this.encoding = Encoding.Unicode;
            else if (bom[0] == 0xfe && bom[1] == 0xff)
                this.encoding = Encoding.BigEndianUnicode; //UTF-16BE
            else if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                this.encoding = Encoding.UTF32;
            else
                this.encoding = Encoding.ASCII;
        }

        public override string ToString()
        {
            return string.Format("      Файл: {0}\nСтатистика: "
                                                 +"({1} - {2}, "
                                                 +"{3} - {4}, "
                                                 +"{5} - {6}, "
                                                 +"{7} - {8}, "
                                                 +"{9} - {10})", 
                                  this.Path.Split('\\').Last(),
                                  this.symbols.Count > 0 ? string.Format(@"['{0}']", this.symbols[0].Charecter) : "", this.symbols.Count > 0 ? this.symbols[0].Frequency.ToString() : "",
                                  this.symbols.Count > 1 ? string.Format(@"['{0}']", this.symbols[1].Charecter) : "", this.symbols.Count > 1 ? this.symbols[1].Frequency.ToString() : "",
                                  this.symbols.Count > 2 ? string.Format(@"['{0}']", this.symbols[2].Charecter) : "", this.symbols.Count > 2 ? this.symbols[2].Frequency.ToString() : "",
                                  this.symbols.Count > 3 ? string.Format(@"['{0}']", this.symbols[3].Charecter) : "", this.symbols.Count > 3 ? this.symbols[3].Frequency.ToString() : "",
                                  this.symbols.Count > 4 ? string.Format(@"['{0}']", this.symbols[4].Charecter) : "", this.symbols.Count > 4 ? this.symbols[4].Frequency.ToString() : "");
        }
        /// <summary>
        /// Hash текстового документа
        /// </summary>
        public byte[] Hash
        {
            get
            {
                return this.hash;
            }
            set
            {
                this.hash = value;
            }
        }
        /// <summary>
        /// Путь к файлу текстового документа
        /// </summary>
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }
        /// <summary>
        /// Набор символов, использованных в текстовом документе в порядке их появления
        /// </summary>
        public List<Symbol> Symbols
        {
            get
            {
                return this.symbols;
            }
            set
            {
                this.symbols = value;
            }
        }
        /// <summary>
        /// Общее количество символов в текстовом документе
        /// </summary>
        public int TotalSymbols
        {
            get
            {
                return this.totalSymbols;
            }
        }
    }
}
