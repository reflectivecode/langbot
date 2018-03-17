CREATE TABLE MemeMessage
(
    Id           INTEGER PRIMARY KEY,
    Guid         TEXT    NOT NULL CHECK(LENGTH(Guid) = 36),
    CreateDate   TEXT    NOT NULL,
    UpdateDate   TEXT    NOT NULL,
    PublishDate  TEXT    NULL,
    DeleteDate   TEXT    NULL,
    DeleteReason TEXT    NULL,
    TeamId       TEXT    NOT NULL,
    TeamDomain   TEXT    NOT NULL,
    ChannelId    TEXT    NOT NULL,
    ChannelName  TEXT    NOT NULL,
    ChannelType  TEXT    NOT NULL,
    UserId       TEXT    NOT NULL,
    UserName     TEXT    NOT NULL,
    TemplateId   TEXT    NOT NULL,
    Message      TEXT    NOT NULL,
    ImageUrl     TEXT    NOT NULL,
    IsAnonymous  INTEGER NOT NULL
);

CREATE UNIQUE INDEX ix_MemeMessage_Guid ON MemeMessage(Guid);

CREATE TABLE Reaction
(
    Id          INTEGER PRIMARY KEY,
    MessageId   INTEGER NOT NULL,
    Type        TEXT    NOT NULL,
    UserId      TEXT    NOT NULL,
    UserName    TEXT    NOT NULL,
    CreateDate  TEXT    NOT NULL,
    Message     TEXT    NULL,
    FOREIGN KEY (MessageId) REFERENCES MemeMessage(Id)
);

CREATE        INDEX ix_Reaction_MessageId_Type        ON Reaction(MessageId, Type);
CREATE UNIQUE INDEX ix_Reaction_MessageId_Type_UserId ON Reaction(MessageId, Type, UserId);

CREATE TABLE PendingResponse
(
    Id          INTEGER PRIMARY KEY,
    MessageId   INTEGER NOT NULL,
    Guid        TEXT    NOT NULL CHECK(LENGTH(Guid) = 36),
    CreateDate  TEXT    NOT NULL,
    ResponseUrl TEXT    NOT NULL,
    TeamId      TEXT    NOT NULL,
    TeamDomain  TEXT    NOT NULL,
    ChannelId   TEXT    NOT NULL,
    ChannelName TEXT    NOT NULL,
    UserId      TEXT    NOT NULL,
    UserName    TEXT    NOT NULL,
    FOREIGN KEY (MessageId) REFERENCES MemeMessage(Id)
);

CREATE UNIQUE INDEX ix_PendingResponse_Guid ON PendingResponse(Guid);
