<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminSettings.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminSettings" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Settings - The Analytical Dojo</title>
    <link href="Content/AdminSettings.css?v=2" rel="stylesheet" />
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
                   class="sidebar-link">
                    Leaderboard
                </a>

                <a href="AdminForum.aspx"
                   class="sidebar-link">
                    Forum Moderation
                </a>

                <a href="AdminSettings.aspx"
                   class="sidebar-link active">
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

            <section class="settings-banner">

                <div class="banner-content">

                    <span class="banner-tag">
                        ACCOUNT MANAGEMENT
                    </span>

                    <h1>
                        Settings
                    </h1>

                    <p>
                        Manage your administrator account information and keep your profile details up to date.
                    </p>

                </div>

                <div class="banner-side">

                    <div class="banner-shape shape-one"></div>
                    <div class="banner-shape shape-two"></div>

                    <div class="banner-card">

                        <div class="banner-card-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <circle cx="12" cy="12" r="3"></circle>
                                <path d="M19.4 15a1.7 1.7 0 0 0 .34 1.88l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06A1.7 1.7 0 0 0 15 19.4a1.7 1.7 0 0 0-1 .6 1.7 1.7 0 0 0-.4 1.1V21a2 2 0 1 1-4 0v-.09A1.7 1.7 0 0 0 8.6 19.4a1.7 1.7 0 0 0-1.88.34l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06A1.7 1.7 0 0 0 4.6 15a1.7 1.7 0 0 0-.6-1 1.7 1.7 0 0 0-1.1-.4H3a2 2 0 1 1 0-4h.09A1.7 1.7 0 0 0 4.6 8.6a1.7 1.7 0 0 0-.34-1.88l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06A1.7 1.7 0 0 0 9 4.6a1.7 1.7 0 0 0 1-.6 1.7 1.7 0 0 0 .4-1.1V3a2 2 0 1 1 4 0v.09A1.7 1.7 0 0 0 15.4 4.6a1.7 1.7 0 0 0 1.88-.34l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06A1.7 1.7 0 0 0 19.4 9c.2.4.6.8 1 1 .3.2.7.3 1.1.3H21a2 2 0 1 1 0 4h-.09a1.7 1.7 0 0 0-1.51.7z"></path>
                            </svg>

                        </div>

                        <span class="banner-card-label">
                            ADMIN PROFILE
                        </span>

                        <strong>
                            Account Settings
                        </strong>

                        <span class="banner-card-sub">
                            Profile · Email · Account Details
                        </span>

                    </div>

                </div>

            </section>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="settings-layout">

                <section class="profile-card">

                    <div class="profile-summary">

                        <asp:Label
                            ID="lblInitial"
                            runat="server"
                            Text="A"
                            CssClass="profile-avatar">
                        </asp:Label>

                        <div class="profile-text">

                            <span class="section-label">
                                ADMIN PROFILE
                            </span>

                            <h2>
                                Account Information
                            </h2>

                            <p>
                                Review and update your personal account details.
                            </p>

                        </div>

                    </div>

                    <div class="account-note">

                        <div class="account-note-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                                <path d="M9 12l2 2 4-4"></path>
                            </svg>

                        </div>

                        <div>

                            <strong>
                                Administrator Account
                            </strong>

                            <span>
                                Username, role and account creation date are read-only.
                            </span>

                        </div>

                    </div>

                </section>

                <section class="settings-card">

                    <div class="card-header">

                        <div>

                            <span class="section-label">
                                PERSONAL DETAILS
                            </span>

                            <h2>
                                Profile Details
                            </h2>

                            <p>
                                You can update your full name and email address.
                            </p>

                        </div>

                        <div class="card-header-icon">

                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                <circle cx="12" cy="7" r="4"></circle>
                                <path d="M5 21v-2a7 7 0 0 1 14 0v2"></path>
                            </svg>

                        </div>

                    </div>

                    <div class="settings-form">

                        <div class="form-grid">

                            <div class="form-group">

                                <label>
                                    Username
                                </label>

                                <div class="input-wrapper readonly-wrapper">

                                    <div class="input-icon">

                                        <svg viewBox="0 0 24 24" aria-hidden="true">
                                            <circle cx="12" cy="7" r="4"></circle>
                                            <path d="M5 21v-2a7 7 0 0 1 14 0v2"></path>
                                        </svg>

                                    </div>

                                    <asp:TextBox
                                        ID="txtUsername"
                                        runat="server"
                                        CssClass="form-input readonly-input"
                                        ReadOnly="true">
                                    </asp:TextBox>

                                </div>

                                <span class="field-note">
                                    Username cannot be changed.
                                </span>

                            </div>

                            <div class="form-group">

                                <label>
                                    Role
                                </label>

                                <div class="input-wrapper readonly-wrapper">

                                    <div class="input-icon">

                                        <svg viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                                        </svg>

                                    </div>

                                    <asp:TextBox
                                        ID="txtRole"
                                        runat="server"
                                        CssClass="form-input readonly-input"
                                        ReadOnly="true">
                                    </asp:TextBox>

                                </div>

                                <span class="field-note">
                                    Your administrator role is system managed.
                                </span>

                            </div>

                            <div class="form-group">

                                <label>
                                    Full Name
                                </label>

                                <div class="input-wrapper">

                                    <div class="input-icon">

                                        <svg viewBox="0 0 24 24" aria-hidden="true">
                                            <circle cx="12" cy="7" r="4"></circle>
                                            <path d="M5 21v-2a7 7 0 0 1 14 0v2"></path>
                                        </svg>

                                    </div>

                                    <asp:TextBox
                                        ID="txtFullName"
                                        runat="server"
                                        CssClass="form-input"
                                        MaxLength="100"
                                        placeholder="Enter your full name">
                                    </asp:TextBox>

                                </div>

                            </div>

                            <div class="form-group">

                                <label>
                                    Email Address
                                </label>

                                <div class="input-wrapper">

                                    <div class="input-icon">

                                        <svg viewBox="0 0 24 24" aria-hidden="true">
                                            <rect x="3" y="5" width="18" height="14" rx="2"></rect>
                                            <path d="M3 7l9 6 9-6"></path>
                                        </svg>

                                    </div>

                                    <asp:TextBox
                                        ID="txtEmail"
                                        runat="server"
                                        CssClass="form-input"
                                        TextMode="Email"
                                        MaxLength="150"
                                        placeholder="Enter your email address">
                                    </asp:TextBox>

                                </div>

                            </div>

                            <div class="form-group account-created-group">

                                <label>
                                    Account Created
                                </label>

                                <div class="input-wrapper readonly-wrapper">

                                    <div class="input-icon">

                                        <svg viewBox="0 0 24 24" aria-hidden="true">
                                            <rect x="3" y="5" width="18" height="16" rx="2"></rect>
                                            <path d="M16 3v4"></path>
                                            <path d="M8 3v4"></path>
                                            <path d="M3 10h18"></path>
                                        </svg>

                                    </div>

                                    <asp:TextBox
                                        ID="txtCreatedAt"
                                        runat="server"
                                        CssClass="form-input readonly-input"
                                        ReadOnly="true">
                                    </asp:TextBox>

                                </div>

                            </div>

                        </div>

                        <div class="settings-actions">

                            <div class="save-note">

                                <svg viewBox="0 0 24 24" aria-hidden="true">
                                    <circle cx="12" cy="12" r="9"></circle>
                                    <path d="M12 8v4"></path>
                                    <path d="M12 16h.01"></path>
                                </svg>

                                <span>
                                    Changes only affect your administrator profile.
                                </span>

                            </div>

                            <asp:Button
                                ID="btnSave"
                                runat="server"
                                Text="Save Changes"
                                CssClass="save-btn"
                                OnClick="btnSave_Click" />

                        </div>

                    </div>

                </section>

            </div>

        </main>

    </div>

</form>

</body>

</html>