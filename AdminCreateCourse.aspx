<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminCreateCourse.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminCreateCourse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Create New Course - The Analytical Dojo</title>

    <link href="Content/AdminCreateCourse.css" rel="stylesheet" />

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

                <div class="breadcrumb">
                    <a href="AdminCourse.aspx">Manage Courses</a>
                    <span>&rsaquo;</span>
                    <span class="current"><asp:Literal ID="litBreadcrumb" runat="server" Text="Create New Course"></asp:Literal></span>
                </div>

                <h1><asp:Literal ID="litHeading" runat="server" Text="Create New Course"></asp:Literal></h1>

                <p>
                    <asp:Literal ID="litSubtitle" runat="server" Text="Create the course record. Add its chapters and materials from the Materials menu."></asp:Literal>
                </p>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="form-card">

                <h2>Course Information</h2>

                <p class="card-subtitle">
                    Define the core details learners will see in the curriculum index.
                </p>

                <div class="form-group">

                    <label>Course Title <span class="required">*</span></label>

                    <asp:TextBox
                        ID="txtTitle"
                        runat="server"
                        CssClass="form-input"
                        MaxLength="100"
                        placeholder="e.g. Advanced Data Visualization with Python">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvTitle"
                        runat="server"
                        ControlToValidate="txtTitle"
                        ErrorMessage="Course title is required."
                        ValidationGroup="CreateCourse"
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                    <span class="field-hint">
                        Use a clear, specific title.
                        <span class="char-left">100</span> characters remaining.
                    </span>

                </div>

                <div class="form-group">

                    <label>Course Description <span class="required">*</span></label>

                    <asp:TextBox
                        ID="txtDescription"
                        runat="server"
                        CssClass="form-input form-textarea"
                        TextMode="MultiLine"
                        Rows="5"
                        placeholder="Summarize the learning outcome in 2-3 sentences.">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvDescription"
                        runat="server"
                        ControlToValidate="txtDescription"
                        ErrorMessage="Course description is required."
                        ValidationGroup="CreateCourse"
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                    <span class="field-hint">
                        Summarize the learning outcome in 2-3 sentences.
                    </span>

                </div>

                <div class="form-group">

                    <label>Difficulty <span class="required">*</span></label>

                    <asp:DropDownList
                        ID="ddlLevel"
                        runat="server"
                        CssClass="form-input">

                        <asp:ListItem Text="Basic" Value="basic"></asp:ListItem>
                        <asp:ListItem Text="Intermediate" Value="intermediate"></asp:ListItem>
                        <asp:ListItem Text="Advanced" Value="advanced"></asp:ListItem>

                    </asp:DropDownList>

                </div>

                <div class="form-group">

                    <label>Course Status <span class="required">*</span></label>

                    <asp:RadioButtonList
                        ID="rblStatus"
                        runat="server"
                        CssClass="segmented"
                        RepeatLayout="Flow"
                        RepeatDirection="Horizontal">

                        <asp:ListItem Text="Under Review" Value="PENDING" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Published" Value="PUBLISHED"></asp:ListItem>

                    </asp:RadioButtonList>

                    <span class="field-hint">
                        Courses under review appear in the Pending Verification list until an admin publishes them.
                    </span>

                </div>

            </div>

            <div class="action-bar">

                <span class="action-note">
                    <asp:Literal ID="litActionNote" runat="server" Text="NEW COURSE &amp;middot; UNSAVED CHANGES"></asp:Literal>
                </span>

                <div class="action-buttons">

                    <asp:HyperLink
                        ID="lnkCancel"
                        runat="server"
                        NavigateUrl="~/AdminCourse.aspx"
                        CssClass="btn btn-outline">
                        Cancel
                    </asp:HyperLink>

                    <asp:Button
                        ID="btnCreate"
                        runat="server"
                        Text="Create Course"
                        CssClass="btn btn-primary"
                        ValidationGroup="CreateCourse"
                        OnClick="btnCreate_Click" />

                </div>

            </div>

        </main>

    </div>

</form>

<script>
    (function () {
        var input = document.getElementById('<%= txtTitle.ClientID %>');
        var counter = input.closest('.form-group').querySelector('.char-left');
        function update() { counter.textContent = input.maxLength - input.value.length; }
        input.addEventListener('input', update);
        update();
    })();
</script>

</body>

</html>
