using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlentyMoney
{
    class Constants
    {
        public  const int betType_Quick = 1;
        public  const int betType_Self = 2;
        public const string suspense_Wallet = "08101599203";//08036083386(2134), 08037223739(1122)
        public  const int mfino_ChannelID = 5;
        public  const string mfino_InstID = "150";
        //public static string mfino_authKey = "eccab2";
        public static string mfino_Service = "Wallet";
        public static string mfino_transferInquiry = "TransferInquiry";
        public static string mfino_transferComfirm = "Transfer";
        public  const int lotto_Amount = 100;
       
        public const int p_messageId = 11;
        public const int p_channelId = 540;
        public const string p_sourceId = "99EGH540";
        public const string p_gameID = "1";
        public const string p_paymentType = "1";
        public const string p_noOfDraws = "1";
        public const string p_advanceDraws = "0";
        public const string p_drawsOffset = "0";
        public const string p_luckyPickFlag_self = "0";
        public const string p_luckyPickFlag_quick = "1";
        public const string p_betType = "1";
        public const string p_multiplier = "1";
        public const string certPath = @"D:\Applogs\Certificate\gameteclabs.cer";
        public const string urlLive = "https://www.gtlegh.com:8443/msgsubsys-ws/GTConnectService/GTConnectBean";
        public const string urlTest = "http://207.137.7.100:8082/msgsubsys-ws/GTConnectService/GTConnectBean?wsdl";

        public static string pending = "0";
        public static string success = "1";
        public static string failed = "2";
    }
}
