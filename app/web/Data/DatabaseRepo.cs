using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

namespace LangBot.Web
{
    public class DatabaseRepo
    {
        private const string MESSAGE_VIEW = "MemeMessageView";
        private const string MESSAGE_TABLE = "MemeMessage";
        private const string REACTION_TABLE = "Reaction";

        private static string Format(Guid guid) => guid.ToString("D");
        private static string Format(DateTime date) => date.ToString("u");

        private readonly IDbConnection _connection;

        public DatabaseRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Test()
        {
            await _connection.QueryAsync($"SELECT COUNT(1) FROM {EPHEMERAL_MESSAGE_TABLE};");
            await _connection.QueryAsync($"SELECT COUNT(1) FROM {IN_CHANNEL_MESSAGE_TABLE};");
            await _connection.QueryAsync($"SELECT COUNT(1) FROM {REACTION_TABLE};");
        }

        public async Task<MemeMessage> InsertMessage(
            string teamId,
            string teamDomain,
            string channelId,
            string channelName,
            string channelType,
            string userId,
            string userName,
            string templateId,
            string message,
            string imageUrl,
            bool isAnonymous)
        {
            if (teamId == null) throw new ArgumentNullException(nameof(teamId));
            if (teamDomain == null) throw new ArgumentNullException(nameof(teamDomain));
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
            if (channelType == null) throw new ArgumentNullException(nameof(channelType));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (templateId == null) throw new ArgumentNullException(nameof(templateId));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (imageUrl == null) throw new ArgumentNullException(nameof(imageUrl));

            var parameters = new
            {
                guid = Format(Guid.NewGuid()),
                now = Format(DateTime.UtcNow),
                teamId,
                teamDomain,
                channelId,
                channelName,
                channelType,
                userId,
                userName,
                templateId,
                message,
                imageUrl,
                isAnonymous,
            };

            using (var transaction = _connection.BeginTransaction())
            {
                return await _connection.QuerySingleAsync<MemeMessage>($@"
                    INSERT {MESSAGE_TABLE} (
                        {nameof(MemeMessage.Guid)},
                        {nameof(MemeMessage.CreateDate)},
                        {nameof(MemeMessage.UpdateDate)},
                        {nameof(MemeMessage.TeamId)},
                        {nameof(MemeMessage.TeamDomain)},
                        {nameof(MemeMessage.ChannelId)},
                        {nameof(MemeMessage.ChannelName)},
                        {nameof(MemeMessage.ChannelType)},
                        {nameof(MemeMessage.UserId)},
                        {nameof(MemeMessage.UserName)},
                        {nameof(MemeMessage.TemplateId)},
                        {nameof(MemeMessage.Message)},
                        {nameof(MemeMessage.ImageUrl)},
                        {nameof(MemeMessage.IsAnonymous)}
                    ) VALUES (
                        @{nameof(parameters.guid)},
                        @{nameof(parameters.now)},
                        @{nameof(parameters.now)},
                        @{nameof(parameters.teamId)},
                        @{nameof(parameters.teamDomain)},
                        @{nameof(parameters.channelId)},
                        @{nameof(parameters.channelName)},
                        @{nameof(parameters.channelType)},
                        @{nameof(parameters.userId)},
                        @{nameof(parameters.userName)},
                        @{nameof(parameters.templateId)},
                        @{nameof(parameters.message)},
                        @{nameof(parameters.imageUrl)},
                        @{nameof(parameters.isAnonymous)}
                    );
                    SELECT *
                        FROM {MESSAGE_VIEW}
                        WHERE {nameof(MemeMessage.Id)} = last_insert_rowid();", parameters, transaction);
            }
        }

        public async Task<MemeMessage> SelectMessage(Guid guid)
        {
            var parameters = new
            {
                guid = Format(guid),
            };

            return await _connection.QuerySingleOrDefaultAsync<MemeMessage>($@"
                SELECT *
                    FROM {MESSAGE_VIEW}
                    WHERE {nameof(MemeMessage.Guid)} = @{nameof(parameters.guid)}", parameters);
        }

        public async Task<MemeMessage> UpdatePreview(Guid guid, string templateId = null, string message = null, string imageUrl = null, bool? isAnonymous = null)
        {
            var parameters = new
            {
                guid = Format(guid),
                templateId,
                message,
                imageUrl,
                isAnonymous,
            };

            using (var transaction = _connection.BeginTransaction())
            {
                return await _connection.QuerySingleOrDefaultAsync<MemeMessage>($@"
                    UPDATE {MESSAGE_TABLE}
                        SET {nameof(MemeMessage.TemplateId)}  = IFNULL(@{nameof(parameters.templateId)},  {nameof(MemeMessage.TemplateId)})
                           ,{nameof(MemeMessage.Message)}     = IFNULL(@{nameof(parameters.message)},     {nameof(MemeMessage.Message)})
                           ,{nameof(MemeMessage.ImageUrl)}    = IFNULL(@{nameof(parameters.imageUrl)},    {nameof(MemeMessage.ImageUrl)})
                           ,{nameof(MemeMessage.IsAnonymous)} = IFNULL(@{nameof(parameters.isAnonymous)}, {nameof(MemeMessage.IsAnonymous)})
                        WHERE {nameof(MemeMessage.Guid)}        = @{nameof(parameters.guid)}
                          AND {nameof(MemeMessage.PublishDate)} = NULL;
                    SELECT *
                        FROM {MESSAGE_VIEW}
                        WHERE {nameof(MemeMessage.Guid)} = @{nameof(parameters.guid)};", parameters, transaction);
            }
        }

        public async Task DeletePreview(Guid guid)
        {
            var parameters = new
            {
                guid = Format(guid),
            };

            await _connection.ExecuteAsync($@"
                DELETE
                    FROM {MESSAGE_TABLE}
                    WHERE {nameof(MemeMessage.Guid)}        = @{nameof(parameters.guid)}
                      AND {nameof(MemeMessage.PublishDate)} = NULL", parameters);
        }

        public async Task<MemeMessage> PublishMessage(Guid guid)
        {
            var parameters = new
            {
                guid = Format(guid),
                now = Format(DateTime.Now),
            };

            using (var transaction = _connection.BeginTransaction())
            {
                return await _connection.QuerySingleOrDefaultAsync<MemeMessage>($@"
                    UPDATE {MESSAGE_TABLE}
                        SET {nameof(MemeMessage.PublishDate)} = @{nameof(parameters.now)}
                        WHERE {nameof(MemeMessage.Guid)}        = @{nameof(parameters.guid)}
                          AND {nameof(MemeMessage.PublishDate)} = NULL;
                    SELECT *
                        FROM {MESSAGE_VIEW}
                        WHERE {nameof(MemeMessage.Guid)}        = @{nameof(parameters.guid)}
                          AND {nameof(MemeMessage.PublishDate)} = @{nameof(parameters.now)}", parameters, transaction);
            }
        }

        public async Task<bool> HasReacted(Guid guid, string type, string userId)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var parameters = new
            {
                guid = Format(guid),
                type,
                userId,
            };

            return await _connection.QuerySingleAsync<bool>($@"
                SELECT COUNT(1)
                    FROM {REACTION_TABLE}
                    WHERE {nameof(Reaction.Type)}        = @{nameof(parameters.type)}
                      AND {nameof(Reaction.UserId)}      = @{nameof(parameters.userId)}
                      AND {nameof(Reaction.MessageGuid)} = @{nameof(parameters.guid)};", parameters);
        }

        public async Task<MemeMessage> AddReaction(Guid guid, string type, string userId, string userName, string message)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            var parameters = new
            {
                guid = Format(guid),
                now = Format(DateTime.UtcNow),
                type,
                userId,
                userName,
                message,
            };

            using (var transaction = _connection.BeginTransaction())
            {
                return await _connection.QuerySingleAsync<MemeMessage>($@"
                    INSERT {REACTION_TABLE} (
                        {nameof(Reaction.MessageGuid)},
                        {nameof(Reaction.Type)},
                        {nameof(Reaction.UserId)},
                        {nameof(Reaction.UserName)},
                        {nameof(Reaction.CreateDate)},
                        {nameof(Reaction.Message)}
                    ) VALUES (
                        @{nameof(parameters.guid)},
                        @{nameof(parameters.type)},
                        @{nameof(parameters.userId)},
                        @{nameof(parameters.userName)},
                        @{nameof(parameters.now)},
                        @{nameof(parameters.message)}
                    );
                    SELECT *
                        FROM {MESSAGE_VIEW}
                        WHERE {nameof(MemeMessage.Guid)} = @{nameof(parameters.guid)};", parameters, transaction);
            }
        }

        public async Task<MemeMessage> RemoveReaction(Guid guid, string type, string userId)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var parameters = new
            {
                guid = Format(guid),
                now = Format(DateTime.UtcNow),
                type,
                userId,
            };

            using (var transaction = _connection.BeginTransaction())
            {
                return await _connection.QuerySingleAsync<MemeMessage>($@"
                    DELETE
                        FROM {REACTION_TABLE}
                        WHERE {nameof(Reaction.MessageGuid)} = @{nameof(parameters.guid)}
                          AND {nameof(Reaction.Type)}        = @{nameof(parameters.type)}
                          AND {nameof(Reaction.UserId)}      = @{nameof(parameters.userId)};
                    SELECT *
                        FROM {MESSAGE_VIEW}
                        WHERE {nameof(MemeMessage.Guid)} = @{nameof(parameters.guid)};", parameters, transaction);
            }
        }
    }
}
