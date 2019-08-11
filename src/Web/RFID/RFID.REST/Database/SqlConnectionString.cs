namespace RFID.REST.Database
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Class representing value sql connection string
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed class SqlConnectionString
    {
        /// <summary>
        /// Creates new connection string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="value"/> is null</exception>
        public static SqlConnectionString Create(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Connection string can't be null");
            }

            return new SqlConnectionString(value);
        }

        private SqlConnectionString(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Connection string
        /// </summary>
        public String Value { get; }
    }
}
