use Rfid;
go

if object_id('administration.usp_insert_tag_if_not_exists', 'P') is not null
	drop procedure administration.usp_insert_tag_if_not_exists;
go
create procedure administration.usp_insert_tag_if_not_exists
	@number nvarchar(100),
	@level_id int,
	@is_active bit,
	@is_deleted bit,
	@user_id int
as
begin
	set nocount on;

	declare @added bit = 0;

	if not exists(select * from access_control.Tags as x where x.Number = @number)
	begin
		-- modification date is updated with trigger after update of the row
		insert into access_control.Tags(IsActive, IsDeleted, LevelId, Number, UserId)
		values(@is_active, @is_deleted, @level_id, @number, @user_id)

		set @added = 1;
	end

	return @added;
end;
go

if object_id('administration.usp_insert_user_if_not_exists', 'P') is not null
	drop procedure administration.usp_insert_user_if_not_exists;
go
create procedure administration.usp_insert_user_if_not_exists
	@username nvarchar(400),
	@user_id int output
as
begin	
	set nocount on;

	set @user_id = (select top 1 x.Name from access_control.Users as x where x.Name = @username);
	declare @added bit = iif(@user_id is not null and @user_id <> 0, 1, 0);

	if @user_id is null or @user_id = 0
	begin
		insert into access_control.Users([Name]) values(@username);
		set @user_id = SCOPE_IDENTITY();
	end

	return @added;
end;
go