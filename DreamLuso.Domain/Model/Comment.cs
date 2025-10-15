using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace DreamLuso.Domain.Model;

public class Comment : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public required string Message { get; set; }
    public int? Rating { get; set; }  // 1-5 estrelas
    public int HelpfulCount { get; set; } = 0;
    public bool IsFlagged { get; set; } = false;
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    
    // Navigation Properties
    public List<Comment> Replies { get; set; }

    public Comment()
    {
        Id = Guid.NewGuid();
        Replies = [];
    }

    [SetsRequiredMembers]
    public Comment(
        Guid propertyId,
        Guid userId,
        string message,
        int? rating = null,
        Guid? parentCommentId = null)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        UserId = userId;
        Message = message;
        Rating = rating;
        ParentCommentId = parentCommentId;
        Replies = [];
        IsFlagged = false;
        HelpfulCount = 0;
    }

    public bool IsReply => ParentCommentId.HasValue;

    public void IncrementHelpfulCount()
    {
        HelpfulCount++;
        UpdateTimestamp();
    }

    public void DecrementHelpfulCount()
    {
        if (HelpfulCount > 0)
            HelpfulCount--;
        UpdateTimestamp();
    }

    public void Flag()
    {
        IsFlagged = true;
        UpdateTimestamp();
    }

    public void Unflag()
    {
        IsFlagged = false;
        UpdateTimestamp();
    }

    public void UpdateMessage(string newMessage)
    {
        Message = newMessage;
        UpdateTimestamp();
    }
}

