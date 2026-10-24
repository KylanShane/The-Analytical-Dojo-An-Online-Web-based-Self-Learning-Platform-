<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminCreateQuiz.aspx.cs"
    Inherits="The_Analytical_Dojo.AdminCreateQuiz" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Create New Quiz - The Analytical Dojo</title>

    <link href="Content/AdminCreateQuiz.css?v=1" rel="stylesheet" />

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

                <a href="AdminForum.aspx" class="sidebar-link">
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
                    <a href="AdminQuiz.aspx">Quiz Management</a>
                    <span>&rsaquo;</span>
                    <span class="current"><asp:Literal ID="litBreadcrumb" runat="server" Text="Create New Quiz"></asp:Literal></span>
                </div>

                <h1><asp:Literal ID="litHeading" runat="server" Text="Question Builder"></asp:Literal></h1>

                <p>
                    <asp:Literal ID="litSubtitle" runat="server" Text="Create an assessment for a chapter and build its questions."></asp:Literal>
                </p>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                Visible="false">
            </asp:Label>

            <div class="builder-card">

                <div class="builder-head">

                    <h2>Question Builder</h2>

                    <span class="builder-tag" id="questionSummary">0 QUESTIONS</span>

                </div>

                <p class="card-subtitle">
                    Objective quizzes use MCQ and True / False questions. Subjective quizzes use written questions that are marked manually.
                </p>

                <div class="builder-grid">

                    <div class="quiz-panel">

                        <h3>Quiz details</h3>

                        <div class="form-group">

                            <label>Quiz Title <span class="required">*</span></label>

                            <asp:TextBox
                                ID="txtTitle"
                                runat="server"
                                CssClass="form-input"
                                MaxLength="100">
                            </asp:TextBox>

                            <asp:RequiredFieldValidator
                                ID="rfvTitle"
                                runat="server"
                                ControlToValidate="txtTitle"
                                ErrorMessage="Quiz title is required."
                                ValidationGroup="Quiz"
                                CssClass="validation-message"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                        </div>

                        <div class="form-group">

                            <label>Quiz Type <span class="required">*</span></label>

                            <asp:RadioButtonList
                                ID="rblQuizType"
                                runat="server"
                                CssClass="segmented"
                                RepeatLayout="Flow"
                                RepeatDirection="Horizontal">

                                <asp:ListItem Text="Objective" Value="OBJ" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Subjective" Value="SUBJ"></asp:ListItem>

                            </asp:RadioButtonList>

                        </div>

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
                                ValidationGroup="Quiz"
                                CssClass="validation-message"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                        </div>

                        <div class="form-group">

                            <label>Chapter <span class="required">*</span></label>

                            <asp:DropDownList
                                ID="ddlChapter"
                                runat="server"
                                CssClass="form-input">
                            </asp:DropDownList>

                            <asp:RequiredFieldValidator
                                ID="rfvChapter"
                                runat="server"
                                ControlToValidate="ddlChapter"
                                InitialValue="0"
                                ErrorMessage="Please select a chapter."
                                ValidationGroup="Quiz"
                                CssClass="validation-message"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                            <span class="field-hint">
                                A quiz belongs to a chapter. Add chapters from the Materials menu.
                            </span>

                        </div>

                        <div class="form-group">

                            <label>Description</label>

                            <asp:TextBox
                                ID="txtDescription"
                                runat="server"
                                CssClass="form-input form-textarea"
                                TextMode="MultiLine"
                                Rows="4">
                            </asp:TextBox>

                        </div>

                        <div class="form-group">

                            <label>Questions</label>

                            <div id="questionNav" class="q-nav"></div>

                        </div>

                        <button type="button" id="btnAddQuestion" class="btn-add">
                            Add Question
                        </button>

                    </div>

                    <div id="questionList"
                         class="question-column"
                         data-quiz-type="<%: CurrentQuizType %>">

                        <asp:Literal ID="litQuestions" runat="server"></asp:Literal>

                    </div>

                </div>

            </div>

            <template id="questionTemplate">
                <asp:Literal ID="litQuestionTemplate" runat="server"></asp:Literal>
            </template>

            <div class="action-bar">

                <span class="action-note">
                    <asp:Literal ID="litActionNote" runat="server" Text="NEW QUIZ &amp;middot; UNSAVED CHANGES"></asp:Literal>
                </span>

                <div class="action-buttons">

                    <asp:HyperLink
                        ID="lnkCancel"
                        runat="server"
                        NavigateUrl="~/AdminQuiz.aspx"
                        CssClass="btn btn-outline">
                        Cancel
                    </asp:HyperLink>

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Create Quiz"
                        CssClass="btn btn-primary"
                        ValidationGroup="Quiz"
                        OnClick="btnSave_Click" />

                </div>

            </div>

        </main>

    </div>

