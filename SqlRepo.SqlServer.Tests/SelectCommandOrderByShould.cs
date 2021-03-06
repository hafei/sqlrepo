using System;
using NUnit.Framework;
using FluentAssertions;
using SqlRepo.Testing;

namespace SqlRepo.SqlServer.Tests
{
    [TestFixture]
    public class SelectCommandOrderByShould : SelectCommandTestBase
    {
        [Test]
        public void ThrowErrorIfOrderingAttemptedBeforeTableSpecified()
        {
            const string ExpectedMessage =
                "A table specification for the entity type and alias must be set using From or one of the Join methods before filtering, sorting or grouping can be applied.";
            this.Command.Invoking(c => c.OrderBy<InnerEntity>(e => e.IntProperty))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(ExpectedMessage);
            this.Command.Invoking(c => c.OrderByDescending<InnerEntity>(e => e.IntProperty))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(ExpectedMessage);
        }

        [Test]
        public void AddSpecificationOnOrderBy()
        {
            this.Command.OrderBy(e => e.IntProperty);
            this.AssertOrderBySpecificationIsValid<TestEntity>(0, IntPropertyName);
        }

        [Test]
        public void AddSpecificationOnOrderByDescending()
        {
            this.Command.OrderByDescending(e => e.IntProperty);
            this.AssertOrderBySpecificationIsValid<TestEntity>(0,
                IntPropertyName,
                orderByDirection: OrderByDirection.Descending);
        }

        [Test]
        public void AddSpecificationOnOrderByWithAlias()
        {
            const string Alias = "a";
            this.Command.From(Alias).OrderBy(e => e.IntProperty, Alias);
            this.AssertOrderBySpecificationIsValid<TestEntity>(0, IntPropertyName, Alias);
        }

        [Test]
        public void AddSpecificationOnOrderByDescendingWithAlias()
        {
            const string Alias = "a";
            this.Command.From(Alias).OrderByDescending(e => e.IntProperty, Alias);
            this.AssertOrderBySpecificationIsValid<TestEntity>(0,
                IntPropertyName,
                Alias,
                OrderByDirection.Descending);
        }

        [Test]
        public void AddMultipleSpecificationsOnOrderBy()
        {
            this.Command.OrderBy(e => e.IntProperty, null, e => e.StringProperty);
            this.AssertOrderBySpecificationIsValid<TestEntity>(0, IntPropertyName);
            this.AssertOrderBySpecificationIsValid<TestEntity>(1, StringPropertyName);
        }

        private void AssertOrderBySpecificationIsValid<T>(int index,
            string identifier,
            string @alias = null,
            OrderByDirection orderByDirection = OrderByDirection.Ascending)
        {
            this.Command.Specification.Orderings.Should()
                .NotBeNullOrEmpty();
            var specification = this.Command.Specification.Orderings[index];
            specification.Identifer.Should()
                         .Be(identifier);
            specification.Direction.Should()
                         .Be(orderByDirection);
            specification.Alias.Should()
                         .Be(@alias);
            specification.EntityType.Should()
                         .Be(typeof(T));
        }
    }
}