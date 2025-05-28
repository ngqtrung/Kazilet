using AutoMapper;
using PRN231_Kazilet_API.Models.Dto;
using PRN231_Kazilet_API.Models.Entities;

namespace PRN231_Kazilet_API.Utils.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            CreateMap<AnswerDto, Answer>();

            CreateMap<Answer, AnswerDto>();

            CreateMap<GameplaySettingDto, GameplaySetting>();

            CreateMap<GameplaySetting, GameplaySettingDto>();

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions))
                .ForMember(dest => dest.numOfQues, opt => opt.MapFrom(src => src.Questions.Count));

            CreateMap<CourseDto, Course>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<LearningHistory, LearningHistoryDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.numOfQues, opt => opt.MapFrom(src => src.Course.Questions.Count));
        }
    }
}
