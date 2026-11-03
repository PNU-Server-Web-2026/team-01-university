using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UniversityManagement.Api.Features.Departments;

namespace UniversityManagement.Api.Tests.Features.Departments;

public class DepartmentsApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly IDepartmentService _departmentService;

    public DepartmentsApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
        _departmentService = factory.Services.GetRequiredService<IDepartmentService>();
        _departmentService.Clear();
    }

    [Fact]
    public async Task GetAll_Empty_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/departments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var departments = await response.Content.ReadFromJsonAsync<List<DepartmentResponse>>();
        Assert.NotNull(departments);
        Assert.Empty(departments);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/departments/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocation()
    {
        var request = new CreateDepartmentRequest(
            "Computer Science",
            "CS",
            "Faculty of Engineering"
        );

        var response = await _client.PostAsJsonAsync("/api/departments", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = response.Headers.Location;
        Assert.NotNull(location);
        Assert.StartsWith("/api/departments/", location.ToString());

        var department = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(department);
        Assert.Equal("Computer Science", department.Name);
        Assert.Equal("CS", department.Code);
        Assert.Equal("Faculty of Engineering", department.Faculty);
    }

    [Fact]
    public async Task Create_InvalidCode_Returns400()
    {
        var request = new CreateDepartmentRequest(
            "Computer Science",
            "cs", // lowercase, should be uppercase
            "Faculty of Engineering"
        );

        var response = await _client.PostAsJsonAsync("/api/departments", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        // First create a department
        var createRequest = new CreateDepartmentRequest(
            "Computer Science",
            "CS",
            "Faculty of Engineering"
        );
        var createResponse = await _client.PostAsJsonAsync("/api/departments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(created);

        // Then get it
        var response = await _client.GetAsync($"/api/departments/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(department);
        Assert.Equal(created.Id, department.Id);
        Assert.Equal("Computer Science", department.Name);
    }

    [Fact]
    public async Task Update_Existing_Returns200()
    {
        // First create a department
        var createRequest = new CreateDepartmentRequest(
            "Computer Science",
            "CS",
            "Faculty of Engineering"
        );
        var createResponse = await _client.PostAsJsonAsync("/api/departments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(created);

        // Then update it
        var updateRequest = new UpdateDepartmentRequest(
            "Computer Science Updated",
            "CSE",
            "Faculty of Engineering"
        );
        var response = await _client.PutAsJsonAsync($"/api/departments/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(department);
        Assert.Equal(created.Id, department.Id);
        Assert.Equal("Computer Science Updated", department.Name);
        Assert.Equal("CSE", department.Code);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var updateRequest = new UpdateDepartmentRequest(
            "Computer Science",
            "CS",
            "Faculty of Engineering"
        );

        var response = await _client.PutAsJsonAsync("/api/departments/999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        // First create a department
        var createRequest = new CreateDepartmentRequest(
            "Computer Science",
            "CS",
            "Faculty of Engineering"
        );
        var createResponse = await _client.PostAsJsonAsync("/api/departments", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(created);

        // Then delete it
        var response = await _client.DeleteAsync($"/api/departments/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var response = await _client.DeleteAsync("/api/departments/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_AfterCreate_ReturnsList()
    {
        // Create two departments
        var request1 = new CreateDepartmentRequest("Computer Science", "CS", "Faculty of Engineering");
        await _client.PostAsJsonAsync("/api/departments", request1);

        var request2 = new CreateDepartmentRequest("Mathematics", "MATH", "Faculty of Science");
        await _client.PostAsJsonAsync("/api/departments", request2);

        // Get all
        var response = await _client.GetAsync("/api/departments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var departments = await response.Content.ReadFromJsonAsync<List<DepartmentResponse>>();
        Assert.NotNull(departments);
        Assert.Equal(2, departments.Count);
    }
}
