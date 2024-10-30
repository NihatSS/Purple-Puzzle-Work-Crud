using PB102App.Models;

namespace PB102App.ViewModels.Admin.Work
{
    using PB102App.Models;
    public class WorkUpdateVM
    {
        public Work Work { get; set; }
        public List<Category> Categories { get; set; }
    }
}
