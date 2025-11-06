using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Shared.IHub
{
    public interface IEcommerceHubClient
    {
        //    Task PostChange(PostDto postDto);
        //    Task PostDelete(Guid postId);
        //    Task PostLike(Guid postId, Guid userId);
        //    Task PostUnLike(Guid postId, Guid userId);
        //    Task PostBookmark(Guid postId, Guid userId);
        //    Task PostUnBookmark(Guid postId, Guid userId);
        //    Task PostCommentAdded(CommentDto commentDto);
        //    Task UserPhotoChange(UserPhotoChange userPhotoChange);
        //    Task ReceiveNotification(NotificationDto notification); // Gộp NotificationGenerated và ReceiveNotification
        //    Task FollowNotification(FollowNotificationDto notification);

        //    // Thêm các phương thức cho chức năng kết bạn
        //    Task FriendRequestSent(FriendRequestNotificationDto notification);
        //    Task FriendRequestAccepted(FriendRequestNotificationDto notification);
        //    Task FriendRequestRejected(FriendRequestNotificationDto notification);
        //    Task FriendRemoved(FriendRequestNotificationDto notification);

        //    // Chat Messages
        //    Task ReceiveMessage(MessageDto message);
        //    Task ReceiveMessage(Guid fromUserId, string message); // Legacy support
        //    Task MessageRead(Guid messageId);
        //    Task MessageDeleted(Guid messageId);
        //    Task AllMessagesRead(Guid fromUserId);

        //    // User Status
        //    Task UserOnlineStatusChanged(Guid userId, bool isOnline);
        //    Task UserTyping(Guid userId, bool isTyping);

        //    // Group/Chat Room Operations
        //    Task JoinedGroup(string groupName);
        //    Task LeftGroup(string groupName);
        //    Task GroupMessage(string groupName, string message);
        //}

        //public record struct UserPhotoChange(Guid UserId, string? UserPhotoUrl);

        //// DTO cho các thông báo kết bạn
        //public record struct FriendRequestNotificationDto(Guid FromUserId, string FromUserName, Guid ToUserId);
    }
}
