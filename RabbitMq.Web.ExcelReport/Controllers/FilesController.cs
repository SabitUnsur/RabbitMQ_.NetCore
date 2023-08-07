using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Web.ExcelReport.Hubs;
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
        private readonly IHubContext<MyHub> _hubContext;

        public FilesController(AppDbContext appDbContext, IHubContext<MyHub> hubContext)
        {
            _appDbContext = appDbContext;
            _hubContext = hubContext;
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
            await _hubContext.Clients.User(userFile.UserId).SendAsync("CompletedFile");
            //Layoutta dinleriz, çünkü kullanıcı nerede gezinirse gezinsin bilgi gitmesi gerek
            //aksi taktirde başka bir sayfada iken bildirimi alamaz
            //Layoutta cdn olarak ekledik

            return Ok(userFile);
        }
    }
}
