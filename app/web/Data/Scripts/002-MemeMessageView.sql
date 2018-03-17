CREATE VIEW MemeMessageView AS
SELECT *,
       (SELECT COUNT(1) FROM Reaction WHERE Reaction.MessageId = MemeMessage.Id AND Reaction.Type = 'up-vote') AS UpVoteCount,
       (SELECT COUNT(1) FROM Reaction WHERE Reaction.MessageId = MemeMessage.Id AND Reaction.Type = 'flag') AS FlagCount
    FROM MemeMessage