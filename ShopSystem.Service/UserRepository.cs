﻿using ShopSystem.Core.Dtos;
using ShopSystem.Core.Services;
using ShopSystem.Repository.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly AppIdentityDbContext _context;

        public UserRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        public void UpdateUser(string userId, UserForUserDto updateUserDto)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.FirstName = updateUserDto.FirstName;
                user.LastName = updateUserDto.LastName;
                // user.Email = updateUserDto.Email;
                // Update other properties as needed
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("User not found");
            }
        }

        public void DeleteUser(string userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("User not found");
            }
        }



    }
}
