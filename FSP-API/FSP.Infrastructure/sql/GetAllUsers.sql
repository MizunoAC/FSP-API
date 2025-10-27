--DECLARE @PageNumber INT = 1;
--DECLARE @PageSize INT = 10;

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

IF OBJECT_ID('tempdb..#FilteredUsers') IS NOT NULL
    DROP TABLE #FilteredRecords;

SELECT ud.[UserID],
       ud.[UserName],
       ui.[Name],
       ui.[LastName], 
	   ui.[Locality], 
	   g.[Gender],
	   ui.[Age],
	   ui.[Email]
       INTO #FilteredUsers
FROM [dbo].[UserInformation] ui
INNER JOIN [dbo].[UserDomain] ud ON ud.[UserID] = ui.[UserID]
INNER JOIN [dbo].[Genders] g ON g.[Id] = ui.[Gender]
WHERE ui.[UserType] = 1


SELECT [UserID],
       [UserName],
       [Name],
       [LastName], 
	   [Locality],
	   [Gender],
	   [Age],
	   [Email]
      FROM #FilteredUsers FR
ORDER BY [Locality] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

DECLARE @Total INT = (SELECT COUNT(*) FROM #FilteredUsers);
DECLARE @TotalPages INT = CEILING(1.0 * @Total / @PageSize);

SELECT
    @PageNumber AS [page],
    @PageSize   AS [size],
    @Total      AS [total],
    @TotalPages AS [totalPages],
    CASE WHEN @PageNumber < @TotalPages THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasNext],
    CASE WHEN @PageNumber > 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasPrev];

DROP TABLE #FilteredUsers;
