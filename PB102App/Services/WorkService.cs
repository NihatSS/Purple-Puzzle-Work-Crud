using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PB102App.Data;
using PB102App.Models;
using PB102App.Services.Interfaces;
using PB102App.ViewModels.Admin.Work;

namespace PB102App.Services
{
    public class WorkService : IWorkService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WorkService(AppDbContext context,
                           IMapper mapper)
        {
            _context = context;    
            _mapper = mapper;
        }

        public async Task CreateAsync(Work work)
        {
            await _context.AddAsync(work);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkVM>> GetAllAsync()
        {
            var works = await _context.Works.Include(m => m.Category)
                                            .Include(m => m.Images)
                                            .OrderByDescending(m=>m.Id)
                                            .ToListAsync();

            return _mapper.Map<IEnumerable<WorkVM>>(works);
        }

        public async Task<SingleWorkVM> GetByIdAsync(int id)
        {
            var work = await _context.Works.Include(m => m.Category)
                                       .Include(m => m.Images)
                                       .FirstOrDefaultAsync(m=>m.Id == id);

            return _mapper.Map<SingleWorkVM>(work);
        }

        public async Task<WorkUpdateVM> GetEditByIdAsync(int id)
        {
            return new WorkUpdateVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Work = await _context.Works.Include(x=>x.Images).Include(x=>x.Category).FirstOrDefaultAsync(m => m.Id == id),
            };
        }

        public async Task<WorkCreateVM> GetWorkCategoriesAsync()
        {
            return new WorkCreateVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Work = await _context.Works.FirstOrDefaultAsync()
            };
        }
    }
}
