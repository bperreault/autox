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
