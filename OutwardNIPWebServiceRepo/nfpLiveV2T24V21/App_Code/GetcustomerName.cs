using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for GetcustomerName
/// </summary>
public class GetcustomerName
{
    List<string> s = new List<string>();
    public string Frm_bra_code = ""; public string Frm_cus_num = "";
    public string Frm_cur_code = ""; public string Frm_led_code = "";
    public string Frm_sub_acct_code = ""; public decimal FrmAcctbal = 0;
    public int sta_code = 0; public string email = ""; public int cus_class = 0; public string nubanval = "";
    public string bvn = ""; public int rest_ind = 0; string stacodeval = ""; string cusStatus = "";
    public string Rest_txt; public int Rest_code = 0; public string res = string.Empty;
    public string getTheCustomerName(string frmAcct)
    {
        //get full account
        string fullAct = "";
        string orignatorName = "";

        EACBS.banks b = new EACBS.banks();
        
        DataSet ds = b.getAccountFullInfo(frmAcct);
       //DataSet ds = b.GetAccountfromDByAccountNum(frmAcct);
        try
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                orignatorName = dr["cus_sho_name"].ToString();// HttpUtility.HtmlEncode(dr["cus_sho_name"].ToString());
                Frm_bra_code = HttpUtility.HtmlEncode(dr["T24_BRA_CODE"].ToString());
                Frm_cus_num = HttpUtility.HtmlEncode(dr["T24_CUS_NUM"].ToString());
                Frm_cur_code = HttpUtility.HtmlEncode(dr["T24_CUR_CODE"].ToString());
                if (Frm_cur_code == "566")
                {
                    Frm_cur_code = "NGN";
                }
                Frm_led_code = HttpUtility.HtmlEncode(dr["T24_LED_CODE"].ToString());
                //Frm_sub_acct_code = HttpUtility.HtmlEncode(dr["sub_acct_code"].ToString());
                stacodeval = dr["STA_CODE"].ToString();
                if (stacodeval == "ACTIVE")
                {
                    sta_code = 1;
                }
                else if (stacodeval == "INACTIVE")
                {
                    sta_code = 2;
                }
                FrmAcctbal = decimal.Parse(dr["UsableBal"].ToString());
                email = HttpUtility.HtmlEncode(dr["EMAIL"].ToString());
                cusStatus = dr["CustomerStatus"].ToString();
                //if (cusStatus == "Private Client - Standard")
                //{
                //    cus_class = 1;//individual
                //}
                //else
                //{
                //    cus_class = 2;//corporate
                //}
                //cus_class = int.Parse();
                rest_ind = int.Parse(dr["REST_IND"].ToString());
                bvn = dr["BVN"].ToString();
                nubanval = dr["NUBAN"].ToString();
                Rest_txt = dr["REST_FLAG"].ToString();
                if (Rest_txt == "FALSE")
                {
                    //Rest_code = ds.Tables["RestrictFlag"].Rows[0]["uptoAmount"];
                    if (rest_ind == 0)
                    {
                        Rest_code = -1;
                    }
                    else
                    {
                        Rest_code = rest_ind;
                    }
                }
                else if (Rest_txt == "TRUE")
                {
                    //Rest_code = rest_ind;  
                    foreach (DataRow dr1 in ds.Tables["RestrictFlag"].Rows)
                    {
                        //add the restriction code(s) to the list
                        s.Add(dr1["RESTRCODE"].ToString());
                    }

                    //loop through the list and separate the restrictions with ,

                    foreach (string unique in s)
                    {
                        res += unique + ",";
                    }

                    res = res.TrimEnd(new char[] { ',' });
                    res = "(" + res + ")";
                }
            }
            else
            {
                //call EACBS
                DataSet ds1 = b.getAccountFullInfo(frmAcct);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds1.Tables[0].Rows[0];
                    orignatorName = dr["cus_sho_name"].ToString();// HttpUtility.HtmlEncode(dr["cus_sho_name"].ToString());
                    Frm_bra_code = HttpUtility.HtmlEncode(dr["T24_BRA_CODE"].ToString());
                    Frm_cus_num = HttpUtility.HtmlEncode(dr["T24_CUS_NUM"].ToString());
                    Frm_cur_code = HttpUtility.HtmlEncode(dr["T24_CUR_CODE"].ToString());
                    if (Frm_cur_code == "566")
                    {
                        Frm_cur_code = "NGN";
                    }
                    Frm_led_code = HttpUtility.HtmlEncode(dr["T24_LED_CODE"].ToString());
                    //Frm_sub_acct_code = HttpUtility.HtmlEncode(dr["sub_acct_code"].ToString());
                    stacodeval = dr["STA_CODE"].ToString();
                    if (stacodeval == "ACTIVE")
                    {
                        sta_code = 1;
                    }
                    else if (stacodeval == "INACTIVE")
                    {
                        sta_code = 2;
                    }
                    FrmAcctbal = decimal.Parse(dr["UsableBal"].ToString());
                    email = HttpUtility.HtmlEncode(dr["EMAIL"].ToString());
                    cusStatus = dr["CustomerStatus"].ToString();
                    //if (cusStatus == "Private Client - Standard")
                    //{
                    //    cus_class = 1;//individual
                    //}
                    //else
                    //{
                    //    cus_class = 2;//corporate
                    //}
                    //cus_class = int.Parse();
                    rest_ind = int.Parse(dr["REST_IND"].ToString());
                    bvn = dr["BVN"].ToString();
                    nubanval = dr["NUBAN"].ToString();
                    Rest_txt = dr["REST_FLAG"].ToString();
                    if (Rest_txt == "FALSE")
                    {
                        //Rest_code = ds.Tables["RestrictFlag"].Rows[0]["uptoAmount"];
                        if (rest_ind == 0)
                        {
                            Rest_code = -1;
                        }
                        else
                        {
                            Rest_code = rest_ind;
                        }
                    }
                    else if (Rest_txt == "TRUE")
                    {
                        //Rest_code = rest_ind;  
                        foreach (DataRow dr1 in ds.Tables["RestrictFlag"].Rows)
                        {
                            //add the restriction code(s) to the list
                            s.Add(dr1["RESTRCODE"].ToString());
                        }

                        //loop through the list and separate the restrictions with ,

                        foreach (string unique in s)
                        {
                            res += unique + ",";
                        }

                        res = res.TrimEnd(new char[] { ',' });
                        res = "(" + res + ")";
                    }
                }//close tag for if
            }//close tag for else
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error occured getting customer details for Acct " + frmAcct + " " + ex.ToString());
            Mylogger.Error("Error occured getting customer details for Acct " + frmAcct, ex);
        }
        return orignatorName;
    }
}