using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRepository<Partner>> _partnerRepoMock;
        private readonly PartnersController _controller;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            // Настраиваем AutoFixture с поддержкой Moq
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization { ConfigureMembers = false });

            // Замораживаем mock репозитория, чтобы он использовался везде один и тот же
            _partnerRepoMock = _fixture.Freeze<Mock<IRepository<Partner>>>();

            // Создаём контроллер вручную (MVC-инфраструктура не нужна)
            _controller = new PartnersController(_partnerRepoMock.Object);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            _partnerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Partner)null);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 10)
                .With(x => x.EndDate, DateTime.Now.AddDays(3))
                .Create();

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerInactive_ReturnsBadRequest()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .WithIsActive(false)
                .Build();

            _partnerRepoMock.Setup(r => r.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 10)
                .With(x => x.EndDate, DateTime.Now.AddDays(5))
                .Create();

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value.Should().Be("Данный партнер не активен");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitLessOrEqualZero_ReturnsBadRequest()
        {
            // Arrange
            var partner = new PartnerBuilder().Build();

            _partnerRepoMock.Setup(r => r.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 0)
                .With(x => x.EndDate, DateTime.Now.AddDays(5))
                .Create();

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value.Should().Be("Лимит должен быть больше 0");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WithActiveLimit_CancelsOldAndResetsCount()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .WithIssuedPromoCodes(5)
                .WithActiveLimit(100)
                .Build();

            _partnerRepoMock.Setup(r => r.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            _partnerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 300)
                .With(x => x.EndDate, DateTime.Now.AddDays(15))
                .Create();

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(0);
            partner.PartnerLimits.Should().Contain(l => l.Limit == 300);
            partner.PartnerLimits.Should().Contain(l => l.CancelDate != null);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_SavesNewLimitToRepository()
        {
            // Arrange
            var partner = new PartnerBuilder().Build();

            _partnerRepoMock.Setup(r => r.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            _partnerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 50)
                .With(x => x.EndDate, DateTime.Now.AddDays(10))
                .Create();

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();

            _partnerRepoMock.Verify(r => r.UpdateAsync(It.Is<Partner>(
                p => p.PartnerLimits.Any(l => l.Limit == 50)
            )), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ExpiredLimit_DoesNotResetIssuedPromoCodes()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .WithIssuedPromoCodes(5)
                .WithExpiredLimit(100) // лимит закончился
                .Build();

            _partnerRepoMock.Setup(r => r.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            _partnerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask);

            var request = _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                .With(x => x.Limit, 200)
                .With(x => x.EndDate, DateTime.Now.AddDays(10))
                .Create();

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(5, "если лимит истёк, количество промокодов не должно обнуляться");
        }

    }

    internal class PartnerBuilder
    {
        private readonly Partner _partner;

        public PartnerBuilder()
        {
            _partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = "TestPartner",
                IsActive = true,
                NumberIssuedPromoCodes = 0,
                PartnerLimits = new System.Collections.Generic.List<PartnerPromoCodeLimit>()
            };
        }

        public PartnerBuilder WithIsActive(bool isActive)
        {
            _partner.IsActive = isActive;
            return this;
        }

        public PartnerBuilder WithIssuedPromoCodes(int count)
        {
            _partner.NumberIssuedPromoCodes = count;
            return this;
        }

        public PartnerBuilder WithActiveLimit(int limit)
        {
            _partner.PartnerLimits.Add(new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                Limit = limit,
                PartnerId = _partner.Id,
                CreateDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(10),
                CancelDate = null
            });
            return this;
        }

        public PartnerBuilder WithExpiredLimit(int limit)
        {
            _partner.PartnerLimits.Add(new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                Limit = limit,
                PartnerId = _partner.Id,
                CreateDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(-5), // истёк 5 дней назад
                CancelDate = null
            });
            return this;
        }

        public Partner Build() => _partner;
    }
}