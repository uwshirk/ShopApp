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
    public partial class LocationWindow : Window
    {
        private List<Location> locationList = new List<Location>();
        private List<Location> searchResults = new List<Location>();
        private Location selectedLocation;

        public LocationWindow()
        {
            InitializeComponent();

            FillListBox();
        }

        public void FillListBox()
        {
            listBox_locations.Items.Clear();
            locationList = DBA.LocationDBA.getLocations();
            foreach (Location l in locationList)
            {
                listBox_locations.Items.Add(l);
            }

            ClearTextBox();
        }

        public void FillListBoxAfterSearch()
        {
            listBox_locations.Items.Clear();

            foreach (Location l in searchResults)
            {
                listBox_locations.Items.Add(l);
            }
        }

        public void ClearTextBox()
        {
            txt_aisleLoc.Text = "";
            txt_sectionLoc.Text = "";
            txt_spotLoc.Text = "";
            txt_locDept.Text = "";

            lbl_locationError.Content = "";
        }

        public bool ValidateInput(String function)
        {
            bool isValid = true;
            if (txt_aisleLoc.Equals("") || txt_sectionLoc.Equals("") || txt_spotLoc.Equals(""))
            {
                lbl_locationError.Content = "Please fill out all fields before submitting";
                isValid = false;
            }
            else if(!txt_locDept.Equals(""))
            {
                int dId;
                bool isParsable = Int32.TryParse(txt_locDept.Text, out dId);
                if (!isParsable)
                {
                    lbl_locationError.Content = "Please enter a number for Location";
                    isValid = false;
                }
                else if (dId > 0)
                {
                    Department lDepartment = DBA.DepartmentDBA.getDepartmentById(dId);
                    if (lDepartment.DepartmentId == 0)
                    {
                        lbl_locationError.Content = "Department does not exist";
                        isValid = false;
                    }
                }
            }
            if(isValid)
            {   
                foreach (Location l in locationList)
                {
                    if(l.AisleLoc.Equals(txt_aisleLoc.Text) && l.SectionLoc.Equals(txt_sectionLoc.Text) && l.SpotLoc.Equals(txt_spotLoc.Text))
                    {
                        if(function.Equals("Add"))
                        {
                            lbl_locationError.Content = "Location already exists";
                            isValid = false;
                        }
                        else
                        {
                            if(l.LocationId != selectedLocation.LocationId)
                            {
                                lbl_locationError.Content = "Location already exists";
                                isValid = false;
                            }
                        }
                    }
                }
                //Location addEditLocation = new Location(txt_aisleLoc.Text, txt_sectionLoc.Text, txt_spotLoc.Text);
                //foreach (Location l in locationList)
                //{
                //    if (l.ToString().Equals(addEditLocation.ToString()))
                //    {
                //        lbl_locationError.Content = "Location already exists";
                //        isValid = false;
                //    }
                //}
            }

            return isValid;
        }

        private void Btn_shopApp_Click(object sender, RoutedEventArgs e)
        {
            ShopAppWindow mainWindow = new ShopAppWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Btn_addLocation_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput("Add"))
            {
                Location addedLocation;
                if (txt_locDept.Text.Equals(""))
                {
                    addedLocation = new Location(txt_aisleLoc.Text, txt_sectionLoc.Text, txt_spotLoc.Text);
                }
                else
                {
                    Department lDepartment = DBA.DepartmentDBA.getDepartmentById(Int32.Parse(txt_locDept.Text));
                    addedLocation = new Location(txt_aisleLoc.Text, txt_sectionLoc.Text, txt_spotLoc.Text, lDepartment);
                }

                DBA.LocationDBA.addLocation(addedLocation);

                FillListBox();
            }
        }

        private void Btn_editLocation_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput("Edit"))
            {
                selectedLocation = listBox_locations.SelectedItem as Location;
                foreach (Location l in locationList)
                {
                    if (l.LocationId == selectedLocation.LocationId)
                    {
                        Location edittedLocation;
                        if(txt_locDept.Text.Equals(""))
                        {
                            edittedLocation = new Location(selectedLocation.LocationId, txt_aisleLoc.Text, txt_sectionLoc.Text, txt_spotLoc.Text);
                            
                        }    
                        else
                        {
                            Department lDepartment = DBA.DepartmentDBA.getDepartmentById(Int32.Parse(txt_locDept.Text));
                            edittedLocation = new Location(selectedLocation.LocationId, txt_aisleLoc.Text, txt_sectionLoc.Text, txt_spotLoc.Text, lDepartment);
                        }
                        DBA.LocationDBA.editLocation(edittedLocation, selectedLocation.LocationId);
                    }
                }

                FillListBox();
            }
        }

        private void Btn_deleteLocation_Click(object sender, RoutedEventArgs e)
        {
            selectedLocation = listBox_locations.SelectedItem as Location;
            DBA.LocationDBA.deleteLocation(selectedLocation.LocationId);

            FillListBox();
        }

        private void PopulateListFields(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_locations.SelectedItem != null)
            {
                btn_editLocation.IsEnabled = true;
                btn_deleteLocation.IsEnabled = true;

                selectedLocation = listBox_locations.SelectedItem as Location;

                txt_aisleLoc.Text = selectedLocation.AisleLoc;
                txt_sectionLoc.Text = selectedLocation.SectionLoc;
                txt_spotLoc.Text = selectedLocation.SpotLoc;
                txt_locDept.Text = selectedLocation.LocationDepartment.DepartmentId.ToString();
            }
        }

        private void SearchLocations(object sender, KeyEventArgs e)
        {
            searchResults.Clear();

            if (cmb_locationSearch.SelectedIndex == -1)
            {
                lbl_searchError.Content = "Please select a search category";
            }
            else
            {
                lbl_searchError.Content = "";

                if (txt_searchLocation.Text.Equals(""))
                {
                    FillListBox();
                }
                else
                {
                    if (cmb_locationSearch.SelectedIndex == 0)
                    {
                        foreach (Location l in locationList)
                        {
                            if (l.AisleLoc.ToUpper().Contains(txt_searchLocation.Text.ToUpper()))
                            {
                                searchResults.Add(l);
                            }
                        }
                    }
                    else if (cmb_locationSearch.SelectedIndex == 1)
                    {
                        foreach (Location l in locationList)
                        {
                            if (l.SectionLoc.ToUpper().Contains(txt_searchLocation.Text.ToUpper()))
                            {
                                searchResults.Add(l);
                            }
                        }
                    }
                    else if (cmb_locationSearch.SelectedIndex == 2)
                    {
                        foreach (Location l in locationList)
                        {
                            if (l.SpotLoc.ToUpper().Contains(txt_searchLocation.Text.ToUpper()))
                            {
                                searchResults.Add(l);
                            }
                        }
                    }
                    
                    FillListBoxAfterSearch();
                }
            }
        }
    }
}
