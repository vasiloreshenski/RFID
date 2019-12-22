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

create table access_control.Direction
(
	Id int not null identity(1, 1),
	[Name] nvarchar(100) not null

	constraint PK_access_control_Direction primary key (Id),
);
go

create unique index uindx_access_control_Direction_Name on access_control.Direction([Name]);
go

insert into access_control.Direction([Name]) values ('Entrance'), ('Exit');
go

-- create access points
create table access_control.AccessPoints
(
	Id int not null identity(1, 1),
	[Description] nvarchar(max) not null,
	SerialNumber nvarchar(400) not null,
	IsActive bit not null,
	IsDeleted bit not null,
	CreateDate datetime2 not null default(GETDATE()),
	ModificationDate datetime2 null,
	LevelId int not null,
	DirectionId int not null

	constraint PK_access_control_AccesPoints primary key (Id),
	constraint FK_access_control_AccessPoints_Access_Level foreign key (LevelId) references access_control.AccessLevels(Id),
	constraint FK_access_control_AccessPoints_Direction foreign key(DirectionId) references access_control.Direction(Id)
);
go

create unique index uidx_access_control_AccessPoints_SerialNumber on access_control.AccessPoints(SerialNumber);
create index idx_access_control_AccessPoints_Level on access_control.AccessPoints(LevelId);
create index idx_access_control_AccessPoints_Direction on access_control.AccessPoints(DirectionID);
go

create trigger tr_access_control_AccessPoints_ModificationDate 
on access_control.AccessPoints
after update
as
	update access_control.AccessPoints
	set ModificationDate = GETDATE()
	where Id in (select x.Id from inserted as x)
go

-- create unknown access points

create table access_control.UnKnownAccessPoints
(
	Id int not null identity(1, 1),
	SerialNumber nvarchar(400) not null,
	AccessDate datetime2 not null,
	IsDeleted bit not null

	constraint PK_access_control_UnknownAccessPoints primary key (Id)
);
go

create unique index uidx_access_control_UnknonwnAccessPoints_SerialNumber on access_control.UnKnownAccessPoints(SerialNumber);
go

create trigger tr_access_control_UnKnownAccessPoints_AccessDate
on access_control.UnKNownAccessPoints
after update
as
	update access_control.UnKnownAccessPoints
	set AccessDate = GETDATE()
	where Id in (select x.Id from inserted as x);
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

create table access_control.UnknownTags
(
	Id int not null identity(1, 1),
	Number nvarchar(100) not null,
	AccessDate datetime2 not null,
	IsDeleted bit not null

	constraint PK_access_control_UnknownTags primary key (Id)
);

create unique index uindx_access_control_UnknownTags_Number on access_control.UnknownTags(Number);
go

create trigger tr_access_control_UnknownTags_AccessDate
on access_control.UnknownTags
after update
as
	update access_control.UnknownTags
	set AccessDate = GETDATE()
	where Id in (
		select x.Id from inserted as x
	)
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

--insert into administration.Users(Email, PasswordHash) 
--values ('test@test.com', 'AQAAAAEAACcQAAAAELVbscf+FDZwR7Y2Ii9b4zH+lOl2JjvTYKpaIiGbm1j3EaGXMUh4c6sM3/OQDCs5uQ==');
--go

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

	constraint PK_administration_UsersRoles primary key(UserId, RoleId),
	constraint FK_administration_UsersRoles foreign key (UserId) references administration.Users(Id) on delete cascade,
	constraint FK_administration_Users_Roles foreign Key (RoleId) references administration.Roles(Id)
);
go

-- create user refresh tokens

create table administration.RefreshTokens
(
	Id int not null,
	Token nvarchar(100) not null

	constraint PK_administration_RefreshToken primary key (Id),
	constraint FK_administration_RefereshToken_Users foreign key (Id) references administration.Users(Id) on delete cascade
);
go

-- stat

create schema stat;
go


-- create events

