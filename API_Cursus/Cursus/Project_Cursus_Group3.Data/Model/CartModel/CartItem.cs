using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.CartModel
{
    public class CartItem
    {
        public int CourseId { get; set; }
        public DateTime AddedDate = DateTime.UtcNow.AddHours(7);
    }
}
