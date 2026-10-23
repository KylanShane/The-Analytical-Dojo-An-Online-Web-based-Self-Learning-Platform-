<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminCreateMaterial.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminCreateMaterial" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Create New Material - The Analytical Dojo</title>

    <link href="Content/AdminCreateMaterial.css?v=1" rel="stylesheet" />

</head>

<body>

<form id="form1" runat="server" enctype="multipart/form-data">

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

                <div class="breadcrumb">
                    <a href="AdminMaterial.aspx">Manage Materials</a>
                    <span>&rsaquo;</span>
                    <span class="current"><asp:Literal ID="litBreadcrumb" runat="server" Text="Create New Material"></asp:Literal></span>
                </div>

                <h1><asp:Literal ID="litHeading" runat="server" Text="Create New Material"></asp:Literal></h1>

                <p>
                    <asp:Literal ID="litSubtitle" runat="server" Text="Add a slide deck or video to a chapter of a course."></asp:Literal>
                </p>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="form-card">

                <h2>Material Information</h2>

                <p class="card-subtitle">
                    Choose where the material belongs, then describe it and upload the file.
                </p>

                <div class="form-row">

                    <div class="form-group">

                        <label>Course <span class="required">*</span></label>

                        <asp:DropDownList
                            ID="ddlCourse"
                            runat="server"
                            CssClass="form-input"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged">
                        </asp:DropDownList>

                        <asp:RequiredFieldValidator
                            ID="rfvCourse"
                            runat="server"
                            ControlToValidate="ddlCourse"
                            InitialValue="0"
                            ErrorMessage="Please select a course."
                            ValidationGroup="Material"
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                    </div>

                    <div class="form-group">

                        <label>Chapter <span class="required">*</span></label>

                        <asp:DropDownList
                            ID="ddlChapter"
                            runat="server"
                            CssClass="form-input"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlChapter_SelectedIndexChanged">
                        </asp:DropDownList>

                        <asp:RequiredFieldValidator
                            ID="rfvChapter"
                            runat="server"
                            ControlToValidate="ddlChapter"
                            InitialValue="0"
                            ErrorMessage="Please select a chapter."
                            ValidationGroup="Material"
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                    </div>

                </div>

                <asp:Panel
                    ID="pnlNewChapter"
                    runat="server"
                    CssClass="new-chapter-box"
                    Visible="false">

                    <div class="form-group">

                        <label>New Chapter Title <span class="required">*</span></label>

                        <asp:TextBox
                            ID="txtChapterTitle"
                            runat="server"
                            CssClass="form-input"
                            MaxLength="100">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvChapterTitle"
                            runat="server"
                            ControlToValidate="txtChapterTitle"
                            ErrorMessage="Chapter title is required."
                            ValidationGroup="Material"
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                        <span class="field-hint">
                            <span class="char-left">100</span> characters remaining.
                        </span>

                    </div>

                    <div class="form-group">

                        <label>New Chapter Description <span class="required">*</span></label>

                        <asp:TextBox
                            ID="txtChapterDescription"
                            runat="server"
                            CssClass="form-input form-textarea"
                            TextMode="MultiLine"
                            Rows="3">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvChapterDescription"
                            runat="server"
                            ControlToValidate="txtChapterDescription"
                            ErrorMessage="Chapter description is required."
                            ValidationGroup="Material"
                            CssClass="validation-message"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>

                    </div>

                </asp:Panel>

                <div class="form-group">

                    <label>Material Title <span class="required">*</span></label>

                    <asp:TextBox
                        ID="txtTitle"
                        runat="server"
                        CssClass="form-input"
                        MaxLength="100"
                        placeholder="e.g. Data Types - Slides">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvTitle"
                        runat="server"
                        ControlToValidate="txtTitle"
                        ErrorMessage="Material title is required."
                        ValidationGroup="Material"
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                    <span class="field-hint">
                        Use a clear, specific title.
                        <span class="char-left">100</span> characters remaining.
                    </span>

                </div>

                <div class="form-group">

                    <label>Description <span class="required">*</span></label>

                    <asp:TextBox
                        ID="txtDescription"
                        runat="server"
                        CssClass="form-input form-textarea"
                        TextMode="MultiLine"
                        Rows="5">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvDescription"
                        runat="server"
                        ControlToValidate="txtDescription"
                        ErrorMessage="Description is required."
                        ValidationGroup="Material"
                        CssClass="validation-message"
                        Display="Dynamic">
                    </asp:RequiredFieldValidator>

                    <span class="field-hint">
                        Summarize what learners will get from this material.
                    </span>

                </div>

                <div class="form-group">

                    <label>Material Type <span class="required">*</span></label>

                    <asp:RadioButtonList
                        ID="rblType"
                        runat="server"
                        CssClass="segmented material-type"
                        RepeatLayout="Flow"
                        RepeatDirection="Horizontal">

                        <asp:ListItem Text="PPT" Value="PPT" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Video" Value="VIDEO"></asp:ListItem>

                    </asp:RadioButtonList>

                </div>

                <div class="dropzone"
                     data-existing="<%: ExistingFileName %>"
                     data-orig-type="<%: ExistingFileType %>">

                    <asp:FileUpload
                        ID="fuFile"
                        runat="server"
                        CssClass="dz-input" />

                    <svg width="44" height="44" viewBox="0 0 24 24" fill="none" stroke="#777895" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round">
                        <path d="M7 18a4 4 0 0 1-.5-7.97A5.5 5.5 0 0 1 17 8.5a4.5 4.5 0 0 1 0 9.5" />
                        <path d="M12 21v-8m0 0-3 3m3-3 3 3" />
                    </svg>

                    <div class="dz-title">Select a file or drag and drop here</div>

                    <div class="dz-hint">PPT, PPTX or PDF, file size no more than 50MB</div>

                    <div class="dz-file"></div>

                    <button type="button" class="dz-select">SELECT FILE</button>

                </div>

                <span class="field-hint">
                    Select the course and chapter first. Changing them reloads the page and clears a chosen file.
                </span>

            </div>

            <div class="action-bar">

                <span class="action-note">
                    <asp:Literal ID="litActionNote" runat="server" Text="NEW MATERIAL &amp;middot; UNSAVED CHANGES"></asp:Literal>
                </span>

                <div class="action-buttons">

                    <asp:HyperLink
                        ID="lnkCancel"
                        runat="server"
                        NavigateUrl="~/AdminMaterial.aspx"
                        CssClass="btn btn-outline">
                        Cancel
                    </asp:HyperLink>

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Create Material"
                        CssClass="btn btn-primary"
                        ValidationGroup="Material"
                        OnClick="btnSave_Click" />

                </div>

            </div>

        </main>

    </div>

