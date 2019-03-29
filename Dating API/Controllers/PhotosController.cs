﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatingAPI.DTOs;
using DatingAPI.Model;
using DatingAPI.Helpers;
using DatingAPI.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PhotosController : Controller
    {
        private ICloudinaryHelper _cloudinaryHelper;
        private IDatingRepository _datingRepository;

        public PhotosController(ICloudinaryHelper cloudinaryHelper, IDatingRepository datingRepository)
        {
            _cloudinaryHelper = cloudinaryHelper;
            _datingRepository = datingRepository;
        }


        // GET: api/photos
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/photos/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/photos
        [HttpPost]
        [ProducesResponseType(typeof(PhotoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPhotoForUser([FromBody] PhotoForCreationDTO photoForCreationDTO)
        {
            if (photoForCreationDTO.UserID < 0) return new BadRequestObjectResult(new Error { ErrorMessage = "invalid UserID" });

            // check File is not null
            if(photoForCreationDTO.File.Length <= 0) return new BadRequestObjectResult(new Error { ErrorMessage = "invalid file" });

            //call cloudinary to save file
            var cloudinaryResponse = await _cloudinaryHelper.UploadPhotoToCloudinary(photoForCreationDTO.File.FileName, photoForCreationDTO.File);

            //isFirstPhoto
            var isFirstPhoto = await _datingRepository.IsThereMainPhotoForUser(photoForCreationDTO.UserID);
            var isMain = (isFirstPhoto) ? true : false;


            // save response properties to photos table in DB
            var photo = _cloudinaryHelper.ParsePhoto(cloudinaryResponse.PublicId, cloudinaryResponse.Uri.ToString(), photoForCreationDTO.UserID, 
                        photoForCreationDTO.Description, photoForCreationDTO.DateAdded, isMain);
            await _datingRepository.SavePhoto(photo);
            
            //return photo

            return new OkObjectResult(new PhotoDTO());
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
