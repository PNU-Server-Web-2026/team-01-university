using System.Net; 
using System.Net.Http.Json; 
using Microsoft.AspNetCore.Mvc.Testing; 
using UniversityManagement.Api.Features.Students; 

namespace UniversityManagement.Api.Tests.Features.Students; 


public class StudentsApiTests : IClassFixture<WebApplicationFactory<Program>> 
{
    private readonly HttpClient _client; 

    public StudentsApiTests(WebApplicationFactory<Program> factory) 
    {
        _client = factory.CreateClient(); 
    }

    [Fact] 
    public async Task GetAll_Returns200AndArray()
    {
        var response = await _client.GetAsync("/api/students");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var students = await response.Content.ReadFromJsonAsync<List<StudentResponse>>();
        Assert.NotNull(students); 
    }

    [Fact] 
    public async Task Create_Returns201AndLocationHeader()
    {
        
        var request = new CreateStudentRequest("Ivan", "Franko", "ivan@test.com", "12345678", new DateOnly(1856, 8, 27));
        
        
        var response = await _client.PostAsJsonAsync("/api/students", request);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        
        
        var created = await response.Content.ReadFromJsonAsync<StudentResponse>();
        Assert.True(created!.Id > 0); 
    }

    [Fact] 
    public async Task GetById_NotExisting_Returns404()
    {
        
        var response = await _client.GetAsync("/api/students/9999");
        
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] 
    public async Task Update_NotExisting_Returns404()
    {
        
        var request = new UpdateStudentRequest("Taras", "Shevchenko", "taras@test.com", "87654321", new DateOnly(1814, 3, 9));
        
        
        var response = await _client.PutAsJsonAsync("/api/students/9999", request);
        
       
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] 
    public async Task CreateUpdateDelete_ReturnsCorrectStatuses()
    {
       
        var createRequest = new CreateStudentRequest("Lesya", "Ukrainka", "lesya@test.com", "11112222", new DateOnly(1871, 2, 25));
        var createResponse = await _client.PostAsJsonAsync("/api/students", createRequest);
        var createdStudent = await createResponse.Content.ReadFromJsonAsync<StudentResponse>();
        var id = createdStudent!.Id;

        
        var updateRequest = new UpdateStudentRequest("Lesya", "Kosach-Kvitka", "lesya@test.com", "11112222", new DateOnly(1871, 2, 25));
        var updateResponse = await _client.PutAsJsonAsync($"/api/students/{id}", updateRequest);
        
        
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

      
        var deleteResponse = await _client.DeleteAsync($"/api/students/{id}");
        
        
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}