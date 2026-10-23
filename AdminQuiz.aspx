<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminQuiz.aspx.cs"
    MaintainScrollPositionOnPostBack="true"
    Inherits="The_Analytical_Dojo.AdminQuiz" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Quiz Management - The Analytical Dojo</title>

    <link href="Content/AdminQuiz.css?v=1" rel="stylesheet" />

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

                <div class="sidebar-group open">

                    <a href="AdminCourse.aspx" class="sidebar-link">
                        Courses
                    </a>

                    <div class="sidebar-submenu">
                        <a href="AdminMaterial.aspx" class="sidebar-sublink">Materials</a>
                        <a href="AdminQuiz.aspx" class="sidebar-sublink active">Quizzes</a>
                    </div>

                </div>

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

                <div class="breadcrumb">
                    <a href="AdminDashboard.aspx">Admin</a>
                    <span>&rsaquo;</span>
                    <span class="current">Quiz Management</span>
                </div>

                <h1>Quiz Management</h1>

                <p>
                    Create assessments, maintain question banks, and protect published learning records.
                </p>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="user-card toolbar-card">

                <div class="filter-row no-gap-bottom">

                    <div class="search-box">

                        <asp:TextBox
                            ID="txtSearch"
                            runat="server"
                            CssClass="search-input"
                            placeholder="Search quizzes, courses or chapters">
                        </asp:TextBox>

                    </div>

                    <div class="filter-box">

                        <asp:DropDownList
                            ID="ddlTypeFilter"
                            runat="server"
                            CssClass="filter-dropdown">

                            <asp:ListItem Text="All Quiz Types" Value=""></asp:ListItem>
                            <asp:ListItem Text="Objective" Value="OBJ"></asp:ListItem>
                            <asp:ListItem Text="Subjective" Value="SUBJ"></asp:ListItem>

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

                    <a href="AdminCreateQuiz.aspx" class="create-btn">
                        + Add Quiz
                    </a>

                </div>

            </div>

            <div class="user-section">

                <div class="user-card">

                    <h2 class="index-title">Quiz Index</h2>

                    <p class="index-summary">
                        <asp:Label ID="lblIndexSummary" runat="server" Text="0 quizzes"></asp:Label>
                    </p>

                    <div class="table-wrapper">

                        <asp:GridView
                            ID="gvQuizzes"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="users-table"
                            GridLines="None"
                            DataKeyNames="quiz_id"
                            OnRowCommand="gvQuizzes_RowCommand">

                            <Columns>

                                <asp:TemplateField HeaderText="Quiz Title">

                                    <ItemTemplate>

                                        <strong class="quiz-name"><%#: Eval("quiz_title") %></strong>

                                        <span class="quiz-meta">
                                            Created <%# Eval("created_at", "{0:dd MMM yyyy}") %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Type">

                                    <ItemTemplate>

                                        <span class='<%# GetTypeClass(Eval("quiz_type")) %>'>
                                            <%#: FormatType(Eval("quiz_type")) %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:BoundField DataField="course_title" HeaderText="Course" />

                                <asp:BoundField DataField="chapter_title" HeaderText="Chapter" />

                                <asp:BoundField DataField="question_count" HeaderText="Questions" />

                                <asp:TemplateField HeaderText="Actions">

                                    <ItemTemplate>

                                        <div class="action-group">

                                            <asp:Button
                                                ID="btnView"
                                                runat="server"
                                                Text="View"
                                                CssClass="view-btn"
                                                CommandName="ViewQuiz"
                                                CommandArgument='<%# Eval("quiz_id") %>'
                                                CausesValidation="false" />

                                            <asp:Button
                                                ID="btnUpdate"
                                                runat="server"
                                                Text="Update"
                                                CssClass="edit-btn"
                                                CommandName="UpdateQuiz"
                                                CommandArgument='<%# Eval("quiz_id") %>'
                                                CausesValidation="false" />

                                            <asp:Button
                                                ID="btnDelete"
                                                runat="server"
                                                Text="Delete"
                                                CssClass="remove-btn"
                                                CommandName="DeleteQuiz"
                                                CommandArgument='<%# Eval("quiz_id") %>'
                                                CausesValidation="false"
                                                OnClientClick="return confirm('Delete this quiz together with all its questions and answers? This cannot be undone.');" />

                                        </div>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                            <EmptyDataTemplate>

                                <div class="empty-message">
                                    No quizzes found.
                                </div>

                            </EmptyDataTemplate>

                        </asp:GridView>

                    </div>

                </div>

            </div>

        </main>

    </div>

<asp:Panel
    ID="pnlView"
    runat="server"
    CssClass="modal-overlay"
    Visible="false">

    <div class="modal-card">

        <div class="modal-head">

            <h2>Quiz Details</h2>

            <button type="button" class="modal-x modal-close" aria-label="Close">&times;</button>

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

            <button type="button" class="reset-btn modal-close">Close</button>

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
</script>

</body>

</html>
