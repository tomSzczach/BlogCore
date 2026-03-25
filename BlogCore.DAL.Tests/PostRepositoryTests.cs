using BlogCore.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.DAL.Tests
{
    [TestClass]
    public class PostRepositoryTests : IntegrationTestBase
    {
        [TestMethod]
        public void AddPost_ValidPost_IncreasesCountByOne()
        {
            var before = _repository.GetAllPosts().Count();

            var post = DataGenerator.GetPostFaker().Generate();

            _repository.AddPost(post);

            var after = _repository.GetAllPosts().Count();

            Assert.AreEqual(before + 1, after);
        }

        [TestMethod]
        public void AddPost_NullAuthor_ThrowsDbUpdateException()
        {
            var post = DataGenerator.GetPostFaker().Generate();
            post.Author = null!;

            Assert.ThrowsException<DbUpdateException>(() => _repository.AddPost(post));
        }

        [TestMethod]
        public void AddPost_NullContent_ThrowsDbUpdateException()
        {
            var post = DataGenerator.GetPostFaker().Generate();
            post.Content = null!;

            Assert.ThrowsException<DbUpdateException>(() => _repository.AddPost(post));
        }

        [TestMethod]
        public void GetAllPosts_EmptyDb_ReturnsZero()
        {
            var posts = _repository.GetAllPosts();

            Assert.IsNotNull(posts);
            Assert.AreEqual(0, posts.Count());
        }

        [TestMethod]
        public void AddPost_LongContent_SavesCorrectly()
        {
            var longContent = new Bogus.Faker().Lorem.Paragraphs(5);

            var post = DataGenerator.GetPostFaker()
                .RuleFor(p => p.Content, _ => longContent)
                .Generate();

            _repository.AddPost(post);

            var savedPost = _repository.GetAllPosts().Single();

            Assert.AreEqual(longContent, savedPost.Content);
        }

        [TestMethod]
        public void AddPost_SpecialCharactersInAuthor_SavesCorrectly()
        {
            const string specialAuthor = "Zażółć Gęślą Jaźń 123!";

            var post = DataGenerator.GetPostFaker()
                .RuleFor(p => p.Author, _ => specialAuthor)
                .Generate();

            _repository.AddPost(post);

            var savedPost = _repository.GetAllPosts().Single();

            Assert.AreEqual(specialAuthor, savedPost.Author);
        }
    }
}