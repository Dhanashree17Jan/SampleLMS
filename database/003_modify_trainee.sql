-- Non-destructive migration: contact corrections preserve roster, login and eligibility.
BEGIN;
CREATE OR REPLACE FUNCTION schoolnet.save_trainee_contact(p_id bigint,p_data jsonb,p_actor bigint)
RETURNS bigint LANGUAGE plpgsql AS $$
DECLARE t schoolnet.trainee; v_name text=btrim(p_data->>'name'); v_mobile text=btrim(p_data->>'mobile');
 v_email text=btrim(p_data->>'email'); v_reason text=btrim(p_data->>'reason');
BEGIN
 SELECT * INTO t FROM schoolnet.trainee WHERE id=p_id FOR UPDATE;
 IF NOT FOUND THEN RAISE EXCEPTION 'Trainee not found.'; END IF;
 PERFORM schoolnet.require_project(p_actor,t.project_id);
 IF coalesce(length(v_name),0) NOT BETWEEN 1 AND 200 THEN RAISE EXCEPTION 'Enter a name between 1 and 200 characters.'; END IF;
 IF v_mobile IS NULL OR v_mobile !~ '^\+?[0-9]{10,15}$' THEN RAISE EXCEPTION 'Enter a mobile number with 10 to 15 digits and an optional +.'; END IF;
 IF v_email IS NULL OR length(v_email)>254 OR v_email !~ '^[^[:space:]@]+@[^[:space:]@]+\.[^[:space:]@]+$' THEN RAISE EXCEPTION 'Enter a valid email address of up to 254 characters.'; END IF;
 IF coalesce(length(v_reason),0) NOT BETWEEN 1 AND 1000 THEN RAISE EXCEPTION 'Enter a correction reason between 1 and 1000 characters.'; END IF;
 PERFORM schoolnet.set_actor(p_actor,v_reason);
 UPDATE schoolnet.trainee SET name=v_name,mobile=v_mobile,email=v_email,updated_at=now() WHERE id=p_id;
 RETURN p_id;
END $$;
INSERT INTO schoolnet.schema_version(version) VALUES(3) ON CONFLICT(version) DO NOTHING;
COMMIT;
