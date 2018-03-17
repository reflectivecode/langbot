using Dapper;
using EnumsNET;
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
        private const string RESPONSE_TABLE = "Response";

        private static string Format(Guid guid) => guid.ToString("D");
        private static string Format(DateTime date) => date.ToString("u");
        private static string Format(ChannelType type) => type.ToString();

        private readonly IDbConnection _connection;

        public DatabaseRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task Test()
        {
            await _connection.QueryAsync($"SELECT COUNT(1) FROM {MESSAGE_VIEW};");
            await _connection.QueryAsync($"SELECT COUNT(1) FROM {MESSAGE_TABLE};");
            await _connection.QueryAsync($"SELECT COUNT(1) FROM {REACTION_TABLE};");
        }

        public async Task<MemeMessage> InsertMessage(
            string teamId,
            string teamDomain,
            string channelId,
            string channelName,
            ChannelType channelType,
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
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (templateId == null) throw new ArgumentNullException(nameof(templateId));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (imageUrl == null) throw new ArgumentNullException(nameof(imageUrl));

            channelType.Validate(nameof(channelType));

            var parameters = new
            {
                guid = Format(Guid.NewGuid()),
                now = Format(DateTime.UtcNow),
                teamId,
                teamDomain,
                channelId,
                channelName,
                channelType = Format(channelType),
                userId,
                userName,
                templateId,
                message,
                imageUrl,
                isAnonymous,
            };

            return await _connection.QuerySingleAsync<MemeMessage>($@"
                INSERT INTO {MESSAGE_TABLE} (
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
                    WHERE {nameof(MemeMessage.Id)} = last_insert_rowid();", parameters);
        }

        public async Task<MemeMessage> SelectMessage(int id)
        {
            var parameters = new
            {
                id,
            };

            return await _connection.QuerySingleOrDefaultAsync<MemeMessage>($@"
                SELECT *
                    FROM {MESSAGE_VIEW}
                    WHERE {nameof(MemeMessage.Id)} = @{nameof(parameters.id)}", parameters);
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

        public async Task<MemeMessage> UpdatePreview(int id, string templateId = null, string message = null, string imageUrl = null, bool? isAnonymous = null)
        {
            var parameters = new
            {
                id,
                templateId,
                message,
                imageUrl,
                isAnonymous,
            };

            return await _connection.QuerySingleOrDefaultAsync<MemeMessage>($@"
                UPDATE {MESSAGE_TABLE}
                    SET {nameof(MemeMessage.TemplateId)}  = IFNULL(@{nameof(parameters.templateId)},  {nameof(MemeMessage.TemplateId)})
                       ,{nameof(MemeMessage.Message)}     = IFNULL(@{nameof(parameters.message)},     {nameof(MemeMessage.Message)})
                       ,{nameof(MemeMessage.ImageUrl)}    = IFNULL(@{nameof(parameters.imageUrl)},    {nameof(MemeMessage.ImageUrl)})
                       ,{nameof(MemeMessage.IsAnonymous)} = IFNULL(@{nameof(parameters.isAnonymous)}, {nameof(MemeMessage.IsAnonymous)})
                    WHERE {nameof(MemeMessage.Id)}          = @{nameof(parameters.id)}
                      AND {nameof(MemeMessage.PublishDate)} IS NULL;
                SELECT *
                    FROM {MESSAGE_VIEW}
                    WHERE {nameof(MemeMessage.Id)} = @{nameof(parameters.id)};", parameters);
        }

        public async Task DeletePreview(int id)
        {
            var parameters = new
            {
                id,
            };

            await _connection.ExecuteAsync($@"
                DELETE
                    FROM {MESSAGE_TABLE}
                    WHERE {nameof(MemeMessage.Id)}          = @{nameof(parameters.id)}
                      AND {nameof(MemeMessage.PublishDate)} IS NULL", parameters);
        }

        public async Task<MemeMessage> PublishMessage(int id)
        {
            var parameters = new
            {
                id,
                now = Format(DateTime.Now),
            };

            return await _connection.QuerySingleOrDefaultAsync<MemeMessage>($@"
                UPDATE {MESSAGE_TABLE}
                    SET {nameof(MemeMessage.PublishDate)} = @{nameof(parameters.now)}
                    WHERE {nameof(MemeMessage.Id)}        = @{nameof(parameters.id)}
                      AND {nameof(MemeMessage.PublishDate)} IS NULL;
                SELECT *
                    FROM {MESSAGE_VIEW}
                    WHERE {nameof(MemeMessage.Id)}          = @{nameof(parameters.id)}
                      AND {nameof(MemeMessage.PublishDate)} = @{nameof(parameters.now)}", parameters);
        }

        public async Task<bool> HasReacted(int messageId, string type, string userId)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var parameters = new
            {
                messageId,
                type,
                userId,
            };

            return await _connection.QuerySingleAsync<bool>($@"
                SELECT COUNT(1)
                    FROM {REACTION_TABLE}
                    WHERE {nameof(Reaction.Type)}      = @{nameof(parameters.type)}
                      AND {nameof(Reaction.UserId)}    = @{nameof(parameters.userId)}
                      AND {nameof(Reaction.MessageId)} = @{nameof(parameters.messageId)};", parameters);
        }

        public async Task<MemeMessage> AddReaction(int messageId, string type, string userId, string userName, string message)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            var parameters = new
            {
                messageId,
                now = Format(DateTime.UtcNow),
                type,
                userId,
                userName,
                message,
            };

            return await _connection.QuerySingleAsync<MemeMessage>($@"
                INSERT INTO {REACTION_TABLE} (
                    {nameof(Reaction.MessageId)},
                    {nameof(Reaction.Type)},
                    {nameof(Reaction.UserId)},
                    {nameof(Reaction.UserName)},
                    {nameof(Reaction.CreateDate)},
                    {nameof(Reaction.Message)}
                ) VALUES (
                    @{nameof(parameters.messageId)},
                    @{nameof(parameters.type)},
                    @{nameof(parameters.userId)},
                    @{nameof(parameters.userName)},
                    @{nameof(parameters.now)},
                    @{nameof(parameters.message)}
                );
                SELECT *
                    FROM {MESSAGE_VIEW}
                    WHERE {nameof(MemeMessage.Id)} = @{nameof(parameters.messageId)};", parameters);
        }

        public async Task<MemeMessage> RemoveReaction(int messageId, string type, string userId)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var parameters = new
            {
                messageId,
                now = Format(DateTime.UtcNow),
                type,
                userId,
            };

            return await _connection.QuerySingleAsync<MemeMessage>($@"
                DELETE
                    FROM {REACTION_TABLE}
                    WHERE {nameof(Reaction.MessageId)} = @{nameof(parameters.messageId)}
                      AND {nameof(Reaction.Type)}      = @{nameof(parameters.type)}
                      AND {nameof(Reaction.UserId)}    = @{nameof(parameters.userId)};
                SELECT *
                    FROM {MESSAGE_VIEW}
                    WHERE {nameof(MemeMessage.Id)} = @{nameof(parameters.messageId)};", parameters);
        }

        public async Task<Response> InsertResponse(
            int messageId,
            string responseUrl,
            string teamId,
            string teamDomain,
            string channelId,
            string channelName,
            string userId,
            string userName)
        {
            if (responseUrl == null) throw new ArgumentNullException(nameof(responseUrl));
            if (teamId == null) throw new ArgumentNullException(nameof(teamId));
            if (teamDomain == null) throw new ArgumentNullException(nameof(teamDomain));
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            if (channelName == null) throw new ArgumentNullException(nameof(channelName));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (userName == null) throw new ArgumentNullException(nameof(userName));

            var parameters = new
            {
                guid = Format(Guid.NewGuid()),
                now = Format(DateTime.UtcNow),
                messageId,
                responseUrl,
                teamId,
                teamDomain,
                channelId,
                channelName,
                userId,
                userName,
            };

            return await _connection.QuerySingleAsync<Response>($@"
                INSERT INTO {RESPONSE_TABLE} (
                    {nameof(Response.MessageId)},
                    {nameof(Response.Guid)},
                    {nameof(Response.CreateDate)},
                    {nameof(Response.ResponseUrl)},
                    {nameof(Response.TeamId)},
                    {nameof(Response.TeamDomain)},
                    {nameof(Response.ChannelId)},
                    {nameof(Response.ChannelName)},
                    {nameof(Response.UserId)},
                    {nameof(Response.UserName)}
                ) VALUES (
                    @{nameof(parameters.messageId)},
                    @{nameof(parameters.guid)},
                    @{nameof(parameters.now)},
                    @{nameof(parameters.responseUrl)},
                    @{nameof(parameters.teamId)},
                    @{nameof(parameters.teamDomain)},
                    @{nameof(parameters.channelId)},
                    @{nameof(parameters.channelName)},
                    @{nameof(parameters.userId)},
                    @{nameof(parameters.userName)}
                );
                SELECT *
                    FROM {RESPONSE_TABLE}
                    WHERE {nameof(Response.Id)} = last_insert_rowid();", parameters);
        }

        public async Task<Response> SelectResponse(Guid guid)
        {
            var parameters = new
            {
                guid = Format(guid),
            };

            return await _connection.QuerySingleOrDefaultAsync<Response>($@"
                SELECT *
                    FROM {RESPONSE_TABLE}
                    WHERE {nameof(Response.Guid)} = @{nameof(parameters.guid)};", parameters);
        }

        public async Task DeleteResponse(int responseId)
        {
            var parameters = new
            {
                responseId,
            };

            await _connection.ExecuteAsync($@"
                DELETE
                    FROM {RESPONSE_TABLE}
                    WHERE {nameof(Response.Id)} = @{nameof(parameters.responseId)};", parameters);
        }
    }
}
