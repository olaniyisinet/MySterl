<%@ Page Language="C#" MasterPageFile="~/BranchOps/all.master" AutoEventWireup="true" CodeFile="assisgnroles.aspx.cs" Inherits="BranchOps_assisgnroles" Title="Branch Operations | Assign" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="Frm1">
<table border="1" class="nibbstable" style="width: 697px">
    <tr>
        <td class="nibbstabletd" colspan="3">
            <wpt:infoBar ID="InfoBar1" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="3">
            <h2>
                Assign Roles</h2> 
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2">
            <strong>Select Username to assign role</strong></td>
        <td class="nibbstabletd" colspan="1">
            <asp:DropDownList ID="ddlusername" runat="server" DataSourceID="SqlDataSource1" DataTextField="username"
                DataValueField="username">
            </asp:DropDownList><asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:nfp %>"
                SelectCommand="spd_DisplayUsers" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" >
            <strong>Assign Role</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:CheckBoxList ID="chklist" runat="server" DataSourceID="SqlDataSource2" DataTextField="descr"
                DataValueField="usertypeid">
            </asp:CheckBoxList><asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:nfp %>"
                SelectCommand="spd_DisplayRoles" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="3">
        <asp:Button ID="btn_submit" runat="server" Text="Submit" OnClick="btn_submit_Click" /></td>
    </tr>
          </table> 
          </div>
</asp:Content>

