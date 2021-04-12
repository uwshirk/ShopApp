using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp
{
    class Location
    {
        #region fields
        private String aisleLoc;
        private String sectionLoc;
        private String spotLoc;
        private Department locationDepartment;
        private int locationId;
        #endregion

        #region constructors
        public Location()
        {
            this.LocationId = 1;
            this.aisleLoc = "";
            this.sectionLoc = "";
            this.spotLoc = "";
            this.LocationDepartment = new Department();
        }

        public Location(String aLoc, String secLoc, String spLoc)
        {
            this.LocationId = 1;
            this.AisleLoc = aLoc;
            this.SectionLoc = secLoc;
            this.SpotLoc = spLoc;
            this.LocationDepartment = new Department();
        }

        public Location(String aLoc, String secLoc, String spLoc, Department lDepartment)
        {
            this.LocationId = 1;
            this.AisleLoc = aLoc;
            this.SectionLoc = secLoc;
            this.SpotLoc = spLoc;
            this.LocationDepartment = lDepartment;
        }

        public Location(int lId, String aLoc, String secLoc, String spLoc)
        {
            this.LocationId = lId;
            this.AisleLoc = aLoc;
            this.SectionLoc = secLoc;
            this.SpotLoc = spLoc;
            this.LocationDepartment = new Department();
        }

        public Location(int lId, String aLoc, String secLoc, String spLoc, Department lDepartment)
        {
            this.LocationId = lId;
            this.AisleLoc = aLoc;
            this.SectionLoc = secLoc;
            this.SpotLoc = spLoc;
            this.LocationDepartment = lDepartment;
        }
        #endregion

        #region properties
        public String AisleLoc { get { return this.aisleLoc; } set { this.aisleLoc = value; } }
        public String SectionLoc { get { return this.sectionLoc; } set { this.sectionLoc = value; } }
        public String SpotLoc { get { return this.spotLoc; } set { this.spotLoc = value; } }
        public Department LocationDepartment { get { return this.locationDepartment; } set { this.locationDepartment = value; } }
        public int LocationId { get { return this.locationId; } set { this.locationId = value; } }
        #endregion

        public String getLocationString()
        {
            return AisleLoc + "-" + SectionLoc + "-" + SpotLoc;
        }

        #region overrides
        public override string ToString()
        {
            return AisleLoc + "-" + SectionLoc + "-" + SpotLoc + "\nDepartment: " + LocationDepartment.DepartmentId;
        }

        public override bool Equals(object obj)
        {
            if (obj is Location)
            {
                Location loc = (Location)obj;
                return ((this.AisleLoc.Equals(loc.AisleLoc)) && (this.SectionLoc.Equals(loc.SectionLoc)) && (this.SpotLoc.Equals(loc.SpotLoc)));
            }
            return false;
        }
        #endregion
    }
}