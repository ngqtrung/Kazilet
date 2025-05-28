using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;
using static PRN231_Kazilet_API.Services.Impl.CourseService;

namespace PRN231_Kazilet_API.Services.Impl
{
    public class LearningHistoryService : ILearningHistory
    {
        private readonly PRN231_Kazilet_v2Context _context;

        private readonly IMapper _mapper;

        public LearningHistoryService(PRN231_Kazilet_v2Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool AddLearningHistory(int userId,int courseId)
        {
            var existedOne = _context.LearningHistories.FirstOrDefault(l => l.CourseId == courseId && l.UserId == userId);
            var learningHistory = _mapper.Map<LearningHistory>(existedOne);
            
            if (existedOne != null) {
                learningHistory.LearningDate = DateTime.Now;
                _context.LearningHistories.Update(learningHistory);
            }
            else
            {
                LearningHistory lh = new LearningHistory();
                lh.CourseId = courseId;
                lh.UserId = userId;
                lh.LearningDate = DateTime.Now;
                _context.LearningHistories.Add(lh);
            }
            
            return _context.SaveChanges()>0?true : false;
        }
        public List<CourseDto> GetTop5Course()
        {
            List<CourseCount> lc = new List<CourseCount>();
            foreach (var course in _context.Courses.ToList())
            {
                if (!lc.Contains(lc.FirstOrDefault(lc => lc.CourseId == course.Id)))
                {
                    lc.Add(new CourseCount(course.Id, _context.LearningHistories.Where(lh => lh.CourseId == course.Id).ToList().Count));
                }
            }
            lc = lc.OrderByDescending(lc => lc.Count).Take(5).ToList();
            List<Course> l = new List<Course>();
            foreach (var a in lc)
            {
                l.Add(_context.Courses.Include(c => c.Questions).Include(c => c.CreatedByNavigation).FirstOrDefault(c => c.Id == a.CourseId));
            }
            return _mapper.Map<List<CourseDto>>(l);
        }
        public List<LearningHistoryDto> GetAllLearningHistoriesByUserId(int userId)
        {
            return _mapper.Map<List<LearningHistoryDto>>(_context.LearningHistories.Include(l => l.User).Include(l => l.Course).Include(l=>l.Course.Questions).Where(l => l.UserId == userId).ToList().OrderByDescending(l=>l.LearningDate));
        }
    }
}
