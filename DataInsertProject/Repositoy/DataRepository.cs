using DataInsertProject.Context;
using DataInsertProject.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataInsertProject.Repositoy
{
    public class DataRepository
    {
        private readonly AppDbContext _context;

        public DataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task InsertDataAsync(IEnumerable<DataModel> dataModels)
        {
            await _context.DataModels.AddRangeAsync(dataModels);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DataModel>> GetAllDataAsync()
        {
           return await _context.Set<DataModel>().ToListAsync();

        }


    }
}
