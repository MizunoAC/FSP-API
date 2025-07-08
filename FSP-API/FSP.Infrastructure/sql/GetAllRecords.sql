--DECLARE @UserId INT = 2

WITH FilteredStatus AS (
    SELECT StatusId
    FROM [dbo].[UserRecordsStatus]
    WHERE [Description] = @RecordStatus
)
SELECT 
    AR.[RecordId],
    AR.[CommonNoun], 
    AE.[State] AS [AnimalState], 
    AR.[Description],
    AL.[Location],
    AP.[Image],
    RS.[Description] AS [Status]
FROM FilteredStatus FS
INNER JOIN [dbo].[UserRecordsStatus] RS ON FS.StatusId = RS.StatusId
INNER JOIN [dbo].[UserRecords] AR ON AR.RecordState = FS.StatusId
INNER JOIN [dbo].[UserRecordsLocation] AL ON AR.RecordId = AL.RecordId
INNER JOIN [dbo].[UserRecordsPicture] AP ON AP.RecordId = AL.RecordId
INNER JOIN [dbo].[RecordAnimalState] AE ON AE.Id = AR.AnimalState;
