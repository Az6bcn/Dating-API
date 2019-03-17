﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingAPI.Data;
using DatingAPI.DTOs;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 400)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepository.GetUsers();

            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

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

            var userDetaiedDTO = _mapper.Map<UserDetailDTO>(user);
            return Ok(userDetaiedDTO);
        }

        [HttpPost("Member/" + "{id: int}"+ "/edit-profile")]
        [ProducesResponseType(typeof(UserDetailDTO), 201)]
        [ProducesResponseType(typeof(UserDetailDTO), 400)]
        [ProducesResponseType(typeof(UserDetailDTO), 404)]
        public async Task<IActionResult> EditUserProfile([FromBody] UserDetailDTO userDetailDTO, int id)
        {
            if (id < 0 || (id != userDetailDTO.Id))
            {
                return new BadRequestObjectResult(new Error { ErrorMessage = "Member Id cannot be null" });
            }

            // check if user exists
            var userDB = await _datingRepository.GetUser(userDetailDTO.Id);

            if (userDB == null)
            {
                return new NotFoundObjectResult(new Error { ErrorMessage = "User not found" });
            }
            
            var userToEdit = _mapper.Map<User>(userDetailDTO);

            //userDB.Interests = userToEdit.Interests;
            //userDB.Introduction = userToEdit.Introduction;
            //userDB.KnownAs = userToEdit.KnownAs;
            //userDB.LookingFor = userToEdit.LookingFor;
            //userDB.Username = userToEdit.Username;
            //userDB.City = userToEdit.City;
            //userDB.Country = userToEdit.Country;
            //userDB.DateOfBirth = userToEdit.DateOfBirth;
            //userDB.Gender = userToEdit.Gender;





            return Ok();
        }
    }
}