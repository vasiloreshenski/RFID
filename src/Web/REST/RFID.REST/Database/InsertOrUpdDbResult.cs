﻿namespace RFID.REST.Database
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
        public static InsertOrUpdDbResult Create(int id, bool isInserted, bool isUpdated)
        {
            if (id == default)
            {
                return NotFound;
            }

            return new InsertOrUpdDbResult
            {
                Id = id,
                IsInserted = isInserted,
                IsUpdated = isUpdated
            };
        }

        /// <summary>
        /// Represents not updated and not inserted state
        /// </summary>
        public static InsertOrUpdDbResult NotFound { get; } = new InsertOrUpdDbResult { Id = 0, IsInserted = false, IsUpdated = false };

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

        public bool HasIdentity => this.Id != default;
    }
}
