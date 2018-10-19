using System;

namespace com.sbp.instantacct.entity
{
    public class EntityBase
    {
        public EntityBase()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            RecordStatus = RecordStatus.Active;
        }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public RecordStatus RecordStatus { get; set; }         
        public string Slug { get; set; }
    }
}
