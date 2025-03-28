using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Extensions;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.ViewModels.ChapterDTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Repository
{
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        private readonly CursusDbContext _context;
        private readonly IConfiguration _configuration;

        public ChapterRepository(CursusDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<AddChapterModel>> AddChaptersAsync(List<AddChapterModel> addChapterModels, string userName)
        {
            foreach (var addChapterModel in addChapterModels)
            {
                var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == addChapterModel.CourseId);

                if (course == null)
                {
                    throw new KeyNotFoundException($"Course {addChapterModel.CourseId} does not exist.");
                }

                if (course.Username != userName)
                {
                    throw new InvalidOperationException($"User cannot add chapters to course {addChapterModel.CourseId} that does not belong to them.");
                }

                if (string.IsNullOrWhiteSpace(addChapterModel.ChapterTitle))
                {
                    throw new InvalidOperationException($"Chapter title must not be null.");
                }

                if (await _context.Chapter.AnyAsync(c => c.CourseId == addChapterModel.CourseId && c.ChapterTitle == addChapterModel.ChapterTitle))
                {
                    throw new InvalidOperationException($"Chapter title '{addChapterModel.ChapterTitle}' must be unique within the course {addChapterModel.CourseId}.");
                }

                if (ContainsSpecialCharacters(addChapterModel.ChapterTitle))
                {
                    throw new ArgumentException("ChapterTitle cannot contain special characters.");
                }

                if (ContainsSpecialCharacters(addChapterModel.SubDescription))
                {
                    throw new ArgumentException("SubDescription cannot contain special characters.");
                }

                if (ContainsSpecialCharacters(addChapterModel.Description))
                {
                    throw new ArgumentException("Description cannot contain special characters.");
                }

                if(addChapterModel.Process < 0 && addChapterModel.Process > 100)
                {
                    throw new ArgumentException("Process must be between 0 and 100%.");
                }

                var chapter = new Chapter
                {
                    CourseId = addChapterModel.CourseId,
                    ChapterTitle = addChapterModel.ChapterTitle,
                    SubDescription = addChapterModel.SubDescription,
                    Description = addChapterModel.Description,
                    Process = addChapterModel.Process,
                    Duration = addChapterModel.Duration,
                    Status = "Active",
                };

                _context.Chapter.Add(chapter);
            }

            await _context.SaveChangesAsync();

            return addChapterModels;
        }

        public async Task<Chapter> DeleteChapterAsync(int id)
        {
            var chapter = await _context.Chapter
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.ChapterId == id);

            if (chapter == null)
            {
                throw new KeyNotFoundException($"Chapter {id} does not exist.");
            }

            if (chapter.Lessons.Any())
            {
                throw new InvalidOperationException("This Chapter is already had Students studying. Cannot inactive!");
            }

            chapter.Status = "Inactive";
            await _context.SaveChangesAsync();

            return chapter;
        }

        public async Task<Chapter?> GetChapterByIdAsync(int id)
        {
            return await Entities
              .Include(x => x.Course)
              .FirstOrDefaultAsync(x => x.ChapterId == id);
        }

        public async Task UpdateChapter(Chapter chapter)
        {
            _context.Chapter.Update(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChapterAsync(int id, UpdateChapterModel updateChapterModel)
        {
            var chapter = await _context.Chapter.FindAsync(id);

            if (chapter == null)
            {
                throw new KeyNotFoundException("Chapter does not exist.");
            }

            if (updateChapterModel.CourseId.HasValue)
            {
                var courseExists = await _context.Course.AnyAsync(c => c.CourseId == updateChapterModel.CourseId.Value);
                if (!courseExists)
                {
                    throw new InvalidOperationException($"Course {updateChapterModel.CourseId} does not exist.");
                }
                chapter.CourseId = updateChapterModel.CourseId;
            }

            chapter.ChapterTitle = updateChapterModel.ChapterTitle ?? chapter.ChapterTitle;
            chapter.SubDescription = updateChapterModel.SubDescription ?? chapter.SubDescription;
            chapter.Description = updateChapterModel.Description ?? chapter.Description;
            chapter.Process = updateChapterModel.Process ?? chapter.Process;
            chapter.Duration = updateChapterModel.Duration ?? chapter.Duration;

            if (await _context.Chapter.AnyAsync(c => c.CourseId == chapter.CourseId &&
                                             c.ChapterTitle == chapter.ChapterTitle &&
                                             c.ChapterId != chapter.ChapterId))
            {
                throw new InvalidOperationException("Chapter title must be unique within the course.");
            }

            await UpdateChapter(chapter);
        }

        public async Task<List<ChapterViewModel>> ViewActiveChaptersAsync(
                string userName,
                string? searchContent = "",
                string? sortBy = "ChapterTitle",
                bool ascending = true,
                int? pageNumber = null,
                int? pageSize = null,
                int? filterDuration = null)
        {

            int actualPageNumber = pageNumber ?? 1;
            int actualPageSize = pageSize ?? 10;

            var chaptersQuery = _context.Chapter
                .Include(x => x.Course)
                .Where(c => c.Course.Username == userName)
                .FilterByStatus("Active")
                .SearchByTitle(searchContent)
                .FilterByDuration(filterDuration);

            var sortColumn = sortBy?.ToLower() ?? "chaptertitle";

            chaptersQuery = sortColumn switch
            {
                "chaptertitle" => ascending ? chaptersQuery.OrderBy(c => c.ChapterTitle) : chaptersQuery.OrderByDescending(c => c.ChapterTitle),
                "coursetitle" => ascending ? chaptersQuery.OrderBy(c => c.Course.CourseTitle) : chaptersQuery.OrderByDescending(c => c.Course.CourseTitle),
                _ => chaptersQuery
            };

            chaptersQuery = chaptersQuery.ApplyPaging(actualPageNumber, actualPageSize);

            var chapters = await chaptersQuery
                .Select(c => new ChapterViewModel
                {
                    CourseId = c.CourseId,
                    ChapterTitle = c.ChapterTitle,
                    SubDescription = c.SubDescription,
                    Description = c.Description,
                    Process = c.Process,
                    Duration = c.Duration,
                    Status = c.Status,
                    Course = c.Course
                })
                .ToListAsync();

            return chapters;
        }

        private bool ContainsSpecialCharacters(string? input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return input.Any(ch => "!@$%^&*()_+={}[]:;\"'<>,.?/".Contains(ch));
        }
    }
}
