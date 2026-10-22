using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace The_Analytical_Dojo
{
    public partial class AskQuestion : DojoPage
    {
        private string HistoryKey
        {
            get { return "DojoAiChatHistory_" + UserId; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TutorState.Text = DojoAi.IsConfigured
                ? "<span class='dojo-chat-status dojo-chat-status-on'>AI ready</span>"
                : "<span class='dojo-chat-status dojo-chat-status-off'>Add API key</span>";
            Send.Enabled = DojoAi.IsConfigured;
            RenderConversation();
        }

        protected void Send_Click(object sender, EventArgs e)
        {
            string question = (Question.Text ?? string.Empty).Trim();
            if (question.Length < 5 || question.Length > 1000)
            {
                Show("Write a question between 5 and 1,000 characters.");
                return;
            }
            if (!DojoAi.IsConfigured)
            {
                Show("The API key is not available to the web server yet.");
                return;
            }

            DateTime last = Session["LastAiRequestAt"] is DateTime
                ? (DateTime)Session["LastAiRequestAt"] : DateTime.MinValue;
            if ((DateTime.UtcNow - last).TotalSeconds < 5)
            {
                Show("Please wait a few seconds before sending another message.");
                return;
            }

            List<DojoChatTurn> history = GetHistory();
            Session["LastAiRequestAt"] = DateTime.UtcNow;
            try
            {
                string answer = DojoAi.Answer(question, history);
                history.Add(new DojoChatTurn { Role = "user", Text = question });
                history.Add(new DojoChatTurn { Role = "model", Text = answer });
                if (history.Count > 16) history.RemoveRange(0, history.Count - 16);
                Session[HistoryKey] = history;
                Question.Text = string.Empty;
                Response.Redirect("AskQuestion.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                int code = response == null ? 0 : (int)response.StatusCode;
                if (code == 401 || code == 403)
                    Show("Google found the request but rejected the API key. Check the key in Google AI Studio.");
                else if (code == 429)
                    Show("The Gemini API rate limit was reached. Please try again shortly.");
                else if (code == 404)
                    Show("The Gemini model is unavailable for this API key. Check model access in Google AI Studio.");
                else
                    Show("The AI chatbot could not connect right now. Please try again.");
            }
            catch (InvalidOperationException)
            {
                Show("The AI chatbot did not return an answer. Please try again.");
            }
            catch (Newtonsoft.Json.JsonException)
            {
                Show("The AI chatbot returned an unexpected response. Please try again.");
            }
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            Session.Remove(HistoryKey);
            Question.Text = string.Empty;
            RenderConversation();
        }

        private List<DojoChatTurn> GetHistory()
        {
            List<DojoChatTurn> history = Session[HistoryKey] as List<DojoChatTurn>;
            if (history == null)
            {
                history = new List<DojoChatTurn>();
                Session[HistoryKey] = history;
            }
            return history;
        }

        private void RenderConversation()
        {
            List<DojoChatTurn> history = GetHistory();
            if (history.Count == 0)
            {
                Conversation.Text = "<div class='dojo-chat-empty'><span class='dojo-chat-empty-mark' aria-hidden='true'>D</span>" +
                    "<h2>What can I help you explore?</h2>" +
                    "<p>Ask about SQL, Excel, statistics, visualization, Python, machine learning, or any other data analytics topic.</p>" +
                    "<div class='dojo-chat-suggestions'>" +
                    "<button type='button' data-chat-prompt='Explain data preprocessing with a simple example.'>Explain data preprocessing</button>" +
                    "<button type='button' data-chat-prompt='When should I use a SQL JOIN in data analysis?'>Help me with SQL joins</button>" +
                    "<button type='button' data-chat-prompt='How do I choose the right chart for my data?'>Choose the right chart</button>" +
                    "</div></div>";
                return;
            }

            StringBuilder html = new StringBuilder();
            foreach (DojoChatTurn turn in history)
            {
                bool fromBot = turn.Role == "model";
                html.Append("<article class='dojo-chat-turn ");
                html.Append(fromBot ? "dojo-chat-turn-ai" : "dojo-chat-turn-user");
                html.Append("'><span class='dojo-chat-avatar' aria-hidden='true'>");
                html.Append(fromBot ? "D" : "Y");
                html.Append("</span><div class='dojo-chat-bubble'><strong>");
                html.Append(fromBot ? "Dojo AI" : "You");
                html.Append("</strong><p>");
                html.Append(H(turn.Text).Replace("\r\n", "<br />").Replace("\n", "<br />"));
                html.Append("</p></div></article>");
            }
            Conversation.Text = html.ToString();
        }

        private void Show(string message)
        {
            Status.Visible = true;
            Status.Text = H(message);
        }
    }
}
