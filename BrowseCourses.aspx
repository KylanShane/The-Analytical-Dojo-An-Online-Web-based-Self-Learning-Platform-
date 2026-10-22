<%@ Page Title="Browse courses | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BrowseCourses.aspx.cs" Inherits="The_Analytical_Dojo.BrowseCourses" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">

    <div class="eyebrow">CATALOGUE</div>
    <h1>Browse courses<span class="yellow">.</span></h1>
    <p class="muted">Enrol to open slides and quizzes. Enrolled courses appear under My courses.</p>

    <asp:Label ID="Message" runat="server" CssClass="notice" Visible="false" />

    <asp:Literal ID="Catalogue" runat="server" />

</asp:Content>
