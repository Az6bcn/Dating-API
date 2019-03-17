using AutoMapper;
using AutoMapper.Configuration;
using DatingAPI.Controllers;
using DatingAPI.Data;
using DatingAPI.DTOs;
using DatingAPI.DTOs.Profiles;
using DatingAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingAPI_Test.Controller
{
    [TestClass]
    public class UsersControllerTest
    {
        private Mock<IDatingRepository> _datingRepositoryMock;
        private List<User> users;
        private IMapper mapper;

        public UsersControllerTest()
        {
           // users = new List<UserDTO>();
        }

        [TestInitialize]
        public void init()
        {
            _datingRepositoryMock = new Mock<IDatingRepository>();
            

            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile<UserProfile>();
            });

            mapper = config.CreateMapper();

            users = new List<User>
            {
                new User {
                    Id = 1,
                    City = "Barcelona",
                    Country = "Spain",
                    CreatedAt = DateTime.Now,
                    DateOfBirth = DateTime.Now,
                    Gender = "Male",
                    KnownAs = "AZAZXXS",
                    LastActive = DateTime.Now,
                    Username = "Joey"
                },
                new User {
                    Id = 2,
                    City = "London",
                    Country = "Uk",
                    CreatedAt = DateTime.Now,
                    DateOfBirth = DateTime.Now,
                    Gender = "Male",
                    KnownAs = "ssdss",
                    LastActive = DateTime.Now,
                    Username = "azeez"
                },
                new User {
                    Id = 3,
                    City = "Barcelona",
                    Country = "Spain",
                    CreatedAt = DateTime.Now,
                    DateOfBirth = DateTime.Now,
                    Gender = "Male",
                    KnownAs = "AZAZXXS",
                    LastActive = DateTime.Now,
                    Username = "Joey"
                }
            };
        }

        [Description("Should return a list of all users, in dto model")]
        [TestMethod]
        public void GetAllUsers_WhenCalled_ShouldReturnListOfUsersDTOAsync()
        {
            // Arrange
            // set up concrete method implementation for GetUsers() in the interface
            _datingRepositoryMock.Setup(s => s.GetUsers()).Returns(Task.FromResult(users.AsEnumerable()));

            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var usersDtoList =  (OkObjectResult)controller.GetUsers().Result;
            var count = usersDtoList.Value as List<UserDTO>;

            //Assert
            Assert.IsInstanceOfType(usersDtoList.Value, typeof(IEnumerable<UserDTO>));
            Assert.AreEqual(3, count.Count);

        }

        [Description("Should return a list of all users, in dto model")]
        [TestMethod]
        public void Testo1()
        {
            Assert.AreEqual("1", "1");
        }

        [Description("Should return user of the ID, in dto model")]
        [TestMethod]
        public void GetUserById_WhenCalled_ShouldReturnUserWithID()
        {
            // Arrange
            // set up concrete method implementation for GetUser(id) in the interface
            _datingRepositoryMock.Setup(s => s.GetUser(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(users.AsEnumerable()
                    .Where(x => x.Id == id ).First()));

            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var usersDto = (OkObjectResult)controller.GetUserByID(2).Result;
            var count = usersDto.Value as UserDetailDTO;

            //Assert
            Assert.IsInstanceOfType(usersDto.Value, typeof(UserDetailDTO));
            Assert.AreEqual(2, count.Id);

        }
    }
}
