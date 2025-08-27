--DECLARE @PageNumber INT = 2;
--DECLARE @PageSize INT = 2;

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

IF OBJECT_ID('tempdb..#FilteredCatalog') IS NOT NULL
    DROP TABLE #FilteredCatalog;

SELECT 
       WC.[CatalogId],
       WC.[Specie],
       WC.[CommonNoun],
       WC.[Description],
       WC.[Habits],
       WC.[Habitat],
       WC.[Reproduction],
       WC.[Distribution],
       WC.[Feeding],
       WC.[Category],
       WCP.[Image],
       DM.[Map]
INTO #FilteredCatalog
FROM [dbo].[Wildlife_Catalog] WC
INNER JOIN [dbo].[Wildlife_Catalog_Picture] WCP ON WCP.CatalogId = WC.CatalogId
INNER JOIN [dbo].[DistributionMaps] DM ON DM.CatalogId = WC.CatalogId;

SELECT 
       FC.[CatalogId],
       FC.[Specie],
       FC.[CommonNoun],
       FC.[Description],
       FC.[Habits],
       FC.[Habitat],
       FC.[Reproduction],
       FC.[Distribution],
       FC.[Feeding],
       FC.[Category],
       FC.[Image],
       FC.[Map]
FROM #FilteredCatalog FC
ORDER BY FC.CatalogId DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

DECLARE @Total INT = (SELECT COUNT(*) FROM #FilteredCatalog);
DECLARE @TotalPages INT = CEILING(1.0 * @Total / @PageSize);

SELECT
    @PageNumber AS [page],
    @PageSize   AS [size],
    @Total      AS [total],
    @TotalPages AS [totalPages],
    CASE WHEN @PageNumber < @TotalPages THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasNext],
    CASE WHEN @PageNumber > 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [hasPrev];

DROP TABLE #FilteredCatalog;