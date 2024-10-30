using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PB102App.Data;
using PB102App.Models;
using PB102App.Services.Interfaces;
using PB102App.ViewModels.Admin.Work;

namespace PB102App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WorkController : Controller
    {
        private readonly IWorkService _workService;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        public WorkController(IWorkService workService , IWebHostEnvironment env, AppDbContext context)
        {
            _workService = workService;
            _env = env;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _workService.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            var work = await _workService.GetByIdAsync((int)id);

            if (work is null) return NotFound();

            return View(work);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await _workService.GetWorkCategoriesAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkCreateVM request)
        {
            if (!ModelState.IsValid) return View(request);

            var work = new Work
            {
                Title = request.Work.Title,
                Description = request.Work.Description,
                CategoryId = request.Work.SelectCategoryId,
                Images = new List<WorkImage>() 
            };

            if (request.Work.UploadedImages != null && request.Work.UploadedImages.Count > 0)
            {
                for (int i = 0; i < request.Work.UploadedImages.Count; i++)
                {
                    var item = request.Work.UploadedImages[i];
                    string fileName = $"{Guid.NewGuid()}_{item.FileName}";
                    string path = Path.Combine(_env.WebRootPath, "assets/img", fileName);

                    using (FileStream stream = new(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    
                    var workImage = new WorkImage
                    {
                        Image = fileName,
                        Work = work,
                        IsMain = (i == 0)
                    };

                    work.Images.Add(workImage);
                }
            }

            await _workService.CreateAsync(work);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var workEditVM = await _workService.GetEditByIdAsync((int)id);

            if (workEditVM == null) return NotFound();

            return View(workEditVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, WorkUpdateVM workEditVM)
        {
            if (id == null) return NotFound();

            var findData = await _context.Works
                .Include(w => w.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (findData == null) return NotFound();

            findData.Title = workEditVM.Work.Title;
            findData.Description = workEditVM.Work.Description;
            findData.CategoryId = workEditVM.Work.SelectCategoryId;

            if (workEditVM.Work.Category != null)
            {
                findData.Category.Name = workEditVM.Work.Category.Name;
            }

            if (workEditVM.Work.UploadedImages != null && workEditVM.Work.UploadedImages.Count > 0)
            {
                foreach (var item in workEditVM.Work.UploadedImages)
                {
                    string fileName = $"{Guid.NewGuid()}_{item.FileName}";
                    string path = Path.Combine(_env.WebRootPath, "assets/img", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    findData.Images.Add(new WorkImage
                    {
                        Image = fileName,
                        IsMain = (findData.Images.Count == 0),
                        WorkId = findData.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var work = await _context.Works.FirstOrDefaultAsync(x => x.Id == id);

            if (work == null) return NotFound();
            _context.Works.Remove(work);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
