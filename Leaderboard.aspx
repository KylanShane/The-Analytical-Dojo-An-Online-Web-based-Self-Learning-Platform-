<%@ Page Title="Leaderboard | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Leaderboard.aspx.cs" Inherits="The_Analytical_Dojo.Leaderboard" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">

    <!-- Title + your standing -->
    <asp:Literal ID="Hero" runat="server" />

    <!-- View switch and filters -->
    <asp:Literal ID="Filters" runat="server" />

    <!-- Podium and ranked list -->
    <asp:Literal ID="Board" runat="server" />

</asp:Content>
