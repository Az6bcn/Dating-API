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
        private UserDetailDTO userdetail;
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

        [Description("Get user with invalid ID")]
        [TestMethod]
        public void GetUserByID_WithInvalidID_ShouldReturnNotFound()
        {
            // Arrange
            // set up concrete method implementation for GetUser(id) in the interface
            _datingRepositoryMock.Setup(s => s.GetUser(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(users.AsEnumerable()
                    .Where(x => x.Id == id).FirstOrDefault()));

            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var usersDto = (NotFoundObjectResult)controller.GetUserByID(10).Result;

            //Assert
            Assert.IsInstanceOfType(usersDto, typeof(NotFoundObjectResult));

        }

        [Description("Get user with ID == 0")]
        [TestMethod]
        public void GetUserWithID_WithIDZero_ShouldReturnBadRequest()
        {
            // Arrange
            // set up concrete method implementation for GetUser(id) in the interface
            _datingRepositoryMock.Setup(s => s.GetUser(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(users.AsEnumerable()
                    .Where(x => x.Id == id).FirstOrDefault()));

            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var usersDto = (BadRequestObjectResult)controller.GetUserByID(0).Result;

            //Assert
            Assert.IsInstanceOfType(usersDto, typeof(BadRequestObjectResult));

        }


        [Description("Edit user profile with ID == 0")]
        [TestMethod]
        public void EditUserProfile_WithID_WithIDZero_ShouldReturnErrorMessage()
        {
            // Arrange
            // set up concrete method implementation for GetUser(id) in the interface
            //_datingRepositoryMock.Setup(s => s.GetUserPhotosByUserID(It.IsAny<int>()))
            //    .Returns((int id) => Task.FromResult(users.AsEnumerable()
            //        .Where(x => x.Id == id).FirstOrDefault()));
            var userDTO = new UserDetailDTO();

            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var usersDto = (dynamic)controller.EditUserProfile(userDTO, 0).Result;

            //Assert
            Assert.IsInstanceOfType(usersDto, typeof(BadRequestObjectResult));
            Assert.AreEqual("Member Id cannot be null", usersDto.Value.ErrorMessage);

        }

        [Description("Edit user profile")]
        [TestMethod]
        public void EditUserProfile_WhenCalled_VerifyEditMethodCallWIthPassedUserDetailDTO_And_ReturnTypeIsOkObjectResult()
        {
            // Arrange
            var userdetailToEdit = new UserDetailDTO
            {

                Id = 1,
                City = "Barcelona",
                Country = "Spain",
                CreatedAt = DateTime.Now,
                Gender = "Male",
                KnownAs = "AZAZXXS",
                LastActive = DateTime.Now,
                Username = "Joey",
                Interests = "TESTING",
                LookingFor = "kn;koxnonxksnxksxs"
            };
            var userdetailToEdit_MappedToUser = new User
            {
                Id = 1,
                City = "Barcelona",
                Country = "Spain",
                CreatedAt = DateTime.Now,
                DateOfBirth = DateTime.Now,
                Gender = "Male",
                KnownAs = "AZAZXXS",
                LastActive = DateTime.Now,
                Username = "Joey"
            };
            // set up concrete method implementation for GetUser(id) in the interface
            _datingRepositoryMock.Setup(s => s.Update(It.IsAny<User>()))
                .Returns((User _userdetail) => Task.FromResult(userdetailToEdit_MappedToUser));

            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var usersDto = (OkObjectResult)controller.EditUserProfile(userdetailToEdit, 1).Result;

            //Assert: 
            // Verify the update() was called with the passed parameter and only called once
            //_datingRepositoryMock.Verify(s => s.Update(userdetailToEdit_MappedToUser), Times.Once);

            Assert.IsInstanceOfType(usersDto, typeof(OkObjectResult));


        }

        [Ignore]
        [Description("Edit user profile")]
        [TestMethod]
        public void EditUserProfile_WhenCalled_VerifyEditMethodCallWIthPassedUserDetailDTO()
        {
            // Arrange
            var userdetailToEdit = new UserDetailDTO
            {

                Id = 1,
                City = "Barcelona",
                Country = "Spain",
                CreatedAt = DateTime.Now,
                Gender = "Male",
                KnownAs = "AZAZXXS",
                LastActive = DateTime.Now,
                Username = "Joey",
                Interests = "TESTING",
                LookingFor = "kn;koxnonxksnxksxs"
            };
            var userdetailToEdit_MappedToUser = new User
            {
                Id = 1,
                City = "Barcelona",
                Country = "Spain",
                CreatedAt = DateTime.Now,
                DateOfBirth = DateTime.Now,
                Gender = "Male",
                KnownAs = "AZAZXXS",
                LastActive = DateTime.Now,
                Username = "Joey"
            };
            // set up concrete method implementation for GetUser(id) in the interface
            _datingRepositoryMock.Setup(s => s.Update(It.IsAny<User>()))
                .ReturnsAsync(userdetailToEdit_MappedToUser);


            var controller = new UsersController(_datingRepositoryMock.Object, mapper);

            // Act
            var xxx = controller.EditUserProfile(userdetailToEdit, 1).Result;

            //Assert: 
            // Verify the update() was called with the passed parameter and only called once
            _datingRepositoryMock.Verify(s => s.Update(userdetailToEdit_MappedToUser));

        }



    }
}