create table stat.[Events]
(
	Id int not null identity(1, 1),
	TagNumber nvarchar(100) not null,
	TagLevelId int null,
	TagIsActive bit null,
	TagIsDeleted bit null,
	TagIsUnknown bit not null,
	UserId int null,
	AccessPointSerialNumber nvarchar(400) not null,
	AccessPointLevelId int null,
	AccessPointDirectionId int null,
	AccessPointIsActive bit null,
	AccessPointIsDeleted bit null,
	AccessPointIsUnknown bit not null,
	CreateDate datetime2 not null default(GETDATE()),

	constraint PK_stat_Events primary key(Id),
	constraint FK_stat_Events_TagLevelId foreign key(TagLevelId) references access_control.AccessLevels(Id),
	constraint FK_stat_Events_UserId foreign key(UserId) references access_control.Users(Id),
	constraint FK_stat_Events_AccessPointLevelId foreign key(AccessPointLevelId) references access_control.AccessLevels(Id),
	constraint FK_stat_Events_AccessPointDirectionId foreign key(AccessPointDirectionId) references access_control.Direction(id) 
	
);
go

create index idx_stat_Events_TagNumber on stat.[Events](TagNumber);
create index idx_stat_Events_TagLevelId on stat.[Events](TagLevelId);
create index idx_stat_Evnets_TagIsActive on stat.[Events](TagIsActive);
create index idx_stat_Events_TagIsDeleted on stat.[Events](TagIsDeleted);
create index idx_stat_Events_TagIsUnknown on stat.[Events](TagIsUnknown);
create index idx_stat_Events_UserId on stat.[Events](UserId);
create index idx_stat_Events_AccessPointSerialNumber on stat.[Events](AccessPointSerialNumber);
create index idx_stat_Events_AccessPointLevelId on stat.[Events](AccessPointLevelId);
create index idx_stat_Events_AccessPointDirectionId on stat.[Events](AccessPointDirectionId);
create index idx_stat_Events_AccessPointIsActive on stat.[Events](AccessPointIsActive);
create index idx_stat_Events_AccessPointIsDeleted on stat.[Events](AccessPointIsDeleted);
create index idx_stat_Events_AccessPointIsUnknown on stat.[Events](AccessPointIsUnknown);
go

-- log

create schema [log];
go

create table [log].[Type]
(
	Id int not null identity(1, 1),
	[Name] nvarchar(100) not null

	constraint PK_log_type primary key(Id)
);
go

create table [log].Client
(
	Id int not null identity(1, 1),
	CreateDate datetime2 not null default(getdate()),
	[TypeId] int not null,
	[Message] nvarchar(max) not null

	constraint PK_log_client primary key(Id),
	constraint FK_log_client_type foreign key (TypeId) references [log].[Type]
);
go

insert into [log].[Type]([Name]) values ('Error');
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

if object_id('access_control.f_get_access_points', 'IF') is not null
	drop function access_control.f_get_access_points;
go

if object_id('access_control.f_get_unknown_access_points', 'IF') is not null
	drop function access_control.f_get_unknown_access_points;
go

if object_id('access_control.f_get_tags', 'IF') is not null
	drop function access_control.f_get_tags;
go

if object_id('access_control.f_get_users', 'IF') is not null
	drop function access_control.f_get_users;
go

if object_id('access_control.f_get_unknown_tags', 'IF') is not null
	drop function access_control.f_get_unknown_tags;
go

if object_id('access_control.f_check_access', 'FN') is not null
	drop function access_control.f_check_access;
go

if object_id('stat.f_get_avg_entrance_time_in_minutes', 'IF') is not null
	drop function stat.f_get_avg_entrance_time;
go

if object_id('stat.f_get_avg_entrance_time_for_userin_minutes_', 'IF') is not null
	drop function stat.f_get_avg_entrance_time_for_user;
go

if object_id('stat.f_get_entrance_time_for_user', 'IF') is not null
	drop function stat.f_get_entrance_time_for_user;
go

if object_id('stat.f_get_avg_exit_time_in_minutes', 'IF') is not null
	drop function stat.f_get_avg_exit_time;
