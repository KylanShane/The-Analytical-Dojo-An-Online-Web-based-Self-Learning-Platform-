<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminLeaderboard.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminLeaderboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Leaderboard - The Analytical Dojo</title>

    <link href="Content/AdminLeaderboard.css" rel="stylesheet" />

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

                <a href="AdminDashboard.aspx" class="sidebar-link">
                    Dashboard
                </a>

                <a href="AdminUsers.aspx" class="sidebar-link">
                    Users
                </a>

                <div class="sidebar-group">
                    <a href="AdminCourse.aspx" class="sidebar-link">
                        Courses

                    </a>

                    <div class="sidebar-submenu">
                        <a href="AdminMaterial.aspx" class="sidebar-sublink">Materials</a>
                        <a href="AdminQuiz.aspx" class="sidebar-sublink">Quizzes</a>
                    </div>
                </div>

                <a href="AdminLeaderboard.aspx" class="sidebar-link active">
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

                    <h1>Leaderboard</h1>

                    <p>
                        View the highest-performing students and Peer Leaders.
                    </p>

                </div>

            </div>

            <div class="top-section">

                <div class="top-card first-place">

                    <span class="rank-label">
                        1st Place
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

                </div>

                <div class="top-card second-place">

                    <span class="rank-label">
                        2nd Place
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

                </div>

                <div class="top-card third-place">

                    <span class="rank-label">
                        3rd Place
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

                </div>

            </div>

            <div class="leaderboard-section">

                <div class="leaderboard-card">

                    <div class="filter-row">

                        <div class="search-box">

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

                                <asp:TemplateField
                                    HeaderText="Rank">

                                    <ItemTemplate>

                                        <span class="rank-number">
                                            <%# Eval("actual_rank") %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:BoundField
                                    DataField="full_name"
                                    HeaderText="Student" />

                                <asp:BoundField
                                    DataField="username"
                                    HeaderText="Username" />

                                <asp:TemplateField
                                    HeaderText="Role">

                                    <ItemTemplate>

                                        <span class='<%# GetRoleClass(Eval("role")) %>'>
                                            <%#: Eval("role") %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:BoundField
                                    DataField="courses_enrolled"
                                    HeaderText="Courses Enrolled" />

                                <asp:TemplateField
                                    HeaderText="Average Score">

                                    <ItemTemplate>

                                        <%# FormatPercentage(Eval("average_score")) %>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField
                                    HeaderText="Average Completion">

                                    <ItemTemplate>

                                        <%# FormatPercentage(Eval("average_completion")) %>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                            <EmptyDataTemplate>

                                <div class="empty-message">
                                    No leaderboard data found.
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