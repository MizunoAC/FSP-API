SELECT [Location] 
FROM [dbo].[UserRecordsLocation] URL
INNER JOIN [dbo].[UserRecords] UR ON UR.RecordId = URL.RecordId
INNER JOIN [dbo].[Wildlife_Catalog] WC ON WC.CatalogId = UR.CatalogId 
WHERE WC.CatalogId = @CatalogId
