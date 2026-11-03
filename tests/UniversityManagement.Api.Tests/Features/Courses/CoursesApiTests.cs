using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UniversityManagement.Api.Features.Courses;

namespace UniversityManagement.Api.Tests.Features.Courses;

public class CoursesApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ICourseService _courseService;

    public CoursesApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
        _courseService = factory.Services.GetRequiredService<ICourseService>();
        _courseService.Clear();
    }

    [Fact]
    public async Task GetAll_Empty_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var courses = await response.Content.ReadFromJsonAsync<List<CourseResponse>>();
        Assert.NotNull(courses);
        Assert.Empty(courses);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/courses/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocation()
    {
        var request = new CreateCourseRequest(
            "Introduction to Programming",
            "CS-101",
            3,
            50,
            1
        );

        var response = await _client.PostAsJsonAsync("/api/courses", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = response.Headers.Location;
        Assert.NotNull(location);
        Assert.StartsWith("/api/courses/", location.ToString());

        var course = await response.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(course);
        Assert.Equal("Introduction to Programming", course.Title);
        Assert.Equal("CS-101", course.Code);
        Assert.Equal(3, course.Credits);
        Assert.Equal(50, course.Capacity);
        Assert.Equal(1, course.Semester);
    }

    [Fact]
    public async Task Create_InvalidCredits_Returns400()
    {
        var request = new CreateCourseRequest(
            "Introduction to Programming",
            "CS-101",
            20, // Invalid: must be 1-15
            50,
            1
        );

        var response = await _client.PostAsJsonAsync("/api/courses", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        // First create a course
        var createRequest = new CreateCourseRequest(
            "Data Structures",
            "CS-201",
            4,
            40,
            2
        );
        var createResponse = await _client.PostAsJsonAsync("/api/courses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(created);

        // Then get it
        var response = await _client.GetAsync($"/api/courses/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var course = await response.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(course);
        Assert.Equal(created.Id, course.Id);
        Assert.Equal("Data Structures", course.Title);
    }

    [Fact]
    public async Task Update_Existing_Returns200()
    {
        // First create a course
        var createRequest = new CreateCourseRequest(
            "Database Systems",
            "CS-202",
            3,
            45,
            2
        );
        var createResponse = await _client.PostAsJsonAsync("/api/courses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(created);

        // Then update it
        var updateRequest = new UpdateCourseRequest(
            "Advanced Database Systems",
            "CS-302",
            4,
            35,
            4
        );
        var response = await _client.PutAsJsonAsync($"/api/courses/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var course = await response.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(course);
        Assert.Equal(created.Id, course.Id);
        Assert.Equal("Advanced Database Systems", course.Title);
        Assert.Equal("CS-302", course.Code);
        Assert.Equal(4, course.Credits);
        Assert.Equal(35, course.Capacity);
        Assert.Equal(4, course.Semester);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var updateRequest = new UpdateCourseRequest(
            "Machine Learning",
            "CS-401",
            5,
            25,
            5
        );

        var response = await _client.PutAsJsonAsync("/api/courses/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        // First create a course
        var createRequest = new CreateCourseRequest(
            "Artificial Intelligence",
            "CS-402",
            4,
            30,
            6
        );
        var createResponse = await _client.PostAsJsonAsync("/api/courses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CourseResponse>();
        Assert.NotNull(created);

        // Then delete it
        var response = await _client.DeleteAsync($"/api/courses/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var response = await _client.DeleteAsync("/api/courses/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_AfterCreate_ReturnsList()
    {
        // Create two courses
        var request1 = new CreateCourseRequest("Introduction to Programming", "CS-101", 3, 50, 1);
        await _client.PostAsJsonAsync("/api/courses", request1);

        var request2 = new CreateCourseRequest("Data Structures", "CS-201", 4, 40, 2);
        await _client.PostAsJsonAsync("/api/courses", request2);

        // Get all
        var response = await _client.GetAsync("/api/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var courses = await response.Content.ReadFromJsonAsync<List<CourseResponse>>();
        Assert.NotNull(courses);
        Assert.Equal(2, courses.Count);
    }
}
