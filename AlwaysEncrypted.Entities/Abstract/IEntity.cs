using System;

namespace AlwaysEncrypted.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
        public DateTime Created { get; set; }
    }
}
