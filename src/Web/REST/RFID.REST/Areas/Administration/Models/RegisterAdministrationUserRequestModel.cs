﻿namespace RFID.REST.Areas.Administration.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Model used for registering administration users
    /// </summary>
    public class RegisterAdministrationUserRequestModel
    {
        /// <summary>
        /// Email for the user - this will be your login account
        /// </summary>
        [Required]
        [EmailAddress]
        public String Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public String Password { get; set; }

        /// <summary>
        /// Roles for the user
        /// </summary>
        [BindRequired]
        [EnumDataType(typeof(UserRoles))]
        public UserRoles Roles { get; set; }
    }
}
