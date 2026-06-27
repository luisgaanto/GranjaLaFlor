using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB_GranjaLaFlor.Services
{
    public class BroilerHouseService
    {
        private readonly ApplicationDbContext _context;

        public BroilerHouseService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Returns all active Broiler Houses.
         * Broiler Houses are managed directly from the database,
         * therefore this service only provides read operations.
         */
        public async Task<List<BroilerHouse>> GetAllActiveAsync()
        {
            return await _context.BroilerHouses
                .AsNoTracking()
                .Where(broilerHouse => broilerHouse.BroilerHouseState)
                .OrderBy(broilerHouse => broilerHouse.BroilerHouseName)
                .ToListAsync();
        }
    }
}