--DECLARE @UserId INT = 1001
--DECLARE @PageNumber INT = 1;
--DECLARE @PageSize INT = 2;
--DECLARE @RecordStatus VARCHAR(500)= 'Accepted'

  
DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

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
INNER JOIN [dbo].[RecordAnimalState] AE ON AE.Id = AR.AnimalState
WHERE AR.[UserId] = @UserId
  AND RS.[Description] = @RecordStatus
  ORDER BY AR.[RecordId] DESC
  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;