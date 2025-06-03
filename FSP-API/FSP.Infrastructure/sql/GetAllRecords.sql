--DECLARE @UserId INT = 2

SELECT AR.[RecordId],
       AR.[CommonNoun], 
       AE.[State] AS [AnimalState], 
       AR.[Description],
       AL.[Location],
       AP.[Image],
       RS.[Description] AS [Status]
 FROM [dbo].[UserRecords] AR
 INNER JOIN [dbo].[UserRecordsLocation] AL ON AR.[RecordId] = AL.[RecordId]
 INNER JOIN [dbo].[UserRecordsPicture] AP ON AP.[RecordId] = AL.[RecordId]
 INNER JOIN [dbo].[UserRecordsStatus] RS ON AR.[RecordState] = RS.[StatusId] 
 INNER JOIN [dbo].[RecordAnimalState] AE ON AE.[Id] = AR.[AnimalState]
WHERE RS.[Description] = @RecordStatus