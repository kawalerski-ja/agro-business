using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Klasa błędu stosowana w klasie Rośliny, aby symululacja była bardziej standardowa (nie było wartości tak dużych/małych, że aż nierealnych
    /// </summary>
    internal class ArgumentOutOfRangeException : Exception
    {
        public ArgumentOutOfRangeException(string message, string v) : base(message) { }
    }
}
