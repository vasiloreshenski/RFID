use master;
go

drop database Rfid;
go

create database Rfid;
go

use Rfid;
go

-- access control

create schema access_control;
go

-- create level types

create table access_control.AccessLevels
(
	Id int not null identity(1, 1),
	[Name] nvarchar(100) not null

	constraint PK_access_control_AccessLevel primary key(Id)
);
go

create index idx_access_control_AccessLevel on access_control.AccessLevels([Name]);
go

insert into access_control.AccessLevels([Name]) values ('Low'), ('Mid'), ('High');
go

-- create access points
create table access_control.AccessPoints
(
	Id int not null identity(1, 1),
	[Description] nvarchar(max) not null,
	Identifier uniqueidentifier not null,
	IsActive bit not null,
	CreateDate datetime2 not null default(GETDATE()),
	ModificationDate datetime2 null,
	LevelId int not null

	constraint PK_access_control_AccesPoints primary key (Id),
	constraint FK_access_control_AccessPoints_Access_Level foreign key (LevelId) references access_control.AccessLevels(Id)
);
go

create unique index uidx_access_control_AccessPoints_Identifier on access_control.AccessPoints(Identifier);
create index idx_access_control_AccessPoints_LevelId on access_control.AccessPoints(LevelId);
go

create trigger tr_access_control_AccessPoints_ModificationDate 
on access_control.AccessPoints
after update
as
	update access_control.AccessPoints
	set ModificationDate = GETDATE()
	where Id in (select x.Id from inserted as x)
go

-- create Tag users

create table access_control.Users
(
	Id int not null identity(1, 1),
	[Name] nvarchar(400) not null

	constraint PK_access_control_Users primary key(Id)
);
go

create index idx_access_control_Users_Name on access_control.Users([Name]);
go


-- create Tags

create table access_control.Tags
(
	Id int not null identity(1, 1),
	[Number] nvarchar(100) not null,
	[LevelId] int not null,
	[IsActive] bit not null,
	[IsDeleted] bit not null,
	[CreateDate] datetime2 not null default(GETDATE()),
	[ModificationDate] datetime2 null,
	UserId int not null

	constraint PK_access_control_Tags primary key(Id),
	constraint FK_access_control_Tags_AccessLevel foreign key(LevelId) references access_control.AccessLevels(Id),
	constraint FK_access_control_Tags_Users foreign key(UserId) references access_control.Users(Id)
);
go

create unique index idx_access_control_Tags_Number on access_control.Tags(Number);
create index idx_access_control_Tags_LevelId on access_control.Tags(LevelId);
create index idx_access_control_Tags_IsActive on access_control.Tags(IsActive);
create index idx_access_control_Tags_IsDeleted on access_control.Tags(IsDeleted);
create index idx_access_control_Tags_CreateDate on access_control.Tags(CreateDate);
create index idx_access_control_Tags_ModificationDate on access_control.Tags(ModificationDate);
go

create trigger tr_access_control_Tags_ModificationDate
on access_control.Tags
after update
as
	update access_control.Tags
	set ModificationDate = GETDATE()
	where Id in (select x.Id from inserted as x);

go

-- create event types

create table access_control.EventTypes
(
	Id int not null identity(1, 1),
	[Name] nvarchar(100) not null

	constraint PK_access_control_EventTypes primary key(Id)
);
go

create index idx_access_control_EventTypes on access_control.EventTypes([Name]);
go

-- create events

create table access_control.[Events]
(
	Id int not null identity(1, 1),
	TagId int not null,
	EventTypeId int not null,
	CreateDate datetime2 default(GETDATE()),
	[Message] nvarchar(400) null

	constraint PK_access_control_Events primary key(Id),
	constraint FK_access_control_Events_Tags foreign key(TagId) references [access_control].[Tags],
	constraint FK_access_control_Events_EventTypes foreign key(EventTypeId) references [access_control].[EventTypes]
);
go

create index idx_access_control_Events_TagId on access_control.Events(TagId);
create index idx_access_control_Events_EventTypeId on access_control.Events(EventTypeId);
create index idx_access_control_Events_CreateDate on access_control.Events(CreateDate);
go

-- administration

create schema administration;
go

-- create user roles

create table administration.Roles
(
	Id int not null,
	[Name] nvarchar(400) not null,

	constraint PK_administration_Roles primary key (id)
);
go

create unique index uidx_administration_Roles_Name on administration.Roles([Name]);
go

insert into administration.Roles(Id, [Name]) values (1, 'Admin');
go

-- create users

create table administration.Users
(
	Id int not null identity (1, 1),
	Email nvarchar(400) not null,
	PasswordHash nvarchar(max) not null,
	CreateDate datetime2 not null default(GETDATE()),
	ModificationDate datetime2 null

	constraint PK_administration_Users primary key(Id),
);
go

create unique index uidx_administration_Users_Email on administration.Users(Email);
go

create trigger tr_administration_Users_ModificationDate
on administration.Users
after update
as
	update administration.Users
	set ModificationDate = GETDATE()
	where Id in (select i.Id from inserted as i);
go

-- create user - user roles relation

create table administration.UsersRoles
(
	UserId int not null,
	RoleId int not null,

	constraint PK_administration_UserRoles primary key(UserId, RoleId)
);
go

-- create user refresh tokens

create table administration.RefreshTokens
(
	Id int not null,
	Token nvarchar(100) not null

	constraint PK_administration_RefreshToken primary key (Id),
	constraint FK_administration_RefereshToken_Users foreign key (Id) references administration.Users(Id)
);
go


-- functions

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


-- procedures

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
		update access_control.AccessPoints
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
			update administration.RefreshTokens
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