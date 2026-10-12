-- Optional local demo only. Seed once; leave existing application data untouched.
BEGIN;
SET LOCAL search_path TO schoolnet, public;
SELECT set_config('schoolnet.demo_hash',@hash,true);
DO $$ DECLARE admin_id bigint; trainer bigint; coordinator bigint; learner bigint; p1 bigint; p2 bigint;
 st bigint; dist bigint; blk bigint; cl bigint; s1 bigint; s2 bigint; s3 bigint; ft bigint; plan_id bigint; b1 bigint; b2 bigint; b3 bigint; b4 bigint; person bigint; participants bigint[]; payload jsonb; today date=(now() AT TIME ZONE 'Asia/Kolkata')::date;
BEGIN
 IF EXISTS(SELECT 1 FROM project WHERE code='DEMO-TEACH') THEN RETURN; END IF;
 SELECT id INTO admin_id FROM app_user WHERE role='Administrator' AND active ORDER BY id LIMIT 1;
 PERFORM set_actor(admin_id,'Optional local demonstration data');
 INSERT INTO app_user(username,name,password_hash,role) VALUES('trainer','Demo Trainer',current_setting('schoolnet.demo_hash'),'Trainer') RETURNING id INTO trainer;
 INSERT INTO app_user(username,name,password_hash,role) VALUES('coordinator','Demo Coordinator',current_setting('schoolnet.demo_hash'),'Coordinator') RETURNING id INTO coordinator;
 INSERT INTO app_user(username,name,password_hash,role) VALUES('trainee','Demo Trainee',current_setting('schoolnet.demo_hash'),'Trainee') RETURNING id INTO learner;
 p1=save_master('project',0,'{"code":"DEMO-TEACH","name":"Teacher Excellence","description":"Build confident classrooms through practical teacher development.","active":true}',admin_id);
 p2=save_master('project',0,'{"code":"DEMO-DIGITAL","name":"Digital Learning Initiative","description":"Bring technology-enabled learning to every school.","active":true}',admin_id);
 INSERT INTO user_project VALUES(coordinator,p1);
 st=save_master('location',0,'{"name":"Maharashtra","kind":"State","parent_id":""}',admin_id);
 dist=save_master('location',0,jsonb_build_object('name','Pune','kind','District','parent_id',st),admin_id);
 blk=save_master('location',0,jsonb_build_object('name','Haveli','kind','Block','parent_id',dist),admin_id);
 cl=save_master('location',0,jsonb_build_object('name','Central cluster','kind','Cluster','parent_id',blk),admin_id);
 s1=save_master('school',0,jsonb_build_object('code','DEMO-S01','name','Vidya Mandir School','cluster_id',cl,'projects',jsonb_build_array(p1,p2),'active',true),admin_id);
 s2=save_master('school',0,jsonb_build_object('code','DEMO-S02','name','Government Model School','cluster_id',cl,'projects',jsonb_build_array(p1),'active',true),admin_id);
 s3=save_master('school',0,jsonb_build_object('code','DEMO-S03','name','Sunrise Public School','cluster_id',cl,'projects',jsonb_build_array(p2),'active',true),admin_id);
 ft=save_master('template',0,'{"name":"Teacher training reflection","questions":[{"id":"q1","label":"How confident are you with classroom engagement?","type":"Rating","required":true,"choices":[]},{"id":"q2","label":"Which approach will you apply in your classroom?","type":"Choice","required":true,"choices":["Activity-based learning","Digital resources","Peer collaboration"]},{"id":"q3","label":"What support would help you most?","type":"Text","required":false,"choices":[]}]}',admin_id);
 plan_id=save_plan(0,jsonb_build_object('plan_code','DEMO-TTP','plan_name','Teacher Development Essentials','number_of_days',5,'description','A five-day journey from reflection to classroom practice.','template_id',ft),admin_id);
 UPDATE trainingplandetails SET topics=CASE day_number WHEN 2 THEN 'Learner engagement and inclusive teaching' WHEN 3 THEN 'Digital tools and classroom activities' ELSE 'Assessment and collaborative lesson design' END WHERE training_plan_id=plan_id AND activity_type='Classroom training';
 CALL publish_plan(plan_id,admin_id);
 FOR i IN 1..8 LOOP
 person=save_master('trainee',0,jsonb_build_object('username','demo.teacher'||i,'name',(ARRAY['Ananya Sharma','Rahul Deshmukh','Priya Nair','Amit Joshi','Meera Patil','Sanjay Rao','Kavita Singh','Arjun Verma'])[i],'mobile','98765432'||lpad(i::text,2,'0'),'email','demo.teacher'||i||'@example.com','project_id',CASE WHEN i<=6 THEN p1 ELSE p2 END,'school_id',CASE WHEN i<=3 THEN s1 WHEN i<=6 THEN s2 ELSE s3 END,'user_id',CASE WHEN i=1 THEN learner::text ELSE '' END,'active',true),admin_id);
 END LOOP;
 payload=jsonb_build_object('code','DEMO-B01','name','Pune · Classroom Champions','project_id',p1,'school_id',s1,'training_plan_id',plan_id,'number_of_days',5,'start_date',today-2,'end_date',today+2,'trainer_id',trainer);
 b1=save_batch(0,payload,admin_id); SELECT array_agg(id) INTO participants FROM trainee WHERE school_id=s1;
 CALL assign_trainees(b1,participants,admin_id); CALL transition_batch(b1,'Scheduled','',admin_id);
 FOR day IN 1..3 LOOP
 SELECT jsonb_agg(jsonb_build_object('id',a.id,'revision',a.revision,'status','Present','remarks','')) INTO payload FROM attendance a JOIN batch_assignment ba ON ba.id=a.assignment_id JOIN trainingplandetails d ON d.training_plan_detail_id=a.training_plan_detail_id WHERE ba.batch_id=b1 AND d.day_number=day;
 CALL save_attendance(b1,day,payload,'Demo attendance entered by administrator',admin_id); END LOOP;
 SELECT id INTO person FROM batch_assignment WHERE batch_id=b1 ORDER BY id LIMIT 1;
 CALL submit_feedback(person,1,'{"q1":"4","q2":"Activity-based learning","q3":"More practical classroom examples"}','Offline demo response',admin_id);
 payload=jsonb_build_object('code','DEMO-B02','name','Haveli · Learning Together','project_id',p1,'school_id',s2,'training_plan_id',plan_id,'number_of_days',5,'start_date',today+14,'end_date',today+18,'trainer_id',trainer);
 b2=save_batch(0,payload,admin_id); SELECT array_agg(id) INTO participants FROM trainee WHERE school_id=s2;
 CALL assign_trainees(b2,participants,admin_id); CALL transition_batch(b2,'Scheduled','',admin_id);
 payload=jsonb_build_object('code','DEMO-B03','name','Digital Educators · Foundation','project_id',p2,'school_id',s3,'training_plan_id',plan_id,'number_of_days',5,'start_date',today-20,'end_date',today-16,'trainer_id',trainer);
 b3=save_batch(0,payload,admin_id); SELECT array_agg(id) INTO participants FROM trainee WHERE school_id=s3;
 CALL assign_trainees(b3,participants,admin_id); CALL transition_batch(b3,'Scheduled','',admin_id);
 FOR day IN 1..5 LOOP
 SELECT jsonb_agg(jsonb_build_object('id',a.id,'revision',a.revision,'status','Present','remarks','')) INTO payload FROM attendance a JOIN batch_assignment ba ON ba.id=a.assignment_id JOIN trainingplandetails d ON d.training_plan_detail_id=a.training_plan_detail_id WHERE ba.batch_id=b3 AND d.day_number=day;
 CALL save_attendance(b3,day,payload,'Historical demo attendance',admin_id); END LOOP;
 CALL transition_batch(b3,'Completed','',admin_id);
 payload=jsonb_build_object('code','DEMO-B04','name','Teacher Excellence · Next Cohort','project_id',p1,'school_id',s1,'training_plan_id',plan_id,'number_of_days',5,'start_date',today+30,'end_date',today+34,'trainer_id',trainer);
 b4=save_batch(0,payload,admin_id);
END $$;
COMMIT;
