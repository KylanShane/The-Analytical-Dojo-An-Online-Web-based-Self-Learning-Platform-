<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminForum.aspx.cs"
    MaintainScrollPositionOnPostBack="true"
    Inherits="The_Analytical_Dojo.AdminForum" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Forum Moderation - The Analytical Dojo</title>
    <link href="Content/AdminForum.css?v=2" rel="stylesheet" />
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
                   class="sidebar-link">
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
                   class="sidebar-link active">
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

            <section class="forum-banner">

                <div class="banner-content">

                    <span class="banner-tag">
                        COMMUNITY MANAGEMENT
                    </span>

                    <h1>
                        Forum Moderation
                    </h1>

                    <p>
                        Review discussions across courses and manage posts or replies that require moderation.
                    </p>

                </div>

                <div class="banner-side">

                    <div class="banner-shape shape-one"></div>
                    <div class="banner-shape shape-two"></div>

                    <div class="banner-card">

                        <div class="banner-card-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <path d="M21 15a4 4 0 0 1-4 4H8l-5 3V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4z"></path>
                                <path d="M8 9h8"></path>
                                <path d="M8 13h5"></path>
                            </svg>

                        </div>

                        <span class="banner-card-label">
                            DISCUSSION CONTROL
                        </span>

                        <strong>
                            Keep the forum healthy
                        </strong>

                        <span class="banner-card-sub">
                            Posts · Replies · Moderation
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
                        FORUM OVERVIEW
                    </span>

                    <h2>
                        Moderation Summary
                    </h2>

                </div>

                <span class="section-description">
                    Current discussion status
                </span>

            </div>

            <div class="summary-grid">

                <div class="summary-card card-purple">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M21 15a4 4 0 0 1-4 4H8l-5 3V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4z"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            ACTIVE POSTS
                        </span>

                        <asp:Label
                            ID="lblActivePosts"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Visible discussions
                        </span>

                    </div>

                </div>

                <div class="summary-card card-blue">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M8 9h8"></path>
                            <path d="M8 13h6"></path>
                            <path d="M18 18H9l-5 3V8a4 4 0 0 1 4-4h9a4 4 0 0 1 4 4v7"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            ACTIVE REPLIES
                        </span>

                        <asp:Label
                            ID="lblActiveReplies"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Visible replies
                        </span>

                    </div>

                </div>

                <div class="summary-card card-red">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M3 6h18"></path>
                            <path d="M8 6V4h8v2"></path>
                            <path d="M19 6l-1 14H6L5 6"></path>
                            <path d="M10 11v5"></path>
                            <path d="M14 11v5"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            REMOVED POSTS
                        </span>

                        <asp:Label
                            ID="lblDeletedPosts"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Soft-deleted posts
                        </span>

                    </div>

                </div>

                <div class="summary-card card-yellow">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M3 6h18"></path>
                            <path d="M8 6V4h8v2"></path>
                            <path d="M19 6l-1 14H6L5 6"></path>
                            <path d="M10 11v5"></path>
                            <path d="M14 11v5"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            REMOVED REPLIES
                        </span>

                        <asp:Label
                            ID="lblDeletedReplies"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <span class="summary-caption">
                            Soft-deleted replies
                        </span>

                    </div>

                </div>

            </div>

            <section class="forum-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            DISCUSSION DIRECTORY
                        </span>

                        <h2>
                            Forum Content
                        </h2>

                        <p>
                            Search, filter, remove or restore forum posts and replies.
                        </p>

                    </div>

                    <div class="card-header-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M21 15a4 4 0 0 1-4 4H8l-5 3V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4z"></path>
                            <path d="M8 9h8"></path>
                            <path d="M8 13h5"></path>
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
                            placeholder="Search content, name, username or course">
                        </asp:TextBox>

                    </div>

                    <asp:DropDownList
                        ID="ddlCourse"
                        runat="server"
                        CssClass="filter-dropdown">
                    </asp:DropDownList>

                    <asp:DropDownList
                        ID="ddlType"
                        runat="server"
                        CssClass="filter-dropdown">

                        <asp:ListItem
                            Text="All Types"
                            Value="">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Posts"
                            Value="POST">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Replies"
                            Value="REPLY">
                        </asp:ListItem>

                    </asp:DropDownList>

                    <asp:DropDownList
                        ID="ddlStatus"
                        runat="server"
                        CssClass="filter-dropdown">

                        <asp:ListItem
                            Text="All Status"
                            Value="">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Active"
                            Value="ACTIVE">
                        </asp:ListItem>

                        <asp:ListItem
                            Text="Removed"
                            Value="DELETED">
                        </asp:ListItem>

                    </asp:DropDownList>

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
                        ID="gvForum"
                        runat="server"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        CssClass="forum-table"
                        OnRowCommand="gvForum_RowCommand">

                        <Columns>

                            <asp:TemplateField HeaderText="Type">

                                <ItemTemplate>

                                    <span class='<%# GetTypeClass(Eval("content_type")) %>'>
                                        <%#: Eval("content_type") %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="User">

                                <ItemTemplate>

                                    <div class="user-info">

                                        <div class="user-avatar">
                                            <%# Eval("full_name").ToString().Length > 0 ? Eval("full_name").ToString().Substring(0, 1).ToUpper() : "U" %>
                                        </div>

                                        <div>

                                            <div class="user-name">
                                                <%#: Eval("full_name") %>
                                            </div>

                                            <div class="username">
                                                @<%#: Eval("username") %>
                                            </div>

                                        </div>

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Course">

                                <ItemTemplate>

                                    <div class="course-name">
                                        <%#: Eval("course_title") %>
                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Content">

                                <ItemTemplate>

                                    <div class="content-text">
                                        <%#: Eval("content") %>
                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Created">

                                <ItemTemplate>

                                    <span class="date-text">
                                        <%# FormatDate(Eval("created_at")) %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status">

                                <ItemTemplate>

                                    <span class='<%# GetStatusClass(Eval("is_deleted")) %>'>
                                        <%# GetStatusText(Eval("is_deleted")) %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">

                                <ItemTemplate>

                                    <div class="action-buttons">

                                        <asp:Button
                                            ID="btnDelete"
                                            runat="server"
                                            Text="Remove"
                                            CssClass="remove-btn"
                                            CommandName="DeleteContent"
                                            CommandArgument='<%# Eval("content_type") + "|" + Eval("content_id") %>'
                                            Visible='<%# !IsDeleted(Eval("is_deleted")) %>'
                                            CausesValidation="false"
                                            OnClientClick="return confirm('Remove this forum content? You can restore it later.');" />

                                        <asp:Button
                                            ID="btnRestore"
                                            runat="server"
                                            Text="Restore"
                                            CssClass="restore-btn"
                                            CommandName="RestoreContent"
                                            CommandArgument='<%# Eval("content_type") + "|" + Eval("content_id") %>'
                                            Visible='<%# IsDeleted(Eval("is_deleted")) %>'
                                            CausesValidation="false"
                                            OnClientClick="return confirm('Restore this forum content?');" />

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                        <EmptyDataTemplate>

                            <div class="empty-state">

                                <div class="empty-state-icon">

                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M21 15a4 4 0 0 1-4 4H8l-5 3V7a4 4 0 0 1 4-4h10a4 4 0 0 1 4 4z"></path>
                                    </svg>

                                </div>

                                <strong>
                                    No forum content found
                                </strong>

                                <span>
                                    Try changing your search or filter settings.
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