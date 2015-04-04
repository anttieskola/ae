using AE.EF.Abstract;
using AE.News.Entity;
using AE.WebUI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AE.WebUI.Controllers.Api
{
    public class NewsController : ApiController
    {
        // how many articles to write in response in one go
        private readonly int MAX_ARTICLE_COUNT = 10;

        // news repository
        private IBasicRepository _repo;

        // injection constructor
        public NewsController(IBasicRepository repository)
        {
            _repo = repository;
        }

        /// <summary>
        /// Tag list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<TagModel> Tags()
        {
            var tags = from t in _repo.Query<Tag>()
                       select new TagModel
                       {
                           Id = t.TagId,
                           Name = t.Name
                       };
            return tags.ToList();
        }

        /// <summary>
        /// Single article
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ArticleModel Article([FromUri]int id)
        {
            var article = (from a in _repo.Query<Article>()
                           where a.ArticleId == id
                           select new ArticleModel
                           {
                               Id = a.ArticleId,
                               Title = a.Title,
                               Description = a.Description,
                               Content = a.Content,
                               ImageUrl = a.ImageUrl,
                               SourceUrl = a.SourceUrl,
                               Date = a.Date
                           }).FirstOrDefault();
            if (article != null)
            {
                return article as ArticleModel;
            }
            return null;
        }

        /// <summary>
        /// All articles
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ArticleModel> Articles([FromUri]int page = 1)
        {
            if (page > 0)
            {
                // skip/page
                int skip = (page - 1) * MAX_ARTICLE_COUNT;
                var articles = (from a in _repo.Query<Article>()
                                orderby a.Date descending
                                select new ArticleModel
                                {
                                    Id = a.ArticleId,
                                    Title = a.Title,
                                    Description = a.Description,
                                    Content = a.Content,
                                    ImageUrl = a.ImageUrl,
                                    SourceUrl = a.SourceUrl,
                                    Date = a.Date
                                }).Skip(skip).Take(MAX_ARTICLE_COUNT);
                return articles;
            }
            return null;
        }

        /// <summary>
        /// Articles with specific tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ArticleModel> Articles([FromUri]int tag, [FromUri]int page = 1)
        {
            // check tag does exist before query
            Tag check = _repo.Query<Tag>().Where(x => x.TagId == tag).FirstOrDefault();
            if (check != null && page > 0)
            {
                // skip/page
                int skip = (page - 1) * MAX_ARTICLE_COUNT;
                // get articles and generate models
                var articles = (from a in _repo.Query<Article>()
                                where a.Tags.Any(x => x.TagId == check.TagId)
                                orderby a.Date descending
                                select new ArticleModel
                                {
                                    Id = a.ArticleId,
                                    Title = a.Title,
                                    Description = a.Description,
                                    Content = a.Content,
                                    ImageUrl = a.ImageUrl,
                                    SourceUrl = a.SourceUrl,
                                    Date = a.Date
                                }).Skip(skip).Take(MAX_ARTICLE_COUNT);
                return articles;
            }
            return null;
        }
    }
}
