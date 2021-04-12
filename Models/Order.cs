using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp
{
    class Order
    {
        #region fields
        private int orderId;
        private Customer orderCustomer;
        private double orderTotal;
        private DateTime orderTime;
        #endregion

        public List<Product> orderItems;
        public List<Product> orderUnavailables;
        public List<Product> orderSubstitutes;

        #region constructors
        public Order()
        {
            this.OrderId = 0;
            this.OrderCustomer = new Customer();
            this.OrderTotal = 0.0;
            this.OrderTime = new DateTime();
        }

        public Order(int oId, Customer oCId, double oTotal, DateTime oTime)
        {
            this.OrderId = oId;
            this.OrderCustomer = oCId;
            this.OrderTotal = oTotal;
            this.OrderTime = oTime;
        }
        #endregion

        #region properties
        public int OrderId { get { return this.orderId; } set { this.orderId = value; } }
        public Customer OrderCustomer { get { return this.orderCustomer; } set { this.orderCustomer = value; } }
        public double OrderTotal { get { return this.orderTotal; } set { this.orderTotal = value; } }
        public DateTime OrderTime { get { return this.orderTime; } set { this.orderTime = value; } }
        #endregion

        #region overrides
        public override string ToString()
        {
            return "Order ID: " + OrderId + "\nCustomer Name: " + OrderCustomer.CustomerName + "\nOrder Total: " + OrderTotal + "\nOrder Time: " + OrderTime;
        }

        public override bool Equals(object obj)
        {
            if(obj is Order)
            {
                Order ord = (Order)obj;
                return (this.OrderId == ord.OrderId);
            }
            return false;
        }
        #endregion
    }
}
