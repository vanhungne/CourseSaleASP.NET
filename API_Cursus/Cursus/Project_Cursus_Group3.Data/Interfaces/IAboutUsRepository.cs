using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.AboutUsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Interfaces
{
    public interface IAboutUsRepository
    {
        Task<AboutUs> Add(CreateAboutUs createAboutUs);
        Task<AboutUs> Update(int id,CreateAboutUs updateAboutUs);
        Task Delete(int id);
        Task<AboutUs> Get(int id);
        Task<IEnumerable<AboutUs>> GetAll();
    }
}
