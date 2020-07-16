/* �����}�X�^ */
create table �����}�X�^ (����id numeric primary key, ������ text, �X�V���� timestamp not null default current_timestamp);

create function set_update_time() returns opaque as 'begin �X�V���� := ''now''; return new; end; ' language 'plpgsql';

create trigger update_tri before update on �����}�X�^ for each row execute procedure set_update_time();

insert into �����}�X�^ values (0, '���'); 
insert into �����}�X�^ values (1, '�Ǘ���'); 



/* ���[�U�[�}�X�^ */
create table ���[�U�[�}�X�^ (���[�U�[id varchar(20) primary key, ���O text, �p�X���[�h text, ����id numeric, �폜�t���O numeric, �X�V���� timestamp not null default current_timestamp, foreign key (����id) references �����}�X�^ (����id));

insert into ���[�U�[�}�X�^ values ('aiueo', '�c�����Y', '$2a$11$p58noHMznd5kTgqbeo59ruq4qgjbMV8giXsVF8clis.iBj/EGH9b2', 1, 0); 



/* ���[���e�[�u�� */
create table ���[���e�[�u�� (���M�҃��[�U�[id varchar(20), ��M�҃��[�U�[id varchar(20), ���� text, ���b�Z�[�W text, �X�V���� timestamp not null default current_timestamp, foreign key (��M�҃��[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id), foreign key (���M�҃��[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id));



/*�@�ΑӃe�[�u�� */
create table �ΑӃe�[�u�� (���[�U�[id varchar(20), �Ζ��J�n���� timestamp, �Ζ��I������ timestamp, �x�e���� numeric, �X�V���� timestamp not null default current_timestamp, foreign key (���[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id));



/* �\��e�[�u�� */
create table �\��e�[�u�� (�\��id smallserial primary key,���[�U�[id varchar(20), �\����e varchar(10), �J�n���� timestamp, �I������ timestamp, �X�V���� timestamp not null default current_timestamp, foreign key (���[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id));

insert into �\��e�[�u��(���[�U�[id,�\����e,�J�n����,�I������) values ('aiueo', '��c', '2020-03-31 09:00:00', '2020-03-31 17:45:00'); 



/* �{�݃e�[�u�� */
create table �{�݃e�[�u�� (�{��id smallserial primary key,�{�ݖ� text, �J���J�n���� timestamp, �J���I������ timestamp, �X�V���� timestamp not null default current_timestamp);

insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('�{�݂𗘗p���Ȃ�', '2020-01-01 00:00:00', '2020-01-01 23:59:59'); 
insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('��c��1', '2020-01-01 09:00:00', '2020-01-01 19:00:00');
insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('��c��2', '2020-01-01 09:00:00', '2020-01-01 19:00:00');
insert into �{�݃e�[�u��(�{�ݖ�,�J���J�n����,�J���I������) values ('�x�e��', '2020-01-01 08:00:00', '2020-01-01 20:00:00');



/* �{�ݗ��p�󋵃e�[�u�� */
create table �{�ݗ��p�󋵃e�[�u�� (���[�U�[id varchar(20), �{��id smallint, �\��id smallint, �X�V���� timestamp not null default current_timestamp, , foreign key (���[�U�[id) references ���[�U�[�}�X�^ (���[�U�[id), foreign key (�{��id) references �{�݃e�[�u�� (�{��id));










