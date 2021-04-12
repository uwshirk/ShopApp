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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShopAppWindow : Window
    {
        private bool validUser = true;

        public ShopAppWindow()
        {
            InitializeComponent();

            if (validUser)
            {
                this.btn_product.IsEnabled = true;
                this.btn_department.IsEnabled = true;
                this.btn_location.IsEnabled = true;
                this.btn_order.IsEnabled = true;
                this.btn_shopRoute.IsEnabled = true;

                this.btn_login.Content = "Logout";
            }
            //if statement to determine if user is logged in
            //if yes show log out if no show log in
        }

        private void productView(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            this.Close();
        }

        private void locationView(object sender, RoutedEventArgs e)
        {
            LocationWindow locatinWindow = new LocationWindow();
            locatinWindow.Show();
            this.Close();
        }

        private void departmentView(object sender, RoutedEventArgs e)
        {
            DepartmentWindow departmentWindow = new DepartmentWindow();
            departmentWindow.Show();
            this.Close();
        }

        private void shopRouteView(object sender, RoutedEventArgs e)
        {
            ShopRouteWindow shopRouteWindow = new ShopRouteWindow();
            shopRouteWindow.Show();
            this.Close();
        }

        private void orderView(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow();
            orderWindow.Show();
            this.Close();
        }
    }
}
