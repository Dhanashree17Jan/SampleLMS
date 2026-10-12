document.querySelector('.menu-toggle')?.addEventListener('click',()=>document.querySelector('.sidebar')?.classList.toggle('open'));
document.querySelectorAll('[data-confirm]').forEach(button=>button.addEventListener('click',event=>{if(!confirm(button.dataset.confirm))event.preventDefault();}));
const project=document.querySelector('#project-select'),school=document.querySelector('#school-select');
function schools(){if(!project||!school)return;for(const option of school.options){if(!option.value)continue;const projects=JSON.parse(option.dataset.projects||'[]');option.hidden=!!project.value&&!projects.map(String).includes(project.value);option.disabled=option.hidden;}if(school.selectedOptions[0]?.disabled)school.value='';updateSchoolLocation();}
function updateSchoolLocation(){const field=document.querySelector('#school-location');if(field)field.value=school?.selectedOptions[0]?.dataset.location||'';}
project?.addEventListener('change',schools);school?.addEventListener('change',updateSchoolLocation);schools();
const plan=document.querySelector('#batch-plan'),start=document.querySelector('#batch-start');
function dates(){if(!plan||!start)return;const days=Number(plan.selectedOptions[0]?.dataset.days);const duration=document.querySelector('#batch-duration'),end=document.querySelector('#batch-end');duration.value=days||'';if(days&&start.value){const date=new Date(start.value+'T00:00:00Z');date.setUTCDate(date.getUTCDate()+days-1);end.value=date.toISOString().slice(0,10);}else end.value='';}
plan?.addEventListener('change',dates);start?.addEventListener('change',dates);dates();
document.querySelector('#mark-present')?.addEventListener('click',()=>document.querySelectorAll('select[name="status"]:not(:disabled)').forEach(select=>select.value='Present'));
document.querySelector('#add-question')?.addEventListener('click',()=>{const root=document.querySelector('#questions'),rows=root.querySelectorAll('.question-row'),row=rows[0].cloneNode(true);row.querySelectorAll('input').forEach(input=>{if(input.type==='checkbox'){input.value=String(rows.length);input.checked=true;}else input.value='';});root.append(row);row.querySelector('input').focus();});
const planDays=document.querySelector('#plan-days'),dayList=document.querySelector('#plan-day-list');
// Keep removed draft rows available until navigation so changing duration does not lose entered work.
const draftDays=new Map(dayList?[...dayList.children].map(row=>[Number(row.dataset.day),row]):[]);
const blankDay=dayList?.firstElementChild.cloneNode(true);
blankDay?.querySelectorAll('input,select').forEach(field=>field.value='');
function synchronizePlanDays(){
 if(!planDays||!dayList)return false;
 const n=Number(planDays.value);
 if(!Number.isInteger(n)||n<2||n>365)return false;
 const previous=dayList.children.length;
 while(dayList.children.length>n)dayList.lastElementChild.remove();
 while(dayList.children.length<n){const day=dayList.children.length+1;const row=draftDays.get(day)||blankDay.cloneNode(true);draftDays.set(day,row);dayList.append(row);}
 for(let i=0;i<n;i++){
  const row=dayList.children[i],day=i+1,boundary=day===1||day===n;
  row.dataset.day=day;
  row.querySelector('legend').textContent='Day '+day+' · '+(day===1?'Pre form':boundary?'Feedback':'Classroom training');
  row.querySelector('.topic-label').hidden=boundary;
  row.querySelector('.template-label').hidden=!boundary;
  const title=row.querySelector('[name="day_title"]');
  if(n!==previous&&(!title.value||['Initial feedback','Final feedback','Pre form','Feedback'].includes(title.value)||/^Classroom training day \d+$/.test(title.value)))
   title.value=day===1?'Pre form':boundary?'Feedback':'Classroom training day '+day;
  // Retain the row's template choice for duration reversals; the server ignores it on classroom days.
 }
 document.querySelector('#plan-day-count').textContent=n+' days';
 return true;
}
planDays?.addEventListener('input',synchronizePlanDays);
planDays?.addEventListener('change',synchronizePlanDays);
document.querySelector('#plan-form')?.addEventListener('submit',event=>{
 if(!synchronizePlanDays()||dayList.children.length!==Number(planDays.value)){event.preventDefault();planDays.reportValidity();}
});
