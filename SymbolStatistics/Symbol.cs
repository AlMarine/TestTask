namespace SymbolStatisticsLib
{
    public class Symbol
    {
        private char charecter;
        private int frequency;

        /// <summary>
        /// Класс, представляющий символ и количество его появлений в текстовом документе
        /// </summary>
        /// <param name="charecter">Символ</param>
        public Symbol(char charecter)
        {
            this.charecter = charecter;
            this.frequency = 0;
        }
        /// <summary>
        /// Символ
        /// </summary>
        public char Charecter
        {
            get
            {
                return this.charecter;
            }
        }
        /// <summary>
        /// Частота появления символа в текстовом документе
        /// </summary>
        public int Frequency
        {
            get
            {
                return this.frequency;
            }
            set
            {
                this.frequency = value;
            }
        }
    }
}
