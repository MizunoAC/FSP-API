--DECLARE @PageNumber INT = 2;
--DECLARE @PageSize INT = 2;

DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

SELECT wc.[CatalogId],
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
  FROM [dbo].[Wildlife_Catalog] WC
  INNER JOIN [dbo].[Wildlife_Catalog_Picture] WCP ON WCP.CatalogId = WC.CatalogId
  INNER JOIN [dbo].[DistributionMaps] DM ON DM.CatalogId = WC.CatalogId
  ORDER BY WC.CatalogId DESC
  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;