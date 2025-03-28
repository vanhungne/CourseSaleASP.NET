using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Data.ViewModels.ChapterDTO;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Services
{
    public class ChapterServices : IChapterServices
    {
        private readonly IChapterRepository _chapterRepository;

        public ChapterServices(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<List<AddChapterModel>> AddChaptersAsync(List<AddChapterModel> addChapterModel, string userName)
        {
            await _chapterRepository.AddChaptersAsync(addChapterModel, userName);
            return addChapterModel;
        }

        public async Task<Chapter> DeleteChapterAsync(int id)
        {
            return await _chapterRepository.DeleteChapterAsync(id);
        }

        public async Task<Chapter?> GetChapterByIdAsync(int id)
        {
            return await _chapterRepository.GetChapterByIdAsync(id);
        }

        public Task UpdateChapter(Chapter chapter)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateChapterAsync(int id, UpdateChapterModel updateChapterModel)
        {
            await _chapterRepository.UpdateChapterAsync(id, updateChapterModel);
        }
        
        public async Task<List<ChapterViewModel>> ViewActiveChaptersAsync(string userName,string? searchContent, string? sortBy, bool ascending, int? pageNumber, int? pageSize, int? filterDuration)
        {
            return await _chapterRepository.ViewActiveChaptersAsync(userName, searchContent, sortBy, ascending, pageNumber, pageSize, filterDuration);
        }
    }
}
