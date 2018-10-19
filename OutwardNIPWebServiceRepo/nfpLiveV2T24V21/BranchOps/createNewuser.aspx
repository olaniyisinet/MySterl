<%@ Page Language="C#" MasterPageFile="~/BranchOps/all.master" AutoEventWireup="true" CodeFile="createNewuser.aspx.cs" Inherits="BranchOps_createNewuser" Title="Branch Operations | Create User" %>
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
                Create Account</h2> 
        </td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2">
            <strong>Bracode</strong></td>
        <td class="nibbstabletd" colspan="1">
            <asp:TextBox ID="txtbracode" runat="server" MaxLength="4"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" >
            <strong>Username</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:TextBox ID="txtusername" runat="server" MaxLength="50"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" >
            <strong>Tellerid</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:TextBox ID="txttellerid" runat="server" MaxLength="6"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2">
            <strong>Email</strong></td>
        <td class="nibbstabletd" colspan="1">
            <asp:TextBox ID="txtemail" runat="server" MaxLength="150"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2">
            <strong>Firstname</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:TextBox ID="txtfirstname" runat="server" MaxLength="100"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" >
            <strong>Lastname</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:TextBox ID="txtlastname" runat="server" MaxLength="100"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" >
            <strong>Start Time</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:DropDownList ID="ddlstarttime" runat="server" CssClass="formtext">
                <asp:ListItem Selected="True" Value="0">Select Start Time</asp:ListItem>
                <asp:ListItem Value="8">8AM</asp:ListItem>
                <asp:ListItem Value="9">9AM</asp:ListItem>
                <asp:ListItem Value="10">10AM</asp:ListItem>
                <asp:ListItem Value="11">11AM</asp:ListItem>
                <asp:ListItem Value="12">12PM</asp:ListItem>
                <asp:ListItem Value="13">1PM</asp:ListItem>
                <asp:ListItem Value="14">2PM</asp:ListItem>
                <asp:ListItem Value="15">3PM</asp:ListItem>
                <asp:ListItem Value="16">4PM</asp:ListItem>
                <asp:ListItem Value="17">5PM</asp:ListItem>
                <asp:ListItem Value="18">6PM</asp:ListItem>
                <asp:ListItem Value="19">7PM</asp:ListItem>
                <asp:ListItem Value="20">8PM</asp:ListItem>
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="2" >
            <strong>End TIme</strong></td>
        <td class="nibbstabletd" colspan="1" >
            <asp:DropDownList ID="ddlendtime" runat="server" CssClass="formtext">
                <asp:ListItem Selected="True" Value="0">End Time</asp:ListItem>
                <asp:ListItem Value="8">8AM</asp:ListItem>
                <asp:ListItem Value="9">9AM</asp:ListItem>
                <asp:ListItem Value="10">10AM</asp:ListItem>
                <asp:ListItem Value="11">11AM</asp:ListItem>
                <asp:ListItem Value="12">12PM</asp:ListItem>
                <asp:ListItem Value="13">1PM</asp:ListItem>
                <asp:ListItem Value="14">2PM</asp:ListItem>
                <asp:ListItem Value="15">3PM</asp:ListItem>
                <asp:ListItem Value="16">4PM</asp:ListItem>
                <asp:ListItem Value="17">5PM</asp:ListItem>
                <asp:ListItem Value="18">6PM</asp:ListItem>
                <asp:ListItem Value="19">7PM</asp:ListItem>
                <asp:ListItem Value="20">8PM</asp:ListItem>
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="nibbstabletd" colspan="3">
        <asp:Button ID="btn_submit" runat="server" Text="Submit" OnClick="btn_submit_Click" /></td>
    </tr>
          </table> 
          </div>
</asp:Content>

