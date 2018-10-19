using System;

public class Account
{
    public string bra_code;
    public string cus_num;
    public string cur_code;
    public string led_code;
    public string sub_acct_code;
    public decimal cle_bal;
    public decimal avail_bal;
    public decimal bal_limit;
    public string cus_sho_name;
    public string ref_key;
    public int rtnMssg;
    public string bankname = "Sterling Bank Plc";
    public string bankcode = "232";
    public string mobile;
    public int status;

    public string fullaccount()
    {
        return bra_code + "-" + cus_num + "-" + cur_code + "-" + led_code + "-" + sub_acct_code;
    }

}
