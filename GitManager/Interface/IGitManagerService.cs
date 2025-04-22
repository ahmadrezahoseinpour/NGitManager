
namespace GitManager.Interface
{
    public interface IGitManagerService
    {

        public IIssueService Issue { get; }
        public IEpicService Epic { get; }
        public IUserService User{ get; }
    }
}
