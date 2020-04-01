create table ���[���e�[�u�� (���M�҃��[�U�[id varchar(20), ��M�҃��[�U�[id varchar(20), ���� text, ���b�Z�[�W text, �X�V���� timestamp not null default current_timestamp, foreign key (��M�҃��[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id), foreign key (���M�҃��[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id));

create function set_update_time() returns opaque as 'begin �X�V���� := ''now''; return new; end; ' language 'plpgsql';

create trigger update_tri before update on ���[���e�[�u�� for each row execute procedure set_update_time();


create table �����}�X�^ (����id numeric primary key, ������ text, �X�V���� timestamp not null default current_timestamp);

create trigger update_tri before update on �����}�X�^ for each row execute procedure set_update_time();


create table ���[�U�[�}�X�^ (���[�U�[id varchar(20) primary key, ��M�҃��[�U�[id varchar(20), ���O text, �p�X���[�h text, ����id numeric, �폜�t���O numeric, �X�V���� timestamp not null default current_timestamp, foreign key (����id) references �����}�X�^ (����id));

create trigger update_tri before update on ���[�U�[�}�X�^ for each row execute procedure set_update_time();



create table �\��e�[�u�� (�\��id smallserial primary key,���[�U�[id varchar(20), �\����e varchar(10), �J�n���� timestamp, �I������ timestamp, �X�V���� timestamp not null default current_timestamp, foreign key (���[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id));

create trigger update_tri before update on �\��e�[�u�� for each row execute procedure set_update_time();


create table �{�݃e�[�u�� (�{��id smallserial primary key,���[�U�[id varchar(20), �{�ݖ� text, �J���J�n���� timestamp, �J���I������ timestamp, �X�V���� timestamp not null default current_timestamp);

create trigger update_tri before update on �{�݃e�[�u�� for each row execute procedure set_update_time();


create table �{�ݗ��p�󋵃e�[�u�� (���[�U�[id varchar(20), �{��id smallint, �\��id smallint, �X�V���� timestamp not null default current_timestamp, , foreign key (���[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id), foreign key (�{��id) references �{�݃e�[�u�� (�{��id));

create trigger update_tri before update on �{�ݗ��p�󋵃e�[�u�� for each row execute procedure set_update_time();



insert into ���[�U�[�}�X�^ values ('aiueo', '�c�����Y', 'pass', 1, 0); 

insert into �����}�X�^ values (0, '���'); 
insert into �����}�X�^ values (1, '�Ǘ���'); 

insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('�{�݂𗘗p���Ȃ�', '2020-01-01 00:00:00', '2020-01-01 23:59:59'); 
insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('��c��1', '2020-01-01 09:00:00', '2020-01-01 19:00:00');
insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('��c��2', '2020-01-01 09:00:00', '2020-01-01 19:00:00');
insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('�x�e��', '2020-01-01 08:00:00', '2020-01-01 20:00:00');
