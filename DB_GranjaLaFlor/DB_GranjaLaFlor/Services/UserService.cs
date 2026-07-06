using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace DB_GranjaLaFlor.Services
{
    public class UserService
    {
        /*
         * Registers Microsoft's PasswordHasher service in the Dependency Injection (DI)
         * container. Whenever a class requires IPasswordHasher<User>, ASP.NET Core
         * automatically creates a PasswordHasher<User> instance and injects it.
        */
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        private static string NormalizeText(string value)
        {
            return Regex.Replace(
                value.Trim(),
                @"\s+",
                " ");
        }


        public async Task<List<User>> GetAllActiveAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(user => user.Role)
                .Where(user => user.UserState)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllInactiveAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(user => user.Role)
                .Where(user => !user.UserState)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.UserId == id);
        }

        public async Task<User?> GetActiveByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Users
                .AsNoTracking()
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user =>
                    user.UserEmail == normalizedEmail &&
                    user.UserState);
        }

        public async Task CreateAsync(User user)
        {
            user.UserName = NormalizeText(user.UserName);
            user.UserEmail = user.UserEmail.Trim().ToLower();

            user.UserDescription = string.IsNullOrWhiteSpace(user.UserDescription)
                ? null
                : NormalizeText(user.UserDescription);

            var emailExists = await _context.Users
                .AnyAsync(existingUser => existingUser.UserEmail == user.UserEmail);

            if (emailExists)
            {
                throw new InvalidOperationException("El correo electrónico ya existe.");
            }

            var nameExists = await _context.Users
                .AnyAsync(existingUser => existingUser.UserName == user.UserName);

            if (nameExists)
            {
                throw new InvalidOperationException("El usuario ya existe.");
            }

            user.UserState = true;

            /*
              * Hashes the user's password before saving it into the database.
              * The original password is never stored, only its secure hash.
             */
            user.UserPassword = _passwordHasher.HashPassword(user,user.UserPassword);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            user.UserName = NormalizeText(user.UserName);
            user.UserEmail = user.UserEmail.Trim().ToLower();

            user.UserDescription = string.IsNullOrWhiteSpace(user.UserDescription)
                ? null
                : NormalizeText(user.UserDescription);

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(existingUser => existingUser.UserId == user.UserId);

            if (existingUser == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");
            }

            var emailExists = await _context.Users
                .AnyAsync(existingUser =>
                    existingUser.UserEmail == user.UserEmail &&
                    existingUser.UserId != user.UserId);

            if (emailExists)
            {
                throw new InvalidOperationException("El correo electrónico ya existe.");
            }

            var nameExists = await _context.Users
                .AnyAsync(existingUser =>
                    existingUser.UserName == user.UserName &&
                    existingUser.UserId != user.UserId);

            if (nameExists)
            {
                throw new InvalidOperationException("El usuario ya existe.");
            }

            existingUser.UserName = user.UserName;
            existingUser.UserEmail = user.UserEmail;
            // Password is not updated from this method.
            // Password changes will be handled through a separate Password Recovery process.
            existingUser.UserDescription = user.UserDescription;
            existingUser.RoleId = user.RoleId;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id, int currentUserId)
        {
            /*
              * Business Rule | Current User Protection: The authenticated user cannot deactivate their own account.
              * The current user is identified using the NameIdentifier claim created during Login.
            */
            if (id == currentUserId)
            {
                throw new InvalidOperationException(
                    "No puede desactivar el usuario con el que inició sesión.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.UserId == id);

            if (user == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");
            }

            user.UserState = false;

            await _context.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.UserId == id);

            if (user == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");
            }

            user.UserState = true;

            await _context.SaveChangesAsync();
        }



    }
}