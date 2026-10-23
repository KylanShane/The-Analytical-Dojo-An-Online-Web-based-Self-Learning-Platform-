<%@ Page Title="Viewer | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="MaterialView.aspx.cs" Inherits="The_Analytical_Dojo.MaterialView" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">

    <!-- Title, breadcrumbs and actions -->
    <asp:Literal ID="ViewerHead" runat="server" />

    <div class="viewer-layout" data-viewer-layout>

        <!-- Large slide / video stage -->
        <asp:Literal ID="Stage" runat="server" />

        <!-- Every material in this course -->
        <asp:Literal ID="Playlist" runat="server" />

    </div>

</asp:Content>
