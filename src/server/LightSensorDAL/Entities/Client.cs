namespace LightSensorDAL.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string HashedPassword { get; set; }
    }
}
