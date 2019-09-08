use Rfid;
go

if object_id('administration.usp_insert_user', 'P') is not null
	drop procedure administration.usp_insert_user;
go

if object_id('administration.usp_insert_or_update_tag', 'P') is not null
	drop procedure administration.usp_insert_or_update_tag;
go

if object_id('administration.usp_insert_user_if_not_exists', 'P') is not null
	drop procedure administration.usp_insert_user_if_not_exists;
go

if object_id('administration.usp_insert_or_update_access_point', 'P') is not null
	drop procedure administration.usp_insert_or_update_access_point;
go

if object_id('administration.usp_replace_refresh_token', 'P') is not null
	drop procedure administration.usp_replace_refresh_token
go


if type_id ('dbo.IntList') is not null
	drop type dbo.IntList;
go

create type dbo.IntList as table
(
	[Value] int
);
go


create procedure administration.usp_insert_or_update_tag
	@number nvarchar(100),
	@level_id int,
	@is_active bit,
	@is_deleted bit,
	@user_id int,
	@identity int output
as
begin
	set nocount on;

	set @identity = (select top 1 x.Id from access_control.Tags as x where x.Number = @number)
	declare @added bit = iif(@identity is null or @identity = 0, 1, 0);

	if @added = 1
	begin
		-- modification date is updated with trigger after update of the row
		insert into access_control.Tags(IsActive, IsDeleted, LevelId, Number, UserId)
		values(@is_active, @is_deleted, @level_id, @number, @user_id)

		set @identity = SCOPE_IDENTITY();
	end
	else
	begin
		update access_control.Tags
		-- update any of the columns if update value was provided
		set 
			IsActive = iif(@is_active is not null, @is_active, IsActive),
			IsDeleted = iif(@is_deleted is not null, @is_deleted, IsDeleted),
			UserId = iif(@user_id is not null, @user_id, UserId)
	end

	return @added;
end;
go

create procedure administration.usp_insert_user_if_not_exists
	@username nvarchar(400),
	@identity int output
as
begin	
	set nocount on;

	set @identity = (select top 1 x.Name from access_control.Users as x where x.Name = @username);
	declare @added bit = iif(@identity is not null and @identity <> 0, 1, 0);

	if @identity is null or @identity = 0
	begin
		insert into access_control.Users([Name]) values(@username);
		set @identity = SCOPE_IDENTITY();
	end

	return @added;
end;
go

create procedure administration.usp_insert_or_update_access_point
	@identifier uniqueidentifier,
	@description nvarchar(max),
	@is_active bit,
	@level_id int,
	@identity int output
as
begin
	set nocount on;
	
	set @identity = (select top 1 x.Id from access_control.AccessPoints as x where x.Identifier = @identifier);
	declare @added bit = iif(@identity is null or @identity = 0, 1, 0);

	if @added = 1
	begin
		insert into access_control.AccessPoints(Identifier, [Description], IsActive, LevelId) 
		values(@identifier, @description, @is_active, @level_id);
		set @identity = SCOPE_IDENTITY();
	end
	else
	begin
		update access_control.AccessPoint
			set
				[Description] = iif(@description is not null, @description, [Description]),
				[IsActive] = iif(@is_active is not null, @is_active, IsActive),
				[LevelId] = iif(@level_id is not null, @level_id, LevelId)
	end

	return @added;
end;
go

create procedure administration.usp_insert_user
	@email nvarchar(400),
	@password_hash nvarchar(max),
	@roles dbo.IntList readonly,
	@identity int output
as
begin
	insert into administration.Users(Email, PasswordHash) values (@email, @password_hash)

	set @identity = SCOPE_IDENTITY();

	insert into administration.UsersRoles(UserId, RoleId)
	select
		@identity,
		r.[Value]
	from @roles as r
end;
go

create procedure administration.usp_replace_refresh_token
	@email nvarchar(400),
	@refresh_token nvarchar(100),
	@identity int output
as
begin
	set @identity = (select top 1 u.Id from administration.Users as u where u.Email = @email);
	declare @added bit;

	if @identity is not null and @identity <> 0
	begin
		if exists(select * from administration.RefreshTokens as rt where rt.Id = @identity)
		begin
			update administration.RefereshTokens
			set Token = @refresh_token
			where Id = @identity

			set @added = 0;
		end
		else
		begin
			insert into administration.RefreshTokens(Id, Token)  values(@identity, @refresh_token);

			set @added = 1;
		end
	end

	return @added;
end;
go