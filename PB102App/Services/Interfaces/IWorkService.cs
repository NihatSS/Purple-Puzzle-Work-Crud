using PB102App.Models;
using PB102App.ViewModels.Admin.Work;

namespace PB102App.Services.Interfaces
{
    public interface IWorkService
    {
        Task<IEnumerable<WorkVM>> GetAllAsync();
        Task<SingleWorkVM> GetByIdAsync(int id);
        Task<WorkCreateVM> GetWorkCategoriesAsync();
        Task<WorkUpdateVM> GetEditByIdAsync(int id); 
        Task CreateAsync(Work work);
    }
}
