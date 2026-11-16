using Xunit;

namespace UniversityManagement.Api.Tests;

[CollectionDefinition("Api collection")]
public class SharedTestCollection : ICollectionFixture<ApiFactory>
{
    
}