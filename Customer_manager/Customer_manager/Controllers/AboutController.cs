using Customer_manager.Models;
using Customer_manager.Repositories;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Customer_manager.Controllers
{
	public class AboutController:Controller
	{
		private readonly UserManager<Customers> _userManager;
		private readonly GenericRepository<About> _repository;
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public AboutController(IWebHostEnvironment webHostEnvironment, GenericRepository<About> repository, UserManager<Customers> userManager, ApplicationDbContext context)
		{
			_repository = repository;
			_userManager = userManager;
			_context = context;
			_webHostEnvironment = webHostEnvironment;

		}

		public ActionResult Index()
		{
			// İçine Hizmetler listesini ekleyin
			var customerInfo = _repository.GetAll();

			return View(customerInfo);
		}

        // GET: About/Create
        public IActionResult Create(int id)
        {

            var about = _repository.GetById(id);
            return View(about);
           
        }
        // POST: About/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(About about)
        {
            if (about.Photo != null && about.Photo.Length > 0)
            {
                // Dosya adını oluşturun
                var fileName = Path.GetFileName(about.Photo.FileName);

                // Dosya yolunu oluşturun
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                // Hedef dizini oluşturun
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Dosyayı kaydedin
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await about.Photo.CopyToAsync(stream);
                }

                // Dosya yolunu veritabanına kaydedin
                about.PhotoPath = "/images/" + fileName;
                ModelState.Clear();
                TryValidateModel(about);
            }

            if (ModelState.IsValid)
            {
                int CustomerId= GetLoggedInCustomerInfoId();
                about.CustomerInfoId = CustomerId;
                // Veritabanında mevcut kaydı güncelleyin
                _repository.Insert(about);
                return RedirectToAction("Index", "Admin");
            }
            return View(about);
        }

        [HttpGet]
		public ActionResult Edit(int id)
		{

			var entity = _repository.GetById(id);
			return View(entity);
		}

        [HttpPost]
        public async Task<ActionResult> Edit(About entity)
        {
            if (ModelState.IsValid)
            {
                if (entity.Photo != null && entity.Photo.Length > 0)
                {
                    // Dosya adını oluşturun
                    var fileName = Path.GetFileName(entity.Photo.FileName);

                    // Dosya yolunu oluşturun
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // Hedef dizini oluşturun
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Dosyayı kaydedin
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await entity.Photo.CopyToAsync(stream);
                    }

                    // Dosya yolunu veritabanına kaydedin
                    entity.PhotoPath = "/images/" + fileName;
                }

                // Veritabanında mevcut kaydı güncelleyin
                _repository.Update(entity);
                return RedirectToAction("Index", "Admin");
            }
            return View(entity);
        }
        private int GetLoggedInCustomerInfoId()
        {

            string loggedInUserName = User.Identity.Name;

            var customerInfo = _context.CustomerInfos
                .FirstOrDefault(ci => ci.Customer.CustomerKey == loggedInUserName);

            return customerInfo?.CustomerInfoId ?? 0;

        }
    }
}
