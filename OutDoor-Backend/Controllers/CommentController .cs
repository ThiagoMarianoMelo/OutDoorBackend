using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutDoor_Models;
using OutDoor_Models.Models;
using OutDoor_Models.Models.UtilModel;
using OutDoor_Models.Requests.User;
using OutDoor_Models.Services;
using OutDoor_Repository;
using System.Net;

namespace OutDoor_Backend.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService CommentService;
        private readonly IPostService PostService;
        private IRabbitMQService RabbitMQService;

        public CommentController(ICommentService commentService, IRabbitMQService rabbitMQService, IPostService postService)
        {
            CommentService = commentService;
            RabbitMQService = rabbitMQService;
            PostService = postService;
        }

        [HttpPost]
        [Route("/api/comment")]
        public async Task<ActionResult> Create([FromBody] CreateCommentRequestModel request)
        {
            var result = await CommentService.postNewComment(request);

            try //Try send Notification to Queue
            {
                var PostInformation = await PostService.getPostInfoById(request.PostId);
                RabbitMQService.SendNotificationToQueue<NotificationModel>(new NotificationModel()
                {
                    PostName = PostInformation.Title,
                    UserEmail = PostInformation.User.Email,
                    UserName = PostInformation.User.Name
                });

                await PostService.AddNewRating(request.PostId, request.rating);
            }
            catch (Exception ex)
            {
                throw new ControllerException("Um erro inesperado ocorreu ao tentar gerar notificação ao usuário")
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Source = nameof(CommentController)
                };
            }


            return CreatedAtAction(nameof(Create), result);
        }
            
    }
}
