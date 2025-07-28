SELECT COUNT(1)
        FROM [dbo].[RefreshTokens]
        WHERE [UserId] = @UserID
          AND [Token] = @Token
          AND [ExpiresAt] > GETDATE()
          AND [IsRevoked] = 0