<%@ Page Title="Course forum | The Analytical Dojo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CourseForum.aspx.cs" Inherits="The_Analytical_Dojo.CourseForum" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="Main" runat="server">
    <div class="learning-back"><a href="CoursePage.aspx?id=<%= CourseId %>">← Back to course</a></div>

    <section class="learning-hero forum-hero">
        <div>
            <span class="learning-kicker">COURSE COMMUNITY</span>
            <h1><asp:Literal ID="CourseTitle" runat="server" /> forum<span class="yellow">.</span></h1>
            <p>Share an idea, ask your classmates, and learn from different approaches. This discussion belongs to this course.</p>
            <div class="learning-hero-actions">
                <a class="btn" href="#new-post">Start a discussion</a>
            </div>
        </div>
        <div class="forum-hero-art" aria-hidden="true"><span>01</span><span>↗</span><strong>Think aloud.<br />Learn together.</strong></div>
    </section>

    <asp:Label ID="Status" runat="server" CssClass="notice" Visible="false" role="status" />

    <div class="forum-layout">
        <div>
            <section class="panel forum-compose" id="new-post">
                <div class="section-head"><div><span class="learning-kicker">YOUR VOICE</span><h2>Start a discussion</h2></div><span class="forum-compose-mark" aria-hidden="true">✳</span></div>
                <p class="muted">Keep it connected to the course. Give enough detail so classmates can join in.</p>
                <asp:Label runat="server" AssociatedControlID="PostContent" Text="What would you like to discuss?" CssClass="forum-label" />
                <asp:TextBox ID="PostContent" runat="server" CssClass="input forum-textarea" TextMode="MultiLine" Rows="5" MaxLength="4000" placeholder="For example: When should we clean data in the analytics workflow?" />
                <div class="forum-compose-footer"><small>8–4,000 characters</small><asp:Button ID="Post" runat="server" CssClass="btn" Text="Publish discussion ↗" OnClick="Post_Click" /></div>
            </section>

            <section class="forum-feed" aria-labelledby="discussion-heading">
                <div class="section-head"><div><span class="learning-kicker">THE CONVERSATION</span><h2 id="discussion-heading">Course discussions <asp:Literal ID="PostCount" runat="server" /></h2></div><a class="text-link" href="CourseForum.aspx?course=<%= CourseId %>">Latest first ↻</a></div>
                <label class="forum-search-label" for="forumSearch">Find a discussion</label><input id="forumSearch" class="input forum-search" type="search" placeholder="Search this course’s discussions" autocomplete="off" /><p id="forumSearchEmpty" class="empty" hidden>No discussions match that search.</p>
                <asp:Literal ID="Feed" runat="server" />

                <!-- One reply box, moved under the post or reply being answered (Scripts/dojo.js) -->
                <div class="reply-composer" data-reply-composer hidden>
                    <label class="reply-composer-label" for="replyContent">Replying to <strong data-reply-name></strong></label>
                    <textarea id="replyContent" name="replyContent" class="input" rows="3" maxlength="2000" placeholder="Add to the conversation…"></textarea>
                    <input type="hidden" name="replyPost" data-reply-post />
                    <input type="hidden" name="replyParent" data-reply-parent />
                    <div class="reply-composer-footer">
                        <small><span data-reply-chars>0</span> / 2,000 · Ctrl + Enter to send</small>
                        <span>
                            <button type="button" class="btn ghost" data-reply-cancel>Cancel</button>
                            <button type="submit" class="btn" name="sendReply" value="1" data-reply-send disabled>Reply ↩</button>
                        </span>
                    </div>
                </div>
            </section>
        </div>
        <aside class="forum-side">
            <div class="panel forum-side-card"><span class="learning-kicker">HOW IT WORKS</span><h3>Make learning social</h3><ol><li>Bring a question or insight from this course.</li><li>Read what others are exploring.</li><li>Return to the course materials when you need context.</li></ol></div>
        </aside>
    </div>
</asp:Content>
