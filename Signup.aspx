<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="The_Analytical_Dojo.Signup" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>Sign up — The Analytical Dojo</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600;700&amp;family=Space+Grotesk:wght@400;500;600;700&amp;display=swap" rel="stylesheet" />
    <link href="Content/dojo-theme.css?v=20261022" rel="stylesheet" />
    <script src="Scripts/theme.js"></script>
    <style>
        .auth-card a { color: #4f46e5; font-weight: 600; text-decoration: underline; text-underline-offset: 3px; }
        .auth-card a:visited { color: #4f46e5; }
        .auth-card a:hover { opacity: .8; }
        :root[data-theme="dark"] .auth-card a,
        :root[data-theme="dark"] .auth-card a:visited { color: #f8c030; }

        .field-hint { display: block; margin-top: 6px; font-size: .8rem; opacity: .65; }
        .field-hint.ok { color: #22c55e; opacity: 1; }
        .field-hint.bad { color: #ef4444; opacity: 1; }

        .pw-rules { list-style: none; margin: 8px 0 0; padding: 0; display: grid; grid-template-columns: 1fr 1fr; gap: 4px 12px; font-size: .8rem; }
        .pw-rules li { opacity: .65; }
        .pw-rules li::before { content: "○ "; }
        .pw-rules li.ok { color: #22c55e; opacity: 1; }
        .pw-rules li.ok::before { content: "✓ "; }
    </style>
</head>
<body class="auth-page">
    <form id="form1" runat="server">
        <div class="auth-split">
            <section class="auth-showcase" aria-hidden="true">
                <canvas id="dataField"></canvas>
                <img class="showcase-logo" src="Content/img/logo-dark.png" alt="" />
                <div>
                    <span class="showcase-kicker">// self-learning platform</span>
                    <h2>Train like a data analyst. Level up one kata at a time.</h2>
                    <p>Courses, quizzes, a live SQL lab and a community forum, all in one dojo.</p>
                    <div class="showcase-terminal">
                        <header><i></i><i></i><i></i></header>
                        <pre id="typeOut"></pre>
                    </div>
                    <div class="showcase-stats">
                        <span><b>SQL</b>live lab</span>
                        <span><b>Quiz</b>instant feedback</span>
                        <span><b>Badges</b>track progress</span>
                    </div>
                </div>
            </section>

            <section class="auth-panel">
                <div class="auth-card">
                    <img id="logoImg" class="logo" src="Content/img/logo-light.png" alt="The Analytical Dojo" />
                    <span class="eyebrow-mono">New student</span>
                    <h1>Join the dojo</h1>
                    <p class="sub">Create your account to start training.</p>

                    <asp:Label ID="lblError" runat="server" CssClass="auth-error" Visible="false" />
                    <div id="jsError" class="auth-error" style="display:none" role="alert"></div>

                    <div class="field">
                        <label for="<%= txtFullName.ClientID %>">Full name</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="input" MaxLength="100" autocomplete="name" />
                    </div>
                    <div class="field">
                        <label for="<%= txtUsername.ClientID %>">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="input" MaxLength="50" autocomplete="username" />
                        <small id="userHint" class="field-hint">3–50 characters. Letters, numbers, dots, hyphens and underscores. Start with a letter or number.</small>
                    </div>
                    <div class="field">
                        <label for="<%= txtEmail.ClientID %>">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" TextMode="Email" MaxLength="150" autocomplete="email" />
                    </div>
                    <div class="field">
                        <label for="<%= txtPassword.ClientID %>">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" MaxLength="72" autocomplete="new-password" />
                        <ul id="pwRules" class="pw-rules">
                            <li data-r="len">8+ characters</li>
                            <li data-r="upper">1 uppercase letter</li>
                            <li data-r="lower">1 lowercase letter</li>
                            <li data-r="num">1 number</li>
                            <li data-r="sym">1 symbol (!@#$…)</li>
                        </ul>
                    </div>
                    <div class="field">
                        <label for="<%= txtConfirm.ClientID %>">Confirm password</label>
                        <asp:TextBox ID="txtConfirm" runat="server" CssClass="input" TextMode="Password" MaxLength="72" autocomplete="new-password" />
                    </div>

                    <asp:Button ID="btnSignup" runat="server" Text="Create account →" CssClass="btn"
                                OnClick="btnSignup_Click" OnClientClick="return clientCheck();" />

                    <div style="display:flex;align-items:center;gap:12px;margin:18px 0;opacity:.6;font-size:.85rem;">
                        <span style="flex:1;height:1px;background:currentColor;opacity:.3;"></span>
                        or
                        <span style="flex:1;height:1px;background:currentColor;opacity:.3;"></span>
                    </div>

                    <div id="g_id_onload"
                         data-client_id="348938239435-rbvohc5nk1m9s0ahv5hjsbfqh4epurrp.apps.googleusercontent.com"
                         data-callback="onGoogleSignIn"
                         data-auto_prompt="false"></div>
                    <div class="g_id_signin"
                         data-type="standard" data-size="large"
                         data-theme="outline" data-text="signup_with" data-width="320"></div>

                    <asp:HiddenField ID="hfGoogleToken" runat="server" />
                    <asp:Button ID="btnGoogle" runat="server" OnClick="btnGoogle_Click"
                                UseSubmitBehavior="false" CausesValidation="false" style="display:none" />

                    <p class="sub" style="text-align:center;margin-top:18px;">
                        Already have an account? <a href="Login.aspx">Log in</a>
                    </p>
                    <p class="auth-foot">train.your.data</p>
                </div>
            </section>
        </div>

        <button type="button" id="themeBtn" class="theme-toggle" aria-label="Toggle theme">◐</button>

        <script src="https://accounts.google.com/gsi/client" async defer></script>
        <script>
            function onGoogleSignIn(response) {
                document.getElementById('<%= hfGoogleToken.ClientID %>').value = response.credential;
                __doPostBack('<%= btnGoogle.UniqueID %>', '');
            }

            // Instant feedback; the server repeats every check, so this is only a convenience.
            var USER_RE = /^[A-Za-z0-9][A-Za-z0-9._-]{2,49}$/;

            function pwChecks(pw) {
                return {
                    len: pw.length >= 8,
                    upper: /[A-Z]/.test(pw),
                    lower: /[a-z]/.test(pw),
                    num: /[0-9]/.test(pw),
                    sym: /[!-\/:-@\[-`{-~]/.test(pw)
                };
            }

            function clientCheck() {
                function v(id) { return document.getElementById(id).value; }
                var name = v('<%= txtFullName.ClientID %>').trim();
                var user = v('<%= txtUsername.ClientID %>').trim();
                var email = v('<%= txtEmail.ClientID %>').trim();
                var pw = v('<%= txtPassword.ClientID %>');
                var cf = v('<%= txtConfirm.ClientID %>');
                var msg = '';

                if (name.length < 2) msg = 'Enter your full name.';
                else if (!USER_RE.test(user))
                    msg = 'Username must be 3–50 characters, start with a letter or number, and use only letters, numbers, dots, hyphens or underscores.';
                else if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(email)) msg = 'Enter a valid email address.';
                else {
                    var c = pwChecks(pw), missing = [];
                    if (!c.len) missing.push('at least 8 characters');
                    if (!c.upper) missing.push('an uppercase letter');
                    if (!c.lower) missing.push('a lowercase letter');
                    if (!c.num) missing.push('a number');
                    if (!c.sym) missing.push('a symbol');
                    if (missing.length) msg = 'Password needs ' + missing.join(', ') + '.';
                    else if (pw !== cf) msg = 'Password and confirmation do not match.';
                }

                var box = document.getElementById('jsError');
                if (msg) { box.textContent = msg; box.style.display = 'block'; return false; }
                box.style.display = 'none';
                return true;
            }

            // Live hints while typing
            (function () {
                var u = document.getElementById('<%= txtUsername.ClientID %>'), uh = document.getElementById('userHint');
                var p = document.getElementById('<%= txtPassword.ClientID %>');
                var items = document.querySelectorAll('#pwRules li');
                u.addEventListener('input', function () {
                    var t = u.value.trim();
                    uh.className = 'field-hint' + (t ? (USER_RE.test(t) ? ' ok' : ' bad') : '');
                });
                p.addEventListener('input', function () {
                    var c = pwChecks(p.value);
                    items.forEach(function (li) { li.classList.toggle('ok', !!c[li.getAttribute('data-r')]); });
                });
            })();
        </script>

        <script>
            (function () {
                var logo = document.getElementById('logoImg'), btn = document.getElementById('themeBtn');
                function sync() {
                    var dark = document.documentElement.dataset.theme === 'dark';
                    logo.src = dark ? 'Content/img/logo-dark.png' : 'Content/img/logo-light.png';
                    btn.textContent = dark ? '☀' : '◐';
                }
                btn.addEventListener('click', function () {
                    var t = document.documentElement.dataset.theme === 'dark' ? 'light' : 'dark';
                    document.documentElement.dataset.theme = t;
                    try { localStorage.setItem('dojo-theme', t); } catch (e) { }
                    sync();
                });
                sync();

                var reduce = matchMedia('(prefers-reduced-motion: reduce)').matches;

                // Typed SQL in the terminal card
                var out = document.getElementById('typeOut');
                var lines = ['<span class="k">INSERT INTO</span> dojo.students', '<span class="k">VALUES</span> (<span class="k">\'you\'</span>);', '-- 1 row added: welcome'];
                var plain = lines.map(function (l) { return l.replace(/<[^>]+>/g, ''); });
                if (reduce) { out.innerHTML = lines.join('\n'); }
                else {
                    var li = 0, ci = 0;
                    (function type() {
                        var done = lines.slice(0, li).join('\n') + (li ? '\n' : '');
                        if (li >= lines.length) { out.innerHTML = lines.join('\n') + ' <span class="caret"></span>'; setTimeout(function () { li = 0; ci = 0; type(); }, 3500); return; }
                        ci++;
                        out.innerHTML = done + plain[li].slice(0, ci).replace(/</g, '&lt;') + '<span class="caret"></span>';
                        if (ci >= plain[li].length) { li++; ci = 0; setTimeout(type, 380); } else setTimeout(type, 38);
                    })();
                }

                // Drifting data points joined by faint lines
                var c = document.getElementById('dataField');
                if (!c || reduce) return;
                var ctx = c.getContext('2d'), pts = [], w, h, mouse = { x: -999, y: -999 };
                function size() { var r = c.getBoundingClientRect(); w = c.width = r.width * devicePixelRatio; h = c.height = r.height * devicePixelRatio; }
                size(); addEventListener('resize', size);
                for (var i = 0; i < 46; i++) pts.push({ x: Math.random() * w, y: Math.random() * h, vx: (Math.random() - .5) * .35, vy: (Math.random() - .5) * .35 });
                c.parentNode.addEventListener('pointermove', function (e) { var r = c.getBoundingClientRect(); mouse.x = (e.clientX - r.left) * devicePixelRatio; mouse.y = (e.clientY - r.top) * devicePixelRatio; });
                (function frame() {
                    ctx.clearRect(0, 0, w, h);
                    var link = 150 * devicePixelRatio;
                    for (var i = 0; i < pts.length; i++) {
                        var p = pts[i]; p.x += p.vx; p.y += p.vy;
                        if (p.x < 0 || p.x > w) p.vx *= -1; if (p.y < 0 || p.y > h) p.vy *= -1;
                        for (var j = i + 1; j < pts.length; j++) {
                            var q = pts[j], d = Math.hypot(p.x - q.x, p.y - q.y);
                            if (d < link) { ctx.strokeStyle = 'rgba(142,144,255,' + (1 - d / link) * .25 + ')'; ctx.lineWidth = devicePixelRatio; ctx.beginPath(); ctx.moveTo(p.x, p.y); ctx.lineTo(q.x, q.y); ctx.stroke(); }
                        }
                        var near = Math.hypot(p.x - mouse.x, p.y - mouse.y) < link;
                        ctx.fillStyle = near ? 'rgba(248,192,48,.95)' : 'rgba(201,203,255,.55)';
                        ctx.beginPath(); ctx.arc(p.x, p.y, (near ? 2.6 : 1.6) * devicePixelRatio, 0, 7); ctx.fill();
                    }
                    requestAnimationFrame(frame);
                })();
            })();
        </script>
    </form>
</body>
</html>
