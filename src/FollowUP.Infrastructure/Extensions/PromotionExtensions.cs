using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Extensions
{
    public static class PromotionExtensions
    {
        public static async Task<List<InstaMedia>> GetMediaByHashtagAsync(this IInstaApi instaApi, InstagramAccount account, Promotion promotion)
        {
            var firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(1));
            if (firstHashtagResponse.Info.Message == "challenge_required")
            {
                await instaApi.GetLoggedInChallengeDataInfoAsync();
                await instaApi.AcceptChallengeAsync();
                firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(1));
            }

            if (!firstHashtagResponse.Succeeded)
                return null;

            var firstMediaResponseList = firstHashtagResponse.Value.Medias;
            var firstMediaList = new List<InstaMedia>();

            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Getting media by tag {promotion.Label}...");

            foreach (var media in firstMediaResponseList)
            {
                if (media.InstaIdentifier != promotion.LastMediaId)
                    firstMediaList.Add(media);
                else break;
            }

            if (firstMediaList.Count > 0)
                return firstMediaList;

            var pagination = PaginationParameters.MaxPagesToLoad(1);
            pagination.StartFromMinId(promotion.NextMinId);

            var hashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, pagination);

            if (!hashtagResponse.Succeeded)
                return null;

            var hashtagMediaList = hashtagResponse.Value.Medias;

            return hashtagMediaList;
        }

        public static async Task<InstaFriendshipShortStatusList> GetRelationshipsByPromotionAsync(this IInstaApi instaApi, InstagramAccount account, Promotion promotion)
        {
            InstaFriendshipShortStatusList followersRelationships;
            var pagination = PaginationParameters.MaxPagesToLoad(1);
            do
            {
                var followersRequest = await instaApi.UserProcessor.GetUserFollowersAsync(promotion.Label, pagination);
                if (followersRequest.Info.Message == "challenge_required")
                {
                    await instaApi.GetLoggedInChallengeDataInfoAsync();
                    await instaApi.AcceptChallengeAsync();
                    followersRequest = await instaApi.UserProcessor.GetUserFollowersAsync(promotion.Label, pagination);
                }

                if (!followersRequest.Succeeded)
                    return null;

                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Getting relationships by user {promotion.Label}...");

                var userIds = new List<long>();

                var followers = followersRequest.Value;

                foreach (var follower in followers)
                    userIds.Add(follower.Pk);

                var followersRelationshipRequest = await instaApi.UserProcessor.GetFriendshipStatusesAsync(userIds.ToArray());

                if (!followersRelationshipRequest.Succeeded)
                    return null;

                followersRelationships = followersRelationshipRequest.Value;

                var relationshipsToRemove = new InstaFriendshipShortStatusList();

                foreach (var relationship in followersRelationships)
                    if (relationship.Following)
                        relationshipsToRemove.Add(relationship);

                foreach (var relationship in relationshipsToRemove)
                    followersRelationships.Remove(relationship);

                pagination.NextMaxId = followersRequest.Value.NextMaxId;
            }
            while (followersRelationships.Count <= 0);

            return followersRelationships;
        }
    }
}
