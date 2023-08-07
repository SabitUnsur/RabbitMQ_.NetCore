using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Web.ExcelReport.Models;

namespace RabbitMq.Web.ExcelReport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //WorkerService dışarıya veri açmadığından oluşturulan dosyayı buraya kaydedecek.
    //Eğer biz AWS vs. kullanıyor olsasydık buraya gerek yoktu direkt oraya worker serviceden kayıt atabilirdik
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public FilesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]

        //userId zaten ilk etapta excel oluşturulurken kaydedildiği için burada alınmadı
        public async Task<IActionResult> Upload (IFormFile file, int fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();

            var userFile = await _appDbContext.UserFiles.FirstAsync(x => x.Id == fileId);

            // GetExtension() ile sadece uzantıyı aldık
            //dosya adının ve uzantısının alınması

            var filePath =  userFile.FileName +  Path.GetExtension(file.FileName); 

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath); //kaydedilecek yeri belirleme

            using FileStream stream = new(path, FileMode.Create); //Belirtilen yola dosyanın oluşturulma isteği
             
            await file.CopyToAsync(stream); //Oluşturulan dosyanın içeriğini aktardık

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            await _appDbContext.SaveChangesAsync();

            //SignalR notifications

            return Ok(userFile);
        }
    }
}
