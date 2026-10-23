(function(){
 'use strict';
 function applyTheme(){const dark=document.documentElement.dataset.theme==='dark';document.querySelectorAll('[data-logo]').forEach(img=>{img.src=dark?'Content/img/logo-dark.png':'Content/img/logo-light.png';img.onerror=()=>{img.hidden=true;};});document.querySelectorAll('[data-theme-toggle]').forEach(b=>{b.textContent=dark?'☀':'◐';b.setAttribute('aria-label',dark?'Switch to light theme':'Switch to dark theme');});}
 applyTheme();document.querySelectorAll('[data-theme-toggle]').forEach(b=>b.addEventListener('click',()=>{const t=document.documentElement.dataset.theme==='dark'?'light':'dark';document.documentElement.dataset.theme=t;try{localStorage.setItem('dojo-theme',t);}catch(e){}applyTheme();}));
 const pageName=p=>(p||'').toLowerCase().split(/[?#]/)[0].split('/').pop().replace(/\.aspx$/,'')||'studentmainpage';const current=pageName(location.pathname);document.querySelectorAll('.navrow a').forEach(a=>{if(pageName(a.getAttribute('href'))===current)a.setAttribute('aria-current','page');});
 const header=document.querySelector('[data-site-header]');
 if(header){header.addEventListener('pointermove',event=>{const r=header.getBoundingClientRect();header.style.setProperty('--header-x',(event.clientX-r.left)+'px');header.style.setProperty('--header-y',(event.clientY-r.top)+'px');});addEventListener('scroll',()=>header.classList.toggle('is-scrolled',scrollY>8),{passive:true});}
 const reduceMotion=matchMedia('(prefers-reduced-motion: reduce)').matches;
 const scrollBar=document.querySelector('[data-scroll-progress]');
 if(scrollBar){const setScroll=()=>{const max=document.documentElement.scrollHeight-innerHeight;scrollBar.style.setProperty('--scroll',max>0?Math.min(1,scrollY/max):0);};addEventListener('scroll',setScroll,{passive:true});addEventListener('resize',setScroll);setScroll();}
 // Pointer spotlight on cards
 document.addEventListener('pointermove',event=>{const card=event.target.closest&&event.target.closest('.panel,.stat,.course-row');if(!card)return;const r=card.getBoundingClientRect();card.style.setProperty('--mx',(event.clientX-r.left)+'px');card.style.setProperty('--my',(event.clientY-r.top)+'px');},{passive:true});
 // Gentle 3D tilt on course cards
 if(!reduceMotion)document.querySelectorAll('.course-card').forEach(card=>{card.addEventListener('pointermove',event=>{const r=card.getBoundingClientRect(),x=(event.clientX-r.left)/r.width-.5,y=(event.clientY-r.top)/r.height-.5;card.style.transform='perspective(800px) rotateX('+(-y*6)+'deg) rotateY('+(x*6)+'deg) translateY(-6px)';});card.addEventListener('pointerleave',()=>{card.style.transform='';});});
 // Click ripple on buttons
 document.addEventListener('click',event=>{const btn=event.target.closest&&event.target.closest('.btn,.chat-send');if(!btn||reduceMotion||btn.tagName==='INPUT')return;const r=btn.getBoundingClientRect(),size=Math.max(r.width,r.height),dot=document.createElement('span');dot.className='ripple';dot.style.width=dot.style.height=size+'px';dot.style.left=(event.clientX-r.left-size/2)+'px';dot.style.top=(event.clientY-r.top-size/2)+'px';btn.appendChild(dot);setTimeout(()=>dot.remove(),650);});
 // Count-up numbers in stat tiles
 function countUp(el){const text=el.textContent.trim(),m=text.match(/^(\D*)(\d[\d,]*(?:\.\d+)?)(.*)$/);if(!m||reduceMotion)return;const target=parseFloat(m[2].replace(/,/g,'')),decimals=(m[2].split('.')[1]||'').length,start=performance.now(),dur=900;function step(now){const t=Math.min(1,(now-start)/dur),v=target*(1-Math.pow(1-t,3));el.textContent=m[1]+v.toLocaleString('en-US',{minimumFractionDigits:decimals,maximumFractionDigits:decimals})+m[3];if(t<1)requestAnimationFrame(step);else el.textContent=text;}requestAnimationFrame(step);}
 // Progress bars fill in when they come into view
 function fillProgress(el){if(reduceMotion)return;if(el.tagName==='PROGRESS'){const v=el.value;el.value=0;const s=performance.now();(function step(now){const t=Math.min(1,(now-s)/900);el.value=v*(1-Math.pow(1-t,3));if(t<1)requestAnimationFrame(step);})(s);}else{const w=el.style.width;el.style.width='0';requestAnimationFrame(()=>requestAnimationFrame(()=>{el.style.width=w;}));}}
 // Scroll reveal
 const revealTargets=[...document.querySelectorAll('.dashboard .panel,.dashboard .stat,.dashboard .course-card,.dashboard .course-row,.dashboard .forum-post,.dashboard .learning-hero,.dashboard .course-community-card,.dashboard .chat-rail-card,.dashboard .lesson,.dashboard .achievement')];
 if('IntersectionObserver' in window&&!reduceMotion){const io=new IntersectionObserver(entries=>entries.forEach(entry=>{if(!entry.isIntersecting)return;const el=entry.target;el.classList.add('is-in');el.querySelectorAll('.stat strong').forEach(countUp);if(el.matches('.stat'))el.querySelectorAll('strong').forEach(countUp);el.querySelectorAll('progress,.track span').forEach(fillProgress);io.unobserve(el);setTimeout(()=>el.classList.remove('reveal','is-in'),1100);}),{threshold:.12,rootMargin:'0px 0px -40px 0px'});const groups=new Map();revealTargets.forEach(el=>{const n=groups.get(el.parentNode)||0;groups.set(el.parentNode,n+1);el.style.setProperty('--reveal-delay',Math.min(n,6)*0.06+'s');el.classList.add('reveal');io.observe(el);});}
 const nav=document.querySelector('.navrow'),indicator=nav&&nav.querySelector('.nav-indicator'),navLinks=nav?[...nav.querySelectorAll('a')]:[];
 function placeIndicator(link){if(!indicator)return;indicator.style.opacity=link?'1':'0';navLinks.forEach(a=>a.classList.toggle('is-lit',a===link));if(!link)return;indicator.style.width=link.offsetWidth+'px';indicator.style.transform='translateX('+link.offsetLeft+'px)';}
 if(nav){const active=nav.querySelector('[aria-current="page"]');requestAnimationFrame(()=>placeIndicator(active));if(document.fonts)document.fonts.ready.then(()=>placeIndicator(nav.querySelector('[aria-current="page"]')||active));navLinks.forEach(link=>link.addEventListener('mouseenter',()=>placeIndicator(link)));nav.addEventListener('mouseleave',()=>placeIndicator(nav.querySelector('[aria-current="page"]')||active));addEventListener('resize',()=>placeIndicator(nav.querySelector('[aria-current="page"]')||active));}
 const noticeButton=document.querySelector('[data-notification-toggle]'),noticePanel=document.querySelector('[data-notification-panel]');
 if(noticeButton&&noticePanel){noticeButton.addEventListener('click',event=>{event.stopPropagation();const open=noticePanel.hidden;noticePanel.hidden=!open;noticeButton.setAttribute('aria-expanded',String(open));});noticePanel.addEventListener('click',event=>event.stopPropagation());document.addEventListener('click',()=>{noticePanel.hidden=true;noticeButton.setAttribute('aria-expanded','false');});}
 const searchOpen=document.querySelector('[data-search-open]'),scrim=document.querySelector('[data-search-scrim]'),globalSearch=document.getElementById('globalCourseSearch'),results=document.querySelector('[data-search-results]'),indexNode=document.getElementById('dojoCourseIndex');
 let courseIndex=[],searchSelection=0,visibleCourses=[];
 try{courseIndex=indexNode?JSON.parse(indexNode.textContent):[];}catch(ignore){}
 function renderCourseSearch(){if(!results)return;const query=(globalSearch.value||'').trim().toLowerCase();visibleCourses=courseIndex.filter(course=>(course.title+' '+course.level).toLowerCase().includes(query)).slice(0,8);searchSelection=Math.min(searchSelection,Math.max(0,visibleCourses.length-1));results.innerHTML=visibleCourses.length?visibleCourses.map((course,i)=>'<a class="command-result '+(i===searchSelection?'is-selected':'')+'" href="'+(course.enrolled?'CoursePage.aspx?id=':'BrowseCourses.aspx?course=')+course.id+'"><span class="command-code">'+String(i+1).padStart(2,'0')+'</span><span><strong>'+escapeMarkup(course.title)+'</strong><small>'+escapeMarkup(course.level)+' · '+(course.enrolled?'Continue learning':'Available to enrol')+'</small></span><span class="command-arrow">↗</span></a>').join(''):'<div class="command-empty">No course matches that search.</div>';}
 function escapeMarkup(value){const node=document.createElement('span');node.textContent=value==null?'':String(value);return node.innerHTML;}
 function openCourseSearch(){if(!scrim)return;scrim.hidden=false;document.body.classList.add('palette-open');globalSearch.value='';searchSelection=0;renderCourseSearch();setTimeout(()=>globalSearch.focus(),0);}
 function closeCourseSearch(){if(!scrim)return;scrim.hidden=true;document.body.classList.remove('palette-open');}
 if(searchOpen)searchOpen.addEventListener('click',openCourseSearch);
 if(globalSearch)globalSearch.addEventListener('input',()=>{searchSelection=0;renderCourseSearch();});
 if(scrim)scrim.addEventListener('click',event=>{if(event.target===scrim)closeCourseSearch();});
 addEventListener('keydown',event=>{if((event.ctrlKey||event.metaKey)&&event.key.toLowerCase()==='k'){event.preventDefault();openCourseSearch();return;}if(!scrim||scrim.hidden)return;if(event.key==='Escape')closeCourseSearch();if(event.key==='ArrowDown'&&visibleCourses.length){event.preventDefault();searchSelection=(searchSelection+1)%visibleCourses.length;renderCourseSearch();}if(event.key==='ArrowUp'&&visibleCourses.length){event.preventDefault();searchSelection=(searchSelection-1+visibleCourses.length)%visibleCourses.length;renderCourseSearch();}if(event.key==='Enter'&&visibleCourses[searchSelection]){event.preventDefault();location.href=(visibleCourses[searchSelection].enrolled?'CoursePage.aspx?id=':'BrowseCourses.aspx?course=')+visibleCourses[searchSelection].id;}});
 let filter='all';function filterCourses(){const items=document.querySelectorAll('[data-course]');if(!items.length)return;let count=0;const box=document.getElementById('courseSearch');const q=box?(box.value||'').toLowerCase():'';items.forEach(c=>{const title=(c.dataset.title||'').toLowerCase();c.hidden=!(title.indexOf(q)!==-1&&(filter==='all'||c.dataset.status===filter));if(!c.hidden)count++;});const empty=document.getElementById('noMatches');if(empty)empty.hidden=count>0;}
 const search=document.getElementById('courseSearch');if(search)search.addEventListener('input',filterCourses);document.querySelectorAll('[data-filter]').forEach(b=>b.addEventListener('click',()=>{filter=b.dataset.filter;document.querySelectorAll('[data-filter]').forEach(x=>x.setAttribute('aria-pressed',String(x===b)));filterCourses();}));
 document.querySelectorAll('.hex-badge').forEach(btn=>btn.addEventListener('click',()=>{const detail=document.getElementById('badgeDetail');if(detail)detail.textContent=btn.getAttribute('data-badge-detail')||btn.title;}));
 const review=document.getElementById('reviewSearch');if(review)review.addEventListener('input',()=>{let n=0;document.querySelectorAll('[data-review]').forEach(r=>{r.hidden=!r.textContent.toLowerCase().includes(review.value.toLowerCase());if(!r.hidden)n++;});const empty=document.getElementById('reviewEmpty');if(empty)empty.hidden=n>0;});
 const forumSearch=document.getElementById('forumSearch');if(forumSearch)forumSearch.addEventListener('input',()=>{let visible=0;document.querySelectorAll('[data-forum-post]').forEach(post=>{post.hidden=!post.textContent.toLowerCase().includes(forumSearch.value.trim().toLowerCase());if(!post.hidden)visible++;});const empty=document.getElementById('forumSearchEmpty');if(empty)empty.hidden=visible>0||forumSearch.value.trim()==='';});
 // Leaderboard: quiz picker, jump to my row, podium count-up
 document.querySelectorAll('[data-nav-select]').forEach(select=>select.addEventListener('change',()=>{location.href=select.value;}));
 const jumpMe=document.querySelector('[data-jump-me]'),myRow=document.getElementById('lb-me');
 if(jumpMe&&myRow)jumpMe.addEventListener('click',()=>{myRow.scrollIntoView({block:'center',behavior:reduceMotion?'auto':'smooth'});myRow.classList.remove('is-pinged');void myRow.offsetWidth;myRow.classList.add('is-pinged');});
 document.querySelectorAll('.lb-podium [data-count-to],.lb-me [data-count-to]').forEach(countUp);
 // Forum: one reply box that moves under the post / reply being answered
 const composer=document.querySelector('[data-reply-composer]');
 if(composer){
  const box=composer.querySelector('textarea'),send=composer.querySelector('[data-reply-send]'),chars=composer.querySelector('[data-reply-chars]');
  const syncReply=()=>{const n=box.value.trim().length;chars.textContent=box.value.length;send.disabled=n<2;};
  document.addEventListener('click',event=>{
   const trigger=event.target.closest('[data-reply-to]');
   if(trigger){
    const anchor=trigger.closest('.reply-body')||trigger.closest('.forum-post-actions');
    composer.querySelector('[data-reply-post]').value=trigger.dataset.post;
    composer.querySelector('[data-reply-parent]').value=trigger.dataset.parent||'';
    composer.querySelector('[data-reply-name]').textContent=trigger.dataset.name;
    document.querySelectorAll('.is-replying').forEach(el=>el.classList.remove('is-replying'));
    anchor.classList.add('is-replying');
    anchor.after(composer);composer.hidden=false;composer.classList.remove('composer-in');void composer.offsetWidth;composer.classList.add('composer-in');
    syncReply();box.focus();
    return;
   }
   if(event.target.closest('[data-reply-cancel]')){composer.hidden=true;box.value='';document.querySelectorAll('.is-replying').forEach(el=>el.classList.remove('is-replying'));}
   const toggle=event.target.closest('[data-thread-toggle]');
   if(toggle){const thread=toggle.closest('[data-forum-post]').querySelector('[data-thread]');const open=toggle.getAttribute('aria-expanded')!=='true';toggle.setAttribute('aria-expanded',String(open));thread.hidden=!open;}
  });
  box.addEventListener('input',syncReply);
  box.addEventListener('keydown',event=>{if(event.key==='Enter'&&(event.ctrlKey||event.metaKey)&&!send.disabled){event.preventDefault();send.click();}if(event.key==='Escape')composer.querySelector('[data-reply-cancel]').click();});
  // Flash the reply you just posted
  const fresh=location.hash.indexOf('#reply-')===0&&document.getElementById(location.hash.slice(1));
  if(fresh){fresh.classList.add('is-new');setTimeout(()=>fresh.scrollIntoView({block:'center',behavior:reduceMotion?'auto':'smooth'}),300);}
 }
 const chatInput=document.querySelector('[data-chat-input]');if(chatInput){const chatWindow=document.querySelector('[data-chat-window]');if(chatWindow)chatWindow.scrollTop=chatWindow.scrollHeight;document.querySelectorAll('[data-chat-prompt]').forEach(button=>button.addEventListener('click',()=>{chatInput.value=button.dataset.chatPrompt||'';chatInput.focus();}));chatInput.addEventListener('keydown',event=>{if(event.key==='Enter'&&!event.shiftKey&&!event.isComposing){event.preventDefault();const send=document.querySelector('.dojo-chat-send');if(send&&!send.disabled)send.click();}});}
 const examples={average:'SELECT course, ROUND(AVG(score), 1) AS avg_score\nFROM my_quiz_results\nGROUP BY course\nORDER BY avg_score DESC;',all:'SELECT course, quiz, score\nFROM my_quiz_results\nORDER BY score DESC;',filter:'SELECT course, quiz, score\nFROM my_quiz_results\nWHERE score >= 80\nORDER BY score DESC;'};
 const editor=document.querySelector('.sql-editor');if(editor){document.querySelectorAll('[data-example]').forEach(b=>b.addEventListener('click',()=>{editor.value=examples[b.dataset.example];editor.focus();}));document.querySelector('[data-query-reset]').addEventListener('click',()=>{editor.value=examples.average;editor.focus();});}
 // Material viewer: fullscreen, theatre mode and keyboard shortcuts
 const stage=document.querySelector('[data-viewer-stage]');
 if(stage){
  const layout=document.querySelector('[data-viewer-layout]'),theatreBtn=document.querySelector('[data-viewer-theatre]'),fullBtn=document.querySelector('[data-viewer-fullscreen]');
  const toggleFull=()=>{if(document.fullscreenElement)document.exitFullscreen();else if(stage.requestFullscreen)stage.requestFullscreen();};
  const setTheatre=on=>{layout.classList.toggle('is-theatre',on);if(theatreBtn)theatreBtn.setAttribute('aria-pressed',String(on));try{localStorage.setItem('dojo-theatre',on?'1':'0');}catch(e){}};
  try{if(localStorage.getItem('dojo-theatre')==='1')setTheatre(true);}catch(e){}
  if(fullBtn)fullBtn.addEventListener('click',toggleFull);
  if(theatreBtn)theatreBtn.addEventListener('click',()=>setTheatre(!layout.classList.contains('is-theatre')));
  document.addEventListener('fullscreenchange',()=>{if(fullBtn)fullBtn.textContent=document.fullscreenElement?'⤡ Exit fullscreen':'⤢ Fullscreen';});
  // PDF slides: render page-by-page with PDF.js so they fill the stage
  let deck=null;
  // Image slide decks (material_slides): one slide visible at a time
  const imageSlides=[...stage.querySelectorAll('[data-slide]')];
  if(imageSlides.length){
   const thumbs=[...stage.querySelectorAll('[data-slide-go]')],progress=stage.querySelector('[data-slide-progress]'),bar=stage.querySelector('.viewer-bar');
   const controls=document.createElement('span');controls.className='slide-controls';
   controls.innerHTML='<button type="button" class="viewer-tool" data-slide-step="-1" aria-label="Previous slide">◀</button><span class="slide-count" aria-live="polite"></span><button type="button" class="viewer-tool" data-slide-step="1" aria-label="Next slide">▶</button>';
   bar.insertBefore(controls,bar.querySelector('.viewer-bar-actions'));
   const counter=controls.querySelector('.slide-count');
   const fromHash=parseInt((location.hash.match(/^#slide-(\d+)$/)||[])[1],10);
   let index=fromHash>0&&fromHash<=imageSlides.length?fromHash-1:0;
   function showSlide(next,dir){
    imageSlides.forEach((s,i)=>{s.classList.toggle('is-active',i===next);s.classList.remove('slide-in','slide-in-back');});
    if(dir&&!reduceMotion){void imageSlides[next].offsetWidth;imageSlides[next].classList.add(dir>0?'slide-in':'slide-in-back');}
    thumbs.forEach((t,i)=>t.classList.toggle('is-active',i===next));
    if(thumbs[next])thumbs[next].scrollIntoView({block:'nearest',inline:'center',behavior:reduceMotion?'auto':'smooth'});
    index=next;counter.textContent=(next+1)+' / '+imageSlides.length;
    progress.style.width=((next+1)/imageSlides.length*100)+'%';
    stage.querySelectorAll('[data-slide-step="-1"]').forEach(b=>b.disabled=next===0);
    stage.querySelectorAll('[data-slide-step="1"]').forEach(b=>b.disabled=next===imageSlides.length-1);
    // Warm the next image so it appears instantly
    const upcoming=imageSlides[next+1]&&imageSlides[next+1].querySelector('img');if(upcoming)upcoming.loading='eager';
    try{history.replaceState(null,'',location.pathname+location.search+'#slide-'+(next+1));}catch(e){}
   }
   const go=step=>{const target=index+step;if(target<0||target>=imageSlides.length)return false;showSlide(target,step);return true;};
   stage.addEventListener('click',event=>{const step=event.target.closest('[data-slide-step]');if(step){go(Number(step.dataset.slideStep));return;}const jump=event.target.closest('[data-slide-go]');if(jump){const n=Number(jump.dataset.slideGo);showSlide(n,n>index?1:-1);}});
   // Swipe on touch screens
   let touchX=null;
   stage.addEventListener('touchstart',event=>{touchX=event.touches[0].clientX;},{passive:true});
   stage.addEventListener('touchend',event=>{if(touchX===null)return;const dx=event.changedTouches[0].clientX-touchX;if(Math.abs(dx)>45)go(dx<0?1:-1);touchX=null;});
   showSlide(index,0);
   deck={go:go};
  }
  const pdfSrc=stage.dataset.pdf,frame=stage.querySelector('.viewer-frame'),pdfFrame=frame&&frame.querySelector('iframe');
  if(pdfSrc&&frame){
   const pdfjsBase='https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/';
   const fallback=()=>{frame.classList.remove('is-loading');if(pdfFrame&&pdfFrame.dataset.src)pdfFrame.src=pdfFrame.dataset.src;};
   frame.classList.add('is-loading');
   const lib=document.createElement('script');lib.src=pdfjsBase+'pdf.min.js';lib.onerror=fallback;
   lib.onload=()=>{window.pdfjsLib.GlobalWorkerOptions.workerSrc=pdfjsBase+'pdf.worker.min.js';window.pdfjsLib.getDocument(pdfSrc).promise.then(doc=>{deck=buildDeck(doc);}).catch(fallback);};
   document.head.appendChild(lib);
  }
  function buildDeck(doc){
   frame.classList.remove('is-loading');if(pdfFrame)pdfFrame.remove();
   const wrap=document.createElement('div');wrap.className='slide-deck';
   const canvas=document.createElement('canvas');wrap.appendChild(canvas);
   const prevZone=document.createElement('button'),nextZone=document.createElement('button');
   prevZone.type=nextZone.type='button';prevZone.className='slide-zone is-prev';nextZone.className='slide-zone is-next';prevZone.setAttribute('aria-label','Previous slide');nextZone.setAttribute('aria-label','Next slide');prevZone.textContent='‹';nextZone.textContent='›';
   frame.append(wrap,prevZone,nextZone);
   const controls=document.createElement('span');controls.className='slide-controls';
   controls.innerHTML='<button type="button" class="viewer-tool" data-slide="-1" aria-label="Previous slide">◀</button><span class="slide-count" aria-live="polite"></span><button type="button" class="viewer-tool" data-slide="1" aria-label="Next slide">▶</button><button type="button" class="viewer-tool" data-zoom="-1" aria-label="Zoom out">−</button><button type="button" class="viewer-tool" data-zoom="0" aria-label="Fit to screen">Fit</button><button type="button" class="viewer-tool" data-zoom="1" aria-label="Zoom in">+</button>';
   const bar=stage.querySelector('.viewer-bar');bar.insertBefore(controls,bar.querySelector('.viewer-bar-actions'));
   const counter=controls.querySelector('.slide-count');
   let page=1,zoom=1,task=null;
   function render(){
    doc.getPage(page).then(p=>{
     const base=p.getViewport({scale:1}),fit=Math.min(frame.clientWidth/base.width,frame.clientHeight/base.height)*0.96,scale=fit*zoom,dpr=window.devicePixelRatio||1,vp=p.getViewport({scale:scale*dpr});
     if(task)task.cancel();
     canvas.width=vp.width;canvas.height=vp.height;canvas.style.width=vp.width/dpr+'px';canvas.style.height=vp.height/dpr+'px';
     wrap.classList.toggle('is-zoomed',zoom>1);
     task=p.render({canvasContext:canvas.getContext('2d'),viewport:vp});task.promise.catch(()=>{});
     counter.textContent=page+' / '+doc.numPages;
     prevZone.disabled=page===1;nextZone.disabled=page===doc.numPages;
    });
   }
   function go(step){const target=Math.max(1,Math.min(doc.numPages,page+step));if(target===page)return false;page=target;canvas.classList.remove('slide-in');void canvas.offsetWidth;if(!reduceMotion)canvas.classList.add(step>0?'slide-in':'slide-in-back');render();return true;}
   prevZone.addEventListener('click',()=>go(-1));nextZone.addEventListener('click',()=>go(1));
   controls.addEventListener('click',event=>{const b=event.target.closest('button');if(!b)return;if(b.dataset.slide)go(Number(b.dataset.slide));if(b.dataset.zoom){const z=Number(b.dataset.zoom);zoom=z===0?1:Math.max(.5,Math.min(3,zoom+z*.25));render();}});
   addEventListener('resize',render);document.addEventListener('fullscreenchange',()=>setTimeout(render,60));
   if(layout)layout.addEventListener('transitionend',render);
   render();
   return{go:go};
  }
  addEventListener('keydown',event=>{if(event.ctrlKey||event.metaKey||event.altKey||/^(INPUT|TEXTAREA|SELECT)$/.test(document.activeElement.tagName)||(scrim&&!scrim.hidden))return;const k=event.key.toLowerCase();if(k==='f'){event.preventDefault();toggleFull();}else if(k==='t'){setTheatre(!layout.classList.contains('is-theatre'));}else if(event.key==='ArrowRight'||event.key==='PageDown'){event.preventDefault();if(deck&&deck.go(1))return;const n=document.querySelector('[data-viewer-next]');if(n)location.href=n.href;}else if(event.key==='ArrowLeft'||event.key==='PageUp'){event.preventDefault();if(deck&&deck.go(-1))return;const p=document.querySelector('[data-viewer-prev]');if(p)location.href=p.href;}});
  const currentItem=document.querySelector('.viewer-item.is-current'),list=currentItem&&currentItem.closest('ol');if(list)list.scrollTop=currentItem.offsetTop-list.offsetTop-80;
 }
 const raw=document.getElementById('activityData');if(!raw)return;
 const data=JSON.parse(raw.textContent),today=new Date(data.today+'T00:00:00Z');
 const monday=new Date(today);monday.setUTCDate(monday.getUTCDate()-((monday.getUTCDay()+6)%7));
 let week=new Date(monday),metric='materials',selected=0;
 const values=new Map(data.rows.map(r=>[r.metric+'|'+r.day,Number(r.n)]));
 const dates=data.rows.map(r=>r.day).sort();const minDate=dates.length?new Date(dates[0]+'T00:00:00Z'):new Date(monday);minDate.setUTCDate(minDate.getUTCDate()-((minDate.getUTCDay()+6)%7));
 if(minDate>monday)minDate.setTime(monday.getTime());
 const names=['Mon','Tue','Wed','Thu','Fri','Sat','Sun'];
 const labels={materials:'materials completed',quizzes:'quiz attempts submitted'};
 function dateAt(d,n){const v=new Date(d);v.setUTCDate(v.getUTCDate()+n);return v;}
 function key(d){return d.toISOString().slice(0,10);}
 function value(d){return values.get(metric+'|'+key(d))||0;}
 function fmt(d){return d.toLocaleDateString('en-GB',{day:'numeric',month:'short',year:'numeric',timeZone:'UTC'});}
 function draw(){
  const current=names.map((_,i)=>value(dateAt(week,i))),previous=names.map((_,i)=>value(dateAt(week,i-7))),max=Math.max(1,...current,...previous),total=current.reduce((a,b)=>a+b,0),old=previous.reduce((a,b)=>a+b,0),delta=total-old;
  document.getElementById('weekLabel').textContent=fmt(week)+' – '+fmt(dateAt(week,6));
  document.getElementById('activitySummary').textContent=total+' '+labels[metric]+' this week · '+(delta===0?'same as the previous week':Math.abs(delta)+(delta>0?' more':' fewer')+' than the previous week')+(total===0?'. Use Previous to explore earlier activity.':'.');
  const chart=document.getElementById('activityChart');chart.replaceChildren();
  names.forEach((name,i)=>{const b=document.createElement('button');b.type='button';b.className='bar-slot';b.setAttribute('aria-pressed',String(i===selected));b.setAttribute('aria-label',name+': '+current[i]+' '+labels[metric]+', previous week '+previous[i]);
   const back=document.createElement('span');back.className='bar-previous';back.style.height=(previous[i]/max*78)+'%';
   const front=document.createElement('span');front.className='bar-current';front.style.height=(current[i]/max*78)+'%';
   const count=document.createElement('b');count.className='bar-count';count.textContent=current[i];count.style.bottom='calc('+current[i]/max*78+'% + 10px)';
   const label=document.createElement('span');label.className='bar-label';label.textContent=name;b.append(back,front,count,label);b.addEventListener('click',()=>{selected=i;draw();});chart.append(b);
  });
  document.getElementById('dayDetail').textContent=names[selected]+': '+current[selected]+' '+labels[metric]+' · previous week: '+previous[selected];
  document.getElementById('nextWeek').disabled=week>=monday;document.getElementById('prevWeek').disabled=week<=minDate;
 }
 document.querySelectorAll('[data-metric]').forEach(b=>b.addEventListener('click',()=>{metric=b.dataset.metric;document.querySelectorAll('[data-metric]').forEach(x=>x.setAttribute('aria-pressed',String(x===b)));draw();}));
 document.getElementById('prevWeek').addEventListener('click',()=>{week=dateAt(week,-7);draw();});document.getElementById('nextWeek').addEventListener('click',()=>{week=dateAt(week,7);draw();});draw();
})();
