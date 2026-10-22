<%@ Page Title="Profile | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="The_Analytical_Dojo.Profile" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">

    <div class="eyebrow">PRIVATE ACCOUNT</div>
    <h1>Your profile<span class="yellow">.</span></h1>
    <p class="muted">Only you can open this page. Other students cannot view your details.</p>

    <div class="profile-layout">

        <section class="panel profile-card">
            <div class="avatar avatar-lg" aria-hidden="true">
                <%= H(ProfileInitials) %>
            </div>
            <h2><%= H(Session["FullName"]) %></h2>
            <p class="muted">Signed in as <%= H(Session["Username"]) %></p>
            <asp:Button ID="Logout" runat="server" CssClass="btn ghost" Text="Log out" OnClick="Logout_Click" CausesValidation="false" />
        </section>

        <section class="panel">
            <h2>Account details</h2>
            <p class="muted">You can change your username, email and password. Confirm with your current password.</p>

            <asp:Label ID="Status" runat="server" CssClass="notice" Visible="false" role="status" />

            <div class="field">
                <asp:Label runat="server" AssociatedControlID="FullName" Text="Full name" />
                <asp:TextBox ID="FullName" runat="server" CssClass="input" MaxLength="100" />
            </div>

            <div class="field">
                <asp:Label runat="server" AssociatedControlID="Username" Text="Username" />
                <asp:TextBox ID="Username" runat="server" CssClass="input" MaxLength="50" />
            </div>

            <div class="field">
                <asp:Label runat="server" AssociatedControlID="Email" Text="Email" />
                <asp:TextBox ID="Email" runat="server" CssClass="input" TextMode="Email" MaxLength="150" />
            </div>

            <div class="field">
                <asp:Label runat="server" AssociatedControlID="CurrentPassword" Text="Current password" />
                <asp:TextBox ID="CurrentPassword" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <div class="field">
                <asp:Label runat="server" AssociatedControlID="NewPassword" Text="New password (optional)" />
                <asp:TextBox ID="NewPassword" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <div class="field">
                <asp:Label runat="server" AssociatedControlID="ConfirmPassword" Text="Confirm new password" />
                <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="input" TextMode="Password" />
            </div>

            <asp:Button ID="Save" runat="server" CssClass="btn" Text="Save changes" OnClick="Save_Click" />
        </section>
    </div>

</asp:Content>