go

if object_id('stat.f_get_avg_exit_time_for_user_in_minutes', 'IF') is not null
	drop function stat.f_get_avg_exit_time_for_user;
go

if object_id('stat.f_get_exit_time_for_user', 'IF') is not null
	drop function stat.f_get_exit_time_for_user;
go

if object_id('stat.f_get_avg_work_hour_norm_in_minutes', 'IF') is not null
	drop function stat.f_get_avg_work_hour_norm_in_minutes;
go

if object_id('stat.f_get_work_hour_norm_in_minutes', 'IF') is not null
	drop function stat.f_get_work_hour_norm_in_minutes;
go

if object_id('stat.f_get_avg_work_hour_norm_for_user_in_minutes', 'IF') is not null
	drop function stat.f_get_avg_work_hour_norm_for_user;
go

if object_id('stat.f_convert_datetime2_hour_to_float', 'FN') is not null
	drop function stat.f_convert_datetime2_hour_to_float;
go

if object_id('stat.f_get_datetime2_time_as_seconds', 'FN') is not null
	drop function stat.f_get_datetime2_time_as_seconds;
go

if object_id('stat_f_get_users', 'IF') is not null
	drop function stat_f_get_users;
go

if object_id('stat.f_date_in_range', 'FN') is not null
	drop function stat.f_date_in_range;
go

create function stat.f_date_in_range(@date datetime2, @start_date datetime2, @end_date datetime2)
returns bit
as
begin
	return	case
			when @start_date is not null and @end_date is not null then iif(cast(@date as date) >= cast(@start_date as date) and cast(@date as date) <= cast(@end_date as date), 1, 0)
			when @start_date is not null then  iif(cast(@date as date) >= cast(@start_date as date), 1, 0)
			when @end_date is not null then iif(cast(@date as date) <= cast(@end_date as date), 1, 0)
			else 1
			end
end;
go


create function access_control.f_get_access_level_for_access_point(@serial_number nvarchar(400))
returns int
as
begin	
	declare @access_level int;

	select
		@access_level = ap.LevelId
	from access_control.AccessPoints as ap
	where ap.SerialNumber = @serial_number

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

create function access_control.f_get_access_points(@page int, @page_size int, @is_active bit, @is_deleted bit)
returns table
as
return
(
	with access_points
	as
	(
		select
			ap.Id,
			ap.SerialNumber,
			ap.[Description],
			ap.LevelId,
			ap.DirectionId,
			ap.CreateDate,
			ap.ModificationDate,
			ap.IsActive,
			ap.IsDeleted
		from access_control.AccessPoints as ap
		where iif(@is_active is null, 1, iif(@is_active = ap.IsActive, 1, 0)) = 1
		and iif(@is_deleted is null, 1, iif(@is_deleted = ap.IsDeleted, 1, 0)) = 1
	)

	select
		ap.Id,
		ap.SerialNumber,
		ap.[Description],
		ap.LevelId,
		ap.DirectionId,
		ap.CreateDate,
		ap.ModificationDate,
		ap.IsActive,
		ap.IsDeleted
	from access_points as ap
	order by ap.CreateDate
	offset iif(@page is null or @page_size is null, 0, @page * @page_size) rows
	fetch next iif(
		@page_size is null,
		iif(exists(select * from access_points), (select count(*) from access_points), 1),
		iif(@page_size <= 0, 1, @page_size)
	) rows only
);
go

create function access_control.f_get_unknown_access_points(@page int, @page_size int)
returns table
as
return
(
	with access_points
	as
	(
		select
			uap.Id,
			uap.SerialNumber,
			uap.AccessDate
		from access_control.UnKnownAccessPoints as uap
		where uap.IsDeleted = 0
	)

	select
		uap.Id,
		uap.SerialNumber,
		uap.AccessDate
	from access_points as uap
	order by uap.AccessDate
	offset iif(@page is null or @page_size is null, 0, @page * @page_size) rows
	fetch next iif(@page_size is null, 
		iif(exists(select * from access_points), (select count(*) from access_points), 1),
		iif(@page_size <= 0, 1, @page_size)
	) rows only
);
go

