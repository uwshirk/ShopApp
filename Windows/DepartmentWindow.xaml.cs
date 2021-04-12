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
    /// Interaction logic for DepartmentWindow.xaml
    /// </summary>
    public partial class DepartmentWindow : Window
    {
        private List<Department> departmentList = new List<Department>();
        private List<Department> searchResults = new List<Department>();
        private Department selectedDepartment;

        public DepartmentWindow()
        {
            InitializeComponent();

            FillListBox();
        }

        public void FillListBox()
        {
            listBox_departments.Items.Clear();
            departmentList = DBA.DepartmentDBA.getDepartments();
            foreach (Department d in departmentList)
            {
                listBox_departments.Items.Add(d);
            }

            ClearTextBox();
        }

        public void FillListBoxAfterSearch()
        {
            listBox_departments.Items.Clear();

            foreach (Department d in searchResults)
            {
                listBox_departments.Items.Add(d);
            }
        }

        public void ClearTextBox()
        {
            txt_departmentId.Text = "";
            txt_departmentName.Text = "";
            txt_departmentManager.Text = "";

            lbl_departmentError.Content = "";
        }

        public bool ValidateInput(String function)
        {
            bool isValid = true;
            if (txt_departmentId.Equals("") || txt_departmentName.Equals("") || txt_departmentManager.Equals(""))
            {
                lbl_departmentError.Content = "Please fill out all fields before submitting";
                isValid = false;
            }
            else if (isValid)
            {
                int dId;
                bool isParsable = Int32.TryParse(txt_departmentId.Text, out dId);
                if (!isParsable)
                {
                    lbl_departmentError.Content = "Please enter a number for ID";
                    isValid = false;
                }                

                if (isValid)
                {
                    if (function.Equals("Add"))
                    {
                        foreach (Department d in departmentList)
                        {
                            if (d.DepartmentId == dId)
                            {
                                lbl_departmentError.Content = "Department already exists";
                                isValid = false;
                            }
                        }
                    }
                    else
                    {
                        if (!(selectedDepartment.DepartmentId == dId))
                        {
                            foreach (Department d in departmentList)
                            {
                                if (d.DepartmentId == dId)
                                {
                                    lbl_departmentError.Content = "Department already exists";
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

        private void PopulateListFields(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_departments.SelectedItem != null)
            {
                btn_editDepartment.IsEnabled = true;
                btn_deleteDepartment.IsEnabled = true;

                selectedDepartment = listBox_departments.SelectedItem as Department;

                txt_departmentId.Text = selectedDepartment.DepartmentId.ToString();
                txt_departmentName.Text = selectedDepartment.DepartmentName;
                txt_departmentManager.Text = selectedDepartment.DepartmentManager;
            }
        }

        private void SearchDepartments(object sender, KeyEventArgs e)
        {
            searchResults.Clear();

            if (cmb_departmentSearch.SelectedIndex == -1)
            {
                lbl_searchError.Content = "Please select a search category";
            }
            else
            {
                lbl_searchError.Content = "";

                if (txt_searchDepartment.Text.Equals(""))
                {
                    FillListBox();
                }
                else
                {
                    if (cmb_departmentSearch.SelectedIndex == 0)
                    {
                        foreach (Department d in departmentList)
                        {
                            if (d.DepartmentId.ToString().ToUpper().Contains(txt_searchDepartment.Text.ToUpper()))
                            {
                                searchResults.Add(d);
                            }
                        }
                    }
                    else if (cmb_departmentSearch.SelectedIndex == 1)
                    {
                        foreach (Department d in departmentList)
                        {
                            if (d.DepartmentName.ToString().ToUpper().Contains(txt_searchDepartment.Text.ToUpper()))
                            {
                                searchResults.Add(d);
                            }
                        }
                    }
                    else if (cmb_departmentSearch.SelectedIndex == 2)
                    {
                        foreach (Department d in departmentList)
                        {
                            if (d.DepartmentManager.ToString().ToUpper().Contains(txt_searchDepartment.Text.ToUpper()))
                            {
                                searchResults.Add(d);
                            }
                        }
                    }

                    FillListBoxAfterSearch();
                }
            }
        }

        private void Btn_addDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput("Add"))
            {
                Department addedDepartment = new Department(Int32.Parse(txt_departmentId.Text), txt_departmentName.Text, txt_departmentManager.Text);

                DBA.DepartmentDBA.addDepartment(addedDepartment);

                FillListBox();
            }
        }

        private void Btn_editDepartment_Click(object sender, RoutedEventArgs e)
        {            
            if (ValidateInput("Edit"))
            {
                selectedDepartment = listBox_departments.SelectedItem as Department;
                foreach (Department d in departmentList)
                {
                    if (d.DepartmentId == selectedDepartment.DepartmentId)
                    {
                        Department edittedDepartment = new Department(Int32.Parse(txt_departmentId.Text), txt_departmentName.Text, txt_departmentManager.Text);
                        DBA.DepartmentDBA.editDepartment(edittedDepartment, selectedDepartment.DepartmentId);
                    }
                }

                FillListBox();
            }
        }

        private void Btn_deleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            selectedDepartment = listBox_departments.SelectedItem as Department;
            DBA.DepartmentDBA.deleteDepartment(selectedDepartment.DepartmentId);

            FillListBox();
        }


    }
}
