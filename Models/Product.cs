using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp
{
    class Product
    {
        #region fields
        //Example values: productId = 10230401; productDept = 1; productName = Kleenex; productDesc = Facial Tissue; productCost = 2.99; productStock = 120; productLocation = A1-2-23;
        private int productId;
        private Department productDept;
        private String productName;
        private String productDesc;
        private double productCost;
        private int productStock;
        private Location productLocation;
        
        //unknown if necessary
        //public List<ShopRoute> shopRoutes;

        #endregion

        #region constructors
        public Product()
        {
            this.ProductId = 0;
            this.ProductDept = new Department();
            this.ProductName = "NULL";
            this.ProductDesc = "NULL";
            this.ProductCost = 0;
            this.ProductStock = 0;
            this.ProductLocation = new Location();
        }

        public Product(int pID, String pName, String pDesc, double pCost, int pStock)
        {
            this.ProductId = pID;
            this.ProductDept = new Department();
            this.ProductName = pName;
            this.ProductDesc = pDesc;
            this.ProductCost = pCost;
            this.ProductStock = pStock;
            this.ProductLocation = new Location();
        }

        public Product(int pID, String pName, String pDesc, double pCost, int pStock, Location pLocation)
        {
            this.ProductId = pID;
            this.ProductDept = new Department();
            this.ProductName = pName;
            this.ProductDesc = pDesc;
            this.ProductCost = pCost;
            this.ProductStock = pStock;
            this.ProductLocation = pLocation;
        }

        public Product(int pID, Department pDept, String pName, String pDesc, double pCost, int pStock)
        {
            this.ProductId = pID;
            this.ProductDept = pDept;
            this.ProductName = pName;
            this.ProductDesc = pDesc;
            this.ProductCost = pCost;
            this.ProductStock = pStock;
            this.ProductLocation = new Location();
        }

        public Product (int pID, Department pDept, String pName, String pDesc, double pCost, int pStock, Location pLocation )
        {
            this.ProductId = pID;
            this.ProductDept = pDept;
            this.ProductName = pName;
            this.ProductDesc = pDesc;
            this.ProductCost = pCost;
            this.ProductStock = pStock;
            this.ProductLocation = pLocation;
        }
        #endregion

        #region properties
        public int ProductId { get { return this.productId; } set { this.productId = value; } }
        public Department ProductDept { get { return this.productDept; } set { this.productDept = value; } }
        public String ProductName { get { return this.productName; } set { this.productName = value; } }
        public String ProductDesc { get { return this.productDesc; } set { this.productDesc = value; } }
        public double ProductCost { get { return this.productCost; } set { this.productCost = value; } }
        public int ProductStock { get { return this.productStock; } set { this.productStock = value; } }
        public Location ProductLocation { get { return this.productLocation; } set { this.productLocation = value; } }
        #endregion

        #region overrides

        public override string ToString()
        {
            return "Product ID: " + ProductId + "\nProduct Department: "
                + ProductDept.DepartmentId + "\nProduct Name: " + ProductName
                + "\nProduct Description: " + ProductDesc + "\nProduct Cost: $"
                + ProductCost + "\nProduct Stock: " + ProductStock
                + "\nProduct Location: " + ProductLocation.getLocationString();
        }

        public override bool Equals(object obj)
        {
           if(obj is Product)
            {
                Product prod = (Product)obj;
                return (this.ProductId == prod.ProductId);
            }
            return false;
        }

        #endregion
    }
}
