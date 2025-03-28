using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.ViewModels.StoreInforDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class StoreInforRepository : Repository<StoreInfos>, IStoreInforRepository
    {
        private readonly IMapper _mapper;
        private readonly CursusDbContext _context;
        public StoreInforRepository(CursusDbContext dbContext,IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
            _context = dbContext;
        }

        public async Task<StoreInfos> AddOrUpdateStoreInfor(StoreViewModel info)
        {
            var existingStoreInfo = await _context.StoreInfos.FirstOrDefaultAsync();
            StoreInfos storeInfo;
            if (existingStoreInfo != null)
            {
                storeInfo = existingStoreInfo;
            }
            else
            {
                storeInfo = new StoreInfos();
            }
            if (info.BranchName != null)
                storeInfo.BranchName = info.BranchName;

            if (info.SupportHotline != null)
                storeInfo.SupportHotline = info.SupportHotline;

            if (info.PhoneNumber != null)
                storeInfo.PhoneNumber = info.PhoneNumber;

            if (info.Address != null)
                storeInfo.Address = info.Address;

            if (info.WorkingTime != null)
                storeInfo.WorkingTime = info.WorkingTime;

            if (info.Email != null)
                storeInfo.Email = info.Email;

            if (info.Facebook != null)
                storeInfo.Facebook = info.Facebook;

            if (info.Zalo != null)
                storeInfo.Zalo = info.Zalo;

            if (info.YouTube != null)
                storeInfo.YouTube = info.YouTube;

            if (info.Instagram != null)
                storeInfo.Instagram = info.Instagram;
            if (existingStoreInfo != null)
            {
                _context.StoreInfos.Update(storeInfo); // Cập nhật
            }
            else
            {
                await _context.StoreInfos.AddAsync(storeInfo); // Thêm mới
            }
            await _context.SaveChangesAsync();
            return storeInfo;
        }


        public async Task DeleteStoreInfor(int id)
        {
            var exis = await _context.StoreInfos.FindAsync(id);
            if(exis != null)
            {
                  _context.StoreInfos.Remove(exis);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<StoreViewModel> GetStoreViewModel()
        {
            var info = await _context.StoreInfos.FirstOrDefaultAsync();
            if(info != null)
            {
                var viewModel = _mapper.Map<StoreViewModel>(info);
                return viewModel;
            }
            return null;
        }
    }
}
