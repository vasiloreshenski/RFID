use Rfid;
go

if object_Id('access_control.f_get_access_level_for_access_point', 'FN') is not null
	drop function access_control.f_get_access_level_for_access_point;
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


if object_id('access_control.f_get_access_level_for_tag', 'FN') is not null
	drop function access_control.f_get_access_level_for_tag;
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