use master;
go

--drop database Rfid;

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
