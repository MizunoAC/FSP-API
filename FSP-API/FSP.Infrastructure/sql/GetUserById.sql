--DECLARE @UserId INT = 2

SELECT ud.[UserName],
       ui.[Name],
       ui.[LastName], 
	   ui.[Locality], 
	   g.[Gender],
	   ui.[Age],
	   ui.[Email]
FROM [dbo].[UserInformation] ui
INNER JOIN [dbo].[UserDomain] ud ON ud.[UserID] = ui.[UserID]
INNER JOIN [dbo].[Genders] g ON g.[Id] = ui.[Gender]
WHERE ui.[UserID] = @UserId