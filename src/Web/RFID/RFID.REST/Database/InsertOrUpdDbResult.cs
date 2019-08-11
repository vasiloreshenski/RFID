namespace RFID.REST.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents result from insert or update operation in the database
    /// </summary>
    public class InsertOrUpdDbResult
    {
        /// <summary>
        /// Identity of the table
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// True if inserted
        /// </summary>
        public bool IsInserted { get; set; }

        /// <summary>
        /// True if updated
        /// </summary>
        public bool IsUpdated { get; set; }
    }
}
