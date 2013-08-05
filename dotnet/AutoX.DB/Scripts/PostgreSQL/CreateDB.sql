-- Database: autox

-- DROP DATABASE autox;

CREATE DATABASE autox
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'English_United States.1252'
       LC_CTYPE = 'English_United States.1252'
       CONNECTION LIMIT = -1;


-- Table: content

-- DROP TABLE content;

CREATE TABLE content
(
  id text,
  data text
)
WITH (
  OIDS=FALSE
);
ALTER TABLE content
  OWNER TO postgres;

-- Index: id_idx

-- DROP INDEX id_idx;

CREATE INDEX id_idx
  ON content
  USING btree
  (id COLLATE pg_catalog."default");
ALTER TABLE content CLUSTER ON id_idx;



-- Table: relationship

-- DROP TABLE relationship;

CREATE TABLE relationship
(
  master text,
  type text,
  slave text
)
WITH (
  OIDS=FALSE
);
ALTER TABLE relationship
  OWNER TO postgres;

-- Index: c_idx

-- DROP INDEX c_idx;

CREATE INDEX c_idx
  ON relationship
  USING btree
  (master COLLATE pg_catalog."default", type COLLATE pg_catalog."default");

-- Index: master_key

-- DROP INDEX master_key;

CREATE INDEX master_key
  ON relationship
  USING btree
  (master COLLATE pg_catalog."default");

