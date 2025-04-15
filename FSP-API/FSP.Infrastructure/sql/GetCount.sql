SELECT 
    COUNT(DISTINCT ud.[UserId]) AS [Users],
    COUNT(ur.[RecordId]) AS [Records]
FROM 
    [dbo].[UserDomain] ud
INNER JOIN 
    [dbo].[UserRecords] ur ON ud.[UserId] = ur.[UserId]