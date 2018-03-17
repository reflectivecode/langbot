CREATE VIEW MemeMessageView AS
SELECT *,
       UpVoteCount = (SELECT COUNT(1) FROM Reaction WHERE Reaction.MessageId = MemeMessage.Id AND Reaction.Type = 'up-vote'),
       FlagCount   = (SELECT COUNT(1) FROM Reaction WHERE Reaction.MessageId = MemeMessage.Id AND Reaction.Type = 'flag')
    FROM MemeMessage