create function access_control.f_get_tags(@page int, @page_size int, @is_active bit, @is_deleted bit)
returns table
as
return
(
	with tags
	as
	(
		select
			t.Id,
			t.CreateDate,
			t.IsActive,
			t.IsDeleted,
			t.LevelId,
			t.ModificationDate,
			t.Number,
			u.[Name] as [UserName]
		from access_control.Tags as t
		join access_control.Users as u on t.UserId = u.Id
		where (iif(@is_active is null, 1, iif(@is_active = t.isActive, 1, 0))) = 1
		and (iif(@is_deleted is null, 1, iif(@is_deleted = t.IsDeleted, 1, 0))) = 1
	)

	select
		t.Id,
		t.CreateDate,
		t.IsActive,
		t.IsDeleted,
		t.LevelId,
		t.ModificationDate,
		t.Number,
		t.UserName as [UserName]
	from tags as t
	order by t.CreateDate
	offset (iif(@page is null or @page_size is null, 0, @page * @page_size)) rows
	fetch next iif(@page_size is null,
			iif(exists(select * from tags), (select count(*) from tags), 1),
			iif(@page_size <= 0, 1, @page_size)
	) rows only
);
go

create function access_control.f_get_unknown_tags(@page int, @page_size int)
returns table
as
return
(
	with tags
	as
	(
		select
			x.Id,
			x.AccessDate,
			x.Number
		from access_control.UnknownTags as x
		where x.IsDeleted = 0
	)

	select
		x.Id,
		x.AccessDate,
		x.Number
	from tags as x
	order by x.AccessDate
	offset (iif(@page is null or @page_size is null, 0, @page * @page_size)) rows
	fetch next iif(@page_size is null, 
		iif(exists(select * from tags), (select count(*) from tags), 1),
		iif(@page_size <= 0, 1, @page_size)
	) rows only
);
go

create function access_control.f_get_users()
returns table
as
return
(
	select
		u.Id,
		u.[Name]
	from access_control.Users as u
);
go


create function access_control.f_check_access(@access_point_serial_number nvarchar(400), @tag_number nvarchar(100))
returns bit
begin
	declare @access_point_level_id int = (select top 1 ap.LevelId from access_control.AccessPoints as ap where ap.SerialNumber = @access_point_serial_number);
	declare @tag_level_id int = (select top 1 t.LevelId from access_control.Tags as t where t.Number = @tag_number);
	declare @has_access bit;
	if @access_point_level_id is not null and @tag_level_id is not null
	begin
		set @has_access = iif(@tag_level_id >= @access_point_level_id, 1, 0);
	end

	return @has_access;
end;
go

create function stat.f_convert_datetime2_hour_to_float(@date_time datetime2)
returns float
begin
	return cast(cast((cast(@date_time as datetime) - cast(cast(@date_time as date) as datetime)) as datetime) as float);
end;
go


create function stat.f_get_datetime2_time_as_seconds(@date_time datetime2)
returns int
begin
	return DATEPART(HOUR, @date_time) * 3600 + DATEPART(MINUTE, @date_time) * 60 + DATEPART(SECOND, @date_time);
end
go

create function stat.f_get_avg_entrance_time_for_user_in_minutes(@user_id int)
returns table
as
return
(
	with entrance_info
	as
	(
		select
			e.CreateDate,
			row_number() over(partition by cast(e.CreateDate as Date) order by e.CreateDate) as entrance_rnk
		from stat.[Events] as e
		where e.UserId = @user_id
		and e.AccessPointDirectionId = 1
		and e.TagIsActive = 1
		and e.AccessPointIsActive = 1
	)

	select
		AVG(stat.f_get_datetime2_time_as_seconds(x.CreateDate) / 60) as [avg_time]
	from entrance_info as x
	where x.entrance_rnk = 1 -- gets the first entrance per day
);
go

