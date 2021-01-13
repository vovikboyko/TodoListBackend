CREATE PROC dbo.Answer_Delete
	(
	@AnswerId int
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM dbo.Answer
	WHERE AnswerID = @AnswerId
END
GO

CREATE PROC dbo.Answer_Get_ByTaskId
	(
	@TaskId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT AnswerId, TaskId, Content, Username, Created
	FROM dbo.Answer 
	WHERE TaskId = @TaskId
END
GO

CREATE PROC dbo.Answer_Post
	(
	@TaskId int,
	@Content nvarchar(max),
	@UserId nvarchar(150),
	@UserName nvarchar(150),
	@Created datetime2
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.Answer
		(TaskId, Content, UserId, UserName, Created)
	SELECT @TaskId, @Content, @UserId, @UserName, @Created

	SELECT AnswerId, Content, UserName, UserId, Created
	FROM dbo.Answer
	WHERE AnswerId = SCOPE_IDENTITY()
END
GO

CREATE PROC dbo.Answer_Put
	(
	@AnswerId int,
	@Content nvarchar(max)
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE dbo.Answer
	SET Content = @Content
	WHERE AnswerId = @AnswerId

	SELECT a.AnswerId, a.TaskId, a.Content, u.UserName, a.Created
	FROM dbo.Answer a
		LEFT JOIN AspNetUsers u ON a.UserId = u.Id
	WHERE AnswerId = @AnswerId
END
GO


CREATE PROC dbo.Task_AddForLoadTest
AS
BEGIN
	DECLARE @i int = 1

	WHILE @i < 10000
	BEGIN
		INSERT INTO dbo.Task
			(Title, Content, UserId, UserName, Created)
		VALUES('Task ' + CAST(@i AS nvarchar(5)), 'Content ' + CAST(@i AS nvarchar(5)), 'User1', 'User1', GETUTCDATE())
		SET @i = @i + 1
	END
END
GO

CREATE PROC dbo.Task_Delete
	(
	@TaskId int
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE
	FROM dbo.Task
	WHERE TaskID = @TaskId
END
GO

CREATE PROC dbo.Task_Exists
	(
	@TaskId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT CASE WHEN EXISTS (SELECT TaskId
		FROM dbo.Task
		WHERE TaskId = @TaskId) 
        THEN CAST (1 AS BIT) 
        ELSE CAST (0 AS BIT) END AS Result

END
GO

CREATE PROC dbo.Task_GetMany
AS
BEGIN
	SET NOCOUNT ON

	SELECT TaskId, Title, Content, UserId, UserName, Created
	FROM dbo.Task 
END
GO

CREATE PROC dbo.Task_GetMany_BySearch
	(
	@Search nvarchar(100)
)
AS
BEGIN
	SET NOCOUNT ON

		SELECT TaskId, Title, Content, UserId, UserName, Created
		FROM dbo.Task 
		WHERE Title LIKE '%' + @Search + '%'

	UNION

		SELECT TaskId, Title, Content, UserId, UserName, Created
		FROM dbo.Task 
		WHERE Content LIKE '%' + @Search + '%'
END
GO

CREATE PROC dbo.Task_GetMany_BySearch_WithPaging
	(
	@Search nvarchar(100),
	@PageNumber int,
	@PageSize int
)
AS
BEGIN
	SELECT TaskId, Title, Content, UserId, UserName, Created
	FROM
		(	SELECT TaskId, Title, Content, UserId, UserName, Created
			FROM dbo.Task 
			WHERE Title LIKE '%' + @Search + '%'

		UNION

			SELECT TaskId, Title, Content, UserId, UserName, Created
			FROM dbo.Task 
			WHERE Content LIKE '%' + @Search + '%') Sub
	ORDER BY TaskId
	OFFSET @PageSize * (@PageNumber - 1) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

CREATE PROC dbo.Task_GetMany_WithAnswers
AS
BEGIN
	SET NOCOUNT ON

	SELECT q.TaskId, q.Title, q.Content, q.UserName, q.Created,
		a.TaskId, a.AnswerId, a.Content, a.Username, a.Created
	FROM dbo.Task q
		LEFT JOIN dbo.Answer a ON q.TaskId = a.TaskId
END
GO

CREATE PROC dbo.Task_GetSingle
	(
	@TaskId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT TaskId, Title, Content, UserId, Username, Created
	FROM dbo.Task 
	WHERE TaskId = @TaskId
END
GO

CREATE PROC dbo.Task_GetUnanswered
AS
BEGIN
	SET NOCOUNT ON

	SELECT TaskId, Title, Content, UserId, UserName, Created
	FROM dbo.Task q
	WHERE NOT EXISTS (SELECT *
	FROM dbo.Answer a
	WHERE a.TaskId = q.TaskId)
END
GO

CREATE PROC dbo.Task_Post
	(
	@Title nvarchar(100),
	@Created datetime2
)
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO dbo.Task
		(Title, Created)
	VALUES(@Title, @Created)

	SELECT SCOPE_IDENTITY() AS TaskId
END
GO

CREATE PROC dbo.Task_Put
	(
	@TaskId int,
	@Title nvarchar(100),
	@Content nvarchar(max)
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE dbo.Task
	SET Title = @Title, Content = @Content
	WHERE TaskID = @TaskId
END
GO

CREATE PROC dbo.Answer_Get_ByAnswerId
	(
	@AnswerId int
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT AnswerId, Content, Username, Created
	FROM dbo.Answer 
	WHERE AnswerId = @AnswerId
END
GO