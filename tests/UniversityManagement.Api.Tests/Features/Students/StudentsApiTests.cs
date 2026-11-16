using System.Collections.Concurrent;
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
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        // Перевірка GET за заголовком Location
        var getByLocationResponse = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getByLocationResponse.StatusCode);

        var fetched = await getByLocationResponse.Content.ReadFromJsonAsync<StudentResponse>();
        Assert.Equal(created, fetched);
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

        var updateRequest = new UpdateStudentRequest("Larysa", "Kosach-Kvitka", "larysa@test.com", "33334444", new DateOnly(1871, 2, 26));
        var updateResponse = await _client.PutAsJsonAsync($"/api/students/{id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // Перевірка, що всі поля збереглися після PUT
        var getAfterUpdateResponse = await _client.GetAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.OK, getAfterUpdateResponse.StatusCode);

        var updatedStudent = await getAfterUpdateResponse.Content.ReadFromJsonAsync<StudentResponse>();
        Assert.NotNull(updatedStudent);
        Assert.Equal(id, updatedStudent.Id);
        Assert.Equal(updateRequest.FirstName, updatedStudent.FirstName);
        Assert.Equal(updateRequest.LastName, updatedStudent.LastName);
        Assert.Equal(updateRequest.Email, updatedStudent.Email);
        Assert.Equal(updateRequest.RecordBookNumber, updatedStudent.RecordBookNumber);
        Assert.Equal(updateRequest.BirthDate , updatedStudent.BirthDate );

        var deleteResponse = await _client.DeleteAsync($"/api/students/{id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task ConcurrentUpdatesAndReads_NeverReturnMixedState()
    {
        var createRequest = new CreateStudentRequest("InitFirst", "InitLast", "init@test.com", "00000001", new DateOnly(2005, 1, 1));
        var createResponse = await _client.PostAsJsonAsync("/api/students", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<StudentResponse>();
        var id = created!.Id;

        var validStates = new[]
        {
            new UpdateStudentRequest("AlphaFirst", "AlphaLast", "alpha@test.com", "11111111", new DateOnly(2004, 1, 1)),
            new UpdateStudentRequest("BetaFirst", "BetaLast", "beta@test.com", "22222222", new DateOnly(2004, 2, 2)),
            new UpdateStudentRequest("GammaFirst", "GammaLast", "gamma@test.com", "33333333", new DateOnly(2004, 3, 3))
        };

        var errors = new ConcurrentBag<string>();

        var writers = Enumerable.Range(0, 30).Select(i => Task.Run(async () =>
        {
            var payload = validStates[i % validStates.Length];
            var response = await _client.PutAsJsonAsync($"/api/students/{id}", payload);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                errors.Add($"Unexpected PUT status: {response.StatusCode}");
            }
        }));

        var readers = Enumerable.Range(0, 30).Select(_ => Task.Run(async () =>
        {
            var student = await _client.GetFromJsonAsync<StudentResponse>($"/api/students/{id}");
            if (student is null)
            {
                errors.Add("Student returned null during concurrent update.");
                return;
            }

            var matchesInit = student.FirstName == createRequest.FirstName
                              && student.LastName == createRequest.LastName
                              && student.Email == createRequest.Email
                              && student.RecordBookNumber == createRequest.RecordBookNumber
                              && student.BirthDate  == createRequest.BirthDate ;

            var matchesAnyValid = validStates.Any(v =>
                student.FirstName == v.FirstName &&
                student.LastName == v.LastName &&
                student.Email == v.Email &&
                student.RecordBookNumber == v.RecordBookNumber &&
                student.BirthDate  == v.BirthDate );

            if (!matchesInit && !matchesAnyValid)
            {
                errors.Add($"Torn/mixed state detected: {student}");
            }
        }));

        await Task.WhenAll(writers.Concat(readers));
        Assert.Empty(errors);
    }
}