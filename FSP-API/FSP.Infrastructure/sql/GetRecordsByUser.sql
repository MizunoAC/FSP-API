DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

IF OBJECT_ID('tempdb..#FilteredRecords') IS NOT NULL
    DROP TABLE #FilteredRecords;

SELECT
    AR.[RecordId],
    AR.[CommonNoun],
    AE.[State] AS [AnimalState],
    AR.[Description],
    AL.[Location],
    AP.[Image],
    RS.[Description] AS [Status]
INTO #FilteredRecords
FROM [dbo].[UserRecordsStatus] RS
INNER JOIN [dbo].[UserRecords] AR ON AR.RecordState = RS.StatusId
INNER JOIN [dbo].[UserRecordsLocation] AL ON AR.RecordId = AL.RecordId
INNER JOIN [dbo].[UserRecordsPicture] AP ON AP.RecordId = AL.RecordId
INNER JOIN [dbo].[RecordAnimalState] AE ON AE.Id = AR.AnimalState
WHERE AR.[UserId] = @UserId
  AND RS.[Description] = @RecordStatus;

SELECT
    FR.[RecordId],
    FR.[CommonNoun],
    FR.[AnimalState],
    FR.[Description],
    FR.[Location],
    FR.[Image],
    FR.[Status]
FROM #FilteredRecords FR
ORDER BY FR.[RecordId] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

DECLARE @Total INT = (SELECT COUNT(*) FROM #FilteredRecords);
DECLARE @TotalPages INT = CEILING(1.0 * @Total / @PageSize);

SELECT
    @PageNumber AS [page],
    @PageSize AS [size],
    @Total AS [total],
    @TotalPages AS [totalPages],
    CASE WHEN @PageNumber < @TotalPages THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasNext],
    CASE WHEN @PageNumber > 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasPrev];

DROP TABLE #FilteredRecords;
