using System;
using System.Collections.Generic;
using System.Text;
using Tj.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
namespace NIPHandleRequeryIMAL
{
    class Gadget
    {
        public string getStatus(string input)
        {
            string str = "<span";
            switch (input)
            {
                case "0": str += " style='color:#777;'>Pending for Name Request"; break;
                case "1": str += " style='color:#f90;'>Awaiting NIBSS Name Response"; break;
                case "2": str += " style='color:#30f;'>Ready for HOP Authorization"; break;
                case "3": str += " style='color:#c69;'>Authorized but awaiting NIBSS Response"; break;
                case "31": str += " style='color:#c69;'>Authorized but Not Sent to NIBSS"; break;
                case "32": str += " style='color:#c69;'>Authorized but Rejected by NIBSS"; break;
                case "4": str += " style='color:#0c0;'>Successful Transaction"; break;
                case "99": str += " style='color:#c69;'>Transaction Rejected"; break;
            }
            return str + "</span>";
        }

        public string getApproval(string input)
        {
            string str = "<span";
            switch (input)
            {
                case "0": str += " style='color:#777;'>PENDING"; break;
                case "1": str += " style='color:green;'>APPROVED"; break;
                case "2": str += " style='color:red;'>REJECTED"; break;
            }
            return str + "</span>";
        }
        public string getStatus2(string input)
        {
            string str = "<span";
            switch (input)
            {
                case "0": str += " style='color:orange;'>Pending"; break;
                case "1": str += " style='color:orange;'>Transaction Received and currently been processed by NIBSS.  You "; break;
                case "2": str += " style='color:green;'>You can proceed to approve transaction"; break;
                case "3": str += " style='color:orange;'>Approved transactions has been received by NIBSS and is currently been processed"; break;
                case "4": str += " style='color:orange;'>Transaction procssing was successful"; break;
            }
            return str + "</span>";
        }
        //to encrpt
        public string enkrypt(string strEnk)
        {
            SymCryptography cryptic = new SymCryptography();
            cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";

            return cryptic.Encrypt(strEnk);

        }
        //to decrypt
        public string dekrypt(string strDek)
        {
            SymCryptography cryptic = new SymCryptography();
            cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";
            return cryptic.Decrypt(strDek);
        }

        public string GenerateRndNumber(int cnt)
        {
            string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Random rand1 = new Random();
            string txt = "";
            for (int j = 0; j < cnt; j++)
                txt += key2[rand1.Next(0, 9)];
            return txt;
        }

        public string newPaymentRef(int refid)
        {
            long k = refid + 1000000000;
            return k.ToString();
        }

        public string newTrnxRef(string bra_code)
        {
            return bra_code + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public string newSessionId(string bankcode)
        {
            Thread.Sleep(50);
            return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12);
        }
        public string newRecordId(string bankcode)
        {
            Thread.Sleep(50);
            return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12);
        }

        public string newBatchNum(string bankcode)
        {
            Thread.Sleep(50);
            return "232" + bankcode + DateTime.Now.ToString("yyMMdd") + GenerateRndNumber(12);
        }

        public string TRUmoneyToISOmoney(decimal amt)
        {
            amt = amt * 100;
            return amt.ToString("000000000000");
        }

        public decimal ISOmoneyToTRUmoney(string amt)
        {
            decimal t = Convert.ToDecimal(amt);
            return t / 100;
        }


        public DateTime checkDate(string year, string month, string day)
        {
            //date checker
            DateTime datechecker = new DateTime();
            int y = 0;
            int m = 0;
            int d = 0;
            y = Convert.ToInt32(year);
            m = Convert.ToInt32(month);
            d = Convert.ToInt32(day);
            try
            {
                datechecker = new DateTime(y, m, d);
            }
            catch (Exception ex)
            {
                int j = DateTime.Now.Year - 90;
                datechecker = new DateTime(j, 1, 1);
            }
            return datechecker;
        }
        public DateTime checkDate(string dob)
        {
            //date checker
            DateTime datechecker = new DateTime();
            try
            {
                char[] sep = { '-' };
                string[] dt = dob.Split(sep);
                int y = Convert.ToInt32(dt[0]);
                int m = Convert.ToInt32(dt[1]);
                int d = Convert.ToInt32(dt[2]);
                datechecker = new DateTime(y, m, d);
            }
            catch (Exception ex)
            {
                int j = DateTime.Now.Year - 90;
                datechecker = new DateTime(j, 1, 1);
            }
            return datechecker;
        }

