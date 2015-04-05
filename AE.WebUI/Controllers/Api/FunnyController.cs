using AE.EF.Abstract;
using AE.Funny.Entity;
using AE.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AE.WebUI.Controllers.Api
{
    public class FunnyController : ApiController
    {
        // how many posts to return in response
        private readonly int MAX_POST_COUNT = 10;

        // funny repo
        private IBasicRepository _repo;

        // injection constructor
        public FunnyController(IBasicRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Funny posts (paged)
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<PostModel> Posts([FromUri]int page = 1)
        {
            if (page > 0)
            {
                // paging
                int skip = (page - 1) * MAX_POST_COUNT;
                // posts
                List<PostModel> posts = (from p in _repo.Query<Post>()
                                         orderby p.Created descending
                                         select new PostModel
                                         {
                                             Id = p.PostId,
                                             Title = p.Title,
                                             ImageUrl = p.ImageUrl,
                                             SourceUrl = "http://www.reddit.com/" + p.RedditId
                                         }).Skip(skip).Take(MAX_POST_COUNT).ToList();
                // comments
                foreach (var post in posts)
                {
                    post.Comments = (from c in _repo.Query<Comment>()
                                     where c.Post.PostId == post.Id
                                     select c.Text).ToList();
                }
                return posts;
            }
            return null;
        }
    }
}
