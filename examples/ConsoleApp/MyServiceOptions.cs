using OptionExpressions;

namespace ConsoleApp
{
    [EnableExpressions]
    public class MyServiceOptions
    {
        public string ServiceName { get; set; }

        public bool Enabled { get; set; }

        public int Value { get; set; }
    }
}