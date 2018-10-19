using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.DirectoryServices;

public class ldap
{
    public string username;
    public string fullname;
    public string firstname;
    public string lastname;
    public string email;
    public string mobile;
    public string pager;
    public string sip;
    public string title;
    public string cug;
    public string err;
    public string department;

    public bool login(string username, string password)
    {
        //string path = "LDAP://sterlingbank";
        string path = "LDAP://sterlingbank.com";
        DirectoryEntry entry = new DirectoryEntry(path, @"sterlingbank\" + username, password);
        try
        {
            Object native = entry.NativeObject;
            return true;
        }
        catch (Exception ex)
        {
            err = ex.Message;
            return false;
        } 
    }
    protected string GetProperty(SearchResult searchResult, string PropertyName)
    {
        if (searchResult.Properties.Contains(PropertyName))
        {
            return searchResult.Properties[PropertyName][0].ToString();
        }
        else
        {
            return string.Empty;
        }
    }
    public bool GetUserDetails(string xusername)
    {
        try
        {
            int c = 0;
            string _path = "LDAP://sterlingbank";
            string _filterAttribute = xusername;
            DirectorySearcher dSearch = new DirectorySearcher(_path);
            dSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + _filterAttribute + "))";
            foreach (SearchResult sResultSet in dSearch.FindAll())
            {
                c++;
                username = GetProperty(sResultSet, "sAMAccountName"); //username
                fullname = GetProperty(sResultSet, "cn");	 // full Name
                firstname = GetProperty(sResultSet, "givenName");	// First Name
                lastname = GetProperty(sResultSet, "sn");	 // Last Name
                title = GetProperty(sResultSet, "title"); // Company
                sip = GetProperty(sResultSet, "msRTCSIP-PrimaryUserAddress");	 //communicator
                mobile = GetProperty(sResultSet, "mobile");	//City
                cug = GetProperty(sResultSet, "pager");	//Country
                email = GetProperty(sResultSet, "mail"); //Email
                department = GetProperty(sResultSet, "department"); //department
            }
            if (c > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            err = ex.Message;
            return false;
        }
    }
    //public Staff GetInfo(string xusername)
    //{
    //    Staff s = new Staff();
    //    s.username = "0";
    //    try
    //    {
    //        string _path = "LDAP://sterlingbank";
    //        string _filterAttribute = xusername;
    //        DirectorySearcher dSearch = new DirectorySearcher(_path);
    //        dSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + _filterAttribute + "))";
    //        foreach (SearchResult sResultSet in dSearch.FindAll())
    //        {
    //            s.username = GetProperty(sResultSet, "sAMAccountName"); //username
    //            s.fullName = GetProperty(sResultSet, "cn");	 // full Name
    //            s.firstName = GetProperty(sResultSet, "givenName");	// First Name
    //            s.lastName = GetProperty(sResultSet, "sn");	 // Last Name
    //            s.jobTitle = GetProperty(sResultSet, "title"); // Company
    //            s.communicator = GetProperty(sResultSet, "msRTCSIP-PrimaryUserAddress");	 //communicator
    //            s.mobile = GetProperty(sResultSet, "mobile");	//City
    //            s.email = GetProperty(sResultSet, "mail"); //Email
    //            s.deptName = GetProperty(sResultSet, "department"); //department
    //            s.grade = GetProperty(sResultSet, "extensionAttribute1"); //grade
    //            s.gender = GetProperty(sResultSet, "extensionAttribute3"); //gender
    //            s.mstatus = GetProperty(sResultSet, "extensionAttribute4"); //marital status
    //            s.cardnumber = GetProperty(sResultSet, "employeeID"); //staff num
    //            s.country = GetProperty(sResultSet, "co"); //Email
    //            s.stateName = GetProperty(sResultSet, "st"); //Email
    //            s.unitName = GetProperty(sResultSet, "division"); //Email
    //            s.region = GetProperty(sResultSet, "streetAddress"); //Email
    //            s.officeLocation = GetProperty(sResultSet, "extensionAttribute2"); //Email
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        err = ex.Message;
    //    }

    //    return s;
    //}
    public DataTable searchUsers(string xusername)
    {
        DataTable dt = new DataTable("sr");
        DataColumn col;

        col = dt.Columns.Add("username", Type.GetType("System.String"));
        col = dt.Columns.Add("fullname", Type.GetType("System.String"));
        col = dt.Columns.Add("email", Type.GetType("System.String"));

        DataRow row;
        try
        {
            string _path = "LDAP://sterlingbank";
            string _filterAttribute = xusername;
            DirectorySearcher dSearch = new DirectorySearcher(_path);
            dSearch.Filter = "(&(objectClass=user)(cn=*" + _filterAttribute + "*))";
            foreach (SearchResult sResultSet in dSearch.FindAll())
            {
                row = dt.NewRow();
                row["username"] = GetProperty(sResultSet, "sAMAccountName"); //username
                row["fullname"] = GetProperty(sResultSet, "cn");	 // full Name
                row["email"] = GetProperty(sResultSet, "mail"); //Email
                if (row["email"] != "")
                {
                    dt.Rows.Add(row);
                }
            }
        }
        catch (Exception ex)
        {
            
        }
        DataView v = dt.DefaultView;
        v.Sort = "fullname";
        return v.ToTable();
    }

}
