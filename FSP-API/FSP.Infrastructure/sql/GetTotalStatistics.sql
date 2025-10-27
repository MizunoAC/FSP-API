SELECT
    SUM(CASE WHEN S.Description = 'Pending' THEN 1 ELSE 0 END) AS Pending,
    SUM(CASE WHEN S.Description = 'Accepted' THEN 1 ELSE 0 END) AS Accepted,
    SUM(CASE WHEN S.Description = 'Rejected' THEN 1 ELSE 0 END) AS Rejected,
    COUNT(*) AS TotalRecords
FROM UserRecords U
INNER JOIN UserRecordsStatus S ON U.RecordState = S.StatusId;
