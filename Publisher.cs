using System.Collections.Generic;

namespace BookStoreSqlLoader
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public List<Book> Books { get; set; }
    }
}
