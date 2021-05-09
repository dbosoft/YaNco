<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SAPWebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <p>Error Message: <%=Session["ErrorMessage"]%></p>
    
    <% if(Session["Companies"] != null)
          { %>
    <p>Companies:</p>
            <table class="table">
            <thead>
            <tr>
                <th>Code</th>
                <th>Name</th>
            </tr>
            </thead>
            <tbody>
                <% foreach (var (code, name) in (Session["Companies"] as IEnumerable<(string code, string name)>))
                   { %>

                <tr>
                    <td><%=code %></td>
                    <td><%=name %></td>
                </tr>
            <% } %>
            </tbody>
        </table>
        <% } %>
</asp:Content>
