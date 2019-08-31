namespace RFID.REST.Areas.Auth.Services
{
    using RFID.REST.Areas.Auth.Models;
    using RFID.REST.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Auth
    {
        private readonly SqlConnectionFactory connectionFactory;

        public bool TryIssueTokenForUser(TokenGenerationRequestModel model, out String token)
        {
            throw new NotImplementedException();
        }
    }
}
