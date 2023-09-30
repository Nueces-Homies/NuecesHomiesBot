CREATE TABLE Movies (
    Id int NOT NULL PRIMARY KEY,
    TmdbId int NOT NULL,
    ChannelId int NOT NULL,
    Name varchar(256) NOT NULL,
    ReleaseTime int NOT NULL 
)