create function stat.f_get_avg_entrance_time_in_minutes()
returns table
as
return
(
	select
		avg(x.avg_time) as [avg_time]
	from access_control.Users as u
	cross apply stat.f_get_avg_entrance_time_for_user_in_minutes(u.Id) as x
);
go

create function stat.f_get_avg_exit_time_for_user_in_minutes(@user_id int)
returns table
as
return
(	
	with exit_info
	as
	(
		select
			e.CreateDate,
			row_number() over(partition by cast(e.CreateDate as Date) order by e.CreateDate desc) as exit_rnk
		from stat.[Events] as e
		where e.UserId = @user_id
		and e.AccessPointDirectionId = 2
		and e.TagIsActive = 1
		and e.AccessPointIsActive = 1
	)

	select
		AVG(stat.f_get_datetime2_time_as_seconds(x.CreateDate) / 60) as [avg_time]
	from exit_info as x
	where x.exit_rnk = 1 -- gets the last exit per day
);
go

create function stat.f_get_avg_exit_time_in_minutes()
returns table
as
return
(
	select
		avg(x.avg_time) as [avg_time]
	from access_control.Users as u
	cross apply stat.f_get_avg_exit_time_for_user_in_minutes(u.Id) as x
);
go

create function stat.f_get_work_hour_norm_in_minutes(@user_id int, @start_date datetime2, @end_date datetime2)
returns table
as
return
(
	with pair_info
	as
	(
		select
			e.CreateDate as entrance_date,
			lead(e.CreateDate, 1, null) over(order by e.CreateDate) as exit_date,
			e.AccessPointDirectionId
		from stat.[Events] as e
		where e.UserId = @user_id
		and e.TagIsActive = 1
		and e.AccessPointIsActive = 1
		and stat.f_date_in_range(e.CreateDate, @start_date, @end_date) = 1
	),
	hour_info
	as
	(
		select
			x.entrance_date,
			stat.f_get_datetime2_time_as_seconds(x.entrance_date) as entrance_time_in_seconds,
			x.exit_date,
			stat.f_get_datetime2_time_as_seconds(x.exit_date) as exit_time_in_seconds
		from pair_info as x
		where x.AccessPointDirectionId = 1
	),
	norm_pair_info
	as
	(
		select
			x.entrance_date,
			x.exit_date,
			x.entrance_time_in_seconds,
			x.exit_time_in_seconds,
			(x.exit_time_in_seconds - x.entrance_time_in_seconds) as norm_in_seconds
		from hour_info as x
	),
	norm_for_day
	as
	(
		select
			(sum(x.norm_in_seconds) - 8 * 3600) as norm,
			cast(x.entrance_date as date) as [day]
		from norm_pair_info as x
		group by cast(x.entrance_date as date)
	)

	select
		(x.norm / 60) as norm,
		x.[day]
	from norm_for_day as x
);
go


create function stat.f_get_avg_work_hour_norm_for_user_in_minutes(@user_id int)
returns table
as
return
(
	SELECT
		AVG(x.norm) AS avg_time
	FROM stat.f_get_work_hour_norm_in_minutes(@user_id, null, null) AS X
);
go

create function stat.f_get_avg_work_hour_norm_in_minutes()
returns table
as
return
(
	select
		avg(x.avg_time) as avg_time
	from access_control.Users as u
	cross apply stat.f_get_avg_work_hour_norm_for_user_in_minutes(u.Id) as x
);
go

create function stat.f_get_users()
returns table
as
return
(
	select
		u.Id,
		u.[Name] as [UserName],
		entrance.avg_time as AvgEntranceTimeInMinutes,
		[exit].avg_time as AvgExitTimeInMinutes,
		workHour.[avg_time] as AvgWorkHourNormInMinutes
	from access_control.Users as u
	cross apply stat.f_get_avg_entrance_time_for_user_in_minutes(u.Id) as entrance
	cross apply stat.f_get_avg_exit_time_for_user_in_minutes(u.Id) as [exit]
	cross apply stat.f_get_avg_work_hour_norm_for_user_in_minutes(u.Id) as [workHour]
);
go

