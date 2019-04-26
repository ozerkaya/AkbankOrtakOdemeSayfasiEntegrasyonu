using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AkbankEntegrasyon.Models
{
    public class AkbankOdemeModel
    {
        public string clientid { get; set; }
        public string amount { get; set; }
        public string oid { get; set; }
        public string okUrl { get; set; }
        public string failUrl { get; set; }
        public string islemtipi { get; set; }
        public string taksit { get; set; }
        public string rnd { get; set; }
        public string hash { get; set; }
        public string currency { get; set; }
        public string storetype { get; set; }
        public string refreshtime { get; set; }
        public string instalmentCount { get; set; }
        public string transactionType { get; set; }
        public string storekey { get; set; }
        public string gate { get; set; }
    }
}