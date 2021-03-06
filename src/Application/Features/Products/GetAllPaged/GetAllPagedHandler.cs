﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domian.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.GetAllPaged
{
    public class GetAllPagedHandler : IRequestHandler<GetAllPagedQuery, PagedResponse<ProductItemDto>>
    {
        private readonly IDbContext _db;
        private readonly IMapper _mapper;
        private readonly IUriBuilder _uriBuilder;

        public GetAllPagedHandler(IDbContext db, IMapper mapper, IUriBuilder uriBuilder)
        {
            _db = db;
            _mapper = mapper;
            _uriBuilder = uriBuilder;
        }

        public async Task<PagedResponse<ProductItemDto>> Handle(
            GetAllPagedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> products = _db.Products.AsNoTracking();

            products = ApplyFilters(products, request);
            products = ApplySort(products, request.SortDirection);

            PagedResponse<ProductItemDto> response = await products
                .ProjectTo<ProductItemDto>(_mapper.ConfigurationProvider)
                .PaginateAsync(request);

            SetPictureUrls(response.Items);

            return response;
        }

        private static IQueryable<Product> ApplyFilters(IQueryable<Product> products, GetAllPagedQuery request)
        {
            if (request.Types.Count > 0)
                products = products.Where(x => request.Types.Contains(x.TypeId));

            if (request.Brands.Count > 0)
                products = products.Where(x => request.Brands.Contains(x.BrandId));

            if (request.Category.HasValue)
                products = products.Where(x => x.Category == request.Category);

            if (request.MinPrice.HasValue)
                products = products.Where(x => x.Price >= request.MinPrice);

            if (request.MaxPrice.HasValue)
                products = products.Where(x => x.Price <= request.MaxPrice);

            return products;
        }

        private static IQueryable<Product> ApplySort(IQueryable<Product> products, SortDirection? direction)
        {
            return direction switch
            {
                SortDirection.PriceUp => products.OrderBy(x => x.Price),
                SortDirection.PriceDown => products.OrderByDescending(x => x.Price),
                SortDirection.Newly => products.OrderBy(x => x.UpdateDate ?? x.CreateDate),
                _ => products
            };
        }

        private void SetPictureUrls(IEnumerable<ProductItemDto> items)
        {
            foreach (ProductItemDto item in items)
            {
                if (!string.IsNullOrEmpty(item.PictureUrl))
                {
                    item.PictureUrl = _uriBuilder.GetPictureUrl(item.PictureUrl);
                }
            }
        }
    }
}