<%@ Page Title="Ask Question | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AskQuestion.aspx.cs" Inherits="The_Analytical_Dojo.AskQuestion" ResponseEncoding="utf-8" %>
<asp:Content ID="ChatHead" ContentPlaceHolderID="Head" runat="server">
    <link href="<%= ResolveUrl("~/Content/chat.css") %>?v=20261014b" rel="stylesheet" />
</asp:Content>
<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">
    <div class="dojo-chat">
        <header class="dojo-chat-bar">
            <div class="dojo-chat-identity">
                <span class="dojo-chat-mark" aria-hidden="true">D</span>
                <div><span class="dojo-chat-eyebrow">THE ANALYTICAL DOJO</span><h1>Dojo AI</h1></div>
            </div>
            <div class="dojo-chat-actions">
                <asp:Literal ID="TutorState" runat="server" />
                <asp:Button ID="Clear" runat="server" CssClass="dojo-chat-new" Text="New chat" OnClick="Clear_Click" />
            </div>
        </header>

        <section class="dojo-chat-frame" aria-label="Data analytics chatbot">
            <div class="dojo-chat-feed" data-chat-window aria-live="polite">
                <asp:Literal ID="Conversation" runat="server" />
                <span id="chat-bottom"></span>
            </div>
            <div class="dojo-chat-composer">
                <asp:Label ID="Status" runat="server" CssClass="dojo-chat-error" Visible="false" role="status" />
                <asp:Label runat="server" AssociatedControlID="Question" CssClass="dojo-chat-label" Text="Message Dojo AI" />
                <div class="dojo-chat-input-wrap">
                    <asp:TextBox ID="Question" runat="server" CssClass="dojo-chat-input" TextMode="MultiLine" Rows="2" MaxLength="1000" placeholder="Ask anything about data analytics..." data-chat-input="true" />
                    <asp:Button ID="Send" runat="server" CssClass="dojo-chat-send" Text="Send" aria-label="Send message" OnClick="Send_Click" />
                </div>
                <p class="dojo-chat-hint">Enter to send. Shift + Enter for a new line. AI can make mistakes.</p>
            </div>
        </section>
    </div>
</asp:Content>
