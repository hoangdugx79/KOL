using KOL_KOC_TAAA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid GetCurrentUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idString, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var conversations = await _chatService.GetUserConversationsAsync(userId);
        ViewBag.CurrentUserId = userId;
        return View(conversations);
    }

    [HttpGet]
    public async Task<IActionResult> Conversation(Guid id)
    {
        var userId = GetCurrentUserId();
        var conversation = await _chatService.GetConversationAsync(id, userId);
        
        if (conversation == null) return NotFound();

        var messages = await _chatService.GetMessageHistoryAsync(id);
        
        ViewBag.CurrentUserId = userId;
        ViewBag.Messages = messages;
        
        return View(conversation);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(Guid conversationId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return BadRequest();

        var userId = GetCurrentUserId();
        var message = await _chatService.SendMessageAsync(conversationId, userId, content);

        return Json(new { 
            success = true, 
            message = new {
                senderId = message.SenderUserId,
                content = message.Content,
                sentAt = message.SentAt.ToString("HH:mm")
            }
        });
    }

    [HttpGet]
    [Route("Chat/StartWithKol/{kolId}")]
    public async Task<IActionResult> StartWithKol(Guid kolId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Challenge();

        // Don't chat with yourself
        if (userId == kolId)
        {
            TempData["ErrorMessage"] = "Bạn không thể nhắn tin cho chính mình.";
            return RedirectToAction("Index");
        }

        var conversation = await _chatService.GetOrCreateDirectConversationAsync(userId, kolId);
        return RedirectToAction(nameof(Conversation), new { id = conversation.Id });
    }
}
