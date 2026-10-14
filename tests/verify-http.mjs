import fs from 'node:fs';
import assert from 'node:assert/strict';
const config=JSON.parse(fs.readFileSync(new URL('../Schoolnet.WEBUI/appsettings.Local.json',import.meta.url),'utf8').replace(/^\uFEFF/,''));
const base=process.env.TEST_BASE_URL||'http://localhost:5080';
let checks=0;
const pass=label=>{checks++;console.log('PASS: '+label);};
function token(html){const input=html.match(/<input[^>]*name="__RequestVerificationToken"[^>]*>/)?.[0];assert(input,'Antiforgery token present');return input.match(/value="([^"]+)"/)?.[1];}
class Session{
 constructor(){this.cookies=new Map();}
 async request(path,{method='GET',form}={}){
  const response=await fetch(base+path,{method,redirect:'manual',headers:{cookie:[...this.cookies].map(([k,v])=>k+'='+v).join('; '),...(form?{'content-type':'application/x-www-form-urlencoded'}:{})},body:form?new URLSearchParams(form):undefined});
  for(const raw of response.headers.getSetCookie()){const pair=raw.split(';')[0],index=pair.indexOf('=');this.cookies.set(pair.slice(0,index),pair.slice(index+1));}
  return {status:response.status,location:response.headers.get('location'),text:await response.text(),type:response.headers.get('content-type')};
 }
 async login(username){const page=await this.request('/Account/Login');assert.equal(page.status,200);const result=await this.request('/Account/Login',{method:'POST',form:{username,password:config.Bootstrap.Password,__RequestVerificationToken:token(page.text)}});assert.equal(result.status,302);assert.equal(result.location,'/');pass(username+' login');}
 async page(path){const result=await this.request(path);assert.equal(result.status,200,path+' returns 200');assert(!result.text.includes('Exception:'),path+' has no server exception');return result.text;}
}
const anonymous=new Session();assert.equal((await anonymous.request('/')).status,302);pass('Anonymous dashboard redirects to sign in');
const admin=new Session();await admin.login('admin');
const pages=['/','/Masters?kind=project','/Masters?kind=school','/Masters?kind=trainee','/Masters?kind=location','/Masters?kind=user','/Masters?kind=template','/Masters/Edit?kind=project','/Masters/Edit?kind=school','/Masters/Edit?kind=trainee','/Masters/Edit?kind=location','/Masters/Edit?kind=user','/Masters/Edit?kind=template','/Plans','/Plans/Edit','/Batches','/Batches/Edit','/Reports','/Reports/Audit'];
for(const path of pages){await admin.page(path);pass('Admin screen '+path);}
const planForm=await admin.page('/Plans/Edit');
assert(planForm.includes('value="4"'));assert.equal((planForm.match(/class="plan-day"/g)||[]).length,4);pass('New training plan defaults to exactly four activities');
const mismatched=await admin.request('/Plans/Edit',{method:'POST',form:{id:'0',plan_code:'__INVALID_ROW_COUNT',plan_name:'Invalid row count',number_of_days:'4',day_title:'Pre form',__RequestVerificationToken:token(planForm)}});
assert.equal(mismatched.status,200);assert(mismatched.text.includes('Provide exactly one activity for each training day.'));pass('Server rejects missing training day rows');
const invalidDuration=await admin.request('/Plans/Edit',{method:'POST',form:{id:'0',plan_code:'__INVALID_DURATION',plan_name:'Invalid duration',number_of_days:'366',__RequestVerificationToken:token(planForm)}});
assert.equal(invalidDuration.status,200);assert(invalidDuration.text.includes('Number of days must be between 2 and 365.'));pass('Server rejects out-of-range duration with bounded error page');
const batches=await admin.page('/Batches');
function batchId(code){const row=[...batches.matchAll(/<tr>([\s\S]*?)<\/tr>/g)].find(m=>m[1].includes(code));assert(row,code+' found');return row[1].match(/\/Batches\/Detail\/(\d+)/)[1];}
const ongoing=batchId('DEMO-B01'),completed=batchId('DEMO-B03'),draft=batchId('DEMO-B04');
const detail=await admin.page('/Batches/Detail/'+ongoing);assert(detail.includes('Day')&&detail.includes('Ananya Sharma'));pass('Batch detail shows shared activities and roster');
const planId=detail.match(/href="\/Plans\/Detail\/(\d+)/)?.[1]||(await admin.page('/Plans')).match(/href="\/Plans\/Detail\/(\d+)/)[1];await admin.page('/Plans/Detail/'+planId);pass('Shared plan detail');
await admin.page('/Batches/Edit/'+draft);await admin.page('/Attendance?id='+ongoing+'&day=3');pass('Draft editing and current day attendance');
const ownAssignment=detail.match(/\/Feedback\?assignment=(\d+)&(?:amp;)?day=1/)[1];await admin.page('/Feedback?assignment='+ownAssignment+'&day=1');pass('Versioned feedback form');
const csrf=await admin.request('/Batches/Transition',{method:'POST',form:{id:draft,status:'Scheduled'}});assert.equal(csrf.status,400);pass('POST without antiforgery rejected');
const invalidPage=await admin.page('/Masters/Edit?kind=project');const invalid=await admin.request('/Masters/Edit?kind=project',{method:'POST',form:{kind:'project',id:'0',name:'',code:'',active:'true',__RequestVerificationToken:token(invalidPage)}});assert.equal(invalid.status,200);assert(invalid.text.includes('Check the required fields'));pass('Invalid form returns a visible validation error');
const csv=await admin.request('/Reports?export=true');assert.equal(csv.status,200);assert(csv.type.startsWith('text/csv'));assert(csv.text.includes('Teacher Development Essentials'));pass('CSV export contains training records');
const coordinator=new Session();await coordinator.login('coordinator');const scoped=await coordinator.page('/Batches');assert(!scoped.includes('Digital Educators'));assert(scoped.includes('Classroom Champions'));pass('Coordinator sees only assigned project batches');assert.equal((await coordinator.request('/Batches/Detail/'+completed)).status,404);pass('Coordinator direct request outside project denied');
const traineeDirectory=await admin.page('/Masters?kind=trainee');
const traineeIds=[...traineeDirectory.matchAll(/href="\/Masters\/ModifyTrainee\/(\d+)"/g)].map(m=>m[1]);assert(traineeIds.length>0);pass('Modify trainee action appears in directory');
const coordinatorDirectory=await coordinator.page('/Masters?kind=trainee');
const ownTrainee=coordinatorDirectory.match(/href="\/Masters\/ModifyTrainee\/(\d+)"/)[1];
const foreignTrainee=traineeIds.find(id=>!coordinatorDirectory.includes('/Masters/ModifyTrainee/'+id+'"'));assert(foreignTrainee);
const modifyPath='/Masters/ModifyTrainee/'+ownTrainee;
const modifyPage=await coordinator.page(modifyPath);
const contact={name:modifyPage.match(/name="name"[^>]*value="([^"]*)"/)[1],mobile:modifyPage.match(/name="mobile"[^>]*value="([^"]*)"/)[1],email:modifyPage.match(/name="email"[^>]*value="([^"]*)"/)[1],reason:'HTTP verification of existing contact details'};
for(const [field,value,message] of [['name',' ','Enter a name'],['mobile','123','Enter a mobile number'],['email','bad','Enter a valid email'],['reason',' ','Enter a correction reason']]){
 const invalidContact=await coordinator.request(modifyPath,{method:'POST',form:{...contact,[field]:value,__RequestVerificationToken:token(modifyPage)}});assert.equal(invalidContact.status,200);assert(invalidContact.text.includes(message));
}pass('Contact correction validates all three fields and reason on server');
assert.equal((await coordinator.request(modifyPath,{method:'POST',form:contact})).status,400);pass('Contact correction requires antiforgery token');
assert.equal((await coordinator.request('/Masters/ModifyTrainee/'+foreignTrainee)).status,404);
assert.equal((await coordinator.request('/Masters/ModifyTrainee/'+foreignTrainee,{method:'POST',form:{...contact,__RequestVerificationToken:token(modifyPage)}})).status,404);pass('Coordinator contact GET and POST reject trainees outside assigned projects');
assert.equal((await admin.request('/Masters/ModifyTrainee/9223372036854775807')).status,404);pass('Unknown trainee modification returns 404');
const trainer=new Session();await trainer.login('trainer');await trainer.page('/Attendance?id='+ongoing+'&day=3');const adminOnly=await trainer.request('/Masters?kind=user');assert.equal(adminOnly.status,302);assert(adminOnly.location.includes('/Account/Denied'));pass('Trainer administrative access denied');
const trainerModify=await trainer.request(modifyPath);assert.equal(trainerModify.status,302);assert(trainerModify.location.includes('/Account/Denied'));pass('Trainer contact correction denied');
const futureBatch=batchId('DEMO-B02');
function attendanceSnapshot(html){return [...html.matchAll(/<tr>([\s\S]*?)<\/tr>/g)].filter(m=>m[1].includes('name="attendance_id"')).map(m=>({id:m[1].match(/name="attendance_id"[^>]*value="(\d+)"/)[1],revision:m[1].match(/name="revision"[^>]*value="(\d+)"/)[1],status:m[1].match(/<option[^>]*selected[^>]*>([^<]+)<\/option>/)[1]}));}
const futurePath='/Attendance?id='+futureBatch+'&day=1';
const futurePage=await admin.page(futurePath),before=attendanceSnapshot(futurePage);assert(before.length>0);
assert(futurePage.includes('Attendance cannot be marked for a future date.'));assert(!futurePage.includes('Save attendance'));assert(!futurePage.includes('id="mark-present"'));assert(!futurePage.includes('name="reason"'));
assert([...futurePage.matchAll(/<(?:select|input)[^>]*name="(?:status|remarks)"[^>]*>/g)].every(m=>m[0].includes('disabled')));pass('Future attendance controls are unavailable with a dated explanation');
for(const [role,session] of [['Administrator',admin],['Coordinator',coordinator],['Trainer',trainer]]){
 for(const status of ['Present','Absent']){
  const formPage=await session.page(futurePath);
  const save=await session.request('/Attendance/Save',{method:'POST',form:{id:futureBatch,day:'1',attendance_id:before[0].id,revision:before[0].revision,status,remarks:'Tampered future entry',reason:'Administrator override',start_date:'2000-01-01',date:'2000-01-01',__RequestVerificationToken:token(formPage)}});
  assert.equal(save.status,302);const rejected=await session.page(save.location);
  assert(rejected.includes('Attendance cannot be marked for a future date.'));assert.deepEqual(attendanceSnapshot(rejected),before);
 }
 pass(role+' cannot submit future Present or Absent through a tampered POST');
}
const invalidDay=await admin.request('/Attendance/Save',{method:'POST',form:{id:futureBatch,day:'0',__RequestVerificationToken:token(await admin.page(futurePath))}});assert.equal(invalidDay.status,404);pass('Attendance preflight rejects invalid stored batch day');
const learner=new Session();await learner.login('trainee');const personal=await learner.page('/Batches/Detail/'+ongoing);assert(personal.includes('Ananya Sharma'));assert(!personal.includes('Rahul Deshmukh'));pass('Trainee sees only own roster record');assert.equal((await learner.request('/Batches/Detail/'+completed)).status,404);pass('Trainee direct request for another batch denied');const otherAssignment=[...detail.matchAll(/\/Feedback\?assignment=(\d+)&(?:amp;)?day=1/g)].map(m=>m[1]).find(id=>id!==ownAssignment);assert.equal((await learner.request('/Feedback?assignment='+otherAssignment+'&day=1')).status,404);pass('Trainee cannot access another trainee feedback');const personalCsv=await learner.request('/Reports?export=true');assert(!personalCsv.text.includes('Rahul Deshmukh'));assert(personalCsv.text.includes('Ananya Sharma'));pass('Trainee export contains only own records');
console.log(`${checks} HTTP checks passed.`);
