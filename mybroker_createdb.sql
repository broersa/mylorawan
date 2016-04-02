create table applications (
  appkey bigserial primary key,
  appname varchar(50) unique not null,
  appeui varchar(16) unique not null
);

create table devices (
  devkey bigserial primary key,
  devapp bigint references applications (appkey),
  deveui varchar(16) unique not null,
  devappkey varchar(32) unique not null
);

create table devaddrs (
  deakey bigserial primary key,
  deanwkid bigint not null,
  deanwkaddr bigint check (deanwkaddr >= 0 and deanwkaddr < '16777216'::bigint) not null
);

DO
$do$
BEGIN
FOR i IN 0..10000 LOOP
    insert into devaddrs (deanwkid, deanwkaddr) values (0, i);
END LOOP;
END
$do$;

create table sessions (
  seskey bigserial primary key,
  sesdev bigint references devices (devkey),
  sesdea bigint references devaddrs (deakey),
  sesdevnonce varchar(4) not null,
  sesappnonce varchar(6) not null,
  sesnwkskey varchar(32) not null,
  sesappskey varchar(32) not null,
  sesactive timestamp with time zone not null
);
