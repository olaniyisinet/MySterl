using MMWS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Mfino
/// </summary>
public class MfinoEngine : BaseEngine
{
    public string DoActivation(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            resp = prms;
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, "0000", req.SessionID);
            q.ActivationCode = prm["ACTVCODE"];
            q.NewPIN = prm["NEWPIN"];
            q.ConfirmPIN = prm["CONFIRMPIN"];
            resp = Account.Activation(q);
            setStatusflag(2, req);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string DoRegistration(UReq req)
    {
        return "-3";

    }

    public string DoSendMoneyWB(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.Confirmed = prm["CONFIRM"];
            q.SourcePocketCode = "1";
            q.DestPocketCode = "2";
            q.DestMobile = req.Msisdn;
            resp = Transfer.Confirm(q);
            setStatusflag(2, req);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string DoSendMoneyBW(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.Confirmed = prm["CONFIRM"];
            q.SourcePocketCode = "2";
            q.DestPocketCode = "1";
            q.DestMobile = req.Msisdn;
            resp = Transfer.Confirm(q);
            setStatusflag(2, req);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string DoSendOtherMoneyPhoneReg(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.Confirmed = prm["CONFIRM"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestPocketCode = prm["TOACCT"];
            q.DestMobile = prm["TOPHONE"];
            resp = Transfer.Confirm(q);
            setStatusflag(2, req);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
    
    public string DoSendOtherMoneyPhoneNoReg(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.Confirmed = prm["CONFIRM"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestPocketCode = prm["TOACCT"];
            q.DestMobile = prm["TOPHONE"];
            resp = Transfer.Confirm(q);
            setStatusflag(2, req);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string DoSendOtherMoneyBank(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.Confirmed = prm["CONFIRM"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestPocketCode = prm["TOACCT"];
            q.DestBankAccount = prm["TONUBAN"];
            q.DestBankCode = prm["TOBANK"];
            q.DestMobile = req.Msisdn;
            resp = Transfer.ConfirmInterBank(q);
        }
        catch
        {
        }
        return resp;
    }

    public string DoAirtimePurchaseToSelf(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Confirmed = prm["CONFIRM"];
            q.Amount = prm["AMOUNT"];
            q.CompanyID = prm["TOMNO"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestMobile = req.Msisdn;
            q.Amount = prm["AMOUNT"];
            resp = Buy.AirtimePurchaseConfirm(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
    
    public string DoAirtimePurchaseToOthers(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Confirmed = prm["CONFIRM"];
            q.Amount = prm["AMOUNT"];
            q.CompanyID = prm["TOMNO"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestMobile = prm["TOPHONE"];
            q.Amount = prm["AMOUNT"];
            resp = Buy.AirtimePurchaseConfirm(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string DoAcctCheckBalance(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            resp = prms;
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.SourcePocketCode = prm["FROMACCT"];
            resp = Account.CheckBalance(q);
            setStatusflag(2, req);
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
        }
        return resp;
    }

    public string DoChangePIN(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            resp = prms;
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.NewPIN = prm["NEWPIN"];
            q.ConfirmPIN = prm["CONFIRMPIN"];
            resp = Account.ChangePIN(q);
            setStatusflag(2, req);
        }
        catch
        {
        }
        return resp;
    }

    public string DoLast3Trnx(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            resp = prms;
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.SourcePocketCode = prm["FROMACCT"];
            resp = Account.History(q);
            setStatusflag(2, req);
        }
        catch
        {
        }
        return resp;
    }
    
    public string DoCashOutViaAgent(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            resp = prms;
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.AgentCode = prm["FROMAGENT"];
            q.ConfirmPIN = prm["CONFIRMPIN"];
            resp = Transfer.CashOut(q);
            setStatusflag(2, req);
        }
        catch
        {
        }
        return resp;
    }

    public string PaintConfirmSelfWB(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.SourcePocketCode = "1";
            q.DestPocketCode = "2";
            q.DestMobile = req.Msisdn;
            resp = Transfer.Inquiry(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
    
    public string PaintConfirmSelfBW(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.SourcePocketCode = "2";
            q.DestPocketCode = "1";
            q.DestMobile = req.Msisdn;
            resp = Transfer.Inquiry(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string PaintSendToMobileRegInquiry(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestPocketCode = prm["TOACCT"];
            q.DestMobile = prm["TOPHONE"];
            resp = Transfer.Inquiry(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
    
    public string PaintSendToMobileNoRegInquiry(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestPocketCode = prm["TOACCT"];
            q.DestMobile = prm["TOPHONE"];
            q.SubFirstName = prm["FNAME"];
            q.SubLastName = prm["FNAME"];
            resp = Transfer.InquiryUnReg(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
       
    public string PaintSendToBankInquiry(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestPocketCode = prm["TOACCT"];
            q.DestBankAccount = prm["TONUBAN"];
            q.DestBankCode = prm["TOBANK"];
            resp = Transfer.InquiryInterBank(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }

    public string PaintMNOPurchaseConfirmSelf(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.CompanyID = prm["TOMNO"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestMobile = req.Msisdn;
            resp = Buy.AirtimePurchaseInquiry(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
    
    public string PaintMNOPurchaseConfirmOthers(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.CompanyID = prm["TOMNO"];
            q.SourcePocketCode = prm["FROMACCT"];
            q.DestMobile = prm["TOPHONE"];
            resp = Buy.AirtimePurchaseInquiry(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
    
    public string PaintCashOutViaAgentInquiry(UReq req)
    {
        string resp;
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Request q = new Request(req.Msisdn, prm["PIN"], req.SessionID);
            q.Amount = prm["AMOUNT"];
            q.AgentCode = prm["FROMAGENT"];
            q.SourcePocketCode = prm["FROMACCT"];
            resp = Transfer.CashOutInquiry(q);
        }
        catch (Exception ex)
        {
            resp = ex.Message;
        }
        return resp;
    }
}