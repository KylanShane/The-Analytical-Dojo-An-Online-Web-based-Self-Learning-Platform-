<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    Inherits="The_Analytical_Dojo.adminDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Admin Dashboard - The Analytical Dojo</title>
    <link href="Content/AdminDashboard.css?v=6" rel="stylesheet" />
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
                   class="sidebar-link active">
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

            <section class="dashboard-banner">

                <div class="banner-content">

                    <span class="banner-tag">
                        ADMIN OVERVIEW
                    </span>

                    <h1>
                        Welcome back, Admin
                    </h1>

                    <p>
                        Monitor learners, review courses and keep the platform running smoothly.
                    </p>

                    <div class="banner-actions">

                        <a href="AdminUsers.aspx"
                           class="banner-primary">
                            Manage Users
                        </a>

                        <a href="AdminCourse.aspx"
                           class="banner-secondary">
                            Manage Courses
                        </a>

                    </div>

                </div>

                <div class="banner-side">

                    <div class="banner-shape shape-one"></div>
                    <div class="banner-shape shape-two"></div>

                    <div class="banner-card">

                        <span class="banner-card-label">
                            PLATFORM STATUS
                        </span>

                        <strong>
                            Everything in one place
                        </strong>

                        <span class="banner-card-sub">
                            Users · Courses · Quizzes · Forum
                        </span>

                        <div class="banner-indicators">

                            <span></span>
                            <span></span>
                            <span></span>

                        </div>

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
                        PLATFORM SNAPSHOT
                    </span>

                    <h2>
                        Key Metrics
                    </h2>

                </div>

                <span class="section-description">
                    Live overview of your learning platform
                </span>

            </div>

            <div class="stats-grid">

                <div class="stat-card stat-purple">

                    <div class="stat-icon">

                        <svg viewBox="0 0 24 24"
                             aria-hidden="true">
                            <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"></path>
                            <circle cx="9" cy="7" r="4"></circle>
                            <path d="M22 21v-2a4 4 0 0 0-3-3.87"></path>
                            <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                        </svg>

                    </div>

                    <div class="stat-content">

                        <span class="stat-label">
                            REGISTERED STUDENTS
                        </span>

                        <asp:Label
                            ID="lblActiveStudents"
                            runat="server"
                            Text="0"
                            CssClass="stat-number">
                        </asp:Label>

                        <span class="stat-caption">
                            Student accounts
                        </span>

                    </div>

                </div>

                <div class="stat-card stat-teal">

                    <div class="stat-icon">

                        <svg viewBox="0 0 24 24"
                             aria-hidden="true">
                            <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                            <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                        </svg>

                    </div>

                    <div class="stat-content">

                        <span class="stat-label">
                            PUBLISHED COURSES
                        </span>

                        <asp:Label
                            ID="lblTotalCourses"
                            runat="server"
                            Text="0"
                            CssClass="stat-number">
                        </asp:Label>

                        <span class="stat-caption">
                            Available courses
                        </span>

                    </div>

                </div>

                <div class="stat-card stat-yellow">

                    <div class="stat-icon">

                        <svg viewBox="0 0 24 24"
                             aria-hidden="true">
                            <path d="M9 11l3 3L22 4"></path>
                            <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"></path>
                        </svg>

                    </div>

                    <div class="stat-content">

                        <span class="stat-label">
                            QUIZ ATTEMPTS TODAY
                        </span>

                        <asp:Label
                            ID="lblQuizAttemptsToday"
                            runat="server"
                            Text="0"
                            CssClass="stat-number">
                        </asp:Label>

                        <span class="stat-caption">
                            Attempts today
                        </span>

                    </div>

                </div>

                <div class="stat-card stat-blue">

                    <div class="stat-icon">

                        <svg viewBox="0 0 24 24"
                             aria-hidden="true">
                            <circle cx="12" cy="12" r="9"></circle>
                            <path d="M8 16L16 8"></path>
                            <circle cx="8.5" cy="8.5" r="1"></circle>
                            <circle cx="15.5" cy="15.5" r="1"></circle>
                        </svg>

                    </div>

                    <div class="stat-content">

                        <span class="stat-label">
                            AVERAGE COURSE SCORE
                        </span>

                        <asp:Label
                            ID="lblAverageCourseScore"
                            runat="server"
                            Text="0%"
                            CssClass="stat-number">
                        </asp:Label>

                        <span class="stat-caption">
                            Across enrollments
                        </span>

                    </div>

                </div>

            </div>

            <div class="two-column-grid">

                <section class="content-card">

                    <div class="card-header">

                        <div>

                            <span class="section-label">
                                LEARNING PROGRESS
                            </span>

                            <h2>
                                Course Completion
                            </h2>

                            <p>
                                Track how students are progressing through enrolled courses.
                            </p>

                        </div>

                        <div class="card-icon purple-icon">

                            <svg viewBox="0 0 24 24"
                                 aria-hidden="true">
                                <circle cx="12" cy="12" r="9"></circle>
                                <path d="M8 12l3 3 5-6"></path>
                            </svg>

                        </div>

                    </div>

                    <div class="metric-grid">

                        <div class="metric-item">

                            <span class="metric-label">
                                TOTAL ENROLLMENTS
                            </span>

                            <asp:Label
                                ID="lblTotalEnrollments"
                                runat="server"
                                Text="0"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                Course registrations
                            </span>

                        </div>

                        <div class="metric-item">

                            <span class="metric-label">
                                COMPLETED
                            </span>

                            <asp:Label
                                ID="lblCompletedEnrollments"
                                runat="server"
                                Text="0"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                Finished courses
                            </span>

                        </div>

                        <div class="metric-item">

                            <span class="metric-label">
                                IN PROGRESS
                            </span>

                            <asp:Label
                                ID="lblInProgressEnrollments"
                                runat="server"
                                Text="0"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                Active learning
                            </span>

                        </div>

                        <div class="metric-item">

                            <span class="metric-label">
                                AVG. COMPLETION
                            </span>

                            <asp:Label
                                ID="lblAverageCompletion"
                                runat="server"
                                Text="0%"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                Overall progress
                            </span>

                        </div>

                    </div>

                </section>

                <section class="content-card">

                    <div class="card-header">

                        <div>

                            <span class="section-label">
                                ASSESSMENTS
                            </span>

                            <h2>
                                Quiz Performance
                            </h2>

                            <p>
                                Monitor quiz participation and overall student performance.
                            </p>

                        </div>

                        <div class="card-icon yellow-icon">

                            <svg viewBox="0 0 24 24"
                                 aria-hidden="true">
                                <path d="M9 11l3 3L22 4"></path>
                                <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"></path>
                            </svg>

                        </div>

                    </div>

                    <div class="metric-grid">

                        <div class="metric-item">

                            <span class="metric-label">
                                TOTAL ATTEMPTS
                            </span>

                            <asp:Label
                                ID="lblTotalQuizAttempts"
                                runat="server"
                                Text="0"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                All attempts
                            </span>

                        </div>

                        <div class="metric-item">

                            <span class="metric-label">
                                SUBMITTED
                            </span>

                            <asp:Label
                                ID="lblSubmittedQuizAttempts"
                                runat="server"
                                Text="0"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                Completed attempts
                            </span>

                        </div>

                        <div class="metric-item">

                            <span class="metric-label">
                                AVERAGE SCORE
                            </span>

                            <asp:Label
                                ID="lblAverageQuizScore"
                                runat="server"
                                Text="0"
                                CssClass="metric-value">
                            </asp:Label>

                            <span class="metric-caption">
                                Overall score
                            </span>

                        </div>

                        <div class="metric-item">

                            <span class="metric-label">
                                MOST ATTEMPTED
                            </span>

                            <asp:Label
                                ID="lblMostAttemptedQuiz"
                                runat="server"
                                Text="No attempts yet"
                                CssClass="quiz-name">
                            </asp:Label>

                            <span class="metric-caption">
                                Most popular quiz
                            </span>

                        </div>

                    </div>

                </section>

            </div>

            <section class="content-card full-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            CONTENT REVIEW
                        </span>

                        <h2>
                            Pending Course Verification
                        </h2>

                        <p>
                            Review submitted courses before publishing them to students.
                        </p>

                    </div>

                    <asp:Label
                        ID="lblPendingCourseCount"
                        runat="server"
                        Text="0 Pending"
                        CssClass="pending-count">
                    </asp:Label>

                </div>

                <div class="pending-list">

                    <asp:Repeater
                        ID="rptPendingCourses"
                        runat="server"
                        OnItemCommand="rptPendingCourses_ItemCommand">

                        <ItemTemplate>

                            <div class="pending-row">

                                <div class="pending-icon">

                                    <svg viewBox="0 0 24 24"
                                         aria-hidden="true">
                                        <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path>
                                        <path d="M4 4.5A2.5 2.5 0 0 1 6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5z"></path>
                                    </svg>

                                </div>

                                <div class="pending-info">

                                    <span class="pending-type">
                                        COURSE SUBMISSION
                                    </span>

                                    <h3>
                                        <%#: Eval("course_title") %>
                                    </h3>

                                    <span>
                                        Submitted by
                                        <strong>
                                            <%#: Eval("creator_name") %>
                                        </strong>
                                    </span>

                                </div>

                                <div class="pending-actions">

                                    <asp:Button
                                        ID="btnPublish"
                                        runat="server"
                                        Text="Publish"
                                        CssClass="publish-btn"
                                        CommandName="PublishCourse"
                                        CommandArgument='<%# Eval("course_id") %>'
                                        OnClientClick="return confirm('Publish this course?');" />

                                    <asp:Button
                                        ID="btnReject"
                                        runat="server"
                                        Text="Reject"
                                        CssClass="reject-btn"
                                        CommandName="RejectCourse"
                                        CommandArgument='<%# Eval("course_id") %>'
                                        OnClientClick="return confirm('Reject this course?');" />

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

                <asp:Panel
                    ID="pnlNoPendingCourses"
                    runat="server"
                    CssClass="empty-state"
                    Visible="false">

                    <div class="empty-state-icon">

                        <svg viewBox="0 0 24 24"
                             aria-hidden="true">
                            <circle cx="12" cy="12" r="9"></circle>
                            <path d="M8 12l3 3 5-6"></path>
                        </svg>

                    </div>

                    <strong>
                        No pending courses
                    </strong>

                    <span>
                        Everything has been reviewed.
                    </span>

                </asp:Panel>

            </section>

            <section class="content-card full-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            PLATFORM ACTIVITY
                        </span>

                        <h2>
                            Recent Activity
                        </h2>

                        <p>
                            Latest activity from students and courses.
                        </p>

                    </div>

                    <span class="live-badge">
                        LIVE
                    </span>

                </div>

                <div class="activity-list">

                    <asp:Repeater
                        ID="rptRecentActivity"
                        runat="server">

                        <ItemTemplate>

                            <div class="activity-row">

                                <div class="activity-dot"></div>

                                <div class="activity-content">

                                    <span class="activity-type">
                                        <%#: Eval("activity_type") %>
                                    </span>

                                    <h3>
                                        <%#: Eval("activity_title") %>
                                    </h3>

                                    <span>
                                        <%#: Eval("actor_name") %>
                                    </span>

                                </div>

                                <div class="activity-time">

                                    <%# Eval(
                                        "activity_date",
                                        "{0:dd MMM yyyy, hh:mm tt}"
                                    ) %>

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                </div>

                <asp:Panel
                    ID="pnlNoRecentActivity"
                    runat="server"
                    CssClass="empty-state"
                    Visible="false">

                    <div class="empty-state-icon">

                        <svg viewBox="0 0 24 24"
                             aria-hidden="true">
                            <circle cx="12" cy="12" r="9"></circle>
                            <path d="M12 8v4l3 2"></path>
                        </svg>

                    </div>

                    <strong>
                        No recent activity
                    </strong>

                    <span>
                        New activity will appear here.
                    </span>

                </asp:Panel>

            </section>

        </main>

    </div>

</form>

</body>

</html>