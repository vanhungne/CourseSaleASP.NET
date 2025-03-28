using Google;
using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        private readonly CursusDbContext _dbcontext;

        public WalletRepository(CursusDbContext dbcontext) : base(dbcontext)
        {

            _dbcontext = dbcontext;
        }

        public async Task<Wallet?> GetWalletByUserNameAsync(string userName)
        {
            return await Entities
                .Include(w => w.Transactions) 
                .FirstOrDefaultAsync(w => w.UserName == userName); 
        }
    }
}
