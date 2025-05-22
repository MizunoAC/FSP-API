SELECT CONCAT([Name],' ', [LastName]) AS FullName,
       [Email] 
   FROM [dbo].[UserRecords] UR
INNER JOIN [dbo].[UserInformation] UI ON UR.UserId = UI.UserID 
WHERE [RecordId] = @RecordId