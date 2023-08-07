using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Web.ExcelReport.Models;
using RabbitMq.Web.ExcelReport.Services;
using Shared;

namespace RabbitMq.Web.ExcelReport.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMQPublisher)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                CreatedDate = DateTime.Now,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };

            _appDbContext.UserFiles.Add(userFile);
            _appDbContext.SaveChanges();

            _rabbitMQPublisher.Publish(new CreateExcelMessage()
            {
                FileId = userFile.Id,
               // UserId = user.Id
            });

            //rabbitmq mesaj gönder

            TempData["StartCreatingExcel"] = true;
            //Bir requestten diğerine data taşırken tempdata kullanıllır.
            //Files.cshtml içinde karşılandı


            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return View(await _appDbContext.UserFiles.Where(x => x.UserId == user.Id).ToListAsync());
        }
    }
}
