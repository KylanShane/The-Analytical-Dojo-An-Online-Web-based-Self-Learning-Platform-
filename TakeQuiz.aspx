<%@ Page Title="Take Quiz | The Analytical Dojo" Language="C#" AutoEventWireup="true" CodeBehind="TakeQuiz.aspx.cs" Inherits="The_Analytical_Dojo.TakeQuiz" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>Quiz Focus Mode | The Analytical Dojo</title>

    <link
        href="Content/dojo-theme.css"
        rel="stylesheet" />

    <script src="Scripts/theme.js"></script>

</head>

<body class="dojo">

    <form id="form1" runat="server">

        <!-- Required by the shared DojoPage validation -->
        <input
            type="hidden"
            name="dojoCsrf"
            value="<%= H(Csrf) %>" />

        <main class="quiz-wrap">

            <div class="focus-top">

                <span class="pill">
                    ✦ FOCUS MODE
                </span>

                <a
                    class="text-link"
                    href="CoursePage.aspx?id=<%= CourseId %>"
                    onclick="return confirm('Leave this quiz? Your unsubmitted answers will not be saved.');">
                    Leave quiz
                </a>

            </div>

            <!-- Quiz title and description -->
            <asp:Literal
                ID="Intro"
                runat="server" />

            <p class="muted">
                Answer every question, then submit when you are ready.
                Answers are saved on submission.
            </p>

            <asp:Label
                ID="ErrorMessage"
                runat="server"
                CssClass="notice"
                role="alert"
                Visible="false" />

            <!-- Questions are created in the code-behind -->
            <asp:PlaceHolder
                ID="Questions"
                runat="server" />

            <asp:Button
                ID="Submit"
                runat="server"
                CssClass="btn"
                Text="Submit my answers →"
                OnClick="Submit_Click" />

        </main>

    </form>

</body>
</html>
