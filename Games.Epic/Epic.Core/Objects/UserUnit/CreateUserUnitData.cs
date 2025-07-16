using System;

namespace Epic.Core.Objects.UserUnit
{
    public class CreateUserUnitData
    {
        public Guid UserId { get; set; }
        public Guid UnitTypeId { get; set; }
        public int Amount { get; set; }
    }
}