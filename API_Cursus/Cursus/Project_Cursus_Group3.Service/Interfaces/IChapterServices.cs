using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.ViewModels.ChapterDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IChapterServices
    {
        Task<Chapter?> GetChapterByIdAsync(int id);
        Task<List<ChapterViewModel>> ViewActiveChaptersAsync(
                    string userName,
                    string? searchContent,
                    string? sortBy,
                    bool ascending,
                    int? pageNumber,
                    int? pageSize,
                    int? filterDuration
        );
        Task<List<AddChapterModel>> AddChaptersAsync(List<AddChapterModel> addChapterModel, string userName);
        Task UpdateChapter(Chapter chapter);
        Task UpdateChapterAsync(int id, UpdateChapterModel updateChapterModel);
        Task<Chapter> DeleteChapterAsync(int id);
    }
}
