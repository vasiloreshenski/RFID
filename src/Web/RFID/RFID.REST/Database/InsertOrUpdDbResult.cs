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
        public static InsertOrUpdDbResult Create(int id, bool? isInserted)
        {
            if (isInserted == null)
            {
                return NotFound;
            }
            else
            {
                return Create(id, isInserted.Value);
            }
        }

        public static InsertOrUpdDbResult Create(int id, bool isInserted)
        {
            return new InsertOrUpdDbResult
            {
                Id = id,
                IsInserted = isInserted,
                IsUpdated = isInserted == false && id != default
            };
        }

        /// <summary>
        /// Represents not updated and not inserted state
        /// </summary>
        public static InsertOrUpdDbResult NotFound => Create(0, false);

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

        /// <summary>
        /// True if not found
        /// </summary>
        public bool IsNotFound => Object.ReferenceEquals(this, NotFound);
    }
}
