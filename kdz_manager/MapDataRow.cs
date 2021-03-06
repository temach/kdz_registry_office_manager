﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// Карты одна строка в CSV файл со свойствами.
    /// </summary>
    class MapDataRow
    {
        public int ROWNUM { get; set; }
        public string CommonName { get; set; }
        public string FullName { get; set; }
        public string ShortName {get;set;}
        public string AdmAreaCode {get;set;}
        public string AdmArea {get;set;}
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
        public float X_WGS {get;set;}
        public float Y_WGS {get;set;}
        public string GLOBALID {get;set;}

        /// <summary>
        /// Беспараметрическая конструктор, необходимые для использования в CSV парсер.
        /// </summary>
        public  MapDataRow()
        {
            ROWNUM = 0;
            CommonName = string.Empty;
            FullName = string.Empty;
            ShortName = string.Empty;
            AdmAreaCode = string.Empty;
            AdmArea = string.Empty;
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
            X_WGS = 0.0f;
            Y_WGS = 0.0f;
            GLOBALID = string.Empty;
        }
    }
}
