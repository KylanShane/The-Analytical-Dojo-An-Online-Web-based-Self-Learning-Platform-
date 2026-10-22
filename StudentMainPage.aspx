<%@ Page Title="Home | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentMainPage.aspx.cs" Inherits="The_Analytical_Dojo.StudentMainPage" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">

    <div class="hero-grid">

        <section class="welcome">
            <p class="muted">Welcome back, <%= H(Session["FullName"]) %></p>

            <asp:Literal ID="NextMaterial" runat="server" />

            <div class="hero-actions">
                <asp:HyperLink ID="ResumeLink" runat="server" CssClass="btn">
                    ▷ Resume learning
                </asp:HyperLink>
                <asp:HyperLink ID="AttemptLink" runat="server" CssClass="btn ghost">
                    Attempt a quiz
                </asp:HyperLink>
            </div>
        </section>

        <section class="panel activity">
            <div class="section-head">
                <div>
                    <h2>Your week in data</h2>
                </div>
                <div class="segmented" role="group" aria-label="Activity type">
                    <button type="button" data-metric="materials" aria-pressed="true">Materials</button>
                    <button type="button" data-metric="quizzes" aria-pressed="false">Quizzes</button>
                    <button type="button" data-metric="forum" aria-pressed="false">Ask Question</button>
                </div>
            </div>

            <p id="activitySummary" class="muted" aria-live="polite"></p>

            <div id="activityChart" class="bars" aria-label="Daily activity comparison"></div>

            <div class="chart-foot">
                <span id="dayDetail" aria-live="polite">Select a day to explore.</span>
                <span>■ This week <i>▧ Last week</i></span>
            </div>

            <div class="week-controls">
                <button type="button" class="text-btn" id="prevWeek">← Previous</button>
                <strong id="weekLabel"></strong>
                <button type="button" class="text-btn" id="nextWeek">Next →</button>
            </div>
        </section>
    </div>

    <div class="home-split">

        <div class="home-main">

            <section class="panel sql-panel">
                <div class="section-head">
                    <div>
                        <h2>Query your own results</h2>
                        <p class="muted">Run this SQL to see your average quiz score by course.</p>
                    </div>
                    <a class="btn" href="SqlLab.aspx">▷ Run query</a>
                </div>

                <asp:Label ID="QueryLabel" runat="server" AssociatedControlID="QueryText" CssClass="sr-only" Text="SQL editor" />

                <asp:TextBox ID="QueryText" runat="server" TextMode="MultiLine" Rows="5" CssClass="sql-editor" />

                <div class="sql-actions">
                    <asp:Button ID="RunQuery" runat="server" Text="▷ Run query" CssClass="btn" OnClick="RunQuery_Click" />
                    <button type="button" class="btn ghost" data-query-reset>Reset query</button>
                </div>

                <asp:Label ID="QueryStatus" runat="server" CssClass="notice" role="status" aria-live="polite" Visible="false" />
                <asp:Literal ID="QueryResult" runat="server" />
            </section>

            <section>
                <div class="section-head">
                    <div>
                        <h2>My courses</h2>
                        <p class="muted">Progress counts finished materials and passed MCQ / true-false quizzes. Subjective answers have no mark.</p>
                    </div>
                    <div class="segmented" role="group" aria-label="Course status">
                        <button type="button" data-filter="all" aria-pressed="true">All</button>
                        <button type="button" data-filter="progress" aria-pressed="false">In progress</button>
                        <button type="button" data-filter="complete" aria-pressed="false">Completed</button>
                    </div>
                </div>

                <div class="course-list">
                    <asp:Literal ID="Courses" runat="server" />
                </div>
                <p id="noMatches" class="empty" hidden>No courses match this filter.</p>
            </section>
        </div>

        <aside class="home-side">

            <section class="panel" id="achievements">
                <h2>Your badges</h2>
                <asp:Literal ID="Badges" runat="server" />
            </section>

            <section class="panel" id="leaderboard">
                <h2>Quiz leaderboard</h2>
                <asp:DropDownList
                    ID="QuizPick"
                    runat="server"
                    CssClass="input leaderboard-select"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="QuizPick_SelectedIndexChanged" />
                <asp:Literal ID="LeaderboardList" runat="server" />
            </section>

        </aside>
    </div>

    <script type="application/json" id="activityData">
        <asp:Literal ID="ActivityJson" runat="server" />
    </script>

</asp:Content>
