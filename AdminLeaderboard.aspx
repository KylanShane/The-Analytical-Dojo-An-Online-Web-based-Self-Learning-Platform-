<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminLeaderboard.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminLeaderboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Leaderboard - The Analytical Dojo</title>
    <link href="Content/AdminLeaderboard.css?v=2" rel="stylesheet" />
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
                   class="sidebar-link active">
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

            <section class="leaderboard-banner">

                <div class="banner-content">

                    <span class="banner-tag">
                        STUDENT PERFORMANCE
                    </span>

                    <h1>
                        Leaderboard
                    </h1>

                    <p>
                        View the highest-performing students and Peer Leaders based on course scores and completion progress.
                    </p>

                </div>

                <div class="banner-side">

                    <div class="banner-shape shape-one"></div>
                    <div class="banner-shape shape-two"></div>

                    <div class="banner-card">

                        <div class="banner-card-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <path d="M8 21h8"></path>
                                <path d="M12 17v4"></path>
                                <path d="M7 4h10v4a5 5 0 0 1-10 0z"></path>
                                <path d="M7 6H4v2a4 4 0 0 0 4 4"></path>
                                <path d="M17 6h3v2a4 4 0 0 1-4 4"></path>
                            </svg>

                        </div>

                        <span class="banner-card-label">
                            PERFORMANCE RANKING
                        </span>

                        <strong>
                            Top Learners
                        </strong>

                        <span class="banner-card-sub">
                            Students · Peer Leaders
                        </span>

                    </div>

                </div>

            </section>

            <div class="section-heading">

                <div>

                    <span class="section-label">
                        TOP PERFORMERS
                    </span>

                    <h2>
                        Top Three
                    </h2>

                </div>

                <span class="section-description">
                    Highest-ranked learners across all enrollments
                </span>

            </div>

            <div class="top-three-grid">

                <div class="top-card second-place">

                    <div class="rank-circle">
                        2
                    </div>

                    <div class="medal-icon silver-medal">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <circle cx="12" cy="14" r="6"></circle>
                            <path d="M9 8L6 2h5l1 4"></path>
                            <path d="M15 8l3-6h-5l-1 4"></path>
                        </svg>

                    </div>

                    <span class="rank-label">
                        SECOND PLACE
                    </span>

                    <asp:Label
                        ID="lblSecondName"
                        runat="server"
                        Text="No Data"
                        CssClass="top-name">
                    </asp:Label>

                    <asp:Label
                        ID="lblSecondRole"
                        runat="server"
                        Text="-"
                        CssClass="top-role">
                    </asp:Label>

                    <asp:Label
                        ID="lblSecondScore"
                        runat="server"
                        Text="0%"
                        CssClass="top-score">
                    </asp:Label>

                    <span class="score-caption">
                        Average score
                    </span>

                </div>

                <div class="top-card first-place">

                    <div class="rank-circle">
                        1
                    </div>

                    <div class="medal-icon gold-medal">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path d="M8 21h8"></path>
                            <path d="M12 17v4"></path>
                            <path d="M7 4h10v4a5 5 0 0 1-10 0z"></path>
                            <path d="M7 6H4v2a4 4 0 0 0 4 4"></path>
                            <path d="M17 6h3v2a4 4 0 0 1-4 4"></path>
                        </svg>

                    </div>

                    <span class="rank-label">
                        FIRST PLACE
                    </span>

                    <asp:Label
                        ID="lblFirstName"
                        runat="server"
                        Text="No Data"
                        CssClass="top-name">
                    </asp:Label>

                    <asp:Label
                        ID="lblFirstRole"
                        runat="server"
                        Text="-"
                        CssClass="top-role">
                    </asp:Label>

                    <asp:Label
                        ID="lblFirstScore"
                        runat="server"
                        Text="0%"
                        CssClass="top-score">
                    </asp:Label>

                    <span class="score-caption">
                        Average score
                    </span>

                </div>

                <div class="top-card third-place">

                    <div class="rank-circle">
                        3
                    </div>

                    <div class="medal-icon bronze-medal">

                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <circle cx="12" cy="14" r="6"></circle>
                            <path d="M9 8L6 2h5l1 4"></path>
                            <path d="M15 8l3-6h-5l-1 4"></path>
                        </svg>

                    </div>

                    <span class="rank-label">
                        THIRD PLACE
                    </span>

                    <asp:Label
                        ID="lblThirdName"
                        runat="server"
                        Text="No Data"
                        CssClass="top-name">
                    </asp:Label>

                    <asp:Label
                        ID="lblThirdRole"
                        runat="server"
                        Text="-"
                        CssClass="top-role">
                    </asp:Label>

                    <asp:Label
                        ID="lblThirdScore"
                        runat="server"
                        Text="0%"
                        CssClass="top-score">
                    </asp:Label>

                    <span class="score-caption">
                        Average score
                    </span>

                </div>

            </div>

            <section class="leaderboard-card">

                <div class="card-header">

                    <div>

                        <span class="section-label">
                            FULL RANKING
                        </span>

                        <h2>
                            Student Rankings
                        </h2>

                        <p>
                            Search the leaderboard and filter results by learner role.
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
                            placeholder="Search student name or username">
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
                        ID="gvLeaderboard"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="leaderboard-table"
                        GridLines="None">

                        <Columns>

                            <asp:TemplateField HeaderText="Rank">

                                <ItemTemplate>

                                    <span class='<%# Convert.ToInt32(Eval("actual_rank")) <= 3 ? "rank-number rank-top" : "rank-number" %>'>
                                        #<%# Eval("actual_rank") %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Student">

                                <ItemTemplate>

                                    <div class="student-info">

                                        <div class="student-avatar">
                                            <%# Eval("full_name").ToString().Length > 0 ? Eval("full_name").ToString().Substring(0, 1).ToUpper() : "S" %>
                                        </div>

                                        <div>

                                            <div class="student-name">
                                                <%#: Eval("full_name") %>
                                            </div>

                                            <div class="student-username">
                                                @<%#: Eval("username") %>
                                            </div>

                                        </div>

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Role">

                                <ItemTemplate>

                                    <span class='<%# GetRoleClass(Eval("role")) %>'>
                                        <%#: Eval("role") %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Courses Enrolled">

                                <ItemTemplate>

                                    <span class="courses-count">
                                        <%# Eval("courses_enrolled") %>
                                    </span>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Average Score">

                                <ItemTemplate>

                                    <div class="percentage-cell">

                                        <strong>
                                            <%# FormatPercentage(Eval("average_score")) %>
                                        </strong>

                                        <div class="progress-track">

                                            <div
                                                class="progress-fill score-fill"
                                                style='width:<%# Math.Min(100, Convert.ToDouble(Eval("average_score"))) %>%'>
                                            </div>

                                        </div>

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Average Completion">

                                <ItemTemplate>

                                    <div class="percentage-cell">

                                        <strong>
                                            <%# FormatPercentage(Eval("average_completion")) %>
                                        </strong>

                                        <div class="progress-track">

                                            <div
                                                class="progress-fill completion-fill"
                                                style='width:<%# Math.Min(100, Convert.ToDouble(Eval("average_completion"))) %>%'>
                                            </div>

                                        </div>

                                    </div>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                        <EmptyDataTemplate>

                            <div class="empty-state">

                                <div class="empty-state-icon">

                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M8 21h8"></path>
                                        <path d="M12 17v4"></path>
                                        <path d="M7 4h10v4a5 5 0 0 1-10 0z"></path>
                                    </svg>

                                </div>

                                <strong>
                                    No leaderboard data found
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