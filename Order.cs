using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierApp
{
    public class Order
    {
        public string OrderId { get; set; }
        public string Weight { get; set; }
        public string Area { get; set; }
        public DateTime DeliveryTime { get; set; }
    }
}
