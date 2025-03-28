using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.ViewModels.StoreInforDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class StoreInforSerrvice : IStoreInforSerrvice
    {
        private readonly IStoreInforRepository _repo;

        public StoreInforSerrvice(IStoreInforRepository repo)
        {
            _repo = repo;
        }

        public async Task<StoreInfos> AddOrUpdateStoreInfor(StoreViewModel info)
        {
           return await _repo.AddOrUpdateStoreInfor(info);
        }

        public async Task DeleteStoreInfor(int id)
        {
           await _repo.DeleteStoreInfor(id);
        }

        public async Task<StoreViewModel> GetStoreViewModel()
        {
           return await _repo.GetStoreViewModel();
        }
    }
}
