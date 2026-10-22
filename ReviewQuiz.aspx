<%@ Page Title="Review Quiz | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ReviewQuiz.aspx.cs" Inherits="The_Analytical_Dojo.ReviewQuiz" %>

<asp:Content
    ID="PageContent"
    ContentPlaceHolderID="Main"
    runat="server">

    <div class="eyebrow">
        REFLECT. UNDERSTAND. IMPROVE.
    </div>

    <h1>
        Review your quizzes<span class="yellow">.</span>
    </h1>

    <p class="muted">
        Every attempt is a chance to learn something new.
    </p>

    <!-- Search controls or selected attempt summary -->
    <asp:Literal
        ID="Summary"
        runat="server" />

    <!-- Submitted quiz attempts -->
    <asp:Literal
        ID="History"
        runat="server" />

    <!-- Answers for the selected attempt -->
    <asp:Literal
        ID="Answers"
        runat="server" />

</asp:Content>