SELECT u.Users, r.Records
FROM (
    SELECT COUNT(*) AS Users
    FROM [dbo].[UserInformation]
    WHERE [Blocked] = 0
) u
CROSS JOIN (
    SELECT COUNT(ur.RecordId) AS Records
    FROM [dbo].[UserDomain] ud
    LEFT JOIN [dbo].[UserRecords] ur 
        ON ud.UserId = ur.UserId
        AND ur.RecordState NOT IN (3)
) r;
