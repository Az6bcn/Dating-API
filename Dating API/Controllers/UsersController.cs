using System;
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
        private readonly DataDbContext _dataDbContext;
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(DataDbContext dataDbContext, IDatingRepository datingRepository, IMapper mapper)
        {
            _dataDbContext = dataDbContext;
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        [HttpGet("GetUsers")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(typeof(IEnumerable<User>), 400)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepository.GetUsers();

           var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

           return Ok(usersDTO);
        }


        [HttpGet("GetUserByID" + "/{userID:int}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(User), 400)]
        [ProducesResponseType(typeof(User), 404)]
        public async Task<IActionResult> GetUserByID(int userID)
        {
            if(userID == 0)
            {
                return new BadRequestObjectResult(new { ErrorMessage = "Cannot be 0" });
            }

            var user = await _datingRepository.GetUser(userID);

            if(user == null)
            {
                return new NotFoundObjectResult(new { ErrorMessage = "No user found" });
            }

            var userDetaiedDTO = _mapper.Map<UserDetailDTO>(user);
            return Ok(userDetaiedDTO);
        }
    }
}