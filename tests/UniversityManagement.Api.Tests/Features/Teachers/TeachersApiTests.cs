using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UniversityManagement.Api.Features.Teachers;
using Xunit;

namespace UniversityManagement.Api.Tests.Features.Teachers;

public class TeachersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TeachersApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndArray()
    {
        var response = await _client.GetAsync("/api/teachers");
        response.EnsureSuccessStatusCode();

        var teachers = await response.Content.ReadFromJsonAsync<TeacherResponse[]>();
        Assert.NotNull(teachers);
    }

    [Fact]
    public async Task GetById_NotExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/teachers/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation_AndCanBeFetched()
    {
        var request = new CreateTeacherRequest("Ivan", "Franko", "ivan@test.com", AcademicTitle.Professor);

        var postResponse = await _client.PostAsJsonAsync("/api/teachers", request);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);

        var createdTeacher = await postResponse.Content.ReadFromJsonAsync<TeacherResponse>();
        Assert.NotNull(createdTeacher);
        Assert.Equal("Ivan", createdTeacher.FirstName);

        var getResponse = await _client.GetAsync(postResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Update_Existing_ReturnsNoContent()
    {
        var createRequest = new CreateTeacherRequest("Taras", "Shevchenko", "taras@test.com", AcademicTitle.AssociateProfessor);
        var postResponse = await _client.PostAsJsonAsync("/api/teachers", createRequest);
        var createdTeacher = await postResponse.Content.ReadFromJsonAsync<TeacherResponse>();

        var updateRequest = new UpdateTeacherRequest("Taras", "Shevchenko", "newemail@test.com", AcademicTitle.Professor);
        var putResponse = await _client.PutAsJsonAsync($"/api/teachers/{createdTeacher!.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
    }

    [Fact]
    public async Task Update_NotExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateTeacherRequest("Test", "Test", "test@test.com", AcademicTitle.Assistant);
        var putResponse = await _client.PutAsJsonAsync("/api/teachers/9999", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContent()
    {
        var createRequest = new CreateTeacherRequest("Lesya", "Ukrainka", "lesya@test.com", AcademicTitle.SeniorLecturer);
        var postResponse = await _client.PostAsJsonAsync("/api/teachers", createRequest);
        var createdTeacher = await postResponse.Content.ReadFromJsonAsync<TeacherResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/teachers/{createdTeacher!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NotExisting_ReturnsNotFound()
    {
        var deleteResponse = await _client.DeleteAsync("/api/teachers/9999");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
}
