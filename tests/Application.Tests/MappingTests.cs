﻿using System;
using System.Collections.Generic;
using Application.Features.Carts.GetItems;
using Application.Features.ProductBrands.GetAll;
using Application.Features.Products.GetAllPaged;
using Application.Features.Products.GetById;
using Application.Features.ProductTypes.GetAll;
using AutoMapper;
using Domian.Entities;
using FluentAssertions;
using Xunit;

namespace Application.Tests
{
    public class MappingTests
    {
        private static MapperConfiguration CreateConfiguration()
        {
            return new MapperConfiguration(config =>
            {
                config.AddMaps(typeof(GetByIdProfile).Assembly);
            });
        }

        [Fact]
        public void ShouldAssertValidConfiguration()
        {
            // Arrange
            MapperConfiguration configuration = CreateConfiguration();

            // Act
            Action act = () => configuration.AssertConfigurationIsValid();

            // Assert
            act.Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(Mappings))]
        public void ShouldAllowMap(Type sourceType, Type destinationType)
        {
            // Arrange
            MapperConfiguration configuration = CreateConfiguration();

            // Act
            Action act = () =>
            {
                object instance = Activator.CreateInstance(sourceType);

                IMapper mapper = configuration.CreateMapper();

                mapper.Map(instance, sourceType, destinationType);
            };

            // Assert
            act.Should().NotThrow();
        }

        public static IEnumerable<object[]> Mappings
        {
            get
            {
                yield return new object[] { typeof(Product), typeof(ProductInfoDto) };
                yield return new object[] { typeof(Product), typeof(ProductItemDto) };
                yield return new object[] { typeof(Product), typeof(CartItemDto) };

                yield return new object[] { typeof(ProductBrand), typeof(ProductBrandDto) };

                yield return new object[] { typeof(ProductType), typeof(ProductTypeDto) };
            }
        }
	}
}
