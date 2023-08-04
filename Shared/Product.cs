using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    //Bu class mesaj olarak rabbitmq'ya gidecek
    //Consumer tarafında da okunacak
    public class Product
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public decimal Price { get; set; }
        public int Stock{ get; set; }
    }
}
