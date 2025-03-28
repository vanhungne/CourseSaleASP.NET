using AutoMapper;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.ViewModels.StoreInforDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IStoreInforRepository
    {
        Task<StoreInfos> AddOrUpdateStoreInfor(StoreViewModel info);
        Task DeleteStoreInfor(int id);
        Task<StoreViewModel> GetStoreViewModel();
    }
}
