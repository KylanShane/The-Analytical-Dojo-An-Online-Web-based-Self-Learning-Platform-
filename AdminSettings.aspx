<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminSettings.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminSettings" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Admin Settings - The Analytical Dojo</title>

    <link href="Content/AdminSettings.css" rel="stylesheet" />

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

                <a href="AdminLeaderboard.aspx" class="sidebar-link">
                    Leaderboard
                </a>

                <a href="#" class="sidebar-link">
                    Forum Moderation
                </a>

                <a href="AdminSettings.aspx" class="sidebar-link active">
                    Settings
                </a>

            </nav>

        </aside>

        <main class="main-content">

            <div class="page-header">

                <div>

                    <h1>Admin Settings</h1>

                    <p>
                        Manage your administrator account information.
                    </p>

                </div>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="settings-card">

                <div class="settings-header">

                    <div class="profile-avatar">

                        <span>
                            A
                        </span>

                    </div>

                    <div>

                        <h2>Account Information</h2>

                        <p>
                            Update your personal account details.
                        </p>

                    </div>

                </div>

                <div class="form-grid">

                    <div class="form-group">

                        <label>
                            Username
                        </label>

                        <asp:TextBox
                            ID="txtUsername"
                            runat="server"
                            CssClass="form-input readonly-input"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>

                    <div class="form-group">

                        <label>
                            Role
                        </label>

                        <asp:TextBox
                            ID="txtRole"
                            runat="server"
                            CssClass="form-input readonly-input"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>

                    <div class="form-group">

                        <label>
                            Full Name
                        </label>

                        <asp:TextBox
                            ID="txtFullName"
                            runat="server"
                            CssClass="form-input"
                            MaxLength="100">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvFullName"
                            runat="server"
                            ControlToValidate="txtFullName"
                            ErrorMessage="Full name is required."
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                    </div>

                    <div class="form-group">

                        <label>
                            Email
                        </label>

                        <asp:TextBox
                            ID="txtEmail"
                            runat="server"
                            CssClass="form-input"
                            TextMode="Email"
                            MaxLength="150">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvEmail"
                            runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="Email is required."
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                        <asp:RegularExpressionValidator
                            ID="revEmail"
                            runat="server"
                            ControlToValidate="txtEmail"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Enter a valid email address."
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RegularExpressionValidator>

                    </div>

                    <div class="form-group">

                        <label>
                            Account Created
                        </label>

                        <asp:TextBox
                            ID="txtCreatedAt"
                            runat="server"
                            CssClass="form-input readonly-input"
                            ReadOnly="true">
                        </asp:TextBox>

                    </div>

                </div>

                <div class="settings-actions">

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save Changes"
                        CssClass="save-btn"
                        OnClick="btnSave_Click" />

                </div>

            </div>

        </main>

    </div>

</form>

</body>

</html>