<%@ Page Title="SQL Playground | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="SqlLab.aspx.cs" Inherits="The_Analytical_Dojo.SqlLab" %>

<asp:Content
    ID="PageContent"
    ContentPlaceHolderID="Main"
    runat="server">

    <div class="eyebrow">
        LEARN BY DOING
    </div>

    <h1>
        Your data. Your queries<span class="yellow">.</span>
    </h1>

    <p class="muted">
        Practise SQL using your submitted objective quiz results.
    </p>

    <div class="two-col sql-layout">

        <!-- SQL editor and results -->
        <section class="panel">

            <div class="section-head">

                <h2>Query your own results</h2>

                <span class="pill">
                    READ ONLY
                </span>

            </div>

            <asp:Label
                ID="QueryLabel"
                runat="server"
                AssociatedControlID="QueryText"
                CssClass="muted"
                Text="SQL editor" />

            <asp:TextBox
                ID="QueryText"
                runat="server"
                TextMode="MultiLine"
                Rows="8"
                CssClass="sql-editor" />

            <div class="sql-actions">

                <asp:Button
                    ID="Run"
                    runat="server"
                    Text="▷ Run query"
                    CssClass="btn"
                    OnClick="Run_Click" />

                <button
                    type="button"
                    class="btn ghost"
                    data-query-reset>
                    Reset query
                </button>

            </div>

            <asp:Label
                ID="Status"
                runat="server"
                CssClass="notice"
                role="status"
                aria-live="polite"
                Visible="false" />

            <asp:Literal
                ID="Results"
                runat="server" />

        </section>

        <!-- Instructions and starter queries -->
        <aside class="panel">

            <h3>Your practice table</h3>

            <p>
                <code>my_quiz_results</code>
            </p>

            <p class="muted">
                course · quiz · score
            </p>

            <p class="muted">
                Score is a percentage. Each submitted objective
                attempt is one row. Reflections are excluded.
            </p>

            <h3>Try a starter</h3>

            <button
                type="button"
                class="query-example"
                data-example="average">
                01 / Average score by course ↗
            </button>

            <button
                type="button"
                class="query-example"
                data-example="all">
                02 / Explore every attempt ↗
            </button>

            <button
                type="button"
                class="query-example"
                data-example="filter">
                03 / Find scores of 80% or more ↗
            </button>

            <details>

                <summary>Supported SQL</summary>

                <p class="muted">
                    Use SELECT * or a comma-separated list of
                    course, quiz and score FROM my_quiz_results.
                </p>

                <p class="muted">
                    Optionally add WHERE score &gt;= 80.
                    You can also use =, &gt;, &lt; and &lt;=.
                </p>

                <p class="muted">
                    You can sort using ORDER BY score ASC or DESC.
                    The average-by-course starter is also supported.
                </p>

                <p class="muted">
                    This is a guided SQL playground with limited
                    syntax, not a full SQL engine.
                </p>

            </details>

        </aside>

    </div>

</asp:Content>