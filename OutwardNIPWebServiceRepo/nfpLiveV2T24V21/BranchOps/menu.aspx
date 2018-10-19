<%@ Page Language="C#" MasterPageFile="~/BranchOps/all.master" AutoEventWireup="true" CodeFile="menu.aspx.cs" Inherits="BranchOps_menu" Title="Branch Operations | Menu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width="700" border="1" class="nibbstable">
    <tr>
        <td class="nibbstabletd" colspan="2">
            <h2>
                Menu</h2>
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2">
            <ul>
            <div class='fxnBtn'>
                <p>
                   <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/BranchOps/tssSetup.aspx">Manage Tss Account</asp:HyperLink></p>
                      </div>
                 <div class='fxnBtn'>  <li>
                       <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/BranchOps/createNewuser.aspx">Create New User</asp:HyperLink></li></div>
                       <li><div class='fxnBtn'>
                           <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/BranchOps/assisgnroles.aspx">Assign Roles</asp:HyperLink></div></li>
                       
                       <li>
                        <div class='fxnBtn'>
                            <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/BranchOps/explcode.aspx">Manage Explanation Code</asp:HyperLink>
                        </div>
                        
                        <div class='fxnBtn'>
                            <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/BranchOps/managefee.aspx">Manage Fees</asp:HyperLink>
                        </div>
       
                       </li>
                       </ul>
        </td>
    </tr>
          </table> 
</asp:Content>

