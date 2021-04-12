using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp
{
    class Customer
    {
        #region fields
        private int customerId;
        private String customerName;
        private String customerPhone;
        private String customerEmail;
        private String customerAddress;

        public List<Order> orders;
        #endregion

        #region constructors
        public Customer()
        {
            this.CustomerId = 0;
            this.CustomerName = "";
            this.CustomerPhone = "";
            this.CustomerEmail = "";
            this.CustomerAddress = "";
        }

        public Customer(int cId, String cName, String cPhone, String cEmail, String cAddress)
        {
            this.CustomerId = cId;
            this.CustomerName = cName;
            this.CustomerPhone = cPhone;
            this.CustomerEmail = cEmail;
            this.CustomerAddress = cAddress;
        }
        #endregion

        #region properties
        public int CustomerId { get { return this.customerId; } set { this.customerId = value; } }
        public String CustomerName { get { return this.customerName; } set { this.customerName = value; } }
        public String CustomerPhone { get { return this.customerPhone; } set { this.customerPhone = value; } }
        public String CustomerEmail { get { return this.customerEmail; } set { this.customerEmail = value; } }
        public String CustomerAddress { get { return this.customerAddress; } set { this.customerAddress = value; } }
        #endregion

        #region overrides
        public override String ToString()
        {
            return "ID: " + CustomerId + "\nName: " + CustomerName + "\nPhone: " + CustomerPhone
                + "\nEmail: " + CustomerEmail + "\nAddress: " + CustomerAddress;
        }
        public override bool Equals(object obj)
        {
            if(obj is Customer)
            {
                Customer cust = (Customer)obj;
                return (this.CustomerId == cust.CustomerId);
            }
            return false;
        }

        #endregion
    }
}
