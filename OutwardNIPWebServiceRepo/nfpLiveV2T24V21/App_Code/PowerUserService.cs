using System;
using System.Data;
using System.Configuration;

public class PowerUserService
{
    public bool login(string username, string password)
    {
        Connect c = new Connect("spd_LogonUser");
        c.addparam("username", username);
        c.addparam("password", password);
        DataSet ds = c.query("user");
        if (ds.Tables[0].Rows.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public PowerUser getUser(string username)
    {
        //get user's details from database
        Connect c = new Connect("spd_Getuserdetails");
        c.addparam("username", username);
        DataSet ds = c.query("user");
        PowerUser u = new PowerUser();
        if (ds.Tables[0].Rows.Count > 0)
        {
            u = setUser(ds.Tables[0].Rows[0]);
        }
        else
        {
            u.username = "";
        }

        return u;
    }

    public PowerUser setUser(DataRow dr)
    {
        PowerUser u = new PowerUser();
        u.username = dr["username"].ToString();
        u.firstname = dr["firstname"].ToString();
        u.lastname = dr["lastname"].ToString();
        u.fullname = u.lastname + ", " + u.firstname;
        u.starttime = dr["starttime"].ToString();
        u.endtime = dr["endtime"].ToString();
        u.bracode = dr["bra_code"].ToString();
        u.tellerId = dr["tellerid"].ToString();
        //u.utype = dr["usertype"].ToString();
        u.email = dr["email"].ToString();
        return u;
    }

    public PowerRole[] getUserRoles(string username)
    {
        
        Connect c = new Connect("spd_GetuserRole");
        c.addparam("username", username);
        DataSet ds = c.query("user");
        int cnt = ds.Tables[0].Rows.Count;
        PowerRole[] rs = new PowerRole[cnt];
        for(int i = 0; i < cnt; i++)
        {
            DataRow dr = ds.Tables[0].Rows[i];
            PowerRole r = new PowerRole();
            r.roleId = Convert.ToInt16(dr["Usertypeid"]);
            r.roleName = dr["utype"].ToString();
            r.roleDesc = dr["Descrp"].ToString();
            rs[i] = r;
        }
        return rs;
    }

    public string getBranchEmails(string bracode, string usertype)
    {
        Connect c = new Connect("spd_Getbranchemails");
        c.addparam("branchcode", bracode);
        c.addparam("usertype", usertype);
        DataSet ds = c.query("bra");
        string email = "nil";
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            email =  dr["email"].ToString();
        }
        return email;
    }

    //create user
    public int AddNewUser(PowerUser p)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_createNewUser");
            c.addparam("@bra_code", p.bracode);
            c.addparam("@username", p.username);
            c.addparam("@email", p.email);
            c.addparam("@firstname", p.username);
            c.addparam("@lastname", p.lastname);
            c.addparam("@updatedby", p.updatedby);
            c.addparam("@lastupdated", p.lastupdated);
            c.addparam("@starttime", p.starttime);
            c.addparam("@endtime", p.endtime);
            c.addparam("@Tellerid", p.tellerId);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    //drop user Roles
    public int DropOldRoles(PowerUser p)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_dropoldroles");
            c.addparam("@username", p.username);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    public int AssignRoles(PowerUser p)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_Assignroles");
            c.addparam("@Usertypeid", p.Usertypeid);
            c.addparam("@username", p.username);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
}
