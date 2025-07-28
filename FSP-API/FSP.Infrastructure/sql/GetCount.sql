SELECT 
    COUNT(DISTINCT ud.[UserId]) AS [Users],
    COUNT(ur.[RecordId]) AS [Records]
FROM 
    [dbo].[UserDomain] ud
INNER JOIN 
    [dbo].[UserInformation] ui ON ui.UserID = ud.UserID AND ui.[Delete] = 0
LEFT JOIN 
    [dbo].[UserRecords] ur ON ud.[UserId] = ur.[UserId] and ur.RecordState NOT IN (3)
	