// Quiz focus mode: one question at a time, live progress, keyboard shortcuts.
// Without JavaScript every question stays visible and the form still submits.
(function () {
    'use strict';

    var root = document.documentElement;
    var reduceMotion = matchMedia('(prefers-reduced-motion: reduce)').matches;

    // Theme-aware logo (dojo.js is not loaded on this focus page)
    document.querySelectorAll('[data-logo]').forEach(function (img) {
        img.src = root.dataset.theme === 'dark' ? 'Content/img/logo-dark.png' : 'Content/img/logo-light.png';
    });

    var cards = Array.prototype.slice.call(document.querySelectorAll('[data-question]'));
    var form = document.getElementById('form1');
    var submit = document.querySelector('.quiz-submit');
    if (!cards.length || !submit) return;

    var stepper = document.querySelector('[data-quiz-stepper]');
    var dotsBox = document.querySelector('[data-quiz-dots]');
    var prevBtn = document.querySelector('[data-quiz-prev]');
    var nextBtn = document.querySelector('[data-quiz-next]');
    var submitRow = document.querySelector('[data-quiz-submit-row]');
    var meter = document.querySelector('[data-quiz-meter]');
    var count = document.querySelector('[data-quiz-count]');
    var toast = document.querySelector('[data-quiz-toast]');
    var current = 0;

    var cheers = ['Locked in!', 'Nice pick!', 'Keep the streak going', 'On a roll', 'Sharp thinking', 'Onwards, sensei', 'Data-driven move', 'Nailed the choice'];

    // Letter badges A/B/C/D next to each option
    cards.forEach(function (card) {
        card.querySelectorAll('.quiz-options label').forEach(function (label, i) {
            label.setAttribute('data-letter', String.fromCharCode(65 + i));
        });
    });

    function answered(card) {
        if (card.dataset.kind === 'subjective') {
            var box = card.querySelector('textarea');
            return !!(box && box.value.trim());
        }
        return !!card.querySelector('input[type=radio]:checked');
    }

    function refresh() {
        var done = cards.filter(answered).length;
        var pct = Math.round(done / cards.length * 100);
        meter.style.width = pct + '%';
        count.textContent = done + ' / ' + cards.length + ' answered';
        cards.forEach(function (card, i) {
            card.classList.toggle('is-answered', answered(card));
            var dot = dotsBox.children[i];
            if (dot) {
                dot.classList.toggle('is-done', answered(card));
                dot.classList.toggle('is-current', i === current);
            }
        });
        submit.classList.toggle('is-ready', done === cards.length);
    }

    function say(text, tone) {
        toast.textContent = text;
        toast.className = 'quiz-toast is-shown' + (tone ? ' is-' + tone : '');
        clearTimeout(say.timer);
        say.timer = setTimeout(function () { toast.className = 'quiz-toast'; }, 1600);
    }

    function show(index, direction) {
        current = Math.max(0, Math.min(cards.length - 1, index));
        cards.forEach(function (card, i) {
            var active = i === current;
            card.hidden = !active;
            card.classList.remove('slide-left', 'slide-right');
            if (active && !reduceMotion && direction) {
                void card.offsetWidth;
                card.classList.add(direction > 0 ? 'slide-left' : 'slide-right');
            }
        });
        prevBtn.disabled = current === 0;
        var last = current === cards.length - 1;
        nextBtn.hidden = last;
        submitRow.classList.toggle('is-visible', last);
        refresh();
        var focusable = cards[current].querySelector('input:checked') || cards[current].querySelector('input, textarea');
        if (focusable && direction) focusable.focus({ preventScroll: true });
    }

    function firstUnanswered() {
        for (var i = 0; i < cards.length; i++) if (!answered(cards[i])) return i;
        return -1;
    }

    function nudge(index) {
        show(index, index > current ? 1 : -1);
        var card = cards[index];
        card.classList.remove('shake');
        void card.offsetWidth;
        card.classList.add('shake');
    }

    // Build the stepper
    cards.forEach(function (card, i) {
        var dot = document.createElement('button');
        dot.type = 'button';
        dot.className = 'quiz-dot';
        dot.textContent = i + 1;
        dot.setAttribute('aria-label', 'Go to question ' + (i + 1));
        dot.addEventListener('click', function () { show(i, i > current ? 1 : -1); });
        dotsBox.appendChild(dot);
    });
    stepper.hidden = false;
    document.body.classList.add('quiz-stepper-on');

    prevBtn.addEventListener('click', function () { show(current - 1, -1); });
    nextBtn.addEventListener('click', function () { show(current + 1, 1); });

    cards.forEach(function (card, i) {
        card.addEventListener('change', function (event) {
            if (event.target.type !== 'radio') return;
            var label = card.querySelector('label[for="' + event.target.id + '"]');
            if (label && !reduceMotion) {
                label.classList.remove('pop');
                void label.offsetWidth;
                label.classList.add('pop');
            }
            refresh();
            var left = cards.length - cards.filter(answered).length;
            say(left === 0 ? 'All answered! Ready to submit 🎯' : cheers[Math.floor(Math.random() * cheers.length)], left === 0 ? 'gold' : '');
            // Auto-advance to the next question after a short beat
            if (i === current && i < cards.length - 1) {
                setTimeout(function () { if (current === i) show(i + 1, 1); }, reduceMotion ? 0 : 520);
            }
        });
        card.addEventListener('input', function (event) {
            if (event.target.tagName === 'TEXTAREA') refresh();
        });
    });

    addEventListener('keydown', function (event) {
        if (event.ctrlKey || event.metaKey || event.altKey) return;
        var typing = document.activeElement && document.activeElement.tagName === 'TEXTAREA';
        if (typing) return;
        var card = cards[current];
        var n = parseInt(event.key, 10);
        var letter = /^[a-d]$/i.test(event.key) ? event.key.toUpperCase().charCodeAt(0) - 64 : 0;
        var pick = n >= 1 && n <= 9 ? n : letter;
        if (pick) {
            var radios = card.querySelectorAll('input[type=radio]');
            if (radios[pick - 1]) {
                radios[pick - 1].checked = true;
                radios[pick - 1].dispatchEvent(new Event('change', { bubbles: true }));
                event.preventDefault();
            }
        } else if (event.key === 'ArrowRight') {
            show(current + 1, 1);
        } else if (event.key === 'ArrowLeft') {
            show(current - 1, -1);
        }
    });

    // Block submit until everything is answered, then show the grading overlay
    form.addEventListener('submit', function (event) {
        var missing = firstUnanswered();
        if (missing !== -1) {
            event.preventDefault();
            nudge(missing);
            say('Question ' + (missing + 1) + ' still needs an answer', 'warn');
            return;
        }
        document.querySelector('[data-quiz-grading]').hidden = false;
    });

    // Timer
    var timer = document.querySelector('[data-quiz-timer]');
    var started = Date.now();
    setInterval(function () {
        var s = Math.floor((Date.now() - started) / 1000);
        timer.textContent = String(Math.floor(s / 60)).padStart(2, '0') + ':' + String(s % 60).padStart(2, '0');
    }, 1000);

    // After a server-side validation error, open the first unanswered question
    var error = document.querySelector('.notice[role=alert]');
    var start = error && firstUnanswered() !== -1 ? firstUnanswered() : 0;
    show(start, 0);
})();
