<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminCourse.aspx.cs"
    MaintainScrollPositionOnPostBack="true"
    Inherits="The_Analytical_Dojo.AdminCourse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Manage Courses - The Analytical Dojo</title>

    <link href="Content/AdminCourse.css?v=4" rel="stylesheet" />

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

                    <a href="AdminCourse.aspx" class="sidebar-link active">
                        Courses
                    </a>

                    <div class="sidebar-submenu">
                        <a href="AdminMaterial.aspx" class="sidebar-sublink">Materials</a>
                        <a href="AdminQuiz.aspx" class="sidebar-sublink">Quizzes</a>
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

                <div>

                    <h1>Manage Courses</h1>

                    <p>
                        View, edit and remove courses on the platform.
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
                        CssClass="summary-sub">
                    </asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">
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
                        CssClass="summary-sub">
                    </asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">
                        INTERMEDIATE COURSES
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
                        CssClass="summary-sub">
                    </asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">
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
                        CssClass="summary-sub">
                    </asp:Label>

                </div>

            </div>

            <div class="pending-section">

                <div class="pending-card">

                    <div class="section-header-row">

                        <h2>Pending Verification Courses</h2>

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

                                    <div class="pending-top">

                                        <span class="pending-chip">
                                            Course
                                        </span>

                                        <span class="pending-level">
                                            <%#: FormatText(Eval("course_level")) %>
                                        </span>

                                    </div>

                                    <h3 class="pending-title">
                                        <%#: Eval("course_title") %>
                                    </h3>

                                    <div class="pending-bottom">

                                        <p class="pending-by">
                                            Proposed by: <%#: Eval("creator_name") %>
                                        </p>

                                        <div class="pending-actions">

                                            <button type="button" class="pending-review-btn review-btn">
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

                                </div>

                                <div class="pending-detail">

                                    <h4>Course information</h4>

                                    <div class="detail-grid">

                                        <div>
                                            <span class="detail-label">COURSE TITLE</span>
                                            <span class="detail-value"><%#: Eval("course_title") %></span>
                                        </div>

                                        <div>
                                            <span class="detail-label">SUBMITTED BY</span>
                                            <span class="detail-value"><%#: Eval("creator_name") %></span>
                                        </div>

                                        <div>
                                            <span class="detail-label">LEVEL</span>
                                            <span class='<%# GetLevelClass(Eval("course_level")) %>'>
                                                <%#: FormatText(Eval("course_level")) %>
                                            </span>
                                        </div>

                                        <div>
                                            <span class="detail-label">STATUS</span>
                                            <span class='<%# GetStatusClass(Eval("status")) %>'>
                                                <%#: FormatText(Eval("status")) %>
                                            </span>
                                        </div>

                                        <div class="detail-full">
                                            <span class="detail-label">DESCRIPTION</span>
                                            <span class="detail-value"><%#: OrDash(Eval("description")) %></span>
                                        </div>

                                    </div>

                                    <h4>Course details</h4>

                                    <%# GetChapterDetailsHtml(Eval("course_id")) %>

                                </div>

                            </div>

                        </ItemTemplate>

                    </asp:Repeater>

                    <asp:Panel
                        ID="pnlNoPending"
                        runat="server"
                        CssClass="empty-message"
                        Visible="false">

                        No courses are currently waiting for verification.

                    </asp:Panel>

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
                                placeholder="Search course title">
                            </asp:TextBox>

                        </div>

                        <div class="filter-box">

                            <asp:DropDownList
                                ID="ddlLevelFilter"
                                runat="server"
                                CssClass="filter-dropdown">

                                <asp:ListItem Text="All Levels" Value=""></asp:ListItem>
                                <asp:ListItem Text="Basic" Value="basic"></asp:ListItem>
                                <asp:ListItem Text="Intermediate" Value="intermediate"></asp:ListItem>
                                <asp:ListItem Text="Advanced" Value="advanced"></asp:ListItem>

                            </asp:DropDownList>

                        </div>

                        <div class="filter-box">

                            <asp:DropDownList
                                ID="ddlStatusFilter"
                                runat="server"
                                CssClass="filter-dropdown">

                                <asp:ListItem Text="All Status" Value=""></asp:ListItem>
                                <asp:ListItem Text="Published" Value="PUBLISHED"></asp:ListItem>
                                <asp:ListItem Text="Pending" Value="PENDING"></asp:ListItem>
                                <asp:ListItem Text="Archived" Value="ARCHIVED"></asp:ListItem>

                            </asp:DropDownList>

                        </div>

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

                        <a href="AdminCreateCourse.aspx" class="create-btn">
                            + Create
                        </a>

                    </div>

                    <div class="table-wrapper">

                        <asp:GridView
                            ID="gvCourses"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="users-table"
                            GridLines="None"
                            DataKeyNames="course_id"
                            OnRowCommand="gvCourses_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="course_id"
                                    HeaderText="ID" />

                                <asp:BoundField
                                    DataField="course_title"
                                    HeaderText="Course Title" />

                                <asp:TemplateField
                                    HeaderText="Level">

                                    <ItemTemplate>

                                        <span class='<%# GetLevelClass(Eval("course_level")) %>'>
                                            <%#: FormatText(Eval("course_level")) %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField
                                    HeaderText="Status">

                                    <ItemTemplate>

                                        <span class='<%# GetStatusClass(Eval("status")) %>'>
                                            <%#: FormatText(Eval("status")) %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField
                                    HeaderText="Action">

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

                                <div class="empty-message">
                                    No courses found.
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

            <h2>Course Details</h2>

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

    document.addEventListener('click', function (e) {
        var btn = e.target.closest('.review-btn');
        if (!btn) { return; }
        var item = btn.closest('.pending-item');
        var open = item.classList.toggle('open');
        btn.textContent = open ? 'Hide' : 'Review';
    });
</script>

</body>

</html>
