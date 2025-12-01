using MediatR;
using Microsoft.EntityFrameworkCore;
using StudentHelperAPI.Core.Abstractions;
using StudentHelperAPI.Core.Common;
using StudentHelperAPI.Features.AI.Send;
using StudentHelperAPI.Models;

namespace StudentHelperAPI.Features.AI.Send
{
    public class SendMessageHandler : IRequestHandler<SendMessageRequest, Result<SendMessageResponse>>
    {
        private readonly HelperDbContext _context;
        private readonly IAiService _aiService;
        private readonly ILogger<SendMessageHandler> _logger;

        public SendMessageHandler(
            HelperDbContext context,
            IAiService aiService,
            ILogger<SendMessageHandler> logger)
        {
            _context = context;
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<Result<SendMessageResponse>> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("=== START AI REQUEST ===");
                _logger.LogInformation("User: {UserId}, Context: {ContextType}",
                    request.UserId, request.ContextType);

                _logger.LogInformation("Calling AI service...");
                var aiResult = await _aiService.GetResponseAsync(request.Message, request.ContextType, cancellationToken);

                if (aiResult.IsFailure)
                {
                    _logger.LogError("AI service error: {Error}", aiResult.Error);
                    return Result.Failure<SendMessageResponse>(aiResult.Error);
                }

                _logger.LogInformation("AI response received: {Length} chars", aiResult.Value.Length);
                //TODU Не создавать каждый раз новый чат, нормально сохронять в бд
                var conversation = new AiConversation
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Title = request.Message.Length > 50 ? request.Message[..47] + "..." : request.Message,
                    ContextType = request.ContextType,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var userMessage = new AiMessage
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    Content = request.Message,
                    IsUserMessage = true,
                    AiModel = "user",
                    CreatedAt = DateTime.UtcNow
                };

                var aiMessage = new AiMessage
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    Content = aiResult.Value,
                    IsUserMessage = false,
                    AiModel = "gigachat",
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    _context.AiConversations.Add(conversation);
                    _context.AiMessages.Add(userMessage);
                    _context.AiMessages.Add(aiMessage);

                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Successfully saved to database");
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database save error");

                    _logger.LogWarning("Returning AI response despite database error");

                    return Result.Success(new SendMessageResponse(
                        Answer: aiResult.Value,
                        ConversationId: conversation.Id.ToString(),
                        CreatedAt: DateTime.UtcNow
                    ));
                }

                _logger.LogInformation("=== END AI REQUEST ===");

                return Result.Success(new SendMessageResponse(
                    Answer: aiResult.Value,
                    ConversationId: conversation.Id.ToString(),
                    CreatedAt: DateTime.UtcNow
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in SendMessageHandler");
                return Result.Failure<SendMessageResponse>($"Internal server error: {ex.Message}");
            }
        }
    }
}