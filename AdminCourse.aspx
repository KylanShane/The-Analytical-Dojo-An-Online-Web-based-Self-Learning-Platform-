<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminCourse.aspx.cs"
    MaintainScrollPositionOnPostBack="true"
    Inherits="The_Analytical_Dojo.AdminCourse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Manage Courses - The Analytical Dojo</title>
    <link href="Content/AdminCourse.css?v=5" rel="stylesheet" />
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

                <div class="sidebar-group open">

                    <a href="AdminCourse.aspx"
                       class="sidebar-link active">
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

            <section class="course-banner">

                <div class="banner-content">

                    <span class="banner-tag">
                        COURSE MANAGEMENT
                    </span>

                    <h1>
                        Manage Courses
                    </h1>

                    <p>
                        Review course submissions, manage published content and keep the learning catalogue organised.
                    </p>

                    <div class="banner-actions">

                        <a href="AdminCreateCourse.aspx"
                           class="banner-primary">
                            Create Course
                        </a>

                        <a href="AdminMaterial.aspx"
                           class="banner-secondary">
                            View Materials
                        </a>

                    </div>

                </div>

                <div class="banner-side">

                    <div class="banner-shape shape-one"></div>
                    <div class="banner-shape shape-two"></div>

                    <div class="banner-card">

                        <div class="banner-card-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                            </svg>

                        </div>

                        <span class="banner-card-label">
                            COURSE CATALOGUE
                        </span>

                        <strong>
                            Learning Content
                        </strong>

                        <span class="banner-card-sub">
                            Basic · Intermediate · Advanced
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
                        COURSE OVERVIEW
                    </span>

                    <h2>
                        Course Summary
                    </h2>

                </div>

                <span class="section-description">
                    Current course catalogue
                </span>

            </div>

            <div class="summary-grid">

                <div class="summary-card card-purple">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                            <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            TOTAL COURSES
                        </span>

                        <asp:Label
                            ID="lblTotalCourses"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <asp:Label
                            ID="lblTotalSub"
                            runat="server"
                            Text="0 Published"
                            CssClass="summary-caption">
                        </asp:Label>

                    </div>

                </div>

                <div class="summary-card card-blue">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M4 5h16"></path>
                            <path d="M4 10h16"></path>
                            <path d="M4 15h10"></path>
                            <path d="M4 20h7"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            BASIC COURSES
                        </span>

                        <asp:Label
                            ID="lblBasicCourses"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <asp:Label
                            ID="lblBasicSub"
                            runat="server"
                            Text="-"
                            CssClass="summary-caption">
                        </asp:Label>

                    </div>

                </div>

                <div class="summary-card card-yellow">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M4 18l5-5 4 4 7-8"></path>
                            <path d="M15 9h5v5"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            INTERMEDIATE
                        </span>

                        <asp:Label
                            ID="lblIntermediateCourses"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <asp:Label
                            ID="lblIntermediateSub"
                            runat="server"
                            Text="-"
                            CssClass="summary-caption">
                        </asp:Label>

                    </div>

                </div>

                <div class="summary-card card-green">

                    <div class="summary-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M12 2l3 6 6.5.9-4.7 4.6 1.1 6.5L12 17l-5.9 3 1.1-6.5L2.5 8.9 9 8z"></path>
                        </svg>

                    </div>

                    <div>

                        <span class="summary-label">
                            ADVANCED COURSES
                        </span>

                        <asp:Label
                            ID="lblAdvancedCourses"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                        <asp:Label
                            ID="lblAdvancedSub"
                            runat="server"
                            Text="-"
                            CssClass="summary-caption">
                        </asp:Label>

                    </div>

                </div>

            </div>

            <section class="pending-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            CONTENT REVIEW
                        </span>

                        <h2>
                            Pending Verification
                        </h2>

                        <p>
                            Review newly submitted courses before publishing them to students.
                        </p>

                    </div>

                    <asp:Label
                        ID="lblPendingCount"
                        runat="server"
                        Text="0 Pending"
                        CssClass="pending-count">
                    </asp:Label>

                </div>

                <asp:Repeater
                    ID="rptPendingCourses"
                    runat="server"
                    OnItemCommand="rptPendingCourses_ItemCommand">

                    <ItemTemplate>

                        <div class="pending-item">

                            <div class="pending-head">

                                <div class="pending-main">

                                    <div class="pending-icon">

                                        <svg viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                            <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                                        </svg>

                                    </div>

                                    <div class="pending-info">

                                        <div class="pending-top">

                                            <span class="pending-chip">
                                                COURSE
                                            </span>

                                            <span class="pending-level">
                                                <%#: FormatText(Eval("course_level")) %>
                                            </span>

                                        </div>

                                        <h3 class="pending-title">
                                            <%#: Eval("course_title") %>
                                        </h3>

                                        <p class="pending-by">
                                            Proposed by
                                            <strong>
                                                <%#: Eval("creator_name") %>
                                            </strong>
                                        </p>

                                    </div>

                                </div>

                                <div class="pending-actions">

                                    <button
                                        type="button"
                                        class="review-btn">
                                        Review
                                    </button>

                                    <asp:Button
                                        ID="btnPublish"
                                        runat="server"
                                        Text="Approve"
                                        CssClass="publish-btn"
                                        CommandName="PublishCourse"
                                        CommandArgument='<%# Eval("course_id") %>'
                                        CausesValidation="false"
                                        OnClientClick="return confirm('Approve and publish this course?');" />

                                </div>

                            </div>

                            <div class="pending-detail">

                                <h4>
                                    Course Information
                                </h4>

                                <div class="detail-grid">

                                    <div>

                                        <span class="detail-label">
                                            COURSE TITLE
                                        </span>

                                        <span class="detail-value">
                                            <%#: Eval("course_title") %>
                                        </span>

                                    </div>

                                    <div>

                                        <span class="detail-label">
                                            SUBMITTED BY
                                        </span>

                                        <span class="detail-value">
                                            <%#: Eval("creator_name") %>
                                        </span>

                                    </div>

                                    <div>

                                        <span class="detail-label">
                                            LEVEL
                                        </span>

                                        <span class='<%# GetLevelClass(Eval("course_level")) %>'>
                                            <%#: FormatText(Eval("course_level")) %>
                                        </span>

                                    </div>

                                    <div>

                                        <span class="detail-label">
                                            STATUS
                                        </span>

                                        <span class='<%# GetStatusClass(Eval("status")) %>'>
                                            <%#: FormatText(Eval("status")) %>
                                        </span>

                                    </div>

                                    <div class="detail-full">

                                        <span class="detail-label">
                                            DESCRIPTION
                                        </span>

                                        <span class="detail-value">
                                            <%#: OrDash(Eval("description")) %>
                                        </span>

                                    </div>

                                </div>

                                <h4>
                                    Course Details
                                </h4>

                                <%# GetChapterDetailsHtml(Eval("course_id")) %>

                            </div>

                        </div>

                    </ItemTemplate>

                </asp:Repeater>

                <asp:Panel
                    ID="pnlNoPending"
                    runat="server"
                    CssClass="empty-state"
                    Visible="false">

                    <div class="empty-state-icon">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <circle cx="12" cy="12" r="9"></circle>
                            <path d="M8 12l3 3 5-6"></path>
                        </svg>

                    </div>

                    <strong>
                        No pending courses
                    </strong>

                    <span>
                        All submitted courses have been reviewed.
                    </span>

                </asp:Panel>

            </section>

            <section class="course-list-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            COURSE DIRECTORY
                        </span>

                        <h2>
                            All Courses
                        </h2>

                        <p>
                            Search, filter and manage courses on the platform.
                        </p>

                    </div>

                    <div class="card-header-icon">

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
                            placeholder="Search course title">
                        </asp:TextBox>

                    </div>

                    <asp:DropDownList
                        ID="ddlLevelFilter"
                        runat="server"
                        CssClass="filter-dropdown">

                        <asp:ListItem Text="All Levels" Value=""></asp:ListItem>
                        <asp:ListItem Text="Basic" Value="basic"></asp:ListItem>
                        <asp:ListItem Text="Intermediate" Value="intermediate"></asp:ListItem>
                        <asp:ListItem Text="Advanced" Value="advanced"></asp:ListItem>

                    </asp:DropDownList>

                    <asp:DropDownList
                        ID="ddlStatusFilter"
                        runat="server"
                        CssClass="filter-dropdown">

                        <asp:ListItem Text="All Status" Value=""></asp:ListItem>
                        <asp:ListItem Text="Published" Value="PUBLISHED"></asp:ListItem>
                        <asp:ListItem Text="Pending" Value="PENDING"></asp:ListItem>
                        <asp:ListItem Text="Archived" Value="ARCHIVED"></asp:ListItem>

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

                    <a href="AdminCreateCourse.aspx"
                       class="create-btn">
                        Create Course
                    </a>

                </div>

                <div class="table-wrapper">

                    <asp:GridView
                        ID="gvCourses"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="courses-table"
                        GridLines="None"
                        DataKeyNames="course_id"
                        OnRowCommand="gvCourses_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="course_id"
                                HeaderText="ID" />

                            <asp:TemplateField HeaderText="Course">

                                <ItemTemplate>

                                    <div class="course-info">

                                        <div class="course-icon-small">

                                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                                <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                                <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                                            </svg>

                                        </div>

                                        <span class="course-title">
                                            <%#: Eval("course_title") %>
                                        </span>

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Level">

                                <ItemTemplate>

                                    <span class='<%# GetLevelClass(Eval("course_level")) %>'>
                                        <%#: FormatText(Eval("course_level")) %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status">

                                <ItemTemplate>

                                    <span class='<%# GetStatusClass(Eval("status")) %>'>
                                        <%#: FormatText(Eval("status")) %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">

                                <ItemTemplate>

                                    <div class="action-group">

                                        <asp:Button
                                            ID="btnView"
                                            runat="server"
                                            Text="View"
                                            CssClass="view-btn"
                                            CommandName="ViewCourse"
                                            CommandArgument='<%# Eval("course_id") %>'
                                            CausesValidation="false" />

                                        <asp:Button
                                            ID="btnEdit"
                                            runat="server"
                                            Text="Edit"
                                            CssClass="edit-btn"
                                            CommandName="EditCourse"
                                            CommandArgument='<%# Eval("course_id") %>'
                                            CausesValidation="false" />

                                        <asp:Button
                                            ID="btnDelete"
                                            runat="server"
                                            Text="Delete"
                                            CssClass="remove-btn"
                                            CommandName="DeleteCourse"
                                            CommandArgument='<%# Eval("course_id") %>'
                                            CausesValidation="false"
                                            OnClientClick="return confirm('Delete this course together with all its chapters and materials? This cannot be undone.');" />

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                        <EmptyDataTemplate>

                            <div class="empty-state">

                                <div class="empty-state-icon">

                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                        <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                                    </svg>

                                </div>

                                <strong>
                                    No courses found
                                </strong>

                                <span>
                                    Try changing your search or filters.
                                </span>

                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </section>

        </main>

    </div>

    <asp:Panel
        ID="pnlView"
        runat="server"
        CssClass="modal-overlay"
        Visible="false">

        <div class="modal-card">

            <div class="modal-head">

                <div>

                    <span class="section-label">
                        COURSE INFORMATION
                    </span>

                    <h2>
                        Course Details
                    </h2>

                </div>

                <button
                    type="button"
                    class="modal-x modal-close"
                    aria-label="Close">
                    &times;
                </button>

            </div>

            <div class="modal-body">

                <div class="pending-detail">

                    <asp:Literal
                        ID="litViewContent"
                        runat="server">
                    </asp:Literal>

                </div>

            </div>

            <div class="modal-foot">

                <button
                    type="button"
                    class="reset-btn modal-close">
                    Close
                </button>

            </div>

        </div>

    </asp:Panel>

</form>

<script>
    document.addEventListener('click', function (e) {
        var overlay = e.target.closest('.modal-overlay');

        if (overlay && (e.target === overlay || e.target.closest('.modal-close'))) {
            overlay.style.display = 'none';
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            document.querySelectorAll('.modal-overlay').forEach(function (o) {
                o.style.display = 'none';
            });
        }
    });

    document.addEventListener('click', function (e) {
        var btn = e.target.closest('.review-btn');

        if (!btn) {
            return;
        }

        var item = btn.closest('.pending-item');
        var open = item.classList.toggle('open');

        btn.textContent = open ? 'Hide Details' : 'Review';
    });
</script>

</body>

</html>