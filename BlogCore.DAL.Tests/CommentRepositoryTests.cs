using BlogCore.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.DAL.Tests
{
    [TestClass]
    public class CommentRepositoryTests : IntegrationTestBase
    {
        [TestMethod]
        public void GetCommentsByPostId_ReturnsExactlyThreeCommentsForPost()
        {
            var post = DataGenerator.GetPostFaker().Generate();
            _repository.AddPost(post);

            var comments = DataGenerator.GetCommentFaker(post.Id).Generate(3);
            foreach (var comment in comments)
            {
                _repository.AddComment(comment);
            }

            var result = _repository.GetCommentsByPostId(post.Id).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(c => c.PostId == post.Id));
        }

        [TestMethod]
        public void AddComment_ValidData_IncreasesCountForPost()
        {
            var post = DataGenerator.GetPostFaker().Generate();
            _repository.AddPost(post);

            var comment = DataGenerator.GetCommentFaker(post.Id).Generate();
            _repository.AddComment(comment);

            var result = _repository.GetCommentsByPostId(post.Id).ToList();

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetCommentsByPostId_NonExistentPost_ReturnsEmpty()
        {
            var result = _repository.GetCommentsByPostId(999999);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void AddComment_OrphanComment_ThrowsException()
        {
            var comment = DataGenerator.GetCommentFaker(123456).Generate();

            Assert.ThrowsException<DbUpdateException>(() => _repository.AddComment(comment));
        }

        [TestMethod]
        public void MultipleComments_DifferentPosts_ReturnsOnlyCorrectOnes()
        {
            var post1 = DataGenerator.GetPostFaker().Generate();
            var post2 = DataGenerator.GetPostFaker().Generate();

            _repository.AddPost(post1);
            _repository.AddPost(post2);

            var commentsForPost1 = DataGenerator.GetCommentFaker(post1.Id).Generate(5);
            var commentsForPost2 = DataGenerator.GetCommentFaker(post2.Id).Generate(2);

            foreach (var comment in commentsForPost1)
            {
                _repository.AddComment(comment);
            }

            foreach (var comment in commentsForPost2)
            {
                _repository.AddComment(comment);
            }

            var result = _repository.GetCommentsByPostId(post1.Id).ToList();

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(c => c.PostId == post1.Id));
        }

        [TestMethod]
        public void AddComment_NullContent_ThrowsDbUpdateException()
        {
            var post = DataGenerator.GetPostFaker().Generate();
            _repository.AddPost(post);

            var comment = DataGenerator.GetCommentFaker(post.Id).Generate();
            comment.Content = null!;

            Assert.ThrowsException<DbUpdateException>(() => _repository.AddComment(comment));
        }

        [TestMethod]
        public void DeletePost_CascadeDeleteComments()
        {
            var post = DataGenerator.GetPostFaker().Generate();
            _repository.AddPost(post);

            var comments = DataGenerator.GetCommentFaker(post.Id).Generate(3);
            foreach (var comment in comments)
            {
                _repository.AddComment(comment);
            }

            var beforeDelete = _repository.GetCommentsByPostId(post.Id).Count();
            _repository.DeletePost(post.Id);
            var afterDelete = _repository.GetCommentsByPostId(post.Id).Count();

            Assert.AreEqual(3, beforeDelete);
            Assert.AreEqual(0, afterDelete);
        }
    }
}