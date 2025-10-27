--DECLARE @UserId INT = 2
--DECLARE @PageNumber INT = 1;
--DECLARE @PageSize INT = 10;
--DECLARE @RecordStatus VARCHAR(50) = 'Rejected'

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

IF OBJECT_ID('tempdb..#FilteredRecords') IS NOT NULL
    DROP TABLE #FilteredRecords;

SELECT
    UD.UserName,
    AR.RecordId,
    AR.CommonNoun,
    (SELECT State FROM RecordAnimalState AE WHERE AE.Id = AR.AnimalState) AS AnimalState,
    AR.Description,
    AL.Location,
    AP.ImageGuid,
    (SELECT Description FROM UserRecordsStatus RS WHERE RS.StatusId = AR.RecordState) AS Status,
    AR.CreatedDate,
    AR.AcceptedDate,
    ISNULL(AR.[RejectedReason], '') AS RejectedReason
INTO #FilteredRecords
FROM UserRecords AR
JOIN UserDomain UD ON UD.UserId = AR.UserId
JOIN UserRecordsLocation AL ON AL.RecordId = AR.RecordId
JOIN UserRecordsPicture AP ON AP.RecordId = AR.RecordId
WHERE AR.RecordState = (SELECT StatusId FROM UserRecordsStatus WHERE Description = @RecordStatus);


SELECT 
    FR.[UserName],
    FR.[RecordId],
    FR.[CommonNoun],
    FR.[AnimalState],
    FR.[Description],
    FR.[Location],
    FR.[ImageGuid],
    FR.[Status],
	FR.[CreatedDate],
    FR.[AcceptedDate],
    FR.[RejectedReason]
FROM #FilteredRecords FR
ORDER BY FR.[RecordId] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

DECLARE @Total INT = (SELECT COUNT(*) FROM #FilteredRecords);
DECLARE @TotalPages INT = CEILING(1.0 * @Total / @PageSize);

SELECT
    @PageNumber AS [page],
    @PageSize   AS [size],
    @Total      AS [total],
    @TotalPages AS [totalPages],
    CASE WHEN @PageNumber < @TotalPages THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasNext],
    CASE WHEN @PageNumber > 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasPrev];

DROP TABLE #FilteredRecords;
