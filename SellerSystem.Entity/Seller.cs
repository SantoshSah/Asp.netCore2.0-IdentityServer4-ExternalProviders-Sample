using System;
using System.Collections.Generic;
using System.Text;

namespace SellerSystem.Entity
{
    public class Seller
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; }
        public string StoreWebAddress { get; set; }
    }
}
