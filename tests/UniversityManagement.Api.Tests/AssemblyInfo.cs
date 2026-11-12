using Xunit;

// Serilog request logging пише через статичний Log.Logger, спільний для всіх
// тестових хостів. Паралельний запуск тестів призвів би до гонки, тому
// вимикаємо паралелізацію для всієї збірки тестів.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