</form>

<script>
    (function () {
        var MAX_BYTES = 50 * 1024 * 1024;
        var TYPES = {
            PPT:   { accept: '.ppt,.pptx,.pdf', exts: ['ppt', 'pptx', 'pdf'], label: 'PPT, PPTX or PDF' },
            VIDEO: { accept: '.mp4,.webm,.mov', exts: ['mp4', 'webm', 'mov'], label: 'MP4, WEBM or MOV' }
        };

    var typeName = '<%= rblType.UniqueID %>';
    var dz = document.querySelector('.dropzone');
    var input = dz.querySelector('.dz-input');
    var msg = dz.querySelector('.dz-file');
    var hint = dz.querySelector('.dz-hint');

    function currentType() {
        var r = document.querySelector('input[name="' + typeName + '"]:checked');
        return r ? r.value : 'PPT';
    }

    function setMessage(text, isError) {
        msg.textContent = text;
        msg.classList.toggle('dz-bad', !!isError);
        dz.classList.toggle('dz-error', !!isError);
    }

    function sameTypeAsStored() {
        return dz.getAttribute('data-existing') &&
            currentType() === dz.getAttribute('data-orig-type');
    }

    function showExisting() {
        if (input.files.length) { return; }
        if (sameTypeAsStored()) {
            setMessage('Current file: ' + dz.getAttribute('data-existing') + ' (select a file to replace it)', false);
        } else {
            setMessage('', false);
        }
    }

    function applyType(clear) {
        var t = TYPES[currentType()];
        input.accept = t.accept;
        hint.textContent = t.label + ', file size no more than 50MB';
        if (clear) { input.value = ''; showExisting(); }
    }

    function check() {
        if (!input.files.length) { showExisting(); return; }
        var f = input.files[0];
        var ext = f.name.split('.').pop().toLowerCase();
        if (TYPES[currentType()].exts.indexOf(ext) === -1) {
            input.value = '';
            setMessage('Unsupported file type. Use ' + TYPES[currentType()].label + '.', true);
        } else if (f.size > MAX_BYTES) {
            input.value = '';
            setMessage('File is larger than 50MB.', true);
        } else {
            setMessage(f.name, false);
        }
    }

    function updateCount(el) {
        var group = el.closest('.form-group');
        var c = group && group.querySelector('.char-left');
        if (c && el.maxLength > 0) { c.textContent = el.maxLength - el.value.length; }
    }

    document.addEventListener('change', function (e) {
        if (e.target.name === typeName) { applyType(true); }
        if (e.target === input) { check(); }
    });

    document.addEventListener('input', function (e) {
        if (e.target.matches('input[maxlength]')) { updateCount(e.target); }
    });

    dz.querySelector('.dz-select').addEventListener('click', function () { input.click(); });

    dz.addEventListener('dragover', function (e) { e.preventDefault(); dz.classList.add('dz-over'); });
    dz.addEventListener('dragleave', function () { dz.classList.remove('dz-over'); });
    dz.addEventListener('drop', function (e) {
        e.preventDefault();
        dz.classList.remove('dz-over');
        if (e.dataTransfer.files.length) { input.files = e.dataTransfer.files; check(); }
    });

    document.getElementById('form1').addEventListener('submit', function (e) {
        if (!input.files.length && !sameTypeAsStored()) {
            setMessage('Please select a file for this material.', true);
            e.preventDefault();
            dz.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    });

    document.querySelectorAll('input[maxlength]').forEach(updateCount);
    applyType(false);
    check();
})();
</script>

</body>

</html>
