namespace ToDo.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string? File {  get; set; }

        public DateTime DeadLine { get; set; }
        
    }
}
