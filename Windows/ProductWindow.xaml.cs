using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShopApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private List<Product> productList = new List<Product>();
        private List<Product> searchResults = new List<Product>();
        private Product selectedProduct;
        String[] locationList;
        private Department pDepartment;
        private Location pLocation;

        public ProductWindow()
        {
            InitializeComponent();

            FillListBox();
        }

        public void FillListBox()
        {
            listBox_products.Items.Clear();
            productList = DBA.ProductDBA.getProducts();
            foreach (Product p in productList)
            {                
                listBox_products.Items.Add(p);
            }

            ClearTextBox();
        }

        public void FillListBoxAfterSearch()
        {
            listBox_products.Items.Clear();

            foreach (Product p in searchResults)
            {
                listBox_products.Items.Add(p);
            }
        }

        public void ClearTextBox()
        {
            txt_productId.Text = "";
            txt_productDepartment.Text = "";
            txt_productName.Text = "";
            txt_productDesc.Text = "";
            txt_productCost.Text = "";
            txt_productStock.Text = "";
            txt_productLocation.Text = "";

            lbl_productError.Content = "";
        }       

        public bool ValidateInput(String function)
        {
            bool isValid = true;
            if (txt_productId.Text.Equals("") || txt_productName.Text.Equals(""))
            {
                lbl_productError.Content = "Please fill out the product ID and Name";
                isValid = false;
            }
            else if (isValid)
            {
                int pId;
                bool isParsable = Int32.TryParse(txt_productId.Text, out pId);
                if (!isParsable)
                {
                    lbl_productError.Content = "Please enter a number for ID";
                    isValid = false;
                }

                if (!txt_productDepartment.Text.Equals(""))
                {
                    int pDept;
                    isParsable = Int32.TryParse(txt_productDepartment.Text, out pDept);
                    if (!isParsable)
                    {
                        lbl_productError.Content = "Please enter a number for Department";
                        isValid = false;
                    }
                    else if (pDept > 0)
                    {
                        pDepartment = DBA.DepartmentDBA.getDepartmentById(pDept);
                        if (pDepartment.DepartmentId == 0)
                        {
                            lbl_productError.Content = "Department does not exist";
                            isValid = false;
                        }
                    }
                }                

                if (!txt_productCost.Text.Equals(""))
                {
                    double pCost;
                    isParsable = Double.TryParse(txt_productCost.Text, out pCost);
                    if (!isParsable)
                    {
                        lbl_productError.Content = "Please enter a number for Cost";
                        isValid = false;
                    }
                }     
                
                if (!txt_productStock.Text.Equals(""))
                {
                    int pStock;
                    isParsable = Int32.TryParse(txt_productStock.Text, out pStock);
                    if (!isParsable)
                    {
                        lbl_productError.Content = "Please enter a number for Stock";
                        isValid = false;
                    }
                }

                if (!txt_productLocation.Text.Equals(""))
                {
                    locationList = txt_productLocation.Text.Split('-');
                    if(locationList.Length<3)
                    {
                        lbl_productError.Content = "Please enter a full location";
                        isValid = false;
                    }
                    else if(locationList.Length>3)
                    {
                        lbl_productError.Content = "Location is too long";
                        isValid = false;
                    }
                    else if(DBA.LocationDBA.getLocationByLoc(txt_productLocation.Text).LocationId == 1
                        && !txt_productLocation.Text.Equals("Hold-Hold-Hold"))
                    {
                        lbl_productError.Content = "Location does not exist";
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    if (function.Equals("Add"))
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductId == pId)
                            {
                                lbl_productError.Content = "Product already exists";
                                isValid = false;
                            }
                        }
                    }
                    else
                    {
                        if (!(selectedProduct.ProductId == pId))
                        {
                            foreach (Product p in productList)
                            {
                                if (p.ProductId == pId)
                                {
                                    lbl_productError.Content = "Product already exists";
                                    isValid = false;
                                }
                            }
                        }
                    }
                }
            }

            return isValid;
        }

        private void Btn_shopApp_Click(object sender, RoutedEventArgs e)
        {
            ShopAppWindow mainWindow = new ShopAppWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Btn_addProduct_Click(object sender, RoutedEventArgs e)
        {
            if(ValidateInput("Add"))
            {
                Product addedProduct;
                double pCost;
                int pStock;

                if (txt_productCost.Text.Equals(""))
                {
                    pCost = 0.0;
                }
                else
                {
                    pCost = Double.Parse(txt_productCost.Text);
                }

                if (txt_productStock.Text.Equals(""))
                {
                    pStock = 0;
                }
                else
                {
                    pStock = Int32.Parse(txt_productStock.Text);
                }

                if (txt_productLocation.Text.Equals("") && txt_productDepartment.Text.Equals(""))
                {
                    addedProduct = new Product(Int32.Parse(txt_productId.Text), txt_productName.Text, txt_productDesc.Text,
                        pCost, pStock);
                }
                else if(txt_productLocation.Text.Equals(""))
                {
                    pDepartment = DBA.DepartmentDBA.getDepartmentById(Int32.Parse(txt_productDepartment.Text));
                    addedProduct = new Product(Int32.Parse(txt_productId.Text), pDepartment, txt_productName.Text, txt_productDesc.Text,
                        pCost, pStock);
                }
                else if(txt_productDepartment.Text.Equals(""))
                {
                    pLocation = DBA.LocationDBA.getLocationByLoc(txt_productLocation.Text);
                    addedProduct = new Product(Int32.Parse(txt_productId.Text), txt_productName.Text, txt_productDesc.Text,
                        pCost, pStock, pLocation);
                }
                else
                {
                    pLocation = DBA.LocationDBA.getLocationByLoc(txt_productLocation.Text);
                    pDepartment = DBA.DepartmentDBA.getDepartmentById(Int32.Parse(txt_productDepartment.Text));                    

                    addedProduct = new Product(Int32.Parse(txt_productId.Text), pDepartment, txt_productName.Text, txt_productDesc.Text,
                        pCost, pStock, pLocation);
                }

                DBA.ProductDBA.addProduct(addedProduct);

                FillListBox();
            }            
        }

        private void Btn_editProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput("Edit"))
            {
                pLocation = DBA.LocationDBA.getLocationByLoc(txt_productLocation.Text);
                pDepartment = DBA.DepartmentDBA.getDepartmentById(Int32.Parse(txt_productDepartment.Text));
                selectedProduct = listBox_products.SelectedItem as Product;
                foreach(Product p in productList)
                {
                    if(p.ProductId == selectedProduct.ProductId)
                    {
                        Product edittedProduct = new Product(Int32.Parse(txt_productId.Text), pDepartment, txt_productName.Text, txt_productDesc.Text, Double.Parse(txt_productCost.Text), Int32.Parse(txt_productStock.Text), pLocation);
                        DBA.ProductDBA.editProduct(edittedProduct, selectedProduct.ProductId);
                    }
                }

                FillListBox();                
            }
        }

        private void Btn_deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            selectedProduct = listBox_products.SelectedItem as Product;
            DBA.ProductDBA.deleteProduct(selectedProduct.ProductId);

            FillListBox();
        }

        private void PopulateListFields(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_products.SelectedItem != null)
            {
                btn_editProduct.IsEnabled = true;
                btn_deleteProduct.IsEnabled = true;

                selectedProduct = listBox_products.SelectedItem as Product;

                txt_productId.Text = selectedProduct.ProductId.ToString();
                txt_productDepartment.Text = selectedProduct.ProductDept.DepartmentId.ToString();
                txt_productName.Text = selectedProduct.ProductName; 
                txt_productDesc.Text = selectedProduct.ProductDesc; 
                txt_productCost.Text = selectedProduct.ProductCost.ToString(); 
                txt_productStock.Text = selectedProduct.ProductStock.ToString(); 
                txt_productLocation.Text = selectedProduct.ProductLocation.getLocationString();
            }            
        }

        private void SearchProducts(object sender, KeyEventArgs e)
        {
            searchResults.Clear();

            if(cmb_productSearch.SelectedIndex == -1)
            {
                lbl_searchError.Content = "Please select a search category";
            }
            else
            {
                lbl_searchError.Content = "";

                if (txt_searchProduct.Text.Equals(""))
                {
                    FillListBox();
                }
                else
                {
                    if (cmb_productSearch.SelectedIndex == 0)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductId.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    else if (cmb_productSearch.SelectedIndex == 1)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductDept.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    else if (cmb_productSearch.SelectedIndex == 2)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductName.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    else if (cmb_productSearch.SelectedIndex == 3)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductDesc.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    else if (cmb_productSearch.SelectedIndex == 4)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductCost.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    else if (cmb_productSearch.SelectedIndex == 5)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductStock.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    else if (cmb_productSearch.SelectedIndex == 6)
                    {
                        foreach (Product p in productList)
                        {
                            if (p.ProductLocation.ToString().ToUpper().Contains(txt_searchProduct.Text.ToUpper()))
                            {
                                searchResults.Add(p);
                            }
                        }
                    }
                    
                    FillListBoxAfterSearch();
                }
            }
        }
    }
}
