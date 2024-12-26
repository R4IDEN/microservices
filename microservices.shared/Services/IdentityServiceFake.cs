using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace microservices.shared.Services
{
    public class IdentityServiceFake : IIdentityService
    {
        public Guid GetUserId => Guid.Parse("a2a84ffc-e498-4f12-83d2-26774a65071e");

        public string UserName => "Memolinin ızdırabı";
    }
}