create function stat.f_get_entrance_time_for_user(@user_id int, @start_date datetime2, @end_date datetime2)
returns table
as
return
(
	with group_info
	as
	(
		select
			e.CreateDate as entrance_time,
			row_number() over(partition by cast(e.CreateDate as date) order by e.CreateDate asc) as rnk
		from stat.[Events] as e
		where e.UserId = @user_id
		and e.TagIsActive = 1
		and e.AccessPointIsActive = 1
		and e.AccessPointDirectionId = 1 -- entrance
		and stat.f_date_in_range(e.CreateDate, @start_date, @end_date) = 1
	)

	select
		x.entrance_time
	from group_info as x
	where x.rnk = 1
);
go

create function stat.f_get_exit_time_for_user(@user_id int, @start_date datetime2, @end_date datetime2)
returns table
as
return
(
	with group_info
	as
	(
		select
			e.CreateDate as entrance_time,
			row_number() over(partition by cast(e.CreateDate as date) order by e.CreateDate desc) as rnk
		from stat.[Events] as e
		where e.UserId = @user_id
		and e.TagIsActive = 1
		and e.AccessPointIsActive = 1
		and e.AccessPointDirectionId = 2 -- exit
		and stat.f_date_in_range(e.CreateDate, @start_date, @end_date) = 1
	)

	select
		x.entrance_time
	from group_info as x
	where x.rnk = 1
);
go

-- procedures

if object_id('administration.usp_insert_user_if_not_exists', 'P') is not null
	drop procedure administration.usp_insert_user_if_not_exists;
go

if object_id('access_control.usp_insert_tag_if_not_exists', 'P') is not null
	drop procedure access_control.usp_insert_tag_if_not_exists;
go

if object_id('access_control.usp_insert_or_update_tag', 'P') is not null
	drop procedure access_control.usp_insert_or_update_tag;
go

if object_id('access_control.usp_insert_user_if_not_exists', 'P') is not null
	drop procedure access_control.usp_insert_user_if_not_exists;
go

if object_id('access_control.usp_insert_access_point_if_not_exists', 'P') is not null
	drop procedure access_control.usp_insert_access_point_if_not_exists;
go

if object_id('access_control.usp_insert_or_update_access_point', 'P') is not null
	drop procedure access_control.usp_insert_or_update_access_point;
go

if object_id('administration.usp_replace_refresh_token', 'P') is not null
	drop procedure administration.usp_replace_refresh_token
go

if object_id('access_control.usp_insert_or_update_unknown_access_point', 'P') is not null
	drop procedure access_control.usp_insert_or_update_unknown_access_point;
go

if object_id('access_control.usp_delete_unknown_access_point', 'P') is not null
	drop procedure access_control.usp_delete_unknown_access_point;
go

if object_id('access_control.usp_delete_access_point', 'P') is not null
	drop procedure access_control.usp_delete_access_point;
go

if object_id('access_control.usp_insert_or_update_unknown_tag', 'P') is not null
	drop procedure access_control.usp_insert_or_update_unknown_tag;
go

if object_id('access_control.usp_delete_unknown_tag', 'P') is not null
	drop procedure access_control.usp_delete_unknown_tag;
go

if object_id('stat.usp_insert_event', 'P') is not null
	drop procedure access_control.usp_insert_event;
go

if object_id('log.insert_client_log', 'P') is not null
	drop procedure [log].insert_client_log;
go

if object_id('administration.export', 'P') is not null
	drop procedure administration.export;
go

if type_id ('dbo.IntList') is not null
	drop type dbo.IntList;
go

create type dbo.IntList as table
(
	[Value] int
);
go


create procedure access_control.usp_insert_or_update_tag
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
			UserId = iif(@user_id is not null, @user_id, UserId),
			LevelId = iif(@level_id is not null, @level_id, LevelId)
		where Id = @identity
	end

	return @added;
end;
go

