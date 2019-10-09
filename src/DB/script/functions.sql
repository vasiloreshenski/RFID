use Rfid;
go

if object_Id('access_control.f_get_access_level_for_access_point', 'FN') is not null
	drop function access_control.f_get_access_level_for_access_point;
go

if object_id('access_control.f_get_access_level_for_tag', 'FN') is not null
	drop function access_control.f_get_access_level_for_tag;
go

if object_id('administration.f_get_user', 'IF') is not null
	drop function administration.f_get_user;
go

create function access_control.f_get_access_level_for_access_point(@identifier uniqueidentifier)
returns int
as
begin	
	declare @access_level int;

	select
		@access_level = ap.LevelId
	from access_control.AccessPoints as ap
	where ap.Identifier = @identifier

	return @access_level;
end
go

create function access_control.f_get_access_level_for_tag(@tag_id int)
returns int
as
begin
	declare @access_level int;

	select
		@access_level = t.Id
	from access_control.Tags as t
	where t.Id = @tag_id

	return @access_level;
end
go

create function administration.f_get_user(@email nvarchar(400))
returns table
as
return
(
	select
		u.Email,
		u.PasswordHash,
		SUM(ur.RoleId) as [RoleId],
		rt.Token as [RefreshToken]
	from administration.Users as u
	join administration.UsersRoles as ur on u.Id = ur.UserId
	join administration.RefreshTokens as rt on u.Id = rt.Id
	where u.Email = @email
	group by u.Email, u.PasswordHash, rt.Token
);
go
