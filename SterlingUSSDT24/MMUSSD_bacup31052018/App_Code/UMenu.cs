using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class UMenu
{     
    public static void GetMenu(ref UReq r)
    {
        string basecode = "*822";
        //*822#
        //*822*d(3+)#
        //*822*76#
        //*822*5#
        //*822*3*d(11+)#
        //*822*4*d(11+)#
        //*822*6*d(10)#
        //*822*d(3+)*d(11)#
        char[] sep = { '*', '#' };
        string[] bits = r.Msg.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        switch(bits.Length)
        {
            case 2:
                if (bits[0] == "822" && bits[1] == "6")
                {
                    GetMenu3(bits, basecode, ref r);
                }
                else if (bits[0] == "822" && bits[1] == "21")
                {
                    GetMenu3(bits, basecode, ref r);
                }
                else if (bits[0] == "822" && bits[1] == "41")
                {
                    GetMenu3(bits, basecode, ref r);
                }
                else if (bits[0] == "822" && bits[1] == "42")
                {
                    GetMenu3(bits, basecode, ref r);
                }
                else
                {
                    GetMenu2(bits, basecode, ref r);
                }
                break;
            case 3:
                GetMenu3(bits, basecode, ref r);
                break;
            case 4:
                GetMenu4(bits, basecode, ref r);
                break;
            default:
                GetMenu1(bits, basecode, ref r);
                break;
        } 
    }

    private static void GetMenu1(string[] bits, string basecode, ref UReq r)
    {
        //only base code
        r.sub_op = 1;
    }

    private static void GetMenu2(string[] bits, string basecode, ref UReq r)
    {
        //*822*5#
        //*822*76#
        //*822*d(3+)#
        switch(bits[1])
        {
            case "0":
                r.sub_op = 16;
                break;
            case "2":
                r.sub_op = 10;
                break;
            case "3":
                r.sub_op = 11;
                break;
            case "7":
                r.sub_op = 16;
                break;
            case "5":
                r.sub_op = 5;
                break;
            case "8":
                r.sub_op = 5;
                break;
            case "30":
                r.sub_op = 30;
                break;
            case "40":
                r.sub_op = 40;
                break;
            case "76":
                r.sub_op = 76;
                break;
            case "19":
                r.sub_op = 19;
                break;
            case "15":
                r.sub_op = 15;
                break;
            case "18":
                r.sub_op = 18;
                break;
            case "20":
                r.sub_op = 20;
                break;
            default:
                r.sub_op = 10;
                break;
        }
    }

    private static void GetMenu3(string[] bits, string basecode, ref UReq r)
    {
        //*822*1*0001596789# go recharge reg
        //*822*3*d(11+)#
        //*822*4*d(11+)#
        //*822*6*d(10)#
        //*822*8*d(11+)#
        //*822*d(3+)*d(11)#
        switch (bits[1])
        {
            case "1":
                r.sub_op = 12;
                break;
            case "3":
                r.sub_op = 3;
                break;
            case "4":
                r.sub_op = 4;
                break;
            case "6":
                r.sub_op = 6;
                break;
            case "7":
                r.sub_op = 7;
                break;
            case "8":
                r.sub_op = 8;
                break;
            case "9":
                r.sub_op = 9;
                break;
            case "13":
                r.sub_op = 13;
                break;
            case "21":
                r.sub_op = 21;
                break;
            case "41":
                r.sub_op = 41;
                break;
            case "42":
                r.sub_op = 42;
                break;
            case "20":
                r.sub_op = 20;
                break;
            default:
                r.sub_op = 11;
                break;
        }
    }
    private static void GetMenu4(string[] bits, string basecode, ref UReq r)
    {
        //*822*1*0001596789# go recharge reg
        //*822*3*d(11+)#
        //*822*4*d(11+)#
        //*822*6*d(10)#
        //*822*8*d(11+)#
        //*822*d(3+)*d(11)#
        switch (bits[1])
        {
            case "4":
                r.sub_op = 9;
                break;
            case "5":
                r.sub_op = 14;
                break;
            case "9":
                r.sub_op = 9;
                break;
            case "14":
                r.sub_op = 14;
                break;
            case "22":
                r.sub_op = 22;
                break;
            case "28":
                r.sub_op = 28;
                break;
            default:
                r.sub_op = 11;
                break;
        }
    }
}