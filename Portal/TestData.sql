/* 権限マスタ */
create table 権限マスタ (権限id numeric primary key, 権限名 text, 更新日時 timestamp not null default current_timestamp);

create function set_update_time() returns opaque as 'begin 更新日時 := ''now''; return new; end; ' language 'plpgsql';

create trigger update_tri before update on 権限マスタ for each row execute procedure set_update_time();

insert into 権限マスタ values (0, '一般'); 
insert into 権限マスタ values (1, '管理者'); 



/* ユーザーマスタ */
create table ユーザーマスタ (ユーザーid varchar(20) primary key, 名前 text, パスワード text, 権限id numeric, 削除フラグ numeric, 更新日時 timestamp not null default current_timestamp, foreign key (権限id) references 権限マスタ (権限id));

insert into ユーザーマスタ values ('aiueo', '田中太郎', '$2a$11$p58noHMznd5kTgqbeo59ruq4qgjbMV8giXsVF8clis.iBj/EGH9b2', 1, 0); 



/* メールテーブル */
create table メールテーブル (送信者ユーザーid varchar(20), 受信者ユーザーid varchar(20), 件名 text, メッセージ text, 更新日時 timestamp not null default current_timestamp, foreign key (受信者ユーザーid) references ユーザーマスタ (ユーザーid), foreign key (送信者ユーザーid) references ユーザーマスタ (ユーザーid));



/*　勤怠テーブル */
create table 勤怠テーブル (ユーザーid varchar(20), 勤務開始時間 timestamp, 勤務終了時間 timestamp, 休憩時間 numeric, 更新日時 timestamp not null default current_timestamp, foreign key (ユーザーid) references ユーザーマスタ (ユーザーid));



/* 予定テーブル */
create table 予定テーブル (予定id smallserial primary key,ユーザーid varchar(20), 予定内容 varchar(10), 開始日時 timestamp, 終了日時 timestamp, 更新日時 timestamp not null default current_timestamp, foreign key (ユーザーid) references ユーザーマスタ (ユーザーid));

insert into 予定テーブル(ユーザーid,予定内容,開始日時,終了日時) values ('aiueo', '会議', '2020-03-31 09:00:00', '2020-03-31 17:45:00'); 



/* 施設テーブル */
create table 施設テーブル (施設id smallserial primary key,施設名 text, 開放開始時間 timestamp, 開放終了時間 timestamp, 更新日時 timestamp not null default current_timestamp);

insert into 施設テーブル(施設名,開放開始時間,開放終了時間) values ('施設を利用しない', '2020-01-01 00:00:00', '2020-01-01 23:59:59'); 
insert into 施設テーブル(施設名,開放開始時間,開放終了時間) values ('会議室1', '2020-01-01 09:00:00', '2020-01-01 19:00:00');
insert into 施設テーブル(施設名,開放開始時間,開放終了時間) values ('会議室2', '2020-01-01 09:00:00', '2020-01-01 19:00:00');
insert into 施設テーブル(施設名,開放開始時間,開放終了時間) values ('休憩室', '2020-01-01 08:00:00', '2020-01-01 20:00:00');



/* 施設利用状況テーブル */
create table 施設利用状況テーブル (ユーザーid varchar(20), 施設id smallint, 予定id smallint, 更新日時 timestamp not null default current_timestamp, , foreign key (ユーザーid) references ユーザーマスタ (ユーザーid), foreign key (施設id) references 施設テーブル (施設id));










