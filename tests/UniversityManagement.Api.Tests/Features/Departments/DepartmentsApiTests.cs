using System.Net;
using System.Net.Http.Json;
using UniversityManagement.Api.Features.Departments;

namespace UniversityManagement.Api.Tests.Features.Departments;

public class DepartmentsApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const int MissingId = int.MaxValue;

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ContainsCreatedDepartment()
    {
        var created = await CreateDepartmentAsync();

        var response = await _client.GetAsync("/api/departments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var departments = await response.Content.ReadFromJsonAsync<List<DepartmentResponse>>();
        Assert.NotNull(departments);
        Assert.Contains(departments, d => d == created);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        var created = await CreateDepartmentAsync();

        var response = await _client.GetAsync($"/api/departments/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.Equal(created, department);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/departments/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocation()
    {
        var request = new CreateDepartmentRequest("Computer Science", UniqueCode(), "Faculty of Engineering");

        var response = await _client.PostAsJsonAsync("/api/departments", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var department = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(department);
        Assert.Equal(request.Name, department.Name);
        Assert.Equal(request.Code, department.Code);
        Assert.Equal(request.Faculty, department.Faculty);

        var location = response.Headers.Location;
        Assert.NotNull(location);
        Assert.EndsWith($"/api/departments/{department.Id}", location.ToString());

        var getResponse = await _client.GetAsync(location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(department, await getResponse.Content.ReadFromJsonAsync<DepartmentResponse>());
    }

    [Fact]
    public async Task Create_InvalidCode_Returns400()
    {
        var request = new CreateDepartmentRequest("Computer Science", "cs", "Faculty of Engineering");

        var response = await _client.PostAsJsonAsync("/api/departments", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Existing_Returns204AndUpdatesDepartment()
    {
        var created = await CreateDepartmentAsync();
        var updateRequest = new UpdateDepartmentRequest("Computer Science Updated", UniqueCode(), "Faculty of Science");

        var response = await _client.PutAsJsonAsync($"/api/departments/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var department = await _client.GetFromJsonAsync<DepartmentResponse>($"/api/departments/{created.Id}");
        Assert.Equal(
            new DepartmentResponse(created.Id, updateRequest.Name, updateRequest.Code, updateRequest.Faculty),
            department);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var updateRequest = new UpdateDepartmentRequest("Computer Science", UniqueCode(), "Faculty of Engineering");

        var response = await _client.PutAsJsonAsync($"/api/departments/{MissingId}", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_InvalidCode_Returns400()
    {
        var created = await CreateDepartmentAsync();
        var updateRequest = new UpdateDepartmentRequest("Computer Science", "cs", "Faculty of Engineering");

        var response = await _client.PutAsJsonAsync($"/api/departments/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_Returns204AndRemovesDepartment()
    {
        var created = await CreateDepartmentAsync();

        var response = await _client.DeleteAsync($"/api/departments/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/departments/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/departments/{MissingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<DepartmentResponse> CreateDepartmentAsync()
    {
        var request = new CreateDepartmentRequest("Computer Science", UniqueCode(), "Faculty of Engineering");
        var response = await _client.PostAsJsonAsync("/api/departments", request);
        response.EnsureSuccessStatusCode();

        var department = await response.Content.ReadFromJsonAsync<DepartmentResponse>();
        Assert.NotNull(department);
        return department;
    }

    private static string UniqueCode() =>
        string.Concat(Guid.NewGuid().ToByteArray().Take(10).Select(b => (char)('A' + b % 26)));
}
