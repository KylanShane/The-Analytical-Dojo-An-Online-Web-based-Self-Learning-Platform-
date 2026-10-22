<%@ Page Title="Take Quiz | The Analytical Dojo" Language="C#" AutoEventWireup="true" CodeBehind="TakeQuiz.aspx.cs" Inherits="The_Analytical_Dojo.TakeQuiz" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">

    <meta charset="utf-8" />

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <title>Quiz Focus Mode | The Analytical Dojo</title>

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600;700&amp;family=Space+Grotesk:wght@400;500;600;700&amp;display=swap" rel="stylesheet" />

    <link
        href="Content/dojo-theme.css?v=20261022"
        rel="stylesheet" />

    <script src="Scripts/theme.js"></script>

</head>

<body class="dojo quiz-page">

    <form id="form1" runat="server">

        <!-- Required by the shared DojoPage validation -->
        <input
            type="hidden"
            name="dojoCsrf"
            value="<%= H(Csrf) %>" />

        <!-- Sticky quiz HUD: progress, timer, exit -->
        <header class="quiz-hud" data-quiz-hud>
            <div class="quiz-hud-inner">
                <img class="quiz-hud-logo" data-logo src="Content/img/logo-dark.png" alt="The Analytical Dojo" />
                <div class="quiz-hud-progress">
                    <div class="quiz-hud-meta">
                        <span data-quiz-count>0 / 0 answered</span>
                        <span class="quiz-timer" data-quiz-timer aria-label="Time elapsed">00:00</span>
                    </div>
                    <div class="quiz-meter"><span data-quiz-meter></span></div>
                </div>
                <a
                    class="quiz-leave"
                    href="CoursePage.aspx?id=<%= CourseId %>"
                    onclick="return confirm('Leave this quiz? Your unsubmitted answers will not be saved.');">
                    ✕ <span>Leave</span>
                </a>
            </div>
        </header>

        <main class="quiz-wrap">

            <section class="quiz-intro">
                <span class="pill">✦ FOCUS MODE</span>

                <!-- Quiz title and description -->
                <asp:Literal
                    ID="Intro"
                    runat="server" />

                <p class="quiz-tip">
                    Pick an answer to lock it in. Use <kbd>1</kbd>–<kbd>4</kbd> to choose,
                    <kbd>←</kbd> <kbd>→</kbd> to move between questions.
                </p>
            </section>

            <asp:Label
                ID="ErrorMessage"
                runat="server"
                CssClass="notice notice-error"
                role="alert"
                Visible="false" />

            <!-- Questions are created in the code-behind -->
            <div class="quiz-deck" data-quiz-deck>
                <asp:PlaceHolder
                    ID="Questions"
                    runat="server" />
            </div>

            <!-- Stepper controls (shown by Scripts/quiz.js) -->
            <nav class="quiz-stepper" data-quiz-stepper hidden aria-label="Question navigation">
                <button type="button" class="btn ghost" data-quiz-prev>← Back</button>
                <div class="quiz-dots" data-quiz-dots></div>
                <button type="button" class="btn" data-quiz-next>Next →</button>
            </nav>

            <div class="quiz-submit-row" data-quiz-submit-row>
                <asp:Button
                    ID="Submit"
                    runat="server"
                    CssClass="btn quiz-submit"
                    Text="Submit my answers →"
                    OnClick="Submit_Click" />
            </div>

        </main>

        <div class="quiz-toast" data-quiz-toast role="status" aria-live="polite"></div>

        <div class="quiz-grading" data-quiz-grading hidden>
            <div class="quiz-grading-card">
                <span class="quiz-spinner" aria-hidden="true"></span>
                <strong>Grading your answers…</strong>
                <small>Hold tight, sensei is checking.</small>
            </div>
        </div>

    </form>

    <script src="Scripts/quiz.js?v=20261022"></script>

</body>
</html>
