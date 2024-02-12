create DATABASE factstore;
create user factstore with encrypted password '$(password)';
grant all privileges on database factstore to factstore;