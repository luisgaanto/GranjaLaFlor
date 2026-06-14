
using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB_GranjaLaFlor.Services
{
    public class RoleService
    {
        //==================================================
        // Dependencies
        //==================================================

        private readonly ApplicationDbContext _context;

        //==================================================
        // Constructor
        //==================================================

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        //==================================================
        // Query Methods (Read)
        //==================================================

        public async Task<List<Role>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Role>> GetAllActiveAsync()
        {
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
            throw new NotImplementedException();
        }

        //==================================================
        // Command Methods (Create / Update / Delete)
        //==================================================

        public async Task CreateAsync(Role role)
        {
            role.RoleState = true;

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public async Task SoftDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task ActivateAsync(int id)
        {
            throw new NotImplementedException();
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