</form>

<script>
    (function () {
        var list = document.getElementById('questionList');
        var tpl = document.getElementById('questionTemplate');
        var nav = document.getElementById('questionNav');
        var summary = document.getElementById('questionSummary');
    var typeName = '<%= rblQuizType.UniqueID %>';
    var nextIdx = Date.now();

    function questions() { return list.querySelectorAll('.question'); }

    function quizType() {
        var r = document.querySelector('input[name="' + typeName + '"]:checked');
        return r && r.value === 'SUBJ' ? 'SUBJ' : 'OBJ';
    }

    function pad(n) { return n < 10 ? '0' + n : '' + n; }

    function refresh() {
        var all = questions();

        nav.innerHTML = '';

        all.forEach(function (q, i) {
            q.querySelector('.q-num').textContent = pad(i + 1);
            q.querySelector('.q-remove').style.display = all.length > 1 ? '' : 'none';

            var text = q.querySelector('.q-text').value.trim();
            var item = document.createElement('button');
            item.type = 'button';
            item.className = 'q-nav-item';
            item.setAttribute('data-target', i);

            var no = document.createElement('span');
            no.className = 'q-nav-no';
            no.textContent = pad(i + 1);

            var label = document.createElement('span');
            label.textContent = text ? (text.length > 34 ? text.substring(0, 34) + '...' : text) : 'Untitled question';

            item.appendChild(no);
            item.appendChild(label);
            nav.appendChild(item);
        });

        summary.textContent = all.length + (all.length === 1 ? ' QUESTION' : ' QUESTIONS');
    }

    document.addEventListener('change', function (e) {
        if (e.target.name === typeName) {
            list.setAttribute('data-quiz-type', quizType());
        }

        if (e.target.name && e.target.name.indexOf('q_fmt_') === 0) {
            e.target.closest('.question').setAttribute('data-format', e.target.value);
        }
    });

    document.addEventListener('input', function (e) {
        if (e.target.classList.contains('q-text')) { refresh(); }
    });

    document.addEventListener('click', function (e) {
        var remove = e.target.closest('.q-remove');

        if (remove) {
            if (confirm('Delete this question? This cannot be undone once the quiz is saved.')) {
                remove.closest('.question').remove();
                refresh();
            }
            return;
        }

        var navItem = e.target.closest('.q-nav-item');

        if (navItem) {
            var target = questions()[parseInt(navItem.getAttribute('data-target'), 10)];
            if (target) { target.scrollIntoView({ behavior: 'smooth', block: 'start' }); }
        }
    });

    document.getElementById('btnAddQuestion').addEventListener('click', function () {
        var html = tpl.innerHTML.replace(/__IDX__/g, nextIdx++).replace(/__NUM__/g, '0');
        list.insertAdjacentHTML('beforeend', html);
        refresh();

        var all = questions();
        var last = all[all.length - 1];
        last.scrollIntoView({ behavior: 'smooth', block: 'start' });
        last.querySelector('.q-text').focus();
    });

    list.setAttribute('data-quiz-type', quizType());
    refresh();
})();
</script>

</body>

</html>
