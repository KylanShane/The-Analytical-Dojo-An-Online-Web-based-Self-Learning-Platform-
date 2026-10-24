<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminUsers.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Manage Users - The Analytical Dojo</title>
    <link href="Content/AdminUsers.css?v=2" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <div class="dashboard-wrapper">

        <aside class="sidebar">

            <div class="sidebar-logo">

                <asp:Image
                    ID="imgLogo"
                    runat="server"
                    ImageUrl="~/Images/logo.png"
                    CssClass="logo-img" />

                <span class="admin-label">
                    ADMIN PORTAL
                </span>

            </div>

            <nav class="sidebar-menu">

                <a href="AdminDashboard.aspx"
                   class="sidebar-link">
                    Dashboard
                </a>

                <a href="AdminUsers.aspx"
                   class="sidebar-link active">
                    Users
                </a>

                <div class="sidebar-group">

                    <a href="AdminCourse.aspx"
                       class="sidebar-link">
                        Courses
                    </a>

                    <div class="sidebar-submenu">

                        <a href="AdminMaterial.aspx"
                           class="sidebar-sublink">
                            Materials
                        </a>

                        <a href="AdminQuiz.aspx"
                           class="sidebar-sublink">
                            Quizzes
                        </a>

                    </div>

                </div>

                <a href="AdminLeaderboard.aspx"
                   class="sidebar-link">
                    Leaderboard
                </a>

                <a href="AdminForum.aspx"
                   class="sidebar-link">
                    Forum Moderation
                </a>

                <a href="AdminSettings.aspx"
                   class="sidebar-link">
                    Settings
                </a>

            </nav>

            <div class="sidebar-footer">

                <span>
                    The Analytical Dojo
                </span>

                <small>
                    Administration Console
                </small>

            </div>

        </aside>

        <main class="main-content">

            <section class="users-banner">

                <div class="banner-content">

                    <span class="banner-tag">
                        USER MANAGEMENT
                    </span>

                    <h1>
                        Manage Users
                    </h1>

                    <p>
                        View registered accounts, create administrators and manage Peer Leader access.
                    </p>

                </div>

                <div class="banner-side">

                    <div class="banner-shape shape-one"></div>
                    <div class="banner-shape shape-two"></div>

                    <div class="banner-card">

                        <div class="banner-card-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
                                <circle cx="9" cy="7" r="4"></circle>
                                <path d="M22 21v-2a4 4 0 0 0-3-3.87"></path>
                                <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                            </svg>

                        </div>

                        <span class="banner-card-label">
                            ACCOUNT CONTROL
                        </span>

                        <strong>
                            Users & Roles
                        </strong>

                        <span class="banner-card-sub">
                            Students · Peer Leaders · Admins
                        </span>

                    </div>

                </div>

            </section>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="section-heading">

                <div>

                    <span class="section-label">
                        USER OVERVIEW
                    </span>

                    <h2>
                        Account Summary
                    </h2>

                </div>

                <span class="section-description">
                    Current registered accounts
                </span>

            </div>

            <div class="summary-grid">

                <div class="summary-card card-purple">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
                            <circle cx="9" cy="7" r="4"></circle>
                            <path d="M22 21v-2a4 4 0 0 0-3-3.87"></path>
                            <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            TOTAL USERS
                        </span>

                        <asp:Label
                            ID="lblTotalUsers"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            All registered accounts
                        </span>

                    </div>

                </div>

                <div class="summary-card card-blue">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <circle cx="12" cy="7" r="4"></circle>
                            <path d="M5.5 21v-2a6.5 6.5 0 0 1 13 0v2"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            STUDENTS
                        </span>

                        <asp:Label
                            ID="lblTotalStudents"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Learning accounts
                        </span>

                    </div>

                </div>

                <div class="summary-card card-yellow">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M12 2l3 6 6.5.9-4.7 4.6 1.1 6.5L12 17l-5.9 3 1.1-6.5L2.5 8.9 9 8z"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            PEER LEADERS
                        </span>

                        <asp:Label
                            ID="lblTotalPeerLeaders"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Student contributors
                        </span>

                    </div>

                </div>

                <div class="summary-card card-green">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                            <path d="M9 12l2 2 4-4"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            ADMINS
                        </span>

                        <asp:Label
                            ID="lblTotalAdmins"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Administrator accounts
                        </span>

                    </div>

                </div>

            </div>

            <section class="add-admin-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            ADMINISTRATION
                        </span>

                        <h2>
                            Add New Admin
                        </h2>

                        <p>
                            Create an administrator account with access to the admin portal.
                        </p>

                    </div>

                    <div class="card-header-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <circle cx="9" cy="7" r="4"></circle>
                            <path d="M2 21v-2a7 7 0 0 1 7-7h2"></path>
                            <path d="M19 8v6"></path>
                            <path d="M16 11h6"></path>
                        </svg>

                    </div>

                </div>

                <div class="admin-form-grid">

                    <div class="form-group">

                        <label>
                            Full Name
                        </label>

                        <asp:TextBox
                            ID="txtAdminFullName"
                            runat="server"
                            CssClass="form-input"
                            MaxLength="100"
                            placeholder="Enter full name">
                        </asp:TextBox>

                    </div>

                    <div class="form-group">

                        <label>
                            Username
                        </label>

                        <asp:TextBox
                            ID="txtAdminUsername"
                            runat="server"
                            CssClass="form-input"
                            MaxLength="100"
                            placeholder="Enter username">
                        </asp:TextBox>

                    </div>

                    <div class="form-group">

                        <label>
                            Email Address
                        </label>

                        <asp:TextBox
                            ID="txtAdminEmail"
                            runat="server"
                            CssClass="form-input"
                            MaxLength="150"
                            TextMode="Email"
                            placeholder="Enter email address">
                        </asp:TextBox>

                    </div>

                    <div class="form-group">

                        <label>
                            Password
                        </label>

                        <asp:TextBox
                            ID="txtAdminPassword"
                            runat="server"
                            CssClass="form-input"
                            TextMode="Password"
                            placeholder="Enter password">
                        </asp:TextBox>

                    </div>

                    <div class="form-group">

                        <label>
                            Confirm Password
                        </label>

                        <asp:TextBox
                            ID="txtAdminConfirmPassword"
                            runat="server"
                            CssClass="form-input"
                            TextMode="Password"
                            placeholder="Confirm password">
                        </asp:TextBox>

                    </div>

                </div>

                <div class="admin-form-actions">

                    <asp:Button
                        ID="btnAddAdmin"
                        runat="server"
                        Text="Add Admin"
                        CssClass="add-admin-btn"
                        OnClick="btnAddAdmin_Click" />

                    <asp:Button
                        ID="btnClearAdmin"
                        runat="server"
                        Text="Clear"
                        CssClass="clear-admin-btn"
                        CausesValidation="false"
                        OnClick="btnClearAdmin_Click" />

                </div>

            </section>

            <section class="users-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            ACCOUNT DIRECTORY
                        </span>

                        <h2>
                            Registered Users
                        </h2>

                        <p>
                            Search accounts and manage Peer Leader access.
                        </p>

                    </div>

                    <div class="card-header-icon user-list-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M8 6h13"></path>
                            <path d="M8 12h13"></path>
                            <path d="M8 18h13"></path>
                            <circle cx="3" cy="6" r="1"></circle>
                            <circle cx="3" cy="12" r="1"></circle>
                            <circle cx="3" cy="18" r="1"></circle>
                        </svg>

                    </div>

                </div>

                <div class="filter-panel">

                    <div class="search-box">

                        <div class="search-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <circle cx="11" cy="11" r="7"></circle>
                                <path d="M20 20l-4-4"></path>
                            </svg>

                        </div>

                        <asp:TextBox
                            ID="txtSearch"
                            runat="server"
                            CssClass="search-input"
                            placeholder="Search username, name or email">
                        </asp:TextBox>

                    </div>

                    <asp:DropDownList
                        ID="ddlRoleFilter"
                        runat="server"
                        CssClass="filter-dropdown">

                        <asp:ListItem
                            Text="All Roles"
                            Value="">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Student"
                            Value="STUDENT">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Peer Leader"
                            Value="PEER LEADER">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Admin"
                            Value="ADMIN">
                        </asp:ListItem>

                    </asp:DropDownList>

                    <asp:Button
                        ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="search-btn"
                        CausesValidation="false"
                        OnClick="btnSearch_Click" />

                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="Reset"
                        CssClass="reset-btn"
                        CausesValidation="false"
                        OnClick="btnReset_Click" />

                </div>

                <div class="table-wrapper">

                    <asp:GridView
                        ID="gvUsers"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="users-table"
                        GridLines="None"
                        DataKeyNames="user_id"
                        OnRowCommand="gvUsers_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="user_id"
                                HeaderText="ID" />

                            <asp:TemplateField HeaderText="User">

                                <ItemTemplate>

                                    <div class="table-user">

                                        <div class="table-avatar">
                                            <%# Eval("full_name").ToString().Length > 0 ? Eval("full_name").ToString().Substring(0, 1).ToUpper() : "U" %>
                                        </div>

                                        <div>

                                            <div class="table-name">
                                                <%#: Eval("full_name") %>
                                            </div>

                                            <div class="table-username">
                                                @<%#: Eval("username") %>
                                            </div>

                                        </div>

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="email"
                                HeaderText="Email" />

                            <asp:TemplateField HeaderText="Role">

                                <ItemTemplate>

                                    <span class='<%# GetRoleClass(Eval("role")) %>'>
                                        <%#: Eval("role") %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Peer Leader Since">

                                <ItemTemplate>

                                    <span class="date-text">
                                        <%#: FormatDate(Eval("peer_leader_since")) %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Created">

                                <ItemTemplate>

                                    <span class="date-text">
                                        <%#: FormatDate(Eval("created_at")) %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">

                                <ItemTemplate>

                                    <asp:Panel
                                        ID="pnlPeerLeaderAction"
                                        runat="server"
                                        Visible='<%# IsPeerLeader(Eval("role")) %>'>

                                        <asp:Button
                                            ID="btnRemovePeerLeader"
                                            runat="server"
                                            Text="Remove Peer Leader"
                                            CssClass="remove-btn"
                                            CommandName="RemovePeerLeader"
                                            CommandArgument='<%# Eval("user_id") %>'
                                            CausesValidation="false"
                                            OnClientClick="return confirm('Remove Peer Leader access from this user?');" />

                                    </asp:Panel>

                                    <asp:Label
                                        ID="lblNoAction"
                                        runat="server"
                                        Text="—"
                                        CssClass="no-action"
                                        Visible='<%# !IsPeerLeader(Eval("role")) %>'>
                                    </asp:Label>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                        <EmptyDataTemplate>

                            <div class="empty-state">

                                <div class="empty-state-icon">

                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <circle cx="12" cy="7" r="4"></circle>
                                        <path d="M5 21v-2a7 7 0 0 1 14 0v2"></path>
                                    </svg>

                                </div>

                                <strong>
                                    No users found
                                </strong>

                                <span>
                                    Try changing your search or role filter.
                                </span>

                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </section>

        </main>

    </div>

</form>

</body>

</html>