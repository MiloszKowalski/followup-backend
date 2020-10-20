using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;
using System.Collections.Generic;

namespace FollowUP.Infrastructure.Extensions
{
    public static class MapperExtenstions
    {
        public static IEnumerable<PromotionDto> MapFollowPromotions(this IMapper mapper,
            IEnumerable<FollowPromotion> promotions)
        {
            var promotionDtos = new List<PromotionDto>();

            foreach (var promotion in promotions)
            {
                var promotionDto = mapper.Map<PromotionDto>(promotion);

                promotionDto.PromotionType = promotion.GetPromotionType();

                promotionDtos.Add(promotionDto);
            }

            return promotionDtos;
        }
    }
}
