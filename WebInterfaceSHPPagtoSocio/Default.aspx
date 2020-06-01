<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebInterfaceSHPPagtoSocio._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        <asp:Calendar ID="DataIni" runat="server"></asp:Calendar>
    </p>
    <p>
        &nbsp;</p>
    <p>
        <asp:Calendar ID="DataFin" runat="server"></asp:Calendar>
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    </p>
    <p>
        <asp:Label ID="Label1" runat="server" Width="500px"></asp:Label>
    </p>

</asp:Content>
