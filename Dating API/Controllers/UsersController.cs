using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.DTOs;
using DatingAPI.Helpers;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DatingAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        [HttpGet("GetUsers")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 200)]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 200)]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 400)]
        public async Task<IActionResult> GetUsers(UserParams userParams)
        {
            var users = await _datingRepository.GetUsers(userParams);

            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            // return pagination in the response header
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersDTO);
        }


        [HttpGet("GetUserByID" + "/{userID:int}")]
        [ProducesResponseType(typeof(UserDetailDTO), 200)]
        [ProducesResponseType(typeof(UserDetailDTO), 400)]
        [ProducesResponseType(typeof(UserDetailDTO), 404)]
        public async Task<IActionResult> GetUserByID(int userID)
        {
            if (userID == 0)
            {
                return new BadRequestObjectResult(new { ErrorMessage = "Cannot be 0" });
            }

            var user = await _datingRepository.GetUser(userID);

            if (user == null)
            {
                return new NotFoundObjectResult(new { ErrorMessage = "No user found" });
            }

            var validPhotos = user.Photos.Where(p => p.Deleted == null).ToList();

            var userWithValidPhotos = user;

            userWithValidPhotos.Photos = user.Photos.Where(p => validPhotos.Contains(p)).ToList();

            var userDetaiedDTO = _mapper.Map<UserDetailDTO>(userWithValidPhotos);
            return Ok(userDetaiedDTO);
        }

        [HttpPut("{id:int}"+ "/edit-profile")]
        [ProducesResponseType(typeof(UserDetailDTO), 201)]
        [ProducesResponseType(typeof(UserDetailDTO), 400)]
        [ProducesResponseType(typeof(UserDetailDTO), 404)]
        public async Task<IActionResult> EditUserProfile([FromBody] UserDetailDTO userDetailDTO, int id)
        {
            if (id <= 0 || (id != userDetailDTO.Id))
            {
                return new BadRequestObjectResult(new Error { ErrorMessage = "Member Id cannot be null" });
            }

            var userToEdit = _mapper.Map<User>(userDetailDTO);

            var userDB = await _datingRepository.GetUser(userToEdit.Id);

            if (userDB == null)
            {
                return new BadRequestObjectResult(new Error { ErrorMessage = "User doesn't exist" });
            }

            userDB.City = userToEdit.City;
            userDB.Country = userToEdit.Country;
            userDB.Interests = userToEdit.Interests;
            userDB.Introduction = userToEdit.Introduction;
            userDB.LookingFor = userToEdit.LookingFor;
                
            var editedUser = await _datingRepository.Update(userDB);

            var userToEditDTO = _mapper.Map<UserDetailDTO>(editedUser);

            return Ok(userToEditDTO);
        }

        [HttpPost("{likerUserID:int}/likes/{likeeUserID:int}")]
        [ProducesResponseType(typeof(UserDetailDTO), 201)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SaveLike(int likerUserID, int likeeUserID)
        {
            if (likerUserID < 0  || likeeUserID < 0)
            {
                return new BadRequestObjectResult(new Error { ErrorMessage = "Liker or Likee user id is invalid" });
            }

            // get users
            var liker = _datingRepository.GetUser(likerUserID);
            var likee = _datingRepository.GetUser(likeeUserID);

            if (liker == null || likee == null)
            {
                return new NotFoundObjectResult(new Error { ErrorMessage = "Liker or Likee user does not exist" });
            }

            var likeToSave = new Like
            {
                LikerUserID = likerUserID,
                LikeeUserID = likeeUserID,
                Date = DateTime.Now
            };
            // save like 
            var response = await _datingRepository.SaveLike(likeToSave);

            var responseDTO = _mapper.Map<LikeDTO>(response);

            return new ObjectResult(responseDTO);
        }

        /// <summary>
        // Get list of users liked by user
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userID:int}/likers")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 200)]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 400)]
        public async Task<IActionResult> GetLikedUsers(int userID)
        {
            var users = await _datingRepository.GetLikers(userID);

            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(usersDTO);
        }

        /// <summary>
        /// Get list of User that liked user
        /// </summary>
        /// <param name="likeeUserID"></param>
        /// <returns></returns>
        [HttpGet("{userID:int}/likees")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 200)]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 400)]
        public async Task<IActionResult> GetLikeeUsers(int userID)
        {
            var users = await _datingRepository.GetLikees(userID);

            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(usersDTO);
        }

    }
}