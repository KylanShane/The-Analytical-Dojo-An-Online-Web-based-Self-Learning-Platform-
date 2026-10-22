<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminUsers.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Manage Users - The Analytical Dojo</title>

    <link href="Content/AdminUsers.css" rel="stylesheet" />

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

                <a href="#"
                   class="sidebar-link">
                    Courses
                </a>

                <a href="#"
                   class="sidebar-link">
                    Materials
                </a>

                <a href="#"
                   class="sidebar-link">
                    Quizzes
                </a>

                <a href="#"
                   class="sidebar-link">
                    Enrollments
                </a>

                <a href="#"
                   class="sidebar-link">
                    Analytics
                </a>

                <a href="AdminLeaderboard.aspx"
                   class="sidebar-link">
                    Leaderboard
                </a>

                <a href="#"
                   class="sidebar-link">
                    Forum Moderation
                </a>

                <a href="AdminSettings.aspx"
                   class="sidebar-link">
                    Settings
                </a>

            </nav>

        </aside>

        <main class="main-content">

            <div class="page-header">

                <div>

                    <h1>Manage Users</h1>

                    <p>
                        View users and manage Peer Leader access.
                    </p>

                </div>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="summary-grid">

                <div class="summary-card">

                    <span class="summary-title">
                        TOTAL USERS
                    </span>

                    <asp:Label
                        ID="lblTotalUsers"
                        runat="server"
                        Text="0"
                        CssClass="summary-value">
                    </asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">
                        STUDENTS
                    </span>

                    <asp:Label
                        ID="lblTotalStudents"
                        runat="server"
                        Text="0"
                        CssClass="summary-value">
                    </asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">
                        PEER LEADERS
                    </span>

                    <asp:Label
                        ID="lblTotalPeerLeaders"
                        runat="server"
                        Text="0"
                        CssClass="summary-value">
                    </asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">
                        ADMINS
                    </span>

                    <asp:Label
                        ID="lblTotalAdmins"
                        runat="server"
                        Text="0"
                        CssClass="summary-value">
                    </asp:Label>

                </div>

            </div>

            <div class="user-section">

                <div class="user-card">

                    <div class="filter-row">

                        <div class="search-box">

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="search-input"
                                placeholder="Search username, name or email">
                            </asp:TextBox>

                        </div>

                        <div class="filter-box">

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

                        </div>

                        <asp:Button
                            ID="btnSearch"
                            runat="server"
                            Text="Search"
                            CssClass="search-btn"
                            OnClick="btnSearch_Click" />

                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Reset"
                            CssClass="reset-btn"
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

                                <asp:BoundField
                                    DataField="username"
                                    HeaderText="Username" />

                                <asp:BoundField
                                    DataField="full_name"
                                    HeaderText="Full Name" />

                                <asp:BoundField
                                    DataField="email"
                                    HeaderText="Email" />

                                <asp:TemplateField
                                    HeaderText="Role">

                                    <ItemTemplate>

                                        <span class='<%# GetRoleClass(Eval("role")) %>'>
                                            <%#: Eval("role") %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField
                                    HeaderText="Peer Leader Since">

                                    <ItemTemplate>

                                        <%#: FormatDate(Eval("peer_leader_since")) %>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField
                                    HeaderText="Created">

                                    <ItemTemplate>

                                        <%#: FormatDate(Eval("created_at")) %>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField
                                    HeaderText="Action">

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
                                                OnClientClick="return confirm('Remove Peer Leader access from this user?');" />

                                        </asp:Panel>

                                        <asp:Label
                                            ID="lblNoAction"
                                            runat="server"
                                            Text="-"
                                            CssClass="no-action"
                                            Visible='<%# !IsPeerLeader(Eval("role")) %>'>
                                        </asp:Label>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                            <EmptyDataTemplate>

                                <div class="empty-message">
                                    No users found.
                                </div>

                            </EmptyDataTemplate>

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </main>

    </div>

</form>

</body>

</html>