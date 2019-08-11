use Rfid;
go

if object_id('administration.usp_insert_or_update_tag', 'P') is not null
	drop procedure administration.usp_insert_or_update_tag;
go
create procedure administration.usp_insert_or_update_tag
	@number nvarchar(100),
	@level_id int,
	@is_active bit,
	@is_deleted bit,
	@user_id int,
	@tag_id int output
as
begin
	set nocount on;

	set @tag_id = (select top 1 x.Id from access_control.Tags as x where x.Number = @number)
	declare @added bit = iif(@tag_id is null or @tag_id = 0, 1, 0);

	if not exists(select * from access_control.Tags as x where x.Number = @number)
	begin
		-- modification date is updated with trigger after update of the row
		insert into access_control.Tags(IsActive, IsDeleted, LevelId, Number, UserId)
		values(@is_active, @is_deleted, @level_id, @number, @user_id)

		set @tag_id = SCOPE_IDENTITY();
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