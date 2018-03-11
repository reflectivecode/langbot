CREATE TABLE MemeMessage
(
    Id           INTEGER PRIMARY KEY,
    Guid         TEXT    NOT NULL CHECK(LENGTH(Guid) = 36),
    CreateDate   TEXT    NOT NULL,
    UpdateDate   TEXT    NOT NULL,
    PublishDate  TEXT    NULL,
    DeleteDate   TEXT    NULL,
    TeamId       TEXT    NOT NULL,
    TeamDomain   TEXT    NOT NULL,
    ChannelId    TEXT    NOT NULL,
    ChannelName  TEXT    NOT NULL,
    ChannelType  TEXT    NOT NULL,
    UserId       TEXT    NOT NULL,
    UserName     TEXT    NOT NULL,
    TemplateId   TEXT    NOT NULL,
    Message      TEXT    NOT NULL,
    IsAnonymous  INTEGER NOT NULL,
    Timestamp    TEXT    NULL,
    DeleteReason TEXT    NULL
);

CREATE UNIQUE INDEX ix_MemeMessage_Guid ON MemeMessage(Guid);

CREATE TABLE Reaction
(
    Id          INTEGER PRIMARY KEY,
    MessageGuid TEXT    NOT NULL CHECK(LENGTH(Guid) = 36),
    Type        TEXT    NOT NULL,
    UserId      TEXT    NOT NULL,
    UserName    TEXT    NOT NULL,
    CreateDate  TEXT    NOT NULL,
    Message     TEXT    NULL,
    FOREIGN KEY (MessageGuid) REFERENCES MemeMessage(Guid)
);

CREATE        INDEX ix_Reaction_MessageGuid_Type        ON Reaction(MessageGuid, Type);
CREATE UNIQUE INDEX ix_Reaction_MessageGuid_Type_UserId ON Reaction(MessageGuid, Type, UserId);

GO

CREATE VIEW MemeMessageView AS
SELECT *,
       UpVoteCount = (SELECT COUNT(1) FROM Reaction WHERE MessageGuid = Guid AND Type = 'up-vote'),
       FlagCount   = (SELECT COUNT(1) FROM Reaction WHERE MessageGuid = Guid AND Type = 'flag')
    FROM MemeMessage

GO

CREATE TABLE ResponseUrl
(
    Id          INTEGER PRIMARY KEY,
    Guid        TEXT    NOT NULL CHECK(LENGTH(Guid) = 36),
    CreateDate  TEXT    NOT NULL,
    ResponseUrl TEXT    NOT NULL
);

CREATE UNIQUE INDEX ix_ResponseUrl_Guid ON ResponseUrl(Guid);
