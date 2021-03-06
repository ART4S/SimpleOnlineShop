﻿using AutoMapper;
using Domian.Entities;

namespace Application.Features.Products.GetAllPaged
{
    public class GetAllPagedProfile : Profile
    {
        public GetAllPagedProfile()
        {
            CreateMap<Product, ProductItemDto>(MemberList.Destination)
                .ForMember(src => src.PictureUrl, opt => opt.MapFrom(x => x.MainPictureId));
        }
    }
}
