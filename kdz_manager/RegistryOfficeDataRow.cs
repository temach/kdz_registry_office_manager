using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// Represents some books.
    /// </summary>
    class RegistryOfficeDataRow : IFromMapDataRow<RegistryOfficeDataRow>
    {
        public string ROWNUM { get; set; }
        public string CommonName { get; set; }
        public string FullName { get; set; }
        public string ShortName {get;set;}
        // public string AdmAreaCode {get;set;}
        // public string AdmArea {get;set;}
        public string District {get;set;}
        public string PostalCode {get;set;}
        public string Address {get;set;}
        public string NearestMetroStations {get;set;}
        public string ChiefName {get;set;}
        public string ChiefPosition {get;set;}
        public string ChiefPhone {get;set;}
        public string ContactPhone {get;set;}
        public string ArchivePhone {get;set;}
        public string SignPGU {get;set;}
        public string WorkingHours {get;set;}
        public string ClarificationOfWorkingHours {get;set;}
        public string WebSite {get;set;}
        public string X_WGS {get;set;}
        public string Y_WGS {get;set;}
        public string GLOBALID {get;set;}

        /// <summary>
        /// Parameterless constructor necessary for use in CSV parser.
        /// </summary>
        public RegistryOfficeDataRow()
        {
            CommonName = string.Empty;
            FullName = string.Empty;
            ShortName = string.Empty;
            // AdmAreaCode = string.Empty;
            // AdmArea = string.Empty;
            District = string.Empty;
            PostalCode = string.Empty;
            Address = string.Empty;
            NearestMetroStations = string.Empty;
            ChiefName = string.Empty;
            ChiefPosition = string.Empty;
            ChiefPhone = string.Empty;
            ContactPhone = string.Empty;
            ArchivePhone = string.Empty;
            SignPGU = string.Empty;
            WorkingHours = string.Empty;
            ClarificationOfWorkingHours = string.Empty;
            WebSite = string.Empty;
            X_WGS = string.Empty;
            Y_WGS = string.Empty;
            GLOBALID = string.Empty;
        }

        /// <summary>
        /// Make from Map data raw
        /// </summary>
        /// <param name="input">Original object to deep copy from.</param>
        public RegistryOfficeDataRow(MapDataRow input)
            : this()
        {
            CommonName = input.CommonName;
            FullName = input.FullName;
            ShortName = input.ShortName;
            // AdmAreaCode = string.Empty;
            // AdmArea = string.Empty;
            District = input.District;
            PostalCode = input.PostalCode;
            Address = input.Address;
            NearestMetroStations = input.NearestMetroStations;
            ChiefName = input.ChiefName;
            ChiefPosition = input.ChiefPosition;
            ChiefPhone = input.ChiefPhone;
            ContactPhone = input.ContactPhone;
            ArchivePhone = input.ArchivePhone;
            SignPGU = input.SignPGU;
            WorkingHours = input.WorkingHours;
            ClarificationOfWorkingHours = input.ClarificationOfWorkingHours;
            WebSite = input.WebSite;
            X_WGS = input.X_WGS;
            Y_WGS = input.Y_WGS;
            GLOBALID = input.GLOBALID;
        }

        /// <summary>
        /// Create new instance from parsed data
        /// </summary>
        /// <returns></returns>
        public RegistryOfficeDataRow FromMapDataRow(MapDataRow input)
        {
            return new RegistryOfficeDataRow(input);
        }

    }
}
