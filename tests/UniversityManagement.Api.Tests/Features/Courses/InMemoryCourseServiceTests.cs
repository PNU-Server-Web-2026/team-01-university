using UniversityManagement.Api.Features.Courses;

namespace UniversityManagement.Api.Tests.Features.Courses;

public class InMemoryCourseServiceTests
{
    [Fact]
    public async Task UpdateAsync_RacingWithDeleteAsync_DoesNotResurrectCourse()
    {
        var service = new InMemoryCourseService();
        var updateRequest = new UpdateCourseRequest("Updated Title", "UPD-1", 3, 30, 1);

        for (var i = 0; i < 500; i++)
        {
            var created = await service.CreateAsync(
                new CreateCourseRequest($"Course {i}", $"C-{i}", 3, 30, 1));

            await Task.WhenAll(
                Task.Run(() => service.UpdateAsync(created.Id, updateRequest)),
                Task.Run(() => service.DeleteAsync(created.Id)));

            Assert.Null(await service.GetByIdAsync(created.Id));
        }
    }
}
