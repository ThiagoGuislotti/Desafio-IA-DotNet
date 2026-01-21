using CustomerPlatform.Application.Cqrs.Queries;
using CustomerPlatform.Application.Validators;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Validators
{
    [Trait("Application", "Validators")]
    public sealed class SearchCustomersQueryValidatorTests
    {
        #region Variables
        private readonly SearchCustomersQueryValidator _validator;
        #endregion

        #region SetUp Methods
        public SearchCustomersQueryValidatorTests()
        {
            _validator = new SearchCustomersQueryValidator();
        }
        #endregion

        #region Test Methods - Validate Valid Cases
        [Fact]
        public void Validate_DefaultRequest_ShouldBeValid()
        {
            var query = new SearchCustomersQuery();

            var result = _validator.Validate(query);

            Assert.True(result.IsValid);
        }
        #endregion

        #region Test Methods - Validate Invalid Cases
        [Fact]
        public void Validate_PageNumberZero_ShouldBeInvalid()
        {
            var query = new SearchCustomersQuery { PageNumber = 0 };

            var result = _validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "PageNumber");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Validate_PageSizeOutOfRange_ShouldBeInvalid(int size)
        {
            var query = new SearchCustomersQuery { PageSize = size };

            var result = _validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "PageSize");
        }

        [Fact]
        public void Validate_NameTooLong_ShouldBeInvalid()
        {
            var query = new SearchCustomersQuery { Name = new string('a', 201) };

            var result = _validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Name");
        }
        #endregion
    }
}
