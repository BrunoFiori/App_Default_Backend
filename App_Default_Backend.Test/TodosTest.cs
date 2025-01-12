﻿using App_Default_Backend.Core.Models;
using App_Default_Backend.Core.Services;
using App_Default_Backend.Data.Contract;
using App_Default_Backend.Data.Models;
using AutoMapper;
using Moq;

namespace App_Default_Backend.Test
{
    [TestClass]
    public class TodosTest
    {
        private readonly ServiceTodo _serviceTodo;
        private readonly Mock<IRepositoryTodo> _mockRepositoryTodo;
        private readonly Mock<IMapper> _mockMapper;
        
        public TodosTest()
        {
            _mockRepositoryTodo = new Mock<IRepositoryTodo>();
            _mockMapper = new Mock<IMapper>();
            _serviceTodo = new ServiceTodo(_mockRepositoryTodo.Object, _mockMapper.Object);
        }

        [TestMethod]
        public void ListAllAsync_ShouldReturnListOfOutputTodo()
        {
            
            var inputTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Todo 1", Description = "Description 1" },
                new Todo { Id = 2, Title = "Todo 2", Description = "Description 2" },
                new Todo { Id = 3, Title = "Todo 3", Description = "Description 3" }
            };
            var expectedOutputTodos = new List<OutputTodo>
            {
                new OutputTodo { Id = 1, Title = "Todo 1", Description = "Description 1" },
                new OutputTodo { Id = 2, Title = "Todo 2", Description = "Description 2" },
                new OutputTodo { Id = 3, Title = "Todo 3", Description = "Description 3" }
            };

            _mockRepositoryTodo.Setup(repo => repo.ListAllAsync()).ReturnsAsync(inputTodos);
            _mockMapper.Setup(mapper => mapper.Map<List<OutputTodo>>(inputTodos)).Returns(expectedOutputTodos);

            var result = _serviceTodo.ListAllAsync().Result;

            Assert.AreEqual(expectedOutputTodos, result);
        }

        [TestMethod]
        public void ListAllPagedAsync_ShouldReturnPagedResultOfOutputTodo()
        {
            var inputTodos = new PagedResult<Todo>
            {
                TotalCount = 3,
                PageNumber = 1,
                RecordNumber = 5,
                Items = new List<Todo>
                {
                    new Todo { Id = 1, Title = "Todo 1", Description = "Description 1" },
                    new Todo { Id = 2, Title = "Todo 2", Description = "Description 2" },
                    new Todo { Id = 3, Title = "Todo 3", Description = "Description 3" }
                }
            };
            var expectedOutputTodos = new List<OutputTodo>
            {
                new OutputTodo { Id = 1, Title = "Todo 1", Description = "Description 1" },
                new OutputTodo { Id = 2, Title = "Todo 2", Description = "Description 2" },
                new OutputTodo { Id = 3, Title = "Todo 3", Description = "Description 3" }
            };
            var expectedPagedResult = new PagedResult<OutputTodo>(3, 1, 5, expectedOutputTodos);

            _mockRepositoryTodo.Setup(repo => repo.ListAllPagedAsync<Todo>(It.IsAny<QueryParameters>())).ReturnsAsync(inputTodos);
            _mockMapper.Setup(mapper => mapper.Map<PagedResult<OutputTodo>>(inputTodos)).Returns(expectedPagedResult);

            var result = _serviceTodo.ListAllPagedAsync(It.IsAny<QueryParameters>()).Result;

            Assert.AreEqual(expectedPagedResult, result);
        }
    }
}
