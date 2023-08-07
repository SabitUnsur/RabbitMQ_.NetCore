using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CreateExcelMessage
    {
       // public string UserId { get; set; }

       //FilesControllerda bunu zaten bize userId geliyor dolayısıyla almadık
        public int FileId { get; set; }
        
    }
}
