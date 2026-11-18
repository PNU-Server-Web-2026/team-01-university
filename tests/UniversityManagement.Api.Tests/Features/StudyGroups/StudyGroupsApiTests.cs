using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UniversityManagement.Api.Features.StudyGroups;

namespace UniversityManagement.Api.Tests.Features.StudyGroups;

public class StudyGroupsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StudyGroupsApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Returns200AndArray()
    {
        var response = await _client.GetAsync("/api/study-groups");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var groups = await response.Content.ReadFromJsonAsync<List<StudyGroupResponse>>();
        Assert.NotNull(groups);
    }

    [Fact]
    public async Task GetById_NotExisting_Returns404()
    {
        var response = await _client.GetAsync("/api/study-groups/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Returns201AndLocationHeader()
    {
        var request = new CreateStudyGroupRequest("КН-31", 3, "Комп'ютерні науки");

        var response = await _client.PostAsJsonAsync("/api/study-groups", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<StudyGroupResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("КН-31", created.Name);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        var createRequest = new CreateStudyGroupRequest("КН-32", 3, "Комп'ютерні науки");
        var createResponse = await _client.PostAsJsonAsync("/api/study-groups", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<StudyGroupResponse>();

        var response = await _client.GetAsync($"/api/study-groups/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var group = await response.Content.ReadFromJsonAsync<StudyGroupResponse>();
        Assert.NotNull(group);
        Assert.Equal("КН-32", group.Name);
    }

    [Fact]
    public async Task Update_NotExisting_Returns404()
    {
        var request = new UpdateStudyGroupRequest("КН-31", 3, "Комп'ютерні науки");
        var response = await _client.PutAsJsonAsync("/api/study-groups/99999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Existing_Returns204()
    {
        var createRequest = new CreateStudyGroupRequest("ІПЗ-11", 1, "Інженерія програмного забезпечення");
        var createResponse = await _client.PostAsJsonAsync("/api/study-groups", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<StudyGroupResponse>();

        var updateRequest = new UpdateStudyGroupRequest("ІПЗ-12", 1, "Інженерія програмного забезпечення");
        var updateResponse = await _client.PutAsJsonAsync($"/api/study-groups/{created!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NotExisting_Returns404()
    {
        var response = await _client.DeleteAsync("/api/study-groups/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        var createRequest = new CreateStudyGroupRequest("КН-41", 4, "Комп'ютерні науки");
        var createResponse = await _client.PostAsJsonAsync("/api/study-groups", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<StudyGroupResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/study-groups/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/study-groups/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}