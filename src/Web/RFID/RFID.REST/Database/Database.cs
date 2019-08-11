namespace RFID.REST.Database
{
    using RFID.REST.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Class used to encapsulate the communication with the database stored procedures
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Registers a user with the provided username if does not exists already and returns the Id of the user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="transaction">Transaction</param>
        /// <returns></returns>
        public async Task<int> GetIdOrRegisterUserAsync(String username, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers a tag with the specified number and access level and associated user if such tag does not exists already.
        /// </summary>
        /// <param name="number">Number of the tag</param>
        /// <param name="userId">User id</param>
        /// <param name="accessLevel">Tag access level</param>
        /// <param name="transaction">Transaction</param>
        /// <returns>True if the tag does not already exists and is added to the database</returns>
        public Task<bool> RegisterTagIfNotExistsAsync(String number, int userId, TagAccessLevel accessLevel, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
