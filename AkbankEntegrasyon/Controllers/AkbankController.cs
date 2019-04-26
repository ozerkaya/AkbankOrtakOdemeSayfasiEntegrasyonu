using AkbankEntegrasyon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AkbankEntegrasyon.Controllers
{
    public class AkbankController : Controller
    {
        //www.ozerkaya.info
        //Kart Numarası(Visa) : 4355084355084358
        //Kart Numarası(Master Card) : 5571135571135575
        //Son Kullanma Tarihi : 12/26
        //Güvenlik Numarası : 000
        //Kart 3D Secure Şifresi : a

        public ActionResult OdemeYap()
        {
            AkbankOdemeModel model = new AkbankOdemeModel
            {
                clientid = "100300000", //Banka tarafından onay sonrası verilecek bir ID değeri
                amount = "100", // Ödeme yapılacak tutar
                oid = Guid.NewGuid().ToString("N"), //Order ID değeri tekil bir numara olmalıdır.
                okUrl = "http://localhost:60127/Akbank/OdemeBasarili", // İşlem başarılı olursa bankanın uygulamanızı çağıracağı adres.
                failUrl = "http://localhost:60127/Akbank/OdemeBasarisiz", // İşlem başarısız olursa bankanın uygulamanızı çağıracağı adres.
                rnd = Guid.NewGuid().ToString("N"), //Random Number değeri tekil bir numara olmalıdır.
                currency = "949", // Ödeme yapılacak para birimi kodu 949 Türk lirası
                instalmentCount = "3", // Taksit sayısı
                transactionType = "Auth", // Auth işlem tipi satışa denk gelir.
                storekey = "123456", //Banka tarafından onay sonrası verilecek bir storek key değeri
                storetype = "3D_Pay_Hosting", // Sizin belirlediğiniz bankanın ödeme sayfasını kullanılan öde tipi
                refreshtime = "5", // Bankanın ortak ödeme sayfasında işlem sonucu görülürken geçecek saniye
                gate = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate", // Bankanın 3D Gate adresi.
            };

            string hashstr = model.clientid + model.oid + model.amount + model.okUrl + model.failUrl + model.transactionType + model.instalmentCount + model.rnd + model.storekey;
            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashstr);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            model.hash = Convert.ToBase64String(inputbytes);

            return View(model);
        }

        public ActionResult OdemeBasarili()
        {
            bool hashControl = false;
            string storekey = "123456";
            string mdstatus = Request.Form.Get("mdStatus");
            string hashparams = Request.Form.Get("HASHPARAMS");
            string hashparamsval = Request.Form.Get("HASHPARAMSVAL");

            string paramsval = "";
            int index1 = 0, index2 = 0;

            if (hashparams != null)
            {
                do
                {
                    index2 = hashparams.IndexOf(":", index1);
                    string val = Request.Form.Get(hashparams.Substring(index1, index2 - index1)) == null ? "" : Request.Form.Get(hashparams.Substring(index1, index2 - index1));
                    paramsval += val;
                    index1 = index2 + 1;
                }
                while (index1 < hashparams.Length);

                string hashval = paramsval + storekey;
                string hashparam = Request.Form.Get("HASH");

                System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashval);
                byte[] inputbytes = sha.ComputeHash(hashbytes);

                string hash = Convert.ToBase64String(inputbytes);

                if (!paramsval.Equals(hashparamsval) || !hash.Equals(hashparam))
                {
                    hashControl = false;
                }
            }
            else
            {
                hashControl = false;
            }

            hashControl = true;


            if (hashControl)
            {
                ViewBag.Mesaj = "Satış Onaylandı...";
            }
            else
            {
                ViewBag.Mesaj = "Hask Kontrolü Başarısız. Güvenlik Nedeni ile İşleme Onay Verilmedi!";
            }
            return View();
        }

        public ActionResult OdemeBasarisiz()
        {
            string strMDStatusText = Request.Form.Get("errmsg");
            ViewBag.Mesaj = "Banka İşleme Onay Vermedi. Banka Mesajı:" + strMDStatusText;
            return View();
        }
    }
}