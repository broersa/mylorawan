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

create table nwkaddrs (
  nwkkey bigserial primary key,
  nwkaddr bigint check (nwkaddr >= 0 and nwkaddr < '16777216'::bigint) not null
);

DO
$do$
BEGIN
FOR i IN 0..10000 LOOP
    insert into nwkaddrs (nwkaddr) values (i);
END LOOP;
END
$do$;

create table sessions (
  seskey bigserial primary key,
  sesdev bigint references devices (devkey),
  sesnwk bigint references nwkaddrs (nwkkey),
  sesdevnonce varchar(4) not null,
  sesappnonce varchar(6) not null,
  sesnwkskey varchar(32) not null,
  sesappskey varchar(32) not null,
  sesactive timestamp with time zone not null
);
