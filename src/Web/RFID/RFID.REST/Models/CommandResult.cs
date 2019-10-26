namespace RFID.REST.Models
{
    using Newtonsoft.Json;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommandResult
    {
        public static CommandResult<T> NotFound<T>()
        {
            return new CommandResult<T>(default(T), CommandStatus.NotFound);
        }

        public static CommandResult FromDbResult(InsertOrUpdDbResult dbResult)
        {
            var status = CommandStatus.None;
            if (dbResult.IsInserted)
            {
                status = CommandStatus.Created;
            }
            else if (dbResult.IsUpdated)
            {
                status = CommandStatus.Updated;
            }
            else if (dbResult.IsNotFound)
            {
                status = CommandStatus.NotFound;
            }
            else if (dbResult.HasIdentity)
            {
                status = CommandStatus.Dublicate;
            }
            else
            {
                throw new ArgumentException($"Can't create CommandResult from dbResult with unknown status -> {JsonConvert.SerializeObject(dbResult)}");
            }

            return new CommandResult(status);
        }

        public CommandResult(CommandStatus status)
        {
            Status = status;
        }

        public CommandStatus Status { get; }

        public bool Success => (int)this.Status < 100;
    }

    public class CommandResult<T> : CommandResult
    {
        public CommandResult(T value, CommandStatus status) : base(status)
        {
            this.Value = value;
        }

        public T Value { get; }
    }
}
