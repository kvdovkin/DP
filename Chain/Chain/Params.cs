
namespace Chain
{
    public class Constants
    {
        public const string Exception = "Exception";
        public const string flagIn = "true";
        public const string Host = "localhost";
    }
}

struct Params
{
    public int ListeningPort { get; set; }
    public string NextHost { get; set; }
    public int NextPort { get; set; }
}
