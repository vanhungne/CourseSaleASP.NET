using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.AboutUsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class AboutUsRepository : IAboutUsRepository
    {
        private readonly CursusDbContext _context;
        private readonly IMapper _mapper;

        public AboutUsRepository(CursusDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AboutUs> Add(CreateAboutUs createAboutUs)
        {
          var aboutUsX = _mapper.Map<AboutUs>(createAboutUs);
            await _context.AboutUs.AddAsync(aboutUsX);
            await _context.SaveChangesAsync();
            return aboutUsX;
        }

        public async Task Delete(int id)
        {
            var exisAboutUs = await _context.AboutUs.FirstOrDefaultAsync(x => x.Id == id);
            if (exisAboutUs != null)
            {
                 _context.AboutUs.Remove(exisAboutUs);
               await _context.SaveChangesAsync();
            }
        }

        public async Task<AboutUs> Get(int id)
        {
            var exis = await _context.AboutUs.FindAsync(id);
            if (exis != null)
            {
                return _mapper.Map<AboutUs>(exis);
            }
            else return null;
        }

        public async Task<IEnumerable<AboutUs>> GetAll()
        {
           var list = await _context.AboutUs.ToListAsync();
            return list;
        }

        public async Task<AboutUs> Update(int id, CreateAboutUs updateAboutUs)
        {
           var exis = await _context.AboutUs.FirstOrDefaultAsync(x=>x.Id == id);
            if (exis == null)
            {
                throw new ArgumentException("About Us not exis");
            }
            _mapper.Map(updateAboutUs,exis);
            _context.AboutUs.Update(exis);
            await _context.SaveChangesAsync();
            return exis;
        }
    }
}
