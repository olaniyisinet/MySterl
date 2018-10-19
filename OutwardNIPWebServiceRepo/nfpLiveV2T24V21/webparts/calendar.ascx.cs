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

public partial class webparts_calendar : System.Web.UI.UserControl
{
    private string day;
    private string month;
    private string year;

    public string iDay
    {
        get { return day; }
        set { day = value; }
    }
    public string iMonth
    {
        get { return month; }
        set { month = value; }
    }
    public string iYear
    {
        get { return year; }
        set { year = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int i = 1;
            while (i <= 31)
            {
                ListItem l1 = new ListItem(i.ToString("00"), i.ToString());
                calDay.Items.Add(l1);
                i++;
            }


            string[] month = { "All", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

            //DO MONTHS
            i = 1;
            while (i <= 12)
            {
                ListItem m1 = new ListItem(month[i], i.ToString());
                calMonth.Items.Add(m1);
                i++;
            }


            //do year;
            i = 2010;
            //int h = i + 30;
            int j = DateTime.Now.Year;

            //while (h > i)
            //{
            //    ListItem ei = new ListItem(h.ToString(), h.ToString());
            //    calYear.Items.Add(ei);
            //    h--;
            //}

            while (i <= j)
            {
                ListItem y1 = new ListItem(i.ToString(), i.ToString());
                calYear.Items.Add(y1);
                i++;
            }

            try
            {
                if (iDay == "")
                {
                    calDay.SelectedValue = DateTime.Now.Day.ToString();
                }
                else
                {
                    calDay.SelectedValue = iDay;
                }

                if (iMonth == "")
                {
                    calMonth.SelectedValue = DateTime.Now.Month.ToString();
                }
                else
                {
                    calMonth.SelectedValue = iMonth;
                }

                if (iYear == "")
                {
                    calYear.SelectedValue = DateTime.Now.Year.ToString();
                }
                else
                {
                    calYear.SelectedValue = iYear;
                }

            }
            catch (Exception ex)
            {
                //calDay.SelectedValue = DateTime.Now.Day.ToString();
                //calMonth.SelectedValue = month[DateTime.Now.Month];
                //calYear.SelectedValue = DateTime.Now.Year.ToString();
            }
        }

        iDay = calDay.SelectedValue;
        iMonth = calMonth.SelectedValue;
        iYear = calYear.SelectedValue;
    }

    public DateTime getDate()
    {
        try
        {
            int d = Convert.ToInt32(iDay);
            int m = Convert.ToInt32(iMonth);
            int y = Convert.ToInt32(iYear);
            return new DateTime(y, m, d);
        }
        catch (Exception ex)
        {
            return new DateTime(1900, 1, 1);
        }
    }
}
