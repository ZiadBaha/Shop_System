using Microsoft.AspNetCore.Http;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Errors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Account
{
    public class UserDto : ApiResponse
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public UserRole Role { get; set; }

        public string PhoneNumber { get; set; }

        public TimeSpan? DurationSpent { get; set; }

    }
}
