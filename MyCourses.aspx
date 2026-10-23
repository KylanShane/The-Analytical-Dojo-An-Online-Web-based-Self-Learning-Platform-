<%@ Page
    Title="My Courses | The Analytical Dojo"
    Language="C#"
    MasterPageFile="~/Site.master"
    AutoEventWireup="true"
    CodeBehind="MyCourses.aspx.cs"
    Inherits="The_Analytical_Dojo.MyCourses"
%>

<asp:Content
    ID="PageContent"
    ContentPlaceHolderID="Main"
    runat="server">

    <div class="eyebrow">
        YOUR PERSONAL CURRICULUM
    </div>

    <h1>
        My courses<span class="yellow">.</span>
    </h1>

    <p class="muted">
        Build your skills, one chapter at a time.
    </p>

    <!-- Search and course filters -->
    <div class="filterbar">

        <label class="search-label" for="courseSearch">
            Search courses

            <input
                id="courseSearch"
                type="search"
                placeholder="Search by course title…" />
        </label>

        <div
            class="segmented"
            role="group"
            aria-label="Course status">

            <button
                type="button"
                data-filter="all"
                aria-pressed="true">
                All courses
            </button>

            <button
                type="button"
                data-filter="progress"
                aria-pressed="false">
                In progress
            </button>

            <button
                type="button"
                data-filter="complete"
                aria-pressed="false">
                Completed
            </button>

        </div>

    </div>

    <!-- Course cards loaded from the database -->
    <div class="course-grid">

        <asp:Literal
            ID="Cards"
            runat="server" />

    </div>

    <!-- Shown when search or filters have no matches -->
    <p
        id="noMatches"
        class="empty"
        hidden>
        No courses match your search.
    </p>

</asp:Content>
