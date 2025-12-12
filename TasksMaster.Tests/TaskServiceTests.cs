using Application.Interfaces;
using Application.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksMaster.Tests
{
    public class TaskServiceTests
    {
        #region GetAllTasksAsync Tests
        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnTasks_WhenTasksExist()
        {
            // ARRANGE (Opzetten)

            // 1. Maak voorbeeld Task data
            var expectedTasks = new List<Domain.Task>
            {
                new Domain.Task { Id = Guid.NewGuid(), Title = "Mock Task 1", Status = Domain.TaskStatus.ToDo },
                new Domain.Task { Id = Guid.NewGuid(), Title = "Mock Task 2", Status = Domain.TaskStatus.InProgress }
            };

            // 2. Mock de ITaskRepository
            var mockRepository = new Mock<ITaskRepository>();

            // 3. Instrueer de Mock: Zeg wat er moet gebeuren als de methode wordt aangeroepen
            mockRepository
                .Setup(repo => repo.GetAllTasksAsync()) // Wanneer deze methode wordt aangeroepen...
                .ReturnsAsync(expectedTasks);           // ... retourneer dan onze 'expectedTasks'

            // 4. Initialiseer de Service met de Mocked Repository
            var service = new TaskService(mockRepository.Object);


            // ACT (Uitvoeren)

            // Roep de methode aan die we willen testen
            var result = (await service.GetAllTasksAsync()).ToList();


            // ASSERT (Valideren)

            // 1. Controleer of de Service evenveel items retourneert als de Mock
            Assert.Equal(expectedTasks.Count, result.Count);

            // 2. Controleer of de geretourneerde items de verwachte data bevatten
            Assert.Equal(expectedTasks[0].Title, result[0].Title);
            Assert.Equal(expectedTasks[1].Status, result[1].Status);

            // 3. (Optioneel) Controleer of de methode op de Mock daadwerkelijk is aangeroepen
            mockRepository.Verify(repo => repo.GetAllTasksAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnEmptyList_WhenRepositoryIsEmpty()
        {
            // ARRANGE

            // 1. Mock de ITaskRepository
            var mockRepository = new Mock<ITaskRepository>();

            // 2. Instrueer de Mock: Retourneer een lege lijst
            mockRepository
                .Setup(repo => repo.GetAllTasksAsync())
                .ReturnsAsync(new List<Domain.Task>()); // Retourneer de verwachte lege lijst

            // 3. Initialiseer de Service met de Mock
            var service = new TaskService(mockRepository.Object);


            // ACT

            var result = (await service.GetAllTasksAsync()).ToList();


            // ASSERT

            // De geretourneerde lijst moet leeg zijn
            Assert.NotNull(result);
            Assert.Empty(result);

            // Controleer of de methode op de Mock is aangeroepen
            mockRepository.Verify(repo => repo.GetAllTasksAsync(), Times.Once);
        }
        #endregion

        #region GetTaskByIdAsync Tests
        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            // ARRANGE
            var taskId = Guid.NewGuid();
            var expectedTask = new Domain.Task
            {
                Id = taskId,
                Title = "Mock Task",
                Status = Domain.TaskStatus.ToDo
            };
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync(expectedTask);
            var service = new TaskService(mockRepository.Object);

            // ACT
            var result = await service.GetTaskByIdAsync(taskId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(expectedTask.Id, result!.Id);
            Assert.Equal(expectedTask.Title, result.Title);
            Assert.Equal(expectedTask.Status, result.Status);

            mockRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // ARRANGE
            var taskId = Guid.NewGuid();
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository
                .Setup(repo => repo.GetTaskByIdAsync(taskId))
                .ReturnsAsync((Domain.Task?)null); // Retourneer null voor niet-bestaande taak
            var service = new TaskService(mockRepository.Object);

            // ACT
            var result = await service.GetTaskByIdAsync(taskId);

            // ASSERT
            Assert.Null(result); // Resultaat moet null zijn

            mockRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
        }
        #endregion
    }
}
