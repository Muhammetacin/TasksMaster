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

        #region CreateTaskAsync Tests
        [Fact]
        public async Task CreateTaskAsync_ShouldCreateAndReturnTask()
        {
            // ARRANGE
            var newTask = new Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "New Mock Task",
                Description = "This is a new mock task.",
                Status = Domain.TaskStatus.ToDo
            };
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository
                .Setup(repo => repo.CreateTaskAsync(newTask))
                .ReturnsAsync(newTask);
            var service = new TaskService(mockRepository.Object);

            // ACT
            var result = await service.CreateTaskAsync(newTask);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(newTask.Id, result.Id);
            Assert.Equal(newTask.Title, result.Title);
            Assert.Equal(newTask.Description, result.Description);
            Assert.Equal(newTask.Status, result.Status);

            mockRepository.Verify(repo => repo.CreateTaskAsync(newTask), Times.Once);
        }
        #endregion

        #region UpdateTaskAsync Tests
        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateAndReturnTask_WhenTaskExists()
        {
            // ARRANGE
            var existingTask = new Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Existing Task",
                Description = "This is an existing task.",
                Status = Domain.TaskStatus.ToDo
            };
            var updatedTask = new Domain.Task
            {
                Id = existingTask.Id,
                Title = "Updated Task",
                Description = "This task has been updated.",
                Status = Domain.TaskStatus.InProgress
            };
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository
                .Setup(repo => repo.GetTaskByIdAsync(existingTask.Id))
                .ReturnsAsync(existingTask);
            // Accept any Task instance and return the instance passed to UpdateTaskAsync
            mockRepository
                .Setup(repo => repo.UpdateTaskAsync(It.IsAny<Domain.Task>()))
                .ReturnsAsync((Domain.Task t) => t);
            var service = new TaskService(mockRepository.Object);

            // ACT
            var result = await service.UpdateTaskAsync(updatedTask);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(updatedTask.Id, result.Id);
            Assert.Equal(updatedTask.Title, result.Title);
            Assert.Equal(updatedTask.Description, result.Description);
            Assert.Equal(updatedTask.Status, result.Status);

            mockRepository.Verify(repo => repo.GetTaskByIdAsync(existingTask.Id), Times.Once);
            mockRepository.Verify(repo => repo.UpdateTaskAsync(It.Is<Domain.Task>(t => t.Id == existingTask.Id)), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrowException_WhenTaskDoesNotExist()
        {
            // ARRANGE
            var nonExistentTask = new Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Non-Existent Task",
                Description = "This task does not exist.",
                Status = Domain.TaskStatus.ToDo
            };
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository
                .Setup(repo => repo.GetTaskByIdAsync(nonExistentTask.Id))
                .ReturnsAsync((Domain.Task?)null); // Retourneer null voor niet-bestaande taak
            var service = new TaskService(mockRepository.Object);

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.UpdateTaskAsync(nonExistentTask);
            });

            mockRepository.Verify(repo => repo.GetTaskByIdAsync(nonExistentTask.Id), Times.Once);
        }
        #endregion

        #region DeleteTaskAsync Tests
        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
        {
            // ARRANGE
            var taskId = Guid.NewGuid();
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository
                .Setup(repo => repo.DeleteTaskAsync(taskId))
                .Returns(Task.CompletedTask);
            var service = new TaskService(mockRepository.Object);

            // ACT
            await service.DeleteTaskAsync(taskId);

            // ASSERT
            mockRepository.Verify(repo => repo.DeleteTaskAsync(taskId), Times.Once);
        }
        #endregion
    }
}