        public bool checkEmail(string emailAddress)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                  + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                  + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);
            bool isStrictMatch = reStrict.IsMatch(emailAddress);
            return isStrictMatch;

        }

        public bool validatenum(string number)
        {
            bool ichar = true;
            for (int i = 0; i < number.Length; i++)
            {
                if (!char.IsNumber(number[i]))
                {
                    ichar = false;
                }
            }
            return ichar;
        }
        public string printMoney(decimal amt)
        {
            return amt.ToString("#,###,###,###,##0.00");
        }
        public string printDate(DateTime dt)
        {
            string dtm = dt.ToString("MMM d, yyyy h:mm tt");
            if (dtm == "Jan 1, 0001 12:00 AM" || dtm == "Jan 1, 1900 12:00 AM")
            {
                dtm = "not set";
            }
            dtm = dtm.Replace(" 12:00 AM", "");
            return dtm;
        }
        public decimal makeMoney(string amt)
        {
            decimal mny;
            try
            {
                mny = Convert.ToDecimal(amt);
            }
            catch (Exception ex)
            {
                mny = (decimal)0;
            }
            return mny;
        }

        public bool checkFileType(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            string allowed = ".pdf.jpeg.jpg";
            if (allowed.Contains(ext))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetBankNames(string code)
        {
            string txt = "Unknown Code";
            switch (code)
            {
                case "069": txt = "Intercontinental"; break;
                case "214": txt = "FCMB"; break;
                case "058": txt = "GTB"; break;
                case "221": txt = "Stanbic-IBTC"; break;
                case "057": txt = "Zenith"; break;
                case "082": txt = "BankPHB"; break;
                case "011": txt = "FirstBank"; break;
                case "033": txt = "UBA"; break;
                case "044": txt = "Access Bank"; break;
                case "232": txt = "Sterling Bank"; break;
                case "068": txt = "Standard Chartered"; break;
                case "063": txt = "Diamond"; break;
                case "040": txt = "ETB"; break;
                case "056": txt = "Oceanic"; break;
                case "050": txt = "Ecobank"; break;
                case "076": txt = "Skye"; break;
                case "070": txt = "Fidelity"; break;
                case "023": txt = "Citi"; break;
                case "084": txt = "Spring"; break;
                case "032": txt = "Union Bank"; break;
                case "014": txt = "Afribank"; break;
                case "085": txt = "Finbank"; break;
                case "035": txt = "Wema"; break;
                case "215": txt = "Unity"; break;
                case "039": txt = "NIBSS"; break;
                case "311": txt = "Parkway (Ready Cash)- Union Bank"; break;
                case "314": txt = "Fets (My Wallet)- Skye Bank"; break;
                case "302": txt = "Eartholeum (Qik Qik) - Skye Bank"; break;
                case "305": txt = "Paycom  (Paycom) - Sterling Bank"; break;
            }
            return txt;
        }
        public string responseCodes(string code)
        {
            string txt = "Unknown Code";
            code = code.Trim();
            switch (code)
            {
                case "00": txt = "Approved or completed successfully"; break;
                case "03": txt = "Invalid sender"; break;
                case "05": txt = "Do not honor"; break;
                case "06": txt = "Dormant account"; break;
                case "07": txt = "Invalid account"; break;
                case "08": txt = "Account name mismatch"; break;
                case "09": txt = "Request processing in progress"; break;
                case "12": txt = "Invalid transaction"; break;
                case "13": txt = "Invalid amount"; break;
                case "14": txt = "Invalid Batch Number"; break;
                case "15": txt = "Invalid Session or Record ID"; break;
                case "16": txt = "Unknown Bank Code"; break;
                case "17": txt = "Invalid Channel"; break;
                case "18": txt = "Wrong Method Call"; break;
                case "21": txt = "No action taken"; break;
                case "25": txt = "Unable to locate record"; break;
                case "26": txt = "Duplicate record"; break;
                case "30": txt = "Wrong destination account format"; break;
                case "34": txt = "Suspected fraud"; break;
                case "35": txt = "Contact sending bank"; break;
                case "51": txt = "No sufficient funds"; break;
                case "57": txt = "Transaction not permitted to sender"; break;
                case "58": txt = "Transaction not permitted on channel"; break;
                case "61": txt = "Transfer Limit Exceeded"; break;
                case "63": txt = "Security violation"; break;
                case "65": txt = "Exceeds withdrawal frequency"; break;
                case "68": txt = "Response received too late"; break;
                case "91": txt = "Beneficiary Bank not available"; break;
                case "92": txt = "Routing Error"; break;
                case "94": txt = "Duplicate Transaction"; break;
                case "96": txt = "Corresponding Bank is currently offline."; break;
                case "97": txt = "Timeout waiting for response from destination"; break;
            }
            return txt;
        }
        public string RemoveSpecialChars(string str)
        {
            string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "-", "_", "(", ")", ":", "|", "[", "]" };
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], " ");
                }
            }
            return str;
        }
        public string getMime(string input)
        {
            switch (input)
            {
                case ".jpg": input = "image/jpeg"; break;
                case ".jpeg": input = "image/jpeg"; break;
                case ".pdf": input = "application/pdf"; break;
                case ".gif": input = "image/gif"; break;
                case ".png": input = "image/png"; break;
                case ".tif": input = "image/tiff"; break;
                case ".tiff": input = "image/tiff"; break;
                case ".zip": input = "application/zip"; break;
                case ".zipx": input = "application/zip"; break;
            }
            return input;
        }
        public string GenerateRnd()
        {
            string[] key1 = { "b", "c", "d", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "t", "v", "w", "x", "y", "z" };
            string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Random rand1 = new Random();
            string txt = "";
            //for (int i = 0; i < 7; i++)
            //    txt += key1[rand1.Next(0, 18)];
            for (int j = 0; j < 4; j++)  //the < 4 will return just for digits
                txt += key2[rand1.Next(0, 9)];
            return txt;
        }
    }
}
