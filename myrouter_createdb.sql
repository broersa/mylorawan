create table brokers (
  brokey bigserial primary key,
  broname varchar(100) unique not null,
  broendpoint varchar(100) unique not null
)

