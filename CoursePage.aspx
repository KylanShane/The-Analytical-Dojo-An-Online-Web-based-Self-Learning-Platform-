<%@ Page Title="Course | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="CoursePage.aspx.cs" Inherits="The_Analytical_Dojo.CoursePage" %>

<asp:Content
    ID="PageContent"
    ContentPlaceHolderID="Main"
    runat="server">

    <a class="text-link" href="MyCourses.aspx">
        ← My courses
    </a>

    <!-- Course title, description and progress -->
    <asp:Literal
        ID="Intro"
        runat="server" />

    <!-- Feedback message -->
    <asp:Label
        ID="Message"
        runat="server"
        CssClass="notice"
        Visible="false" />

    <asp:Literal ID="Community" runat="server" />

    <!-- Chapters, learning materials and quizzes -->
    <asp:Literal
        ID="Chapters"
        runat="server" />

</asp:Content>
