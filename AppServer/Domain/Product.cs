using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServer.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description {get; set;}
        public int Stock { get; set;}
        public int Price { get; set; }
        public string Image { get; set; }
        public string Owner { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();

    }
}
