using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Generic;
public partial class sync_getamtwords : System.Web.UI.Page
{
    String firstbactch;
    String Secondbatch;
    String val;
    String val2;
    String val3;

    String nodollarsign;
    String dollarsign;
    decimal amt;
    string formatedval;
    public string RemoveSpecialChars(string str)
    {
        string[] chars = new string[] { ",", " " };
        for (int i = 0; i < chars.Length; i++)
        {
            if (str.Contains(chars[i]))
            {
                str = str.Replace(chars[i], "");
            }
        }
        return str;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["roles"] != null)
        {
            string allowed = "Cso";
            string roles = Convert.ToString(Session["roles"]);
            if (roles.Contains(allowed))
            {
                //proceed
                string collectval = Convert.ToString(Request.Params["amt"]);
                string towords;
                amt = Convert.ToDecimal(collectval);//collect the actually amount figure for formating
                towords = RemoveSpecialChars(collectval);
                if (towords.Contains("."))
                {
                    string[] arrWrd = towords.Split('.');
                    for (int i = 0; i < arrWrd.Length; i++)
                    {
                        firstbactch = arrWrd[0];
                        Secondbatch = arrWrd[1];
                        if (Secondbatch == "00")//check if the secondbatch =00 if yes. process only first batch else process both
                        {
                            curVert f1 = new curVert(firstbactch);
                            string output1 = f1.outText;
                            if (output1 != "")
                                val = output1 + " NAIRA ONLY";
                        }
                        else
                        {
                            curVert f1 = new curVert(firstbactch);
                            curVert f2 = new curVert(Secondbatch);
                            string output1 = f1.outText;
                            string output2 = f2.outText;
                            if (output1 != "" && output2 != "")
                                val = output1 + " NAIRA " + output2 + " KOBO ONLY";
                        }
                    }
                    formatedval = string.Format("{0:c}", amt);
                    if (formatedval.Contains("$"))
                    {
                        string[] arrDollarsplit = formatedval.Split('$');
                        dollarsign = arrDollarsplit[0];//empty separator (Remove dollar sign)
                        nodollarsign = arrDollarsplit[1];//collect value witth no dollar sign
                    }

                    Response.Write(val.ToString() + "__" + nodollarsign); 

                }
                else
                {
                    curVert c = new curVert(towords);
                    string txt = c.outText;
                    if (txt == "Bad Entry")
                    {
                        val2 = "You need to enter only numeric values to proceed";
                        Response.Write(val2.ToString());
                    }
                    else
                    {
                        formatedval = string.Format("{0:c}", amt);
                        if (formatedval.Contains("$"))
                        {
                            string[] arrDollarsplit = formatedval.Split('$');
                            dollarsign = arrDollarsplit[0];//empty separator (Remove dollar sign)
                            nodollarsign = arrDollarsplit[1];//collect value witth no dollar sign
                        }
                        val3 = txt + " NAIRA ONLY";
                        Response.Write(val3.ToString() + "__" + nodollarsign);
                    }
                }
            }
            else
            {
                Response.Redirect("../login.aspx");
            }

        }
        else
        {
            Response.Redirect("../login.aspx");
        }
        
    }
    class curVert
    {
        public char[] input;
        public int grp_cnt;
        public string outText;
        public curVert(string inp)
        {
            try
            {
                input = inp.Trim().ToCharArray();
                loadGRParray();
            }
            catch (Exception ex)
            {
                loadGRParray(false);
            }
        }
        public void loadGRParray(bool b)
        {
            outText = "Bad Entry";
        }
        public void loadGRParray()
        {
            int num_cnt = input.Length - 1;
            int offset = input.Length % 3;
            if (offset == 0) offset = 3;
            grp_cnt = (int)(num_cnt / 3);
            grp_cnt++;
            int cnt1 = 1;
            int cnt2 = 0;
            string[] grp = new string[grp_cnt];

            for (int i = 0; i < input.Length; i++)
            {
                if (i < offset)
                {
                    grp[0] += input[i].ToString();
                }
                else
                {
                    if (cnt2 == 3)
                    {
                        cnt1++;
                        cnt2 = 0;
                    }
                    grp[cnt1] += input[i].ToString();
                    cnt2++;
                }
            }
            string final = doGroup(grp);
            outText = final.Trim();
        }
        public string doGroup(string[] grp)
        {
            string[] mids = { "", " THOUSAND ", " MILLION ", " BILLION", " TRILLION ", " QUADRILLION " };
            string final = "";
            int cnt = grp.Length - 1;
            int n = Convert.ToInt32(grp[grp.Length - 1]);
            for (int i = 0; i < grp.Length; i++)
            {
                if (n > 0 && n < 21 && cnt == 0 && grp.Length > 1)
                {
                    final += " AND ";
                }
                final += doHundred(grp[i]);
                if (grp[i] == "000" || grp[i] == "00" || grp[i] == "0")
                {

                }
                else
                {
                    final += mids[cnt];
                }
                cnt--;
            }
            return final;
        }
        public string doHundred(string s)
        {
            string txt = "";
            string u0 = "";
            string u1 = "";
            string u2 = "";
            int num = Convert.ToInt32(s);

            if (num < 20)
            {
                txt += getBelow20(s);
            }
            else if (num > 19 && num < 100)
            {
                string s2 = Convert.ToString(num);
                u0 = s2.Substring(1, 1); //UNITS
                u1 = s2.Substring(0, 1); //TENS
                switch (u1)
                {
                    case "2": txt += " TWENTY " + getBelow20(u0); break;
                    case "3": txt += " THIRTY " + getBelow20(u0); break;
                    case "4": txt += " FORTY " + getBelow20(u0); break;
                    case "5": txt += " FIFTY " + getBelow20(u0); break;
                    case "6": txt += " SIXTY " + getBelow20(u0); break;
                    case "7": txt += " SEVENTY " + getBelow20(u0); break;
                    case "8": txt += " EIGHTY " + getBelow20(u0); break;
                    case "9": txt += " NINETY " + getBelow20(u0); break;
                }
            }
            else if (num > 99 && num < 1000)
            {
                u2 = s.Substring(0, 1); //HUNDREDS
                u1 = s.Substring(1, 1); //TENS
                u0 = s.Substring(2, 1); //UNITS

                txt += getBelow20(u2);
                txt += " HUNDRED";
                if (u0 == "0" && u1 == "0")
                {
                }
                else
                {
                    switch (u1)
                    {
                        case "0": txt += " AND " + getBelow20(u0); break;
                        case "1": txt += " AND " + getBelow20(u1 + u0); break;
                        case "2": txt += " AND TWENTY " + getBelow20(u0); break;
                        case "3": txt += " AND THIRTY " + getBelow20(u0); break;
                        case "4": txt += " AND FORTY " + getBelow20(u0); break;
                        case "5": txt += " AND FIFTY " + getBelow20(u0); break;
                        case "6": txt += " AND SIXTY " + getBelow20(u0); break;
                        case "7": txt += " AND SEVENTY " + getBelow20(u0); break;
                        case "8": txt += " AND EIGHTY " + getBelow20(u0); break;
                        case "9": txt += " AND NINETY " + getBelow20(u0); break;
                    }
                }
            }

            return txt;
        }
        public string getBelow20(string s)
        {
            string txt = "";
            int num = Convert.ToInt32(s);
            if (num < 20)
            {
                switch (num)
                {
                    case 1: txt = "ONE"; break;
                    case 2: txt = "TWO"; break;
                    case 3: txt = "THREE"; break;
                    case 4: txt = "FOUR"; break;
                    case 5: txt = "FIVE"; break;
                    case 6: txt = "SIX"; break;
                    case 7: txt = "SEVEN"; break;
                    case 8: txt = "EIGHT"; break;
                    case 9: txt = "NINE"; break;
                    case 10: txt = "TEN"; break;
                    case 11: txt = "ELEVEN"; break;
                    case 12: txt = "TWELVE"; break;
                    case 13: txt = "THIRTEEN"; break;
                    case 14: txt = "FOURTEEN"; break;
                    case 15: txt = "FIFTEEN"; break;
                    case 16: txt = "SIXTEEN"; break;
                    case 17: txt = "SEVENTEEN"; break;
                    case 18: txt = "EIGHTEEN"; break;
                    case 19: txt = "NINETEEN"; break;
                }
            }

            return txt;
        }
    }
}
