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
    [Route("api/Messages/{userID}")]
    public class MessagesController : Controller
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get user inbox messages
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpGet("Inbox")]
        [ProducesResponseType(typeof(IEnumerable<MessageInOutBoxDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<MessageInOutBoxDTO>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMessageForUserInbox(int UserID)
        {
            if (UserID < 0) { return new BadRequestObjectResult(new Error { ErrorMessage = "invalid userID" }); };

            var user = _datingRepository.GetByID<User>(UserID);

            if (user == null) { return new NotFoundObjectResult(new Error { ErrorMessage = "User not found" }); };

            // get the inbox for the user: messages where user is the recipient
            var messages = await _datingRepository.GetMessageForUserInbox(UserID);

            var responseDTO = _mapper.Map<IEnumerable<MessageInOutBoxDTO>>(messages);

            return Ok(responseDTO);
        }

        /// <summary>
        /// Get user outbox messages
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpGet("Outbox")]
        [ProducesResponseType(typeof(IEnumerable<MessageInOutBoxDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<MessageInOutBoxDTO>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMessageForUserOutbox(int UserID)
        {
            if (UserID < 0) { return new BadRequestObjectResult(new Error { ErrorMessage = "invalid userID" }); };

            var user = _datingRepository.GetByID<User>(UserID);

            if (user == null) { return new NotFoundObjectResult(new Error { ErrorMessage = "User not found" }); };

            // get the inbox for the user: messages where user is the recipient
            var messages = await _datingRepository.GetMessageForUserOutbox(UserID);

            var responseDTO = _mapper.Map<IEnumerable<MessageInOutBoxDTO>>(messages);

            return Ok(responseDTO);
        }

        /// <summary>
        /// Send/Create a new message
        /// </summary>
        /// <param name="messagesDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<MessagesDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessagesDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessagesDTO), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> CreateMessage([FromBody] MessagesDTO messagesDTO) {

            if (messagesDTO == null) { return BadRequest(new Error { ErrorMessage = "null message" }); }

            var sender = _datingRepository.GetByID<User>(messagesDTO.SenderID);
            if (sender == null) { return NotFound(new Error { ErrorMessage = "Sender not found" }); }

            var recipient = _datingRepository.GetByID<User>(messagesDTO.SenderID);
            if (recipient == null) { return NotFound(new Error { ErrorMessage = "Recipient not found" }); }

            var message = new Message
            {
                SenderID = messagesDTO.SenderID,
                RecipientID = messagesDTO.RecipientID,
                MessageSent = messagesDTO.SentDate,
                Content = messagesDTO.Content,
                IsRead = false,
                RecipientDeleted = false,
                SenderDeleted = false,
            };
            _datingRepository.Add(message);
            var response = await _datingRepository.SaveAll();

            if (!response) { return BadRequest(new Error { ErrorMessage = "Could not save message" }); }

            return Ok(messagesDTO);
        }

        /// <summary>
        /// Get a message
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessage(int messageID) {
            if (messageID < 0) { return BadRequest(new Error { ErrorMessage = "Invalid MessageID" }); }

            var message = await _datingRepository.GetUserMessage(messageID);

            if (message == null) { return NotFound(new Error { ErrorMessage = "Message not found" }); }

            var reposneDTO = _mapper.Map<MessageToReturnDTO>(message);
            return Ok(reposneDTO);
        }

        //get list of messages for the user.
        //[HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<MessageToReturnDTO>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(MessagesDTO), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(MessagesDTO), StatusCodes.Status404NotFound)]
        //public IActionResult GetUserMessagesForUser(int userID, int messageID)
        //{
        //    if (messageID < 0) { return BadRequest(new Error { ErrorMessage = "Invalid MessageID" }); }

        //    var message = _datingRepository.GetMessagesForUser(userID, messageID);

        //    if (message == null) { return NotFound(new Error { ErrorMessage = "Message not found" }); }

        //    var dto = _mapper.Map<MessageToReturnDTO>(message);

        //    return Ok(dto);
        //}

        /// <summary>
        /// Gets converstation/messages between 2 users.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="recipientID"></param>
        /// <returns></returns>
        [HttpGet("thread")]
        [ProducesResponseType(typeof(IEnumerable<MessageToReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessageThread(int userID, int recipientID)
        {
            if (userID < 0 || recipientID < 0) { return BadRequest(new Error { ErrorMessage = "Invalid User or RecipientID" }); }

            var sender = _datingRepository.GetByID<User>(userID);
            if (sender == null) { return NotFound(new Error { ErrorMessage = "User not found" }); }

            var recipient = _datingRepository.GetByID<User>(recipientID);
            if (recipient == null) { return NotFound(new Error { ErrorMessage = "Recipient not found" }); }

            var response = await _datingRepository.GetMessageThread(userID, recipientID);

            var responseDTO = _mapper.Map<IEnumerable<MessageToReturnDTO>>(response);

            return Ok(responseDTO);
        }


        /// <summary>
        /// Deletes a message
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        [HttpDelete()]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageToReturnDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMessage(int messageID)
        {
            if (messageID < 0) { return BadRequest(new Error { ErrorMessage = "Invalid MessageID" }); }

            var message = await _datingRepository.GetUserMessage(messageID);

            if (message == null) { return NotFound(new Error { ErrorMessage = "Message not found" }); }

            return NoContent();
        }

    }
}