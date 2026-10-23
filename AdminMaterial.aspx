<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminMaterial.aspx.cs"
    MaintainScrollPositionOnPostBack="true"
    Inherits="The_Analytical_Dojo.AdminMaterial" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Manage Materials - The Analytical Dojo</title>

    <link href="Content/AdminMaterial.css?v=1" rel="stylesheet" />

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
                        <a href="AdminMaterial.aspx" class="sidebar-sublink active">Materials</a>
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

                    <h1>Manage Materials</h1>

                    <p>
                        View, edit and remove the learning materials of each course.
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

                    <span class="summary-title">TOTAL MATERIALS</span>

                    <asp:Label ID="lblTotalMaterials" runat="server" Text="0" CssClass="summary-value"></asp:Label>

                    <asp:Label ID="lblTotalSub" runat="server" Text="-" CssClass="summary-sub"></asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">PPT MATERIALS</span>

                    <asp:Label ID="lblPptMaterials" runat="server" Text="0" CssClass="summary-value"></asp:Label>

                    <asp:Label ID="lblPptSub" runat="server" Text="Slides and documents" CssClass="summary-sub"></asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">VIDEO MATERIALS</span>

                    <asp:Label ID="lblVideoMaterials" runat="server" Text="0" CssClass="summary-value"></asp:Label>

                    <asp:Label ID="lblVideoSub" runat="server" Text="Lesson videos" CssClass="summary-sub"></asp:Label>

                </div>

                <div class="summary-card">

                    <span class="summary-title">COURSES COVERED</span>

                    <asp:Label ID="lblCoursesCovered" runat="server" Text="0" CssClass="summary-value"></asp:Label>

                    <asp:Label ID="lblCoveredSub" runat="server" Text="-" CssClass="summary-sub"></asp:Label>

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
                                placeholder="Search material or chapter title">
                            </asp:TextBox>

                        </div>

                        <div class="filter-box">

                            <asp:DropDownList
                                ID="ddlCourseFilter"
                                runat="server"
                                CssClass="filter-dropdown">
                            </asp:DropDownList>

                        </div>

                        <div class="filter-box">

                            <asp:DropDownList
                                ID="ddlTypeFilter"
                                runat="server"
                                CssClass="filter-dropdown">

                                <asp:ListItem Text="All Types" Value=""></asp:ListItem>
                                <asp:ListItem Text="PPT" Value="PPT"></asp:ListItem>
                                <asp:ListItem Text="Video" Value="VIDEO"></asp:ListItem>

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

                        <a href="AdminCreateMaterial.aspx" class="create-btn">
                            + Create
                        </a>

                    </div>

                    <div class="table-wrapper">

                        <asp:GridView
                            ID="gvMaterials"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="users-table"
                            GridLines="None"
                            DataKeyNames="material_id"
                            OnRowCommand="gvMaterials_RowCommand">

                            <Columns>

                                <asp:BoundField DataField="material_id" HeaderText="ID" />

                                <asp:BoundField DataField="material_title" HeaderText="Material Title" />

                                <asp:BoundField DataField="course_title" HeaderText="Course" />

                                <asp:BoundField DataField="chapter_title" HeaderText="Chapter" />

                                <asp:TemplateField HeaderText="Type">

                                    <ItemTemplate>

                                        <span class='<%# GetTypeClass(Eval("material_type")) %>'>
                                            <%#: FormatType(Eval("material_type")) %>
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
                                                CommandName="ViewMaterial"
                                                CommandArgument='<%# Eval("material_id") %>'
                                                CausesValidation="false" />

                                            <asp:Button
                                                ID="btnEdit"
                                                runat="server"
                                                Text="Edit"
                                                CssClass="edit-btn"
                                                CommandName="EditMaterial"
                                                CommandArgument='<%# Eval("material_id") %>'
                                                CausesValidation="false" />

                                            <asp:Button
                                                ID="btnDelete"
                                                runat="server"
                                                Text="Delete"
                                                CssClass="remove-btn"
                                                CommandName="DeleteMaterial"
                                                CommandArgument='<%# Eval("material_id") %>'
                                                CausesValidation="false"
                                                OnClientClick="return confirm('Delete this material and its uploaded file? This cannot be undone.');" />

                                        </div>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                            <EmptyDataTemplate>

                                <div class="empty-message">
                                    No materials found.
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

            <h2>Material Details</h2>

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
