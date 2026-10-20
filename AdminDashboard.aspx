<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    Inherits="The_Analytical_Dojo.adminDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Admin Dashboard - The Analytical Dojo</title>
    <link href="Content/AdminDashboard.css" rel="stylesheet" />
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
                   class="sidebar-link active">
                    Dashboard
                </a>

                <a href="AdminUsers.aspx" class="sidebar-link">
                    Users
                </a>

                <a href="#" class="sidebar-link">
                    Courses
                </a>

                <a href="#" class="sidebar-link">
                    Materials
                </a>

                <a href="#" class="sidebar-link">
                    Quizzes
                </a>

                <a href="#" class="sidebar-link">
                    Enrollments
                </a>

                <a href="#" class="sidebar-link">
                    Analytics
                </a>

                <a href="AdminLeaderboard.aspx" class="sidebar-link">
                    Leaderboard
                </a>

                <a href="#" class="sidebar-link">
                    Forum Moderation
                </a>

                <a href="AdminSettings.aspx" class="sidebar-link">
                    Settings
                </a>

            </nav>

        </aside>

        <main class="main-content">

            <div class="page-header">

                <div>

                    <h1>Admin Dashboard</h1>

                    <p>
                        Monitor users, courses, quizzes and platform activity.
                    </p>

                </div>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="stats-grid">

                <div class="stat-card">

                    <span class="stat-title">
                        REGISTERED STUDENTS
                    </span>

                    <asp:Label
                        ID="lblActiveStudents"
                        runat="server"
                        Text="0"
                        CssClass="stat-value">
                    </asp:Label>

                    <span class="stat-description">
                        Registered student accounts
                    </span>

                </div>

                <div class="stat-card">

                    <span class="stat-title">
                        TOTAL COURSES
                    </span>

                    <asp:Label
                        ID="lblTotalCourses"
                        runat="server"
                        Text="0"
                        CssClass="stat-value">
                    </asp:Label>

                    <span class="stat-description">
                        Published courses
                    </span>

                </div>

                <div class="stat-card">

                    <span class="stat-title">
                        QUIZ ATTEMPTS TODAY
                    </span>

                    <asp:Label
                        ID="lblQuizAttemptsToday"
                        runat="server"
                        Text="0"
                        CssClass="stat-value">
                    </asp:Label>

                    <span class="stat-description">
                        Submitted today
                    </span>

                </div>

                <div class="stat-card">

                    <span class="stat-title">
                        AVERAGE COURSE SCORE
                    </span>

                    <asp:Label
                        ID="lblAverageCourseScore"
                        runat="server"
                        Text="0%"
                        CssClass="stat-value">
                    </asp:Label>

                    <span class="stat-description">
                        Across enrolled students
                    </span>

                </div>

            </div>

            <div class="dashboard-section">

                <div class="section-card">

                    <h2>Course Completion Overview</h2>

                    <div class="completion-grid">

                        <div class="completion-item">

                            <span class="completion-label">
                                TOTAL ENROLLMENTS
                            </span>

                            <asp:Label
                                ID="lblTotalEnrollments"
                                runat="server"
                                Text="0"
                                CssClass="completion-value">
                            </asp:Label>

                        </div>

                        <div class="completion-item">

                            <span class="completion-label">
                                COMPLETED
                            </span>

                            <asp:Label
                                ID="lblCompletedEnrollments"
                                runat="server"
                                Text="0"
                                CssClass="completion-value">
                            </asp:Label>

                        </div>

                        <div class="completion-item">

                            <span class="completion-label">
                                IN PROGRESS
                            </span>

                            <asp:Label
                                ID="lblInProgressEnrollments"
                                runat="server"
                                Text="0"
                                CssClass="completion-value">
                            </asp:Label>

                        </div>

                        <div class="completion-item">

                            <span class="completion-label">
                                AVERAGE COMPLETION
                            </span>

                            <asp:Label
                                ID="lblAverageCompletion"
                                runat="server"
                                Text="0%"
                                CssClass="completion-value">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>

            <div class="dashboard-section">

                <div class="section-card">

                    <div class="section-header-row">

                        <h2>Pending Course Verification</h2>

                        <asp:Label
                            ID="lblPendingCourseCount"
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

                                <div class="pending-course-info">

                                    <span class="pending-type">
                                        COURSE
                                    </span>

                                    <h3>
                                        <%#: Eval("course_title") %>
                                    </h3>

                                    <p>
                                        Submitted by:
                                        <%#: Eval("creator_name") %>
                                    </p>

                                </div>

                                <div class="pending-actions">

                                    <asp:Button
                                        ID="btnPublish"
                                        runat="server"
                                        Text="Publish"
                                        CssClass="action-btn publish-btn"
                                        CommandName="PublishCourse"
                                        CommandArgument='<%# Eval("course_id") %>'
                                        OnClientClick="return confirm('Publish this course?');" />

                                    <asp:Button
                                        ID="btnReject"
                                        runat="server"
                                        Text="Reject"
                                        CssClass="action-btn reject-btn"
                                        CommandName="RejectCourse"
                                        CommandArgument='<%# Eval("course_id") %>'
                                        OnClientClick="return confirm('Are you sure you want to reject this course?');" />

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                    <asp:Panel
                        ID="pnlNoPendingCourses"
                        runat="server"
                        CssClass="empty-message"
                        Visible="false">

                        No courses are currently waiting for verification.

                    </asp:Panel>

                </div>

            </div>

            <div class="dashboard-section">

                <div class="section-card">

                    <h2>Quiz Performance</h2>

                    <div class="quiz-grid">

                        <div class="quiz-stat">

                            <span class="quiz-label">
                                TOTAL ATTEMPTS
                            </span>

                            <asp:Label
                                ID="lblTotalQuizAttempts"
                                runat="server"
                                Text="0"
                                CssClass="quiz-value">
                            </asp:Label>

                        </div>

                        <div class="quiz-stat">

                            <span class="quiz-label">
                                SUBMITTED ATTEMPTS
                            </span>

                            <asp:Label
                                ID="lblSubmittedQuizAttempts"
                                runat="server"
                                Text="0"
                                CssClass="quiz-value">
                            </asp:Label>

                        </div>

                        <div class="quiz-stat">

                            <span class="quiz-label">
                                AVERAGE SCORE EARNED
                            </span>

                            <asp:Label
                                ID="lblAverageQuizScore"
                                runat="server"
                                Text="0"
                                CssClass="quiz-value">
                            </asp:Label>

                        </div>

                        <div class="quiz-stat">

                            <span class="quiz-label">
                                MOST ATTEMPTED QUIZ
                            </span>

                            <asp:Label
                                ID="lblMostAttemptedQuiz"
                                runat="server"
                                Text="No attempts yet"
                                CssClass="quiz-name-value">
                            </asp:Label>

                        </div>

                    </div>

                </div>

            </div>

            <div class="dashboard-section">

                <div class="section-card">

                    <h2>Recent Activity</h2>

                    <asp:Repeater
                        ID="rptRecentActivity"
                        runat="server">

                        <ItemTemplate>

                            <div class="activity-item">

                                <div class="activity-main">

                                    <span class="activity-type">
                                        <%#: Eval("activity_type") %>
                                    </span>

                                    <h3>
                                        <%#: Eval("activity_title") %>
                                    </h3>

                                    <p>
                                        <%#: Eval("actor_name") %>
                                    </p>

                                </div>

                                <div class="activity-date">
                                    <%# Eval("activity_date", "{0:dd MMM yyyy, hh:mm tt}") %>
                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                    <asp:Panel
                        ID="pnlNoRecentActivity"
                        runat="server"
                        CssClass="empty-message"
                        Visible="false">

                        No recent activity is available.

                    </asp:Panel>

                </div>

            </div>

        </main>

    </div>

</form>

</body>

</html>