create procedure access_control.usp_insert_tag_if_not_exists
	@number nvarchar(100),
	@level_id int,
	@is_active bit,
	@is_deleted bit,
	@user_id int,
	@identity int output
as
begin
	set nocount on;

	declare @added bit = 0;

	set @identity = (select t.Id from access_control.Tags as t where t.Number = @number);

	if @identity is null or @identity = 0
		exec @added = access_control.usp_insert_or_update_tag @number, @level_id, @is_active, @is_deleted, @user_id, @identity output

	return @added;
end;
go

create procedure access_control.usp_insert_user_if_not_exists
	@username nvarchar(400),
	@identity int output
as
begin	
	set nocount on;

	set @identity = (select top 1 x.Id from access_control.Users as x where x.Name = @username);
	declare @added bit = iif(@identity is null or @identity = 0, 1, 0);

	if @added = 1
	begin
		insert into access_control.Users([Name]) values(@username);
		set @identity = SCOPE_IDENTITY();
	end

	return @added;
end;
go

create procedure access_control.usp_insert_or_update_access_point
	@serial_number nvarchar(400),
	@description nvarchar(max),
	@is_active bit,
	@is_deleted bit,
	@level_id int,
	@direction_id int,
	@identity int output
as
begin
	set nocount on;
	
	set @identity = (select top 1 x.Id from access_control.AccessPoints as x where x.SerialNumber = @serial_number);
	declare @added bit = iif(@identity is null or @identity = 0, 1, 0);

	if @added = 1
	begin
		insert into access_control.AccessPoints(SerialNumber, [Description], IsActive, LevelId, DirectionId, IsDeleted) 
		values(@serial_number, @description, @is_active, @level_id, @direction_id, @is_deleted);
		set @identity = SCOPE_IDENTITY();
	end
	else
	begin
		update access_control.AccessPoints
			set
				[Description] = iif(@description is not null and @description <> '', @description, [Description]),
				[IsActive] = iif(@is_active is not null, @is_active, IsActive),
				[LevelId] = iif(@level_id is not null, @level_id, LevelId),
				[DirectionId] = iif(@direction_id is not null, @direction_id, DirectionId),
				[IsDeleted] = iif(@is_deleted is not null, @is_deleted, IsDeleted)
		where SerialNumber = @serial_number
	end

	return @added;
end;
go

create procedure access_control.usp_insert_access_point_if_not_exists
	@serial_number nvarchar(400),
	@description nvarchar(max),
	@is_active bit,
	@level_id int,
	@direction_id int,
	@identity int output
as
begin
	set @identity = (select top 1 x.Id from access_control.AccessPoints as x where x.SerialNumber = @serial_number);
	declare @added bit;
	if @identity is null or @identity = 0
	begin
		exec @added = access_control.usp_insert_or_update_access_point @serial_number, @description, @is_active, 0, @level_id, @direction_id, @identity output
	end

	return @added;
end;
go

create procedure administration.usp_insert_user_if_not_exists
	@email nvarchar(400),
	@password_hash nvarchar(max),
	@roles dbo.IntList readonly,
	@identity int output
as
begin
	set nocount on;

	set @identity = (select u.Id from administration.Users as u where u.Email = @email)
	declare @added bit = iif(@identity is null or @identity = 0, 1, 0);

	if @added = 1
	begin
		insert into administration.Users(Email, PasswordHash) values (@email, @password_hash)
		set @identity = SCOPE_IDENTITY();

		insert into administration.UsersRoles(UserId, RoleId)
		select
			@identity,
			r.[Value]
		from @roles as r
	end

	return @added;
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

create procedure access_control.usp_insert_or_update_unknown_access_point
	@serial_number nvarchar(400),
	@identity int output
as
begin
	set @identity = (select x.Id from access_control.UnKnownAccessPoints as x where x.SerialNumber = @serial_number);
	declare @added bit = iif(@identity is not null and @identity <> 0, 0, 1);

	if @added = 0
	begin
		update access_control.UnKnownAccessPoints
		set AccessDate = GETDATE()
		where SerialNumber = @serial_number
	end
	else
	begin
		insert into access_control.UnKnownAccessPoints(SerialNumber, AccessDate, IsDeleted)
		values (@serial_number, GETDATE(), 0);
		set @identity = SCOPE_IDENTITY();
	end

	return @added;
