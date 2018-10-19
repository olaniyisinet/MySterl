using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class swcFormat
{
	public swcFormat()
	{
    }
    public string getUserContact(string input)
    {
        if (input.Trim() == "")
        {
            return "";
        }
        string k = "";
        try
        {
            string[] u = input.Split('@');
            k += "&nbsp;<a href='sip:" + u[0] + "@sterlingbank.com'><img alt='chat' src='../image/c.png' border='0' /></a>";
        }
        catch (Exception ex)
        {

        }
        k += "&nbsp;<a href='mailto:" + input + "'><img alt='email' src='../image/e.png' border='0' /></a>";
        return k;
    }

    public string getStatus(string input)
    {
        string str = "<span";
        switch (input)
        {
            case "0": str += " style='color:orange;'>PENDING"; break;
            case "1": str += " style='color:GREEN;'>APPROVED"; break;
            case "2": str += " style='color:RED;'>REJECTED"; break;
        }
        return str + "</span>";
    }

    public string formatCurrency(string input)
    {
        decimal amt = Convert.ToDecimal(input);
        amt = amt * (decimal)100;
        string nocomma = amt.ToString();
        nocomma = nocomma.Replace(".00","");
        return nocomma;
    }

    public string truncateString(string input, int numchars)
    {
        string newstr = input;
        try
        {
            newstr = input.Substring(0, numchars);
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return newstr;
    }

    public string getCurrency(string input)
    {
        switch (input)
        {
            case "1": return "GBP"; break;
            case "2": return "USD"; break;
            case "3": return "EUR"; break;
        }
        return "";
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

    public string getDateNow()
    {
        return DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
    }

    public string getStamp(string code)
    {   
        string savepath = ConfigurationManager.AppSettings["uploadpath"].ToString();
        System.Drawing.Image objImage = System.Drawing.Image.FromFile(savepath + "stamp.png");//From File
        int height = objImage.Height;//Actual image width
        int width = objImage.Width;//Actual image height
        System.Drawing.Bitmap bitmapimage = new System.Drawing.Bitmap(objImage, width, height);// create bitmap with same size of Actual image
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmapimage);
        SolidBrush brush = new SolidBrush(Color.Gray);
        
        string timp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //g.DrawString("Entered by: " + code, new Font("Arial", 8, System.Drawing.FontStyle.Bold), brush, 5, 30);
        g.DrawString("Entered by: " + code + " ( " + timp + ")", new Font("Arial", 8), brush, 5, 30);
        string pixloc = savepath + DateTime.Now.ToString("yyyyMMddHHmmss") + code + ".png";
        bitmapimage.Save(pixloc); //if u want to save image
        bitmapimage.Dispose();
        objImage.Dispose();
        return pixloc;
    }

    public bool target()
    {
        return false;
    }

    public string getThumbnail(string file)
    {
        // generate the pic
        System.Drawing.Image.GetThumbnailImageAbort callback = new System.Drawing.Image.GetThumbnailImageAbort(target);
        System.Drawing.Image objImage = System.Drawing.Image.FromFile(file);
        int h0 = objImage.Height;//Actual image width
        int w0 = objImage.Width;//Actual image height
        int w1 = 500;
        // do a ratio convertion
        int h1 = Convert.ToInt32((w1 * h0) / w0);
        System.Drawing.Image i1 = objImage.GetThumbnailImage(w1, h1, callback, IntPtr.Zero);
        string savepath = ConfigurationManager.AppSettings["uploadpath"].ToString();
        string pixloc = savepath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
        i1.Save(pixloc);
        i1.Dispose();
        objImage.Dispose();
        return pixloc;
    }

}