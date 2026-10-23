// Quiz results: score ring, one-time result card and confetti for 80%+.
(function () {
    'use strict';

    var reduceMotion = matchMedia('(prefers-reduced-motion: reduce)').matches;

    // Fill the score ring
    var ring = document.querySelector('[data-score-ring]');
    if (ring) requestAnimationFrame(function () { ring.classList.add('is-filled'); });

    var scrim = document.querySelector('[data-result]');
    if (!scrim) return;

    // The page container animates (transform), which would trap position:fixed,
    // so the overlay lives directly under <body>.
    document.body.appendChild(scrim);

    // Drop ?fresh=1 so a refresh does not replay the celebration
    try { history.replaceState(null, '', location.pathname + location.search.replace(/[?&]fresh=1/, '').replace(/^&/, '?')); } catch (e) { }

    var mood = scrim.dataset.mood;
    document.body.classList.add('result-open');

    // Count the score up
    var counter = scrim.querySelector('[data-count-to]');
    if (counter) {
        var target = parseFloat(counter.dataset.countTo);
        var startTime = performance.now();
        var duration = reduceMotion ? 0 : 1300;
        (function step(now) {
            var t = duration ? Math.min(1, (now - startTime) / duration) : 1;
            var value = target * (1 - Math.pow(1 - t, 3));
            counter.textContent = t < 1 ? Math.round(value) : counter.dataset.countTo;
            if (t < 1) requestAnimationFrame(step);
        })(startTime);
    }

    function close() {
        scrim.classList.add('is-leaving');
        document.body.classList.remove('result-open');
        setTimeout(function () { scrim.remove(); }, 350);
    }

    scrim.querySelector('[data-result-close]').addEventListener('click', close);
    scrim.addEventListener('click', function (event) { if (event.target === scrim) close(); });
    addEventListener('keydown', function (event) { if (event.key === 'Escape' && document.body.contains(scrim)) close(); });
    setTimeout(function () { var b = scrim.querySelector('[data-result-close]'); if (b) b.focus(); }, 50);

    // Confetti for a distinction, a light sparkle for a pass
    var canvas = scrim.querySelector('[data-confetti]');
    if (!canvas || reduceMotion || (mood !== 'win' && mood !== 'pass')) return;

    var ctx = canvas.getContext('2d');
    var dpr = window.devicePixelRatio || 1;
    var pieces = [];
    var colors = ['#f8c030', '#ffdb7a', '#8e90ff', '#3fd6e4', '#ffffff', '#ff7a90'];
    var w, h;

    function size() {
        w = canvas.width = innerWidth * dpr;
        h = canvas.height = innerHeight * dpr;
    }
    size();
    addEventListener('resize', size);

    function burst(x, y, amount, power) {
        for (var i = 0; i < amount; i++) {
            var angle = Math.random() * Math.PI * 2;
            var speed = (Math.random() * 0.6 + 0.4) * power * dpr;
            pieces.push({
                x: x, y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed - power * 0.6 * dpr,
                size: (Math.random() * 6 + 4) * dpr,
                color: colors[Math.floor(Math.random() * colors.length)],
                spin: Math.random() * 0.3 - 0.15,
                angle: Math.random() * Math.PI,
                life: 1,
                shape: Math.random() < 0.3 ? 'circle' : 'rect'
            });
        }
    }

    if (mood === 'win') {
        burst(w * 0.5, h * 0.45, 160, 14);
        setTimeout(function () { burst(w * 0.15, h * 0.7, 90, 12); burst(w * 0.85, h * 0.7, 90, 12); }, 350);
        setTimeout(function () { burst(w * 0.5, h * 0.3, 120, 11); }, 900);
    } else {
        burst(w * 0.5, h * 0.45, 70, 9);
    }

    (function frame() {
        ctx.clearRect(0, 0, w, h);
        pieces = pieces.filter(function (p) { return p.life > 0 && p.y < h + 40; });
        pieces.forEach(function (p) {
            p.vy += 0.28 * dpr;
            p.vx *= 0.99;
            p.x += p.vx;
            p.y += p.vy;
            p.angle += p.spin;
            p.life -= 0.004;
            ctx.save();
            ctx.globalAlpha = Math.max(0, Math.min(1, p.life * 1.5));
            ctx.translate(p.x, p.y);
            ctx.rotate(p.angle);
            ctx.fillStyle = p.color;
            if (p.shape === 'circle') {
                ctx.beginPath();
                ctx.arc(0, 0, p.size / 2.4, 0, Math.PI * 2);
                ctx.fill();
            } else {
                ctx.fillRect(-p.size / 2, -p.size / 4, p.size, p.size / 2);
            }
            ctx.restore();
        });
        if (pieces.length) requestAnimationFrame(frame);
    })();
})();