end;
go

create procedure access_control.usp_delete_unknown_access_point
	@serial_number nvarchar(400)
as
begin
	update access_control.UnKnownAccessPoints 
	set IsDeleted = 1
	where SerialNumber = @serial_number
end;
go

create procedure access_control.usp_delete_access_point
	@serial_number nvarchar(400),
	@identity int output
as
begin
	
	set @identity = (select top 1 x.Id from access_control.AccessPoints as x where x.SerialNumber = @serial_number);
	declare @deleted bit = iif(@identity is not null and @identity <> 0, 1, 0);

	if @deleted = 1
	begin
		update access_control.AccessPoints
		set IsDeleted = 1
		where SerialNumber = @serial_number
	end
	return @deleted;
end;
go

create procedure access_control.usp_insert_or_update_unknown_tag
	@number nvarchar(100),
	@identity int output
as
begin
	set @identity = (select top 1 x.Id from access_control.UnknownTags as x where x.Number = @number);
	declare @added bit = iif(@identity is null or @identity = 0, 1, 0);
	if @added = 1
	begin
		insert into access_control.UnknownTags(AccessDate, IsDeleted, Number)
		values (GETDATE(), 0, @number);
		set @identity = SCOPE_IDENTITY();		
	end
	else
	begin
		update access_control.UnknownTags
		set AccessDate = GETDATE()
		where Number = @number
	end

	return @added;
end;
go

create procedure access_control.usp_delete_unknown_tag
	@number nvarchar(100)
as
begin
	update access_control.UnknownTags
	set IsDeleted = 1
	where Number = @number
end;
go

create procedure stat.usp_insert_event
	@access_point_serial_number nvarchar(400),
	@tag_number nvarchar(100)
as
begin
	insert into stat.[Events](
		AccessPointDirectionId, 
		AccessPointIsActive,
		AccessPointIsDeleted, 
		AccessPointIsUnknown,  
		AccessPointLevelId, 
		AccessPointSerialNumber, 
		TagIsActive,
		TagIsDeleted,
		TagIsUnknown,
		TagLevelId, 
		TagNumber, 
		UserId
	)
	select
		ap.DirectionId,
		ap.IsActive,
		ap.IsDeleted,
		cast(iif(ap.Id is null, 1, 0) as bit) as accessPointIsUnKnown,
		ap.LevelId,
		x.ap_serial_number,
		t.IsActive,
		t.IsDeleted,
		cast(iif(t.Id is null, 1, 0) as bit) as tagIsUnKnown,
		t.LevelId,
		x.t_number,
		t.UserId
	from (values (@access_point_serial_number, @tag_number)) as x(ap_serial_number, t_number)
	left join access_control.AccessPoints as ap on ap.SerialNumber = x.ap_serial_number
	left join access_control.Tags as t on t.Number = x.t_number
end;
go

create procedure [log].insert_client_log
	@message nvarchar(max),
	@type_id int
as
begin
	insert into [log].Client([Message], [TypeId]) values (@message, @type_id);
end;
go

create procedure administration.export
as
begin
	declare @tags_info nvarchar(max) = (
		select
			*
		from access_control.Tags as tag
		join access_control.Users as [user] on tag.UserId = [user].Id
		for json auto
	)

	declare @access_points_info nvarchar(max) = (
		select
			*
		from access_control.AccessPoints
		for json auto
	)

	declare @events_info nvarchar(max) = (
		select
			*
		from stat.Events
		for json auto
	)

	declare @levels_info nvarchar(max) = (
		select
			*
		from access_control.AccessLevels
		for json auto
	) 

	select
		@tags_info as tags,
		@access_points_info as access_points_info,
		@events_info as events_info,
		@levels_info
end;
go