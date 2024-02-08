begin transaction chatify_database_seeds

begin try
    if not exists (select * from chatify.Accounts)
        begin
            insert into chatify.Accounts (Id, WorkspaceId, Name, Avatar, Type, Active)
            values ('chatify::system', 'chatify::system', 'System', null, 1, 1),
                   ('1-1', '1', 'James Doe', '/api/avatar/1', 2, 1),
                   ('1-2', '1', 'Kevin Hearth', '/api/avatar/2', 2, 1),
                   ('1-3', '1', 'Colin Wolshire', '/api/avatar/3', 2, 1),
                   ('2-1', '2', 'Annabeth Smith', '/api/avatar/4', 2, 1),
                   ('2-2', '2', 'Anna Crossfort', '/api/avatar/5', 2, 1),
                   ('2-3', '2', 'Jessica Parker', '/api/avatar/6', 2, 1)
        end

    commit transaction chatify_database_seeds
end try
begin catch
    rollback transaction chatify_database_seeds;
    throw
end catch