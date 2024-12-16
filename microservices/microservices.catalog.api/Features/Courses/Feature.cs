namespace microservices.catalog.api.Features.Courses
{
    public class Feature
    {
        public int Duration { get; set; }
        public float Rating { get; set; }
        public string TeacherName { get; set; } = default!;
    }
}
