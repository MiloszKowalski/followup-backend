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
        public static async Task<List<InstaMedia>> GetMediaByHashtagAsync(this IInstaApi instaApi, InstagramAccount account, Promotion promotion, IPromotionRepository promotionRepository)
        {
            int firstHashtagCount = 0;
            var firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(1));
            if (firstHashtagResponse.Info.Message == "challenge_required")
            {
                await instaApi.GetLoggedInChallengeDataInfoAsync();
                await instaApi.AcceptChallengeAsync();
                firstHashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, PaginationParameters.MaxPagesToLoad(1));
            }

            if (!firstHashtagResponse.Succeeded)
                return null;

            var firstMediaList = firstHashtagResponse.Value.Medias;
            var mediaToRemoveList = new List<InstaMedia>();

            Console.WriteLine($"[{DateTime.Now}][{account.Username}] Getting media by tag {promotion.Label}...");

            foreach (var media in firstMediaList)
            {
                var blackMedia = await promotionRepository.GetMediaAsync(media.Code, account.Id);

                if (media.Code == blackMedia?.Code && blackMedia?.AccountId == account.Id)
                {
                    firstHashtagCount++;
                    mediaToRemoveList.Add(media);
                }
            }

            if (firstMediaList.Count > firstHashtagCount)
            {
                foreach (var mediaToRemove in mediaToRemoveList)
                    firstMediaList.Remove(mediaToRemove);

                return firstMediaList;
            }

            var pagination = PaginationParameters.MaxPagesToLoad(1);
            pagination.StartFromMinId(promotion.NextMinId);


            int hashtagCount = 0;
            var hashtagResponse = await instaApi.FeedProcessor.GetTagFeedAsync(promotion.Label, pagination);

            if (!hashtagResponse.Succeeded)
                return null;

            var hashtagMediaList = hashtagResponse.Value.Medias;

            foreach (var media in hashtagMediaList)
            {
                var blackMedia = await promotionRepository.GetMediaAsync(media.Code, account.Id);

                if (media.Code == blackMedia?.Code && blackMedia?.AccountId == account.Id)
                {
                    hashtagCount++;
                    mediaToRemoveList.Add(media);
                }
            }

            if (hashtagMediaList.Count > hashtagCount)
            {
                foreach (var mediaToRemove in mediaToRemoveList)
                    hashtagMediaList.Remove(mediaToRemove);

                return hashtagMediaList;
            }

            return null;
        }
    }
}
