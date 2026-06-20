
using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DB_GranjaLaFlor.Services
{
    public class RoleService
    {

        //  ************************************** Dependencies **************************************

        private readonly ApplicationDbContext _context;

        //  ************************************** Constructor **************************************

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        //  ************************************** Validations **************************************

        private static string NormalizeText(string value)
        {
            /*
             * Noramalizing text to keep a unic format in DB. \s+ = espacio en blando, mas de un espacio. " " = allow an space within the string   
             * Private method as it is only going to be  called in RoleController. 
            */
            return Regex.Replace(
                value.Trim(),@"\s+"," ");
        }


        //  ************************************** Query Methods (Read) **************************************

        public async Task<List<Role>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        // Odicial Doc: "The .NET style convention is to add the "Async" suffix to all asynchronous method names."
        public async Task<List<Role>> GetAllActiveAsync()
        {
            // Use .AsNoTracking() when only consultimg DB. It reduces memoruy consuption and improves performance as it does not track objects.  
            return await _context.Roles
                .AsNoTracking()
                .Where(role => role.RoleState)
                .ToListAsync();
        }

        public async Task<List<Role>> GetAllInactiveAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(role => !role.RoleState)
                .ToListAsync();
        }


        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(role => role.RoleId == id);
        }
        //==================================================
        // Command Methods (Create / Update / Delete)
        //==================================================

        public async Task CreateAsync(Role role)
        {
            role.RoleName = NormalizeText(role.RoleName);

            var roleExists = await _context.Roles
                .AnyAsync(existingRole => existingRole.RoleName == role.RoleName);

            if (roleExists)
            {
                throw new InvalidOperationException("El rol ya existe.");
            }

            role.RoleState = true;

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            role.RoleName = NormalizeText(role.RoleName);

            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == role.RoleId);

            if (existingRole == null)
            {
                throw new InvalidOperationException("Rol no encontrado.");
            }

            var roleExists = await _context.Roles
                .AnyAsync(existingRole =>
                    existingRole.RoleName == role.RoleName &&
                    existingRole.RoleId != role.RoleId);

            if (roleExists)
            {
                throw new InvalidOperationException("El rol ya existe.");
            }

            existingRole.RoleName = role.RoleName;
            existingRole.RoleDescription = role.RoleDescription;

            await _context.SaveChangesAsync();
        }


        public async Task SoftDeleteAsync(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(role => role.RoleId == id);

            if (role == null)
            {
                throw new InvalidOperationException("Rol no encontardo.");
            }

            role.RoleState = false;

            await _context.SaveChangesAsync();
        }
        public async Task ActivateAsync(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(role => role.RoleId == id);

            if (role == null)
            {
                throw new InvalidOperationException("Rol no encontardo.");
            }

            role.RoleState = true;

            await _context.SaveChangesAsync();
        }

        //==================================================
        // Private Helper Methods
        //==================================================

    }
}


/*

namespace DB_GranjaLaFlor.Services
{
    public class RoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Role>> GetAllActiveAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(role => role.RoleState)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(role => role.RoleId == id);
        }
    }
}
*/
