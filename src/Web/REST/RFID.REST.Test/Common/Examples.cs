namespace RFID.REST.Test.Common
{
    using RFID.REST.Areas.Administration.Models;
    using RFID.REST.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;


    internal static class Examples
    {
        public static RegisterAdministrationUserRequestModel Administrator()
        {
            return Administrator("test@test.com", "123");
        }

        public static RegisterAdministrationUserRequestModel Administrator(String email, String password)
        {
            return new RegisterAdministrationUserRequestModel { Email = email, Password = password, Roles = UserRoles.Admin };
        }

        public static RegisterTagRequestModel Tag()
        {
            return Tag(Guid.Empty, "test");
        }

        public static RegisterTagRequestModel Tag(Guid guid, String userName)
        {
            return Tag(guid, 2, userName);
        }

        public static RegisterTagRequestModel Tag(Guid guid, int accessLevel, String userName)
        {
            return Tag(guid.ToString(), accessLevel, userName);
        }

        public static RegisterTagRequestModel Tag(String number, int accessLevel, String userName)
        {
            return new RegisterTagRequestModel
            {
                AccessLevel = (AccessLevel)accessLevel,
                Number = number,
                UserName = userName
            };
        }

        public static UpdateTagRequestModel TagUpdate(int id, String username)
        {
            return new UpdateTagRequestModel
            {
                Id = id,
                UserName = username
            };
        }



        public static RegisterAccessPointRequestModel AccessPoint()
        {
            return AccessPoint("test");
        }

        public static RegisterAccessPointRequestModel AccessPoint(String serialNumber)
        {
            return AccessPoint(serialNumber, "test", AccessLevel.Mid);
        }

        public static RegisterAccessPointRequestModel AccessPoint(String serialNumber, String description, AccessLevel accessLevel)
        {
            return new RegisterAccessPointRequestModel
            {
                AccessLevel = accessLevel,
                Description = description,
                IsActive = true,
                SerialNumber = serialNumber,
                Direction = AccessPointDirectionType.Entrance
            };
        }

        public static UnKknownAccessPointMock UnKnownAccessPoint(String serialNumber)
        {
            return new UnKknownAccessPointMock { SerialNumber = serialNumber };
        }

        public static UnKnownTagMock UnKnownTag(String number)
        {
            return new UnKnownTagMock { Number = number };
        }
    }
}
